using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Invoice data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:57:28 PM
    /// </summary>
    public class InvoiceInfo
    {
        public InvoiceInfo()
        {
        }

        System.Int64 _iD = 0;
        System.DateTime _invoiceDate = DateTime.MinValue;
        System.String _invoiceNo = string.Empty;
        System.Int32 _supplierID = 0;
        System.String _supplierName = string.Empty;
        System.Byte _cashCredit = 0;
        System.Decimal _totalAmount = 0;
        System.Decimal _taxAmount = 0;
        System.Decimal _discountPercentage = 0;
        System.Decimal _discountAmount = 0;
        System.Decimal _netAmount = 0;
        System.Decimal _balanceAmount = 0;
        System.Decimal _otherCharges = 0;
        System.String _otherChargeDescription = string.Empty;
        System.String _deliveredBy = string.Empty;
        System.Int32 _receivedBy = 0;
        System.String _description = string.Empty;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime InvoiceDate
        {
            get { return _invoiceDate; }
            set { _invoiceDate = value; }
        }

        public System.String InvoiceNo
        {
            get { return _invoiceNo; }
            set { _invoiceNo = value; }
        }

        public System.Int32 SupplierID
        {
            get { return _supplierID; }
            set { _supplierID = value; }
        }

        public System.String SupplierName
        {
            get { return _supplierName; }
            set { _supplierName = value; }
        }

        public System.Byte CashCredit
        {
            get { return _cashCredit; }
            set { _cashCredit = value; }
        }

        public System.Decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; }
        }

        public System.Decimal TaxAmount
        {
            get { return _taxAmount; }
            set { _taxAmount = value; }
        }

        public System.Decimal DiscountPercentage
        {
            get { return _discountPercentage; }
            set { _discountPercentage = value; }
        }

        public System.Decimal DiscountAmount
        {
            get { return _discountAmount; }
            set { _discountAmount = value; }
        }

        public System.Decimal NetAmount
        {
            get { return _netAmount; }
            set { _netAmount = value; }
        }

        public System.Decimal BalanceAmount
        {
            get { return _balanceAmount; }
            set { _balanceAmount = value; }
        }

        public System.Decimal OtherCharges
        {
            get { return _otherCharges; }
            set { _otherCharges = value; }
        }

        public System.String OtherChargeDescription
        {
            get { return _otherChargeDescription; }
            set { _otherChargeDescription = value; }
        }

        public System.String DeliveredBy
        {
            get { return _deliveredBy; }
            set { _deliveredBy = value; }
        }

        public System.Int32 ReceivedBy
        {
            get { return _receivedBy; }
            set { _receivedBy = value; }
        }

        public System.String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public System.Byte CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public System.DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }

        public System.Byte UpdatedBy
        {
            get { return _updatedBy; }
            set { _updatedBy = value; }
        }

        public System.DateTime UpdatedOn
        {
            get { return _updatedOn; }
            set { _updatedOn = value; }
        }

    }
}