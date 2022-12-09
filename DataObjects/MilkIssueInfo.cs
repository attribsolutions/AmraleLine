using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class MilkIssueInfo
    {
        public MilkIssueInfo()
        { }
        System.Int32 _id = 0;
        public System.Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }
        System.DateTime _milkIssueDate = DateTime.MinValue;
        public System.DateTime MilkIssueDate
        {
            get { return _milkIssueDate; }
            set { _milkIssueDate = value; }
        }

        System.String _lineID = string.Empty;
        public System.String LineID
        {
            get { return _lineID; }
            set { _lineID = value; }
        }

        System.String _name = string.Empty;
        public System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        System.String _lineName = string.Empty;
        public System.String LineName
        {
            get { return _lineName; }
            set { _lineName = value; }
        }

        System.String _mobile = string.Empty;
        public System.String Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        System.String _address = string.Empty;
        public System.String Address
        {
            get { return _address; }
            set { _address = value; }
        }

        System.String _city = string.Empty;
        public System.String City
        {
            get { return _city; }
            set { _city = value; }
        }

        System.Decimal _commission = 0;
        public System.Decimal Commission
        {
            get { return _commission; }
            set { _commission = value; }
        }

        System.Byte _createdBy = 0;
        public System.Byte CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        System.Boolean _isActive = false;
        public System.Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        System.DateTime _createdOn = DateTime.MinValue;
        public System.DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; }
        }

        System.Byte _updatedBy = 0;
        public System.Byte UpdatedBy
        {
            get { return _updatedBy; }
            set { _updatedBy = value; }
        }

        System.DateTime _updatedOn = DateTime.MinValue;
        public System.DateTime UpdatedOn
        {
            get { return _updatedOn; }
            set { _updatedOn = value; }
        }
     

        System.Int32 _totalItems = 0;
        public System.Int32 TotalItems
        {
            get { return _totalItems; }
            set { _totalItems = value; }
        }
    }

    public class MilkIssueItemInfo
    {


        System.Int32 _iD = 0;
        System.Int32 _lineManID = 0;
        System.Int32 _itemCode = 0;
        System.Decimal _quantity = 0;
        System.Decimal _returnquantity = 0;
        
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
        System.String _itemGroup = string.Empty;
        System.String _unit = string.Empty;

        System.DateTime _fromDate = DateTime.MinValue;
        public System.DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; }
        }

        System.DateTime _toDate = DateTime.MinValue;
        public System.DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; }
        }

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int32 LineManID
        {
            get { return _lineManID; }
            set { _lineManID = value; }
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

        public System.Decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public System.Decimal ReturnQuantity
        {
            get { return _returnquantity; }
            set { _returnquantity = value; }
        }
      
    }
}
