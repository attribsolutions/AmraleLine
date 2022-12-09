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
    public class LinemanDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public LinemanDAL()
        { }


        public int AddLineman(LinemanInfo lineman)
        {
            int retval = 0;
            DbCommand command = _db.GetSqlStringCommand("INSERT INTO Linemans (LineID,Name,Mobile,Address,City,Commission,IsActive,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn) "+
                " VALUES (@LineID,@Name,@Mobile,@Address,@City,@Commission,@IsActive,@CreatedBy,@CreatedOn,@UpdatedBy,@UpdatedOn)");

            _db.AddInParameter(command, "LineID", DbType.Int32, lineman.LineId);
            _db.AddInParameter(command, "Name", DbType.String, lineman.Name);
            _db.AddInParameter(command, "Mobile", DbType.String, lineman.Mobile);
            _db.AddInParameter(command, "Address", DbType.String, lineman.Address);
            _db.AddInParameter(command, "City", DbType.String, lineman.City);
            _db.AddInParameter(command, "Commission", DbType.Decimal, lineman.Commission);
            _db.AddInParameter(command, "IsActive", DbType.Boolean, lineman.IsActive);
            _db.AddInParameter(command, "@CreatedBy", DbType.Byte, lineman.CreatedBy);
            _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, lineman.CreatedOn);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, lineman.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, lineman.UpdatedOn);
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retval;
        }


        public void UpdateLineman(LinemanInfo lineman)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Linemans SET LineID = @LineID,Name = @Name,Mobile = @Mobile,Address = @Address,City = @City,Commission = @Commission,IsActive = @IsActive,UpdatedBy = @UpdatedBy,UpdatedOn = @UpdatedOn WHERE ID = @ID");
            _db.AddInParameter(command, "ID", DbType.Int32, lineman.ID);
            _db.AddInParameter(command, "LineID", DbType.Int32, lineman.LineId);
            _db.AddInParameter(command, "Name", DbType.String, lineman.Name);
            _db.AddInParameter(command, "Mobile", DbType.String, lineman.Mobile);
            _db.AddInParameter(command, "Address", DbType.String, lineman.Address);
            _db.AddInParameter(command, "City", DbType.String, lineman.City);
            _db.AddInParameter(command, "Commission", DbType.Decimal, lineman.Commission);
            _db.AddInParameter(command, "IsActive", DbType.Boolean, lineman.IsActive);
            _db.AddInParameter(command, "UpdatedBy", DbType.Int32, lineman.UpdatedBy);
            _db.AddInParameter(command, "UpdatedOn", DbType.DateTime, lineman.UpdatedOn);

            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public BindingList<LinemanInfo> GetLinemanByFilter(string LinemanName, int count)
        {
            BindingList<LinemanInfo> retval = new BindingList<LinemanInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT Linemans.ID,LineID,Lines.LineNumber,Linemans.Name LinemanName,Lines.Name LineName, Mobile,Linemans.Address,City,Commission,IsActive,Linemans.CreatedBy,Linemans.CreatedOn,Linemans.UpdatedBy,Linemans.UpdatedOn FROM LineMans JOIN Lines ON Lines.ID = LineID ORDER BY ID");
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    LinemanInfo lineman = new LinemanInfo();
                    lineman.ID = Convert.ToInt32(dataReader["ID"]);
                    lineman.LineId = Convert.ToInt32(dataReader["LineID"]);
                    lineman.LineNumber = Convert.ToInt32(dataReader["LineNumber"]);
                    lineman.Name = Convert.ToString(dataReader["LinemanName"]);
                    lineman.LineName = Convert.ToString(dataReader["LineName"]);
                    lineman.Mobile = Convert.ToString(dataReader["Mobile"]);
                    lineman.Address = Convert.ToString(dataReader["Address"]);
                    lineman.City = Convert.ToString(dataReader["City"]);
                    lineman.Commission = Convert.ToDecimal(dataReader["Commission"]);
                    lineman.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    //lineman.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    //lineman.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    //lineman.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    //lineman.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                    retval.Add(lineman);
                }
            }
            return retval;
        }

        public void DeleteLineman(int LinemanID)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Linemans WHERE ID = @LinemanID");
            _db.AddInParameter(command, "LinemanID", DbType.Int32, LinemanID);
            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                _db.ExecuteNonQuery(command);
            }
        }



        public BindingList<LinemanInfo> GetLinemans()
        {
            BindingList<LinemanInfo> retval = new BindingList<LinemanInfo>();
            DbCommand command = _db.GetSqlStringCommand(@"SELECT  [ID]  ,[LineID]  ,[Name]  FROM [Linemans]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    LinemanInfo lineman = new LinemanInfo();
                    lineman.ID = Convert.ToInt32(dataReader["ID"]);
                    lineman.LineId = Convert.ToInt32(dataReader["LineID"]);
                    lineman.Name = Convert.ToString(dataReader["Name"]);

                    retval.Add(lineman);
                }
            }
            return retval;
        }
    }
}
