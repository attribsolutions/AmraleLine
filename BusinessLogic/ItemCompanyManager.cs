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
    /// Date	: 06 Oct 2012 07:16:00 PM
    /// </summary>
    public class ItemCompanyManager
    {
        /// <summary>
        /// Adds a new ItemCompany in to the database
        /// </summary>
        /// <param name="itemcompany">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added ItemCompany</returns>

        public int AddItemCompany(ItemCompanyInfo itemcompany)
        {
            int retval = 0;
            ItemCompanyDAL dal = new ItemCompanyDAL();
            retval = dal.AddItemCompany(itemcompany);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the ItemCompany record in the database
        /// </summary>
        /// <param name="itemcompany">Instance with properties set as per the values entered in the form</param>
        public void UpdateItemCompany(ItemCompanyInfo itemcompany)
        {
            ItemCompanyDAL dal = new ItemCompanyDAL();
            dal.UpdateItemCompany(itemcompany);
            dal = null;
        }

        /// <summary>
        /// Gets all the itemcompanies from the database
        /// </summary>
        /// <returns>BindingList of itemcompanies</returns>
        public BindingList<ItemCompanyInfo> GetItemCompaniesAll(string name, bool all, bool orderByDisplayIndex)
        {
            BindingList<ItemCompanyInfo> retval = new BindingList<ItemCompanyInfo>();
            ItemCompanyDAL dal = new ItemCompanyDAL();
            retval = dal.GetItemCompaniesAll(name, all, orderByDisplayIndex);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets ItemCompany record from the database based on the ItemCompanyId
        /// </summary>
        /// <param name="itemcompanyId">Id of the ItemCompany</param>
        /// <returns>Instance of ItemCompany</returns>
        public ItemCompanyInfo GetItemCompany(int itemcompanyId)
        {
            ItemCompanyInfo retval = new ItemCompanyInfo();
            ItemCompanyDAL dal = new ItemCompanyDAL();
            retval = dal.GetItemCompany(itemcompanyId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the ItemCompany based on ItemCompanyId
        /// </summary>
        /// <param name="itemcompanyId">Id of the ItemCompany that is to be deleted</param>
        public void DeleteItemCompany(int itemcompanyId)
        {
            ItemCompanyDAL dal = new ItemCompanyDAL();
            dal.DeleteItemCompany(itemcompanyId);
            dal = null;
        }

        /// <summary>
        /// Checks if the ItemCompany name already exists in the itemcompanies table
        /// This will work for New Mode only(while adding a new itemcompany)
        /// </summary>
        /// <param name="itemcompanyName">Name of the ItemCompany that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name, bool checkDisplayIndex)
        {
            int retVal = 0;
            ItemCompanyDAL dal = new ItemCompanyDAL();
            retVal = dal.GetSameNameCount(name, checkDisplayIndex);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the ItemCompany name already exists in the ItemCompanies table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="itemcompanyName">Name of the ItemCompany that needs to be checked</param>
        /// <param name="itemcompanyId">Id of the ItemCompany that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current itemcompany.
        /// 0 is returned if no records are found</returns>

        public int GetSameNameCountForEditMode(string name, int itemcompanyId, bool checkDisplayIndex)
        {
            int retVal = 0;
            ItemCompanyDAL dal = new ItemCompanyDAL();
            retVal = dal.GetSameNameCountForEditMode(name, itemcompanyId, checkDisplayIndex);
            dal = null;
            return retVal;
        }

        public bool CheckItemCompanyUsed(int itemcompanyId)
        {
            ItemCompanyDAL dal = new ItemCompanyDAL();
            return dal.CheckItemCompanyUsed(itemcompanyId);
        }
    }
}
