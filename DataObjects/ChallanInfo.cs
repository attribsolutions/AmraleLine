using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Challan data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:55:34 PM
    /// </summary>
    public class ChallanInfo
    {
        public ChallanInfo()
        {
        }

        System.Int64 _iD = 0;
        System.DateTime _challanDate = DateTime.MinValue;
        System.String _challanNo = string.Empty;
        System.Int32 _supplierID = 0;
        System.DateTime _receivedDate = DateTime.MinValue;
        System.String _vehicleNo = string.Empty;
        System.String _deliveredBy = string.Empty;
        System.Int32 _receivedBy = 0;
        System.Boolean _isConfirmed = false;
        System.String _description = string.Empty;
        System.Int32 _todivisionID = 0;
        System.Int32 _fromdivisionID = 0;
        System.String _divisionName = string.Empty;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
        System.String _supplierName = string.Empty;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime ChallanDate
        {
            get { return _challanDate; }
            set { _challanDate = value; }
        }

        public System.String ChallanNo
        {
            get { return _challanNo; }
            set { _challanNo = value; }
        }

        public System.Int32 SupplierID
        {
            get { return _supplierID; }
            set { _supplierID = value; }
        }

        public System.String SupplierName
        {
            get { return _supplierName; }
            set { _supplierName = value; }
        }

        public System.DateTime ReceivedDate
        {
            get { return _receivedDate; }
            set { _receivedDate = value; }
        }

        public System.String VehicleNo
        {
            get { return _vehicleNo; }
            set { _vehicleNo = value; }
        }

        public System.String DeliveredBy
        {
            get { return _deliveredBy; }
            set { _deliveredBy = value; }
        }

        public System.Int32 ReceivedBy
        {
            get { return _receivedBy; }
            set { _receivedBy = value; }
        }

        public System.Boolean IsConfirmed
        {
            get { return _isConfirmed; }
            set { _isConfirmed = value; }
        }

        public System.String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public System.Int32 ToDivisionID
        {
            get { return _todivisionID; }
            set { _todivisionID = value; }
        }

        public System.Int32 FromDivisionID
        {
            get { return _fromdivisionID; }
            set { _fromdivisionID = value; }
        }
        public System.String DivisionName
        {
            get { return _divisionName; }
            set { _divisionName = value; }
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