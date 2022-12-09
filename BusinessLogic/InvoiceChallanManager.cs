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
    /// Date	: 16 Apr 2010 03:21:49 PM
    /// </summary>
    public class InvoiceChallanManager
    {
        /// <summary>
        /// Adds a new InvoiceChallan in to the database
        /// </summary>
        /// <param name="invoicechallan">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added InvoiceChallan</returns>

        public int AddInvoiceChallan(InvoiceChallanInfo invoicechallan)
        {
            int retval = 0;
            InvoiceChallanDAL dal = new InvoiceChallanDAL();
            retval = dal.AddInvoiceChallan(invoicechallan);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the InvoiceChallan record in the database
        /// </summary>
        /// <param name="invoicechallan">Instance with properties set as per the values entered in the form</param>
        public void UpdateInvoiceChallan(InvoiceChallanInfo invoicechallan)
        {
            InvoiceChallanDAL dal = new InvoiceChallanDAL();
            dal.UpdateInvoiceChallan(invoicechallan);
            dal = null;
        }

        /// <summary>
        /// Gets all the invoicechallans from the database
        /// </summary>
        /// <returns>BindingList of invoicechallans</returns>
        public BindingList<InvoiceChallanInfo> GetInvoiceChallansAll()
        {
            BindingList<InvoiceChallanInfo> retval = new BindingList<InvoiceChallanInfo>();
            InvoiceChallanDAL dal = new InvoiceChallanDAL();
            retval = dal.GetInvoiceChallansAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets InvoiceChallan record from the database based on the InvoiceChallanId
        /// </summary>
        /// <param name="invoicechallanId">Id of the InvoiceChallan</param>
        /// <returns>Instance of InvoiceChallan</returns>
        public InvoiceChallanInfo GetInvoiceChallan(int invoicechallanId)
        {
            InvoiceChallanInfo retval = new InvoiceChallanInfo();
            InvoiceChallanDAL dal = new InvoiceChallanDAL();
            retval = dal.GetInvoiceChallan(invoicechallanId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the InvoiceChallan based on InvoiceChallanId
        /// </summary>
        /// <param name="invoicechallanId">Id of the InvoiceChallan that is to be deleted</param>
        public void DeleteInvoiceChallan(int invoicechallanId)
        {
            InvoiceChallanDAL dal = new InvoiceChallanDAL();
            dal.DeleteInvoiceChallan(invoicechallanId);
            dal = null;
        }

    }
}