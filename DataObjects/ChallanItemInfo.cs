using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing ChallanItem data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:54:47 PM
    /// </summary>
    public class ChallanItemInfo
    {
        public ChallanItemInfo()
        {
        }

        System.Int64 _iD = 0;
        System.Int64 _challanID = 0;
        System.Int32 _itemID = 0;
        System.Decimal _quantity = 0;
        System.Int32 _unitID = 0;
        System.String _itemName = string.Empty;
        System.String _unitName = string.Empty;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int64 ChallanID
        {
            get { return _challanID; }
            set { _challanID = value; }
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
    }
}