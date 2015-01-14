using System;
using System.Collections;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// This class implements the <see cref="IUndoItem"/> interface for undoing and 
	/// redoing add operations from an <see cref="IDictionary"/>.
	/// </summary>
	public class DictionaryAddUndoItem : UndoItemBase
	{

		private IDictionary _dictionary;
		private object _item;
		private object _key;

		/// <summary>
		/// Initializes a new instance of the DictionaryAddUndoItem class with the 
		/// specified Dictionary, Item and Key values.
		/// </summary>
		/// <param name="dictionary">The dictionary that this undo item represents a add operation to.</param>
		/// <param name="item">The item that was added to the dictionary.</param>
		/// <param name="key">The key that was added to the dictionary.</param>
		public DictionaryAddUndoItem(IDictionary dictionary, object item, object key)
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
			_dictionary.Remove(_key);
		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public override void Redo()
		{
			_dictionary.Add(_key, _item);
		}

		/// <summary>
		/// Gets a reference to the dictionary that an item was 
		/// added to for this DictionaryAddUndoItem.
		/// </summary>
		public IDictionary Dictionary 
		{
			get { return  _dictionary; } 
		}

		/// <summary>
		/// Gets a reference to the item that was added to
		/// the dictionary for this DictionaryAddUndoItem.
		/// </summary>
		public object Item 
		{
			get { return  _item; } 
		}

		/// <summary>
		/// Gets a reference to the key that was added to
		/// the dictionary for this DictionaryAddUndoItem.
		/// </summary>
		public object Key 
		{
			get { return  _key; } 
		}


	}
}
