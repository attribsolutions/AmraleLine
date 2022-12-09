using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Setting data
    /// Author	: Sarika
    /// Date	: 17 Jul 2009 07:52:52 PM
    /// </summary>
    public class SettingInfo
    {
        public SettingInfo()
        {
        }

        System.Int32 _iD = 0;
        System.String _value = string.Empty;

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.String Value
        {
            get { return _value; }
            set { _value = value; }
        }

    }
}

