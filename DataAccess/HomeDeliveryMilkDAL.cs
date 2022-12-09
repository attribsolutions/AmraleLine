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
   public class HomeDeliveryMilkDAL
    {
       SqlDatabase _db = DBConn.CreateDB();

       public HomeDeliveryMilkDAL()
        {

        }
       public BindingList<HomeDeliveryMilkInfo> ShowCustomerForHomedeliveryMilkEntry(int LineID, DateTime MilkDeliveryDate, bool DataExist)
       {
           BindingList<HomeDeliveryMilkInfo> retVal = new BindingList<HomeDeliveryMilkInfo>();
           if (DataExist)
           {
               // DbCommand command = _db.GetSqlStringCommand("SELECT Customers.ID,[CustomerNumber],Customers.Name,Buffalo,Cow FROM Customers join lines on lines.ID=Customers.LineID join MonthlyStanderdMilk on MonthlyStanderdMilk.CustomerID = Customers.ID  WHERE datepart(MM,MonthlyStanderdMilk.StanderdDate)=month(getdate()) and   LineID = @Id");
               // DbCommand command = _db.GetSqlStringCommand("SELECT Customers.ID,[CustomerNumber],Customers.Name,IsNULL(Buffalo,0)Buffalo ,IsNull(Cow,0)Cow FROM HomeDeliveryMilk  join Customers on Customers.ID =  HomeDeliveryMilk.CustomerID  WHERE HomeDeliveryMilk.MilkDeliveryDate=@MilkDeliveryDate And HomeDeliveryMilk.LineID = @LineID");
               DbCommand command = _db.GetSqlStringCommand(@" Select * from ( SELECT Customers.ID,[CustomerNumber],Customers.Name,IsNULL(Buffalo,0)Buffalo ,IsNull(Cow,0)Cow FROM HomeDeliveryMilk  join Customers on Customers.ID =  HomeDeliveryMilk.CustomerID  WHERE HomeDeliveryMilk.MilkDeliveryDate=@MilkDeliveryDate And HomeDeliveryMilk.LineID = @LineID AND isActive='True'
            Union 
            SELECT Customers.ID,[CustomerNumber],Customers.Name,0 as Buffalo ,0 as Cow FROM Customers Where ID NOT IN (SELECT Customers.ID FROM Customers join lines on lines.ID=Customers.LineID join HomeDeliveryMilk on HomeDeliveryMilk.CustomerID = Customers.ID WHERE HomeDeliveryMilk.MilkDeliveryDate=@MilkDeliveryDate And HomeDeliveryMilk.LineID = @LineID  AND isActive='True' )and LineID=@LineID AND isActive='True')HomeDeliveryMilk    order by  HomeDeliveryMilk.CustomerNumber");
               _db.AddInParameter(command, "@LineID", DbType.Int32, LineID);
               _db.AddInParameter(command, "@MilkDeliveryDate", DbType.Date, MilkDeliveryDate);

               using (IDataReader dataReader = _db.ExecuteReader(command))
               {
                   while (dataReader.Read())
                   {
                       HomeDeliveryMilkInfo Customres = new HomeDeliveryMilkInfo();
                       Customres.ID = Convert.ToInt32(dataReader["ID"]);
                       Customres.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                       Customres.Name = Convert.ToString(dataReader["Name"]);
                       Customres.Buffalo = Convert.ToDecimal(dataReader["Buffalo"]);
                       Customres.Cow = Convert.ToDecimal(dataReader["Cow"]);
                       retVal.Add(Customres);
                   }
               }
           }
           else
           {
               DbCommand commandText = _db.GetSqlStringCommand("SELECT Customers.ID,[CustomerNumber],Customers.Name,IsNULL(Buffalo,0)Buffalo ,IsNull(Cow,0)Cow FROM Customers join lines on lines.ID=Customers.LineID join MonthlyStanderdMilk on MonthlyStanderdMilk.CustomerID = Customers.ID  WHERE MonthlyStanderdMilk.StanderdDate=(SELECT MAX(MonthlyStanderdMilk.StanderdDate) FROM MonthlyStanderdMilk  WHERE  MonthlyStanderdMilk.LineID = @LineID ) AND  Customers.LineID = @LineID  AND isActive='True' Order By CustomerNumber");
                   _db.AddInParameter(commandText, "@LineID", DbType.Int32, LineID);

                   using (IDataReader dataReader = _db.ExecuteReader(commandText))
                   {
                       while (dataReader.Read())
                       {
                           HomeDeliveryMilkInfo Customres = new HomeDeliveryMilkInfo();
                           Customres.ID = Convert.ToInt32(dataReader["ID"]);
                           Customres.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                           Customres.Name = Convert.ToString(dataReader["Name"]);
                           Customres.Buffalo = Convert.ToDecimal(dataReader["Buffalo"]);
                           Customres.Cow = Convert.ToDecimal(dataReader["Cow"]);
                           retVal.Add(Customres);
                       }
                   }

               
           }
           return retVal;
       }

       public int HomedeliveryMilkByLineman(HomeDeliveryMilkInfo HomeDeliveryMilkIssue, BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo)
       {
          
           int retval = 0;
           DbTransaction transaction = null;
           try
             {

               using (DbConnection conn = _db.CreateConnection())
               {

                   conn.Open();
                   transaction = conn.BeginTransaction();
                  
                   foreach (HomeDeliveryMilkInfo item in HomeDeliveryMilkInfo)
                   {
                       //if (item.Buffalo > 0 || item.Cow > 0)
                       //{
                       DbCommand HomeDeliveryMilk = _db.GetSqlStringCommand("INSERT INTO [HomeDeliveryMilk] ( [MilkDeliveryDate], [CustomerID],[LineID],[Buffalo],[BuffaloRate],[Cow],[CowRate],[CreatedOn] ,[CreatedBy]) VALUES (@MilkDeliveryDate, @CustomerID,@LineID,@Buffalo,@BuffaloRate,@Cow,@CowRate,@CreatedOn,@CreatedBy)");
                           
                           _db.AddInParameter(HomeDeliveryMilk, "@MilkDeliveryDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate);
                           _db.AddInParameter(HomeDeliveryMilk, "@CustomerID", DbType.Decimal, item.ID);
                           _db.AddInParameter(HomeDeliveryMilk, "@LineID", DbType.Decimal, item.LineID);
                           _db.AddInParameter(HomeDeliveryMilk, "@Buffalo", DbType.Decimal, item.Buffalo);
                           _db.AddInParameter(HomeDeliveryMilk, "@BuffaloRate", DbType.Decimal, item.BuffaloRate);
                           _db.AddInParameter(HomeDeliveryMilk, "@Cow", DbType.Decimal, item.Cow);
                           _db.AddInParameter(HomeDeliveryMilk, "@CowRate", DbType.Decimal, item.CowRate);
                           _db.AddInParameter(HomeDeliveryMilk, "@CreatedOn", DbType.DateTime, HomeDeliveryMilkIssue.CreatedOn);
                           _db.AddInParameter(HomeDeliveryMilk, "@CreatedBy", DbType.Decimal, HomeDeliveryMilkIssue.CreatedBy);
                           
                           _db.ExecuteNonQuery(HomeDeliveryMilk,transaction);
                       //}
                   }
                   transaction.Commit();
               }
           }
           catch (Exception ex)
           {
               transaction.Rollback();
               throw new ApplicationException(ex.Message);
           }

           return retval;
       }



       public BindingList<HomeDeliveryMilkInfo> SearchMilkDeliveryEnteries(string LinemanName, int count)
       {
           BindingList<HomeDeliveryMilkInfo> retval = new BindingList<HomeDeliveryMilkInfo>();
           DbCommand command = _db.GetSqlStringCommand("Select MilkDeliveryDate,CustomerID,Customers.Name,Buffalo,Cow from dbo.HomeDeliveryMilk join Customers on CustomerID=Customers.ID");
           using (IDataReader dataReader = _db.ExecuteReader(command))
           {
               while (dataReader.Read())
               {
                   HomeDeliveryMilkInfo homedeliverymilkinfo = new HomeDeliveryMilkInfo();
                   homedeliverymilkinfo.MilkDeliveryDate = Convert.ToDateTime(dataReader["MilkDeliveryDate"]);
                   homedeliverymilkinfo.ID = Convert.ToInt32(dataReader["CustomerID"]);
                   homedeliverymilkinfo.Name = Convert.ToString(dataReader["Name"]);
                   homedeliverymilkinfo.Buffalo = Convert.ToInt32(dataReader["Buffalo"]);
                   homedeliverymilkinfo.Cow = Convert.ToInt32(dataReader["Cow"]);
                  
                   retval.Add(homedeliverymilkinfo);
               }
           }
           return retval;
       }

       public void DeleteDelivery(int CustomerID)
       {
           DbCommand command = null;
           using (DbConnection conn = _db.CreateConnection())
           {
               DbTransaction transaction = null;
               try
               {
                   conn.Open();
                   transaction = conn.BeginTransaction();
                   command = _db.GetSqlStringCommand("DELETE FROM HomeDeliveryMilk WHERE CustomerID = @CustomerID");
                   _db.AddInParameter(command, "CustomerID", DbType.Int32, CustomerID);
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

       public int CheckTodaysMilkEntryExist(DateTime HomeDeliveryMilkDate, int LineID)
       {
           int retVal = 0;
           DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(CustomerID) FROM HomeDeliveryMilk WHERE HomeDeliveryMilk.MilkDeliveryDate = @HomeDeliveryMilkDate AND HomeDeliveryMilk.LineID = @LineID");
           _db.AddInParameter(command, "@HomeDeliveryMilkDate", DbType.DateTime, HomeDeliveryMilkDate.Date);
           _db.AddInParameter(command, "@LineID", DbType.Int32, LineID);

           using (DbConnection conn = _db.CreateConnection())
           {
               conn.Open();
               retVal = Convert.ToInt32(_db.ExecuteScalar(command));
           }
           return retVal;
       }

       public int UpdateHomedeliveryMilkByLineman(HomeDeliveryMilkInfo HomeDeliveryMilkIssue, BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo)
       {
           int retval = 0;
           DbTransaction transaction = null;
           try
           {

               using (DbConnection conn = _db.CreateConnection())
               {

                   conn.Open();
                   transaction = conn.BeginTransaction();

                   //Delete all records for this line & date

                   DbCommand DeleteHomeDeliveryMilk = _db.GetSqlStringCommand("Delete From HomeDeliveryMilk where  LineID=@LineID And MilkDeliveryDate =@MilkDeliveryDate");

                   _db.AddInParameter(DeleteHomeDeliveryMilk, "@MilkDeliveryDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate);
                   _db.AddInParameter(DeleteHomeDeliveryMilk, "@LineID", DbType.Decimal, HomeDeliveryMilkIssue.LineID);
                   _db.ExecuteNonQuery(DeleteHomeDeliveryMilk, transaction);

                   //Insert into
                   foreach (HomeDeliveryMilkInfo item in HomeDeliveryMilkInfo)
                   {
                       
                     //  DbCommand HomeDeliveryMilk = _db.GetSqlStringCommand("Update  [HomeDeliveryMilk] SET  MilkDeliveryDate=@MilkDeliveryDate, CustomerID=@CustomerID,LineID=@LineID,Buffalo=@Buffalo,Cow=@Cow,CreatedOn=@CreatedOn ,CreatedBy=@CreatedBy WHERE  MilkDeliveryDate=@MilkDeliveryDate AND CustomerID=@CustomerID AND LineID=@LineID ");
                       DbCommand HomeDeliveryMilk = _db.GetSqlStringCommand("INSERT INTO [HomeDeliveryMilk] ( [MilkDeliveryDate], [CustomerID],[LineID],[Buffalo],[BuffaloRate],[Cow],[CowRate],[CreatedOn] ,[CreatedBy]) VALUES (@MilkDeliveryDate, @CustomerID,@LineID,@Buffalo,@BuffaloRate,@Cow,@CowRate,@CreatedOn,@CreatedBy)");
                       _db.AddInParameter(HomeDeliveryMilk, "@MilkDeliveryDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate);
                       _db.AddInParameter(HomeDeliveryMilk, "@CustomerID", DbType.Decimal, item.ID);
                       _db.AddInParameter(HomeDeliveryMilk, "@LineID", DbType.Decimal, item.LineID);
                       _db.AddInParameter(HomeDeliveryMilk, "@Buffalo", DbType.Decimal, item.Buffalo);
                       _db.AddInParameter(HomeDeliveryMilk, "@BuffaloRate", DbType.Decimal, item.BuffaloRate);
                       _db.AddInParameter(HomeDeliveryMilk, "@Cow", DbType.Decimal, item.Cow);
                       _db.AddInParameter(HomeDeliveryMilk, "@CowRate", DbType.Decimal, item.CowRate);
                       _db.AddInParameter(HomeDeliveryMilk, "@CreatedOn", DbType.DateTime, HomeDeliveryMilkIssue.CreatedOn);
                       _db.AddInParameter(HomeDeliveryMilk, "@CreatedBy", DbType.Decimal, HomeDeliveryMilkIssue.CreatedBy);
                       
                       _db.ExecuteNonQuery(HomeDeliveryMilk, transaction);
                       
                   }
                   transaction.Commit();
               }
           }
           catch (Exception ex)
           {
               transaction.Rollback();
               throw new ApplicationException(ex.Message);
           }

           return retval;
       }
    }
}
