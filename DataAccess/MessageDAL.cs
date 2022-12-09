using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.ComponentModel;
using System.Data.Common;
using System.Data;
using DataObjects;




namespace DataAccess
{
   public class MessageDAL
    {
       SqlDatabase _db = DBConn.CreateDB();
       public MessageDAL()
       { 
       
       }
       public BindingList<MessageInfo> GetMessage()
       {
           BindingList<MessageInfo> retval = new BindingList<MessageInfo>();
           DbCommand command = _db.GetSqlStringCommand(@"SELECT [ID],[MESSAGE] FROM MESSAGES ");
           using (IDataReader datareader = _db.ExecuteReader(command))
           {
               while (datareader.Read())
               {
                   MessageInfo message = new MessageInfo();
                   message.ID = Convert.ToInt32(datareader["ID"]);
                   message.Message = Convert.ToString(datareader["MESSAGE"]);
                   retval.Add(message);

               }
           }
           return retval;
       }

       public BindingList<MessageInfo> GetAllMessages(string searchstring, int Count)
       {

           BindingList<MessageInfo> retval = new BindingList<MessageInfo>();
           DbCommand command = _db.GetSqlStringCommand("SELECT ID,Message FROM Messages WHERE Message LIKE '%" + searchstring + "%' ORDER BY Message");
           using (IDataReader reader = _db.ExecuteReader(command))
           {
               while (reader.Read())
               {
                   MessageInfo message = new MessageInfo();
                   message.ID = Convert.ToInt32(reader["ID"]);
                   message.Message = Convert.ToString(reader["MESSAGE"]);
                   retval.Add(message);
                  
               }
           }
           return retval;
       }



       public int AddMessage(MessageInfo newMsg)
       {
           int retval = 0;
           DbCommand command = _db.GetSqlStringCommand("INSERT INTO Messages (Message,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn) " +
               "VALUES(@Message,@CreatedBy,@CreatedOn,@UpdatedBy,@UpdatedOn)");

           _db.AddInParameter(command, "Message", DbType.String, newMsg.Message);
           _db.AddInParameter(command, "@CreatedBy", DbType.Byte, newMsg.CreatedBy);
           _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, newMsg.CreatedOn);
           _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, newMsg.UpdatedBy);
           _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, newMsg.UpdatedOn);

           using (DbConnection con = _db.CreateConnection())
           {
               con.Open();
               retval = Convert.ToInt32(_db.ExecuteScalar(command));
           }
           return retval;
       }

       public void UpdateMessage(MessageInfo newMsg)
       {
           DbCommand command = _db.GetSqlStringCommand("Update Messages SET Message = @Message,UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE ID = @ID");
           _db.AddInParameter(command, "@ID", DbType.Int32, newMsg.ID);
           _db.AddInParameter(command, "Message", DbType.String, newMsg.Message);
           _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, newMsg.UpdatedBy);
           _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, newMsg.UpdatedOn);
           using (DbConnection conn = _db.CreateConnection())
           {
               conn.Open();
               _db.ExecuteNonQuery(command);
           }
       }

       public void DeleteMessage(int MessageID)
       {
           DbCommand command = _db.GetSqlStringCommand("DELETE FROM Messages WHERE ID = @ID");
           _db.AddInParameter(command, "ID", DbType.Int32, MessageID);
           using (DbConnection con = _db.CreateConnection())
           {
               con.Open();
               _db.ExecuteNonQuery(command);
           }
       }
    }
}
