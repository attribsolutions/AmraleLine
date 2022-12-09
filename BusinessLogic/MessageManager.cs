
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DataObjects;
using DataAccess;


namespace BusinessLogic
{
    /// <summary>
    /// Author	: Akash
    /// Date	: 08 June 2017 10:40:36 AM
    /// </summary>
    /// 
    public class MessageManager
    {

        /// <summary>
        /// Gets all the Messages from the database
        /// </summary>
        /// <returns>BindingList of Messages</returns>
        /// 
        public BindingList<MessageInfo> GetMessages()
        {
          BindingList<MessageInfo> retval = new BindingList<MessageInfo>();
          MessageDAL dal = new MessageDAL();
          retval = dal.GetMessage();
          dal = null;
          return retval;

        }

        public BindingList<MessageInfo> GetAllMessages(string searchstring, int Count)
        {
            BindingList<MessageInfo> retval = new BindingList<MessageInfo>();
            MessageDAL dal = new MessageDAL();
            retval = dal.GetAllMessages(searchstring,Count);
            dal = null;
            return retval; 
        }

        public int AddMessage(MessageInfo newMsg)
        {
            int retval = 0;
            MessageDAL dal = new MessageDAL();
            retval = dal.AddMessage(newMsg);
            dal = null;
            return retval;
        }

        public void UpdateMessage(MessageInfo newMsg)
        {
            MessageDAL dal = new MessageDAL();
            dal.UpdateMessage(newMsg);
            dal = null;
        }

        public void DeleteMessage(int MessageID)
        {
            MessageDAL dal = new MessageDAL();
            dal.DeleteMessage(MessageID);
            dal = null;
        }
    }
}
