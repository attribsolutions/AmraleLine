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
    /// Date	: 16 Apr 2010 03:27:17 PM
    /// </summary>
    public class SaleItemManager
    {
        /// <summary>
        /// Adds a new SaleItem in to the database
        /// </summary>
        /// <param name="saleitem">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added SaleItem</returns>

        public int AddSaleItem(SaleItemInfo saleitem)
        {
            int retval = 0;
            SaleItemDAL dal = new SaleItemDAL();
            retval = dal.AddSaleItem(saleitem);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the SaleItem record in the database
        /// </summary>
        /// <param name="saleitem">Instance with properties set as per the values entered in the form</param>
        public void UpdateSaleItem(SaleItemInfo saleitem)
        {
            SaleItemDAL dal = new SaleItemDAL();
            dal.UpdateSaleItem(saleitem);
            dal = null;
        }

        /// <summary>
        /// Gets all the saleitems from the database
        /// </summary>
        /// <returns>BindingList of saleitems</returns>
        public BindingList<SaleItemInfo> GetSaleItemsAll()
        {
            BindingList<SaleItemInfo> retval = new BindingList<SaleItemInfo>();
            SaleItemDAL dal = new SaleItemDAL();
            retval = dal.GetSaleItemsAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets SaleItem record from the database based on the SaleItemId
        /// </summary>
        /// <param name="saleitemId">Id of the SaleItem</param>
        /// <returns>Instance of SaleItem</returns>
        public SaleItemInfo GetSaleItem(int saleitemId)
        {
            SaleItemInfo retval = new SaleItemInfo();
            SaleItemDAL dal = new SaleItemDAL();
            retval = dal.GetSaleItem(saleitemId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the SaleItem based on SaleItemId
        /// </summary>
        /// <param name="saleitemId">Id of the SaleItem that is to be deleted</param>
        public void DeleteSaleItem(int saleitemId)
        {
            SaleItemDAL dal = new SaleItemDAL();
            dal.DeleteSaleItem(saleitemId);
            dal = null;
        }

        public BindingList<SaleItemInfo> GetSaleItemsByDate(DateTime saleDate, DateTime eDate, bool timeWise)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            SaleItemDAL dal = new SaleItemDAL();
            retVal = dal.GetSaleItemsByDate(saleDate, eDate, timeWise);
            return retVal;
        }

        public BindingList<SaleItemInfo> GetSaleItemsByDateItemWise(DateTime saleDate, DateTime eDate, int itemId)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            SaleItemDAL dal = new SaleItemDAL();
            retVal = dal.GetSaleItemsByDateItemWise(saleDate, eDate, itemId);
            return retVal;
        }

        public void ProcessSummary(BindingList<SaleItemInfo> saleItems, DateTime summaryDate, DateTime eDate)
        {
            SaleItemDAL dal = new SaleItemDAL();
            dal.ProcessSummary(saleItems, summaryDate, eDate);
        }

        public void ProcessSummaryNew(DateTime summaryDate, DateTime eDate)
        {
            SaleItemDAL dal = new SaleItemDAL();
            dal.ProcessSummaryNew(summaryDate, eDate);
        }

        public BindingList<SaleItemInfo> GetSaleItemsBySaleId(int saleId)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            SaleItemDAL dal = new SaleItemDAL();
            retVal = dal.GetSaleItemsBySaleId(saleId);
            return retVal;
        }




        //Customized methods added



        public BindingList<SaleItemInfo> GetSaleItemsByCardAndCounter(string cardNumber, int counterID)
        {
            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();
            SaleItemDAL dal = new SaleItemDAL();
            retVal = dal.GetSaleItemsByCardAndCounter(cardNumber, counterID);
            dal = null;
            return retVal;
        }
    }
}