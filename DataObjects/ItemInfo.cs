using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Item data
    /// Author	: Akash
    /// Date	: 31 July 2017 03:21:23 PM
    /// </summary>
    public class ItemInfo
    {
        public ItemInfo()
        {
        }

        System.Int32 _iD = 0;
        System.Int32 _itemCode = 0;
        System.Int32 _mainBranchCode = 0;
        System.String _barCode = string.Empty;
        System.String _name = string.Empty;
        System.String _displayName = string.Empty;
        System.Int32 _displayIndex = 0;
        System.Int32 _itemGroupID = 0;
        System.Int32 _unitID = 0;
        System.Decimal _Gst = 0;
        System.Decimal _lastPurchaseRate = 0;
        System.Decimal _rate = 0;
        System.Decimal _reorderLevel = 0;
        System.Decimal _reorderQuantity = 0;
        System.Decimal _unitWeight = 0;
        System.Boolean _isActive = false;
        System.Boolean _showFixed = false;        
        System.Int32 _counter = 0;
        System.Int32 _fixedDisplayIndex = 0;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
        System.String _itemGroup = string.Empty;
        System.String _unit = string.Empty;
        System.Boolean _isroundOff = false;

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int32 ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }

        public System.Int32 MainBranchCode
        {
            get { return _mainBranchCode; }
            set { _mainBranchCode = value; }
        }

        public System.String BarCode
        {
            get { return _barCode; }
            set { _barCode = value; }
        }

        public System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public System.String DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }
        public System.Int32 DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = value; }
        }

        public System.Int32 ItemGroupID
        {
            get { return _itemGroupID; }
            set { _itemGroupID = value; }
        }

        public System.Int32 UnitID
        {
            get { return _unitID; }
            set { _unitID = value; }
        }

        public System.Decimal Gst
        {
            get { return _Gst; }
            set { _Gst = value; }
        }

        public System.Decimal LastPurchaseRate
        {
            get { return _lastPurchaseRate; }
            set { _lastPurchaseRate = value; }
        }

        public System.Decimal Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        public System.Decimal ReorderLevel
        {
            get { return _reorderLevel; }
            set { _reorderLevel = value; }
        }

        public System.Decimal ReorderQuantity
        {
            get { return _reorderQuantity; }
            set { _reorderQuantity = value; }
        }

        public System.Decimal UnitWeight
        {
            get { return _unitWeight; }
            set { _unitWeight = value; }
        }

        public System.Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public System.Boolean ShowFixed
        {
            get { return _showFixed; }
            set { _showFixed = value; }
        }

        public System.Int32 FixedDisplayIndex
        {
            get { return _fixedDisplayIndex; }
            set { _fixedDisplayIndex = value; }
        }

        public System.Int32 CounterID
        {
            get { return _counter; }
            set { _counter = value; }
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

        public System.String ItemGroup
        {
            get { return _itemGroup; }
            set { _itemGroup = value; }
        }

        public System.String Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public System.Boolean IsRoundOff
        {
            get { return _isroundOff; }
            set { _isroundOff = value; }
        }
    }
}