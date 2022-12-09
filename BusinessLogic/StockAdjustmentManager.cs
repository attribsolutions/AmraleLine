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
    /// Date	: 16 Apr 2010 03:28:44 PM
    /// </summary>
    public class StockAdjustmentManager
    {
        /// <summary>
        /// Adds a new StockAdjustment in to the database
        /// </summary>
        /// <param name="stockadjustment">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added StockAdjustment</returns>

        public int AddStockAdjustment(StockAdjustmentInfo stockadjustment)
        {
            int retval = 0;
            StockAdjustmentDAL dal = new StockAdjustmentDAL();
            retval = dal.AddStockAdjustment(stockadjustment);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the StockAdjustment record in the database
        /// </summary>
        /// <param name="stockadjustment">Instance with properties set as per the values entered in the form</param>
        public void UpdateStockAdjustment(StockAdjustmentInfo stockadjustment)
        {
            StockAdjustmentDAL dal = new StockAdjustmentDAL();
            dal.UpdateStockAdjustment(stockadjustment);
            dal = null;
        }

        /// <summary>
        /// Gets all the stockadjustments from the database
        /// </summary>
        /// <returns>BindingList of stockadjustments</returns>
        public BindingList<StockAdjustmentInfo> GetStockAdjustmentsAll()
        {
            BindingList<StockAdjustmentInfo> retval = new BindingList<StockAdjustmentInfo>();
            StockAdjustmentDAL dal = new StockAdjustmentDAL();
            retval = dal.GetStockAdjustmentsAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets StockAdjustment record from the database based on the StockAdjustmentId
        /// </summary>
        /// <param name="stockadjustmentId">Id of the StockAdjustment</param>
        /// <returns>Instance of StockAdjustment</returns>
        public StockAdjustmentInfo GetStockAdjustment(DateTime stockDate, int itemId, int divisionId)
        {
            StockAdjustmentInfo retval = new StockAdjustmentInfo();
            StockAdjustmentDAL dal = new StockAdjustmentDAL();
            retval = dal.GetStockAdjustment(stockDate, itemId, divisionId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the StockAdjustment based on StockAdjustmentId
        /// </summary>
        /// <param name="stockadjustmentId">Id of the StockAdjustment that is to be deleted</param>
        public void DeleteStockAdjustment(Int64 stockadjustmentId)
        {
            StockAdjustmentDAL dal = new StockAdjustmentDAL();
            dal.DeleteStockAdjustment(stockadjustmentId);
            dal = null;
        }

    }
}