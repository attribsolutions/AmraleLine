using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    /// <summary>
    /// Author	: Kiran
    /// Date	: 17 Apr 2010 03:17:39 PM
    /// </summary>
    public class UnitManager
    {
        /// <summary>
        /// Adds a new Unit in to the database
        /// </summary>
        /// <param name="unit">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Unit</returns>

        public int AddUnit(UnitInfo unit)
        {
            int retval = 0;
            UnitDAL dal = new UnitDAL();
            retval = dal.AddUnit(unit);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Unit record in the database
        /// </summary>
        /// <param name="unit">Instance with properties set as per the values entered in the form</param>
        public void UpdateUnit(UnitInfo unit)
        {
            UnitDAL dal = new UnitDAL();
            dal.UpdateUnit(unit);
            dal = null;
        }

        /// <summary>
        /// Gets all the units from the database
        /// </summary>
        /// <returns>BindingList of units</returns>
        public BindingList<UnitInfo> GetUnitsAll()
        {
            BindingList<UnitInfo> retval = new BindingList<UnitInfo>();
            UnitDAL dal = new UnitDAL();
            retval = dal.GetUnitsAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Unit record from the database based on the UnitId
        /// </summary>
        /// <param name="unitId">Id of the Unit</param>
        /// <returns>Instance of Unit</returns>
        public UnitInfo GetUnit(int unitId)
        {
            UnitInfo retval = new UnitInfo();
            UnitDAL dal = new UnitDAL();
            retval = dal.GetUnit(unitId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Unit based on UnitId
        /// </summary>
        /// <param name="unitId">Id of the Unit that is to be deleted</param>
        public void DeleteUnit(int unitId)
        {
            UnitDAL dal = new UnitDAL();
            dal.DeleteUnit(unitId);
            dal = null;
        }

    }
}