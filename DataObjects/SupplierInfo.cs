using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Supplier data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:01:47 PM
    /// </summary>
    public class SupplierInfo
    {
        public SupplierInfo()
        {
        }

        System.Int32 _iD = 0;
        System.String _name = string.Empty;
        System.String _contactPerson = string.Empty;
        System.String _address = string.Empty;
        System.String _city = string.Empty;
        System.String _state = string.Empty;
        System.String _vatTinNo = string.Empty;
        System.String _phone = string.Empty;
        System.String _mobile = string.Empty;
        System.String _eMail = string.Empty;
        System.Decimal _balance = 0;
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

        public System.String ContactPerson
        {
            get { return _contactPerson; }
            set { _contactPerson = value; }
        }

        public System.String Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public System.String City
        {
            get { return _city; }
            set { _city = value; }
        }

        public System.String State
        {
            get { return _state; }
            set { _state = value; }
        }

        public System.String VatTinNo
        {
            get { return _vatTinNo; }
            set { _vatTinNo = value; }
        }

        public System.String Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public System.String Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        public System.String EMail
        {
            get { return _eMail; }
            set { _eMail = value; }
        }

        public System.Decimal Balance
        {
            get { return _balance; }
            set { _balance = value; }
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