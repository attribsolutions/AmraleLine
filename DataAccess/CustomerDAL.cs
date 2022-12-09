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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Customer table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:07:50 PM
    /// </summary>
    public class CustomerDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public CustomerDAL()
        {

        }
        /// <summary>
        /// Gets all the Customers from the Customers table
        /// </summary>
        /// <returns>BindingList of Customers</returns>
        public BindingList<CustomerInfo> GetCustomersAll()
        {
            BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID],[CustomerNumber], [Name], [ContactPerson], [Address], [City], [Mobile], [EMail], [Balance], [IsActive],[CustomerType],[Barcode],[MemberSince], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Customers ORDER BY [Name]");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    CustomerInfo customer = new CustomerInfo();
                    customer.ID = Convert.ToInt32(dataReader["ID"]);
                    customer.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    customer.Name = Convert.ToString(dataReader["Name"]);
                    customer.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    if (dataReader["Address"] == DBNull.Value)
                    {
                        customer.Address = string.Empty;
                    }
                    else
                    {
                        customer.Address = Convert.ToString(dataReader["Address"]);
                    }
                    if (dataReader["City"] == DBNull.Value)
                    {
                        customer.City = string.Empty;
                    }
                    else
                    {
                        customer.City = Convert.ToString(dataReader["City"]);
                    }
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        customer.Mobile = string.Empty;
                    }
                    else
                    {
                        customer.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        customer.EMail = string.Empty;
                    }
                    else
                    {
                        customer.EMail = Convert.ToString(dataReader["EMail"]);
                    }

                    customer.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    customer.IsActive = Convert.ToBoolean(dataReader["IsActive"]);

                    customer.CustomerType = Convert.ToInt32(dataReader["CustomerType"]);
                    customer.Barcode = Convert.ToString(dataReader["Barcode"]);
                    customer.MemberSince = Convert.ToDateTime(dataReader["MemberSince"]);

                    customer.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    customer.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    customer.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    customer.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(customer);
                }
            }
            return retVal;
        }


        public BindingList<CustomerInfo> GetCustomersCouponsAll()
        {
            BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();
            try
            {
                DbCommand command = _db.GetSqlStringCommand("SELECT [ID],[CustomerNumber], [Name], [ContactPerson], [Address], [City], [Mobile], [EMail], [Balance], [IsActive],[CustomerType],[Barcode],[MemberSince], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Customers WHERE CustomerType = 1 ORDER BY [Name]");

                using (IDataReader dataReader = _db.ExecuteReader(command))
                {
                    while (dataReader.Read())
                    {
                        CustomerInfo customer = new CustomerInfo();
                        customer.ID = Convert.ToInt32(dataReader["ID"]);
                        customer.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                        customer.Name = Convert.ToString(dataReader["Name"]);
                        customer.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                        if (dataReader["Address"] == DBNull.Value)
                        {
                            customer.Address = string.Empty;
                        }
                        else
                        {
                            customer.Address = Convert.ToString(dataReader["Address"]);
                        }
                        if (dataReader["City"] == DBNull.Value)
                        {
                            customer.City = string.Empty;
                        }
                        else
                        {
                            customer.City = Convert.ToString(dataReader["City"]);
                        }
                        if (dataReader["Mobile"] == DBNull.Value)
                        {
                            customer.Mobile = string.Empty;
                        }
                        else
                        {
                            customer.Mobile = Convert.ToString(dataReader["Mobile"]);
                        }
                        if (dataReader["EMail"] == DBNull.Value)
                        {
                            customer.EMail = string.Empty;
                        }
                        else
                        {
                            customer.EMail = Convert.ToString(dataReader["EMail"]);
                        }

                        customer.Balance = Convert.ToDecimal(dataReader["Balance"]);
                        customer.IsActive = Convert.ToBoolean(dataReader["IsActive"]);

                        customer.CustomerType = Convert.ToInt32(dataReader["CustomerType"]);
                        customer.Barcode = Convert.ToString(dataReader["Barcode"]);
                        customer.MemberSince = Convert.ToDateTime(dataReader["MemberSince"]);

                        customer.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                        customer.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                        customer.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                        customer.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                        retVal.Add(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return retVal;
        }

        public BindingList<CustomerInfo> GetCustomersByName(int searchType, string customerName, int lineID)
        {
            BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();

            //string commandText = " SELECT [ID], [CustomerNumber], [Name], [ContactPerson], [Address], [City], [Mobile], [EMail], [Balance],[Deposit], [IsActive], [CustomerType],LineID,LineNumber, [Barcode], [MemberSince], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn], " +
            //    " STUFF((SELECT ','+Items.Name FROM Items JOIN Customer_Items ON Customer_Items.ItemID = Items.ID AND Customer_Items.CustomerID = Customers.ID FOR XML PATH ('')) , 1, 1, '')  AS ItemName FROM Customers join  ";
            string commandText = " SELECT Customers.ID, Customers.CustomerNumber, Customers.Name,NameMarathi, ContactPerson, Customers.Address, [City], [Mobile], [EMail], [Balance],[Deposit], [IsActive], [CustomerType],LineID,LineNumber, [Barcode], [MemberSince], Customers.CreatedBy, Customers.CreatedOn, Customers.UpdatedBy, Customers.UpdatedOn,STUFF((SELECT ','+Items.Name FROM Items JOIN Customer_Items ON Customer_Items.ItemID = Items.ID AND Customer_Items.CustomerID = Customers.ID FOR XML PATH ('')) , 1, 1, '')  AS ItemName FROM Customers join Lines on Lines.ID=Customers.LineID ";

            if (searchType == 0)
                commandText += " WHERE Customers.Name LIKE '%" + customerName + "%' ORDER BY Customers.Name ";
            else if (searchType == 2)
            {   
                //commandText += (customerNo == 0) ? " AND LineID=@LineID  ORDER BY Customers.ID " : " AND Customers.CustomerNumber = @customerNo ORDER BY Customers.ID ";
                commandText += " WHERE LineID= '" + lineID + "' ORDER BY [CustomerNumber] ASC ";
            }
            else
            {
                if (customerName != "")
                    commandText += " WHERE CustomerNumber = '" + customerName + "' ORDER BY [CustomerNumber] ASC ";
                else
                    commandText += " ORDER BY [CustomerNumber] ASC ";
            }
             

            DbCommand command = _db.GetSqlStringCommand(commandText);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    CustomerInfo customer = new CustomerInfo();
                    customer.ID = Convert.ToInt32(dataReader["ID"]);
                    customer.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    customer.Name = Convert.ToString(dataReader["Name"]);
                    customer.CustomerNameMarathi = Convert.ToString(dataReader["NameMarathi"]);
                    customer.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    if (dataReader["Address"] == DBNull.Value)
                    {
                        customer.Address = string.Empty;
                    }
                    else
                    {
                        customer.Address = Convert.ToString(dataReader["Address"]);
                    }
                    if (dataReader["City"] == DBNull.Value)
                    {
                        customer.City = string.Empty;
                    }
                    else
                    {
                        customer.City = Convert.ToString(dataReader["City"]);
                    }
                    if (dataReader["Mobile"] == DBNull.Value)
                    {
                        customer.Mobile = string.Empty;
                    }
                    else
                    {
                        customer.Mobile = Convert.ToString(dataReader["Mobile"]);
                    }
                    if (dataReader["EMail"] == DBNull.Value)
                    {
                        customer.EMail = string.Empty;
                    }
                    else
                    {
                        customer.EMail = Convert.ToString(dataReader["EMail"]);
                    }

                    customer.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    customer.Deposit = Convert.ToString(dataReader["Deposit"]) == "" ? Convert.ToDecimal("0.00") : Convert.ToDecimal(dataReader["Deposit"]);
                    customer.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    //customer.CustomerType = Convert.ToInt32(dataReader["CustomerType"]);
                    customer.LineID = Convert.ToString(dataReader["LineID"])!=""? Convert.ToInt32(dataReader["LineID"]) : -1;
                    //customer.Barcode = Convert.ToString(dataReader["Barcode"]);
                    customer.MemberSince = Convert.ToDateTime(dataReader["MemberSince"]);

                    customer.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    customer.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    customer.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    customer.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                    customer.ItemName = Convert.ToString(dataReader["ItemName"]);
                    customer.LineNumber = Convert.ToInt32(dataReader["LineNumber"]);

                    retVal.Add(customer);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets single Customer based on Id
        /// </summary>
        /// <param name="customerId">Id of the Customer the needs to be retrieved</param>
        /// <returns>Instance of Customer</returns>
        public CustomerInfo GetCustomer(int customerId)
        {
            CustomerInfo retVal = new CustomerInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID],[CustomerNumber], [Name], [ContactPerson], [Address], [City], [Mobile], [EMail], [Balance], [IsActive], [CustomerType] , [Barcode], [MemberSince] , [ItemID], [Quantity], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Customers WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, customerId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    if (dataReader["Address"] == DBNull.Value)
                    {
                        retVal.Address = string.Empty;
                    }
                    else
                    {
                        retVal.Address = Convert.ToString(dataReader["Address"]);
                    }
                    if (dataReader["City"] == DBNull.Value)
                    {
                        retVal.City = string.Empty;
                    }
                    else
                    {
                        retVal.City = Convert.ToString(dataReader["City"]);
                    }
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

                    retVal.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.CustomerType = Convert.ToInt32(dataReader["CustomerType"]);
                    retVal.Barcode = Convert.ToString(dataReader["Barcode"]);
                    retVal.MemberSince = Convert.ToDateTime(dataReader["Membersince"]);
                    retVal.ItemID = Convert.ToInt32(dataReader["ItemID"]);
                    retVal.Quantity = Convert.ToDecimal(dataReader["Quantity"]);

                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds new Customer in to the database
        /// </summary>
        /// <param name="customer">Instance of Customer</param>
        /// <returns>Id of the newly added Customer</returns>
        public int AddCustomer(CustomerInfo customer, BindingList<CustomerItemsInfo> cutomerItems)
        {
            int retval = 0;
            DbCommand commandCustomer = _db.GetSqlStringCommand("INSERT INTO Customers([CustomerNumber], [Name], [NameMarathi],[ContactPerson], [Address], [City], [Mobile], [EMail],  [Balance],[Deposit], [IsActive],[LineID],[MemberSince], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@CustomerNumber, @Name, @NameMarathi,@ContactPerson, @Address, @City, @Mobile, @EMail, @Balance, @Deposit, @IsActive,@LineID, @MemberSince, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Customers')");

            _db.AddInParameter(commandCustomer, "@CustomerNumber", DbType.Int32, customer.CustomerNumber);
            _db.AddInParameter(commandCustomer, "@Name", DbType.String, customer.Name);
            _db.AddInParameter(commandCustomer, "@NameMarathi", DbType.String, customer.CustomerNameMarathi);
            _db.AddInParameter(commandCustomer, "@ContactPerson", DbType.String, customer.ContactPerson);
            if (customer.Address == string.Empty)
            {
                _db.AddInParameter(commandCustomer, "@Address", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandCustomer, "@Address", DbType.String, customer.Address);
            }
            if (customer.City == string.Empty)
            {
                _db.AddInParameter(commandCustomer, "@City", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandCustomer, "@City", DbType.String, customer.City);
            }
            if (customer.Mobile == string.Empty)
            {
                _db.AddInParameter(commandCustomer, "@Mobile", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandCustomer, "@Mobile", DbType.String, customer.Mobile);
            }
            if (customer.EMail == string.Empty)
            {
                _db.AddInParameter(commandCustomer, "@EMail", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandCustomer, "@EMail", DbType.String, customer.EMail);
            }

            _db.AddInParameter(commandCustomer, "@Balance", DbType.Decimal, customer.Balance);
            _db.AddInParameter(commandCustomer, "@Deposit", DbType.Decimal, customer.Deposit);
            _db.AddInParameter(commandCustomer, "@IsActive", DbType.Boolean, customer.IsActive);
            _db.AddInParameter(commandCustomer, "@CustomerType", DbType.Boolean, customer.IsActive);
            _db.AddInParameter(commandCustomer, "@LineID", DbType.Int32, customer.LineID);
            _db.AddInParameter(commandCustomer, "@Barcode", DbType.String, customer.Barcode);
            _db.AddInParameter(commandCustomer, "@MemberSince", DbType.DateTime, customer.MemberSince);

            _db.AddInParameter(commandCustomer, "@CreatedBy", DbType.Byte, customer.CreatedBy);
            _db.AddInParameter(commandCustomer, "@CreatedOn", DbType.DateTime, customer.CreatedOn);
            _db.AddInParameter(commandCustomer, "@UpdatedBy", DbType.Byte, customer.UpdatedBy);
            _db.AddInParameter(commandCustomer, "@UpdatedOn", DbType.DateTime, customer.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    retval = Convert.ToInt32(_db.ExecuteScalar(commandCustomer, transaction));

                    foreach (CustomerItemsInfo item in cutomerItems)
                    {
                        DbCommand commandSales = _db.GetSqlStringCommand("INSERT INTO Customer_Items ([CustomerID], [ItemID], [Quantity]) VALUES (@CustomerID, @ItemID, @Quantity)");
                        _db.AddInParameter(commandSales, "@CustomerID", DbType.Int32, retval);
                        _db.AddInParameter(commandSales, "@ItemID", DbType.Int32, item.ItemID);
                        _db.AddInParameter(commandSales, "@Quantity", DbType.Decimal, item.Quantity);

                        _db.ExecuteNonQuery(commandSales, transaction);
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
        /// Updates the Customer
        /// </summary>
        /// <param name="customer">Instance of Customer class</param>
        public void UpdateCustomer(CustomerInfo customer, BindingList<CustomerItemsInfo> customerItems)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Customers SET [CustomerNumber] = @CustomerNumber, [Name] = @Name, [NameMarathi]=@NameMarathi,[ContactPerson] = @ContactPerson, [Address] = @Address, [City] = @City, [Mobile] = @Mobile, [EMail] = @EMail, [Balance] = @Balance, [Deposit] = @Deposit, [IsActive] = @IsActive, LineID = @LineID, [MemberSince]=@MemberSince , [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, customer.ID);
            _db.AddInParameter(command, "@CustomerNumber", DbType.Int32, customer.CustomerNumber);
            _db.AddInParameter(command, "@Name", DbType.String, customer.Name);
            _db.AddInParameter(command, "@NameMarathi", DbType.String, customer.CustomerNameMarathi);
            _db.AddInParameter(command, "@ContactPerson", DbType.String, customer.ContactPerson);
            if (customer.Address == string.Empty)
            {
                _db.AddInParameter(command, "@Address", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Address", DbType.String, customer.Address);
            }
            if (customer.City == string.Empty)
            {
                _db.AddInParameter(command, "@City", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@City", DbType.String, customer.City);
            }
            if (customer.Mobile == string.Empty)
            {
                _db.AddInParameter(command, "@Mobile", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Mobile", DbType.String, customer.Mobile);
            }
            if (customer.EMail == string.Empty)
            {
                _db.AddInParameter(command, "@EMail", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@EMail", DbType.String, customer.EMail);
            }

            _db.AddInParameter(command, "@Balance", DbType.Decimal, customer.Balance);
            _db.AddInParameter(command, "@Deposit", DbType.Decimal, customer.Deposit);
            _db.AddInParameter(command, "@IsActive", DbType.Boolean, customer.IsActive);
            _db.AddInParameter(command, "@CustomerType", DbType.Int32, customer.CustomerType);
            _db.AddInParameter(command, "@LineID", DbType.Int32, customer.LineID);
            _db.AddInParameter(command, "@Barcode", DbType.String, customer.Barcode);
            _db.AddInParameter(command, "@MemberSince", DbType.DateTime, customer.MemberSince);

            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, customer.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, customer.UpdatedOn);


            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    _db.ExecuteNonQuery(command, transaction);


                    DbCommand commandDeleteCustomerItems = _db.GetSqlStringCommand("DELETE FROM Customer_Items WHERE CustomerID = @CustomerID");
                    _db.AddInParameter(commandDeleteCustomerItems, "@CustomerID", DbType.Int32, customer.ID);
                    _db.ExecuteNonQuery(commandDeleteCustomerItems, transaction);


                    foreach (CustomerItemsInfo item in customerItems)
                    {
                        DbCommand commandSales = _db.GetSqlStringCommand("INSERT INTO Customer_Items ([CustomerID], [ItemID], [Quantity]) VALUES (@CustomerID, @ItemID, @Quantity)");
                        _db.AddInParameter(commandSales, "@CustomerID", DbType.Int32, customer.ID);
                        _db.AddInParameter(commandSales, "@ItemID", DbType.Int32, item.ItemID);
                        _db.AddInParameter(commandSales, "@Quantity", DbType.Decimal, item.Quantity);

                        _db.ExecuteNonQuery(commandSales, transaction);
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
        /// Deletes the Customer from the database
        /// </summary>
        /// <param name="customerId">Id of the Customer that needs to be deleted</param>
        public void DeleteCustomer(int customerId)
        {
            DbCommand command = _db.GetSqlStringCommand("DELETE FROM Customers WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, customerId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                _db.ExecuteNonQuery(command);
            }
        }

        public void UpdateCustomerForLineChange(CustomerInfo customer, BindingList<CustomerInfo> customers)
        {
            try
            {
                using (DbConnection conn = _db.CreateConnection())
                {
                    foreach (CustomerInfo item in customers)
                    {
                        DbCommand command = _db.GetSqlStringCommand("Update Customers Set CustomerNumber=@NewCustomerNumber WHERE Id = @Id");
                        _db.AddInParameter(command, "@Id", DbType.Int32, item.ID);
                        _db.AddInParameter(command, "@NewCustomerNumber", DbType.Int32, item.NewCustomerNumber);
                        // conn.Open();
                        _db.ExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// Gets the count of Customers having the same name
        /// This method works in New Mode only
        /// </summary>
        /// <param name="customerName">Name of the Customer to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCount(string name)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select Count([Name]) From [Customers] WHERE [Name] = @Name");
            _db.AddInParameter(command, "@Name", DbType.String, name);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the count of Customers having the same name
        /// This method works in Edit Mode only
        /// </summary>
        /// <param name="customerName">Name of the Customer to be checked</param>
        /// <returns>No. of records having the same Name. 0 if no records are found</returns>
        public int GetSameNameCountForEditMode(string name, int customerId)
        {
            int retVal = 0;
            DbCommand command = _db.GetSqlStringCommand(" Select Count([Name]) From [Customers] WHERE [Name] = @Name AND Id != @Id");
            _db.AddInParameter(command, "@Name", DbType.String, name);
            _db.AddInParameter(command, "@Id", DbType.Int32, customerId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }
            return retVal;
        }


        public bool CheckCustomerUsed(int customerID, out string tableName)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(CustomerID) FROM Sales WHERE CustomerID = @CustomerID");
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, customerID);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                {
                    tableName = "Sales";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }
        /// <summary>
        /// Gets single Customer based on Id For Sales
        /// </summary>
        /// <param name="customerId">Id of the Customer the needs to be retrieved</param>
        /// <returns>Instance of Customer</returns>
        public CustomerInfo GetCustomerForSales(int customerId)
        {
            CustomerInfo retVal = new CustomerInfo();
            BindingList<CustomerItemsInfo> customerItemsInfo = new BindingList<CustomerItemsInfo>();

            DbCommand command = _db.GetSqlStringCommand("SELECT Customers.[ID],[CustomerNumber], Customers.[Name], [ContactPerson], [Address], [City], [Mobile], [EMail], [Balance], Customers.[IsActive], [CustomerType] , Customers.[Barcode], [MemberSince] , Customers.[CreatedBy], Customers.[CreatedOn], Customers.[UpdatedBy], Customers.[UpdatedOn] FROM Customers WHERE Customers.[CustomerNumber] = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, customerId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt32(dataReader["ID"]);
                    retVal.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    retVal.Name = Convert.ToString(dataReader["Name"]);
                    retVal.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    if (dataReader["Address"] == DBNull.Value)
                    {
                        retVal.Address = string.Empty;
                    }
                    else
                    {
                        retVal.Address = Convert.ToString(dataReader["Address"]);
                    }
                    if (dataReader["City"] == DBNull.Value)
                    {
                        retVal.City = string.Empty;
                    }
                    else
                    {
                        retVal.City = Convert.ToString(dataReader["City"]);
                    }
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

                    retVal.Balance = Convert.ToDecimal(dataReader["Balance"]);
                    retVal.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    retVal.CustomerType = Convert.ToInt32(dataReader["CustomerType"]);
                    retVal.Barcode = Convert.ToString(dataReader["Barcode"]);
                    retVal.MemberSince = Convert.ToDateTime(dataReader["Membersince"]);

                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }

            return retVal;
        }

        public BindingList<CustomerItemsInfo> GetCustomerItemsForSelectedCustomer(int CustomerID)
        {
            BindingList<CustomerItemsInfo> retVal = new BindingList<CustomerItemsInfo>();
            try
            {
                DbCommand commandCustomerItems = _db.GetSqlStringCommand("SELECT [ItemID], [ItemCode], [Quantity] FROM [Customer_Items] JOIN Items ON Customer_Items.ItemID = Items.ID WHERE CustomerID  = @CustomerID ");
                _db.AddInParameter(commandCustomerItems, "@CustomerID", DbType.Int32, CustomerID);

                using (IDataReader reader = _db.ExecuteReader(commandCustomerItems))
                {
                    while (reader.Read())
                    {
                        CustomerItemsInfo customerItems = new CustomerItemsInfo();
                        customerItems.ItemID = Convert.ToInt32(reader["ItemID"]);
                        customerItems.ItemCode = Convert.ToInt32(reader["ItemCode"]);
                        customerItems.Quantity = Convert.ToDecimal(reader["Quantity"]);

                        retVal.Add(customerItems);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retVal;
        }

        public int GetMaxCustomerNumber()
        {
            try
            {
                DbCommand command = null;

                command = _db.GetSqlStringCommand("SELECT ISNULL(Max(CustomerNumber), 0)+1 FROM Customers");

                using (DbConnection conn = _db.CreateConnection())
                {
                    conn.Open();
                    return Convert.ToInt32(_db.ExecuteScalar(command));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public bool CheckCustomerNumber(int CustomerNumber)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT ID FROM Customers WHERE CustomerNumber = @CustomerNumber");
            _db.AddInParameter(command, "@CustomerNumber", DbType.Int32, CustomerNumber);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                {
                    return true;
                }
            }
            return false;
        }


        public bool CheckBarCode(string BarCode)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT ID FROM Customers WHERE Barcode = @Barcode");
            _db.AddInParameter(command, "@Barcode", DbType.String, BarCode);
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                { return true; }
            }
            return false;
        }
        /// <summary>
        /// Purpose : To get the customer from his CSV ID
        /// Author : Amol
        /// Date : 11 th May 2016
        /// <returns></returns>
        public BindingList<CustomerInfo> GetCustomersOnCSV(string CustomerIDCSV)
        {
            try
            {
                BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();
                DbCommand command = _db.GetSqlStringCommand("SELECT [ID],[CustomerNumber], [Name], [ContactPerson], [Address], [City], [Mobile], [EMail], [Balance], [IsActive],[CustomerType],[Barcode],[MemberSince], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Customers WHERE ID IN ("+ @CustomerIDCSV + ") ORDER BY [Name]");

                using (IDataReader dataReader = _db.ExecuteReader(command))
                {
                    while (dataReader.Read())
                    {
                        CustomerInfo customer = new CustomerInfo();
                        customer.ID = Convert.ToInt32(dataReader["ID"]);
                        customer.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                        customer.Name = Convert.ToString(dataReader["Name"]);
                        customer.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                        if (dataReader["Address"] == DBNull.Value)
                        {
                            customer.Address = string.Empty;
                        }
                        else
                        {
                            customer.Address = Convert.ToString(dataReader["Address"]);
                        }
                        if (dataReader["City"] == DBNull.Value)
                        {
                            customer.City = string.Empty;
                        }
                        else
                        {
                            customer.City = Convert.ToString(dataReader["City"]);
                        }
                        if (dataReader["Mobile"] == DBNull.Value)
                        {
                            customer.Mobile = string.Empty;
                        }
                        else
                        {
                            customer.Mobile = Convert.ToString(dataReader["Mobile"]);
                        }
                        if (dataReader["EMail"] == DBNull.Value)
                        {
                            customer.EMail = string.Empty;
                        }
                        else
                        {
                            customer.EMail = Convert.ToString(dataReader["EMail"]);
                        }

                        customer.Balance = Convert.ToDecimal(dataReader["Balance"]);
                        customer.IsActive = Convert.ToBoolean(dataReader["IsActive"]);

                        customer.CustomerType = Convert.ToInt32(dataReader["CustomerType"]);
                        customer.Barcode = Convert.ToString(dataReader["Barcode"]);
                        customer.MemberSince = Convert.ToDateTime(dataReader["MemberSince"]);

                        customer.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                        customer.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                        customer.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                        customer.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                        retVal.Add(customer);
                    }
                }
                return retVal;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public BindingList<CustomerInfo> GetCustomersByLineID(int LineID)
        {
            BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();
            string whereClause = "";
           if (LineID > 0)
                whereClause = "WHERE LineID = @LineID";
            else if(LineID == 0)
                whereClause = "WHERE LineID IS NOT NULL";
           DbCommand command = _db.GetSqlStringCommand("SELECT [ID],[CustomerNumber], [Name], [ContactPerson] ,[LineID] FROM Customers " + whereClause + " ORDER BY [Name]");
            _db.AddInParameter(command, "LineID", DbType.Int32, LineID);
            

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    CustomerInfo customer = new CustomerInfo();
                    customer.ID = Convert.ToInt32(dataReader["ID"]);
                    customer.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    customer.Name = Convert.ToString(dataReader["Name"]);
                    customer.LineID = Convert.ToInt32(dataReader["LineID"]);
                    customer.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                    retVal.Add(customer);
                }
            }
            return retVal;
        }
        public BindingList<CustomerInfo> GetCustomersByIDs(int LineID, int CustomerNo,int CustomerID)
        {
            BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();
            string whereClause = "";
            //if (LineID > 0 && CustomerNo > 0)
            //    whereClause = "WHERE  isActive='True' AND LineID = @LineID OR CustomerNumber = @CustomerNo";
             if (LineID > 0)
             {
                 if (LineID > 0 && CustomerNo > 0)
                 {
                     whereClause = "WHERE  isActive='True' AND LineID = @LineID AND CustomerNumber = @CustomerNo";
                 }
                 else if (LineID > 0 && CustomerID > 0)
                 {
                     whereClause = "WHERE  isActive='True' AND LineID = @LineID AND CustomerID = @CustomerID";
                 }
                 else
                 {
                     whereClause = "WHERE isActive='True' And LineID = @LineID";
                 }

             }
            
             //else if (LineID > 0 && CustomerNo > 0)
             //    whereClause = "WHERE  isActive='True' AND LineID = @LineID AND CustomerNumber = @CustomerNo";
             //else if (LineID > 0 && CustomerID > 0)
                // whereClause = "WHERE  isActive='True' AND LineID = @LineID AND CustomerID = @CustomerID";
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID],[CustomerNumber], [Name], [ContactPerson] FROM Customers " + whereClause + " ORDER BY [Name]");
            _db.AddInParameter(command, "LineID", DbType.Int32, LineID);
            _db.AddInParameter(command, "CustomerNo", DbType.Int32, CustomerNo);
            _db.AddInParameter(command, "CustomerID", DbType.Int32, CustomerID);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    CustomerInfo customer = new CustomerInfo();
                    customer.ID = Convert.ToInt32(dataReader["ID"]);
                    customer.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    customer.Name = Convert.ToString(dataReader["Name"]);
                    customer.ContactPerson = Convert.ToString(dataReader["ContactPerson"]);
                  
                    retVal.Add(customer);
                }
            }
            return retVal;
        }
        public int GetCustomersLastNoByLineID(int LineID)
        {
            int retVal = 0;

            DbCommand command = _db.GetSqlStringCommand("SELECT top 1  ([CustomerNumber]) FROM Customers where LineID=@LineID order by ID desc");
            _db.AddInParameter(command, "@LineID", DbType.Int32, LineID);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {


                    retVal = Convert.ToInt32(dataReader["CustomerNumber"]);
                    
                }
            }
            return retVal;
        }

        public int GetCustomersNumberByID(int LineID, int CustomerID)
        {
            int retVal = 0;

            DbCommand command = _db.GetSqlStringCommand("SELECT [CustomerNumber] FROM Customers where LineID=@LineID AND ID=@CustomerID order by ID desc");
            _db.AddInParameter(command, "@LineID", DbType.Int32, LineID);
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, CustomerID);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {


                    retVal = Convert.ToInt32(dataReader["CustomerNumber"]);

                }
            }
            return retVal;
        }
    }
}