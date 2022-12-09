using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace DataAccess
{
    public class DBConn
    {

        private static string _dbConnectionString = string.Empty;

        public static string DBConnectionString
        {
            get { return _dbConnectionString; }
            set { _dbConnectionString = value; }
        }

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SweetPOS.Properties.Settings.ConnectionString"].ToString(); 
        }

        public static SqlDatabase CreateDB()
        {
            _dbConnectionString = ConfigurationManager.ConnectionStrings["SweetPOS.Properties.Settings.ConnectionString"].ToString();
            if (_dbConnectionString == string.Empty)
            {
                throw new ApplicationException("Database connection not configured.");
            }
            return new SqlDatabase(_dbConnectionString);
        }
    }
}
