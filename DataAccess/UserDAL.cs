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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the User table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:19:39 PM
    /// </summary>
    public class UserDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public UserDAL()
        {

        }
        /// <summary>
        /// Gets all the Users from the Users table
        /// </summary>
        /// <returns>BindingList of Users</returns>
        public BindingList<UserInfo> GetUsersAll(bool showActiveOnly)
        {
            BindingList<UserInfo> retVal = new BindingList<UserInfo>();
            DbCommand command = null;
            if (showActiveOnly)
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users WHERE IsActive = 'True' ORDER BY [Name]");
            else
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.ID = Convert.ToInt32(dataReader["ID"]);
                    user.Name = Convert.ToString(dataReader["Name"]);
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        user.Mobile = string.Empty;
                    }
                    else
                    {
                        user.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        user.EMail = string.Empty;
                    }
                    else
                    {
                        user.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    user.IsSystemUser = Convert.ToBoolean(dataReader["IsSystemUser"]);
                    if (dataReader["LoginName"] == DBNull.Value)
                    {
                        user.LoginName = string.Empty;
                    }
                    else
                    {
                        user.LoginName = Convert.ToString(dataReader["LoginName"]);
                    }
                    if (dataReader["Password"] == DBNull.Value)
                    {
                        user.Password = string.Empty;
                    }
                    else
                    {
                        user.Password = Convert.ToString(dataReader["Password"]);
                    }
                    if (dataReader["UserRole"] == DBNull.Value)
                    {
                        user.UserRole = string.Empty;
                    }
                    else
                    {
                        user.UserRole = Convert.ToString(dataReader["UserRole"]);
                    }
                    user.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    if (dataReader["CardID"] == DBNull.Value)
                    {
                        user.CardID = string.Empty;
                    }
                    else
                    {
                        user.CardID = Convert.ToString(dataReader["CardID"]);
                    }
                    user.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    user.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    user.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    user.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(user);
                }
            }
            return retVal;
        }

        public BindingList<UserInfo> GetAllSystemUsers(bool showActiveOnly)
        {
            BindingList<UserInfo> retVal = new BindingList<UserInfo>();
            DbCommand command = null;
            if (showActiveOnly)
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users WHERE IsActive = 'True' AND [IsSystemUser] = 'True' ORDER BY [Name]");
            else
                command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users WHERE [IsSystemUser] = 'True' ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.ID = Convert.ToInt32(dataReader["ID"]);
                    user.Name = Convert.ToString(dataReader["Name"]);
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        user.Mobile = string.Empty;
                    }
                    else
                    {
                        user.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        user.EMail = string.Empty;
                    }
                    else
                    {
                        user.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    user.IsSystemUser = Convert.ToBoolean(dataReader["IsSystemUser"]);
                    if (dataReader["LoginName"] == DBNull.Value)
                    {
                        user.LoginName = string.Empty;
                    }
                    else
                    {
                        user.LoginName = Convert.ToString(dataReader["LoginName"]);
                    }
                    if (dataReader["Password"] == DBNull.Value)
                    {
                        user.Password = string.Empty;
                    }
                    else
                    {
                        user.Password = Convert.ToString(dataReader["Password"]);
                    }
                    if (dataReader["UserRole"] == DBNull.Value)
                    {
                        user.UserRole = string.Empty;
                    }
                    else
                    {
                        user.UserRole = Convert.ToString(dataReader["UserRole"]);
                    }
                    user.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    if (dataReader["CardID"] == DBNull.Value)
                    {
                        user.CardID = string.Empty;
                    }
                    else
                    {
                        user.CardID = Convert.ToString(dataReader["CardID"]);
                    }
                    user.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    user.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    user.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    user.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(user);
                }
            }
            return retVal;
        }

        public BindingList<UserInfo> GetUsersByName(string userName)
        {
            BindingList<UserInfo> retVal = new BindingList<UserInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users WHERE Name LIKE '" + userName + "%' ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.ID = Convert.ToInt32(dataReader["ID"]);
                    user.Name = Convert.ToString(dataReader["Name"]);
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        user.Mobile = string.Empty;
                    }
                    else
                    {
                        user.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        user.EMail = string.Empty;
                    }
                    else
                    {
                        user.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    user.IsSystemUser = Convert.ToBoolean(dataReader["IsSystemUser"]);
                    if (dataReader["LoginName"] == DBNull.Value)
                    {
                        user.LoginName = string.Empty;
                    }
                    else
                    {
                        user.LoginName = Convert.ToString(dataReader["LoginName"]);
                    }
                    if (dataReader["Password"] == DBNull.Value)
                    {
                        user.Password = string.Empty;
                    }
                    else
                    {
                        user.Password = Convert.ToString(dataReader["Password"]);
                    }
                    if (dataReader["UserRole"] == DBNull.Value)
                    {
                        user.UserRole = string.Empty;
                    }
                    else
                    {
                        user.UserRole = Convert.ToString(dataReader["UserRole"]);
                    }
                    user.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    if (dataReader["CardID"] == DBNull.Value)
                    {
                        user.CardID = string.Empty;
                    }
                    else
                    {
                        user.CardID = Convert.ToString(dataReader["CardID"]);
                    }
                    user.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    user.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    user.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    user.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(user);
                }
            }
            return retVal;
        }

        public UserInfo GetUsersByCardID(String cardID)
        {
            UserInfo user = new UserInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users WHERE CardID = '" + cardID + "' ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    user.ID = Convert.ToInt32(dataReader["ID"]);
                    user.Name = Convert.ToString(dataReader["Name"]);
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        user.Mobile = string.Empty;
                    }
                    else
                    {
                        user.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        user.EMail = string.Empty;
                    }
                    else
                    {
                        user.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    user.IsSystemUser = Convert.ToBoolean(dataReader["IsSystemUser"]);
                    if (dataReader["LoginName"] == DBNull.Value)
                    {
                        user.LoginName = string.Empty;
                    }
                    else
                    {
                        user.LoginName = Convert.ToString(dataReader["LoginName"]);
                    }
                    if (dataReader["Password"] == DBNull.Value)
                    {
                        user.Password = string.Empty;
                    }
                    else
                    {
                        user.Password = Convert.ToString(dataReader["Password"]);
                    }
                    if (dataReader["UserRole"] == DBNull.Value)
                    {
                        user.UserRole = string.Empty;
                    }
                    else
                    {
                        user.UserRole = Convert.ToString(dataReader["UserRole"]);
                    }
                    user.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    if (dataReader["CardID"] == DBNull.Value)
                    {
                        user.CardID = string.Empty;
                    }
                    else
                    {
                        user.CardID = Convert.ToString(dataReader["CardID"]);
                    }
                    user.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    user.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    user.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    user.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return user;
        }

        /// <summary>
        /// Gets single User based on Id
        /// </summary>
        /// <param name="userId">Id of the User the needs to be retrieved</param>
        /// <returns>Instance of User</returns>
        public UserInfo GetUser(int userId)
        {
            UserInfo retVal = new UserInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Users WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, userId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        retVal.Mobile = string.Empty;
                    }
                    else
                    {
                        retVal.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        retVal.EMail = string.Empty;
                    }
                    else
                    {
                        retVal.EMail = Convert.ToString(dataReader["EMail"]);
                    }
                    retVal.IsSystemUser = Convert.ToBoolean(dataReader["IsSystemUser"]);
                    if (dataReader["LoginName"] == DBNull.Value)
                    {
                        retVal.LoginName = string.Empty;
                    }
                    else
                    {
                        retVal.LoginName = Convert.ToString(dataReader["LoginName"]);
                    }
                    if (dataReader["Password"] == DBNull.Value)
                    {
                        retVal.Password = string.Empty;
                    }
                    else
                    {
                        retVal.Password = Convert.ToString(dataReader["Password"]);
                    }
                    if (dataReader["UserRole"] == DBNull.Value)
                    {
                        retVal.UserRole = string.Empty;
                    }
                    else
                    {
                        retVal.UserRole = Convert.ToString(dataReader["UserRole"]);
                    }
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    if (dataReader["CardID"] == DBNull.Value)
                    {
                        retVal.CardID = string.Empty;
                    }
                    else
                    {
                        retVal.CardID = Convert.ToString(dataReader["CardID"]);
                    }
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new User in to the database
        /// </summary>
        /// <param name="user">Instance of User</param>
        /// <returns>Id of the newly added User</returns>
        public int AddUser(UserInfo user, BindingList<DivisionInfo> division)
        {
            
            
            int retval = 0;
            DbCommand commandUser = _db.GetSqlStringCommand("INSERT INTO Users([Name], [Mobile], [EMail], [IsSystemUser], [LoginName], [Password], [UserRole], [IsActive], [CardID], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@Name, @Mobile, @EMail, @IsSystemUser, @LoginName, @Password, @UserRole, @IsActive, @CardID, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Users')");

            _db.AddInParameter(commandUser, "@Name", DbType.String, user.Name);
            if (user.Mobile == string.Empty)
            {
                _db.AddInParameter(commandUser, "@Mobile", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandUser, "@Mobile", DbType.String, user.Mobile);
            }
            if (user.EMail == string.Empty)
            {
                _db.AddInParameter(commandUser, "@EMail", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandUser, "@EMail", DbType.String, user.EMail);
            }
            _db.AddInParameter(commandUser, "@IsSystemUser", DbType.Boolean, user.IsSystemUser);
            if (user.LoginName == string.Empty)
            {
                _db.AddInParameter(commandUser, "@LoginName", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandUser, "@LoginName", DbType.String, user.LoginName);
            }
            if (user.Password == string.Empty)
            {
                _db.AddInParameter(commandUser, "@Password", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandUser, "@Password", DbType.String, user.Password);
            }
            if (user.UserRole == string.Empty)
            {
                _db.AddInParameter(commandUser, "@UserRole", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandUser, "@UserRole", DbType.String, user.UserRole);
            }
            _db.AddInParameter(commandUser, "@IsActive", DbType.Boolean, user.IsActive);
            if (user.CardID == string.Empty)
            {
                _db.AddInParameter(commandUser, "@CardID", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandUser, "@CardID", DbType.String, user.CardID);
            }
            _db.AddInParameter(commandUser, "@CreatedBy", DbType.Byte, user.CreatedBy);
            _db.AddInParameter(commandUser, "@CreatedOn", DbType.DateTime, user.CreatedOn);
            _db.AddInParameter(commandUser, "@UpdatedBy", DbType.Byte, user.UpdatedBy);
            _db.AddInParameter(commandUser, "@UpdatedOn", DbType.DateTime, user.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try 
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    retval = Convert.ToInt32(_db.ExecuteScalar(commandUser, transaction));
                    AddUserDivisions(division, retval,transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            
            return retval;
        }

        public void AddUserDivisions(BindingList<DivisionInfo> divisions, int UserID, DbTransaction transaction)
        {
            foreach (DivisionInfo division in divisions)
            {
                DbCommand command = _db.GetSqlStringCommand("INSERT INTO UserDivisions (UserID,DivisionID) VALUES (@UserID,@DivisionID)");
                _db.AddInParameter(command, "@UserID", DbType.Int32, UserID);
                _db.AddInParameter(command, "@DivisionID", DbType.Int32, division.DivisionID);

                _db.ExecuteNonQuery(command, transaction);
            }
        }

        /// <summary>
        /// Updates the User
        /// </summary>
        /// <param name="user">Instance of User class</param>
        public void UpdateUser(UserInfo user, BindingList<DivisionInfo> Divisions)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Users SET [Name] = @Name, [Mobile] = @Mobile, [EMail] = @EMail, [IsSystemUser] = @IsSystemUser, [LoginName] = @LoginName, [Password] = @Password, [UserRole] = @UserRole, [IsActive] = @IsActive, [CardID] = @CardID, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, user.ID);
            _db.AddInParameter(command, "@Name", DbType.String, user.Name);
            if (user.Mobile == string.Empty)
            {
                _db.AddInParameter(command, "@Mobile", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Mobile", DbType.String, user.Mobile);
            }
            if (user.EMail == string.Empty)
            {
                _db.AddInParameter(command, "@EMail", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@EMail", DbType.String, user.EMail);
            }
            _db.AddInParameter(command, "@IsSystemUser", DbType.Boolean, user.IsSystemUser);
            if (user.LoginName == string.Empty)
            {
                _db.AddInParameter(command, "@LoginName", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@LoginName", DbType.String, user.LoginName);
            }
            if (user.Password == string.Empty)
            {
                _db.AddInParameter(command, "@Password", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Password", DbType.String, user.Password);
            }
            if (user.UserRole == string.Empty)
            {
                _db.AddInParameter(command, "@UserRole", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@UserRole", DbType.String, user.UserRole);
            }
            _db.AddInParameter(command, "@IsActive", DbType.Boolean, user.IsActive);
            if (user.CardID == string.Empty)
            {
                _db.AddInParameter(command, "@CardID", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@CardID", DbType.String, user.CardID);
            }
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, user.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, user.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try 
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);

                    DeleteUserDivisionByUserID(user.ID, transaction);
                    AddUserDivisions(Divisions, user.ID,transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            
        }

        public void DeleteUserDivisionByUserID(Int32 UserID, DbTransaction transaction)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM UserDivisions WHERE UserID=@UserID");
            _db.AddInParameter(command, "@UserID", DbType.Int32, UserID);

            _db.ExecuteNonQuery(command,transaction);
        }
        /// <summary>
        /// Deletes the User from the database
        /// </summary>
        /// <param name="userId">Id of the User that needs to be deleted</param>
        public void DeleteUser(int userId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Users " +
                                                        "WHERE Id=@Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, userId);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);
                    DeleteUserDivisionByUserID(userId,transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            
        }

        /// <summary>
        /// Gets the count of Users having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="userName">Name of the User to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name, bool cardID)
        {
            int retVal = 0;
            DbCommand command = null;
            if (!cardID)
                command = _db.GetSqlStringCommand(" Select Count([LoginName]) From [Users] WHERE [LoginName] = @Name");
            else
                command = _db.GetSqlStringCommand(" Select Count([CardID]) From [Users] WHERE [CardID] = @Name");

            _db.AddInParameter(command, "@Name", DbType.String, name);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the count of Users having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="userName">Name of the User to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int userId, bool cardID)
        {
            int retVal = 0;
            DbCommand command = null;
            if (!cardID)
                command = _db.GetSqlStringCommand(" Select Count([LoginName]) From [Users] WHERE [LoginName] = @Name AND Id != @Id");
            else
                command = _db.GetSqlStringCommand(" Select Count([CardID]) From [Users] WHERE [CardID] = @Name AND Id != @Id");

            _db.AddInParameter(command, "@Name", DbType.String, name);
            _db.AddInParameter(command, "@Id", DbType.Int32, userId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }


        public bool CheckUserUsed(int userId, out string tableName)
        {
            
            DbCommand command2 = _db.GetSqlStringCommand("SELECT COUNT(CreatedBy) FROM Sales WHERE CreatedBy = @UserId");
            _db.AddInParameter(command2, "@UserId", DbType.Int32, userId);

            //Pending for all other tables

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
               
                 if (Convert.ToInt32(_db.ExecuteScalar(command2)) > 0)
                {
                    tableName = "Sales";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }
    }
}