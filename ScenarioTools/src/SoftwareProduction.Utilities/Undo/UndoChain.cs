using System;
using System.Collections;

namespace SoftwareProductions.Utilities.Undo
{

	public delegate void UndoActionEventHandler(object sender, UndoActionEventArgs e);

	/// <summary>
	/// This class implements a circular buffer for <see cref="IUndoItem"/>s.
	/// It allows undo and redo operations to be performed on <see cref="IUndoItem"/>s 
	/// in the correct order. 
	/// </summary>
	/// <remarks>
	/// <p>
	/// To undo and redo changes, record the changes as they occure by adding 
	/// <see cref="IUndoItem"/>s to the UndoChain that represent the changes.
	/// To undo and redo the changes, use the Undo() and Redo() methods on the 
	/// <see cref="UndoChain"/>, which will identify the correct <see cref="IUndoItem"/>
	/// and call its Undo() or Redo() method.
	/// </p>
	/// <p>
	/// Each <see cref="IUndoItem"/> is responsible for undoing and redoing the 
	/// action that it represents. If none of the built in implementations of 
	/// <see cref="IUndoItem"/> can represent the changes you need to record, you
	/// can create your own implementation of <see cref="IUndoItem"/>. You can
	/// inherit from <see cref="UndoItemBase"/> as a starting point, and then you 
	/// only need to implement the Undo and Redo methods, and add properties to
	/// store the information that Undo and Redo need in order to work.
	/// </p>
	/// </remarks>
	public class UndoChain
	{

		private IList _undoList;

		//The maximum number of items this instance will accept before begining 
		//to overwrite the oldest items when new items are added.
		private int _maxItems;		
		
		//The index of the base of the circular buffer. Once we have started 
		//overwriting items, this value will start to move.
		private int _base; // = 0;

		//The number of undo Items in the chain.
		private int _count; // = 0;

		//The index of the top most item. 
		private int _top = -1;

		//The index of the current item. Will always equal the top item 
		//unless one or more undo items have been undone and can be redone (another 
		//undo item has not yet been added). 
		private int _current = -1;

		//We use 2 variables to manage batch edits, one to toggle batch mode
		//and the other to determine if the batch actually has any items.
		//This way toggling batch mode does not leave blank items in the chain.
		private bool _inBatch; // = false;
		private bool _batchStarted; // = false;

		//True if an item has been added since undo or redo has been called.
		//Used to prevent merging with an item unless it has just been added 
		//and no undo or redo operations have been performed.
		private bool _itemJustAdded; // = false;	

		//This flag tells us if we are currently performing an undo or redo.
		//Items added while it is true will be ignored since they result from the
		//undo or redo operation itself and should not become part of the undo history.
		private bool _inOperation; // = false;

		//This flag allows the client code to suspend recording of undo items.
		//Items added while this flag is true will be ignored.
		private bool _suspended; // = false;

		//Used to manage the Dirty property, which can be used to track if there
		//are unsaved changes in the UI.
		private int _cleanItem = -1;

		#region Events

		/// <summary>
		/// Notifies listeners that an Undo has just been performed.
		/// </summary>
		public event UndoActionEventHandler Undone;

		/// <summary>
		/// Notifies listeners that an Redo has just been performed.
		/// </summary>
		/// <remarks>
		/// The Current Item is moved forward before this event is raised, so the 
		/// UndoChain.Current is the item that was redone during this event.
		/// </remarks>
		public event UndoActionEventHandler Redone;

		/// <summary>
		/// Notifies listeners that an Undo is about to be performed.
		/// </summary>
		public event UndoActionEventHandler Undoing;

		/// <summary>
		/// Notifies listeners that an Redo is about to be performed.
		/// </summary>
		/// <remarks>
		/// The Current Item is moved forward before this event is raised, so the 
		/// UndoChain.Current is the item that will be redone during this event.
		/// </remarks>
		public event UndoActionEventHandler Redoing;

		#endregion

		/// <summary>
		/// Initializes a new instance of the UndoChain class.
		/// </summary>
		public UndoChain() : this(int.MaxValue)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the UndoChain class with the specified MaxValue.
		/// </summary>
		public UndoChain(int maxValue)
		{
			_maxItems = maxValue;

			Clear();
		}

		/// <summary>
		/// Gets or sets a value indicating if any changes have been made since this undo chain 
		/// was initialized or since Dirty was last set to false. This can be used to track 
		/// if there are unsaved changes in the UI so the user can be prompted to save if they 
		/// try to close without saving, but will not be promted if there are no changes. 
		/// </summary>
		public bool Dirty 
		{
			get 
			{ 
				return _current != _cleanItem; 
			}
			set 
			{ 
				if (value) 
				{
					_cleanItem = -2;
				}
				else 
				{
					_cleanItem = _current;
				}
			}
		}

		/// <summary>
		/// Clears all items from the UndoChain.
		/// </summary>
		public void Clear() 
		{
			if (_maxItems == int.MaxValue) 
			{
				_undoList = new ArrayList();
			}
			else 
			{
				_undoList = new ArrayList(_maxItems);
			}

			_inBatch = false;
			_batchStarted = false;

			_base = 0;

			_count = 0;
			_top = -1;

			_current = -1;

			_itemJustAdded = false;
			_inOperation = false;

		}

		/// <summary>
		/// Adds the specified <see cref="IUndoItem"/> to the end of the UndoChain.
		/// </summary>
		/// <param name="item">The <see cref="IUndoItem"/> to add to the chain.</param>
		public void AddUndoItem(IUndoItem item) 
		{

			if (_inOperation || _suspended) 
			{
				return;  //<--- Early Exit
			}

			if (! (_inBatch && _batchStarted)) 
			{

                //Give the undo item a chance to check its values and the values of the last 
                //item and cancel the add if it needs to. For example this allows us,
                //if the last item was the same object and property as this item, to merge them.
                //This means that for example if the user types in a text box we will undo the 
                //change all at once not one keystroke at a time.
                IUndoItem current = null;
                if (_itemJustAdded)
                {
                    current = this.Current;
                }

                if (!item.CheckValues(current))
                {
                    return;  //<--- Early Exit
                }

				if (_current == -1) 
				{
					_current = _base;
				}
				else 
				{                    
					//If we are not merging the new item with the current one, 
					//advance the current location.
					_current = (_current + 1) % _maxItems;
				}

				_top = _current;
				_count++;

				//Overwrite the oldest item in the circular buffer if 
				//we have reached the max number of items.
				if (_count > _maxItems) 
				{
					_base = (_base + 1) % _maxItems;
					_count = _maxItems;
				}

				//If this is the first item in a batch, create a batch item.
				if (_inBatch) 
				{
					item = new BatchUndoItem(item);
					_batchStarted = true;
				}

				//Place the new item in the list.
				if (_undoList.Count <= _current) 
				{
					_undoList.Add(item);
				}
				else 
				{
					_undoList[_current] = item;
				}

			}
			else 
			{
				((BatchUndoItem)this.Current).AddSubItem(item);
			}

			_itemJustAdded = true;
		}

		/// <summary>
		/// The current head of the chain, the item that will be undone if Undo is called.
		/// </summary>
		/// <remarks>
		/// This is not generally the item that will be redone if Redo is called, the 
		/// item in front of the Current item is.
		/// During the Redoing and Redone events however the Current item has already 
		/// been moved forward so the current item is the one that will be redone.
		/// </remarks>
		public IUndoItem Current 
		{
			get 
			{
				if (_current < 0) 
				{
					return null;
				}
				else 
				{
					return (IUndoItem)_undoList[_current];
				}
			}
		}

		/// <summary>
		/// The number of items that can be undone in the chain.
		/// </summary>
		public int Count 
		{
			get { return _count; }
		}

		/// <summary>
		/// Reverses the results of the last action recorded in the chain by calling
		/// Undo on the Current <see cref="IUndoItem"/>.
		/// </summary>
		/// <remarks>This operation implicitly calls EndBatchEdit.</remarks>
		public void Undo() 
		{
			EndBatchEdit();

			if (_current < 0) 
			{
				return; //<--- Early Exit
			}

			try 
			{
				_inOperation = true;

				OnUndoing(this.Current);

				//Undo the current item.
				this.Current.Undo();

				IUndoItem item = this.Current;

				//Move the current item back one step.
				if (_current == _base) 
				{
					_current = -1;

				}
				else if (_current == 0) 
				{
					_current = _maxItems -1;
				}
				else 
				{
					_current = (_current - 1) % _maxItems;
				}

				//Decrement the count.
				_count--;

				_itemJustAdded = false;

				OnUndone(item);
			}
			finally 
			{
				_inOperation = false;
			}

			
		}

		/// <summary>
		/// Re-performs the action of the <see cref="IUndoItem"/> most recently Undone.
		/// </summary>
		/// <remarks>This operation implicetly calls EndBatchEdit.</remarks>
		public void Redo() 
		{
			EndBatchEdit();

			if (_current == _top) 
			{
				return; //<--- Early Exit
			}

			//Move the current item formward one step.
			if (_current == -1) 
			{
				_current = _base;
			}
			else 
			{
				_current = (_current + 1) % _maxItems;
			}

			try 
			{

				OnRedoing(this.Current);

				_inOperation = true;

				//Redo the current item.
				this.Current.Redo();

				//increment the count.
				_count++;

				_itemJustAdded = false;

				OnRedone(this.Current);
			}
			finally 
			{
				_inOperation = false;
			}
		}

		/// <summary>
		/// After BeginBatchEdit is called, all <see cref="IUndoItem"/>s added to the chian
		/// we be treated as a single item until EndBatchEdit is called. These items will
		/// be Undone and Redone together as an atomic group.
		/// </summary>
		public void BeginBatchEdit() 
		{
			_inBatch = true;
			_batchStarted = false;
		}

		/// <summary>
		/// After BeginBatchEdit is called, all <see cref="IUndoItem"/>s added to the chian
		/// we be treated as a single item until EndBatchEdit is called. These items will
		/// be Undone and Redone together as an atomic group.
		/// </summary>
		public void EndBatchEdit() 
		{
			_inBatch = false;
			_batchStarted = false;
		}

		/// <summary>
		/// After SuspendEdits is called, and calls to AddUndoItem will be ignored 
		/// until ResumeEdits is called.
		/// </summary>
		public void SuspendEdits() 
		{
			_suspended = true;
		}

		/// <summary>
		/// After SuspendEdits is called, and calls to AddUndoItem will be ignored 
		/// until ResumeEdits is called.
		/// </summary>
		public void ResumeEdits() 
		{
			_suspended = false;
		}

		/// <summary>
		/// Event pattern for the Undone event.
		/// </summary>
		/// <param name="item"></param>
		protected void OnUndone(IUndoItem item) 
		{
			if (Undone != null) 
			{
				Undone(this, new UndoActionEventArgs(item));
			}	
		}

		/// <summary>
		/// Event pattern for the Redone event.
		/// </summary>
		/// <param name="item"></param>
		protected void OnRedone(IUndoItem item) 
		{
			if (Redone != null) 
			{
				Redone(this, new UndoActionEventArgs(item));
			}	
		}

		/// <summary>
		/// Event pattern for the Undoing event.
		/// </summary>
		/// <param name="item"></param>
		protected void OnUndoing(IUndoItem item) 
		{
			if (Undoing != null) 
			{
				Undoing(this, new UndoActionEventArgs(item));
			}	
		}

		/// <summary>
		/// Event pattern for the Redoing event.
		/// </summary>
		/// <param name="item"></param>
		protected void OnRedoing(IUndoItem item) 
		{
			if (Redoing != null) 
			{
				Redoing(this, new UndoActionEventArgs(item));
			}	
		}

		/// <summary>
		/// Returns a value indicating if the undo chain is currently in the 
		/// process of performing an undo or redo operation.
		/// Items added while it is true will be ignored since they result from the
		/// undo or redo operation itself and should not become part of the undo history.
		/// </summary>
		public bool InOperation 
		{
			get { return _inOperation; }
		}

	}
}
