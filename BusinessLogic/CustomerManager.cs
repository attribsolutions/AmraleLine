using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    /// <summary>
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:21:21 PM
    /// </summary>
    public class CustomerManager
    {
        /// <summary>
        /// Adds a new Customer in to the database
        /// </summary>
        /// <param name="customer">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Customer</returns>

        public int AddCustomer(CustomerInfo customer, BindingList<CustomerItemsInfo> cutomerItems)
        {
            int retval = 0;
            CustomerDAL dal = new CustomerDAL();
            retval = dal.AddCustomer(customer,cutomerItems);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Customer record in the database
        /// </summary>
        /// <param name="customer">Instance with properties set as per the values entered in the form</param>
        public void UpdateCustomer(CustomerInfo customer, BindingList<CustomerItemsInfo> customerItem)
        {
            CustomerDAL dal = new CustomerDAL();
            dal.UpdateCustomer(customer,customerItem);
            dal = null;
        }

        /// <summary>
        /// Gets all the customers from the database
        /// </summary>
        /// <returns>BindingList of customers</returns>
        public BindingList<CustomerInfo> GetCustomersAll( )
        {
            BindingList<CustomerInfo> retval = new BindingList<CustomerInfo>();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersAll();
            dal = null;
            return retval;
        }

        public BindingList<CustomerInfo> GetCustomersCouponsAll()
        {
            BindingList<CustomerInfo> retval = new BindingList<CustomerInfo>();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersCouponsAll();
            dal = null;
            return retval;
        }

        public BindingList<CustomerInfo> GetCustomersByName(int searchType, string customerName,int lineID)
        {
            BindingList<CustomerInfo> retval = new BindingList<CustomerInfo>();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersByName(searchType, customerName,lineID);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Customer record from the database based on the CustomerId
        /// </summary>
        /// <param name="customerId">Id of the Customer</param>
        /// <returns>Instance of Customer</returns>
        public CustomerInfo GetCustomer(int customerId)
        {
            CustomerInfo retval = new CustomerInfo();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomer(customerId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Customer based on CustomerId
        /// </summary>
        /// <param name="customerId">Id of the Customer that is to be deleted</param>
        public void DeleteCustomer(int customerId)
        {
            CustomerDAL dal = new CustomerDAL();
            dal.DeleteCustomer(customerId);
            dal = null;
        }

        /// <summary>
        /// Checks if the Customer name already exists in the customers table
        /// This will work for New Mode only(while adding a new customer)
        /// </summary>
        /// <param name="customerName">Name of the Customer that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name)
        {
            int retVal = 0;
            CustomerDAL dal = new CustomerDAL();
            retVal = dal.GetSameNameCount(name);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the Customer name already exists in the Customers table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="customerName">Name of the Customer that needs to be checked</param>
        /// <param name="customerId">Id of the Customer that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current customer.
        /// 0 is returned if no records are found</returns>

        public int GetSameNameCountForEditMode(string name, int customerId)
        {
            int retVal = 0;
            CustomerDAL dal = new CustomerDAL();
            retVal = dal.GetSameNameCountForEditMode(name, customerId);
            dal = null;
            return retVal;
        }


        public bool CheckCustomerUsed(int customerID, out string tableName)
        {
            CustomerDAL dal = new CustomerDAL();
            return dal.CheckCustomerUsed(customerID, out tableName);
        }

        public CustomerInfo GetCustomerForSales(int customerId)
        {
            CustomerInfo retval = new CustomerInfo();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomerForSales(customerId);
            dal = null;
            return retval;
        }

        public BindingList<CustomerItemsInfo> GetCustomerItemsForSelectedCustomer(int CustomerID)
        {
            CustomerDAL dal = new CustomerDAL();
            return dal.GetCustomerItemsForSelectedCustomer(CustomerID);
        }

        public int GetMaxCustomerNumber()
        {
            CustomerDAL dal = new CustomerDAL();
            return dal.GetMaxCustomerNumber();
        }

        public bool CheckCustomerNumber(int CustomerNumber)
        {
            CustomerDAL dal = new CustomerDAL();
            return dal.CheckCustomerNumber(CustomerNumber);
        }

        public bool CheckBarCode(string BarCode)
        {
            CustomerDAL dal = new CustomerDAL();
            return dal.CheckBarCode(BarCode);
        }


        public BindingList<CustomerInfo> GetCustomersByLineID(int LineID)
        {
            BindingList<CustomerInfo> retval = new BindingList<CustomerInfo>();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersByLineID(LineID);
            dal = null;
            return retval;
        }
        public BindingList<CustomerInfo> GetCustomersByIDs(int LineID, int CustomerNo, int CustomerID)
        {
            BindingList<CustomerInfo> retval = new BindingList<CustomerInfo>();
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersByIDs(LineID, CustomerNo, CustomerID);
            dal = null;
            return retval;
        }
        public int GetCustomersLastNoByLineID(int LineID)
        {
            int retval = 0;
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersLastNoByLineID(LineID);
            dal = null;
            return retval;
        }

        public void UpdateCustomerForLineChange(CustomerInfo customer, BindingList<CustomerInfo> customers)
        {
            CustomerDAL dal = new CustomerDAL();
            dal.UpdateCustomerForLineChange(customer, customers);
            dal = null;
        }

        public int GetCustomersNumberByID(int LineID, int CustomerID)
        {
            int retval = 0;
            CustomerDAL dal = new CustomerDAL();
            retval = dal.GetCustomersNumberByID(LineID, CustomerID);
            dal = null;
            return retval;
        }
    }
}