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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Challan table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:07:12 PM
    /// </summary>
    public class ChallanDAL
    {
        SqlDatabase _db = DBConn.CreateDB();
        SettingDAL dal = new SettingDAL();

        public ChallanDAL()
        {

        }
        /// <summary>
        /// Gets all the Challans from the Challans table
        /// </summary>
        /// <returns>BindingList of Challans</returns>
        public BindingList<ChallanInfo> GetChallansAll()
        {
            BindingList<ChallanInfo> retVal = new BindingList<ChallanInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT ID, ChallanDate, ChallanNo, SupplierID, ReceivedDate, VehicleNo, DeliveredBy, ReceivedBy, IsConfirmed, Description, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn FROM Challans");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ChallanInfo challan = new ChallanInfo();
                    challan.ID = Convert.ToInt64(dataReader["ID"]);
                    challan.ChallanDate = Convert.ToDateTime(dataReader["ChallanDate"]);
                    challan.ChallanNo = Convert.ToString(dataReader["ChallanNo"]);
                    challan.SupplierID = Convert.ToInt32(dataReader["SupplierID"]);
                    challan.SupplierName = Convert.ToString(dataReader["SupplierName"]);
                    challan.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    if (dataReader["VehicleNo"] == DBNull.Value)
                    {
                        challan.VehicleNo = string.Empty;
                    }
                    else
                    {
                        challan.VehicleNo = Convert.ToString(dataReader["VehicleNo"]);
                    }
                    if (dataReader["DeliveredBy"] == DBNull.Value)
                    {
                        challan.DeliveredBy = string.Empty;
                    }
                    else
                    {
                        challan.DeliveredBy = Convert.ToString(dataReader["DeliveredBy"]);
                    }
                    challan.ReceivedBy = Convert.ToInt32(dataReader["ReceivedBy"]);
                    challan.IsConfirmed = Convert.ToBoolean(dataReader["IsConfirmed"]);
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        challan.Description = string.Empty;
                    }
                    else
                    {
                        challan.Description = Convert.ToString(dataReader["Description"]);
                    }
                    challan.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    challan.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    challan.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    challan.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(challan);
                }
            }
            return retVal;
        }

        public BindingList<ChallanInfo> GetChallansByFilter(int searchType, string name, object sDate, object eDate, int DivisionID, int ChallanMode)
        {
            BindingList<ChallanInfo> retVal = new BindingList<ChallanInfo>();
            DbCommand command = null;
            string cmdString="";
            if(ChallanMode == 1)
                cmdString = "SELECT Challans.ID, Challans.ChallanDate, Challans.ChallanNo, Challans.SupplierID, Suppliers.Name SupplierName, Challans.ReceivedDate, Challans.VehicleNo, Challans.DeliveredBy, Challans.ReceivedBy, Challans.IsConfirmed, Challans.Description, Challans.FromDivisionID  ,Challans.ToDivisionID,Divisions.DivisionName, Challans.CreatedBy, Challans.CreatedOn, Challans.UpdatedBy, Challans.UpdatedOn FROM Challans LEFT JOIN Suppliers ON Challans.SupplierID = Suppliers.ID  JOIN Divisions ON Challans.ToDivisionID = Divisions.ID WHERE Challans.SupplierID <> 0 AND ";
            else
                cmdString = "SELECT Challans.ID, Challans.ChallanDate, Challans.ChallanNo, Challans.SupplierID, '' AS SupplierName, Challans.ReceivedDate, Challans.VehicleNo, Challans.DeliveredBy, Challans.ReceivedBy, Challans.IsConfirmed, Challans.Description, Challans.FromDivisionID  ,Challans.ToDivisionID,Divisions.DivisionName, Challans.CreatedBy, Challans.CreatedOn, Challans.UpdatedBy, Challans.UpdatedOn FROM Challans LEFT JOIN Suppliers ON Challans.SupplierID = Suppliers.ID  JOIN Divisions ON Challans.ToDivisionID = Divisions.ID WHERE Challans.SupplierID=0 AND ";

            if (searchType == 0)
                if(DivisionID == 0)
                    command = _db.GetSqlStringCommand(cmdString + " Suppliers.Name LIKE '" + name + "%' ORDER BY Suppliers.Name");
                else
                    command = _db.GetSqlStringCommand(cmdString + " ToDivisionID = @ToDivisionID AND Suppliers.Name LIKE '" + name + "%' ORDER BY Suppliers.Name");
            if (searchType == 1)
            {
                if (ChallanMode == 1)
                {
                    if (DivisionID == 0)
                        command = _db.GetSqlStringCommand(cmdString + " Challans.ChallanDate >= @SDate AND Challans.ChallanDate <= @EDate ORDER BY Challans.ChallanDate DESC");
                    else
                        command = _db.GetSqlStringCommand(cmdString + " ToDivisionID = @ToDivisionID AND Challans.ChallanDate >= @SDate AND Challans.ChallanDate <= @EDate ORDER BY Challans.ChallanDate DESC");
                }
                else
                { command = _db.GetSqlStringCommand(cmdString + " Challans.ChallanDate >= @SDate AND Challans.ChallanDate <= @EDate ORDER BY Challans.ChallanDate DESC"); }

                _db.AddInParameter(command, "@ToDivisionID", DbType.Int16, DivisionID);
                _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            }

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ChallanInfo challan = new ChallanInfo();
                    challan.ID = Convert.ToInt64(dataReader["ID"]);
                    challan.ChallanDate = Convert.ToDateTime(dataReader["ChallanDate"]);
                    challan.ChallanNo = Convert.ToString(dataReader["ChallanNo"]);
                    challan.SupplierID = Convert.ToInt32(dataReader["SupplierID"]);
                    challan.SupplierName = Convert.ToString(dataReader["SupplierName"]);
                    challan.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    if (dataReader["VehicleNo"] == DBNull.Value)
                    {
                        challan.VehicleNo = string.Empty;
                    }
                    else
                    {
                        challan.VehicleNo = Convert.ToString(dataReader["VehicleNo"]);
                    }
                    if (dataReader["DeliveredBy"] == DBNull.Value)
                    {
                        challan.DeliveredBy = string.Empty;
                    }
                    else
                    {
                        challan.DeliveredBy = Convert.ToString(dataReader["DeliveredBy"]);
                    }
                    challan.ReceivedBy = Convert.ToInt32(dataReader["ReceivedBy"]);
                    challan.IsConfirmed = Convert.ToBoolean(dataReader["IsConfirmed"]);
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        challan.Description = string.Empty;
                    }
                    else
                    {
                        challan.Description = Convert.ToString(dataReader["Description"]);
                    }
                    challan.ToDivisionID = Convert.ToInt16(dataReader["ToDivisionID"]);
                    challan.FromDivisionID = Convert.ToInt16(dataReader["FromDivisionID"]);
                    challan.DivisionName = Convert.ToString(dataReader["DivisionName"]);
                    challan.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    challan.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    challan.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    challan.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(challan);
                }
            }
            return retVal;
        }

        public BindingList<ChallanInfo> GetChallansBySupplierID(int supplierId)
        {
            BindingList<ChallanInfo> retVal = new BindingList<ChallanInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT ID, ChallanDate, ChallanNo FROM Challans WHERE SupplierId = @SupplierId AND Completed = 'False' ORDER BY ChallanDate");
            _db.AddInParameter(command, "@SupplierId", DbType.Int32, supplierId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ChallanInfo challan = new ChallanInfo();
                    challan.ID = Convert.ToInt64(dataReader["ID"]);
                    challan.ChallanDate = Convert.ToDateTime(dataReader["ChallanDate"]);
                    challan.ChallanNo = Convert.ToString(dataReader["ChallanNo"]);

                    retVal.Add(challan);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Challan based on Id
        /// </summary>
        /// <param name="challanId">Id of the Challan the needs to be retrieved</param>
        /// <returns>Instance of Challan</returns>
        public ChallanInfo GetChallan(Int64 challanId)
        {
            ChallanInfo retVal = new ChallanInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [ChallanDate], [ChallanNo], [SupplierID], [ReceivedDate], [VehicleNo], [DeliveredBy], [ReceivedBy], [IsConfirmed], [Description], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Challans WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, challanId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.ChallanDate = Convert.ToDateTime(dataReader["ChallanDate"]);
                    retVal.ChallanNo = Convert.ToString(dataReader["ChallanNo"]);
                    retVal.SupplierID = Convert.ToInt32(dataReader["SupplierID"]);
                    retVal.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    if (dataReader["VehicleNo"] == DBNull.Value)
                    {
                        retVal.VehicleNo = string.Empty;
                    }
                    else
                    {
                        retVal.VehicleNo = Convert.ToString(dataReader["VehicleNo"]);
                    }
                    if (dataReader["DeliveredBy"] == DBNull.Value)
                    {
                        retVal.DeliveredBy = string.Empty;
                    }
                    else
                    {
                        retVal.DeliveredBy = Convert.ToString(dataReader["DeliveredBy"]);
                    }
                    retVal.ReceivedBy = Convert.ToInt32(dataReader["ReceivedBy"]);
                    retVal.IsConfirmed = Convert.ToBoolean(dataReader["IsConfirmed"]);
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
        /// Adds new Challan in to the database
        /// </summary>
        /// <param name="challan">Instance of Challan</param>
        /// <returns>Id of the newly added Challan</returns>
        public int AddChallan(ChallanInfo challan, BindingList<ChallanItemInfo> challanItems)
        {
            int retval = 0;

            DbCommand commandChallan = _db.GetSqlStringCommand("INSERT INTO Challans([ChallanDate], [ChallanNo], [SupplierID], [ReceivedDate], [VehicleNo], [DeliveredBy], [ReceivedBy], [IsConfirmed], [Description] , [FromDivisionID], [ToDivisionID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                "VALUES (@ChallanDate, @ChallanNo, @SupplierID, @ReceivedDate, @VehicleNo, @DeliveredBy, @ReceivedBy, @IsConfirmed, @Description, @FromDivisionID, @ToDivisionID, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                "SELECT IDENT_CURRENT('Challans')");

            _db.AddInParameter(commandChallan, "@ChallanDate", DbType.DateTime, challan.ChallanDate);
            _db.AddInParameter(commandChallan, "@ChallanNo", DbType.String, challan.ChallanNo);
            _db.AddInParameter(commandChallan, "@SupplierID", DbType.Int32, challan.SupplierID);
            _db.AddInParameter(commandChallan, "@ReceivedDate", DbType.DateTime, challan.ReceivedDate);
            if (challan.VehicleNo == string.Empty)
            {
                _db.AddInParameter(commandChallan, "@VehicleNo", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandChallan, "@VehicleNo", DbType.String, challan.VehicleNo);
            }
            if (challan.DeliveredBy == string.Empty)
            {
                _db.AddInParameter(commandChallan, "@DeliveredBy", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandChallan, "@DeliveredBy", DbType.String, challan.DeliveredBy);
            }
            _db.AddInParameter(commandChallan, "@ReceivedBy", DbType.Int32, challan.ReceivedBy);

            if (challan.ToDivisionID == Convert.ToInt32(dal.GetSetting(34)))
                _db.AddInParameter(commandChallan, "@IsConfirmed", DbType.Boolean, true);
            else
                _db.AddInParameter(commandChallan, "@IsConfirmed", DbType.Boolean, false);
            
            if (challan.Description == string.Empty)
            {
                _db.AddInParameter(commandChallan, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandChallan, "@Description", DbType.String, challan.Description);
            }
            _db.AddInParameter(commandChallan, "@FromDivisionID", DbType.Int32, challan.FromDivisionID);
            _db.AddInParameter(commandChallan, "@ToDivisionID", DbType.Int16, challan.ToDivisionID);
            _db.AddInParameter(commandChallan, "@CreatedBy", DbType.Byte, challan.CreatedBy);
            _db.AddInParameter(commandChallan, "@CreatedOn", DbType.DateTime, challan.CreatedOn);
            _db.AddInParameter(commandChallan, "@UpdatedBy", DbType.Byte, challan.UpdatedBy);
            _db.AddInParameter(commandChallan, "@UpdatedOn", DbType.DateTime, challan.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    retval = Convert.ToInt32(_db.ExecuteScalar(commandChallan, transaction));

                    foreach (ChallanItemInfo challanItem in challanItems)
                    {
                        DbCommand commandChallanItem = _db.GetSqlStringCommand("INSERT INTO ChallanItems(ChallanID, ItemID, Quantity, UnitID) " +
                                                        "VALUES (@ChallanID, @ItemID, @Quantity, @UnitID) ");

                        _db.AddInParameter(commandChallanItem, "@ChallanID", DbType.Int32, retval);
                        _db.AddInParameter(commandChallanItem, "@ItemID", DbType.Int32, challanItem.ItemID);
                        _db.AddInParameter(commandChallanItem, "@Quantity", DbType.Decimal, challanItem.Quantity);
                        _db.AddInParameter(commandChallanItem, "@UnitID", DbType.Int32, challanItem.UnitID);

                        _db.ExecuteScalar(commandChallanItem, transaction);
                    }

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
        /// Updates the Challan
        /// </summary>
        /// <param name="challan">Instance of Challan class</param>
        public void UpdateChallan(ChallanInfo challan, BindingList<ChallanItemInfo> challanItems)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Challans SET [ChallanDate] = @ChallanDate, [ChallanNo] = @ChallanNo, [SupplierID] = @SupplierID,  "+
                " [ReceivedDate] = @ReceivedDate,[VehicleNo] = @VehicleNo, [DeliveredBy] = @DeliveredBy, [ReceivedBy] = @ReceivedBy, [Description] = @Description, " +
                " [ToDivisionID] = @ToDivisionID, FromDivisionID=@FromDivisionID, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, challan.ID);
            _db.AddInParameter(command, "@ChallanDate", DbType.DateTime, challan.ChallanDate);
            _db.AddInParameter(command, "@ChallanNo", DbType.String, challan.ChallanNo);
            _db.AddInParameter(command, "@SupplierID", DbType.Int32, challan.SupplierID);
            _db.AddInParameter(command, "@ReceivedDate", DbType.DateTime, challan.ReceivedDate);
            if (challan.VehicleNo == string.Empty)
            {
                _db.AddInParameter(command, "@VehicleNo", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@VehicleNo", DbType.String, challan.VehicleNo);
            }
            if (challan.DeliveredBy == string.Empty)
            {
                _db.AddInParameter(command, "@DeliveredBy", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@DeliveredBy", DbType.String, challan.DeliveredBy);
            }
            _db.AddInParameter(command, "@ReceivedBy", DbType.Int32, challan.ReceivedBy);

            if (challan.Description == string.Empty)
            {
                _db.AddInParameter(command, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Description", DbType.String, challan.Description);
            }
            _db.AddInParameter(command, "@ToDivisionID", DbType.Int16, challan.ToDivisionID);
            _db.AddInParameter(command, "@FromDivisionID", DbType.Int16, challan.FromDivisionID);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, challan.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, challan.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    _db.ExecuteNonQuery(command, transaction);

                    //Deletes from ChallanItems
                    command = _db.GetSqlStringCommand("DELETE FROM ChallanItems WHERE ChallanID = @Id");
                    _db.AddInParameter(command, "@Id", DbType.Int32, challan.ID);
                    _db.ExecuteNonQuery(command, transaction);

                    //Inserts into ChallanItems
                    foreach (ChallanItemInfo challanItem in challanItems)
                    {
                        DbCommand commandChallanItem = _db.GetSqlStringCommand("INSERT INTO ChallanItems(ChallanID, ItemID, Quantity, UnitID) " +
                                                        "VALUES (@ChallanID, @ItemID, @Quantity, @UnitID) ");

                        _db.AddInParameter(commandChallanItem, "@ChallanID", DbType.Int32, challan.ID);
                        _db.AddInParameter(commandChallanItem, "@ItemID", DbType.Int32, challanItem.ItemID);
                        _db.AddInParameter(commandChallanItem, "@Quantity", DbType.Decimal, challanItem.Quantity);
                        _db.AddInParameter(commandChallanItem, "@UnitID", DbType.Int32, challanItem.UnitID);

                        _db.ExecuteScalar(commandChallanItem, transaction);
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

        /// <summary>
        /// Deletes the Challan from the database
        /// </summary>
        /// <param name="challanId">Id of the Challan that needs to be deleted</param>
        public void DeleteChallan(int challanId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Challans WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, challanId);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    _db.ExecuteNonQuery(command, transaction);

                    //Delete Challan Items

                    command = _db.GetSqlStringCommand("DELETE FROM ChallanItems WHERE ChallanId = @ChallanId");
                    
                    _db.AddInParameter(command, "@ChallanId", DbType.Int32, challanId);
                    
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

        public bool CheckChallanUsed(int challanId, out string tableName)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM InvoiceChallans WHERE ChallanID = @ChallanId");
            _db.AddInParameter(command, "@ChallanId", DbType.Int32, challanId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                {
                    tableName = "Invoice Challans";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }

        public int GetSameNameCount(string challanNo)
        {
            int retVal = 0;

            DbCommand command = _db.GetSqlStringCommand(" Select Count([ID]) From [Challans] WHERE [ChallanNo] = @ChallanNo");
            _db.AddInParameter(command, "@ChallanNo", DbType.String, challanNo);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        public int GetSameNameCountEditMode(string challanNo, int challanID)
        {
            int retVal = 0;

            DbCommand command = _db.GetSqlStringCommand(" Select Count([ID]) From [Challans] WHERE [ChallanNo] = @ChallanNo AND ID != ID");
            _db.AddInParameter(command, "@ChallanNo", DbType.String, challanNo);
            _db.AddInParameter(command, "@Id", DbType.Int32, challanID);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Confirms  the Challan
        /// </summary>
        /// <param name="challan">Object brings all the values of Challan</param>
        /// <returns></returns>
        public bool ConfirmChallan(ChallanInfo challan)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Challans SET [IsConfirmed] = @IsConfirmed WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, challan.ID);
            _db.AddInParameter(command, "@IsConfirmed", DbType.Boolean, true);           

            using (DbConnection conn = _db.CreateConnection())
            {               
                try
                {
                    conn.Open();
                    _db.ExecuteNonQuery(command);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                    throw new ApplicationException(ex.Message);
                }
            }
        }
    }
}