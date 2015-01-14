using System;
using System.Collections;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// BatchUndoItem aggegates a set of IUndoItem instances that should be
	/// undone and redone together as an atomic operation.
	/// </summary>
	public class BatchUndoItem : UndoItemBase	
	{

		private IList _subItems;

		/// <summary>
		/// Initializes a new instance of the BatchUndoItem class.
		/// </summary>
		public BatchUndoItem()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the BatchUndoItem class with the specified first member.
		/// </summary>
		/// <param name="subItem"></param>
		public BatchUndoItem(IUndoItem subItem)
		{
			AddSubItem(subItem);
		}

		/// <summary>
		/// Gets a list of the sub items that make up this BatchUndoItem.
		/// </summary>
		public IList SubItems 
		{
			get { return _subItems; }
		}

		/// <summary>
		/// Adds a sub item to the list. The sub item will be undone after items already in the 
		/// list when this instance's Undo Method is called.
		/// </summary>
		/// <param name="item"></param>
		public void AddSubItem(IUndoItem item) 
		{
			if (_subItems == null) 
			{
				_subItems = new ArrayList();
			}

			_subItems.Add(item);
		}

		#region IUndoItem Members

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// Calls the Undo method of all the sub items in order.
		/// </summary>
		public override void Undo()
		{
			if (_subItems != null) 
			{
				//Undo the sub-items in reverse order.
				for (int i = _subItems.Count -1; i >= 0; i--) 
				{
					((IUndoItem)_subItems[i]).Undo();
				}
			}
		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// Calls the Redo method of all the sub items in order.
		/// </summary>
		public override void Redo()
		{
			if (_subItems != null) 
			{
				//Redo the sub-items in order.
				foreach (IUndoItem item in _subItems) 
				{
					item.Redo();
				}	
			}
		}

		#endregion

	}
}
