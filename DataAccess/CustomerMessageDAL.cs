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
    public class CustomerMessageDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public CustomerMessageDAL()
        { }

        public int AddCustomerMessage(CustomerMessageInfo CustMsg)
        {
            int retval = 0;
            DbCommand command = _db.GetSqlStringCommand("INSERT INTO CustomerMessages (FromDate,ToDate,CustomerID,DefaultMessage,Message,IsComplated,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn) " +
                "VALUES(@FromDate,@ToDate,@CustomerID,@DefautMessage,@Message,@Iscomplated,@CreatedBy,@CreatedOn,@UpdatedBy,@UpdatedOn)");
            _db.AddInParameter(command, "@FromDate", DbType.Date, CustMsg.FromDate);
            _db.AddInParameter(command, "@ToDate", DbType.Date, CustMsg.ToDate);
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, CustMsg.CustomerID);
            _db.AddInParameter(command, "DefautMessage", DbType.String, CustMsg.DefaultMessage);
            _db.AddInParameter(command, "Message", DbType.String, CustMsg.Message);
            _db.AddInParameter(command, "IsComplated", DbType.String, CustMsg.IsComplated);
            _db.AddInParameter(command, "@CreatedBy", DbType.Byte, CustMsg.CreatedBy);
            _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, CustMsg.CreatedOn);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, CustMsg.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, CustMsg.UpdatedOn);

            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                retval = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retval;

        }

        public void UpdateCustomerMessage(CustomerMessageInfo CustMsg)
        {
            DbCommand command = _db.GetSqlStringCommand("Update CustomerMessages SET FromDate = @FromDate, ToDate = @ToDate, CustomerID = @CustomerID, DefaultMessage=@DefaultMessage,Message = @Message,IsComplated=@IsComplated,UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE ID = @ID");
            _db.AddInParameter(command, "@ID", DbType.Int32, CustMsg.ID);
            _db.AddInParameter(command, "@FromDate", DbType.Date, CustMsg.FromDate);
            _db.AddInParameter(command, "@ToDate", DbType.Date, CustMsg.ToDate);
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, CustMsg.CustomerID);
            _db.AddInParameter(command, "@DefaultMessage", DbType.String, CustMsg.DefaultMessage);
            _db.AddInParameter(command, "@Message", DbType.String, CustMsg.Message);
            _db.AddInParameter(command, "@IsComplated", DbType.String, CustMsg.IsComplated);
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, CustMsg.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, CustMsg.UpdatedOn);
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public BindingList<CustomerMessageInfo> GetCustomerMessagesByFilter(string customerName, int Count)
        {
            BindingList<CustomerMessageInfo> retval = new BindingList<CustomerMessageInfo>();
            //DbCommand command = _db.GetSqlStringCommand("SELECT TOP " + Count + "CustomerMessages.ID,FromDate,ToDate,CustomerID,DefaultMessage,Message,IsComplated,Customers.LineID ,Customers.Name,Customers.CustomersNumbar FROM CustomerMessages JOIN Customers On Customers.ID = CustomerMessages.CustomerID WHERE Customers.Name LIKE '%" + customerName.Trim() + "%' ORDER BY ID DESC ");
            DbCommand command = _db.GetSqlStringCommand("SELECT TOP " + Count + "CustomerMessages.ID,FromDate,ToDate,Customers.LineID,Lines.LineNumber,CustomerID,Customers.CustomerNumber,Customers.Name,DefaultMessage,Message,IsComplated FROM CustomerMessages JOIN Customers On Customers.ID = CustomerMessages.CustomerID Join Lines on Lines.ID = Customers.LineID WHERE Customers.Name LIKE '%" + customerName.Trim() + "%' ORDER BY ID DESC ");
            using (IDataReader reader = _db.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    CustomerMessageInfo message = new CustomerMessageInfo();
                    message.ID = Convert.ToInt32(reader["ID"]);
                    message.FromDate = Convert.ToDateTime(reader["FromDate"]);
                    message.ToDate = Convert.ToDateTime(reader["ToDate"]);
                    message.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                    message.DefaultMessage = Convert.ToString(reader["DefaultMessage"]);
                    message.Message = Convert.ToString(reader["Message"]);
                    message.Name = Convert.ToString(reader["Name"]);
                    message.IsComplated = Convert.ToString(reader["IsComplated"]);
                    message.LineID = Convert.ToInt32(reader["LineID"]);
                    message.CustomerNumber = Convert.ToInt32(reader["CustomerNumber"]);
                    message.LineNumber = Convert.ToInt32(reader["LineNumber"]);
                    retval.Add(message);
                }
            }
            return retval;
        }

        public void DeleteCustomerMessage(int MessageID)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM CustomerMessages WHERE ID = @ID");
            _db.AddInParameter(command, "ID", DbType.Int32, MessageID);
            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                _db.ExecuteNonQuery(command);
            }
        }

    }
}
