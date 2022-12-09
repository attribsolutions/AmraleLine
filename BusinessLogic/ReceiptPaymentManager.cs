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
    /// Date	: 16 Apr 2010 03:26:50 PM
    /// </summary>
    public class ReceiptPaymentManager
    {
        /// <summary>
        /// Adds a new ReceiptPayment in to the database
        /// </summary>
        /// <param name="receiptpayment">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added ReceiptPayment</returns>

        public int AddReceiptPayment(ReceiptPaymentInfo receiptpayment)
        {
            int retval = 0;
            ReceiptPaymentDAL dal = new ReceiptPaymentDAL();
            retval = dal.AddReceiptPayment(receiptpayment);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the ReceiptPayment record in the database
        /// </summary>
        /// <param name="receiptpayment">Instance with properties set as per the values entered in the form</param>
        public void UpdateReceiptPayment(ReceiptPaymentInfo receiptpayment, decimal oldAmount)
        {
            ReceiptPaymentDAL dal = new ReceiptPaymentDAL();
            dal.UpdateReceiptPayment(receiptpayment, oldAmount);
            dal = null;
        }

        /// <summary>
        /// Gets all the receiptpayments from the database
        /// </summary>
        /// <returns>BindingList of receiptpayments</returns>
        public BindingList<ReceiptPaymentInfo> GetReceiptPaymentsAll()
        {
            BindingList<ReceiptPaymentInfo> retval = new BindingList<ReceiptPaymentInfo>();
            ReceiptPaymentDAL dal = new ReceiptPaymentDAL();
            retval = dal.GetReceiptPaymentsAll();
            dal = null;
            return retval;
        }

        public BindingList<ReceiptPaymentInfo> GetReceiptPaymentsByFilter(int searchType, string name, object sDate, object eDate)
        {
            BindingList<ReceiptPaymentInfo> retval = new BindingList<ReceiptPaymentInfo>();
            ReceiptPaymentDAL dal = new ReceiptPaymentDAL();
            retval = dal.GetReceiptPaymentsByFilter(searchType, name, sDate, eDate);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets ReceiptPayment record from the database based on the ReceiptPaymentId
        /// </summary>
        /// <param name="receiptpaymentId">Id of the ReceiptPayment</param>
        /// <returns>Instance of ReceiptPayment</returns>
        public ReceiptPaymentInfo GetReceiptPayment(int receiptpaymentId)
        {
            ReceiptPaymentInfo retval = new ReceiptPaymentInfo();
            ReceiptPaymentDAL dal = new ReceiptPaymentDAL();
            retval = dal.GetReceiptPayment(receiptpaymentId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the ReceiptPayment based on ReceiptPaymentId
        /// </summary>
        /// <param name="receiptpaymentId">Id of the ReceiptPayment that is to be deleted</param>
        public void DeleteReceiptPayment(ReceiptPaymentInfo receiptPayment)
        {
            ReceiptPaymentDAL dal = new ReceiptPaymentDAL();
            dal.DeleteReceiptPayment(receiptPayment);
            dal = null;
        }

    }
}