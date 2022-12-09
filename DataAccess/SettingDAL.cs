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
    /// <summary>
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Setting table
    /// Author	: Sarika
    /// Date	: 17 Jul 2009 07:52:26 PM
    /// </summary>
    public class SettingDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public SettingDAL()
        {

        }
        /// <summary>
        /// Gets all the Settings from the Settings table
        /// </summary>
        /// <returns>BindingList of Settings</returns>
        public BindingList<SettingInfo> GetSettingsAll()
        {
            BindingList<SettingInfo> retVal = new BindingList<SettingInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Value] FROM Settings ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SettingInfo setting = new SettingInfo();
                    setting.ID = Convert.ToInt32(dataReader["ID"]);
                    setting.Value = Convert.ToString(dataReader["Value"]);

                    retVal.Add(setting);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Setting based on Id
        /// </summary>
        /// <param name="settingId">Id of the Setting the needs to be retrieved</param>
        /// <returns>Instance of Setting</returns>
        public String GetSetting(int settingId)
        {
            string retVal = string.Empty;
            DbCommand command = _db.GetSqlStringCommand("SELECT [Value] FROM Settings WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, settingId);

            retVal = _db.ExecuteScalar(command).ToString();

            return retVal;
        }

        /// <summary>
        /// Updates the Setting
        /// </summary>
        /// <param name="setting">Instance of Setting class</param>
        public void UpdateSetting(SettingInfo setting)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Settings SET  [Value] = @Value WHERE Id = @ID ");
            _db.AddInParameter(command, "@ID", DbType.Int32, setting.ID);
            _db.AddInParameter(command, "@Value", DbType.String, setting.Value);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the Setting from the database
        /// </summary>
        /// <param name="settingId">Id of the Setting that needs to be deleted</param>
        public void DeleteSetting(int settingId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Settings WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, settingId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public int DatabaseBackup(string backupPath,string DatabaseName)
        {
            int retVal = 0;

            SqlConnection conn = new SqlConnection(DBConn.DBConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"Backup Database " + DatabaseName + " To Disk = '" + backupPath + "' WITH INIT";
            try
            {
                conn.Open();
                cmd.Connection = conn;
                retVal = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return retVal;
        }


        /// <summary>
        /// Adds System Start Time on each computer
        /// </summary>
        /// <returns>Id of the newly added System Start Time</returns>
        public void SaveSystemStartTime(DateTime startDate)
        {
            DbCommand commandSetting = _db.GetSqlStringCommand("INSERT INTO SystemStartTime(StartDateTime) VALUES (@StartDateTime) ");
            _db.AddInParameter(commandSetting, "@StartDateTime", DbType.DateTime, startDate);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteScalar(commandSetting);
            }
        }

        public DateTime GetStartTimeBySessionStartEndTime(DateTime startTime, DateTime endTime)
        {
            DateTime retval = DateTime.MinValue;
            DbCommand commandSetting = _db.GetSqlStringCommand("SELECT ISNULL(MIN(StartDateTime), GETDate()) FROM SystemStartTime WHERE StartDateTime >= @StartTime AND StartDateTime <= @Endtime");
            _db.AddInParameter(commandSetting, "@StartTime", DbType.DateTime, startTime);
            _db.AddInParameter(commandSetting, "@EndTime", DbType.DateTime, endTime);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToDateTime(_db.ExecuteScalar(commandSetting));
            }
            return retval;
        }
    }
}