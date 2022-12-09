using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Counter data
    /// Author	: Kiran
    /// Date	: 01 Mar 2010 12:52:08 PM
    /// </summary>
    public class CounterInfo
    {
        public CounterInfo()
        {
        }

        System.Int32 _iD = 0;
        System.String _name = string.Empty;
        System.String _oldName = string.Empty;
        System.String _password = string.Empty;
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

        public System.String OldName
        {
            get { return _oldName; }
            set { _oldName = value; }
        }

        public System.String Password
        {
            get { return _password; }
            set { _password = value; }
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