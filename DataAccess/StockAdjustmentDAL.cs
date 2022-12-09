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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the StockAdjustment table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:18:16 PM
    /// </summary>
    public class StockAdjustmentDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public StockAdjustmentDAL()
        {

        }
        /// <summary>
        /// Gets all the StockAdjustments from the StockAdjustments table
        /// </summary>
        /// <returns>BindingList of StockAdjustments</returns>
        public BindingList<StockAdjustmentInfo> GetStockAdjustmentsAll()
        {
            BindingList<StockAdjustmentInfo> retVal = new BindingList<StockAdjustmentInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [AdjustmentDate], [ItemID], [SystemQty], [AdjustedQty], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM StockAdjustments ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    StockAdjustmentInfo stockAdjustment = new StockAdjustmentInfo();
                    stockAdjustment.ID = Convert.ToInt64(dataReader["ID"]);
                    stockAdjustment.AdjustmentDate = Convert.ToDateTime(dataReader["AdjustmentDate"]);
                    stockAdjustment.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    stockAdjustment.SystemQty = Convert.ToDecimal(dataReader["SystemQty"]);
                    stockAdjustment.AdjustedQty = Convert.ToDecimal(dataReader["AdjustedQty"]);
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        stockAdjustment.Description = string.Empty;
                    }
                    else
                    {
                        stockAdjustment.Description = Convert.ToString(dataReader["Description"]);
                    }
                    stockAdjustment.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    stockAdjustment.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    stockAdjustment.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    stockAdjustment.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(stockAdjustment);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single StockAdjustment based on Id
        /// </summary>
        /// <param name="stockadjustmentId">Id of the StockAdjustment the needs to be retrieved</param>
        /// <returns>Instance of StockAdjustment</returns>
        public StockAdjustmentInfo GetStockAdjustment(DateTime stockDate, int itemId, int divisionId)
        {
            StockAdjustmentInfo retVal = new StockAdjustmentInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [AdjustmentDate], [ItemID], [SystemQty], [AdjustedQty], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM StockAdjustments WHERE AdjustmentDate = @StockDate AND ItemId = @ItemId AND DivisionID = @DivisionID");
            _db.AddInParameter(command, "@StockDate", DbType.DateTime, stockDate);
            _db.AddInParameter(command, "@ItemId", DbType.Int32, itemId);
            _db.AddInParameter(command, "@divisionId", DbType.Int32, divisionId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.AdjustmentDate = Convert.ToDateTime(dataReader["AdjustmentDate"]);
                    retVal.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    retVal.SystemQty = Convert.ToDecimal(dataReader["SystemQty"]);
                    retVal.AdjustedQty = Convert.ToDecimal(dataReader["AdjustedQty"]);
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        retVal.Description = string.Empty;
                    }
                    else
                    {
                        retVal.Description = Convert.ToString(dataReader["Description"]);
                    }
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new StockAdjustment in to the database
        /// </summary>
        /// <param name="stockadjustment">Instance of StockAdjustment</param>
        /// <returns>Id of the newly added StockAdjustment</returns>
        public int AddStockAdjustment(StockAdjustmentInfo stockAdjustment)
        {
            int retval = 0;
            DbCommand commandStockAdjustment = _db.GetSqlStringCommand("INSERT INTO StockAdjustments([AdjustmentDate], [ItemID], [SystemQty], [AdjustedQty], [Description], [DivisionID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@AdjustmentDate, @ItemID, @SystemQty, @AdjustedQty,  @Description, @DivisionID, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('StockAdjustments')");

            _db.AddInParameter(commandStockAdjustment, "@AdjustmentDate", DbType.DateTime, stockAdjustment.AdjustmentDate);
            _db.AddInParameter(commandStockAdjustment, "@ItemID", DbType.Int32, stockAdjustment.ItemID);
            _db.AddInParameter(commandStockAdjustment, "@SystemQty", DbType.Decimal, stockAdjustment.SystemQty);
            _db.AddInParameter(commandStockAdjustment, "@AdjustedQty", DbType.Decimal, stockAdjustment.AdjustedQty);
            if (stockAdjustment.Description == string.Empty)
            {
                _db.AddInParameter(commandStockAdjustment, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandStockAdjustment, "@Description", DbType.String, stockAdjustment.Description);
            }
            _db.AddInParameter(commandStockAdjustment, "@DivisionID", DbType.Byte, stockAdjustment.DivisionID);
            _db.AddInParameter(commandStockAdjustment, "@CreatedBy", DbType.Byte, stockAdjustment.CreatedBy);
            _db.AddInParameter(commandStockAdjustment, "@CreatedOn", DbType.DateTime, stockAdjustment.CreatedOn);
            _db.AddInParameter(commandStockAdjustment, "@UpdatedBy", DbType.Byte, stockAdjustment.UpdatedBy);
            _db.AddInParameter(commandStockAdjustment, "@UpdatedOn", DbType.DateTime, stockAdjustment.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    retval = Convert.ToInt32(_db.ExecuteScalar(commandStockAdjustment, transaction));

                    DbCommand command = _db.GetSqlStringCommand("UPDATE Stock SET Closing = @Closing, Adjusted = 'True' WHERE StockDate = @StockDate AND ItemID = @ItemID AND DivisionID = @DivisionID ");

                    _db.AddInParameter(command, "@Closing", DbType.Decimal, stockAdjustment.AdjustedQty);
                    _db.AddInParameter(command, "@StockDate", DbType.DateTime, stockAdjustment.AdjustmentDate);
                    _db.AddInParameter(command, "@ItemID", DbType.Int32, stockAdjustment.ItemID);
                    _db.AddInParameter(command, "@DivisionID", DbType.Int32, stockAdjustment.DivisionID);

                    _db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            return retval;
        }

        /// <summary>
        /// Updates the StockAdjustment
        /// </summary>
        /// <param name="stockadjustment">Instance of StockAdjustment class</param>
        public void UpdateStockAdjustment(StockAdjustmentInfo stockAdjustment)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE StockAdjustments SET [AdjustedQty] = @AdjustedQty, [Description] = @Description, [DivisionID] = @DivisionID, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, stockAdjustment.ID);
            _db.AddInParameter(command, "@AdjustedQty", DbType.Decimal, stockAdjustment.AdjustedQty);
            if (stockAdjustment.Description == string.Empty)
            {
                _db.AddInParameter(command, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Description", DbType.String, stockAdjustment.Description);
            }
            _db.AddInParameter(command, "@DivisionID", DbType.Int32, stockAdjustment.DivisionID);

            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, stockAdjustment.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, stockAdjustment.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    _db.ExecuteNonQuery(command, transaction);

                    command = _db.GetSqlStringCommand("UPDATE Stock SET Closing = @Closing, Adjusted = 'True' WHERE StockDate = @StockDate AND ItemID = @ItemID AND DivisionID = @DivisionID");

                    _db.AddInParameter(command, "@Closing", DbType.Decimal, stockAdjustment.AdjustedQty);
                    _db.AddInParameter(command, "@StockDate", DbType.DateTime, stockAdjustment.AdjustmentDate);
                    _db.AddInParameter(command, "@ItemID", DbType.Int32, stockAdjustment.ItemID);
                    _db.AddInParameter(command, "@DivisionID", DbType.Int32, stockAdjustment.DivisionID);

                    _db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Deletes the StockAdjustment from the database
        /// </summary>
        /// <param name="stockadjustmentId">Id of the StockAdjustment that needs to be deleted</param>
        public void DeleteStockAdjustment(Int64 stockadjustmentId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM StockAdjustments WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, stockadjustmentId);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);

                    command = _db.GetSqlStringCommand("UPDATE Stock SET Closing = Opening + Challan - Sale, Adjusted = 'False'");
                    _db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }
    }
}