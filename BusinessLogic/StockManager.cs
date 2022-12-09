using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataAccess;
using DataObjects;

namespace BusinessLogic
{
    public class StockManager
    {
        public List<ItemInfo> GetAllItems()
        {
            List<ItemInfo> retVal = new List<ItemInfo>();
            StockDAL dal = new StockDAL();
            retVal = dal.GetAllItems();
            return retVal;
        }

        public StockInfo Process(DateTime stockDate, int itemID, int divisionID)
        {
            StockInfo retVal = new StockInfo();
            StockDAL dal = new StockDAL();
            retVal = dal.Process(stockDate, itemID, divisionID);
            return retVal;
        }

        public void StockInsert(StockInfo stock)
        {
            StockDAL dal = new StockDAL();
            dal.StockInsert(stock);
        }

        public void StockDelete(DateTime stockDate, int divisionID)
        {
            StockDAL dal = new StockDAL();
            dal.StockDelete(stockDate,divisionID);
        }

        public List<StockInfo> GetByDate(DateTime stockDate, string itemName, int divisionID)
        {
            List<StockInfo> retVal = new List<StockInfo>();
            StockDAL dal = new StockDAL();
            retVal = dal.GetByDate(stockDate, itemName, divisionID);
            return retVal;
        }

        public int GetPreviousDay(DateTime stockDate, int divisionID)
        {
            int retVal = 0;
            StockDAL dal = new StockDAL();
            retVal = dal.GetPreviousDay(stockDate,divisionID);
            return retVal;
        }

        public DateTime GetMaxDate(int divisionID)
        {
            DateTime retVal = DateTime.Today;
            StockDAL dal = new StockDAL();
            retVal = dal.GetMaxDate(divisionID);
            return retVal;
        }

        public int IsAdjusted(DateTime stockDate, int divisionID)
        {
            int retVal = 0;
            StockDAL dal = new StockDAL();
            retVal = dal.IsAdjusted(stockDate,divisionID);
            return retVal;
        }

        public bool CheckDayExist(DateTime stockDate, int divisionID)
        {
            StockDAL dal = new StockDAL();
            return dal.CheckDayExist(stockDate,divisionID);
        }
    }
}
