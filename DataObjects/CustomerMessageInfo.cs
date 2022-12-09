using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class CustomerMessageInfo
    {
        System.Int32 _lineNumber = 0;
        System.Int32 _customerNumber = 0;
        System.String _name = string.Empty;

        System.Int32 _id = 0;
        public System.Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }

        System.DateTime _fromDate = DateTime.MinValue;
        public System.DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; }
        }

        System.DateTime _toDate = DateTime.MinValue;
        public System.DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; }
        }

        System.Int32 _customerID = 0;
        public System.Int32 CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }

        System.Int32 _lineID = 0;
        public System.Int32 LineID
        {
            get { return _lineID; }
            set { _lineID = value; }
        }
        public System.Int32 LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }
        public System.Int32 CustomerNumber
        {
            get { return _customerNumber; }
            set { _customerNumber = value; }
        }

        public System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        System.String _defaultMessage = string.Empty;
        public System.String DefaultMessage
        {
            get { return _defaultMessage; }
            set { _defaultMessage = value; }

        }

        System.String _message = string.Empty;
        public System.String Message
        {
            get { return  _message;}
            set { _message = value; }
        }
        System.String _IsComplated = string.Empty;
        public System.String IsComplated
        {
            get { return _IsComplated; }
            set { _IsComplated = value;}
        
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
