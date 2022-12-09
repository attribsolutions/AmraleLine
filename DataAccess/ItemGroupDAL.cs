using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

using DataObjects;

namespace DataAccess
{
    /// <summary>
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the ItemGroup table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:11:51 PM
    /// </summary>
    public class ItemGroupDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public ItemGroupDAL()
        {

        }
        /// <summary>
        /// Gets all the ItemGroups from the ItemGroups table
        /// </summary>
        /// <returns>BindingList of ItemGroups</returns>
        public BindingList<ItemGroupInfo> GetItemGroupsAll(int itemCompanyId)
        {
            BindingList<ItemGroupInfo> retVal = new BindingList<ItemGroupInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [DisplayName], [DisplayIndex], [CouponGroup], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ItemGroups WHERE IsActive = 'True' AND ItemCompanyID = @ItemCompanyID ORDER BY DisplayIndex");
            _db.AddInParameter(command, "@ItemCompanyID", DbType.Int32, itemCompanyId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ItemGroupInfo itemGroup = new ItemGroupInfo();
                    itemGroup.ID = Convert.ToInt32(dataReader["ID"]);
                    itemGroup.Name = Convert.ToString(dataReader["Name"]);
                    itemGroup.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    itemGroup.DisplayIndex = Convert.ToByte(dataReader["DisplayIndex"]);
                    itemGroup.CouponGroup = Convert.ToBoolean(dataReader["CouponGroup"]);
                    itemGroup.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    itemGroup.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    itemGroup.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    itemGroup.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(itemGroup);
                }
            }
            return retVal;
        }

        public BindingList<ItemGroupInfo> GetItemGroupsByName(string groupName)
        {
            BindingList<ItemGroupInfo> retVal = new BindingList<ItemGroupInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT ItemGroups.ID, ItemGroups.ItemCompanyID, ItemCompanies.Name ItemCompanyName, ItemGroups.Name, ItemGroups.DisplayName, ItemGroups.DisplayIndex, CouponGroup, ItemGroups.IsActive, ItemGroups.CreatedBy, ItemGroups.CreatedOn, ItemGroups.UpdatedBy, ItemGroups.UpdatedOn FROM ItemGroups JOIN ItemCompanies ON ItemGroups.ItemCompanyID = ItemCompanies.ID WHERE ItemGroups.Name LIKE '" + groupName + "%'ORDER BY ItemGroups.Name");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ItemGroupInfo itemGroup = new ItemGroupInfo();
                    itemGroup.ID = Convert.ToInt32(dataReader["ID"]);
                    itemGroup.ItemCompanyID = Convert.ToInt32(dataReader["ItemCompanyID"]);
                    itemGroup.ItemCompanyName = Convert.ToString(dataReader["ItemCompanyName"]);
                    itemGroup.Name = Convert.ToString(dataReader["Name"]);
                    itemGroup.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    itemGroup.DisplayIndex = Convert.ToByte(dataReader["DisplayIndex"]);
                    itemGroup.CouponGroup = Convert.ToBoolean(dataReader["CouponGroup"]);
                    itemGroup.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    itemGroup.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    itemGroup.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    itemGroup.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    itemGroup.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(itemGroup);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single ItemGroup based on Id
        /// </summary>
        /// <param name="itemgroupId">Id of the ItemGroup the needs to be retrieved</param>
        /// <returns>Instance of ItemGroup</returns>
        public ItemGroupInfo GetItemGroup(int itemgroupId)
        {
            ItemGroupInfo retVal = new ItemGroupInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [ItemCompanyID], [Name], [DisplayName], [DisplayIndex], [CouponGroup], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ItemGroups WHERE Id = @Id ");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemgroupId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.ItemCompanyID = Convert.ToInt32(dataReader["ItemCompanyID"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    retVal.DisplayIndex = Convert.ToByte(dataReader["DisplayIndex"]);
                    retVal.CouponGroup = Convert.ToBoolean(dataReader["CouponGroup"]);
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new ItemGroup in to the database
        /// </summary>
        /// <param name="itemgroup">Instance of ItemGroup</param>
        /// <returns>Id of the newly added ItemGroup</returns>
        public int AddItemGroup(ItemGroupInfo itemGroup)
        {
            int retval = 0;
            DbCommand commandItemGroup = _db.GetSqlStringCommand("INSERT INTO ItemGroups([ItemCompanyID], [Name], [DisplayName], [DisplayIndex], [CouponGroup], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@ItemCompanyID, @Name, @DisplayName, @DisplayIndex, @CouponGroup, @IsActive, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('ItemGroups')");

            _db.AddInParameter(commandItemGroup, "@ItemCompanyID", DbType.Int32, itemGroup.ItemCompanyID);
            _db.AddInParameter(commandItemGroup, "@Name", DbType.String, itemGroup.Name);
            _db.AddInParameter(commandItemGroup, "@DisplayName", DbType.String, itemGroup.DisplayName);
            _db.AddInParameter(commandItemGroup, "@DisplayIndex", DbType.Byte, itemGroup.DisplayIndex);
            _db.AddInParameter(commandItemGroup, "@CouponGroup", DbType.Boolean, itemGroup.CouponGroup);
            _db.AddInParameter(commandItemGroup, "@IsActive", DbType.Boolean, itemGroup.IsActive);
            _db.AddInParameter(commandItemGroup, "@CreatedBy", DbType.Byte, itemGroup.CreatedBy);
            _db.AddInParameter(commandItemGroup, "@CreatedOn", DbType.DateTime, itemGroup.CreatedOn);
            _db.AddInParameter(commandItemGroup, "@UpdatedBy", DbType.Byte, itemGroup.UpdatedBy);
            _db.AddInParameter(commandItemGroup, "@UpdatedOn", DbType.DateTime, itemGroup.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandItemGroup));
            }
            return retval;
        }

        /// <summary>
        /// Updates the ItemGroup
        /// </summary>
        /// <param name="itemgroup">Instance of ItemGroup class</param>
        public void UpdateItemGroup(ItemGroupInfo itemGroup)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE ItemGroups SET [ItemCompanyID] = @ItemCompanyID, [Name] = @Name, [DisplayName] = @DisplayName, [DisplayIndex] = @DisplayIndex, [CouponGroup] = @CouponGroup, [IsActive] = @IsActive, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, itemGroup.ID);
            _db.AddInParameter(command, "@ItemCompanyID", DbType.Int32, itemGroup.ItemCompanyID);
            _db.AddInParameter(command, "@Name", DbType.String, itemGroup.Name);
            _db.AddInParameter(command, "@DisplayName", DbType.String, itemGroup.DisplayName);
            _db.AddInParameter(command, "@DisplayIndex", DbType.Byte, itemGroup.DisplayIndex);
            _db.AddInParameter(command, "@CouponGroup", DbType.Boolean, itemGroup.CouponGroup);
            _db.AddInParameter(command, "@IsActive", DbType.Boolean, itemGroup.IsActive);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, itemGroup.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, itemGroup.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the ItemGroup from the database
        /// </summary>
        /// <param name="itemgroupId">Id of the ItemGroup that needs to be deleted</param>
        public void DeleteItemGroup(int itemgroupId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM ItemGroups WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemgroupId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Gets the count of ItemGroups having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="itemgroupName">Name of the ItemGroup to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name, bool checkDisplayIndex)
        {
            int retVal = 0;
            DbCommand command = null;
            if (!checkDisplayIndex)
            {
                command = _db.GetSqlStringCommand(" Select Count([Name]) From [ItemGroups] WHERE [Name] = @Name");
                _db.AddInParameter(command, "@Name", DbType.String, name);
            }
            else
            {
                command = _db.GetSqlStringCommand(" Select Count([DisplayIndex]) From [ItemGroups] WHERE [DisplayIndex] = @DisplayIndex");
                _db.AddInParameter(command, "@DisplayIndex", DbType.Int32, Convert.ToInt32(name));
            }

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the count of ItemGroups having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="itemgroupName">Name of the ItemGroup to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int itemgroupId, bool checkDisplayIndex)
        {
            int retVal = 0;
            DbCommand command = null;
            if (!checkDisplayIndex)
            {
                command = _db.GetSqlStringCommand(" Select Count([Name]) From [ItemGroups] WHERE [Name] = @Name AND Id != @Id");
                _db.AddInParameter(command, "@Name", DbType.String, name);
                _db.AddInParameter(command, "@Id", DbType.Int32, itemgroupId);
            }
            else
            {
                command = _db.GetSqlStringCommand(" Select Count([DisplayIndex]) From [ItemGroups] WHERE [DisplayIndex] = @DisplayIndex AND Id != @Id");
                _db.AddInParameter(command, "@DisplayIndex", DbType.Int32, Convert.ToInt32(name));
                _db.AddInParameter(command, "@Id", DbType.Int32, itemgroupId);
            }

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }


        public bool CheckItemGroupUsed(int groupId)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(ItemGroupID) FROM Items WHERE ItemGroupID = @ItemGroupId");
            _db.AddInParameter(command, "@ItemGroupId", DbType.Int32, groupId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                int cnt = Convert.ToInt32(_db.ExecuteScalar(command));

                if (cnt > 0)
                    return true;
            }
            return false;
        }

        public int CheckCouponGroupCount()
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM ItemGroups WHERE CouponGroup = 'True'");

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }
    }
}