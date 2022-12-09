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

    public class SaleProcessingDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public SaleProcessingDAL()
        {
        }

        /// <summary>
        /// Purpose : To Process Sales Data at the end Of the month
        /// Author  : Jitendra
        /// Date    : 27 July 2015
        /// </summary>
        //public bool ProcessSalesData(DateTime BillDate, int CustomerID, out int errorType, int searchType, string customerName, int customerNo, Boolean isPaidAmount)

        public bool ProcessSalesData(DateTime BillDate, int CustomerID, out int errorType, string CustomersIDs, Boolean isPaidAmount)
        {
            bool result = true;
            CustomerDAL dal = new CustomerDAL();
            int error = 0;

            BindingList<CustomerInfo> retVal = new BindingList<CustomerInfo>();
            

            //Get Last Month First and Last Date
            var month = new DateTime(BillDate.Year, BillDate.Month, 1);
            var firstDate = month;
            var lastDate = month.AddMonths(1).AddDays(-1);
            int saleProcessingID = 0;
            int lastMonth = firstDate.AddDays(-1).Month;
            int lastYear = firstDate.AddDays(-1).Year;

            //if(!CheckIfPaidAmountPresent(BillDate, isPaidAmount,0))
            //{
                //GET Customer List

                //BindingList<SaleProcessingInfo> rVal = GetAllLSalesProcessingList(searchType, customerName, BillDate, customerNo);
                //if (rVal.Count > 0)
                //{
                //    foreach (SaleProcessingInfo saleProcessing in rVal)
                //    {
                //        CustomerInfo Customer = new CustomerInfo();
                //        Customer.ID = saleProcessing.CustomerID;
                //        retVal.Add(Customer);
                //    }
                //}
                //else
                //{ retVal = dal.GetCustomersAll(); }

                if (CustomersIDs != "")
                {
                    retVal = dal.GetCustomersOnCSV(CustomersIDs);
                }
                else
                retVal = dal.GetCustomersAll();

                foreach (CustomerInfo customer in retVal)
                {

                    if (!CheckIfPaidAmountPresent(BillDate, false, customer.ID))
                    try
                    {                        
                        SaleProcessingInfo saleprocess = new SaleProcessingInfo();

                        //Get TotalAmount From Sales
                        saleprocess.Amount = GetTotalAmountoftheCustomer(customer.ID, firstDate, lastDate);


                        //DELETE SaleProcessings And Items

                        DbCommand commandDeleteSaleProcessingItems = _db.GetSqlStringCommand("DELETE p FROM SaleProcessingItems p JOIN SaleProcessing ON p.SaleProcessingID = SaleProcessing.ID WHERE SaleProcessing.CustomerID = @CustomerID AND Month = @Month AND Year = @Year");
                        _db.AddInParameter(commandDeleteSaleProcessingItems, "@CustomerID", DbType.Int32, customer.ID);
                        _db.AddInParameter(commandDeleteSaleProcessingItems, "@Month", DbType.Int32, BillDate.Month);
                        _db.AddInParameter(commandDeleteSaleProcessingItems, "@Year", DbType.Int32, BillDate.Year);
                        _db.ExecuteNonQuery(commandDeleteSaleProcessingItems);


                        DbCommand commandDeleteSaleProcessing = _db.GetSqlStringCommand("DELETE FROM SaleProcessing WHERE CustomerID = @CustomerID AND Month = @Month AND Year = @Year ");
                        _db.AddInParameter(commandDeleteSaleProcessing, "@CustomerID", DbType.Int32, customer.ID);
                        _db.AddInParameter(commandDeleteSaleProcessing, "@Month", DbType.Int32, BillDate.Month);
                        _db.AddInParameter(commandDeleteSaleProcessing, "@Year", DbType.Int32, BillDate.Year);
                        _db.ExecuteNonQuery(commandDeleteSaleProcessing);

                        //========================================================================================================================================================
                        //========================================================================================================================================================
                        ////Commented this section on 29/07/2019 by me (Kiran) because Opening balance not taking properly as expected
                        ////New change below will take the balance from Saleprocessing itself for last (any) month only & from Master if no record present in SaleProcessing

                        ////////Check if previous Sales processing data exist or not
                        //////string previousMonthDate = new DateTime(BillDate.Year, BillDate.Month, 1).AddDays(-1).ToString("yyyyMMdd");
                        //////DbCommand commandCheckSaleProcessingData = _db.GetSqlStringCommand("SELECT COUNT(*)FROM SaleProcessing WHERE CustomerID = @CustomerID AND Month = DATEPART(MM,@PreviousDate) AND Year = DATEPART(YYYY,@PreviousDate) ");
                        //////_db.AddInParameter(commandCheckSaleProcessingData, "@CustomerID", DbType.Int32, customer.ID);
                        //////_db.AddInParameter(commandCheckSaleProcessingData, "@PreviousDate", DbType.String, previousMonthDate);
                        //////int cnt = Convert.ToInt32(_db.ExecuteScalar(commandCheckSaleProcessingData));

                        //////if (cnt > 0)
                        //////{
                        //////    //Get Closing Balance From the SaleProcessing for the particular Customer
                        //////    DbCommand commandSaleOpningBal = _db.GetSqlStringCommand("SELECT TOP 1 (ClosingBalance) ClosingBalance From SaleProcessing WHERE CustomerID = @CustomerID AND Month = @LastMonth AND Year = @LastYear ORDER BY ID DESC ");
                        //////    _db.AddInParameter(commandSaleOpningBal, "@CustomerID", DbType.Int32, customer.ID);
                        //////    _db.AddInParameter(commandSaleOpningBal, "@LastMonth", DbType.Int32, lastMonth);
                        //////    _db.AddInParameter(commandSaleOpningBal, "@LastYear", DbType.Int32, lastYear);
                        //////    using (IDataReader reader = _db.ExecuteReader(commandSaleOpningBal))
                        //////    {
                        //////        while (reader.Read())
                        //////        {
                        //////            saleprocess.OpeningBalance = Convert.ToDecimal(reader["ClosingBalance"]);
                        //////        }
                        //////        reader.Close();
                        //////    }
                        //========================================================================================================================================================
                        //========================================================================================================================================================

                        //Check if previous all Sales processing data exist or not
                        string previousMonthDate = new DateTime(BillDate.Year, BillDate.Month, 1).AddDays(-1).ToString("yyyyMMdd");
                        DbCommand commandCheckSaleProcessingData = _db.GetSqlStringCommand("SELECT COUNT(*) FROM SaleProcessing WHERE CustomerID = @CustomerID AND CAST(CAST([YEAR] as Varchar)+'-'+CAST([MONTH] as Varchar) +'-01' as Date) < @ThisMonthStartDate");
                        _db.AddInParameter(commandCheckSaleProcessingData, "@CustomerID", DbType.Int32, customer.ID);
                        _db.AddInParameter(commandCheckSaleProcessingData, "@ThisMonthStartDate", DbType.Date, month);

                        int cnt = Convert.ToInt32(_db.ExecuteScalar(commandCheckSaleProcessingData));

                        if (cnt > 0)
                        {
                            //Get Closing Balance From the SaleProcessing for the particular Customer from last processing month
                            DbCommand commandSaleOpningBal = _db.GetSqlStringCommand("SELECT TOP 1 ClosingBalance, CAST(CAST([YEAR] as Varchar)+'-'+CAST([MONTH] as Varchar) +'-01' as Date) Dt From SaleProcessing WHERE CustomerID = @CustomerID AND CAST(CAST([YEAR] as Varchar)+'-'+CAST([MONTH] as Varchar) +'-01' as Date) < @ThisMonthStartDate ORDER BY Dt DESC");
                            _db.AddInParameter(commandSaleOpningBal, "@CustomerID", DbType.Int32, customer.ID);
                            _db.AddInParameter(commandSaleOpningBal, "@ThisMonthStartDate", DbType.Date, month);

                            using (IDataReader reader = _db.ExecuteReader(commandSaleOpningBal))
                            {
                                while (reader.Read())
                                {
                                    saleprocess.OpeningBalance = Convert.ToDecimal(reader["ClosingBalance"]);
                                }
                                reader.Close();
                            }
                        }
                        else
                        {
                            //Get Closing Balance From the Customer Master (OpeningBalance field)
                            DbCommand commandSaleOpningBal = _db.GetSqlStringCommand("SELECT ISNULL(Balance, 0) OpeningBalance From Customers WHERE ID = @CustomerID");
                            _db.AddInParameter(commandSaleOpningBal, "@CustomerID", DbType.Int32, customer.ID);
                            using (IDataReader reader = _db.ExecuteReader(commandSaleOpningBal))
                            {
                                while (reader.Read())
                                {
                                    saleprocess.OpeningBalance = Convert.ToDecimal(reader["OpeningBalance"]);
                                }
                                reader.Close();
                            }
                        }
                        //Get Sale Items
                        BindingList<SaleProcessingItemsInfo> retVals = GetSalesItemsofCustomerforthePeriod(customer.ID, firstDate, lastDate);
                       // if (saleprocess.Amount == 0)
                        if (saleprocess.OpeningBalance > 0 || saleprocess.Amount > 0)   //Take those members whose previous balance is there & no transaction in this month.
                        {
                            using (DbConnection conn = _db.CreateConnection())
                            {
                                //Update Sales 
                                UpdateSalesForIsProcessed(customer.ID, firstDate, lastDate);
                                //Insert Into Sale Processing and Items
                                DbTransaction transaction = null;
                                conn.Open();
                                transaction = conn.BeginTransaction();

                               // saleprocess.OpeningBalance = (saleprocess.OpeningBalance * 5 / 100);

                                if (saleprocess.OpeningBalance > 0) //Not applicable for minus (-) balance
                                    saleprocess.OpeningBalance = saleprocess.OpeningBalance + (saleprocess.OpeningBalance * Convert.ToDecimal(0.05));

                                DbCommand commandInsertintoSaleProcessing = _db.GetSqlStringCommand("INSERT INTO SaleProcessing([Month], [Year], [CustomerID], [Amount], [OpeningBalance], [PaidAmount], [ClosingBalance], [AdjustmentAmount]) VALUES (@Month, @Year, @CustomerID, @Amount, @OpeningBalance, @PaidAmount, @ClosingBalance, @AdjustmentAmount) SELECT IDENT_CURRENT('SaleProcessing')");
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@Month", DbType.Int32, BillDate.Month);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@Year", DbType.Int32, BillDate.Year);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@CustomerID", DbType.Int32, customer.ID);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@Amount", DbType.Decimal, saleprocess.Amount);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@OpeningBalance", DbType.Decimal, saleprocess.OpeningBalance);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@PaidAmount", DbType.Decimal, 0);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@ClosingBalance", DbType.Decimal, saleprocess.Amount + saleprocess.OpeningBalance);
                                _db.AddInParameter(commandInsertintoSaleProcessing, "@AdjustmentAmount", DbType.Decimal, 0);

                                saleProcessingID = Convert.ToInt32(_db.ExecuteScalar(commandInsertintoSaleProcessing, transaction));
                                if (retVals.Count > 0)
                                {
                                    foreach (SaleProcessingItemsInfo items in retVals)
                                    {
                                        DbCommand commandInsertintoSaleProcessingItems = _db.GetSqlStringCommand("INSERT INTO SaleProcessingItems(SaleProcessingID, ItemID, Details, Amount, ItemQuantity) VALUES (@SaleProcessingID, @ItemID, @Details, @Amount, @ItemQuantity)");
                                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@SaleProcessingID", DbType.Int32, saleProcessingID);
                                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@ItemID", DbType.Int32, items.ItemID);
                                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@Details", DbType.String, items.Details);
                                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@Amount", DbType.Decimal, items.Amount);
                                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "ItemQuantity", DbType.Decimal, items.ItemQuantity);

                                        _db.ExecuteNonQuery(commandInsertintoSaleProcessingItems, transaction);
                                    }
                                }
                                transaction.Commit();
                                conn.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        throw new ApplicationException(ex.Message);
                    }
                }
                if (saleProcessingID > 0)
                    result = true;
                else
                {
                    error = 2;
                    result = false;
                }
            //}
            //else
            //{
            //    error = 1;
            //    result = false;                
            //}           

            errorType = error;
            return result;
        }
        
        /// <summary>
        /// Author  : Jitendra 
        /// Purpose : Update SalesProcessing table Column IsProcessed To True;
        /// </summary>
        private void UpdateSalesForIsProcessed(int CustomerID, DateTime firstDate, DateTime lastDate)
        {
            try
            {
                DbCommand commandUpdateSales = _db.GetSqlStringCommand("UPDATE Sales SET [IsProcessed] = @IsProcessed WHERE CustomerID=@CustomerID AND BillDate BETWEEN @fromDate AND @toDate ");
                _db.AddInParameter(commandUpdateSales, "@CustomerID", DbType.Int32, CustomerID);
                _db.AddInParameter(commandUpdateSales, "@fromDate", DbType.DateTime, firstDate);
                _db.AddInParameter(commandUpdateSales, "@toDate", DbType.DateTime, lastDate);
                _db.AddInParameter(commandUpdateSales, "@IsProcessed", DbType.Boolean, true);

                _db.ExecuteNonQuery(commandUpdateSales);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Author  : Jitendra
        /// Purpose : Check If Data Is Present for the Selected Month For Processing the Sale
        /// </summary>
        public int CheckIfDataPresentInSalesProcessing(DateTime BillDate, int CustomerID,out SaleProcessingInfo retVal)
        {
            SaleProcessingInfo sales = new SaleProcessingInfo();
            int result = 0;
            try
            {                
                DbCommand commandPaidAmount = _db.GetSqlStringCommand("SELECT [ID], [Month], [Year], [CustomerID], [Amount], [OpeningBalance], [PaidAmount], [ClosingBalance] FROM SaleProcessing WHERE CustomerID=@CustomerID AND SaleProcessing.Month = @Month AND SaleProcessing.Year = @Year");
                _db.AddInParameter(commandPaidAmount, "@CustomerID", DbType.Int32, CustomerID);
                _db.AddInParameter(commandPaidAmount, "@Month", DbType.Int32, BillDate.Month);
                _db.AddInParameter(commandPaidAmount, "@Year", DbType.Int32, BillDate.Year);

                using (IDataReader reader = _db.ExecuteReader(commandPaidAmount))
                {
                    while (reader.Read())
                    {
                        sales.ID = Convert.ToInt32(reader["ID"]);
                        sales.Month = Convert.ToInt32(reader["Month"]);
                        sales.Year = Convert.ToInt32(reader["Year"]);
                        sales.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                        sales.Amount = Convert.ToDecimal(reader["Amount"]);
                        sales.OpeningBalance = Convert.ToDecimal(reader["OpeningBalance"]);
                        sales.PaidAmount = Convert.ToDecimal(reader["PaidAmount"]);
                        sales.ClosingBalance = Convert.ToDecimal(reader["ClosingBalance"]);
                    }
                    reader.Close();
                }
                
                if (sales.ID > 0)
                {
                    if (sales.PaidAmount > 0)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 2;
                    }
                }
                else
                {
                    result = 3;  
                }
            }
            catch (Exception ex)
            {                
                throw;
            }
            retVal = sales;
            return result;
        }

        /// <summary>
        /// Author  : Jitendra
        /// Purpose : Process Sales Data When a Particular Member is Selected 
        /// </summary>
        /// <param name="Mode"> 1 = to Overwirite when Paid Amount Is Present   2 = to Insert New Data WHen No Paid Amount is Present</param>        
        public bool ProcessSalesDataOfSingleCustomers(int Mode, DateTime BillDate, SaleProcessingInfo salesprocessing)
        {
            var month = new DateTime(BillDate.Year, BillDate.Month, 1);
            var firstDate = month;
            var lastDate = month.AddMonths(1).AddDays(-1);
            
            if (Mode == 1)
            {
                decimal Amount = 0;
                try
                {
                    //Overwrite the Sales Table when Paid Amount Is Present
                    //Get New Total Amount
                    Amount = GetTotalAmountoftheCustomer(salesprocessing.CustomerID, firstDate, lastDate);

                    //Get Sale Items
                    BindingList<SaleProcessingItemsInfo> retVals = GetSalesItemsofCustomerforthePeriod(salesprocessing.CustomerID, firstDate, lastDate);

                    //Update sales Table IsProcessed Column To True
                    UpdateSalesForIsProcessed(salesprocessing.CustomerID, firstDate, lastDate);

                    //Update Sales Processing
                    DbCommand commandOverwirteSaleProcessing = _db.GetSqlStringCommand("UPDATE SaleProcessing SET Amount = @Amount , ClosingBalance = @ClosingBalance WHERE ID=@ID ");
                    _db.AddInParameter(commandOverwirteSaleProcessing, "@ID", DbType.Int16, salesprocessing.ID);
                    _db.AddInParameter(commandOverwirteSaleProcessing, "@Amount", DbType.Decimal, Amount);                    
                    _db.AddInParameter(commandOverwirteSaleProcessing, "@ClosingBalance", DbType.Decimal, Amount + salesprocessing.OpeningBalance);

                    _db.ExecuteNonQuery(commandOverwirteSaleProcessing);                    
                    
                    //Delete  from sales Processing Items
                    DbCommand commandDeleteSaleProcessingItems = _db.GetSqlStringCommand("DELETE FROM SaleProcessingItems WHERE SaleProcessingID = @ID");
                    _db.AddInParameter(commandDeleteSaleProcessingItems, "@ID", DbType.Int32, salesprocessing.ID);
                    _db.ExecuteNonQuery(commandDeleteSaleProcessingItems);

                    //Insert New Sales Processing Items
                    foreach (SaleProcessingItemsInfo items in retVals)
                    {
                        DbCommand commandInsertintoSaleProcessingItems = _db.GetSqlStringCommand("INSERT INTO SaleProcessingItems(SaleProcessingID, ItemID, Details,Amount) VALUES (@SaleProcessingID, @ItemID, @Details,@Amount)");
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@SaleProcessingID", DbType.Int32, salesprocessing.ID);
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@ItemID", DbType.Int32, items.ItemID);
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@Details", DbType.String, items.Details);
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@Amount", DbType.Decimal, items.Amount);

                        _db.ExecuteNonQuery(commandInsertintoSaleProcessingItems);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else if(Mode == 2)
            {
                int saleProcessingID = 0;
                decimal Amount = 0;
                //When Paid Amount Not Present Delete the Exsisting Data and Enter New Data
                DbCommand commandDeleteSaleProcessing = _db.GetSqlStringCommand("DELETE FROM SaleProcessing WHERE ID = @ID ");
                _db.AddInParameter(commandDeleteSaleProcessing, "@ID", DbType.Int32, salesprocessing.ID);
                _db.ExecuteNonQuery(commandDeleteSaleProcessing);

                DbCommand commandDeleteSaleProcessingItems = _db.GetSqlStringCommand("DELETE FROM SaleProcessingItems WHERE SaleProcessingID = @ID");
                _db.AddInParameter(commandDeleteSaleProcessingItems, "@ID", DbType.Int32, salesprocessing.ID);               
                _db.ExecuteNonQuery(commandDeleteSaleProcessingItems);

                //Get New Total Amount
                Amount = GetTotalAmountoftheCustomer(salesprocessing.CustomerID, firstDate, lastDate);

                //Update sales Table IsProcessed Column To True
                UpdateSalesForIsProcessed(salesprocessing.CustomerID, firstDate, lastDate);

                //Get Sale Items
                BindingList<SaleProcessingItemsInfo> retVals = GetSalesItemsofCustomerforthePeriod(salesprocessing.CustomerID, firstDate, lastDate);


                //Insert Into SaleProcessing and Sales PrcessingItems
                using (DbConnection conn = _db.CreateConnection())
                {
                    DbTransaction transaction = null;
                    conn.Open();

                    transaction = conn.BeginTransaction();

                    DbCommand commandInsertintoSaleProcessing = _db.GetSqlStringCommand("INSERT INTO SaleProcessing([Month], [Year], [CustomerID], [Amount], [OpeningBalance], [PaidAmount],[ClosingBalance],[AdjustmentAmount]) VALUES (@Month, @Year, @CustomerID, @Amount, @OpeningBalance, @PaidAmount,@ClosingBalance,@AdjustmentAmount) SELECT IDENT_CURRENT('SaleProcessing')");
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@Month", DbType.Int32, BillDate.Month);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@Year", DbType.Int32, BillDate.Year);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@CustomerID", DbType.Int32, salesprocessing.CustomerID);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@Amount", DbType.Decimal, Amount);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@OpeningBalance", DbType.Decimal, salesprocessing.OpeningBalance);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@PaidAmount", DbType.Decimal, 0);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@ClosingBalance", DbType.Decimal, Amount + salesprocessing.OpeningBalance);
                    _db.AddInParameter(commandInsertintoSaleProcessing, "@AdjustmentAmount", DbType.Decimal, 0);

                    saleProcessingID = Convert.ToInt32(_db.ExecuteScalar(commandInsertintoSaleProcessing, transaction));

                    foreach (SaleProcessingItemsInfo items in retVals)
                    {
                        DbCommand commandInsertintoSaleProcessingItems = _db.GetSqlStringCommand("INSERT INTO SaleProcessingItems(SaleProcessingID, ItemID, Details,Amount) VALUES (@SaleProcessingID, @ItemID, @Details,@Amount)");
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@SaleProcessingID", DbType.Int32, saleProcessingID);
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@ItemID", DbType.Int32, items.ItemID);
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@Details", DbType.String, items.Details);
                        _db.AddInParameter(commandInsertintoSaleProcessingItems, "@Amount", DbType.Decimal, items.Amount);

                        _db.ExecuteNonQuery(commandInsertintoSaleProcessingItems, transaction);
                    }
                    transaction.Commit();
                    conn.Close();
                }                
            }
            return false;
        }

        /// <summary>
        /// Author  : Jitendra
        /// Purpose : To Get the List of sales Items of the Customer
        /// </summary>
        private BindingList<SaleProcessingItemsInfo> GetSalesItemsofCustomerforthePeriod(int CustomerID, DateTime firstDate, DateTime lastDate)
        {
            BindingList<SaleProcessingItemsInfo> retVals = new BindingList<SaleProcessingItemsInfo>();
            try
            {
                //DbCommand commandSaleItems = _db.GetSqlStringCommand("SELECT * FROM (SELECT P.ID, STUFF((SELECT ' , ' + CONVERT(VARCHAR, CAST(a.BillDate AS DATE) ) +': '+CONVERT(VARCHAR, b.Quantity) FROM Sales a  JOIN SaleItems b  ON a.ID = b.SaleID  JOIN Items ON Items.ID = b.ItemID WHERE CustomerID=@CustomerID  AND BillDate BETWEEN @fromDate AND @toDate AND ItemID=p.ID FOR XML PATH ('')) , 1, 1, '')  AS Details FROM Items  as p) c WHERE Details is NOT NULL ");
                //DbCommand commandSaleItems = _db.GetSqlStringCommand("SELECT ItemID, STUFF((SELECT CONVERT(Varchar(2),DATEPART(DAY,a.BillDate))+'-'+ CONVERT(VARCHAR, Quantity) + ', '  FROM Sales a JOIN SaleItems  b ON a.ID = b.SaleID JOIN Items c  ON c.ID = b.ItemID  AND e.ItemID = b.ItemID WHERE CustomerID = @CustomerID AND BillDate BETWEEN @fromDate AND @toDate FOR XML PATH('')),5,1,' ') AS [Details], SUM(Amount) Amount, SUM(Quantity) AS ItemQuantity FROM SaleItems e JOIN Sales ON e.SaleID = Sales.ID WHERE  CustomerID = @CustomerID AND BillDate BETWEEN @fromDate AND @toDate GROUP BY ItemID");

                DbCommand commandSaleItems = _db.GetSqlStringCommand("SELECT ItemID, STUFF((SELECT CONVERT(Varchar(2),DATEPART(DD,a.BillDate))+'-'+ CONVERT(VARCHAR, Quantity) + ', '  FROM Sales a JOIN SaleItems  b ON a.ID = b.SaleID JOIN Items c  ON c.ID = b.ItemID  AND e.ItemID = b.ItemID WHERE CustomerID = @CustomerID AND BillDate BETWEEN @fromDate AND @toDate FOR XML PATH('')),9,1,' ') AS [Details], SUM(Amount) Amount, SUM(Quantity) AS ItemQuantity FROM SaleItems e JOIN Sales ON e.SaleID = Sales.ID WHERE  CustomerID = @CustomerID AND BillDate BETWEEN @fromDate AND @toDate GROUP BY ItemID");
                _db.AddInParameter(commandSaleItems, "@CustomerID", DbType.Int32, CustomerID);
                _db.AddInParameter(commandSaleItems, "@fromDate", DbType.DateTime, firstDate);
                _db.AddInParameter(commandSaleItems, "@toDate", DbType.DateTime, lastDate);
                using (IDataReader reader = _db.ExecuteReader(commandSaleItems))
                {
                    while (reader.Read())
                    {
                        SaleProcessingItemsInfo items = new SaleProcessingItemsInfo();
                        items.ItemID = Convert.ToInt32(reader["ItemID"]);
                        items.Details = Convert.ToString(reader["Details"]);
                        items.Amount = Convert.ToDecimal(reader["Amount"]);
                        items.ItemQuantity = Convert.ToDecimal(reader["ItemQuantity"]);

                        retVals.Add(items);
                    }
                }

                //DbCommand commandSaleItemsHomeDelivery = _db.GetSqlStringCommand(@"SELECT  1 as ItemID ,STUFF((SELECT CONVERT(Varchar(2),DATEPART(DD,MilkDeliveryDate))+'-'+ CONVERT(VARCHAR, Cow) + ', '  FROM HomeDeliveryMilk  WHERE CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate FOR XML PATH('')),9,1,' ') AS [Details],(Select Sum(Items.Rate*cow)TotalCow  from HomeDeliveryMilk join items on Items.ID=1 WHERE CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate)as Amount, Cow as ItemQuantity FROM HomeDeliveryMilk JOIN Items  b ON  b.ID=1 WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate UNION
                 //                                                                 SELECT  2 as ItemID ,STUFF((SELECT CONVERT(Varchar(2),DATEPART(DD,MilkDeliveryDate))+'-'+ CONVERT(VARCHAR, Cow) + ', '  FROM HomeDeliveryMilk  WHERE CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate FOR XML PATH('')),9,1,' ') AS [Details],(Select (Items.Rate*Buffalo)TotalBuffalo  from HomeDeliveryMilk join items on Items.ID=2 WHERE CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate)as Amount, Buffalo as ItemQuantity FROM HomeDeliveryMilk JOIN Items  b ON  b.ID=2 WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate ");
                DbCommand commandSaleItemsHomeDelivery = _db.GetSqlStringCommand(@"SELECT ItemID, STUFF((SELECT CONVERT(Varchar(2),DATEPART(DD,a.BillDate))+'-'+ CONVERT(VARCHAR, Quantity) + ', '  FROM (Select 2 ItemID,MilkDeliveryDate BillDate,Buffalo Quantity, Rate * Buffalo Amount FROM HomeDeliveryMilk join items on Items.ID=2 WHERE Buffalo > 0 AND CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate
                                                                                UNION ALL
                                                                                Select 1 ItemID,MilkDeliveryDate BillDate,Cow Quantity, Rate * Cow Amount FROM HomeDeliveryMilk join items on Items.ID=1 WHERE Cow > 0 AND CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate) a JOIN Items c  ON c.ID = a.ItemID  AND e.ItemID = a.ItemID FOR XML PATH('')),9,0,' ') AS [Details], SUM(Amount) Amount, SUM(Quantity) AS ItemQuantity FROM (Select 2 ItemID,MilkDeliveryDate BillDate,Buffalo Quantity, Rate * Buffalo Amount FROM HomeDeliveryMilk join items on Items.ID=2 WHERE Buffalo > 0 AND CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate
                                                                                UNION ALL
                                                                                Select 1 ItemID,MilkDeliveryDate BillDate,Cow Quantity, Rate * Cow Amount FROM HomeDeliveryMilk join items on Items.ID=1 WHERE Cow > 0 AND CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate) e GROUP BY ItemID");
                _db.AddInParameter(commandSaleItemsHomeDelivery, "@CustomerID", DbType.Int32, CustomerID);
                _db.AddInParameter(commandSaleItemsHomeDelivery, "@fromDate", DbType.DateTime, firstDate);
                _db.AddInParameter(commandSaleItemsHomeDelivery, "@toDate", DbType.DateTime, lastDate);
                using (IDataReader reader = _db.ExecuteReader(commandSaleItemsHomeDelivery))
                {
                    while (reader.Read())
                    {
                        SaleProcessingItemsInfo items = new SaleProcessingItemsInfo();
                        items.ItemID = Convert.ToInt32(reader["ItemID"]);
                        items.Details = Convert.ToString(reader["Details"]);
                        items.Amount = Convert.ToDecimal(reader["Amount"]);
                        items.ItemQuantity = Convert.ToDecimal(reader["ItemQuantity"]);

                        retVals.Add(items);
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return retVals;
        }

        /// <summary>
        /// Author  : Jitendra
        /// Purpose : To Get the Total Amount of sales of the Customer for the Month
        /// </summary>
        private decimal GetTotalAmountoftheCustomer(int CustomerID, DateTime firstDate, DateTime lastDate)
        {
            decimal Amount = 0;
            try
            {
                //DbCommand commandSales = _db.GetSqlStringCommand("SELECT ISNULL(SUM([TotalAmount]),0) TotalAmount FROM Sales WHERE CustomerID=@CustomerID AND BillDate BETWEEN @fromDate AND @toDate ");
//                DbCommand commandSales = _db.GetSqlStringCommand(@"Select (SELECT ISNULL(SUM([TotalAmount]),0) TotalAmount FROM Sales WHERE CustomerID=@CustomerID AND BillDate BETWEEN  @fromDate AND @toDate)+
//                                                                (Select ISNULL(SUM(Items.Rate*cow),0) from  HomeDeliveryMilk  join Items on Items.ID=(Select ID From Items where ID=1)WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate)+
//                                                                (Select ISNULL(SUM(Items.Rate*Buffalo),0) from  HomeDeliveryMilk join Items on Items.ID=(Select ID From Items where ID=2)WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate)As TotalAmount from HomeDeliveryMilk WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate");


                //Rate From Transaction Table HomeDeliveryMilk

                DbCommand commandSales = _db.GetSqlStringCommand(@"Select distinct (SELECT ISNULL(SUM([TotalAmount]),0)  FROM Sales WHERE CustomerID=@CustomerID AND BillDate BETWEEN  @fromDate AND @toDate)+
                                                                  (Select ISNULL(SUM(CowRate*cow),0) from  HomeDeliveryMilk  WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate)+
                                                                  (Select ISNULL(SUM(BuffaloRate*Buffalo),0)TotalAmount from  HomeDeliveryMilk WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate) As TotalAmount from HomeDeliveryMilk WHERE  CustomerID = @CustomerID AND MilkDeliveryDate BETWEEN @fromDate AND @toDate ");
                _db.AddInParameter(commandSales, "@CustomerID", DbType.Int32, CustomerID);
                _db.AddInParameter(commandSales, "@fromDate", DbType.DateTime, firstDate);
                _db.AddInParameter(commandSales, "@toDate", DbType.DateTime, lastDate);

                using (IDataReader reader = _db.ExecuteReader(commandSales))
                {
                    while (reader.Read())
                    {
                        Amount = Convert.ToDecimal(reader["totalAmount"]);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Amount;
        }

        /// <summary>
        /// Author  : Jitendra
        /// Prupose : Get the List of All the Processed Members
        /// </summary>
        /// <param name="BillDate"></param>
        /// <returns></returns>
        public BindingList<SaleProcessingInfo> GetAllLSalesProcessingList(int searchType, string customerName, DateTime billDate, Int32 customerNo, Int32 LineID)
        {
            BindingList<SaleProcessingInfo> retVals = new BindingList<SaleProcessingInfo>();            
            try
            {
                string commandText = "SELECT SaleProcessing.ID, Customers.ID CustomerID, Customers.CustomerNumber,Customers.LineID, Customers.Name , SaleProcessing.OpeningBalance, SaleProcessing.Amount, SaleProcessing.PaidAmount, SaleProcessing.ClosingBalance FROM SaleProcessing JOIN Customers ON SaleProcessing.CustomerID = Customers.ID WHERE Month = @Month AND Year = @Year ";

                if (searchType == 0)
                {
                    commandText += " AND Customers.Name LIKE '%" + customerName.Trim() + "%' ORDER BY Customers.Name";                                       
                }
                else if(searchType == 1)
                {
                    commandText += (customerNo == 0) ? " ORDER BY Customers.ID " : " AND Customers.CustomerNumber = @customerNo ORDER BY Customers.CustomerNumber ";
                }
               else if (searchType == 2)
                {
                   // commandText += "AND LineID=@LineID AND Customers.CustomerNumber = @customerNo ORDER BY Customers.ID ";
                    commandText += (customerNo == 0) ? " AND LineID=@LineID  ORDER BY Customers.CustomerNumber " : " AND Customers.CustomerNumber = @customerNo ORDER BY Customers.CustomerNumber ";
                }

                DbCommand commandSaleItems = _db.GetSqlStringCommand(commandText);
                _db.AddInParameter(commandSaleItems, "@Month", DbType.Int32, billDate.Month);
                _db.AddInParameter(commandSaleItems, "@Year", DbType.Int32, billDate.Year);
                _db.AddInParameter(commandSaleItems, "@customerNo", DbType.Int32, customerNo);
                _db.AddInParameter(commandSaleItems, "@LineID", DbType.Int32, LineID);
                
                using (IDataReader reader = _db.ExecuteReader(commandSaleItems))
                {
                    while (reader.Read())
                    {
                        SaleProcessingInfo saleprocessing = new SaleProcessingInfo();
                        saleprocessing.ID = Convert.ToInt32(reader["ID"]);
                        saleprocessing.CustomerNumber = Convert.ToInt32(reader["CustomerNumber"]);
                        saleprocessing.LineNumber = Convert.ToInt32(reader["LineID"]);
                        
                        saleprocessing.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                        saleprocessing.CustomerName = Convert.ToString(reader["Name"]);
                        saleprocessing.Amount = Convert.ToDecimal(reader["Amount"]);
                        saleprocessing.OpeningBalance = Convert.ToDecimal(reader["OpeningBalance"]);
                        saleprocessing.PaidAmount = Convert.ToDecimal(reader["PaidAmount"]);
                        saleprocessing.ClosingBalance = Convert.ToDecimal(reader["ClosingBalance"]);                        

                        retVals.Add(saleprocessing);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return retVals;          

        }

        /// <summary>
        /// Author  : Jitendra
        /// Purpose : To check Whether PaidAmount Is Present When All Members Are Selected To Process
        /// </summary>
        /// <param name="BillDate"></param>
        /// <returns></returns>
        public bool CheckIfPaidAmountPresent(DateTime BillDate, Boolean IsPaidAmount, Int32 CustomerID)
        {
            decimal PaidAmount = 0;
            bool result = false;
            try
            {
                if (!IsPaidAmount)
                {
                    string CommandText = "SELECT SaleProcessing.PaidAmount From SaleProcessing WHERE Month=@Month AND Year = @Year";
                    if (CustomerID > 0)
                        CommandText += " AND CustomerID  = @CustomerID";
                    DbCommand commandSaleItems = _db.GetSqlStringCommand(CommandText);

                    _db.AddInParameter(commandSaleItems, "@Month", DbType.Int32, BillDate.Month);
                    _db.AddInParameter(commandSaleItems, "@Year", DbType.Int32, BillDate.Year);
                    _db.AddInParameter(commandSaleItems, "@CustomerID", DbType.Int32, CustomerID);
                    using (IDataReader reader = _db.ExecuteReader(commandSaleItems))
                    {
                        while (reader.Read())
                        {
                            PaidAmount = Convert.ToInt32(reader["PaidAmount"]);

                            if (PaidAmount > 0)
                            {
                                result = true;
                                return result;
                            }
                            else
                                result = false;
                        }
                    }
                }
                else
                {
                    result = !IsPaidAmount;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public decimal GetClosingBalanceofCustomer(int ID)
        {
            var month = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);
            var Date = month.AddMonths(-1);

            DbCommand command = _db.GetSqlStringCommand("SELECT ClosingBalance FROM SaleProcessing WHERE CustomerID = @CustomerID AND Month=@month AND Year=@Year");
            _db.AddInParameter(command, "@CustomerID", DbType.Int32, ID);
            _db.AddInParameter(command, "@month", DbType.Int32, Date.Month );
            _db.AddInParameter(command, "@year", DbType.Int32, Date.Year);

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                return Convert.ToDecimal(_db.ExecuteScalar(command));
            }
        }

        
    }
}
