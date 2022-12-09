using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing User data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:02:12 PM
    /// </summary>
     
    [Serializable]
    public class UserInfo
    {
        public UserInfo()
        {
        }

        System.Int32 _iD = 0;
        System.String _name = string.Empty;
        System.String _mobile = string.Empty;
        System.String _eMail = string.Empty;
        System.Boolean _isSystemUser = false;
        System.String _loginName = string.Empty;
        System.String _password = string.Empty;
        System.String _userRole = string.Empty;
        System.Boolean _isActive = false;
        System.String _cardID = string.Empty;
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

        public System.Boolean IsSystemUser
        {
            get { return _isSystemUser; }
            set { _isSystemUser = value; }
        }

        public System.String LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }

        public System.String Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public System.String UserRole
        {
            get { return _userRole; }
            set { _userRole = value; }
        }

        public System.Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public System.String CardID
        {
            get { return _cardID; }
            set { _cardID = value; }
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