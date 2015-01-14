using System;
using System.Data;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// DataRowChangeUndoItem records changes to a DataRow's field values,
	/// and stores the original value so that change can be reversed.
	/// </summary>
	public class DataRowChangeUndoItem : UndoItemBase
	{
		private object _oldValue;
		private object _newValue;
		private DataRow _row;
		private string _columnName;

		/// <summary>
		/// Initializes a new instance of the DataRowChangeUndoItem class.
		/// </summary>
		public DataRowChangeUndoItem() 
		{

		}

		/// <summary>
		///  Initializes a new instance of the PropertyChangeUndoItem class with the 
		///  specified values for Row, OldValue, NewValue, and ColumnName.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <param name="columnName"></param>
		public DataRowChangeUndoItem(DataRow row, object oldValue, object newValue, string columnName)
		{
			_row = row;
			_oldValue = oldValue;
			_newValue = newValue;
			_columnName = columnName;
		}

		/// <summary>
		/// Gets or sets the DataRow that was modified 
		/// by the change that is recorded by this UndoItem.
		/// </summary>
		public DataRow Row 
		{
			get { return _row; }
		}

		/// <summary>
		/// Gets or sets the original value of the DataRow's field before it was modified.
		/// </summary>
		public object OldValue 
		{
			get { return _oldValue; }
		}

		/// <summary>
		/// Gets or sets the value that the DataRow's field was changed to.
		/// </summary>
		public object NewValue 
		{
			get { return _newValue; }
			set { _newValue = value; }
		}

		/// <summary>
		/// Gets or sets the name of the DataColumn whose modification 
		/// is recorded in this UndoItem.
		/// </summary>
		public string ColumnName 
		{
			get { return _columnName; }
		}

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		public override void Undo()
		{

			if (_row == null) 
			{
				return;		//<--- Early Exit
			}

			_row[_columnName] = _oldValue;

		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public override void Redo()
		{
			if (_row == null) 
			{
				return;		//<--- Early Exit
			}

			_row[_columnName] = _newValue;

		}

        /// <summary>
        /// Gives the undo item a chance to cancel itself being added to the undo chain.
        /// This could be used for example if the original value and the new value are identical,
        /// or to merge the changes represented by this undo item into the previous one, if 
        /// the same object is being modified.
        /// </summary>
        /// <param name="previousUndoItem">The undo item currently at the top of the undo chain before this item is added.</param>
        /// <returns>A value indicating if this undo item should be added to the undo chain.</returns>
        public override bool CheckValues(IUndoItem previousUndoItem)
        {
            if ((this.NewValue == null && this.OldValue == null) ||
                (this.NewValue != null && this.NewValue.Equals(this.OldValue)))
            {
                return false;
            }

            DataRowChangeUndoItem lastItem = previousUndoItem as DataRowChangeUndoItem;

            if (lastItem != null && lastItem.Row == this.Row && lastItem.ColumnName == this.ColumnName)
            {
                if (this.NewValue != null && TypeChecker.TypeIsAtomic(this.NewValue.GetType()))
                {
                    if (this.NewValue != null && lastItem.NewValue != null &&
                        this.NewValue.GetType() == lastItem.NewValue.GetType())
                    {
                        lastItem.NewValue = this.NewValue;
                        return false;
                    }
                }
            }

            return true;
        }
	}
}
