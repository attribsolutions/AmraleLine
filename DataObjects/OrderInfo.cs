using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Order data
    /// Author	: Kiran
    /// Date	: 05 Aug 2011 03:37:19 PM
    /// </summary>
    public class OrderInfo
    {
        public OrderInfo()
        {
        }

        System.Int32 _iD = 0;
        System.DateTime _orderDate = DateTime.MinValue;
        System.Int32 _customerID = 0;
        System.Int32 _billID = 0;
        System.String _customerName = string.Empty;
        System.String _contactNumber = string.Empty;
        System.DateTime _deliveryDate = DateTime.MinValue;
        System.Decimal _totalAmount = 0;
        System.Decimal _advance = 0;
        System.Decimal _deliveryPayment = 0;
        System.Boolean _isCompleted = false;
        System.String _comments = string.Empty;

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        public System.Int32 CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }

        public System.Int32 BillID
        {
            get { return _billID; }
            set { _billID = value; }
        }

        public System.String CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        public System.DateTime DeliveryDate
        {
            get { return _deliveryDate; }
            set { _deliveryDate = value; }
        }

        public System.Decimal TotalAmount
        {
            get { return _totalAmount; }
            set { _totalAmount = value; }
        }

        public System.Decimal Advance
        {
            get { return _advance; }
            set { _advance = value; }
        }

        public System.Decimal DeliveryPayment
        {
            get { return _deliveryPayment; }
            set { _deliveryPayment = value; }
        }

        public System.Boolean IsCompleted
        {
            get { return _isCompleted; }
            set { _isCompleted = value; }
        }

        public System.String Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

    }
}