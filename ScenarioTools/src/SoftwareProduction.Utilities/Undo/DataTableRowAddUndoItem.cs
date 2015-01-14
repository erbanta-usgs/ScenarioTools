using System;
using System.Data;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// Summary description for DataTableRowAddUndoItem.
	/// </summary>
	public class DataTableRowAddUndoItem : UndoItemBase 
	{

		private DataTable _table;
		private DataRow _row;
		private int _index;
		private object[] _values;

		/// <summary>
		/// Initializes a new instance of the DataTableRowAddUndoItem with the specified table, row and index properties.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="row"></param>
		/// <param name="index"></param>
		/// <exception cref="ArgumentNullException">Thrown if <i>row</i> is null (Nothing in Visual Basic).</exception>
		public DataTableRowAddUndoItem(DataTable table, DataRow row, int index)
		{
			ExceptionHelper.ExceptionIfNull(row, "row");

			_table = table;
			_row = row;
			_index = index;
			_values = _row.ItemArray;
		}

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		public override void Undo()
		{
			if (_index >= 0) 
			{
				_table.Rows.RemoveAt(_index);
			}
			else 
			{
				_table.Rows.Remove(_row);
			}
		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public override void Redo()
		{
			if (_index >= 0) 
			{
				_table.Rows.InsertAt(_row, _index);
			}
			else 
			{
				_table.Rows.Add(_row);
			}
			_row.ItemArray = _values;
		}

		/// <summary>
		/// Gets a reference to the DataTable that a row was 
		/// added to for this DataTableRowAddUndoItem.
		/// </summary>
		public DataTable Table 
		{
			get { return  _table; } 
		}

		/// <summary>
		/// Gets a reference to the row that was added to
		/// the DataTable for this DataTableRowAddUndoItem.
		/// </summary>
		public DataRow Row 
		{
			get { return  _row; } 
		}

		/// <summary>
		/// Gets a reference to the index of the row that was 
		/// added to the DataTable for this DataTableRowAddUndoItem.
		/// </summary>
		public int Index 
		{
			get { return  _index; } 
		}
	}
}
