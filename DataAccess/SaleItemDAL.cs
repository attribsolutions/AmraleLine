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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the SaleItem table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:15:44 PM
    /// </summary>
    public class SaleItemDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public SaleItemDAL()
        {

        }
        /// <summary>
        /// Gets all the SaleItems from the SaleItems table
        /// </summary>
        /// <returns>BindingList of SaleItems</returns>
        public BindingList<SaleItemInfo> GetSaleItemsAll()
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [SaleID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount] FROM SaleItems ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleItemInfo saleItem = new SaleItemInfo();
                    saleItem.ID = Convert.ToInt64(dataReader["ID"]);
                    saleItem.SaleID = Convert.ToInt64(dataReader["SaleID"]);
                    saleItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    saleItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    saleItem.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    saleItem.Vat = Convert.ToDecimal(dataReader["Vat"]);
                    saleItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    saleItem.Amount = Convert.ToDecimal(dataReader["Amount"]);

                    retVal.Add(saleItem);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single SaleItem based on Id
        /// </summary>
        /// <param name="saleitemId">Id of the SaleItem the needs to be retrieved</param>
        /// <returns>Instance of SaleItem</returns>
        public SaleItemInfo GetSaleItem(int saleitemId)
        {
            SaleItemInfo retVal = new SaleItemInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [SaleID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount] FROM SaleItems WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, saleitemId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.SaleID = Convert.ToInt64(dataReader["SaleID"]);
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
        /// Adds new SaleItem in to the database
        /// </summary>
        /// <param name="saleitem">Instance of SaleItem</param>
        /// <returns>Id of the newly added SaleItem</returns>
        public int AddSaleItem(SaleItemInfo saleItem)
        {
            int retval = 0;
            DbCommand commandSaleItem = _db.GetSqlStringCommand("INSERT INTO SaleItems([SaleID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount]) " +
                                                        "VALUES (@SaleID, @ItemID, @Quantity, @UnitID, @Vat, @Rate, @Amount) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('SaleItems')");

            _db.AddInParameter(commandSaleItem, "@SaleID", DbType.Int64, saleItem.SaleID);
            _db.AddInParameter(commandSaleItem, "@ItemID", DbType.Int32, saleItem.ItemID);
            _db.AddInParameter(commandSaleItem, "@Quantity", DbType.Decimal, saleItem.Quantity);
            _db.AddInParameter(commandSaleItem, "@UnitID", DbType.Int32, saleItem.UnitID);
            _db.AddInParameter(commandSaleItem, "@Vat", DbType.Decimal, saleItem.Vat);
            _db.AddInParameter(commandSaleItem, "@Rate", DbType.Decimal, saleItem.Rate);
            _db.AddInParameter(commandSaleItem, "@Amount", DbType.Decimal, saleItem.Amount);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandSaleItem));
            }
            return retval;
        }

        /// <summary>
        /// Updates the SaleItem
        /// </summary>
        /// <param name="saleitem">Instance of SaleItem class</param>
        public void UpdateSaleItem(SaleItemInfo saleItem)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE SaleItems SET [SaleID] = @SaleID, [ItemID] = @ItemID, [Quantity] = @Quantity, [UnitID] = @UnitID, [Vat] = @Vat, [Rate] = @Rate, [Amount] = @Amount WHERE Id = @Id ");
            _db.AddInParameter(command, "@SaleID", DbType.Int64, saleItem.SaleID);
            _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
            _db.AddInParameter(command, "@Quantity", DbType.Decimal, saleItem.Quantity);
            _db.AddInParameter(command, "@UnitID", DbType.Int32, saleItem.UnitID);
            _db.AddInParameter(command, "@Vat", DbType.Decimal, saleItem.Vat);
            _db.AddInParameter(command, "@Rate", DbType.Decimal, saleItem.Rate);
            _db.AddInParameter(command, "@Amount", DbType.Decimal, saleItem.Amount);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the SaleItem from the database
        /// </summary>
        /// <param name="saleitemId">Id of the SaleItem that needs to be deleted</param>
        public void DeleteSaleItem(int saleitemId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM SaleItems " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, saleitemId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public BindingList<SaleItemInfo> GetSaleItemsByDate(DateTime saleDate, DateTime eDate, bool timeWise)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            DbCommand command = null;
            if (timeWise)
                command = _db.GetSqlStringCommand("SELECT Sales.BillDate, SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, Items.ItemGroupID, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount, Sales.CreatedBy, Sales.CreatedOn, Sales.UpdatedBy, Sales.UpdatedOn, Sales.BillDate SaleDate FROM SaleItems INNER JOIN Items ON Items.ID = SaleItems.ItemID INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.CreatedOn >= @SaleDate AND Sales.CreatedOn <= @ESaleDate");
            else
                command = _db.GetSqlStringCommand("SELECT Sales.BillDate, SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, Items.ItemGroupID, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount, Sales.CreatedBy, Sales.CreatedOn, Sales.UpdatedBy, Sales.UpdatedOn, Sales.BillDate SaleDate FROM SaleItems INNER JOIN Items ON Items.ID = SaleItems.ItemID INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillDate >= @SaleDate AND Sales.BillDate <= @ESaleDate");

            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleDate);
            _db.AddInParameter(command, "@ESaleDate", DbType.DateTime, eDate);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleItemInfo saleItem = new SaleItemInfo();
                    saleItem.ID = Convert.ToInt64(dataReader["ID"]);
                    saleItem.SaleID = Convert.ToInt32(dataReader["SaleID"]);
                    saleItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    saleItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    saleItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    saleItem.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    saleItem.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    saleItem.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    saleItem.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    saleItem.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                    saleItem.SaleDate = Convert.ToDateTime(dataReader["SaleDate"]);

                    retVal.Add(saleItem);
                }
            }
            return retVal;
        }

        public BindingList<SaleItemInfo> GetSaleItemsByDateItemWise(DateTime saleDate, DateTime eDate, int itemId)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT Sales.BillDate, SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, Items.ItemGroupID, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount, Sales.CreatedBy, Sales.CreatedOn, Sales.UpdatedBy, Sales.UpdatedOn, Sales.BillDate SaleDate FROM SaleItems INNER JOIN Items ON Items.ID = SaleItems.ItemID INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillDate >= @SaleDate AND Sales.BillDate <= @ESaleDate AND SaleItems.ItemId = @ItemId");
            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleDate);
            _db.AddInParameter(command, "@ESaleDate", DbType.DateTime, eDate);
            _db.AddInParameter(command, "@ItemId", DbType.Int32, itemId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleItemInfo saleItem = new SaleItemInfo();
                    saleItem.ID = Convert.ToInt64(dataReader["ID"]);
                    saleItem.SaleID = Convert.ToInt32(dataReader["SaleID"]);
                    saleItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    saleItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    saleItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    saleItem.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    saleItem.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    saleItem.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    saleItem.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    saleItem.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                    saleItem.SaleDate = Convert.ToDateTime(dataReader["SaleDate"]);

                    retVal.Add(saleItem);
                }
            }
            return retVal;
        }

        public void ProcessSummary(BindingList<SaleItemInfo> saleItems, DateTime summaryDate, DateTime eDate)
        {
            using (DbConnection conn = _db.CreateConnection())
            {
                DbCommand command = null;
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    //This is wrong... check login again.
                    //Cause deleting all summary for selected date
                    //But inserting values for that item only (In case of itemwise summary)
                    command = _db.GetSqlStringCommand("DELETE FROM SaleSummary WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate");
                    _db.AddInParameter(command, "@SaleDate", DbType.DateTime, summaryDate);
                    _db.AddInParameter(command, "@ESaleDate", DbType.DateTime, eDate);

                    _db.ExecuteNonQuery(command, transaction);

                    foreach (SaleItemInfo saleItem in saleItems)
                    {
                        command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM SaleSummary WHERE SaleDate = @SaleDate AND ItemID = @ItemID");
                        _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
                        _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);

                        int cnt = Convert.ToInt32(_db.ExecuteScalar(command, transaction));
                        if (cnt == 0)
                        {
                            command = _db.GetSqlStringCommand("INSERT INTO SaleSummary(SaleDate, ItemID, Quantity, Rate, Amount, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) VALUES (@SaleDate, @ItemID, @Quantity, @Rate, @Amount, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn)");
                            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);
                            _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
                            _db.AddInParameter(command, "@Quantity", DbType.Decimal, saleItem.Quantity);
                            _db.AddInParameter(command, "@Rate", DbType.Decimal, saleItem.Rate);
                            _db.AddInParameter(command, "@Amount", DbType.Decimal, saleItem.Amount);
                            _db.AddInParameter(command, "@CreatedBy", DbType.Int32, saleItem.CreatedBy);
                            _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, saleItem.CreatedOn);
                            _db.AddInParameter(command, "@UpdatedBy", DbType.Int32, saleItem.UpdatedBy);
                            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, saleItem.UpdatedOn);

                            _db.ExecuteNonQuery(command, transaction);
                        }
                        else
                        {
                            command = _db.GetSqlStringCommand("UPDATE SaleSummary SET Quantity = Quantity + @Quantity WHERE SaleDate = @SaleDate AND ItemID = @ItemID");    //Amount updated by trigger.
                            _db.AddInParameter(command, "@Quantity", DbType.Decimal, saleItem.Quantity);
                            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);
                            _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
                            _db.ExecuteNonQuery(command, transaction);

                            command = _db.GetSqlStringCommand("UPDATE SaleSummary SET Amount = Quantity * Rate WHERE SaleDate = @SaleDate");    //Amount updated by trigger.
                            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);
                            _db.ExecuteNonQuery(command, transaction);
                        }

                        ////UnComplete
                        //command = _db.GetSqlStringCommand("INSERT INTO AllSaleItems (SaleID, ItemID, Quantity, UnitID, Vat, Rate, Amount) SELECT SaleID, ItemID, Quantity, UnitID, Vat, Rate, Amount FROM SaleItems");
                        //_db.AddInParameter(command, "@Quantity", DbType.Decimal, saleItem.Quantity);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        private BindingList<SaleItemInfo> GetNonSummarizedSaleItemsByDate(DateTime saleDate, DateTime eDate, DbTransaction trn)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();

            DbCommand command = _db.GetSqlStringCommand("SELECT Sales.BillDate, SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, Items.ItemGroupID, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount, Sales.CreatedBy, Sales.CreatedOn, Sales.UpdatedBy, Sales.UpdatedOn, Sales.BillDate SaleDate FROM SaleItems INNER JOIN Items ON Items.ID = SaleItems.ItemID INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillDate >= @SaleDate AND Sales.BillDate <= @ESaleDate AND IsSummarized = 'False'");

            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleDate);
            _db.AddInParameter(command, "@ESaleDate", DbType.DateTime, eDate);

            using (IDataReader dataReader = _db.ExecuteReader(command, trn))
            {
                while (dataReader.Read())
                {
                    SaleItemInfo saleItem = new SaleItemInfo();
                    saleItem.ID = Convert.ToInt64(dataReader["ID"]);
                    saleItem.SaleID = Convert.ToInt32(dataReader["SaleID"]);
                    saleItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    saleItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    saleItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    saleItem.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    saleItem.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    saleItem.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    saleItem.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    saleItem.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                    saleItem.SaleDate = Convert.ToDateTime(dataReader["SaleDate"]);

                    retVal.Add(saleItem);
                }
            }
            return retVal;
        }

        public void ProcessSummaryNew(DateTime summaryDate, DateTime eDate)
        {
            using (DbConnection conn = _db.CreateConnection())
            {
                DbCommand command = null;
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    BindingList<SaleItemInfo> saleItems = GetNonSummarizedSaleItemsByDate(summaryDate, eDate, transaction);

                    foreach (SaleItemInfo saleItem in saleItems)
                    {
                        command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM SaleSummary WHERE SaleDate = @SaleDate AND ItemID = @ItemID");
                        _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
                        _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);

                        int cnt = Convert.ToInt32(_db.ExecuteScalar(command, transaction));
                        if (cnt == 0)
                        {
                            command = _db.GetSqlStringCommand("INSERT INTO SaleSummary(SaleDate, ItemID, Quantity, Rate, Amount, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) VALUES (@SaleDate, @ItemID, @Quantity, @Rate, @Amount, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn)");
                            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);
                            _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
                            _db.AddInParameter(command, "@Quantity", DbType.Decimal, saleItem.Quantity);
                            _db.AddInParameter(command, "@Rate", DbType.Decimal, saleItem.Rate);
                            _db.AddInParameter(command, "@Amount", DbType.Decimal, saleItem.Amount);
                            _db.AddInParameter(command, "@CreatedBy", DbType.Int32, saleItem.CreatedBy);
                            _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, saleItem.CreatedOn);
                            _db.AddInParameter(command, "@UpdatedBy", DbType.Int32, saleItem.UpdatedBy);
                            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, saleItem.UpdatedOn);

                            _db.ExecuteNonQuery(command, transaction);
                        }
                        else
                        {
                            command = _db.GetSqlStringCommand("UPDATE SaleSummary SET Quantity = Quantity + @Quantity WHERE SaleDate = @SaleDate AND ItemID = @ItemID");    //Amount updated by trigger.
                            _db.AddInParameter(command, "@Quantity", DbType.Decimal, saleItem.Quantity);
                            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);
                            _db.AddInParameter(command, "@ItemID", DbType.Int32, saleItem.ItemID);
                            _db.ExecuteNonQuery(command, transaction);

                            command = _db.GetSqlStringCommand("UPDATE SaleSummary SET Amount = Quantity * Rate WHERE SaleDate = @SaleDate");    //Amount updated by trigger.
                            _db.AddInParameter(command, "@SaleDate", DbType.DateTime, saleItem.SaleDate);
                            _db.ExecuteNonQuery(command, transaction);
                        }

                        //Update as Summarized
                        command = _db.GetSqlStringCommand("UPDATE SaleItems SET IsSummarized = 'True' WHERE ID = @ID");
                        _db.AddInParameter(command, "@ID", DbType.Int32, saleItem.ID);
                        _db.ExecuteNonQuery(command, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public BindingList<SaleItemInfo> GetSaleItemsBySaleId(int saleId)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, SaleItems.Quantity, SaleItems.UnitID, Units.Name UnitName, SaleItems.Vat, SaleItems.Rate, SaleItems.Amount FROM SaleItems INNER JOIN Units ON SaleItems.UnitID = Units.ID WHERE SaleItems.SaleID = @SaleId");
            _db.AddInParameter(command, "@SaleId", DbType.Int32, saleId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleItemInfo saleItem = new SaleItemInfo();
                    saleItem.ID = Convert.ToInt64(dataReader["ID"]);
                    saleItem.SaleID = Convert.ToInt64(dataReader["SaleID"]);
                    saleItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    saleItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    saleItem.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    saleItem.UnitName = Convert.ToString(dataReader["UnitName"]);
                    saleItem.Vat = Convert.ToDecimal(dataReader["Vat"]);
                    saleItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    saleItem.Amount = Convert.ToDecimal(dataReader["Amount"]);

                    retVal.Add(saleItem);
                }
            }
            return retVal;
        }

        


        //Customized methods added



        public BindingList<SaleItemInfo> GetSaleItemsByCardAndCounter(string cardNumber, int counterID)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            DbCommand command = null;
            if (counterID == 0)
            {
                command = _db.GetSqlStringCommand("SELECT SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount, SaleItems.CounterID, SaleItems.CardSerialNumber, SaleItems.CreatedBy, SaleItems.CreatedOn, SaleItems.UpdatedBy, SaleItems.UpdatedOn, " +
                    "Items.Name FROM SaleItems INNER JOIN Items ON SaleItems.ItemID = Items.ID WHERE CardSerialNumber = @CardSerialNumber AND SaleID = 0 ");
            }
            else
            {
                command = _db.GetSqlStringCommand("SELECT SaleItems.ID, SaleItems.SaleID, SaleItems.ItemID, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount, SaleItems.CounterID, SaleItems.CardSerialNumber, SaleItems.CreatedBy, SaleItems.CreatedOn, SaleItems.UpdatedBy, SaleItems.UpdatedOn, " +
                    "Items.Name FROM SaleItems INNER JOIN Items ON SaleItems.ItemID = Items.ID WHERE CardSerialNumber = @CardSerialNumber AND CounterID = @CounterID AND SaleID = 0 ");
            }

            _db.AddInParameter(command, "@CardSerialNumber", DbType.String, cardNumber);
            _db.AddInParameter(command, "@CounterID", DbType.Int32, counterID);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    //SaleItemInfo saleItem = new SaleItemInfo();
                    //saleItem.ID = Convert.ToInt64(dataReader["ID"]);
                    //saleItem.SaleID = Convert.ToInt32(dataReader["SaleID"]);
                    //saleItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    //saleItem.ItemName = Convert.ToString(dataReader["Name"]);
                    //saleItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    //saleItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    //saleItem.Amount = Convert.ToDecimal(dataReader["Amount"]);
                    //saleItem.CounterID = Convert.ToInt32(dataReader["CounterID"]);
                    //saleItem.CardSerialNumber = Convert.ToString(dataReader["CardSerialNumber"]);
                    //saleItem.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    //saleItem.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    //saleItem.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    //saleItem.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    //retVal.Add(saleItem);
                }
            }
            return retVal;
        }
    }
}