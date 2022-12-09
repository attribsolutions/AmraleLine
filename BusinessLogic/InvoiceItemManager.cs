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
    /// Date	: 16 Apr 2010 03:22:19 PM
    /// </summary>
    public class InvoiceItemManager
    {
        /// <summary>
        /// Adds a new InvoiceItem in to the database
        /// </summary>
        /// <param name="invoiceitem">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added InvoiceItem</returns>

        public int AddInvoiceItem(InvoiceItemInfo invoiceitem)
        {
            int retval = 0;
            InvoiceItemDAL dal = new InvoiceItemDAL();
            retval = dal.AddInvoiceItem(invoiceitem);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the InvoiceItem record in the database
        /// </summary>
        /// <param name="invoiceitem">Instance with properties set as per the values entered in the form</param>
        public void UpdateInvoiceItem(InvoiceItemInfo invoiceitem)
        {
            InvoiceItemDAL dal = new InvoiceItemDAL();
            dal.UpdateInvoiceItem(invoiceitem);
            dal = null;
        }

        /// <summary>
        /// Gets all the invoiceitems from the database
        /// </summary>
        /// <returns>BindingList of invoiceitems</returns>
        public BindingList<InvoiceItemInfo> GetInvoiceItemsAll()
        {
            BindingList<InvoiceItemInfo> retval = new BindingList<InvoiceItemInfo>();
            InvoiceItemDAL dal = new InvoiceItemDAL();
            retval = dal.GetInvoiceItemsAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets InvoiceItem record from the database based on the InvoiceItemId
        /// </summary>
        /// <param name="invoiceitemId">Id of the InvoiceItem</param>
        /// <returns>Instance of InvoiceItem</returns>
        public InvoiceItemInfo GetInvoiceItem(int invoiceitemId)
        {
            InvoiceItemInfo retval = new InvoiceItemInfo();
            InvoiceItemDAL dal = new InvoiceItemDAL();
            retval = dal.GetInvoiceItem(invoiceitemId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the InvoiceItem based on InvoiceItemId
        /// </summary>
        /// <param name="invoiceitemId">Id of the InvoiceItem that is to be deleted</param>
        public void DeleteInvoiceItem(int invoiceitemId)
        {
            InvoiceItemDAL dal = new InvoiceItemDAL();
            dal.DeleteInvoiceItem(invoiceitemId);
            dal = null;
        }

    }
}