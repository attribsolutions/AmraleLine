using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class DispatchManager
    {
        public BindingList<ChallanItemInfo> GetChallanItemsByDate(DateTime challanDate)
        {
            DispatchDAL dal = new DispatchDAL();
            return dal.GetChallanItemsByDate(challanDate);
        }
    }
}
