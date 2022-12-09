using System;

namespace DataObjects
{
    public class SalePaymentInfo
    {
        public SalePaymentInfo()
        {
        }
        System.Int32 _iD = 0;
        System.Int32 _processingID = 0;        
        System.Int32 _customerID = 0;
        System.Int32 _paymentMode = 0;
        System.DateTime _paymentDate = DateTime.Now;
        System.String _customerName = string.Empty;
        System.String _paymentModeName = string.Empty;
        System.Decimal _openingBalance = 0;
        System.Decimal _paidAmount = 0;
        System.Decimal _closingBalance = 0;
        System.Decimal _adjustmentAmount = 0;
        System.String _comment = string.Empty;
        System.String _chequeNo = string.Empty;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Int32 _lineNumber = 0;
        System.Int32 _customerNumber = 0;
        System.Int32 _receiptNumber = 0;
        

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        public System.Int32 ProcessingID
        {
            get { return _processingID; }
            set { _processingID = value; }
        }
        public System.Int32 CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }
        public System.Int32 PaymentMode
        {
            get { return _paymentMode; }
            set { _paymentMode = value; }
        }

        public System.DateTime PaymentDate
        {
            get { return _paymentDate; }
            set { _paymentDate = value; }
        }
        public System.Int32 CustomerNumber
        {
            get { return _customerNumber; }
            set { _customerNumber = value; }
        }
        public System.Int32 LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }
        public System.String CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }
        public System.String PaymentModeName
        {
            get { return _paymentModeName; }
            set { _paymentModeName = value; }
        }

        public System.Decimal OpeningBalance
        {
            get { return _openingBalance; }
            set { _openingBalance = value; }
        }
        public System.Decimal PaidAmount
        {
            get { return _paidAmount; }
            set { _paidAmount = value; }
        }
        public System.Decimal ClosingBalance
        {
            get { return _closingBalance; }
            set { _closingBalance = value; }
        }
        public System.Decimal AdjustmentAmount
        {
            get { return _adjustmentAmount; }
            set { _adjustmentAmount = value; }
        }
        public System.String Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }
        public System.String ChequeNo
        {
            get { return _chequeNo; }
            set { _chequeNo = value; }
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
        public System.Int32 ReceiptNo
        {
            get { return _receiptNumber; }
            set { _receiptNumber = value; }
        }
       
    }
}
