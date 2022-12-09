using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using DataObjects;

namespace DataAccess
{
    public class RateDAL
    {
        SqlDatabase _db = DBConn.CreateDB();
        public RateDAL()
        { }

        public BindingList<RatesInfo> GetRatesByFilter(string itemName, int count)
        {
            BindingList<RatesInfo> retval = new BindingList<RatesInfo>();
            DbCommand command = null;
            string cmdString = "SELECT TOP " + count + "Rates.ID,ItemID,Items.Name ItemName, Rates.VAT,Rates.Rate,EffectiveFrom FROM Rates JOIN Items ON Items.ID = Rates.ItemID WHERE Rates.IsLastUpdated = @IsLastUpdated AND Items.Name LIKE '%" + itemName.Trim() + "%' ORDER BY Items.Name";
            command = _db.GetSqlStringCommand(cmdString);
            _db.AddInParameter(command, "@IsLastUpdated", DbType.Boolean, true);
            using (IDataReader reader = _db.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    RatesInfo rate = new RatesInfo();
                    rate.ID = Convert.ToInt32(reader["ID"]);
                    rate.ItemID = Convert.ToInt32(reader["ItemID"]);
                    rate.ItemName = Convert.ToString(reader["ItemName"]);
                    rate.VAT = Convert.ToDecimal(reader["VAT"]);
                    rate.Rate = Convert.ToDecimal(reader["Rate"]);
                    rate.EffectiveFrom = Convert.ToDateTime(reader["EffectiveFrom"]);
                    retval.Add(rate);
                }
            }
            return retval;
        }

        public int AddRate(RatesInfo rate)
        {
            int retval = 0;
            DbCommand command = _db.GetSqlStringCommand("INSERT INTO Rates (ItemID,VAT,Rate,EffectiveFrom,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn)  VALUES (@ItemID,@VAT,@Rate,@EffectiveFrom,@CreatedBy,@CreatedOn,@UpdatedBy,@UpdatedOn)");
            _db.AddInParameter(command, "@ItemID", DbType.Int32, rate.ItemID);
            _db.AddInParameter(command, "@VAT", DbType.Decimal, rate.VAT);
            _db.AddInParameter(command, "@Rate", DbType.Decimal, rate.Rate);
            _db.AddInParameter(command, "@EffectiveFrom", DbType.Date, rate.EffectiveFrom);
            _db.AddInParameter(command, "@CreatedBy", DbType.Byte, rate.CreatedBy);
            _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, rate.CreatedOn);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, rate.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, rate.UpdatedOn);
            using (DbConnection con = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                con.Open();
                transaction = con.BeginTransaction();
                SetLastUpdatedItem(rate.ItemID, transaction);
                retval = Convert.ToInt32(_db.ExecuteScalar(command,transaction));
                transaction.Commit();
            }
            return retval;
        }

        private void SetLastUpdatedItem(int itemID, DbTransaction transaction)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Rates SET IsLastUpdated = @Value WHERE ID = (SELECT TOP 1 ID FROM Rates WHERE ItemID = @ItemID Order BY ID DESC)");
            _db.AddInParameter(command, "@Value", DbType.Boolean, false);
            _db.AddInParameter(command, "@ItemID", DbType.Int32, itemID);
            _db.ExecuteNonQuery(command, transaction);
        }

        public RatesInfo GetRateByItemCode(int itemID)
        {
            RatesInfo retval = new RatesInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT ID,ItemID,Rate,VAT FROM Rates WHERE ID = (SELECT TOP 1 ID FROM Rates WHERE ItemID = @ItemID Order BY ID DESC)");
            _db.AddInParameter(command, "@ItemID", DbType.Int32, itemID);
            using (IDataReader reader = _db.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    retval.ID = Convert.ToInt32(reader["ID"]);
                    retval.ItemID = Convert.ToInt32(reader["ItemID"]);
                    retval.Rate = Convert.ToDecimal(reader["Rate"]);
                    retval.VAT = Convert.ToDecimal(reader["VAT"]);
                }
            }
            return retval;
        }

        public void DeleteRate(int RateID)
        {
            DbCommand command = _db.GetSqlStringCommand("Update Rates SET IsLastUpdated = @IsLastUpdated WHERE ID = @ID");
            _db.AddInParameter(command, "@IsLastUpdated", DbType.Boolean, false);
            _db.AddInParameter(command, "@ID", DbType.Int32, RateID);
            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                _db.ExecuteNonQuery(command);
            }
        }

    }
}
