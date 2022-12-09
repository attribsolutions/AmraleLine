using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class MonthlyMilkStanderdInfo
    {

        public MonthlyMilkStanderdInfo()
        {
        }

        System.Int32    _iD = 0;
        System.Int32    _customerNumber = 0;
        System.String   _name = string.Empty;
        System.Int32    _itemID = 0;
        System.Int32    _itemCode = 0;
        System.Decimal  _quantity = 0;
        System.Decimal _buffalo = 0;

        System.Decimal _cow = 0;
        System.String   _itemName = string.Empty;
        System.Int32    _lineID = 0;
        System.Int32    _linemanID = 0;
        System.Byte     _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte     _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
        System.DateTime _milkStandereddate = DateTime.MinValue;

        public System.DateTime MilkDeliveryDate
        {
            get { return _milkStandereddate; }
            set { _milkStandereddate = value; }
        }
        public System.Int32 LinemanID
        {
            get { return _linemanID; }
            set { _linemanID = value; }
        }
        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        public System.Int32 CustomerNumber
        {
            get { return _customerNumber; }
            set { _customerNumber = value; }
        }
        public System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public System.Int32 ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }

        public System.Decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
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
        public System.String ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        public System.Int32 LineID
        {
            get { return _lineID; }
            set { _lineID = value; }
        }
        public System.Decimal Cow
        {
            get { return _cow; }
            set { _cow = value; }
        }

        public System.Decimal Buffalo
        {
            get { return _buffalo; }
            set { _buffalo = value; }
        }
    }
}
