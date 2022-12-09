using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Sale data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:59:39 PM
    /// </summary>
    public class SaleInfo
    {
        public SaleInfo()
        {
        }

        System.Boolean _select = false;
        System.Int64 _iD = 0;
        System.DateTime _billDate = DateTime.MinValue;
        System.String _billNo = string.Empty;
        System.Int32 _cashCredit = 0;
        System.Int32 _customerID = 0;
        System.Int32 _lineID = 0;
        System.Int32 _customerNumber = 0;
        System.Decimal _totalAmount = 0;
        System.Decimal _discountPercentage = 0;
        System.Decimal _discountAmount = 0;
        System.Decimal _netAmount = 0;
        System.Decimal _roundedAmount = 0;
        System.Decimal _balanceAmount = 0;
        System.String _description = string.Empty;
        System.Boolean _rfidTransaction = false;
        System.String _cardPayDetails = string.Empty;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
        System.String _customerName = string.Empty;
        System.Boolean _isPrint = true;
        System.Boolean _iscouponSale = false;
        System.Decimal _totalWeight = 0;
        System.Decimal _actualWeight = 0;
        System.Boolean _billFromOrder = false;
        System.Int32 _orderID = 0;
        System.String _userName = string.Empty;
        System.Int32 _divisionID = 0;
        System.Boolean _isProcessed = false;

        public System.Boolean Select
        {
            get { return _select; }
            set { _select = value; }
        }

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime BillDate
        {
            get { return _billDate; }
            set { _billDate = value; }
        }

        public System.String BillNo
        {
            get { return _billNo; }
            set { _billNo = value; }
        }

        public System.Int32 CashCredit
        {
            get { return _cashCredit; }
            set { _cashCredit = value; }
        }

        public System.Int32 CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }

        public System.Int32 LineNumber
        {
            get { return _lineID; }
            set { _lineID = value; }
        }
        public System.Int32 CustomerNumber
        {
            get { return _customerNumber; }
            set { _customerNumber = value; }
        }
        public System.String CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        public System.Decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; }
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

        public System.Decimal RoundedAmount
        {
            get { return _roundedAmount; }
            set { _roundedAmount = value; }
        }

        public System.Decimal BalanceAmount
        {
            get { return _balanceAmount; }
            set { _balanceAmount = value; }
        }

        public System.String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public System.Boolean RFIDTransaction
        {
            get { return _rfidTransaction; }
            set { _rfidTransaction = value; }
        }

        public System.String CardPaymentDetails
        {
            get { return _cardPayDetails; }
            set { _cardPayDetails = value; }
        }

        public System.Int32 DivisionID
        {
            get { return _divisionID; }
            set { _divisionID = value; }
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

        public System.Boolean IsPrint
        {
            get { return _isPrint; }
            set { _isPrint = value; }
        }

        public System.Boolean IsCouponSale
        {
            get { return _iscouponSale; }
            set { _iscouponSale = value; }
        }

        public System.Decimal TotalWeight
        {
            get { return _totalWeight; }
            set { _totalWeight = value; }
        }

        public System.Decimal ActualWeight
        {
            get { return _actualWeight; }
            set { _actualWeight = value; }
        }

        public System.Boolean BillFromOrder
        {
            get { return _billFromOrder; }
            set { _billFromOrder = value; }
        }

        public System.Int32 OrderID
        {
            get { return _orderID; }
            set { _orderID = value; }
        }

        public System.String UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public System.Boolean IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }

    }
}