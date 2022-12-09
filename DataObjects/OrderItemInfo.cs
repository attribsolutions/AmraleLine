using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing OrderItem data
    /// Author	: Kiran
    /// Date	: 05 Aug 2011 03:40:43 PM
    /// </summary>
    public class OrderItemInfo
    {
        public OrderItemInfo()
        {
        }

        System.Int32 _iD = 0;
        System.Int32 _orderID = 0;
        System.Int32 _itemID = 0;
        System.Int32 _unitID = 0;
        System.String _itemName = string.Empty;
        System.String _unitName = string.Empty;
        System.Decimal _quantity = 0;
        System.Decimal _rate = 0;
        System.Decimal _vat = 0;
        System.Decimal _amount = 0;

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int32 OrderID
        {
            get { return _orderID; }
            set { _orderID = value; }
        }

        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public System.Int32 UnitID
        {
            get { return _unitID; }
            set { _unitID = value; }
        }

        public System.String ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public System.String UnitName
        {
            get { return _unitName; }
            set { _unitName = value; }
        }

        public System.Decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public System.Decimal Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        public System.Decimal Vat
        {
            get { return _vat; }
            set { _vat = value; }
        }

        public System.Decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

    }
}