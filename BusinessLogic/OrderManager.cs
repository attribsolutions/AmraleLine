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
    /// Date	: 05 Aug 2011 03:35:23 PM
    /// </summary>
    public class OrderManager
    {
        /// <summary>
        /// Adds a new Order in to the database
        /// </summary>
        /// <param name="order">New instance with properties set as per the values entered in the form</param>
        /// <returns>Id of the newly added Order</returns>

        public int AddOrder(OrderInfo order, BindingList<OrderItemInfo> orderItems, CustomerInfo customer)
        {
            int retval = 0;
            OrderDAL dal = new OrderDAL();
            retval = dal.AddOrder(order, orderItems, customer);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Updates the Order record in the database
        /// </summary>
        /// <param name="order">Instance with properties set as per the values entered in the form</param>
        public void UpdateOrder(OrderInfo order, BindingList<OrderItemInfo> orderItems)
        {
            OrderDAL dal = new OrderDAL();
            dal.UpdateOrder(order, orderItems);
            dal = null;
        }

        /// <summary>
        /// Gets all the orders from the database
        /// </summary>
        /// <returns>BindingList of orders</returns>
        public BindingList<OrderInfo> GetOrdersAll()
        {
            BindingList<OrderInfo> retval = new BindingList<OrderInfo>();
            OrderDAL dal = new OrderDAL();
            retval = dal.GetOrdersAll();
            dal = null;
            return retval;
        }

        /// <summary>
        /// Gets Order record from the database based on the OrderId
        /// </summary>
        /// <param name="orderId">Id of the Order</param>
        /// <returns>Instance of Order</returns>
        public OrderInfo GetOrder(int orderId)
        {
            OrderInfo retval = new OrderInfo();
            OrderDAL dal = new OrderDAL();
            retval = dal.GetOrder(orderId);
            dal = null;
            return retval;
        }

        /// <summary>
        /// Deletes the Order based on OrderId
        /// </summary>
        /// <param name="orderId">Id of the Order that is to be deleted</param>
        public void DeleteOrder(int orderId)
        {
            OrderDAL dal = new OrderDAL();
            dal.DeleteOrder(orderId);
            dal = null;
        }

        public bool CheckOrderUsed(int orderID, out string tableName)
        {
            OrderDAL dal = new OrderDAL();
            return dal.CheckOrderUsed(orderID, out tableName);
            dal = null;
        }

        public BindingList<OrderInfo> GetOrdersByFilter(int searchType, string name, object sDate, object eDate)
        {
            OrderDAL dal = new OrderDAL();
            return dal.GetOrdersByFilter(searchType, name, sDate, eDate);
        }

        public BindingList<OrderItemInfo> GetOrderItemsByOrderId(int orderId)
        {
            OrderDAL dal = new OrderDAL();
            return dal.GetOrderItemsByOrderId(orderId);
        }
    }
}