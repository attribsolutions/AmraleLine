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
    public class InfoManager
    {
        /// <summary>
        /// Gets Setting record from the database based on the InfoId
        /// </summary>
        /// <param name="infoId">Id of the Info</param>
        /// <returns>Instance of Info</returns>
        public String GetInfo(int infoId)
        {
            string retval = string.Empty;
            InfoDAL dal = new InfoDAL();
            retval = dal.GetInfo(infoId).ToString();
            dal = null;
            return retval;
        }

        public void AddInfo(int id, string value)
        {
            InfoDAL dal = new InfoDAL();
            dal.AddInfo(id, value);
        }
    }
}
