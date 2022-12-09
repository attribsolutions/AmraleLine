using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Unit data
    /// Author	: Kiran
    /// Date	: 17 Apr 2010 03:16:26 PM
    /// </summary>
    public class UnitInfo
    {
        public UnitInfo()
        {
        }

        System.Int32 _iD = 0;
        System.String _name = string.Empty;

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

    }
}