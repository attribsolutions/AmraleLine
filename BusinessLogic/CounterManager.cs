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
    /// Date	: 01 Mar 2010 12:49:15 PM
    /// </summary>
    public class CounterManager
    {
        /// <summary>
        /// Adds a new Counter in to the database
        /// </summary>
        /// <param name="counter">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Counter</returns>

        public int AddCounter(CounterInfo counter)
        {
            int retval = 0;
            CounterDAL dal = new CounterDAL();
            retval = dal.AddCounter(counter);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Counter record in the database
        /// </summary>
        /// <param name="counter">Instance with properties set as per the values entered in the form</param>
        public void UpdateCounter(CounterInfo counter)
        {
            CounterDAL dal = new CounterDAL();
            dal.UpdateCounter(counter);
            dal = null;
        }

        /// <summary>
        /// Gets all the counters from the database
        /// </summary>
        /// <returns>BindingList of counters</returns>
        public BindingList<CounterInfo> GetCountersAll()
        {
            BindingList<CounterInfo> retval = new BindingList<CounterInfo>();
            CounterDAL dal = new CounterDAL();
            retval = dal.GetCountersAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Counter record from the database based on the CounterId
        /// </summary>
        /// <param name="counterId">Id of the Counter</param>
        /// <returns>Instance of Counter</returns>
        public CounterInfo GetCounter(int counterId)
        {
            CounterInfo retval = new CounterInfo();
            CounterDAL dal = new CounterDAL();
            retval = dal.GetCounter(counterId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Counter based on CounterId
        /// </summary>
        /// <param name="counterId">Id of the Counter that is to be deleted</param>
        public void DeleteCounter(int counterId, string counterName)
        {
            CounterDAL dal = new CounterDAL();
            dal.DeleteCounter(counterId, counterName);
            dal = null;
        }

        /// <summary>
        /// Checks if the Counter name already exists in the counters table
        /// This will work for New Mode only(while adding a new counter)
        /// </summary>
        /// <param name="counterName">Name of the Counter that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name)
        {
            int retVal = 0;
            CounterDAL dal = new CounterDAL();
            retVal = dal.GetSameNameCount(name);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the Counter name already exists in the Counters table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="counterName">Name of the Counter that needs to be checked</param>
        /// <param name="counterId">Id of the Counter that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current counter.
        /// 0 is returned if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int counterId)
        {
            int retVal = 0;
            CounterDAL dal = new CounterDAL();
            retVal = dal.GetSameNameCountForEditMode(name, counterId);
            dal = null;
            return retVal;
        }
    }
}