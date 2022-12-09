using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Stock data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:00:07 PM
    /// </summary>
    public class StockInfo
    {
        public StockInfo()
        {
        }

        System.Int64 _iD = 0;
        System.DateTime _stockDate = DateTime.MinValue;
        System.Int32 _itemID = 0;
        System.String _itemName = string.Empty;
        System.Decimal _opening = 0;
        System.Decimal _challan = 0;
        System.Decimal _ibInward = 0;
        System.Decimal _ibOutward = 0;
        System.Decimal _transfer = 0;
        System.Decimal _sale = 0;
        System.Decimal _closing = 0;
        System.Boolean _adjusted = false;

        System.Int32 _divisionID = 0;
        System.Byte _createdBy = 0;
        System.Byte _createdOn = 0;
        

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime StockDate
        {
            get { return _stockDate; }
            set { _stockDate = value; }
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

        public System.Decimal Opening
        {
            get { return _opening; }
            set { _opening = value; }
        }

        public System.Decimal Challan
        {
            get { return _challan; }
            set { _challan = value; }
        }
        public System.Decimal IBInward
        {
            get { return _ibInward; }
            set { _ibInward = value; }
        }
        public System.Decimal IBOutward
        {
            get { return _ibOutward; }
            set { _ibOutward = value; }
        }        
        public System.Decimal Sale
        {
            get { return _sale; }
            set { _sale = value; }
        }

        public System.Decimal Closing
        {
            get { return _closing; }
            set { _closing = value; }
        }

        public System.Boolean Adjusted
        {
            get { return _adjusted; }
            set { _adjusted = value; }
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

        public System.Byte CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }

        

    }
}