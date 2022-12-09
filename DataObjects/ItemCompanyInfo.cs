using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing ItemGroup data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:57:52 PM
    /// </summary>
    public class ItemCompanyInfo
    {
        public ItemCompanyInfo()
        {
        }

        System.Int32 _iD = 0;
        System.String _name = string.Empty;
        System.String _displayName = string.Empty;
        System.Byte _displayIndex = 0;
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