using System;


namespace DataObjects
{
    public class SaleProcessingInfo
    {
        public SaleProcessingInfo()
        {
        }

        System.Int32 _iD = 0;
        System.Int32 _month = 0;
        System.Int32 _year = 0;
        System.Int32 _customerNumber = 0;
        System.Int32 _customerID = 0;
        System.String _customerName = string.Empty;
        System.Decimal _openingBalance = 0;
        System.Decimal _amount = 0;
        System.Decimal _paidAmount = 0;
        System.Decimal _closingBalance = 0;
        System.Int32 _lineNumber = 0;

       

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        public System.Int32 Month
        {
            get { return _month; }
            set { _month = value; }
        }
        public System.Int32 Year
        {
            get { return _year; }
            set { _year = value; }
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
        public System.Int32 CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }
        public System.String CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }
        public System.Decimal OpeningBalance
        {
            get { return _openingBalance; }
            set { _openingBalance = value; }
        }
        public System.Decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
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
    }

    public class SaleProcessingItemsInfo
    {
        public SaleProcessingItemsInfo()
        {
        }
        System.Int32 _iD = 0;
        System.Int32 _saleProcessingID = 0;
        System.Int32 _itemID = 0;
        System.String _details = string.Empty;
        System.Decimal _amount = 0;
        System.Decimal _itemQuantity = 0;

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        public System.Int32 SaleProcessingID
        {
            get { return _saleProcessingID; }
            set { _saleProcessingID = value; }
        }
        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }
        public System.String Details
        {
            get { return _details; }
            set { _details = value; }
        }
        public System.Decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        public System.Decimal ItemQuantity
        {
            get { return _itemQuantity; }
            set { _itemQuantity = value; }
        }
    }
}
