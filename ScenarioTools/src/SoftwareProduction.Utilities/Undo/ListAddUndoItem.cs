using System;
using System.Collections;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// This class implements the <see cref="IUndoItem"/> interface for undoing and 
	/// redoing add operations to an <see cref="IList"/>.
	/// </summary>
	public class ListAddUndoItem : UndoItemBase 
	{

		private IList _list;
		private object _item;
		private int _index;

		/// <summary>
		/// Initializes a new instance of the ListAddUndoItem class, 
		/// with the specified List, Item, and Index values.
		/// </summary>
		/// <param name="list">The list that this undo item represents an add operation on.</param>
		/// <param name="item">The item that was added to the list.</param>
		/// <param name="index">
		/// The index that the item was added at. A value of -1 indicates that the item 
		/// was added to the end of the list. If the list may contain multiple instances
		/// of the same object, the index must be specified, not left as -1, otherwise
		/// undo/redo operations will not work correctly.
		/// </param>
		public ListAddUndoItem(IList list, object item, int index)
		{
			_list = list;
			_item = item;
			_index = index;
		}

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		public override void Undo()
		{
			if (_index >= 0) 
			{
				_list.RemoveAt(_index);
			}
			else 
			{
				_list.Remove(_item);
			}
		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public override void Redo()
		{
			if (_index >= 0) 
			{
				_list.Insert(_index, _item);
			}
			else 
			{
				_list.Add(_item);
			}
		}

		/// <summary>
		/// Gets a reference to the list that an item was 
		/// added to for this ListAddUndoItem.
		/// </summary>
		public IList List 
		{
			get { return  _list; } 
		}

		/// <summary>
		/// Gets a reference to the item that was added to
		/// the list for this ListAddUndoItem.
		/// </summary>
		public object Item 
		{
			get { return  _item; } 
		}

		/// <summary>
		/// Gets a reference to the index of the item that was 
		/// added to the list for this ListAddUndoItem.
		/// </summary>
		public int Index 
		{
			get { return  _index; } 
		}
	}
}
