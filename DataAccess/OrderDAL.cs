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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Order table
    /// Author	: Kiran
    /// Date	: 05 Aug 2011 03:36:15 PM
    /// </summary>
    public class OrderDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public OrderDAL()
        {

        }
        /// <summary>
        /// Gets all the Orders from the Orders table
        /// </summary>
        /// <returns>BindingList of Orders</returns>
        public BindingList<OrderInfo> GetOrdersAll()
        {
            BindingList<OrderInfo> retVal = new BindingList<OrderInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], ISNULL(BillID, 0) BillID, [OrderDate], [CustomerID], [DeliveryDate], [TotalAmount], [Advance], [DeliveryPayment], [IsCompleted], [Comments] FROM Orders ORDER BY [ID]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    OrderInfo order = new OrderInfo();
                    order.ID = Convert.ToInt32(dataReader["ID"]);
                    order.BillID = Convert.ToInt32(dataReader["BillID"]);
                    order.OrderDate = Convert.ToDateTime(dataReader["OrderDate"]);
                    order.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                    order.DeliveryDate = Convert.ToDateTime(dataReader["DeliveryDate"]);
                    order.TotalAmount = Convert.ToDecimal(dataReader["TotalAmount"]);
                    order.Advance = Convert.ToDecimal(dataReader["Advance"]);
                    order.DeliveryPayment = Convert.ToDecimal(dataReader["DeliveryPayment"]);
                    order.IsCompleted = Convert.ToBoolean(dataReader["IsCompleted"]);
                    if (dataReader["Comments"] == DBNull.Value)
                    {
                        order.Comments = string.Empty;
                    }
                    else
                    {
                        order.Comments = Convert.ToString(dataReader["Comments"]);
                    }

                    retVal.Add(order);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Order based on Id
        /// </summary>
        /// <param name="orderId">Id of the Order the needs to be retrieved</param>
        /// <returns>Instance of Order</returns>
        public OrderInfo GetOrder(int orderId)
        {
            OrderInfo retVal = new OrderInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], ISNULL(BillID, 0) BillID, [OrderDate], [CustomerID], [DeliveryDate], [TotalAmount], [Advance], [DeliveryPayment], [IsCompleted], [Comments] FROM Orders WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, orderId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.BillID = Convert.ToInt32(dataReader["BillID"]);
                    retVal.OrderDate = Convert.ToDateTime(dataReader["OrderDate"]);
                    retVal.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                    retVal.DeliveryDate = Convert.ToDateTime(dataReader["DeliveryDate"]);
                    retVal.TotalAmount = Convert.ToDecimal(dataReader["TotalAmount"]);
                    retVal.Advance = Convert.ToDecimal(dataReader["Advance"]);
                    retVal.DeliveryPayment = Convert.ToDecimal(dataReader["DeliveryPayment"]);
                    retVal.IsCompleted = Convert.ToBoolean(dataReader["IsCompleted"]);
                    if (dataReader["Comments"] == DBNull.Value)
                    {
                        retVal.Comments = string.Empty;
                    }
                    else
                    {
                        retVal.Comments = Convert.ToString(dataReader["Comments"]);
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new Order in to the database
        /// </summary>
        /// <param name="order">Instance of Order</param>
        /// <returns>Id of the newly added Order</returns>
        public int AddOrder(OrderInfo order, BindingList<OrderItemInfo> orderItems, CustomerInfo customer)
        {
            int retval = 0;
            
            DbCommand commandCustomer = _db.GetSqlStringCommand("INSERT INTO Customers(Name, ContactPerson, Mobile, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) " +
                                              "VALUES (@Name, @ContactPerson, @Mobile, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                       "SELECT IDENT_CURRENT('Customers')");

            _db.AddInParameter(commandCustomer, "@Name", DbType.String, customer.Name);
            _db.AddInParameter(commandCustomer, "@ContactPerson", DbType.String, customer.ContactPerson);
            _db.AddInParameter(commandCustomer, "@Mobile", DbType.String, customer.Mobile);
            _db.AddInParameter(commandCustomer, "@CreatedBy", DbType.Int32, customer.CreatedBy);
            _db.AddInParameter(commandCustomer, "@CreatedOn", DbType.DateTime, customer.CreatedOn);
            _db.AddInParameter(commandCustomer, "@UpdatedBy", DbType.Int32, customer.CreatedBy);
            _db.AddInParameter(commandCustomer, "@UpdatedOn", DbType.DateTime, customer.CreatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    if (customer != null)
                    {
                        order.CustomerID = Convert.ToInt32(_db.ExecuteScalar(commandCustomer, transaction));
                    }

                    DbCommand commandOrder = _db.GetSqlStringCommand("INSERT INTO Orders([OrderDate], [CustomerID], [DeliveryDate], [TotalAmount], [Advance], [DeliveryPayment], [IsCompleted], [Comments]) " +
                                                       "VALUES (@OrderDate, @CustomerID, @DeliveryDate, @TotalAmount, @Advance, @DeliveryPayment, @IsCompleted, @Comments) " + Environment.NewLine +
                                                       "SELECT IDENT_CURRENT('Orders')");

                    _db.AddInParameter(commandOrder, "@OrderDate", DbType.DateTime, order.OrderDate);
                    _db.AddInParameter(commandOrder, "@CustomerID", DbType.Int32, order.CustomerID);
                    _db.AddInParameter(commandOrder, "@DeliveryDate", DbType.DateTime, order.DeliveryDate);
                    _db.AddInParameter(commandOrder, "@TotalAmount", DbType.Decimal, order.TotalAmount);
                    _db.AddInParameter(commandOrder, "@Advance", DbType.Decimal, order.Advance);
                    _db.AddInParameter(commandOrder, "@DeliveryPayment", DbType.Decimal, order.DeliveryPayment);
                    _db.AddInParameter(commandOrder, "@IsCompleted", DbType.Boolean, order.IsCompleted);
                    if (order.Comments == string.Empty)
                    {
                        _db.AddInParameter(commandOrder, "@Comments", DbType.String, DBNull.Value);
                    }
                    else
                    {
                        _db.AddInParameter(commandOrder, "@Comments", DbType.String, order.Comments);
                    }

                    retval = Convert.ToInt32(_db.ExecuteScalar(commandOrder, transaction));

                    foreach (OrderItemInfo item in orderItems)
                    {
                        DbCommand commandOrderItem = _db.GetSqlStringCommand("INSERT INTO OrderItems([OrderID], [ItemID], [UnitID], [Quantity], [Rate], [Vat], [Amount]) " +
                                                           "VALUES (@OrderID, @ItemID, @UnitID, @Quantity, @Rate, @Vat, @Amount) ");

                        _db.AddInParameter(commandOrderItem, "@OrderID", DbType.Int32, retval);
                        _db.AddInParameter(commandOrderItem, "@ItemID", DbType.Int32, item.ItemID);
                        _db.AddInParameter(commandOrderItem, "@UnitID", DbType.Int32, item.UnitID);
                        _db.AddInParameter(commandOrderItem, "@Quantity", DbType.Decimal, item.Quantity);
                        _db.AddInParameter(commandOrderItem, "@Rate", DbType.Decimal, item.Rate);
                        _db.AddInParameter(commandOrderItem, "@Vat", DbType.Decimal, item.Vat);
                        _db.AddInParameter(commandOrderItem, "@Amount", DbType.Decimal, item.Amount);

                        _db.ExecuteScalar(commandOrderItem, transaction);
                    }

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

        /// <summary>
        /// Updates the Order
        /// </summary>
        /// <param name="order">Instance of Order class</param>
        public void UpdateOrder(OrderInfo order, BindingList<OrderItemInfo> orderItems)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Orders SET [OrderDate] = @OrderDate, [CustomerID] = @CustomerID, [DeliveryDate] = @DeliveryDate, [TotalAmount] = @TotalAmount, [Advance] = @Advance, [DeliveryPayment] = @DeliveryPayment, [IsCompleted] = @IsCompleted, [Comments] = @Comments WHERE Id = @Id ");

            _db.AddInParameter(command, "@ID", DbType.Int32, order.ID);
            _db.AddInParameter(command, "@OrderDate", DbType.DateTime, order.OrderDate);
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, order.CustomerID);
            _db.AddInParameter(command, "@DeliveryDate", DbType.DateTime, order.DeliveryDate);
            _db.AddInParameter(command, "@TotalAmount", DbType.Decimal, order.TotalAmount);
            _db.AddInParameter(command, "@Advance", DbType.Decimal, order.Advance);
            _db.AddInParameter(command, "@DeliveryPayment", DbType.Decimal, order.DeliveryPayment);
            _db.AddInParameter(command, "@IsCompleted", DbType.Boolean, order.IsCompleted);
            if (order.Comments == string.Empty)
            {
                _db.AddInParameter(command, "@Comments", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Comments", DbType.String, order.Comments);
            }

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;

                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    _db.ExecuteNonQuery(command, transaction);

                    DbCommand commandOrderItemsDelete = _db.GetSqlStringCommand("DELETE FROM OrderItems WHERE OrderID = @OrderId");
                    _db.AddInParameter(commandOrderItemsDelete, "@OrderID", DbType.Int32, order.ID);

                    _db.ExecuteScalar(commandOrderItemsDelete, transaction);

                    foreach (OrderItemInfo item in orderItems)
                    {
                        DbCommand commandOrderItem = _db.GetSqlStringCommand("INSERT INTO OrderItems([OrderID], [ItemID], [UnitID], [Quantity], [Rate], [Vat], [Amount]) " +
                                                           "VALUES (@OrderID, @ItemID, @UnitID, @Quantity, @Rate, @Vat @Amount) ");

                        _db.AddInParameter(commandOrderItem, "@OrderID", DbType.Int32, order.ID);
                        _db.AddInParameter(commandOrderItem, "@ItemID", DbType.Int32, item.ItemID);
                        _db.AddInParameter(commandOrderItem, "@UnitID", DbType.Int32, item.UnitID);
                        _db.AddInParameter(commandOrderItem, "@Quantity", DbType.Decimal, item.Quantity);
                        _db.AddInParameter(commandOrderItem, "@Rate", DbType.Decimal, item.Rate);
                        _db.AddInParameter(commandOrderItem, "@Vat", DbType.Decimal, item.Vat);
                        _db.AddInParameter(commandOrderItem, "@Amount", DbType.Decimal, item.Amount);

                        _db.ExecuteScalar(commandOrderItem, transaction);
                    }

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
        /// Deletes the Order from the database
        /// </summary>
        /// <param name="orderId">Id of the Order that needs to be deleted</param>
        public void DeleteOrder(int orderId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Orders WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, orderId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public bool CheckOrderUsed(int orderID, out string tableName)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT IsNull(BillID,0) FROM Orders WHERE ID = @OrderId");
            _db.AddInParameter(command, "@OrderId", DbType.Int32, orderID);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                int billNo = Convert.ToInt32(_db.ExecuteScalar(command));
                if (billNo > 0)
                {
                    tableName = "Bill No. "+billNo;
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }

        public BindingList<OrderInfo> GetOrdersByFilter(int searchType, string name, object sDate, object eDate)
        {
            BindingList<OrderInfo> retVal = new BindingList<OrderInfo>();
            string cmdString = "SELECT Orders.ID, OrderDate, BillID, CustomerID, Customers.Name CustomerName, DeliveryDate, TotalAmount, Advance, DeliveryPayment, IsCompleted, Comments FROM Orders INNER JOIN Customers ON Orders.CustomerID = Customers.ID ";

            DbCommand command = null;
            if (searchType == 0)
                command = _db.GetSqlStringCommand(cmdString + " WHERE Customers.Name LIKE '" + name + "%' ORDER BY Customers.Name");
            if (searchType == 1)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Orders.OrderDate >= @SDate AND Orders.OrderDate <= @EDate ORDER BY Orders.OrderDate DESC");
                _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            }
            if (searchType == 2)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Orders.TotalAmount >= @Amount ORDER BY Orders.OrderDate DESC");
                _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
            }
            if (searchType == 3)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Orders.TotalAmount <= @Amount ORDER BY Orders.OrderDate DESC");
                _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
            }
            if (searchType == 4)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Orders.IsCompleted = 'False'");
            }
            if (searchType == 5)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Orders.DeliveryDate >= @SDate AND Orders.DeliveryDate <= @EDate ORDER BY Orders.DeliveryDate DESC");
                _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            }

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    OrderInfo order = new OrderInfo();
                    order.ID = Convert.ToInt32(dataReader["ID"]);
                    order.BillID = Convert.ToInt32(dataReader["BillID"]);
                    order.OrderDate = Convert.ToDateTime(dataReader["OrderDate"]);
                    order.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                    order.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                    order.DeliveryDate = Convert.ToDateTime(dataReader["DeliveryDate"]);
                    order.TotalAmount = Convert.ToDecimal(dataReader["TotalAmount"]);
                    order.Advance = Convert.ToDecimal(dataReader["Advance"]);
                    order.DeliveryPayment = Convert.ToDecimal(dataReader["DeliveryPayment"]);
                    order.IsCompleted = Convert.ToBoolean(dataReader["IsCompleted"]);
                    if (dataReader["Comments"] == DBNull.Value)
                    {
                        order.Comments = string.Empty;
                    }
                    else
                    {
                        order.Comments = Convert.ToString(dataReader["Comments"]);
                    }

                    retVal.Add(order);
                }
            }
            return retVal;
        }

        public BindingList<OrderItemInfo> GetOrderItemsByOrderId(int orderId)
        {
            BindingList<OrderItemInfo> retVal = new BindingList<OrderItemInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT OrderItems.ID, [OrderID], [ItemID], Items.Name ItemName, OrderItems.UnitID, Units.Name UnitName, [Quantity], OrderItems.Rate, OrderItems.Vat, [Amount] FROM OrderItems JOIN Units ON UnitID = Units.ID JOIN Items ON OrderItems.ItemID = Items.ID WHERE OrderID = @OrderId ORDER BY [ID]");
            _db.AddInParameter(command, "@OrderId", DbType.Int32, orderId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    OrderItemInfo orderItem = new OrderItemInfo();
                    orderItem.ID = Convert.ToInt32(dataReader["ID"]);
                    orderItem.OrderID = Convert.ToInt32(dataReader["OrderID"]);
                    orderItem.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    orderItem.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    orderItem.ItemName = Convert.ToString(dataReader["ItemName"]);
                    orderItem.UnitName = Convert.ToString(dataReader["UnitName"]);
                    orderItem.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    orderItem.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    orderItem.Vat = Convert.ToDecimal(dataReader["Vat"]);
                    orderItem.Amount = Convert.ToDecimal(dataReader["Amount"]);

                    retVal.Add(orderItem);
                }
            }
            return retVal;
        }
    }
}