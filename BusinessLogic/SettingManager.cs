using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    /// <summary>
    /// Author	: Sarika
    /// Date	: 17 Jul 2009 07:53:06 PM
    /// </summary>
    public class SettingManager
    {
        /// <summary>
        /// Updates the Setting record in the database
        /// </summary>
        /// <param name="setting">Instance with properties set as per the values entered in the form</param>
        public void UpdateSetting(SettingInfo setting)
        {
            SettingDAL dal = new SettingDAL();
            dal.UpdateSetting(setting);
            dal = null;
        }

        /// <summary>
        /// Gets all the settings from the database
        /// </summary>
        /// <returns>BindingList of settings</returns>
        public BindingList<SettingInfo> GetSettingsAll()
        {
            BindingList<SettingInfo> retval = new BindingList<SettingInfo>();
            SettingDAL dal = new SettingDAL();
            retval = dal.GetSettingsAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Setting record from the database based on the SettingId
        /// </summary>
        /// <param name="settingId">Id of the Setting</param>
        /// <returns>Instance of Setting</returns>
        public String GetSetting(int settingId)
        {
            string retval = string.Empty;
            SettingDAL dal = new SettingDAL();
            retval = dal.GetSetting(settingId).ToString();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Setting based on SettingId
        /// </summary>
        /// <param name="settingId">Id of the Setting that is to be deleted</param>
        public void DeleteSetting(int settingId)
        {
            SettingDAL dal = new SettingDAL();
            dal.DeleteSetting(settingId);
            dal = null;
        }

        public int DatabaseBackup(string backupPath, string DatabaseName)
        {
            int retVal = 0;
            SettingDAL dal = new SettingDAL();
            retVal = dal.DatabaseBackup(backupPath, DatabaseName);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Adds System Start Time on each computer
        /// </summary>
        /// <returns>Id of the newly added System Start Time</returns>
        public void SaveSystemStartTime(DateTime startDate)
        {
            SettingDAL dal = new SettingDAL();
            dal.SaveSystemStartTime(startDate);
        }

        public DateTime GetStartTimeBySessionStartEndTime(DateTime startTime, DateTime endTime)
        {
            SettingDAL dal = new SettingDAL();
            return dal.GetStartTimeBySessionStartEndTime(startTime, endTime);
        }
    }
}
