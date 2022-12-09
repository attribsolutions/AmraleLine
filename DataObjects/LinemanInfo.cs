using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class LinemanInfo
    {
        public LinemanInfo()
        { }
        System.DateTime _fromDate = DateTime.MinValue;
        public System.DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; }
        }

        System.Int32 _id = 0;
        public System.Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }
        System.Int32 _milkissueid = 0;
        public System.Int32 MilkIssueID
        {
            get { return _milkissueid; }
            set { _milkissueid = value; }
        }

        System.Int32 _lineID = 0;
        
        public System.Int32 LineId
        {
            get { return _lineID; }
            set { _lineID = value; }
        }
        System.Int32 _lineNumber = 0;

        public System.Int32 LineNumber
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

        System.String _lineName = string.Empty;
        public System.String LineName
        {
            get { return _lineName; }
            set { _lineName = value; }
        }

        System.String _mobile = string.Empty;
        public System.String Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        System.String _address = string.Empty;
        public System.String Address
        {
            get { return _address; }
            set { _address = value; }
        }

        System.String _city = string.Empty;
        public System.String City
        {
            get { return _city; }
            set { _city = value; }
        }

        System.Decimal _commission = 0;
        public System.Decimal Commission
        {
            get { return _commission; }
            set { _commission = value; }
        }

        System.Byte _createdBy = 0;
        public System.Byte CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        System.Boolean _isActive = false;
        public System.Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
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
