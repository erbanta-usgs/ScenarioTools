using System;
using System.Collections;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// This class implements the <see cref="IUndoItem"/> interface for undoing and 
	/// redoing remove operations from an <see cref="IDictionary"/>.
	/// </summary>
	public class DictionaryRemoveUndoItem : UndoItemBase
	{

		private IDictionary _dictionary;
		private object _item;
		private object _key;

		/// <summary>
		/// Initializes a new instance of the DictionaryRemoveUndoItem class with the 
		/// specified Dictionary, Item and Key values.
		/// </summary>
		/// <param name="dictionary">The dictionary that this undo item represents a remove operation from.</param>
		/// <param name="item">The item that was removed from the dictionary.</param>
		/// <param name="key">The key that was removed from the dictionary.</param>
		public DictionaryRemoveUndoItem(IDictionary dictionary, object item, object key)
		{
			_dictionary = dictionary;
			_item = item;
			_key = key;
		}

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		public override void Undo()
		{
			_dictionary.Add(_key, _item);
		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public override void Redo()
		{
			_dictionary.Remove(_key);
		}

		/// <summary>
		/// Gets a reference to the dictionary that an item was 
		/// removed from for this DictionaryRemoveUndoItem.
		/// </summary>
		public IDictionary Dictionary 
		{
			get { return  _dictionary; } 
		}

		/// <summary>
		/// Gets a reference to the item that was removed from 
		/// the dictionary for this DictionaryRemoveUndoItem.
		/// </summary>
		public object Item 
		{
			get { return  _item; } 
		}

		/// <summary>
		/// Gets a reference to the key that was removed from 
		/// the dictionary for this DictionaryRemoveUndoItem.
		/// </summary>
		public object Key 
		{
			get { return  _key; } 
		}
	}
}
