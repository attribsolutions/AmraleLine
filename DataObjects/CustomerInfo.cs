using System;

namespace DataObjects
{
    /// <summary>
    /// Purpose	: Class used for storing Customer data
    /// Author	: Akash
    /// Date	: 25 june 2017 02:56:01 PM
    /// </summary>
    public class CustomerInfo
    {
        public CustomerInfo()
        {
        }

        System.Int32 _iD = 0;
        System.Int32 _customerNumber = 0;
        System.Int32 _newCustomerNumber = 0;
        System.String _name = string.Empty;
        System.String _contactPerson = string.Empty;
        System.String _address = string.Empty;
        System.String _city = string.Empty;
        System.String _mobile = string.Empty;
        System.String _eMail = string.Empty;
        System.String _vatTinNumber = string.Empty;
        System.Decimal _balance = 0;
        System.Decimal _deposit = 0;
        System.Boolean _isActive = false;
        System.Int32 _customerType = 0;
        System.String _barcode = string.Empty;
        System.DateTime _memberSince = DateTime.MinValue;
        System.Int32 _itemID = 0;
        System.Int32 _itemCode = 0;
        System.Decimal _quantity = 0;
        System.Byte _createdBy = 0;
        System.DateTime _createdOn = DateTime.MinValue;
        System.Byte _updatedBy = 0;
        System.DateTime _updatedOn = DateTime.MinValue;
        System.String _itemName = string.Empty;
        System.Int32 _lineID = 0;
        System.Int32 _lineNumber = 0;
        System.String _CustomerNameMarathi ;

     

        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

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
        public System.Int32 NewCustomerNumber
        {
            get { return _newCustomerNumber; }
            set { _newCustomerNumber = value; }
        }
        public System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public System.String ContactPerson
        {
            get { return _contactPerson; }
            set { _contactPerson = value; }
        }

        public System.String Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public System.String City
        {
            get { return _city; }
            set { _city = value; }
        }

        public System.String Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        public System.String EMail
        {
            get { return _eMail; }
            set { _eMail = value; }
        }

        public System.String VatTinNumber
        {
            get { return _vatTinNumber; }
            set { _vatTinNumber = value; }
        }

        public System.Decimal Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }

        public System.Decimal Deposit
        {
            get { return _deposit; }
            set { _deposit = value; }
        }

        public System.Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public System.Int32 CustomerType
        {
            get { return _customerType; }
            set { _customerType = value; }
        }

        public System.String Barcode
        {
            get { return _barcode; }
            set { _barcode = value; }
        }

        public System.DateTime MemberSince
        {
            get { return _memberSince; }
            set { _memberSince = value; }
        }

        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public System.Int32 ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }

        public System.Decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
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
        public System.String ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        public System.String CustomerNameMarathi
        {
            get { return _CustomerNameMarathi; }
            set { _CustomerNameMarathi = value; }
        }
        

    }

    public class CustomerItemsInfo
    {
        public CustomerItemsInfo()
        {
        }

        System.Int32 _iD = 0;
        System.Int32 _customerId = 0;
        System.Int32 _itemID = 0;
        System.Decimal _quantity = 0;
        System.Int32 _itemCode = 0;
        
        public System.Int32 ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        public System.Int32 CustomerID
        {
            get { return _customerId; }
            set { _customerId = value; }
        }
        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public System.Decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public System.Int32 ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }
        
    }
}