using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;

namespace BusinessLogic
{
   public  class MilkIssueManager
    {
       public BindingList<MilkIssueInfo> GetLinemanByFilter(string LinemanName, int count)
       {
           BindingList<MilkIssueInfo> retval = new BindingList<MilkIssueInfo>();
           MilkIssueDAL dal = new MilkIssueDAL();
           retval = dal.GetLinemanByFilter(LinemanName, count);
           dal = null;
           return retval;
       }

       public BindingList<MilkIssueItemInfo> ShowItemsforMilkIssueEntry()
       {
          // BindingList<MilkIssueItemInfo> retval = new BindingList<MilkIssueItemInfo>();
           MilkIssueDAL dal=new MilkIssueDAL();
           BindingList<MilkIssueItemInfo> retval = dal.ShowItemsforMilkIssueEntry();
           dal = null;
           return retval;
       }

       public int AddMilkIssueToLineMan(LinemanInfo MilkIssue, BindingList<MilkIssueItemInfo> MilkIssueItems)
       {
           int retval = 0;
           MilkIssueDAL dal = new MilkIssueDAL();
            retval = dal.AddMilkIssueToLineMan(MilkIssue, MilkIssueItems);
           dal = null;
           return retval;
       }

       public void DeleteMilkIssue(int MilkIssueID)
       {
           MilkIssueDAL dal = new MilkIssueDAL();
           dal.DeleteMilkIssue(MilkIssueID);
           dal = null;
       }
    }
}
