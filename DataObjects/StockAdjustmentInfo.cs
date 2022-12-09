using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing StockAdjustment data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:01:20 PM
    /// </summary>
    public class StockAdjustmentInfo
    {
        public StockAdjustmentInfo()
        {
        }

        System.Int64 _iD = 0;
        System.DateTime _adjustmentDate = DateTime.MinValue;
        System.Int32 _itemID = 0;
        System.Decimal _systemQty = 0;
        System.Decimal _adjustedQty = 0;
        System.String _description = string.Empty;
        System.Int32 _divisionID = 0;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime AdjustmentDate
        {
            get { return _adjustmentDate; }
            set { _adjustmentDate = value; }
        }

        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public System.Decimal SystemQty
        {
            get { return _systemQty; }
            set { _systemQty = value; }
        }

        public System.Decimal AdjustedQty
        {
            get { return _adjustedQty; }
            set { _adjustedQty = value; }
        }

        public System.String Description
        {
            get { return _description; }
            set { _description = value; }
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

    }
}