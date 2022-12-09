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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Item table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:12:33 PM
    /// </summary>
    public class ItemDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public ItemDAL()
        {

        }
        /// <summary>
        /// Gets all the Items from the Items table
        /// </summary>
        /// <returns>BindingList of Items</returns>
        public BindingList<ItemInfo> GetItemsAll()
        {
            BindingList<ItemInfo> retVal = new BindingList<ItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT Items.ID, Items.ItemCode, Items.MainBranchCode, Items.BarCode, Items.Name, Items.DisplayName, Items.ItemGroupID, ItemGroups.Name ItemGroup, Items.UnitID, Units.Name Unit, Items.Gst,  Items.Rate, Items.IsActive, Items.ShowFixed, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE Items.IsActive = 'True' ORDER BY Items.Name");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ItemInfo item = new ItemInfo();
                    item.ID = Convert.ToInt32(dataReader["ID"]);
                    item.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    if (dataReader["MainBranchCode"] == DBNull.Value)
                    {
                        item.MainBranchCode = 0;
                    }
                    else
                    {
                        item.MainBranchCode = Convert.ToInt32(dataReader["MainBranchCode"]);
                    }
                    if (dataReader["BarCode"] == DBNull.Value)
                    {
                        item.BarCode = string.Empty;
                    }
                    else
                    {
                        item.BarCode = Convert.ToString(dataReader["BarCode"]);
                    }
                    item.Name = Convert.ToString(dataReader["Name"]);
                    item.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    item.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    item.ItemGroup = Convert.ToString(dataReader["ItemGroup"]);
                    item.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    item.Unit = Convert.ToString(dataReader["Unit"]);
                    item.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    
                    item.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    
                    item.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    item.ShowFixed = Convert.ToBoolean(dataReader["ShowFixed"]);
                    item.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    item.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    item.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    item.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(item);
                }
            }
            return retVal;
        }

        public BindingList<ItemInfo> GetItemsByFilter(int searchType, string itemName, int count)
        {
            BindingList<ItemInfo> retVal = new BindingList<ItemInfo>();
            DbCommand command = null;
            string cmdString = "SELECT TOP " + count + " Items.ID, Items.ItemCode, Items.MainBranchCode, Items.BarCode, Items.Name, Items.DisplayName, Items.DisplayIndex, Items.ItemGroupID, ItemGroups.Name ItemGroup, UnitID, Units.Name Unit, Items.Gst, Items.Rate, Items.UnitWeight, Items.IsActive, Items.ShowFixed, Items.FixedDisplayIndex, Items.IsRoundOff, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID ";

            if (searchType == 1)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Items.Name LIKE '%" + itemName.Trim() + "%' ORDER BY Items.Name");
            if (searchType == 2)
                command = _db.GetSqlStringCommand(cmdString + "WHERE ItemGroups.Name LIKE '" + itemName.Trim() + "%' ORDER BY ItemGroups.Name");
            if (searchType == 3 && itemName != string.Empty)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Items.ItemCode = @ItemCode");
            if (searchType == 3 && itemName == string.Empty)
                command = _db.GetSqlStringCommand(cmdString + " ORDER BY Items.ItemCode");

            if (searchType == 4 && itemName != string.Empty)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Items.MainBranchCode = @ItemCode");
            if (searchType == 4 && itemName == string.Empty)
                command = _db.GetSqlStringCommand(cmdString + " ORDER BY Items.MainBranchCode");

            if ((searchType == 3 || searchType == 4) && itemName != string.Empty)
                _db.AddInParameter(command, "ItemCode", DbType.Int32, Convert.ToInt32(itemName));

            if (searchType == 11)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Items.Name LIKE '" + itemName.Trim() + "%' ORDER BY Items.Name");

            if (searchType == 102)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Items.IsActive = 'True' AND ShowFixed = 'True' ORDER BY Items.FixedDisplayIndex");
            if (searchType == 103)
                command = _db.GetSqlStringCommand(cmdString + "WHERE Items.IsActive = 'True' AND ShowFixed = 'False' AND ItemGroups.CouponGroup = 'False' ORDER BY Items." + itemName.Trim() + " DESC");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ItemInfo item = new ItemInfo();
                    item.ID = Convert.ToInt32(dataReader["ID"]);
                    item.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    if (dataReader["MainBranchCode"] == DBNull.Value)
                    {
                        item.MainBranchCode = 0;
                    }
                    else
                    {
                        item.MainBranchCode = Convert.ToInt32(dataReader["MainBranchCode"]);
                    }
                    if (dataReader["BarCode"] == DBNull.Value)
                    {
                        item.BarCode = string.Empty;
                    }
                    else
                    {
                        item.BarCode = Convert.ToString(dataReader["BarCode"]);
                    }
                    item.Name = Convert.ToString(dataReader["Name"]);
                    item.ItemGroup = Convert.ToString(dataReader["ItemGroup"]);
                    item.Unit = Convert.ToString(dataReader["Unit"]);
                    item.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    item.DisplayIndex = Convert.ToInt32(dataReader["DisplayIndex"]);
                    item.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    item.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    item.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    
                    item.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    
                    item.UnitWeight = Convert.ToDecimal(dataReader["UnitWeight"]);
                    item.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    item.ShowFixed = Convert.ToBoolean(dataReader["ShowFixed"]);
                    item.FixedDisplayIndex = Convert.ToInt32(dataReader["FixedDisplayIndex"]);
                    item.IsRoundOff = Convert.ToBoolean(dataReader["IsRoundOff"]);
                    item.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    item.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    item.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    item.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(item);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Item based on Id
        /// </summary>
        /// <param name="itemId">Id of the Item the needs to be retrieved</param>
        /// <returns>Instance of Item</returns>
        public ItemInfo GetItem(int itemId)
        {
            ItemInfo retVal = new ItemInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT Items.ID, Items.ItemCode, Items.MainBranchCode, Items.BarCode, Items.Name, Items.DisplayName, Items.ItemGroupID, Items.UnitID, Units.Name Unit, Items.Gst,  Items.Rate, Items.UnitWeight, Items.IsActive, Items.ShowFixed, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN Units ON Items.UnitID = Units.ID WHERE Items.Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    if (dataReader["MainBranchCode"] == DBNull.Value)
                    {
                        retVal.MainBranchCode = 0;
                    }
                    else
                    {
                        retVal.MainBranchCode = Convert.ToInt32(dataReader["MainBranchCode"]);
                    }
                    if (dataReader["BarCode"] == DBNull.Value)
                    {
                        retVal.BarCode = string.Empty;
                    }
                    else
                    {
                        retVal.BarCode = Convert.ToString(dataReader["BarCode"]);
                    }
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    retVal.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    retVal.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    retVal.Unit = Convert.ToString(dataReader["Unit"]);
                    retVal.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    
                    retVal.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    
                    retVal.UnitWeight = Convert.ToDecimal(dataReader["UnitWeight"]);
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.ShowFixed = Convert.ToBoolean(dataReader["ShowFixed"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new Item in to the database
        /// </summary>
        /// <param name="item">Instance of Item</param>
        /// <returns>Id of the newly added Item</returns>
        public int AddItem(ItemInfo item)
        {
            int retval = 0;
            DbCommand commandItem = _db.GetSqlStringCommand("INSERT INTO Items([ItemCode], [MainBranchCode], [BarCode], [Name], [DisplayName], [DisplayIndex], [ItemGroupID], [UnitID],[Gst],[Rate], [UnitWeight], [IsActive], [ShowFixed], [CounterID], [FixedDisplayIndex], [IsRoundOff], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@ItemCode, @MainBranchCode, @BarCode, @Name, @DisplayName, @DisplayIndex, @ItemGroupID, @UnitID,@Gst,@Rate, @UnitWeight, @IsActive, @ShowFixed, @CounterID, @FixedDisplayIndex, @IsRoundOff, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Items')");

            _db.AddInParameter(commandItem, "@ItemCode", DbType.Int32, item.ItemCode);
            if (item.MainBranchCode == 0)
            {
                _db.AddInParameter(commandItem, "@MainBranchCode", DbType.Int32, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandItem, "@MainBranchCode", DbType.Int32, item.MainBranchCode);
            }
            if (item.BarCode == string.Empty)
            {
                _db.AddInParameter(commandItem, "@BarCode", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandItem, "@BarCode", DbType.String, item.BarCode);
            }
            _db.AddInParameter(commandItem, "@Name", DbType.String, item.Name);
            _db.AddInParameter(commandItem, "@DisplayName", DbType.String, item.DisplayName);
            _db.AddInParameter(commandItem, "@DisplayIndex", DbType.Int32, item.DisplayIndex);
            _db.AddInParameter(commandItem, "@ItemGroupID", DbType.Int32, item.ItemGroupID);
            _db.AddInParameter(commandItem, "@UnitID", DbType.Int32, item.UnitID);
            _db.AddInParameter(commandItem, "@Gst", DbType.Decimal, item.Gst);
            _db.AddInParameter(commandItem, "@Rate", DbType.Decimal, item.Rate);
            _db.AddInParameter(commandItem, "@UnitWeight", DbType.Decimal, item.UnitWeight);
            _db.AddInParameter(commandItem, "@IsActive", DbType.Boolean, item.IsActive);
            _db.AddInParameter(commandItem, "@ShowFixed", DbType.Boolean, item.ShowFixed);
             if (item.CounterID == 0)
            {
                _db.AddInParameter(commandItem, "@CounterID", DbType.Int32, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandItem, "@CounterID", DbType.Int32, item.CounterID);
            }

             _db.AddInParameter(commandItem, "@FixedDisplayIndex", DbType.Int32, item.FixedDisplayIndex);
             _db.AddInParameter(commandItem, "@IsRoundOff", DbType.Boolean, item.IsRoundOff);
            _db.AddInParameter(commandItem, "@CreatedBy", DbType.Byte, item.CreatedBy);
            _db.AddInParameter(commandItem, "@CreatedOn", DbType.DateTime, item.CreatedOn);
            _db.AddInParameter(commandItem, "@UpdatedBy", DbType.Byte, item.UpdatedBy);
            _db.AddInParameter(commandItem, "@UpdatedOn", DbType.DateTime, item.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandItem));
            }
            return retval;
        }

        /// <summary>
        /// Updates the Item
        /// </summary>
        /// <param name="item">Instance of Item class</param>
        public void UpdateItem(ItemInfo item)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Items SET [ItemCode] = @ItemCode, [MainBranchCode] = @MainBranchCode, [BarCode] = @BarCode, [Name] = @Name, [DisplayName] = @DisplayName, DisplayIndex = @DisplayIndex, [ItemGroupID] = @ItemGroupID, [UnitID] = @UnitID,[Gst]=@Gst,[Rate]=@Rate, [UnitWeight] = @UnitWeight, [IsActive] = @IsActive, [ShowFixed] = @ShowFixed, FixedDisplayIndex = @FixedDisplayIndex, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, item.ID);
            _db.AddInParameter(command, "@ItemCode", DbType.Int32, item.ItemCode);
            if (item.MainBranchCode == 0)
            {
                _db.AddInParameter(command, "@MainBranchCode", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@MainBranchCode", DbType.String, item.MainBranchCode);
            } 
            if (item.BarCode == string.Empty)
            {
                _db.AddInParameter(command, "@BarCode", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@BarCode", DbType.String, item.BarCode);
            }
            _db.AddInParameter(command, "@Name", DbType.String, item.Name);
            _db.AddInParameter(command, "@DisplayName", DbType.String, item.DisplayName);
            _db.AddInParameter(command, "@DisplayIndex", DbType.Int32, item.DisplayIndex);
            _db.AddInParameter(command, "@ItemGroupID", DbType.Int32, item.ItemGroupID);
            _db.AddInParameter(command, "@UnitID", DbType.Int32, item.UnitID);
            _db.AddInParameter(command, "@Gst", DbType.Decimal, item.Gst);
            _db.AddInParameter(command, "@Rate", DbType.Decimal, item.Rate);
            _db.AddInParameter(command, "@UnitWeight", DbType.Decimal, item.UnitWeight);
            _db.AddInParameter(command, "@IsActive", DbType.Boolean, item.IsActive);
            _db.AddInParameter(command, "@ShowFixed", DbType.Boolean, item.ShowFixed);
            _db.AddInParameter(command, "@FixedDisplayIndex", DbType.Int32, item.FixedDisplayIndex);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, item.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, item.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the Item from the database
        /// </summary>
        /// <param name="itemId">Id of the Item that needs to be deleted</param>
        public void DeleteItem(int itemId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Items WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Gets the count of Items having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="itemName">Name of the Item to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name, byte nameCode)
        {
            int retVal = 0;

            DbCommand command = null;
            if (nameCode == 1)
                command = _db.GetSqlStringCommand(" Select Count([Name]) From [Items] WHERE [Name] = @Name");
            if (nameCode == 2)
                command = _db.GetSqlStringCommand(" Select Count([ItemCode]) From [Items] WHERE [ItemCode] = @ItemCode");
            if (nameCode == 3)
                command = _db.GetSqlStringCommand(" Select Count([MainBranchCode]) From [Items] WHERE [MainBranchCode] = @ItemCode");
            if (nameCode == 4)
                command = _db.GetSqlStringCommand(" Select Count([BarCode]) From [Items] WHERE [BarCode] = @BarCode");
            if (nameCode == 5)
                command = _db.GetSqlStringCommand(" Select Count(DisplayIndex) From [Items] WHERE [DisplayIndex] = @DisplayIndex AND ItemGroupID = @ItemGroupID");
            if (nameCode == 6)
                command = _db.GetSqlStringCommand(" Select Count(FixedDisplayIndex) From [Items] WHERE [FixedDisplayIndex] = @FixedDisplayIndex");

            if (nameCode == 1)
                _db.AddInParameter(command, "@Name", DbType.String, name);
            if (nameCode == 2 || nameCode == 3)
                _db.AddInParameter(command, "@ItemCode", DbType.Int32, Convert.ToInt32(name));
            if (nameCode == 4)
                _db.AddInParameter(command, "@BarCode", DbType.String, name.Trim());
            if (nameCode == 5)
            {
                _db.AddInParameter(command, "@DisplayIndex", DbType.Int32, Convert.ToInt32((name.Split(','))[0]));
                _db.AddInParameter(command, "@ItemGroupID", DbType.Int32, Convert.ToInt32((name.Split(','))[1]));
            }
             if (nameCode == 6)
                _db.AddInParameter(command, "@FixedDisplayIndex", DbType.Int32, Convert.ToInt32(name));

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the count of Items having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="itemName">Name of the Item to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int itemId, byte nameCode)
        {
            int retVal = 0;
            DbCommand command = null;
            if (nameCode == 1)
                command = _db.GetSqlStringCommand(" Select Count([Name]) From [Items] WHERE [Name] = @Name AND Id != @Id");
            if (nameCode == 2)
                command = _db.GetSqlStringCommand(" Select Count([ItemCode]) From [Items] WHERE [ItemCode] = @ItemCode AND Id != @Id");
            if (nameCode == 3)
                command = _db.GetSqlStringCommand(" Select Count([MainBranchCode]) From [Items] WHERE [MainBranchCode] = @ItemCode AND Id != @Id");
            if (nameCode == 4)
                command = _db.GetSqlStringCommand(" Select Count([BarCode]) From [Items] WHERE [BarCode] = @BarCode AND Id != @Id");
            if (nameCode == 5)
                command = _db.GetSqlStringCommand(" Select Count([DisplayIndex]) From [Items] WHERE [DisplayIndex] = @DisplayIndex AND ItemGroupID = @ItemGroupID AND Id != @Id");
            if (nameCode == 6)
                command = _db.GetSqlStringCommand(" Select Count([FixedDisplayIndex]) From [Items] WHERE [FixedDisplayIndex] = @FixedDisplayIndex AND Id != @Id");

            if (nameCode == 1)
                _db.AddInParameter(command, "@Name", DbType.String, name);
            if (nameCode == 2 || nameCode == 3)
                _db.AddInParameter(command, "@ItemCode", DbType.Int32, Convert.ToInt32(name));
            if (nameCode == 4)
                _db.AddInParameter(command, "@BarCode", DbType.String, name.Trim());
            if (nameCode == 5)
            {
                _db.AddInParameter(command, "@DisplayIndex", DbType.Int32, Convert.ToInt32((name.Split(','))[0]));
                _db.AddInParameter(command, "@ItemGroupID", DbType.Int32, Convert.ToInt32((name.Split(','))[1]));
            }
            if (nameCode == 6)
                _db.AddInParameter(command, "@FixedDisplayIndex", DbType.Int32, Convert.ToInt32(name));
            _db.AddInParameter(command, "@Id", DbType.Int32, itemId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        public bool CheckItemUsed(int itemId, out string tableName)
        {
            
            DbCommand command2 = _db.GetSqlStringCommand("SELECT COUNT(ItemID) FROM Stock WHERE ItemID = @ItemID");
            _db.AddInParameter(command2, "@ItemID", DbType.Int32, itemId);
            DbCommand command3 = _db.GetSqlStringCommand("SELECT COUNT(ItemID) FROM StockAdjustments WHERE ItemID = @ItemID");
            _db.AddInParameter(command3, "@ItemID", DbType.Int32, itemId);
            DbCommand command4 = _db.GetSqlStringCommand("SELECT COUNT(ItemID) FROM SaleItems WHERE ItemID = @ItemID");
            _db.AddInParameter(command4, "@ItemID", DbType.Int32, itemId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
              
                 if (Convert.ToInt32(_db.ExecuteScalar(command2)) > 0)
                {
                    tableName = "Stocks";
                    return true;
                }
                else if (Convert.ToInt32(_db.ExecuteScalar(command3)) > 0)
                {
                    tableName = "Stock Adjustment";
                    return true;
                }
                else if (Convert.ToInt32(_db.ExecuteScalar(command4)) > 0)
                {
                    tableName = "Sale Items";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }

        public BindingList<ItemInfo> GetItemsAllByGroupID(int groupID, string counter)
        {
            BindingList<ItemInfo> retVal = new BindingList<ItemInfo>();
            //DbCommand command = _db.GetSqlStringCommand("SELECT Items.ID, Items.ItemCode, Items.MainBranchCode, Items.BarCode, Items.Name, Items.DisplayName, Items.ItemGroupID, ItemGroups.Name ItemGroup, Items.UnitID, Units.Name Unit, Items.Gst, Items.LastPurchaseRate, Items.Rate, Items.ReorderQuantity, Items.IsActive, Items.ShowFixed, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE Items.IsActive = 'True' AND Items.ItemGroupID = @GroupId AND Items.ShowFixed = 'False' ORDER BY Items.DisplayIndex");
            DbCommand command = _db.GetSqlStringCommand("SELECT Items.ID, Items.ItemCode, Items.MainBranchCode, Items.BarCode, Items.Name, Items.DisplayName, Items.ItemGroupID, ItemGroups.Name ItemGroup, Items.UnitID, Units.Name Unit, Items.Gst, Items.Rate, Items.IsActive, Items.ShowFixed, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE Items.IsActive = 'True' AND Items.ItemGroupID = @GroupId ORDER BY Items.DisplayIndex");
            _db.AddInParameter(command, "@GroupId", DbType.Int32, groupID);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ItemInfo item = new ItemInfo();
                    item.ID = Convert.ToInt32(dataReader["ID"]);
                    item.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    if (dataReader["MainBranchCode"] == DBNull.Value)
                    {
                        item.MainBranchCode = 0;
                    }
                    else
                    {
                        item.MainBranchCode = Convert.ToInt32(dataReader["MainBranchCode"]);
                    }
                    if (dataReader["BarCode"] == DBNull.Value)
                    {
                        item.BarCode = string.Empty;
                    }
                    else
                    {
                        item.BarCode = Convert.ToString(dataReader["BarCode"]);
                    }
                    item.Name = Convert.ToString(dataReader["Name"]);
                    item.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    item.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    item.ItemGroup = Convert.ToString(dataReader["ItemGroup"]);
                    item.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    item.Unit = Convert.ToString(dataReader["Unit"]);
                    item.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    
                    item.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    
                    item.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    item.ShowFixed = Convert.ToBoolean(dataReader["ShowFixed"]);
                    item.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    item.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    item.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    item.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(item);
                }
            }
            return retVal;
        }

        public ItemInfo GetItemByBarCode(string barCode)
        {
            ItemInfo retVal = new ItemInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT Items.ID, Items.ItemCode, Items.BarCode, Items.Name, Items.DisplayName, Items.ItemGroupID, ItemGroups.Name ItemGroup, Items.UnitID, Units.Name Unit, Items.Gst, Items.Rate, Items.IsActive, Items.ShowFixed, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE Items.BarCode = @BarCode");
            _db.AddInParameter(command, "@BarCode", DbType.String, barCode);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    if (dataReader["BarCode"] == DBNull.Value)
                    {
                        retVal.BarCode = string.Empty;
                    }
                    else
                    {
                        retVal.BarCode = Convert.ToString(dataReader["BarCode"]);
                    }
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    retVal.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    retVal.ItemGroup = Convert.ToString(dataReader["ItemGroup"]);
                    retVal.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    retVal.Unit = Convert.ToString(dataReader["Unit"]);
                    retVal.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    
                    retVal.Rate = Convert.ToDecimal(dataReader["Rate"]);
                   
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.ShowFixed = Convert.ToBoolean(dataReader["ShowFixed"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        public bool CheckFixedItemCount(int count)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM Items WHERE ShowFixed = 'True'");

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                int cnt = Convert.ToInt32(_db.ExecuteScalar(command));

                if (cnt >= count)
                    return true;
            }
            return false;
        }

        public ItemInfo GetItemByItemCode(int itemCode)
        {
            ItemInfo retVal = new ItemInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT Items.ID, Items.ItemCode, Items.BarCode, Items.Name, Items.DisplayName, Items.ItemGroupID, Items.UnitID, Units.Name Unit, Items.Gst, Items.Rate, Items.IsActive, Items.ShowFixed, Items.CreatedBy, Items.CreatedOn, Items.UpdatedBy, Items.UpdatedOn FROM Items INNER JOIN Units ON Items.UnitID = Units.ID WHERE Items.ItemCode = @ItemCode");
            _db.AddInParameter(command, "@ItemCode", DbType.Int32, itemCode);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    if (dataReader["BarCode"] == DBNull.Value)
                    {
                        retVal.BarCode = string.Empty;
                    }
                    else
                    {
                        retVal.BarCode = Convert.ToString(dataReader["BarCode"]);
                    }
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    retVal.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    retVal.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    retVal.Unit = Convert.ToString(dataReader["Unit"]);
                    retVal.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    
                    retVal.Rate = Convert.ToDecimal(dataReader["Rate"]);
                   
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.ShowFixed = Convert.ToBoolean(dataReader["ShowFixed"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        public int GetMaxItemID(byte codeID)
        {
            DbCommand command = null;
            if (codeID == 1)
                command = _db.GetSqlStringCommand("SELECT ISNULL(Max(ID), 0) FROM Items");
            if (codeID == 2)
                command = _db.GetSqlStringCommand("SELECT ISNULL(Max(ItemCode), 0) FROM Items");

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                return Convert.ToInt32(_db.ExecuteScalar(command));
            }
        }
    }
}