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
    /// Date	: 16 Apr 2010 03:27:50 PM
    /// </summary>
    public class SaleManager
    {
        /// <summary>
        /// Adds a new Sale in to the database
        /// </summary>
        /// <param name="sale">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Sale</returns>

        public int AddSale(SaleInfo sale, BindingList<SaleItemInfo> saleItems, string counterName, out Int64 billNumber)
        {
            int retval = 0;
            SaleDAL dal = new SaleDAL();
            retval = dal.AddSale(sale, saleItems, counterName, out billNumber);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Sale record in the database
        /// </summary>
        /// <param name="sale">Instance with properties set as per the values entered in the form</param>
        public void UpdateSale(SaleInfo sale, BindingList<SaleItemInfo> saleItems)
        {
            SaleDAL dal = new SaleDAL();
            dal.UpdateSale(sale, saleItems);
            dal = null;
        }

        /// <summary>
        /// Gets all the sales from the database
        /// </summary>
        /// <returns>BindingList of sales</returns>
        public BindingList<SaleInfo> GetSalesAll()
        {
            BindingList<SaleInfo> retval = new BindingList<SaleInfo>();
            SaleDAL dal = new SaleDAL();
            retval = dal.GetSalesAll();
            dal = null;
            return retval;
        }

        public BindingList<SaleInfo> GetSalesByFilter(int searchType, string name, object sDate, object eDate, int divisionID, int SaleType)
        {
            BindingList<SaleInfo> retval = new BindingList<SaleInfo>();
            SaleDAL dal = new SaleDAL();
            retval = dal.GetSalesByFilter(searchType, name, sDate, eDate, divisionID,SaleType);
            dal = null;
            return retval;
        }

        public BindingList<SaleInfo> GetSalesByFilterForPrintCardDetails()
        {
            SaleDAL dal = new SaleDAL();
            return dal.GetSalesByFilterForPrintCardDetails();
        }

        public void UpdateSalePrint(int billNumber)
        {
            SaleDAL dal = new SaleDAL();
            dal.UpdateSalePrint(billNumber);
        }

        /// <summary>
        /// Gets Sale record from the database based on the SaleId
        /// </summary>
        /// <param name="saleId">Id of the Sale</param>
        /// <returns>Instance of Sale</returns>
        public SaleInfo GetSale(int saleId)
        {
            SaleInfo retval = new SaleInfo();
            SaleDAL dal = new SaleDAL();
            retval = dal.GetSale(saleId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Sale based on SaleId
        /// </summary>
        /// <param name="saleId">Id of the Sale that is to be deleted</param>
        public void DeleteSale(int saleId, bool deleteAll)
        {
            SaleDAL dal = new SaleDAL();
            dal.DeleteSale(saleId, deleteAll);
            dal = null;
        }

        public int GetNextBillNumber()
        {
            int retVal = 0;
            SaleDAL dal = new SaleDAL();
            retVal = dal.GetNextBillNumber();
            dal = null;
            return retVal;
        }

        public bool CheckSaleUsed(int saleId, out string tableName)
        {
            SaleDAL dal = new SaleDAL();
            return dal.CheckSaleUsed(saleId, out tableName);
        }

        public bool UpdateCardPaymentDetails(int saleId, string cardPaymentDetails)
        {
            SaleDAL dal = new SaleDAL();
            return dal.UpdateCardPaymentDetails(saleId, cardPaymentDetails);
        }
    }
}