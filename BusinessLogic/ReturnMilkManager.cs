using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
    public class ReturnMilkManager
    {
       public BindingList<MilkIssueInfo> GetLinemanByFilter(string LinemanName, int count)
       {
           BindingList<MilkIssueInfo> retval = new BindingList<MilkIssueInfo>();
           ReturnMilkDAL dal = new ReturnMilkDAL();
           retval = dal.GetLinemanByFilter(LinemanName, count);
           dal = null;
           return retval;
       }

       public BindingList<MilkIssueItemInfo> ShowItemsforMilkReturnEntry(MilkIssueInfo milkIssuelinemanInfo)
       {
          // BindingList<MilkIssueItemInfo> retval = new BindingList<MilkIssueItemInfo>();
           ReturnMilkDAL dal = new ReturnMilkDAL();
           BindingList<MilkIssueItemInfo> retval = dal.ShowItemsforMilkReturnEntry(milkIssuelinemanInfo);
           dal = null;
           return retval;
       }

       public int ReturnMilkByLineman(LinemanInfo MilkIssue, BindingList<MilkIssueItemInfo> MilkIssueItems)
       {
           int retval = 0;
           ReturnMilkDAL dal = new ReturnMilkDAL();
           retval = dal.ReturnMilkByLineman(MilkIssue, MilkIssueItems);
           dal = null;
           return retval;
       }

       public void DeleteMilkReturn(int MilkIssueID)
       {
           ReturnMilkDAL dal = new ReturnMilkDAL();
           dal.DeleteMilkReturn(MilkIssueID);
           dal = null;
       }
    }
}
