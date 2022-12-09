using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing ReceiptPayment data
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 02:58:53 PM
    /// </summary>
    public class ReceiptPaymentInfo
    {
        public ReceiptPaymentInfo()
        {
        }

        System.Int64 _iD = 0;
        System.DateTime _transactionDate = DateTime.MinValue;
        System.String _receiptPayment = string.Empty;
        System.String _particulars = string.Empty;
        System.String _payMode = string.Empty;
        System.String _bankName = string.Empty;
        System.String _chequeNo = string.Empty;
        System.Decimal _amount = 0;
        System.Int32 _receivedPaidBy = 0;
        System.Boolean _partyTransaction = false;
        System.Int32 _partyID = 0;
        System.Int64 _billID = 0;
        System.Decimal _dueAmount = 0;
        System.Decimal _balanceAmount = 0;
        System.String _description = string.Empty;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;

        public System.Int64 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.DateTime TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; }
        }

        public System.String ReceiptPayment
        {
            get { return _receiptPayment; }
            set { _receiptPayment = value; }
        }

        public System.String Particulars
        {
            get { return _particulars; }
            set { _particulars = value; }
        }

        public System.String PayMode
        {
            get { return _payMode; }
            set { _payMode = value; }
        }

        public System.String BankName
        {
            get { return _bankName; }
            set { _bankName = value; }
        }

        public System.String ChequeNo
        {
            get { return _chequeNo; }
            set { _chequeNo = value; }
        }

        public System.Decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public System.Int32 ReceivedPaidBy
        {
            get { return _receivedPaidBy; }
            set { _receivedPaidBy = value; }
        }

        public System.Boolean PartyTransaction
        {
            get { return _partyTransaction; }
            set { _partyTransaction = value; }
        }

        public System.Int32 PartyID
        {
            get { return _partyID; }
            set { _partyID = value; }
        }

        public System.Int64 BillID
        {
            get { return _billID; }
            set { _billID = value; }
        }

        public System.Decimal DueAmount
        {
            get { return _dueAmount; }
            set { _dueAmount = value; }
        }

        public System.Decimal BalanceAmount
        {
            get { return _balanceAmount; }
            set { _balanceAmount = value; }
        }

        public System.String Description
        {
            get { return _description; }
            set { _description = value; }
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