using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

using DataObjects;
using System.Data.SqlClient;

namespace DataAccess
{
    public class InfoDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public InfoDAL()
        {

        }

        /// <summary>
        /// Gets single info based on Id
        /// </summary>
        /// <param name="infoId">Id of the Info the needs to be retrieved</param>
        /// <returns>Instance of Info</returns>
        public String GetInfo(int infoId)
        {
            string retVal = string.Empty;
            DbCommand command = _db.GetSqlStringCommand("SELECT [Value] FROM Info WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, infoId);

            retVal = _db.ExecuteScalar(command).ToString();

            return retVal;
        }

        public void AddInfo(int id, string value)
        {
            DbCommand commandSetting = _db.GetSqlStringCommand("UPDATE Info SET Value = @Value WHERE Id = @Id");

            _db.AddInParameter(commandSetting, "@Id", DbType.Int32, id);
            _db.AddInParameter(commandSetting, "@Value", DbType.String, value);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteScalar(commandSetting);
            }
        }
    }
}