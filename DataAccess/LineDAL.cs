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
    public class LineDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public LineDAL()
        {
 
        }
        public int AddLine(LineInfo line)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand("INSERT INTO Lines (Name,LineNumber,Landmark, Address,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn) "+
                "VALUES (@Name,@LineNumber,@Landmark, @Address,@CreatedBy,@CreatedOn,@UpdatedBy,@UpdatedOn)");

            _db.AddInParameter(command, "@Name", DbType.String, line.Name);
            _db.AddInParameter(command, "@LineNumber", DbType.String, line.LineNumber);
            _db.AddInParameter(command, "@Landmark", DbType.String, line.Landmark);
            _db.AddInParameter(command, "@Address", DbType.String, line.Address);
            _db.AddInParameter(command, "@CreatedBy", DbType.Byte, line.CreatedBy);
            _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, line.CreatedOn);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, line.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, line.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        public BindingList<LineInfo> GetLinesByFilter(string lineName, int count)
        {
            BindingList<LineInfo> retVal = new BindingList<LineInfo>();
            DbCommand command = null;
            string cmdString = "SELECT TOP " + count + " ID,Name,LineNumber,Landmark, Address FROM Lines WHERE Lines.Name LIKE '%" + lineName.Trim() + "%' ORDER BY Lines.ID ";
            command = _db.GetSqlStringCommand(cmdString);
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    LineInfo line = new LineInfo();
                    line.ID = Convert.ToInt32(dataReader["ID"]);
                    line.Name = Convert.ToString(dataReader["Name"]);
                    line.LineNumber = Convert.ToString(dataReader["LineNumber"]);
                    line.Landmark = Convert.ToString(dataReader["Landmark"]);
                    line.Address = Convert.ToString(dataReader["Address"]);
                    retVal.Add(line);
                }
            }
            return retVal;
        }

        public void UpdateLine(LineInfo line)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Lines SET Name = @Name, LineNumber = @LineNumber, Landmark = @Landmark, Address = @Address, UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE ID = @ID");

            _db.AddInParameter(command, "@ID", DbType.Int32, line.ID);
            _db.AddInParameter(command, "@Name", DbType.String, line.Name);
            _db.AddInParameter(command, "@LineNumber", DbType.String, line.LineNumber);
            _db.AddInParameter(command, "@Landmark", DbType.String, line.Landmark);
            _db.AddInParameter(command, "@Address", DbType.String, line.Address);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, line.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, line.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public void DeleteLine(int lineID)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Lines WHERE ID = @ID");
            _db.AddInParameter(command, "ID", DbType.Int32, lineID);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public BindingList<LineInfo> GetLines()
        {
            BindingList<LineInfo> retval = new BindingList<LineInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT ID, Name, LineNumber FROM Lines");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    LineInfo line = new LineInfo();
                    line.ID = Convert.ToInt32(dataReader["ID"]);
                    line.Name = Convert.ToString(dataReader["Name"]);
                    line.LineNumber = Convert.ToString(dataReader["LineNumber"]);
                    retval.Add(line);
                }
            }
            return retval;
        }

        public bool CheckLineUsed(int lineID, out string tableName)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(LineID) FROM Customers WHERE LineID = @LineID");
            _db.AddInParameter(command, "@LineID", DbType.Int32, lineID);
            DbCommand command1 = _db.GetSqlStringCommand("SELECT COUNT(LineID) FROM Linemans WHERE LineID = @LineID");
            _db.AddInParameter(command1, "@LineID", DbType.Int32, lineID);

            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                {
                    tableName = "Customers";
                    return true;
                }
                if (Convert.ToInt32(_db.ExecuteScalar(command1)) > 0)
                {
                    tableName = "Linemans";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }
    }
}
