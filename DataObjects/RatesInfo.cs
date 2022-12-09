using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class RatesInfo
    {
        public RatesInfo()
        { }

        System.Int32 _id = 0;
        public System.Int32 ID
        {
            get { return _id; }
            set { _id = value; }
        }

        System.Int32 _itemID = 0;
        public System.Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        System.String _itemName = string.Empty;
        public System.String ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        System.Decimal _vat = 0;
        public System.Decimal VAT
        {
            get { return _vat; }
            set { _vat = value; }
        }

        System.Decimal _rate = 0;
        public System.Decimal Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        System.DateTime _effectiveFrom = DateTime.MinValue;
        public System.DateTime EffectiveFrom
        {
            get { return _effectiveFrom; }
            set { _effectiveFrom = value; }
        }

        System.Boolean _isLastUpdated = true;
        public System.Boolean IsLastUpdated
        {
            get { return _isLastUpdated; }
            set { _isLastUpdated = value; }
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
