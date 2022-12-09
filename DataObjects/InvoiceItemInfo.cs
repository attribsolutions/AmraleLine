using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing InvoiceItem data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:57:02 PM
    /// </summary>
    public class InvoiceItemInfo
    {
        public InvoiceItemInfo()
        {
        }

        System.Int64 _iD = 0;
        System.Int64 _invoiceID = 0;
        System.Int32 _itemID = 0;
        System.String _itemName = string.Empty;
        System.Decimal _quantity = 0;
        System.Int32 _unitID = 0;
        System.Decimal _vat = 0;
        System.Decimal _rate = 0;
        System.Decimal _amount = 0;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int64 InvoiceID
        {
            get { return _invoiceID; }
            set { _invoiceID = value; }
        }

        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public System.String ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public System.Decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public System.Int32 UnitID
        {
            get { return _unitID; }
            set { _unitID = value; }
        }

        public System.Decimal Vat
        {
            get { return _vat; }
            set { _vat = value; }
        }

        public System.Decimal Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        public System.Decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

    }
}