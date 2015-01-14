using System;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// Classes must implement this interface in order to be members of an <see cref="UndoChain"/>.
	/// Undo Items represent modifications to data or objects that can be undone and redone, typically
	/// to implement standard Edit - Undo/Redo commands in a UI. 
	/// </summary>
	/// <remarks>
	/// When a client wants modifications to be undoable, it makes the change and puts 
	/// an undo item that represents that change into an <see cref="UndoChain"/>.
	/// The change can then be undone and redone via the Undo() and Redo() methods on the
	/// undo item, which is managed by the <see cref="UndoChain"/>.
	/// </remarks>
	public interface IUndoItem
	{
		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		void Undo();

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		void Redo();
		
		/// <summary>
		/// Client usable object reference.
		/// </summary>
		object Tag { get; set; }

        /// <summary>
        /// Gives the undo item a chance to cancel itself being added to the undo chain.
        /// This could be used for example if the original value and the new value are identical,
        /// or to merge the changes represented by this undo item into the previous one, if 
        /// the same object is being modified.
        /// </summary>
        /// <param name="previousUndoItem">The undo item currently at the top of the undo chain before this item is added.</param>
        /// <returns>A value indicating if this undo item should be added to the undo chain.</returns>
		/// <remarks>
		/// CheckValues should return true unless there is a specific reason to cancel 
		/// adding this item to the UndoChain.
		/// </remarks>
		bool CheckValues(IUndoItem previousUndoItem);
	}
}
