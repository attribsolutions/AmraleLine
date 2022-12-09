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
    /// Date	: 16 Apr 2010 03:29:35 PM
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Adds a new User in to the database
        /// </summary>
        /// <param name="user">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added User</returns>

        public int AddUser(UserInfo user,BindingList<DivisionInfo> division)
        {
            int retval = 0;
            UserDAL dal = new UserDAL();
            retval = dal.AddUser(user,division);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the User record in the database
        /// </summary>
        /// <param name="user">Instance with properties set as per the values entered in the form</param>
        public void UpdateUser(UserInfo user, BindingList<DivisionInfo> Divisions)
        {
            UserDAL dal = new UserDAL();
            dal.UpdateUser(user, Divisions);
            dal = null;
        }

        /// <summary>
        /// Gets all the users from the database
        /// </summary>
        /// <returns>BindingList of users</returns>
        public BindingList<UserInfo> GetUsersAll(bool showActiveOnly)
        {
            BindingList<UserInfo> retval = new BindingList<UserInfo>();
            UserDAL dal = new UserDAL();
            retval = dal.GetUsersAll(showActiveOnly);
            dal = null;
            return retval;
        }

        public BindingList<UserInfo> GetAllSystemUsers(bool showActiveOnly)
        {
            BindingList<UserInfo> retval = new BindingList<UserInfo>();
            UserDAL dal = new UserDAL();
            retval = dal.GetAllSystemUsers(showActiveOnly);
            dal = null;
            return retval;
        }

        public BindingList<UserInfo> GetUsersByName(string userName)
        {
            BindingList<UserInfo> retval = new BindingList<UserInfo>();
            UserDAL dal = new UserDAL();
            retval = dal.GetUsersByName(userName);
            dal = null;
            return retval;
        }

        public UserInfo GetUsersByCardID(String cardID)
        {
            UserInfo retval = new UserInfo();
            UserDAL dal = new UserDAL();
            retval = dal.GetUsersByCardID(cardID);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets User record from the database based on the UserId
        /// </summary>
        /// <param name="userId">Id of the User</param>
        /// <returns>Instance of User</returns>
        public UserInfo GetUser(int userId)
        {
            UserInfo retval = new UserInfo();
            UserDAL dal = new UserDAL();
            retval = dal.GetUser(userId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the User based on UserId
        /// </summary>
        /// <param name="userId">Id of the User that is to be deleted</param>
        public void DeleteUser(int userId)
        {
            UserDAL dal = new UserDAL();
            dal.DeleteUser(userId);
            dal = null;
        }

        /// <summary>
        /// Checks if the User name already exists in the users table
        /// This will work for New Mode only(while adding a new user)
        /// </summary>
        /// <param name="userName">Name of the User that needs to be checked</param>
        /// <returns>No. of records having the same name. 0 is returned if no records are found</returns>
        public int GetSameNameCount(string name, bool isCardID)
        {
            int retVal = 0;
            UserDAL dal = new UserDAL();
            retVal = dal.GetSameNameCount(name, isCardID);
            dal = null;
            return retVal;
        }

        /// <summary>
        /// Checks if the User name already exists in the Users table
        /// This will work for Edit Mode only(while updating an existing record)
        /// </summary>
        /// <param name="userName">Name of the User that needs to be checked</param>
        /// <param name="userId">Id of the User that needs to be updated</param>
        /// <returns>No. of records having the same name apart for the current user.
        /// 0 is returned if no records are found</returns>

        public int GetSameNameCountForEditMode(string name, int userId, bool isCardID)
        {
            int retVal = 0;
            UserDAL dal = new UserDAL();
            retVal = dal.GetSameNameCountForEditMode(name, userId, isCardID);
            dal = null;
            return retVal;
        }


        public bool CheckUserUsed(int userId, out string tableName)
        {
            UserDAL dal = new UserDAL();
            return dal.CheckUserUsed(userId, out tableName);
        }
    }
}