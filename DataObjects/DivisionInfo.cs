using System;


namespace DataObjects
{
    /// <summary>
    /// Author : Jitendra
    /// Date   : 22 July 2015
    /// </summary>
    [Serializable]
    public class DivisionInfo
    {
        public DivisionInfo()
        {
        }

        System.Int32 _divisionID = 0;
        System.String _divisionName = string.Empty;

        public System.Int32 DivisionID
        {
            get { return _divisionID; }
            set { _divisionID = value; }
        }

        public System.String DivisionName
        {
            get { return _divisionName; }
            set { _divisionName = value; }
        }
    }
}
