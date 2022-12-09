using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class SalePaymentManager
    {
        public int AddSalePayment(SalePaymentInfo salePayment)
        {
            SalePaymentDAL dal = new SalePaymentDAL();
            return dal.AddSalePayment(salePayment);
        }
        public BindingList<SalePaymentInfo> GetSalesPaymentByFilter(int searchType, string name, object sDate, object eDate)
        {
            SalePaymentDAL dal = new SalePaymentDAL();
            return dal.GetSalesPaymentByFilter(searchType,name,sDate,eDate);
        }

        public void DeletePayment(int salePaymentId, int processingId)
        {
            SalePaymentDAL dal = new SalePaymentDAL();
            dal.DeletePayment(salePaymentId, processingId);
            dal = null;
        }



        public bool CheckReceiptNoExists(int receiptNo)
        {
            SalePaymentDAL dal = new SalePaymentDAL();
            return dal.CheckReceiptNoExists(receiptNo);
        }
    }
}
