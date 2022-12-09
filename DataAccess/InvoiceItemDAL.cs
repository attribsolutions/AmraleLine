using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

using DataObjects;

namespace DataAccess
{
    /// <summary>
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the InvoiceItem table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:09:16 PM
    /// </summary>
    public class InvoiceItemDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public InvoiceItemDAL()
        {

        }
        /// <summary>
        /// Gets all the InvoiceItems from the InvoiceItems table
        /// </summary>
        /// <returns>BindingList of InvoiceItems</returns>
        public BindingList<InvoiceItemInfo> GetInvoiceItemsAll()
        {
            BindingList<InvoiceItemInfo> retVal = new BindingList<InvoiceItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [InvoiceID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount] FROM InvoiceItems ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    InvoiceItemInfo invoiceItem = new InvoiceItemInfo();
                    invoiceItem.ID = Convert.ToInt64(dataReader["ID"]);
                    invoiceItem.InvoiceID = Convert.ToInt64(dataReader["InvoiceID"]);
                    invoiceItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    invoiceItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    invoiceItem.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    invoiceItem.Vat = Convert.ToDecimal(dataReader["Vat"]);
                    invoiceItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    invoiceItem.Amount = Convert.ToDecimal(dataReader["Amount"]);

                    retVal.Add(invoiceItem);
                }
            }
            return retVal;
        }

        public BindingList<InvoiceItemInfo> GetInvoiceItemsByInvoiceId(int invoiceId)
        {
            BindingList<InvoiceItemInfo> retVal = new BindingList<InvoiceItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [InvoiceID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount] FROM InvoiceItems WHERE InvoiceID = @InvoiceId");
            _db.AddInParameter(command, "@InvoiceId", DbType.Int32, invoiceId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    InvoiceItemInfo invoiceItem = new InvoiceItemInfo();
                    invoiceItem.ID = Convert.ToInt64(dataReader["ID"]);
                    invoiceItem.InvoiceID = Convert.ToInt64(dataReader["InvoiceID"]);
                    invoiceItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    invoiceItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    invoiceItem.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    invoiceItem.Vat = Convert.ToDecimal(dataReader["Vat"]);
                    invoiceItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    invoiceItem.Amount = Convert.ToDecimal(dataReader["Amount"]);

                    retVal.Add(invoiceItem);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single InvoiceItem based on Id
        /// </summary>
        /// <param name="invoiceitemId">Id of the InvoiceItem the needs to be retrieved</param>
        /// <returns>Instance of InvoiceItem</returns>
        public InvoiceItemInfo GetInvoiceItem(int invoiceitemId)
        {
            InvoiceItemInfo retVal = new InvoiceItemInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [InvoiceID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount] FROM InvoiceItems WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, invoiceitemId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.InvoiceID = Convert.ToInt64(dataReader["InvoiceID"]);
                    retVal.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    retVal.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    retVal.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    retVal.Vat = Convert.ToDecimal(dataReader["Vat"]);
                    retVal.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    retVal.Amount = Convert.ToDecimal(dataReader["Amount"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new InvoiceItem in to the database
        /// </summary>
        /// <param name="invoiceitem">Instance of InvoiceItem</param>
        /// <returns>Id of the newly added InvoiceItem</returns>
        public int AddInvoiceItem(InvoiceItemInfo invoiceItem)
        {
            int retval = 0;
            DbCommand commandInvoiceItem = _db.GetSqlStringCommand("INSERT INTO InvoiceItems([InvoiceID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount]) " +
                                                        "VALUES (@InvoiceID, @ItemID, @Quantity, @UnitID, @Vat, @Rate, @Amount) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('InvoiceItems')");

            _db.AddInParameter(commandInvoiceItem, "@InvoiceID", DbType.Int64, invoiceItem.InvoiceID);
            _db.AddInParameter(commandInvoiceItem, "@ItemID", DbType.Int32, invoiceItem.ItemID);
            _db.AddInParameter(commandInvoiceItem, "@Quantity", DbType.Decimal, invoiceItem.Quantity);
            _db.AddInParameter(commandInvoiceItem, "@UnitID", DbType.Int32, invoiceItem.UnitID);
            _db.AddInParameter(commandInvoiceItem, "@Vat", DbType.Decimal, invoiceItem.Vat);
            _db.AddInParameter(commandInvoiceItem, "@Rate", DbType.Decimal, invoiceItem.Rate);
            _db.AddInParameter(commandInvoiceItem, "@Amount", DbType.Decimal, invoiceItem.Amount);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandInvoiceItem));
            }
            return retval;
        }

        /// <summary>
        /// Updates the InvoiceItem
        /// </summary>
        /// <param name="invoiceitem">Instance of InvoiceItem class</param>
        public void UpdateInvoiceItem(InvoiceItemInfo invoiceItem)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE InvoiceItems SET [InvoiceID] = @InvoiceID, [ItemID] = @ItemID, [Quantity] = @Quantity, [UnitID] = @UnitID, [Vat] = @Vat, [Rate] = @Rate, [Amount] = @Amount WHERE Id = @Id ");
            _db.AddInParameter(command, "@InvoiceID", DbType.Int64, invoiceItem.InvoiceID);
            _db.AddInParameter(command, "@ItemID", DbType.Int32, invoiceItem.ItemID);
            _db.AddInParameter(command, "@Quantity", DbType.Decimal, invoiceItem.Quantity);
            _db.AddInParameter(command, "@UnitID", DbType.Int32, invoiceItem.UnitID);
            _db.AddInParameter(command, "@Vat", DbType.Decimal, invoiceItem.Vat);
            _db.AddInParameter(command, "@Rate", DbType.Decimal, invoiceItem.Rate);
            _db.AddInParameter(command, "@Amount", DbType.Decimal, invoiceItem.Amount);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the InvoiceItem from the database
        /// </summary>
        /// <param name="invoiceitemId">Id of the InvoiceItem that needs to be deleted</param>
        public void DeleteInvoiceItem(int invoiceitemId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM InvoiceItems " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, invoiceitemId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

    }
}