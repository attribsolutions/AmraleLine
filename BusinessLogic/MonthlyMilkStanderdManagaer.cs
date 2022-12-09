using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class MonthlyMilkStanderdManagaer
    {
        public BindingList<MonthlyMilkStanderdInfo> ShowCustomerForHomedeliveryMilkEntry(int LineID)
        {
            BindingList<MonthlyMilkStanderdInfo> retval = new BindingList<MonthlyMilkStanderdInfo>();
            MonthlyMilkStanderdDAL dal = new MonthlyMilkStanderdDAL();
            retval = dal.ShowCustomerForHomedeliveryMilkEntry(LineID);
            dal = null;
            return retval;
        }

        public int HomedeliveryMilkByLineman(MonthlyMilkStanderdInfo HomeDeliveryMilkIssue, BindingList<MonthlyMilkStanderdInfo> HomeDeliveryMilkInfo)
        {
            int retval = 0;
            MonthlyMilkStanderdDAL dal = new MonthlyMilkStanderdDAL();
            retval = dal.HomedeliveryMilkByLineman(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo);
            dal = null;
            return retval;
        }

        public BindingList<MonthlyMilkStanderdInfo> SearchMilkDeliveryEnteries(string LinemanName, int count)
        {
            BindingList<MonthlyMilkStanderdInfo> retval = new BindingList<MonthlyMilkStanderdInfo>();
            MonthlyMilkStanderdDAL dal = new MonthlyMilkStanderdDAL();
            retval = dal.SearchMilkDeliveryEnteries(LinemanName, count);
            dal = null;
            return retval;
        }

        public void DeleteDelivery(int CustomerID)
        {
            MonthlyMilkStanderdDAL dal = new MonthlyMilkStanderdDAL();
            dal.DeleteDelivery(CustomerID);
            dal = null;
        }

        public int CheckTodaysMonthlyStanderedEntryExist(DateTime HomeDeliveryMilkDate, int LineID)
        {
            int retVal = 0;
            MonthlyMilkStanderdDAL dal = new MonthlyMilkStanderdDAL();
            retVal = dal.CheckTodaysMilkEntryExist(HomeDeliveryMilkDate, LineID);

            dal = null;
            return retVal;
        }

        public int UpdateMonthlyStandered(MonthlyMilkStanderdInfo HomeDeliveryMilkIssue, BindingList<MonthlyMilkStanderdInfo> HomeDeliveryMilkInfo)
        {

            int retval = 0;
            MonthlyMilkStanderdDAL dal = new MonthlyMilkStanderdDAL();
            retval = dal.UpdateMonthlyStandered(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo);
            dal = null;
            return retval;
        }
    }
}
