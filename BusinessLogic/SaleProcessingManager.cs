using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class SaleProcessingManager
    {
        public bool ProcessSalesData(DateTime BillDate, int CustomerID, out int errorType, string CustomerIDs, Boolean IsPaidAmount)
        {
            SaleProcessingDAL dal = new SaleProcessingDAL();
            return dal.ProcessSalesData(BillDate, CustomerID, out errorType, CustomerIDs, IsPaidAmount);
        }
        //public bool ProcessSalesData(DateTime BillDate, int CustomerID, out int errorType, int searchType, string customerName, Int32 customerNo, Boolean isPaidAmount)
        //{
        //    SaleProcessingDAL dal = new SaleProcessingDAL();
        //    return dal.ProcessSalesData(BillDate, CustomerID, out errorType, searchType, customerName, customerNo, isPaidAmount);
        //}

        public int CheckIfDataPresentInSalesProcessing(DateTime BillDate, int CustomerID, out SaleProcessingInfo retVal)
        {
            SaleProcessingDAL dal = new SaleProcessingDAL();
            return dal.CheckIfDataPresentInSalesProcessing(BillDate, CustomerID,out retVal);
        }

        public bool ProcessSalesDataOfSingleCustomers(int Mode, DateTime BillDate, SaleProcessingInfo salesprocessing)
        {
            SaleProcessingDAL dal = new SaleProcessingDAL();
            return dal.ProcessSalesDataOfSingleCustomers(Mode, BillDate, salesprocessing);
        }

        public BindingList<SaleProcessingInfo> GetAllLSalesProcessingList(int searchType, string customerName, DateTime BillDate, Int32 customerNo,Int32 LineID)
        {
            SaleProcessingDAL dal = new SaleProcessingDAL();
            return dal.GetAllLSalesProcessingList(searchType, customerName, BillDate, customerNo,LineID);
        }

        public decimal GetClosingBalanceofCustomer(int ID)
        {
            SaleProcessingDAL dal = new SaleProcessingDAL();
            return dal.GetClosingBalanceofCustomer(ID);
        }

        
    }
}
