using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing SaleItem data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:59:16 PM
    /// </summary>
    public class SaleItemInfo
    {
        public SaleItemInfo()
        {
        }

        System.Int64 _iD = 0;
        System.Int64 _saleID = 0;
        System.Int32 _itemID = 0;
        System.Decimal _quantity = 0;
        System.Int32 _unitID = 0;
        System.Decimal _vat = 0;
        System.Decimal _rate = 0;
        System.Decimal _amount = 0;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
        System.DateTime _saleDate = DateTime.MinValue;
        System.String _unitName = string.Empty;
        System.String _customerName = string.Empty;
        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int64 SaleID
        {
            get { return _saleID; }
            set { _saleID = value; }
        }

        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
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

        public System.DateTime SaleDate
        {
            get { return _saleDate; }
            set { _saleDate = value; }
        }

        public System.String UnitName
        {
            get { return _unitName; }
            set { _unitName = value; }
        }

        public System.String CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }
    }
}