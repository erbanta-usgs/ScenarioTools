using System;
using System.Data;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// The DataSetChangeLogger listens for events in <see cref="DataSet"/>s and <see cref="DataTable"/>s,
	/// and automatically records the changes as <see cref="IUndoItem"/>s in an <see cref="UndoChain"/>.
	/// </summary>
	public class DataSetChangeLogger
	{
		UndoChain _undoChain;

		/// <summary>
		/// Initializes a new instance of the DataSetChangeLogger class to use the specified <see cref="UndoChain"/>.
		/// </summary>
		public DataSetChangeLogger(UndoChain undoChain)
		{
			_undoChain = undoChain;
		}

		/// <summary>
		/// Starts recording changes to the specified dataSet in the undoChain.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <exception cref="ArgumentNullException">Thrown if <i>dataSet</i> is null (Nothing in Visual Basic).</exception>
		public void RecordChanges(DataSet dataSet) 
		{
			ExceptionHelper.ExceptionIfNull(dataSet, "dataSet");

			foreach (DataTable table in dataSet.Tables) 
			{
				RecordChanges(table);
			}
		}

		/// <summary>
		/// Starts recording changes to the specified dataTable in the undoChain.
		/// </summary>
		/// <param name="dataTable"></param>
		/// <exception cref="ArgumentNullException">Thrown if <i>dataTable</i> is null (Nothing in Visual Basic).</exception>
		public void RecordChanges(DataTable dataTable) 
		{
			ExceptionHelper.ExceptionIfNull(dataTable, "dataTable");

			dataTable.RowChanged += new DataRowChangeEventHandler(dataTable_RowChanged);
			dataTable.ColumnChanging += new DataColumnChangeEventHandler(dataTable_ColumnChanging);
			dataTable.RowDeleting += new DataRowChangeEventHandler(dataTable_RowDeleting);
		}

		/// <summary>
		/// Stops recording changes to the specified dataSet in the undoChain.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <exception cref="ArgumentNullException">Thrown if <i>dataSet</i> is null (Nothing in Visual Basic).</exception>
		public void StopRecordingChanges(DataSet dataSet) 
		{
			ExceptionHelper.ExceptionIfNull(dataSet, "dataSet");

			foreach (DataTable table in dataSet.Tables) 
			{
				StopRecordingChanges(table);
			}
		}

		/// <summary>
		/// Stops recording changes to the specified dataTable in the undoChain.
		/// </summary>
		/// <param name="dataTable"></param>
		/// <exception cref="ArgumentNullException">Thrown if <i>dataTable</i> is null (Nothing in Visual Basic).</exception>
		public void StopRecordingChanges(DataTable dataTable) 
		{
			ExceptionHelper.ExceptionIfNull(dataTable, "dataTable");

			dataTable.RowChanged -= new DataRowChangeEventHandler(dataTable_RowChanged);
			dataTable.ColumnChanging -=new DataColumnChangeEventHandler(dataTable_ColumnChanging);
			dataTable.RowDeleting -=new DataRowChangeEventHandler(dataTable_RowDeleting);
		}

		private void dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			if (e.Action == DataRowAction.Add) 
			{
				_undoChain.AddUndoItem( new DataTableRowAddUndoItem((DataTable)sender, e.Row, -1) ); 
			}
		}

		private void dataTable_ColumnChanging(object sender, DataColumnChangeEventArgs e)
		{
            _undoChain.AddUndoItem(new DataRowChangeUndoItem(e.Row, e.Row[e.Column], e.ProposedValue, e.Column.ColumnName));
		}

		private void dataTable_RowDeleting(object sender, DataRowChangeEventArgs e)
		{
			if (e.Action == DataRowAction.Delete) 
			{
				_undoChain.AddUndoItem( new DataTableRowDeleteUndoItem((DataTable)sender, e.Row, -1) ); 
			}
		}
	}
}
