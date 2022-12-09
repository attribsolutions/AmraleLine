using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;
namespace BusinessLogic
{
    public class CustomerMessageManager
    {
        public int AddCustomerMessage(CustomerMessageInfo CustMsg)
        {
            int retval = 0;
            CustomerMessageDAL dal = new CustomerMessageDAL();
            retval = dal.AddCustomerMessage(CustMsg);
            dal = null;
            return retval;
        }

        public void UpdateCustomerMessage(CustomerMessageInfo CustMsg)
        {
            CustomerMessageDAL dal = new CustomerMessageDAL();
            dal.UpdateCustomerMessage(CustMsg);
            dal = null;
        }

        public BindingList<CustomerMessageInfo> GetCustomerMessagesByFilter(string customerName, int Count)
        {
            BindingList<CustomerMessageInfo> retval = new BindingList<CustomerMessageInfo>();
            CustomerMessageDAL dal = new CustomerMessageDAL();
            retval = dal.GetCustomerMessagesByFilter(customerName, Count);
            dal = null;
            return retval;
        }

        public void DeleteCustomerMessage(int MessageID)
        {
            CustomerMessageDAL dal = new CustomerMessageDAL();
            dal.DeleteCustomerMessage(MessageID);
            dal = null;
        }
    }
}
