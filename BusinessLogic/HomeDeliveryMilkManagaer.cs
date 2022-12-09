using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class HomeDeliveryMilkManagaer
    {
        public BindingList<HomeDeliveryMilkInfo> ShowCustomerForHomedeliveryMilkEntry(int LineID, DateTime MilkDeliveryDate, bool DataExist)
        {
            BindingList<HomeDeliveryMilkInfo> retval = new BindingList<HomeDeliveryMilkInfo>();
            HomeDeliveryMilkDAL dal = new HomeDeliveryMilkDAL();
            retval = dal.ShowCustomerForHomedeliveryMilkEntry(LineID, MilkDeliveryDate, DataExist);
            dal = null;
            return retval;
        }

        public int HomedeliveryMilkByLineman(HomeDeliveryMilkInfo HomeDeliveryMilkIssue, BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo)
        {
            int retval = 0;
            HomeDeliveryMilkDAL dal = new HomeDeliveryMilkDAL();
            retval = dal.HomedeliveryMilkByLineman(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo);
            dal = null;
            return retval;
        }

        public BindingList<HomeDeliveryMilkInfo> SearchMilkDeliveryEnteries(string LinemanName, int count)
        {
            BindingList<HomeDeliveryMilkInfo> retval = new BindingList<HomeDeliveryMilkInfo>();
            HomeDeliveryMilkDAL dal = new HomeDeliveryMilkDAL();
            retval = dal.SearchMilkDeliveryEnteries(LinemanName, count);
            dal = null;
            return retval;
        }

        public void DeleteDelivery(int CustomerID)
        {
            HomeDeliveryMilkDAL dal = new HomeDeliveryMilkDAL();
            dal.DeleteDelivery(CustomerID);
            dal = null;
        }

        public int CheckTodaysMilkEntryExist(DateTime HomeDeliveryMilkDate, int LineID)
        {
            int retVal = 0;
            HomeDeliveryMilkDAL dal = new HomeDeliveryMilkDAL();
            retVal=dal.CheckTodaysMilkEntryExist(HomeDeliveryMilkDate, LineID);
            
            dal = null;
            return retVal;
        }

        public int UpdateHomedeliveryMilkByLineman(HomeDeliveryMilkInfo HomeDeliveryMilkIssue, BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo)
        {
            int retval = 0;
            HomeDeliveryMilkDAL dal = new HomeDeliveryMilkDAL();
            retval = dal.UpdateHomedeliveryMilkByLineman(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo);
            dal = null;
            return retval;
        }
    }
}
