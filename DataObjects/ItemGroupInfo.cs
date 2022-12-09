using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing ItemGroup data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:57:52 PM
    /// </summary>
    public class ItemGroupInfo
    {
        public ItemGroupInfo()
        {
        }

        System.Int32 _iD = 0;
        System.Int32 _itemCompanyID = 0;
        System.String _itemCompanyName = string.Empty;
        System.String _name = string.Empty;
        System.String _displayName = string.Empty;
        System.Byte _displayIndex = 0;
        System.Boolean _couponGroup = false;
        System.Boolean _isActive = false;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int32 ItemCompanyID
        {
            get { return _itemCompanyID; }
            set { _itemCompanyID = value; }
        }

        public System.String ItemCompanyName
        {
            get { return _itemCompanyName; }
            set { _itemCompanyName = value; }
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

        public System.Byte DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = value; }
        }

        public System.Boolean CouponGroup
        {
            get { return _couponGroup; }
            set { _couponGroup = value; }
        }

        public System.Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
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