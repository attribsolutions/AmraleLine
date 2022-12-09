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
    /// Date	: 16 Apr 2010 03:29:09 PM
    /// </summary>
    public class SupplierManager
    {
        /// <summary>
        /// Adds a new Supplier in to the database
        /// </summary>
        /// <param name="supplier">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Supplier</returns>

        public int AddSupplier(SupplierInfo supplier)
        {
            int retval = 0;
            SupplierDAL dal = new SupplierDAL();
            retval = dal.AddSupplier(supplier);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Supplier record in the database
        /// </summary>
        /// <param name="supplier">Instance with properties set as per the values entered in the form</param>
        public void UpdateSupplier(SupplierInfo supplier)
        {
            SupplierDAL dal = new SupplierDAL();
            dal.UpdateSupplier(supplier);
            dal = null;
        }

        /// <summary>
        /// Gets all the suppliers from the database
        /// </summary>
        /// <returns>BindingList of suppliers</returns>
        public BindingList<SupplierInfo> GetSuppliersAll(bool showActiveOnly)
        {
            BindingList<SupplierInfo> retval = new BindingList<SupplierInfo>();
            SupplierDAL dal = new SupplierDAL();
            retval = dal.GetSuppliersAll(showActiveOnly);
            dal = null;
            return retval;
        }

        public BindingList<SupplierInfo> GetSuppliersByName(string supplierName, bool showActiveOnly)
        {
            BindingList<SupplierInfo> retval = new BindingList<SupplierInfo>();
            SupplierDAL dal = new SupplierDAL();
            retval = dal.GetSuppliersByName(supplierName, showActiveOnly);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Supplier record from the database based on the SupplierId
        /// </summary>
        /// <param name="supplierId">Id of the Supplier</param>
        /// <returns>Instance of Supplier</returns>
        public SupplierInfo GetSupplier(int supplierId)
        {
            SupplierInfo retval = new SupplierInfo();
            SupplierDAL dal = new SupplierDAL();
            retval = dal.GetSupplier(supplierId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Supplier based on SupplierId
        /// </summary>
        /// <param name="supplierId">Id of the Supplier that is to be deleted</param>
        public void DeleteSupplier(int supplierId)
        {
            SupplierDAL dal = new SupplierDAL();
            dal.DeleteSupplier(supplierId);
            dal = null;
        }

        /// <summary>
        /// Checks if the Supplier name already exists in the suppliers table
        /// This will work for New Mode only(while adding a new supplier)
        /// </summary>
        /// <param name="supplierName">Name of the Supplier that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name)
        {
            int retVal = 0;
            SupplierDAL dal = new SupplierDAL();
            retVal = dal.GetSameNameCount(name);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the Supplier name already exists in the Suppliers table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="supplierName">Name of the Supplier that needs to be checked</param>
        /// <param name="supplierId">Id of the Supplier that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current supplier.
        /// 0 is returned if no records are found</returns>

        public int GetSameNameCountForEditMode(string name, int supplierId)
        {
            int retVal = 0;
            SupplierDAL dal = new SupplierDAL();
            retVal = dal.GetSameNameCountForEditMode(name, supplierId);
            dal = null;
            return retVal;
        }


        public bool CheckSupplierUsed(int supplierId, out string tableName)
        {
            SupplierDAL dal = new SupplierDAL();
            return dal.CheckSupplierUsed(supplierId, out tableName);
        }
    }
}