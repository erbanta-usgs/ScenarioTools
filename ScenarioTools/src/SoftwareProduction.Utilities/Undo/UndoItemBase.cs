using System;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// This is an abstract class that partially implements the <see cref="IUndoItem"/> interface.
	/// It implements the tag property, so that specific sub-classes can focus on just the 
	/// undo and redo logic and implement any properties required to do that.
	/// </summary>
	public abstract class UndoItemBase : IUndoItem
	{

		private object _tag;

		#region IUndoItem Members

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		public abstract void Undo();

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public abstract void Redo();

		/// <summary>
		/// Client usable object reference.
		/// </summary>
		public virtual object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

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
        public virtual bool CheckValues(IUndoItem previousUndoItem)
        {
            return true;
        }

		#endregion
	}
}
