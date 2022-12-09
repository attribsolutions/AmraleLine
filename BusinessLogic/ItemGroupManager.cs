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
    /// Date	: 16 Apr 2010 03:23:27 PM
    /// </summary>
    public class ItemGroupManager
    {
        /// <summary>
        /// Adds a new ItemGroup in to the database
        /// </summary>
        /// <param name="itemgroup">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added ItemGroup</returns>

        public int AddItemGroup(ItemGroupInfo itemgroup)
        {
            int retval = 0;
            ItemGroupDAL dal = new ItemGroupDAL();
            retval = dal.AddItemGroup(itemgroup);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the ItemGroup record in the database
        /// </summary>
        /// <param name="itemgroup">Instance with properties set as per the values entered in the form</param>
        public void UpdateItemGroup(ItemGroupInfo itemgroup)
        {
            ItemGroupDAL dal = new ItemGroupDAL();
            dal.UpdateItemGroup(itemgroup);
            dal = null;
        }

        /// <summary>
        /// Gets all the itemgroups from the database
        /// </summary>
        /// <returns>BindingList of itemgroups</returns>
        public BindingList<ItemGroupInfo> GetItemGroupsAll(int itemCompanyId)
        {
            BindingList<ItemGroupInfo> retval = new BindingList<ItemGroupInfo>();
            ItemGroupDAL dal = new ItemGroupDAL();
            retval = dal.GetItemGroupsAll(itemCompanyId);
            dal = null;
            return retval;
        }

        public BindingList<ItemGroupInfo> GetItemGroupsByName(string groupName)
        {
            BindingList<ItemGroupInfo> retval = new BindingList<ItemGroupInfo>();
            ItemGroupDAL dal = new ItemGroupDAL();
            retval = dal.GetItemGroupsByName(groupName);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets ItemGroup record from the database based on the ItemGroupId
        /// </summary>
        /// <param name="itemgroupId">Id of the ItemGroup</param>
        /// <returns>Instance of ItemGroup</returns>
        public ItemGroupInfo GetItemGroup(int itemgroupId)
        {
            ItemGroupInfo retval = new ItemGroupInfo();
            ItemGroupDAL dal = new ItemGroupDAL();
            retval = dal.GetItemGroup(itemgroupId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the ItemGroup based on ItemGroupId
        /// </summary>
        /// <param name="itemgroupId">Id of the ItemGroup that is to be deleted</param>
        public void DeleteItemGroup(int itemgroupId)
        {
            ItemGroupDAL dal = new ItemGroupDAL();
            dal.DeleteItemGroup(itemgroupId);
            dal = null;
        }

        /// <summary>
        /// Checks if the ItemGroup name already exists in the itemgroups table
        /// This will work for New Mode only(while adding a new itemgroup)
        /// </summary>
        /// <param name="itemgroupName">Name of the ItemGroup that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name, bool checkDisplayIndex)
        {
            int retVal = 0;
            ItemGroupDAL dal = new ItemGroupDAL();
            retVal = dal.GetSameNameCount(name, checkDisplayIndex);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the ItemGroup name already exists in the ItemGroups table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="itemgroupName">Name of the ItemGroup that needs to be checked</param>
        /// <param name="itemgroupId">Id of the ItemGroup that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current itemgroup.
        /// 0 is returned if no records are found</returns>

        public int GetSameNameCountForEditMode(string name, int itemgroupId, bool checkDisplayIndex)
        {
            int retVal = 0;
            ItemGroupDAL dal = new ItemGroupDAL();
            retVal = dal.GetSameNameCountForEditMode(name, itemgroupId, checkDisplayIndex);
            dal = null;
            return retVal;
        }


        public bool CheckItemGroupUsed(int groupId)
        {
            ItemGroupDAL dal = new ItemGroupDAL();
            return dal.CheckItemGroupUsed(groupId);
        }

        public int CheckCouponGroupCount()
        {
            int retVal = 0;
            ItemGroupDAL dal = new ItemGroupDAL();
            retVal = dal.CheckCouponGroupCount();
            return retVal;
        }
    }
}