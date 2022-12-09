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
    /// Purpose	: This class is the SqlClient Data Access Logic implementation for the Sale table
    /// Author	: Kiran
    /// Date	: 16 Apr 2010 03:16:28 PM
    /// </summary>
    public class SaleDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public SaleDAL()
        {

        }

        public BindingList<SaleInfo> GetSalesAll()
        {
            BindingList<SaleInfo> retVal = new BindingList<SaleInfo>();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [BillDate], [BillNo], [CashCredit], [CustomerID], [TotalAmount], [DiscountPercentage], [DiscountAmount], [NetAmount], [RoundedAmount], [BalanceAmount], [Description], [RFIDTransaction], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Sales ORDER BY []");

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleInfo sale = new SaleInfo();
                    sale.ID = Convert.ToInt64(dataReader["ID"]);
                    sale.BillDate = Convert.ToDateTime(dataReader["BillDate"]);
                    sale.BillNo = Convert.ToString(dataReader["BillNo"]);
                    sale.CashCredit = Convert.ToInt32(dataReader["CashCredit"]);
                    sale.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                    sale.TotalAmount = Convert.ToDecimal(dataReader["TotalAmount"]);
                    sale.DiscountPercentage = Convert.ToDecimal(dataReader["DiscountPercentage"]);
                    sale.DiscountAmount = Convert.ToDecimal(dataReader["DiscountAmount"]);
                    sale.NetAmount = Convert.ToDecimal(dataReader["NetAmount"]);
                    sale.RoundedAmount = Convert.ToDecimal(dataReader["RoundedAmount"]);
                    sale.BalanceAmount = Convert.ToDecimal(dataReader["BalanceAmount"]);
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        sale.Description = string.Empty;
                    }
                    else
                    {
                        sale.Description = Convert.ToString(dataReader["Description"]);
                    }
                    sale.RFIDTransaction = Convert.ToBoolean(dataReader["RFIDTransaction"]);
                    sale.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    sale.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    sale.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    sale.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                    retVal.Add(sale);
                }
            }
            return retVal;
        }

        public BindingList<SaleInfo> GetSalesByFilter(int searchType, string name, object sDate, object eDate, int divisionID, int SaleType)
        {
            BindingList<SaleInfo> retVal = new BindingList<SaleInfo>();
            
            //Commented on 20 March 2015
            //string cmdString = "SELECT Sales.ID, Sales.BillDate, Sales.BillNo, Sales.CustomerID, Customers.Name CustomerName, Sales.CashCredit, Sales.TotalAmount, Sales.DiscountPercentage, Sales.DiscountAmount, Sales.NetAmount, Sales.RoundedAmount, Sales.BalanceAmount, Sales.IsPrinted, Sales.Description, Sales.RFIDTransaction, Sales.CardPaymentDetails, Sales.TotalWeight, Sales.ActualWeight, Sales.CreatedBy, Sales.CreatedOn, Sales.UpdatedBy, Sales.UpdatedOn FROM Sales LEFT OUTER JOIN Customers ON Sales.CustomerID = Customers.ID";
            string cmdString = "SELECT Sales.ID, Sales.BillDate, Sales.BillNo, Sales.CustomerID, Customers.Name CustomerName,Customers.LineID,Customers.CustomerNumber, Sales.CashCredit, Sales.TotalAmount, " +
                " Sales.DiscountPercentage, Sales.DiscountAmount, Sales.NetAmount, Sales.RoundedAmount, Sales.BalanceAmount, Sales.IsPrinted, Sales.Description, "+
                "Sales.RFIDTransaction, Sales.CardPaymentDetails, Sales.TotalWeight, Sales.ActualWeight, Sales.IsCouponSale, Sales.CreatedBy, Sales.CreatedOn, "+
                "Sales.UpdatedBy, Sales.UpdatedOn, Users.Name UserName, CASE  WHEN IsCouponSale = 1 THEN 'CoupanSale' WHEN CustomerID!=0 THEN 'CardSale' WHEN "+
                "CustomerID = 0 AND IsCouponSale = 0 THEN 'CashSale' END AS SaleType FROM Sales LEFT OUTER JOIN Customers ON Sales.CustomerID = Customers.ID "+
                "JOIN Users ON Sales.CreatedBy = Users.ID ";

            DbCommand command = null;
            if (searchType == 0)
            {
                if (name.Trim() != string.Empty)
                    command = _db.GetSqlStringCommand(cmdString + " WHERE Customers.Name LIKE '" + name + "%' AND Sales.DivisionID = @DivisionID AND Sales.BillDate >= @SDate AND Sales.BillDate <= @EDate ORDER BY SaleType,Sales.ID DESC");
                else
                    command = _db.GetSqlStringCommand(cmdString + " AND Sales.DivisionID = @DivisionID  AND Sales.BillDate >= @SDate AND Sales.BillDate <= @EDate ORDER BY SaleType,Sales.ID DESC");

                    _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                    _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            }
            if (searchType == 1)
            {
                string cmdSaleType = "";
                if (SaleType == 0)
                    cmdSaleType = " ORDER BY SaleType,Sales.ID DESC ";
                if (SaleType == 1)
                    cmdSaleType = " AND CustomerID!=0 ORDER BY SaleType,Sales.ID DESC ";
                if (SaleType == 2)
                    cmdSaleType = " AND IsCouponSale = 1 ORDER BY SaleType,Sales.ID DESC ";
                if (SaleType == 3)
                    cmdSaleType = " AND (CustomerID = 0 AND IsCouponSale = 0) ORDER BY SaleType,Sales.ID DESC  ";
                
                command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.BillDate >= @SDate AND Sales.BillDate <= @EDate AND Sales.DivisionID = @DivisionID " + cmdSaleType);
                _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
                _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            }
            if (searchType == 2)
            {
                command = _db.GetSqlStringCommand(cmdString + " WHERE Customers.CustomerNumber = @CustomerNumber AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
                _db.AddInParameter(command, "@CustomerNumber", DbType.Int32, Convert.ToInt32(name));
            }
            //if (searchType == 3)
            //{
            //    command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.NetAmount <= @Amount AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //    _db.AddInParameter(command, "@Amount", DbType.Decimal, Convert.ToDecimal(name));
            //}
            //if (searchType == 4)
            //{
            //    command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.BalanceAmount > 0 AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //}
            //if (searchType == 5)
            //{
            //    command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.IsPrinted = 'True' AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //}
            //if (searchType == 6)
            //{
            //    command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.IsPrinted = 'False' AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //}
            //if (searchType == 7)
            //{
            //    command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.CardPaymentDetails IS NOT NULL AND Sales.BillDate >= @SDate AND Sales.BillDate <= @EDate AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //    _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
            //    _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            //}
            //if (searchType == 8)
            //{
            //    command = _db.GetSqlStringCommand(cmdString + " WHERE Sales.ActualWeight > Sales.TotalWeight AND Sales.BillDate >= @SDate AND Sales.BillDate <= @EDate AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //    _db.AddInParameter(command, "@SDate", DbType.DateTime, (DateTime)sDate);
            //    _db.AddInParameter(command, "@EDate", DbType.DateTime, (DateTime)eDate);
            //}
            //if (searchType == 9)
            //{
            //    command = _db.GetSqlStringCommand("SELECT Sales.ID, Sales.BillDate, Sales.BillNo, Sales.CustomerID, Customers.Name CustomerName, Users.Name Description, "+
            //        "Sales.CashCredit, Sales.TotalAmount, Sales.DiscountPercentage, Sales.DiscountAmount, Sales.NetAmount, Sales.RoundedAmount, Sales.BalanceAmount, "+
            //        "Sales.IsPrinted, Sales.RFIDTransaction, Sales.CardPaymentDetails, Sales.TotalWeight, Sales.ActualWeight, Sales.CreatedBy, Sales.CreatedOn, "+
            //        "Sales.UpdatedBy, Sales.UpdatedOn , CASE  WHEN IsCouponSale = 1 THEN 'CoupanSale' WHEN CustomerID!=0 THEN 'CardSale' WHEN CustomerID = 0 "+
            //        "AND IsCouponSale = 0 THEN 'CashSale' END AS SaleType FROM Sales LEFT OUTER JOIN Customers ON Sales.CustomerID = Customers.ID "+
            //        "JOIN SaleItems ON SaleItems.SaleID = Sales.ID JOIN Users ON Sales.CreatedBy = Users.ID WHERE "+
            //        "SaleItems.IsSummarized = 'False' AND Sales.DivisionID = @DivisionID ORDER BY SaleType,Sales.ID DESC");
            //}

            _db.AddInParameter(command, "@DivisionID", DbType.Int32, divisionID);
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleInfo sale = new SaleInfo();
                    sale.ID = Convert.ToInt64(dataReader["ID"]);
                    sale.BillDate = Convert.ToDateTime(dataReader["CreatedOn"]);
                    sale.BillNo = Convert.ToString(dataReader["BillNo"]);
                    sale.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                    sale.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                    sale.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                    sale.CashCredit = Convert.ToByte(dataReader["CashCredit"]);
                    sale.TotalAmount = Convert.ToDecimal(dataReader["TotalAmount"]);
                    sale.DiscountPercentage = Convert.ToDecimal(dataReader["DiscountPercentage"]);
                    sale.DiscountAmount = Convert.ToDecimal(dataReader["DiscountAmount"]);
                    sale.NetAmount = Convert.ToDecimal(dataReader["NetAmount"]);
                    sale.RoundedAmount = Convert.ToDecimal(dataReader["RoundedAmount"]);
                    sale.BalanceAmount = Convert.ToDecimal(dataReader["BalanceAmount"]);
                    sale.IsPrint = Convert.ToBoolean(dataReader["IsPrinted"]);
                    sale.LineNumber = Convert.ToInt32(dataReader["LineID"]);
                    
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        sale.Description = string.Empty;
                    }
                    else
                    {
                        sale.Description = Convert.ToString(dataReader["Description"]);
                    }
                    sale.RFIDTransaction = Convert.ToBoolean(dataReader["RFIDTransaction"]);
                    if (dataReader["CardPaymentDetails"] == DBNull.Value)
                    {
                        sale.CardPaymentDetails = string.Empty;
                    }
                    else
                    {
                        sale.CardPaymentDetails = Convert.ToString(dataReader["CardPaymentDetails"]);
                    }
                    sale.TotalWeight = Convert.ToDecimal(dataReader["TotalWeight"]);
                    sale.ActualWeight = Convert.ToDecimal(dataReader["ActualWeight"]);
                    sale.IsCouponSale = Convert.ToBoolean(dataReader["IsCouponSale"]);
                    sale.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    sale.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    sale.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    sale.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                    sale.UserName = Convert.ToString(dataReader["UserName"]);
                    sale.DivisionID = divisionID;
                    retVal.Add(sale);
                }
            }
            return retVal;
        }

        public BindingList<SaleInfo> GetSalesByFilterForPrintCardDetails()
        {
            BindingList<SaleInfo> retVal = new BindingList<SaleInfo>();

            DbCommand command = _db.GetSqlStringCommand("SELECT TOP 25 Sales.ID, Sales.BillNo, Sales.RoundedAmount, Sales.IsPrinted, Sales.CardPaymentDetails, Sales.CreatedOn, Users.Name UserName,Customers.Name AS CustomerName FROM Sales JOIN Users ON Sales.CreatedBy = Users.ID LEFT JOIN Customers ON Customers.ID =CustomerID ORDER BY Sales.ID DESC");
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    SaleInfo sale = new SaleInfo();
                    sale.ID = Convert.ToInt64(dataReader["ID"]);
                    sale.BillDate = Convert.ToDateTime(dataReader["CreatedOn"]);
                    sale.BillNo = Convert.ToString(dataReader["BillNo"]);
                    sale.RoundedAmount = Convert.ToDecimal(dataReader["RoundedAmount"]);
                    sale.IsPrint = Convert.ToBoolean(dataReader["IsPrinted"]);
                    if (dataReader["CardPaymentDetails"] == DBNull.Value)
                    {
                        sale.CardPaymentDetails = string.Empty;
                    }
                    else
                    {
                        sale.CardPaymentDetails = Convert.ToString(dataReader["CardPaymentDetails"]);
                    }
                    sale.UserName = Convert.ToString(dataReader["UserName"]);
                    sale.CustomerName = Convert.ToString(dataReader["CustomerName"]);

                    int itemCount = 0;
                    DbCommand commandItem = _db.GetSqlStringCommand("SELECT Name FROM SaleItems JOIN Items ON ItemID = Items.ID WHERE SaleID = @SaleID");
                    _db.AddInParameter(commandItem, "@SaleID", DbType.Int64, sale.ID);
                    using (IDataReader dataReaderItem = _db.ExecuteReader(commandItem))
                    {
                        while (dataReaderItem.Read())
                        {
                            sale.Description += Convert.ToString(dataReaderItem["Name"] + ",");
                            itemCount += 1;
                        }
                    }
                    sale.UpdatedBy = Convert.ToByte(itemCount);

                    retVal.Add(sale);
                }
            }
            return retVal;
        }

        public SaleInfo GetSale(int saleId)
        {
            SaleInfo retVal = new SaleInfo();
            DbCommand command = _db.GetSqlStringCommand("SELECT [ID], [BillDate], [BillNo], [CashCredit], [CustomerID], [TotalAmount], [DiscountPercentage], [DiscountAmount], [NetAmount], [RoundedAmount], [BalanceAmount], [Description], [RFIDTransaction], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn] FROM Sales WHERE Id = @Id");
            _db.AddInParameter(command, "@Id", DbType.Int32, saleId);

            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    retVal.ID = Convert.ToInt64(dataReader["ID"]);
                    retVal.BillDate = Convert.ToDateTime(dataReader["BillDate"]);
                    retVal.BillNo = Convert.ToString(dataReader["BillNo"]);
                    retVal.CashCredit = Convert.ToInt32(dataReader["CashCredit"]);
                    retVal.CustomerID = Convert.ToInt32(dataReader["CustomerID"]);
                    retVal.TotalAmount = Convert.ToDecimal(dataReader["TotalAmount"]);
                    retVal.DiscountPercentage = Convert.ToDecimal(dataReader["DiscountPercentage"]);
                    retVal.DiscountAmount = Convert.ToDecimal(dataReader["DiscountAmount"]);
                    retVal.NetAmount = Convert.ToDecimal(dataReader["NetAmount"]);
                    retVal.RoundedAmount = Convert.ToDecimal(dataReader["RoundedAmount"]);
                    retVal.BalanceAmount = Convert.ToDecimal(dataReader["BalanceAmount"]);
                    if (dataReader["Description"] == DBNull.Value)
                    {
                        retVal.Description = string.Empty;
                    }
                    else
                    {
                        retVal.Description = Convert.ToString(dataReader["Description"]);
                    }
                    retVal.RFIDTransaction = Convert.ToBoolean(dataReader["RFIDTransaction"]);
                    retVal.CreatedBy = Convert.ToByte(dataReader["CreatedBy"]);
                    retVal.CreatedOn = Convert.ToDateTime(dataReader["CreatedOn"]);
                    retVal.UpdatedBy = Convert.ToByte(dataReader["UpdatedBy"]);
                    retVal.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);
                }
            }
            return retVal;
        }

        public int AddSale(SaleInfo sale, BindingList<SaleItemInfo> saleItems, string counterName, out Int64 billNumber)
        {
            int retval = 0;
            if (sale.CustomerNumber > 0)
            {
                DbCommand commandGetCustomerID = _db.GetSqlStringCommand("SELECT ID FROM Customers WHERE CustomerNumber = @CustomerNumber");
                _db.AddInParameter(commandGetCustomerID, "CustomerNumber", DbType.Int32, sale.CustomerNumber);
                sale.CustomerID = Convert.ToInt32(_db.ExecuteScalar(commandGetCustomerID));
            }
            DbCommand commandSale = _db.GetSqlStringCommand("INSERT INTO Sales([BillDate], [BillNo], [CashCredit], [CustomerID], [TotalAmount], [DiscountPercentage], [DiscountAmount], [NetAmount], [RoundedAmount], [BalanceAmount], [IsPrinted], [IsCouponSale], [Description], [RFIDTransaction], [CardPaymentDetails], [TotalWeight], [ActualWeight], [IsProcessed], [DivisionID] , [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]) " +
                                                        "VALUES (@BillDate, @BillNo, @CashCredit, @CustomerID, @TotalAmount, @DiscountPercentage, @DiscountAmount, @NetAmount, @RoundedAmount, @BalanceAmount, @IsPrint, @IsCouponSale, @Description, @RFIDTransaction, @CardPaymentDetails, @TotalWeight, @ActualWeight, @IsProcessed, @DivisionID, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn) " + Environment.NewLine +
                                                        "SELECT IDENT_CURRENT('Sales')");

            _db.AddInParameter(commandSale, "@BillDate", DbType.DateTime, sale.BillDate);
            _db.AddInParameter(commandSale, "@CashCredit", DbType.Int32, sale.CashCredit);
            _db.AddInParameter(commandSale, "@CustomerID", DbType.Int32, sale.CustomerID);
            _db.AddInParameter(commandSale, "@TotalAmount", DbType.Decimal, sale.TotalAmount);
            _db.AddInParameter(commandSale, "@DiscountPercentage", DbType.Decimal, sale.DiscountPercentage);
            _db.AddInParameter(commandSale, "@DiscountAmount", DbType.Decimal, sale.DiscountAmount);
            _db.AddInParameter(commandSale, "@NetAmount", DbType.Decimal, sale.NetAmount);
            _db.AddInParameter(commandSale, "@RoundedAmount", DbType.Decimal, sale.RoundedAmount);
            _db.AddInParameter(commandSale, "@BalanceAmount", DbType.Decimal, sale.BalanceAmount);
            _db.AddInParameter(commandSale, "@IsPrint", DbType.Boolean, sale.IsPrint);
            _db.AddInParameter(commandSale, "@IsCouponSale", DbType.Boolean, sale.IsCouponSale);
            if (sale.Description == string.Empty)
            {
                _db.AddInParameter(commandSale, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSale, "@Description", DbType.String, sale.Description);
            }
            _db.AddInParameter(commandSale, "@RFIDTransaction", DbType.Boolean, sale.RFIDTransaction);
            if (sale.CardPaymentDetails == string.Empty)
            {
                _db.AddInParameter(commandSale, "@CardPaymentDetails", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(commandSale, "@CardPaymentDetails", DbType.String, sale.CardPaymentDetails);
            }
            _db.AddInParameter(commandSale, "@TotalWeight", DbType.Decimal, sale.TotalWeight);
            _db.AddInParameter(commandSale, "@ActualWeight", DbType.Decimal, sale.ActualWeight);
            _db.AddInParameter(commandSale, "@DivisionID", DbType.Int32, sale.DivisionID);
            _db.AddInParameter(commandSale, "@IsProcessed", DbType.Boolean, sale.IsProcessed);

            _db.AddInParameter(commandSale, "@CreatedBy", DbType.Byte, sale.CreatedBy);
            _db.AddInParameter(commandSale, "@CreatedOn", DbType.DateTime, sale.CreatedOn);
            _db.AddInParameter(commandSale, "@UpdatedBy", DbType.Byte, sale.UpdatedBy);
            _db.AddInParameter(commandSale, "@UpdatedOn", DbType.DateTime, sale.UpdatedOn);

            Int64 billNo = 0;
            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    DbCommand commandGetBillNo = _db.GetSqlStringCommand("SELECT ISNULL(MAX(Convert(BigINT, BillNo)), 0) + 1 FROM Sales");

                    conn.Open();
                    transaction = conn.BeginTransaction();

                    billNo = Convert.ToInt64(_db.ExecuteScalar(commandGetBillNo, transaction));
                    _db.AddInParameter(commandSale, "@BillNo", DbType.Int64, billNo);

                    retval = Convert.ToInt32(_db.ExecuteScalar(commandSale, transaction));

                    DbCommand commandSaleDummy = _db.GetSqlStringCommand("INSERT INTO TotSalOrigAdd(SaleID, Amount) VALUES (@SaleID, @Amount)");
                    _db.AddInParameter(commandSaleDummy, "@SaleID", DbType.Int64, retval);
                    _db.AddInParameter(commandSaleDummy, "@Amount", DbType.Decimal, sale.NetAmount);
                    _db.ExecuteScalar(commandSaleDummy, transaction);

                    //AddSaleItems

                    foreach (SaleItemInfo saleItem in saleItems)
                    {
                        DbCommand commandSaleItem = _db.GetSqlStringCommand("INSERT INTO SaleItems([SaleID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount]) " +
                                                            "VALUES (@SaleID, @ItemID, @Quantity, @UnitID, @Vat, @Rate, @Amount) ");

                        _db.AddInParameter(commandSaleItem, "@SaleID", DbType.Int64, retval);
                        _db.AddInParameter(commandSaleItem, "@ItemID", DbType.Int32, saleItem.ItemID);
                        _db.AddInParameter(commandSaleItem, "@Quantity", DbType.Decimal, saleItem.Quantity);
                        _db.AddInParameter(commandSaleItem, "@UnitID", DbType.Int32, saleItem.UnitID);
                        _db.AddInParameter(commandSaleItem, "@Vat", DbType.Decimal, saleItem.Vat);
                        _db.AddInParameter(commandSaleItem, "@Rate", DbType.Decimal, saleItem.Rate);
                        _db.AddInParameter(commandSaleItem, "@Amount", DbType.Decimal, saleItem.Amount);

                        _db.ExecuteNonQuery(commandSaleItem, transaction);
                    }

                    //Update Counter Items Table.
                    DbCommand commandCounterItem = null;
                    foreach (SaleItemInfo saleItem in saleItems)
                    {
                        commandCounterItem = _db.GetSqlStringCommand("UPDATE Items SET " + counterName.Trim() + " = " + counterName.Trim() + " + 1 WHERE ID = @ID");
                        _db.AddInParameter(commandCounterItem, "@ID", DbType.Int32, saleItem.ItemID);
                        
                        _db.ExecuteScalar(commandCounterItem, transaction);
                    }

                    if (sale.BillFromOrder)
                    {
                        DbCommand commandUpdateBillNumberInOrders = null;
                        commandUpdateBillNumberInOrders = _db.GetSqlStringCommand("UPDATE Orders SET BillID = " + retval + " WHERE ID = @OrderID");
                        _db.AddInParameter(commandUpdateBillNumberInOrders, "@OrderID", DbType.Int32, sale.OrderID);

                        _db.ExecuteScalar(commandUpdateBillNumberInOrders, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            billNumber = billNo;
            return retval;
        }

        public void UpdateSale(SaleInfo sale, BindingList<SaleItemInfo> saleItems)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Sales SET [BillNo] = @BillNo,[BillDate] = @BillDate, [CashCredit] = @CashCredit, [CustomerID] = @CustomerID, [TotalAmount] = @TotalAmount, [DiscountPercentage] = @DiscountPercentage, [DiscountAmount] = @DiscountAmount, [NetAmount] = @NetAmount, [RoundedAmount] = @RoundedAmount, [BalanceAmount] = @BalanceAmount, [Description] = @Description, [CardPaymentDetails] = @CardPaymentDetails,[IsCouponSale] = @IsCouponSale, [UpdatedBy] = @UpdatedBy, [UpdatedOn] = @UpdatedOn WHERE Id = @Id ");

            _db.AddInParameter(command, "@Id", DbType.Int32, sale.ID);
            _db.AddInParameter(command, "@BillNo", DbType.Int64, sale.BillNo);
            _db.AddInParameter(command, "BillDate", DbType.DateTime, sale.BillDate);
            _db.AddInParameter(command, "@CashCredit", DbType.Int32, sale.CashCredit);
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, sale.CustomerID);
            _db.AddInParameter(command, "@TotalAmount", DbType.Decimal, sale.TotalAmount);
            _db.AddInParameter(command, "@DiscountPercentage", DbType.Decimal, sale.DiscountPercentage);
            _db.AddInParameter(command, "@DiscountAmount", DbType.Decimal, sale.DiscountAmount);
            _db.AddInParameter(command, "@NetAmount", DbType.Decimal, sale.NetAmount);
            _db.AddInParameter(command, "@RoundedAmount", DbType.Decimal, sale.RoundedAmount);
            _db.AddInParameter(command, "@BalanceAmount", DbType.Decimal, sale.BalanceAmount);
            if (sale.Description == string.Empty)
            {
                _db.AddInParameter(command, "@Description", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@Description", DbType.String, sale.Description);
            }
            _db.AddInParameter(command, "@IsCouponSale", DbType.Boolean, sale.IsCouponSale);
            if (sale.CardPaymentDetails == string.Empty)
            {
                _db.AddInParameter(command, "@CardPaymentDetails", DbType.String, DBNull.Value);
            }
            else
            {
                _db.AddInParameter(command, "@CardPaymentDetails", DbType.String, sale.CardPaymentDetails);
            }
            _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, sale.UpdatedBy);
            _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, sale.UpdatedOn);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    _db.ExecuteNonQuery(command, transaction);

                    //Update Counter Items Table.
                    DbCommand commandDeleteSaleItems = _db.GetSqlStringCommand("DELETE FROM SaleItems WHERE SaleID = @SaleId");
                    _db.AddInParameter(commandDeleteSaleItems, "@SaleId", DbType.Int32, sale.ID);
                    _db.ExecuteScalar(commandDeleteSaleItems, transaction);

                    //Add Sale Items.
                    foreach (SaleItemInfo saleItem in saleItems)
                    {
                        DbCommand commandSaleItem = _db.GetSqlStringCommand("INSERT INTO SaleItems([SaleID], [ItemID], [Quantity], [UnitID], [Vat], [Rate], [Amount]) " +
                                                            "VALUES (@SaleID, @ItemID, @Quantity, @UnitID, @Vat, @Rate, @Amount) ");

                        _db.AddInParameter(commandSaleItem, "@SaleID", DbType.Int64, sale.ID);
                        _db.AddInParameter(commandSaleItem, "@ItemID", DbType.Int32, saleItem.ItemID);
                        _db.AddInParameter(commandSaleItem, "@Quantity", DbType.Decimal, saleItem.Quantity);
                        _db.AddInParameter(commandSaleItem, "@UnitID", DbType.Int32, saleItem.UnitID);
                        _db.AddInParameter(commandSaleItem, "@Vat", DbType.Decimal, saleItem.Vat);
                        _db.AddInParameter(commandSaleItem, "@Rate", DbType.Decimal, saleItem.Rate);
                        _db.AddInParameter(commandSaleItem, "@Amount", DbType.Decimal, saleItem.Amount);

                        _db.ExecuteNonQuery(commandSaleItem, transaction);
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

        public void UpdateSalePrint(int billNumber)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Sales SET IsPrinted = 'True' WHERE BillNo = @BillNo ");

            _db.AddInParameter(command, "@BillNo", DbType.Int32, billNumber);

            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    _db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public void DeleteSale(int saleId, bool deleteAll)
        {
            using (DbConnection conn = _db.CreateConnection())
            {
                DbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    DbCommand command = _db.GetSqlStringCommand("DELETE FROM SaleItems WHERE SaleId = @SaleId");
                    _db.AddInParameter(command, "@SaleId", DbType.Int32, saleId);
                    _db.ExecuteNonQuery(command, transaction);

                    command = _db.GetSqlStringCommand("DELETE FROM Sales WHERE Id = @SaleId");
                    _db.AddInParameter(command, "@SaleId", DbType.Int32, saleId);
                    _db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public int GetNextBillNumber()
        {
            int retVal = 0;
            DbCommand command = null;

            command = _db.GetSqlStringCommand("SELECT ISNULL(MAX(Convert(INT, BillNo)), 0) + 1 FROM Sales");

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                retVal = Convert.ToInt32(_db.ExecuteScalar(command));
            }

            return retVal;
        }

        public bool CheckSaleUsed(int saleId, out string tableName)
        {
            DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(ID) FROM ReceiptPayments WHERE PartyTransaction = 'True' AND ReceiptPayment = 'Receipt' AND BillID = @SaleId");
            _db.AddInParameter(command, "@SaleId", DbType.Int32, saleId);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                if (Convert.ToInt32(_db.ExecuteScalar(command)) > 0)
                {
                    tableName = "Receipt Payments";
                    return true;
                }
            }
            tableName = string.Empty;
            return false;
        }

        public bool UpdateCardPaymentDetails(int saleId, string cardPaymentDetails)
        {
            DbCommand command = _db.GetSqlStringCommand("UPDATE Sales SET CardPaymentDetails = @CardPaymentDetails WHERE ID = @ID");
            _db.AddInParameter(command, "@Id", DbType.Int32, saleId);
            _db.AddInParameter(command, "@CardPaymentDetails", DbType.String, cardPaymentDetails);

            using (DbConnection conn = _db.CreateConnection())
            {
                try
                {
                    conn.Open();
                    _db.ExecuteScalar(command);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return true;
        }
    }
}