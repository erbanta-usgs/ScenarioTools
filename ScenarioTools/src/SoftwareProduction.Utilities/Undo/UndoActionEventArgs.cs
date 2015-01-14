using System;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// Summary description for UndoActionEventArgs.
	/// </summary>
	public class UndoActionEventArgs : EventArgs
	{

		private IUndoItem _undoItem;

		/// <summary>
		/// Instanciates a new instance of UndoActionEventArgs for the specified <see cref="IUndoItem"/>.
		/// </summary>
		/// <param name="undoItem"></param>
		public UndoActionEventArgs(IUndoItem undoItem)
		{
			_undoItem = undoItem;
		}

		/// <summary>
		/// Gets the <see cref="IUndoItem"/> that is being undone or redone.
		/// </summary>
		public IUndoItem UndoItem 
		{
			get { return _undoItem; }
		}
	}
}
