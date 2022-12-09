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
    /// Date	: 16 Apr 2010 03:26:24 PM
    /// </summary>
    public class ItemManager
    {
        /// <summary>
        /// Adds a new Item in to the database
        /// </summary>
        /// <param name="item">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Item</returns>

        public int AddItem(ItemInfo item)
        {
            int retval = 0;
            ItemDAL dal = new ItemDAL();
            retval = dal.AddItem(item);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Item record in the database
        /// </summary>
        /// <param name="item">Instance with properties set as per the values entered in the form</param>
        public void UpdateItem(ItemInfo item)
        {
            ItemDAL dal = new ItemDAL();
            dal.UpdateItem(item);
            dal = null;
        }

        /// <summary>
        /// Gets all the items from the database
        /// </summary>
        /// <returns>BindingList of items</returns>
        public BindingList<ItemInfo> GetItemsAll()
        {
            BindingList<ItemInfo> retval = new BindingList<ItemInfo>();
            ItemDAL dal = new ItemDAL();
            retval = dal.GetItemsAll();
            dal = null;
            return retval;
        }

        public BindingList<ItemInfo> GetItemsByFilter(int searchType, string itemName, int count)
        {
            BindingList<ItemInfo> retval = new BindingList<ItemInfo>();
            ItemDAL dal = new ItemDAL();
            retval = dal.GetItemsByFilter(searchType, itemName, count);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Item record from the database based on the ItemId
        /// </summary>
        /// <param name="itemId">Id of the Item</param>
        /// <returns>Instance of Item</returns>
        public ItemInfo GetItem(int itemId)
        {
            ItemInfo retval = new ItemInfo();
            ItemDAL dal = new ItemDAL();
            retval = dal.GetItem(itemId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Item based on ItemId
        /// </summary>
        /// <param name="itemId">Id of the Item that is to be deleted</param>
        public void DeleteItem(int itemId)
        {
            ItemDAL dal = new ItemDAL();
            dal.DeleteItem(itemId);
            dal = null;
        }

        /// <summary>
        /// Checks if the Item name already exists in the items table
        /// This will work for New Mode only(while adding a new item)
        /// </summary>
        /// <param name="itemName">Name of the Item that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name, byte nameCode)
        {
            int retVal = 0;
            ItemDAL dal = new ItemDAL();
            retVal = dal.GetSameNameCount(name, nameCode);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the Item name already exists in the Items table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="itemName">Name of the Item that needs to be checked</param>
        /// <param name="itemId">Id of the Item that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current item.
        /// 0 is returned if no records are found</returns>

        public int GetSameNameCountForEditMode(string name, int itemId, byte nameCode)
        {
            int retVal = 0;
            ItemDAL dal = new ItemDAL();
            retVal = dal.GetSameNameCountForEditMode(name, itemId, nameCode);
            dal = null;
            return retVal;
        }


        public bool CheckItemUsed(int itemId, out string tableName)
        {
            ItemDAL dal = new ItemDAL();
            return dal.CheckItemUsed(itemId, out tableName);
        }

        public BindingList<ItemInfo> GetItemsAllByGroupID(int groupID, string counter)
        {
            BindingList<ItemInfo> retVal = new BindingList<ItemInfo>();
            ItemDAL dal = new ItemDAL();
            retVal = dal.GetItemsAllByGroupID(groupID, counter);
            dal = null;
            return retVal;
        }

        public ItemInfo GetItemByBarCode(string barCode)
        {
            ItemInfo retVal = new ItemInfo();
            ItemDAL dal = new ItemDAL();
            retVal = dal.GetItemByBarCode(barCode);
            dal = null;
            return retVal;
        }

        public bool CheckFixedItemCount(int count)
        {
            ItemDAL dal = new ItemDAL();
            return dal.CheckFixedItemCount(count);
        }

        public ItemInfo GetItemByItemCode(int itemCode)
        {
            ItemDAL dal = new ItemDAL();
            return dal.GetItemByItemCode(itemCode);
        }

        public int GetMaxItemID(byte codeID)
        {
            ItemDAL dal = new ItemDAL();
            return dal.GetMaxItemID(codeID);
        }
    }
}