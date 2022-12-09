using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    /// <summary>
    /// purpose : class used to stored  Line data
    /// </summary>
    public class LineInfo
    {
        public LineInfo()
        { }

        System.Int32 _id = 0;
        public System.Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }

        System.String _lineNumber = string.Empty;
        public System.String LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        System.String _name = string.Empty;
        public System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        System.String _landmark = string.Empty;
        public System.String Landmark
        {
            get { return _landmark; }
            set { _landmark = value; }
        }

        System.String _address = string.Empty;
        public System.String Address
        {
            get { return _address; }
            set { _address = value; }
        }

        System.Byte _createdBy = 0;
        public System.Byte CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
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

    }
}
