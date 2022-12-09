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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the ItemCompany table
    /// Author	: Kiran
    /// Date	: 06 Oct 2012 07:15:25 PM
    /// </summary>
    public class ItemCompanyDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public ItemCompanyDAL()
        {

        }
        /// <summary>
        /// Gets all the ItemCompanies from the ItemCompanies table
        /// </summary>
        /// <returns>BindingList of ItemCompanies</returns>
        public BindingList<ItemCompanyInfo> GetItemCompaniesAll(string name, bool all, bool orderByDisplayIndex)
        {
            BindingList<ItemCompanyInfo> retVal = new BindingList<ItemCompanyInfo>();
            DbCommand command;
            string orderByClause = string.Empty;

            if (orderByDisplayIndex)
                orderByClause = " ORDER BY DisplayIndex";
            else
                orderByClause = "ORDER BY Name";

            if (all)
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [DisplayName], [DisplayIndex], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ItemCompanies WHERE Name LIKE '%" + name + "%'" + orderByClause);
            else
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [DisplayName], [DisplayIndex], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ItemCompanies WHERE Name LIKE '%" + name + "%' AND IsActive = 'True'" + orderByClause);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    ItemCompanyInfo itemCompany = new ItemCompanyInfo();
                    itemCompany.ID = Convert.ToInt32(dataReader["ID"]);
                    itemCompany.Name = Convert.ToString(dataReader["Name"]);
                    itemCompany.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    itemCompany.DisplayIndex = Convert.ToByte(dataReader["DisplayIndex"]);
                    itemCompany.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    itemCompany.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    itemCompany.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    itemCompany.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    itemCompany.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(itemCompany);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single ItemCompany based on Id
        /// </summary>
        /// <param name="itemcompanyId">Id of the ItemCompany the needs to be retrieved</param>
        /// <returns>Instance of ItemCompany</returns>
        public ItemCompanyInfo GetItemCompany(int itemcompanyId)
        {
            ItemCompanyInfo retVal = new ItemCompanyInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [DisplayName], [DisplayIndex], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM ItemCompanies WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemcompanyId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.DisplayName = Convert.ToString(dataReader["DisplayName"]);
                    retVal.DisplayIndex = Convert.ToByte(dataReader["DisplayIndex"]);
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
        /// Adds new ItemCompany in to the database
        /// </summary>
        /// <param name="itemcompany">Instance of ItemCompany</param>
        /// <returns>Id of the newly added ItemCompany</returns>
        public int AddItemCompany(ItemCompanyInfo itemCompany)
        {
            int retval = 0;
            DbCommand commandItemCompany = _db.GetSqlStringCommand("INSERT INTO ItemCompanies([Name], [DisplayName], [DisplayIndex], [IsActive], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@Name, @DisplayName, @DisplayIndex, @IsActive, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('ItemCompanies')");

            _db.AddInParameter(commandItemCompany, "@Name", DbType.String, itemCompany.Name);
            _db.AddInParameter(commandItemCompany, "@DisplayName", DbType.String, itemCompany.DisplayName);
            _db.AddInParameter(commandItemCompany, "@DisplayIndex", DbType.Byte, itemCompany.DisplayIndex);
            _db.AddInParameter(commandItemCompany, "@IsActive", DbType.Boolean, itemCompany.IsActive);
            _db.AddInParameter(commandItemCompany, "@CreatedBy", DbType.Byte, itemCompany.CreatedBy);
            _db.AddInParameter(commandItemCompany, "@CreatedOn", DbType.DateTime, itemCompany.CreatedOn);
            _db.AddInParameter(commandItemCompany, "@UpdatedBy", DbType.Byte, itemCompany.UpdatedBy);
            _db.AddInParameter(commandItemCompany, "@UpdatedOn", DbType.DateTime, itemCompany.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(commandItemCompany));
            }
            return retval;
        }

        /// <summary>
        /// Updates the ItemCompany
        /// </summary>
        /// <param name="itemcompany">Instance of ItemCompany class</param>
        public void UpdateItemCompany(ItemCompanyInfo itemCompany)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE ItemCompanies SET [Name] = @Name, [DisplayName] = @DisplayName, [DisplayIndex] = @DisplayIndex, [IsActive] = @IsActive, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@ID", DbType.Int32, itemCompany.ID);
            _db.AddInParameter(command, "@Name", DbType.String, itemCompany.Name);
            _db.AddInParameter(command, "@DisplayName", DbType.String, itemCompany.DisplayName);
            _db.AddInParameter(command, "@DisplayIndex", DbType.Byte, itemCompany.DisplayIndex);
            _db.AddInParameter(command, "@IsActive", DbType.Boolean, itemCompany.IsActive);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, itemCompany.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, itemCompany.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Deletes the ItemCompany from the database
        /// </summary>
        /// <param name="itemcompanyId">Id of the ItemCompany that needs to be deleted</param>
        public void DeleteItemCompany(int itemcompanyId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM ItemCompanies " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemcompanyId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Gets the count of ItemCompanies having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="itemcompanyName">Name of the ItemCompany to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name, bool checkDisplayIndex)
        {
            int retVal = 0;
            DbCommand command = null;
            if (!checkDisplayIndex)
            {
                command = _db.GetSqlStringCommand(" Select Count([Name]) From [ItemCompanies] WHERE [Name] = @Name");
                _db.AddInParameter(command, "@Name", DbType.String, name);
            }
            else
            {
                command = _db.GetSqlStringCommand(" Select Count([DisplayIndex]) From [ItemCompanies] WHERE [DisplayIndex] = @DisplayIndex");
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
        /// Gets the count of ItemCompanies having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="itemcompanyName">Name of the ItemCompany to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int itemcompanyId, bool checkDisplayIndex)
        {
            int retVal = 0;
            DbCommand command = null;
            if (!checkDisplayIndex)
            {
                command = _db.GetSqlStringCommand(" Select Count([Name]) From [ItemCompanies] WHERE [Name] = @Name AND Id != @Id");
                _db.AddInParameter(command, "@Name", DbType.String, name);
                _db.AddInParameter(command, "@Id", DbType.Int32, itemcompanyId);
            }
            else
            {
                command = _db.GetSqlStringCommand(" Select Count([DisplayIndex]) From [ItemCompanies] WHERE [DisplayIndex] = @DisplayIndex AND Id != @Id");
                _db.AddInParameter(command, "@DisplayIndex", DbType.Int32, Convert.ToInt32(name));
                _db.AddInParameter(command, "@Id", DbType.Int32, itemcompanyId);
            }

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        public bool CheckItemCompanyUsed(int itemcompanyId)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select COUNT(*) FROM ItemGroups WHERE ItemCompanyID = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, itemcompanyId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));

                if (retVal > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}

