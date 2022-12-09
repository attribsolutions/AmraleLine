using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class RateManager
    {

        public BindingList<RatesInfo> GetRatesByFilter(string itemName, int count)
        {
            BindingList<RatesInfo> retval = new BindingList<RatesInfo>();
            RateDAL dal = new RateDAL();
            retval = dal.GetRatesByFilter(itemName, count);
            dal = null;
            return retval;
        }

        public int AddRate(RatesInfo rate)
        {
            int retval = 0;
            RateDAL dal = new RateDAL();
            retval = dal.AddRate(rate);
            dal = null;
            return retval;
        }

        public RatesInfo GetRateByItemCode(int itemID)
        {
            RatesInfo retval = new RatesInfo();
            RateDAL dal = new RateDAL();
            retval = dal.GetRateByItemCode(itemID);
            dal = null;
            return retval;
        }

        public void DeleteRate(int RateID)
        {
            RateDAL dal = new RateDAL();
            dal.DeleteRate(RateID);
            dal = null;
        }
    }
}
