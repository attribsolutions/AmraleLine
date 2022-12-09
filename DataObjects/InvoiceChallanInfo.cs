using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing InvoiceChallan data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:56:31 PM
    /// </summary>
    public class InvoiceChallanInfo
    {
        public InvoiceChallanInfo()
        {
        }

        System.Int64 _iD = 0;
        System.Int64 _invoiceID = 0;
        System.Int64 _challanID = 0;
        System.Boolean _completed = false;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int64 InvoiceID
        {
            get { return _invoiceID; }
            set { _invoiceID = value; }
        }

        public System.Int64 ChallanID
        {
            get { return _challanID; }
            set { _challanID = value; }
        }

        public System.Boolean Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        public string ChallanDate { get; set; }
    }
}