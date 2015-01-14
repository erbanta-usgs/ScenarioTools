using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SoftwareProductions.Utilities
{
	/// <summary>
	/// This class provides methods to save and retrieve data with an Image field in SQL Server.
	/// The data is passed in chunks, avoiding the need to create a buffer on the client to 
	/// contain the entire image, so it is able to handle large files.
	/// </summary>
	public sealed class SqlBinaryDataManager
	{

		/// <summary>
		/// Size of the buffer in bytes. This is the size of the 'chunks' of data.
		/// </summary>
		public const int BufferSize = 102400 * 2; //200KB

		private SqlBinaryDataManager()
		{
			
		}

		/// <summary>
		/// Writes the bytes in the specified stream to the database. 
		/// </summary>
		/// <param name="stream">The stream containing the bytes to write.</param>
		/// <param name="connection">An open connection to the database. The connection will not be closed when the operation is completed.</param>
		/// <param name="transaction">A optional transaction to perform the operation in. Pass null for no transaction.</param>
		/// <param name="tableName">The name of the table that the data will be stored in.</param>
		/// <param name="fieldName">The name of the image (BLOB) field that the data will be stored in.</param>
		/// <param name="keyFieldName">The name of the primary key field on the table.</param>
		/// <param name="keyValue">The value of the primary key for the row the image will be saved to.</param>
		public static void WriteBinaryData(Stream stream, SqlConnection connection, SqlTransaction transaction, string tableName, string fieldName, string keyFieldName, string keyValue) 
		{
			ExceptionHelper.ExceptionIfNull(stream, "stream");
			ExceptionHelper.ExceptionIfNull(connection, "connection");

			int doneBytes = 0;
			byte[] buffer = new byte[BufferSize];

			//We need to set the image field to null in case it already contains a larger BLOB than the one
			//we are saving, in which case it would leave junk data at the end of the field.
			string clearSql = "UPDATE " + tableName + " SET " + fieldName + " = NULL WHERE " + keyFieldName + " = " + keyValue;
			SqlCommand clearCommand = CreateCommand(clearSql, connection, transaction);
			clearCommand.ExecuteNonQuery();

			while (stream.Length > doneBytes) 
			{
				//The size of the next chunk should be the buffer size or the 
				//size of the remaining data if this is the last chunk.
				int bytesToSend = (int)stream.Length - doneBytes;

				if (bytesToSend > BufferSize) 
				{
					bytesToSend = BufferSize;
				}
				else 
				{
					//Create a new buffer of the correct size if this it the last 
					//chunk and it is smaller than the normal size.
					buffer = new byte[bytesToSend];
				}

				//Read the chunk from the stream. The offset is 0 because the 
				//stream position will automatically move as we read.
				stream.Read(buffer, 0, bytesToSend);
				//stream.Read(buffer, doneBytes, bytesToSend);

				//Get the pointer to pass to UPDATETEXT.
				string pointerSql = "SELECT @Pointer = TEXTPTR(Data) FROM " + tableName + " WHERE " + keyFieldName + " = " + keyValue;

				SqlCommand pointerCommand = CreateCommand(pointerSql, connection, transaction);

				SqlParameter pointer = pointerCommand.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
				pointer.Direction = ParameterDirection.Output;
				pointerCommand.ExecuteNonQuery();

				//Use UPDATETEXT to write the chunk to the table.
				string updateSql = "UPDATETEXT " + tableName + "." + fieldName + " @Pointer @Offset 0 @Bytes";

				SqlCommand command = CreateCommand(updateSql, connection, transaction);

				command.Parameters.Add("@Pointer", pointer.Value);
				command.Parameters.Add("@Offset", doneBytes);
				command.Parameters.Add("@Bytes", buffer);
				command.ExecuteNonQuery();

				doneBytes += bytesToSend;
			}

		}

		/// <summary>
		/// Reads bytes from a database image field into the specified stream.
		/// </summary>
		/// <param name="stream">The stream that the bytes will be written to.</param>
		/// <param name="connection">An open connection to the database. The connection will not be closed when the operation is completed.</param>
		/// <param name="transaction">A optional transaction to perform the operation in. Pass null for no transaction.</param>
		/// <param name="tableName">The name of the table that the data is stored in.</param>
		/// <param name="fieldName">The name of the image field that the data is stored in.</param>
		/// <param name="keyFieldName">The name of the primary key field on the table.</param>
		/// <param name="keyValue">The value of the primary key for the row the image will be read from.</param>
		public static void ReadBinatyData(Stream stream, SqlConnection connection, SqlTransaction transaction, string tableName, string fieldName, string keyFieldName, string keyValue) 
		{
			ExceptionHelper.ExceptionIfNull(stream, "stream");
			ExceptionHelper.ExceptionIfNull(connection, "connection");

			string lengthSql = "SELECT DATALENGTH(" + fieldName + ") FROM " + tableName + " WHERE " + keyFieldName + " = " + keyValue;
			SqlCommand lengthCommand = CreateCommand(lengthSql, connection, transaction); 

			int totalBytes = (int)lengthCommand.ExecuteScalar();
			int doneBytes = 0;

			while (doneBytes < totalBytes) 
			{
				//The size of the next chunk should be the buffer size or the 
				//size of the remaining data if this is the last chunk.
				int bytesToRead = totalBytes - doneBytes;

				if (bytesToRead > BufferSize) 
				{
					bytesToRead = BufferSize;
				}

				//Get the pointer to pass to READTEXT.
				string pointerSql = "SELECT @Pointer = TEXTPTR(Data) FROM " + tableName + " WHERE " + keyFieldName + " = " + keyValue;

				SqlCommand pointerCommand = CreateCommand(pointerSql, connection, transaction);

				SqlParameter pointer = pointerCommand.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
				pointer.Direction = ParameterDirection.Output;
				pointerCommand.ExecuteNonQuery();

				if (! DBNull.Value.Equals(pointer.Value)) 
				{
					//Use READTEXT to read the chunk from the table.
					string readSql = "READTEXT " + tableName + "." + fieldName + " @Pointer @Offset @Size";

					SqlCommand command = CreateCommand(readSql, connection, transaction);

					command.Parameters.Add("@Pointer", pointer.Value);
					command.Parameters.Add("@Offset", doneBytes);
					command.Parameters.Add("@Size", bytesToRead);

					byte[] buffer = (byte[])command.ExecuteScalar();

					//Write the chunk to the stream. The offset is 0 because the 
					//stream position will automatically move as we write.
					stream.Write(buffer, 0, bytesToRead);
					//stream.Write(buffer, doneBytes, bytesToRead);

					doneBytes += bytesToRead;
				}
			}
		}

		/// <summary>
		/// Creates a command using the correct constructor depending 
		/// on whether the transaction is null or not.
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		private static SqlCommand CreateCommand(string commandText, SqlConnection connection, SqlTransaction transaction) 
		{
			SqlCommand command;
			if (transaction == null) 
			{
				command = new SqlCommand(commandText, connection);
			}
			else 
			{
				command = new SqlCommand(commandText, connection, transaction);
			}
			return command;
		}

		/// <summary>
		/// Saves an entire file into an image field in a database.
		/// </summary>
		/// <param name="fileName">The path of the file to read the data from.</param>
		/// <param name="connection">An open connection to the database. The connection will not be closed when the operation is completed.</param>
		/// <param name="transaction">A optional transaction to perform the operation in. Pass null for no transaction.</param>
		/// <param name="tableName">The name of the table that the data will be stored in.</param>
		/// <param name="fieldName">The name of the image field that the data will be stored in.</param>
		/// <param name="keyFieldName">The name of the primary key field on the table.</param>
		/// <param name="keyValue">The value of the primary key for the row the image will be saved to.</param>
		public static void StoreFile(string fileName, SqlConnection connection, SqlTransaction transaction, string tableName, string fieldName, string keyFieldName, string keyValue) 
		{
			ExceptionHelper.ExceptionIfNull(connection, "connection");

			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) 
			{
				WriteBinaryData(stream, connection, transaction, tableName, fieldName, keyFieldName, keyValue);
			}		
		}

		/// <summary>
		/// Reads an image field from a database and stores the result in a file.
		/// </summary>
		/// <param name="fileName">The path of the file to save the data to.</param>
		/// <param name="connection">An open connection to the database. The connection will not be closed when the operation is completed.</param>
		/// <param name="transaction">A optional transaction to perform the operation in. Pass null for no transaction.</param>
		/// <param name="tableName">The name of the table that the data is stored in.</param>
		/// <param name="fieldName">The name of the image field that the data is stored in.</param>
		/// <param name="keyFieldName">The name of the primary key field on the table.</param>
		/// <param name="keyValue">The value of the primary key for the row the image will be read from.</param>
		public static void FetchFile(string fileName, SqlConnection connection, SqlTransaction transaction, string tableName, string fieldName, string keyFieldName, string keyValue) 
		{
			ExceptionHelper.ExceptionIfNull(connection, "connection");

			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) 
			{
				ReadBinatyData(stream, connection, transaction, tableName, fieldName, keyFieldName, keyValue);
			}		
		}

	}
}
