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
   public class MonthlyMilkStanderdDAL
    {
       SqlDatabase _db = DBConn.CreateDB();

       public MonthlyMilkStanderdDAL()
        {

        }
       public BindingList<MonthlyMilkStanderdInfo> ShowCustomerForHomedeliveryMilkEntry(int LineID)
       {

           BindingList<MonthlyMilkStanderdInfo> retVal = new BindingList<MonthlyMilkStanderdInfo>();
           //DbCommand command = _db.GetSqlStringCommand("SELECT Customers.ID,[CustomerNumber],Customers.Name,Buffalo,Cow FROM Customers join lines on lines.ID=Customers.LineID join MonthlyStanderdMilk on MonthlyStanderdMilk.CustomerID = Customers.ID  WHERE datepart(MM,MonthlyStanderdMilk.StanderdDate)=month(getdate()) and   LineID = @Id ");
           //DbCommand command = _db.GetSqlStringCommand(@"SELECT Customers.ID,[CustomerNumber],Customers.Name FROM Customers join lines on lines.ID=Customers.LineID  WHERE   LineID=@LineID ");
           //DbCommand command = _db.GetSqlStringCommand("SELECT Customers.ID,[CustomerNumber],Customers.Name,IsNULL(Buffalo,0)Buffalo ,IsNull(Cow,0)Cow FROM Customers join lines on lines.ID=Customers.LineID join MonthlyStanderdMilk on MonthlyStanderdMilk.CustomerID = Customers.ID  WHERE MonthlyStanderdMilk.StanderdDate=(SELECT MAX(MonthlyStanderdMilk.StanderdDate) FROM MonthlyStanderdMilk  WHERE  MonthlyStanderdMilk.LineID = @LineID ) AND  MonthlyStanderdMilk.LineID = @LineID ");
           DbCommand command = _db.GetSqlStringCommand(@" Select * from (SELECT Customers.ID,[CustomerNumber],Customers.Name,IsNULL(Buffalo,0)Buffalo ,IsNull(Cow,0)Cow FROM Customers join lines on lines.ID=Customers.LineID join MonthlyStanderdMilk on MonthlyStanderdMilk.CustomerID = Customers.ID  WHERE MonthlyStanderdMilk.StanderdDate=(SELECT MAX(MonthlyStanderdMilk.StanderdDate) FROM MonthlyStanderdMilk  WHERE  MonthlyStanderdMilk.LineID = @LineID ) AND  Customers.LineID = @LineID  
            union
            SELECT Customers.ID,[CustomerNumber],Customers.Name,0 as Buffalo ,0 as Cow FROM Customers Where ID NOT IN (SELECT Customers.ID FROM Customers join lines on lines.ID=Customers.LineID join MonthlyStanderdMilk on MonthlyStanderdMilk.CustomerID = Customers.ID  WHERE MonthlyStanderdMilk.StanderdDate=(SELECT MAX(MonthlyStanderdMilk.StanderdDate) FROM MonthlyStanderdMilk  WHERE  MonthlyStanderdMilk.LineID = @LineID ) AND  MonthlyStanderdMilk.LineID = @LineID ) and LineID=@LineID )MonthlyStanderdMilk  order by  MonthlyStanderdMilk.CustomerNumber ");
           _db.AddInParameter(command, "@LineID", DbType.Int32, LineID);

           using (IDataReader dataReader = _db.ExecuteReader(command))
           {
               while (dataReader.Read())
               {
                   MonthlyMilkStanderdInfo Customres = new MonthlyMilkStanderdInfo();
                   Customres.ID = Convert.ToInt32(dataReader["ID"]);
                   Customres.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                   Customres.Name = Convert.ToString(dataReader["Name"]);
                   Customres.Buffalo = Convert.ToDecimal(dataReader["Buffalo"]);
                  Customres.Cow = Convert.ToDecimal(dataReader["Cow"]);
                   retVal.Add(Customres);
               }
           }
           if (retVal.Count==0)
           {
               DbCommand commandText = _db.GetSqlStringCommand(@"SELECT Customers.ID,[CustomerNumber],Customers.Name FROM Customers join lines on lines.ID=Customers.LineID  WHERE   LineID=@LineID ");
               _db.AddInParameter(commandText, "@LineID", DbType.Int32, LineID);
               using (IDataReader dataReader = _db.ExecuteReader(commandText))
               {
                   while (dataReader.Read())
                   {
                       MonthlyMilkStanderdInfo Customres = new MonthlyMilkStanderdInfo();
                       Customres.ID = Convert.ToInt32(dataReader["ID"]);
                       Customres.CustomerNumber = Convert.ToInt32(dataReader["CustomerNumber"]);
                       Customres.Name = Convert.ToString(dataReader["Name"]);
                       //Customres.Buffalo = Convert.ToDecimal(dataReader["Buffalo"]);
                       // Customres.Cow = Convert.ToDecimal(dataReader["Cow"]);
                       retVal.Add(Customres);
                   }
               }
           }
           return retVal;
       }

       public int HomedeliveryMilkByLineman(MonthlyMilkStanderdInfo HomeDeliveryMilkIssue, BindingList<MonthlyMilkStanderdInfo> HomeDeliveryMilkInfo)
       {
           int retval = 0;
           DbTransaction transaction = null;
           try
             {
               using (DbConnection conn = _db.CreateConnection())
               {
                   conn.Open();
                   transaction = conn.BeginTransaction();

                   foreach (MonthlyMilkStanderdInfo item in HomeDeliveryMilkInfo)
                   {
                       if (item.Buffalo >= 0 || item.Cow >= 0)
                       {
                           DbCommand MonthlyMilkStanderdMilk = _db.GetSqlStringCommand("INSERT INTO [MonthlyStanderdMilk] ( [StanderdDate], [CustomerID],[Buffalo],[Cow],[LineID],[CreatedOn] ,[CreatedBy],[UpdatedOn] ,[UpdatedBy]) VALUES (@StanderdDate, @CustomerID,@Buffalo,@Cow,@LineID,@CreatedOn,@CreatedBy,@UpdatedOn,@UpdatedBy)");
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@StanderdDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@CustomerID", DbType.Int32, item.ID);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@LineID", DbType.Int32, item.LineID);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@Buffalo", DbType.Decimal, item.Buffalo);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@Cow", DbType.Decimal, item.Cow);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@CreatedOn", DbType.DateTime, HomeDeliveryMilkIssue.CreatedOn);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@CreatedBy", DbType.Decimal, HomeDeliveryMilkIssue.CreatedBy);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@UpdatedOn", DbType.DateTime, HomeDeliveryMilkIssue.UpdatedOn);
                           _db.AddInParameter(MonthlyMilkStanderdMilk, "@UpdatedBy", DbType.Decimal, HomeDeliveryMilkIssue.UpdatedBy);

                           _db.ExecuteNonQuery(MonthlyMilkStanderdMilk, transaction);
                       }
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

       public BindingList<MonthlyMilkStanderdInfo> SearchMilkDeliveryEnteries(string LinemanName, int count)
       {
           BindingList<MonthlyMilkStanderdInfo> retval = new BindingList<MonthlyMilkStanderdInfo>();
           DbCommand command = _db.GetSqlStringCommand("Select StanderdDate,CustomerID,Customers.Name,Buffalo,Cow from dbo.MonthlyStanderdMilk join Customers on CustomerID=Customers.ID");
           using (IDataReader dataReader = _db.ExecuteReader(command))
           {
               while (dataReader.Read())
               {
                   MonthlyMilkStanderdInfo homedeliverymilkinfo = new MonthlyMilkStanderdInfo();
                   homedeliverymilkinfo.MilkDeliveryDate = Convert.ToDateTime(dataReader["StanderdDate"]);
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
                   command = _db.GetSqlStringCommand("DELETE FROM MonthlyStanderdMilk WHERE CustomerID = @CustomerID");
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

       public int CheckTodaysMilkEntryExist(DateTime StanderdDate, int LineID)
       {
           int retVal = 0;
           DbCommand command = _db.GetSqlStringCommand("SELECT COUNT(CustomerID) FROM MonthlyStanderdMilk WHERE MonthlyStanderdMilk.StanderdDate = @StanderdDate AND MonthlyStanderdMilk.LineID = @LineID");
           _db.AddInParameter(command, "@StanderdDate", DbType.DateTime, StanderdDate.Date);
           _db.AddInParameter(command, "@LineID", DbType.Int32, LineID);

           using (DbConnection conn = _db.CreateConnection())
           {
               conn.Open();
               retVal = Convert.ToInt32(_db.ExecuteScalar(command));
           }
           return retVal;
       }

       public int UpdateMonthlyStandered(MonthlyMilkStanderdInfo HomeDeliveryMilkIssue, BindingList<MonthlyMilkStanderdInfo> HomeDeliveryMilkInfo)
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

                   DbCommand DeleteMonthlyStanderdMilk = _db.GetSqlStringCommand("Delete From MonthlyStanderdMilk where  LineID=@LineID And StanderdDate =@StanderdDate");

                   _db.AddInParameter(DeleteMonthlyStanderdMilk, "@StanderdDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate);
                   _db.AddInParameter(DeleteMonthlyStanderdMilk, "@LineID", DbType.Decimal, HomeDeliveryMilkIssue.LineID);
                   _db.ExecuteNonQuery(DeleteMonthlyStanderdMilk, transaction);

                   //Insert into

                   foreach (MonthlyMilkStanderdInfo item in HomeDeliveryMilkInfo)
                   {
                       //if (item.Buffalo > 0 || item.Cow > 0)
                       //{
                      // DbCommand HomeDeliveryMilk = _db.GetSqlStringCommand("UPDATE  [MonthlyStanderdMilk] SET  [StanderdDate]=@StanderdDate, [CustomerID]=@CustomerID,[Buffalo]=@Buffalo,[Cow]=@Cow,[LineID]=@LineID,[CreatedOn]=@CreatedOn ,[CreatedBy]=@CreatedBy,[UpdatedOn]= @UpdatedOn,[UpdatedBy]=@UpdatedBy WHERE [StanderdDate]=@StanderdDate AND [CustomerID]=@CustomerID AND [LineID]=@LineID ");
                      // //_db.AddInParameter(HomeDeliveryMilk, "@ID", DbType.Int32, item.ID);
                      // _db.AddInParameter(HomeDeliveryMilk, "@StanderdDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate.Date);
                      // _db.AddInParameter(HomeDeliveryMilk, "@CustomerID", DbType.Decimal, item.ID);
                      // _db.AddInParameter(HomeDeliveryMilk, "@LineID", DbType.Decimal, item.LineID);
                      // _db.AddInParameter(HomeDeliveryMilk, "@Buffalo", DbType.Decimal, item.Buffalo);
                      // _db.AddInParameter(HomeDeliveryMilk, "@Cow", DbType.Decimal, item.Cow);
                      // _db.AddInParameter(HomeDeliveryMilk, "@CreatedOn", DbType.DateTime, HomeDeliveryMilkIssue.CreatedOn);
                      // _db.AddInParameter(HomeDeliveryMilk, "@CreatedBy", DbType.Decimal, HomeDeliveryMilkIssue.CreatedBy);
                      //_db.AddInParameter(HomeDeliveryMilk, "@UpdatedOn", DbType.DateTime, HomeDeliveryMilkIssue.UpdatedOn);
                      //  _db.AddInParameter(HomeDeliveryMilk, "@UpdatedBy", DbType.Decimal, HomeDeliveryMilkIssue.UpdatedBy);
                      // _db.ExecuteNonQuery(HomeDeliveryMilk, transaction);
                       //}
                       DbCommand MonthlyMilkStanderdMilk = _db.GetSqlStringCommand("INSERT INTO [MonthlyStanderdMilk] ( [StanderdDate], [CustomerID],[Buffalo],[Cow],[LineID],[CreatedOn] ,[CreatedBy],[UpdatedOn] ,[UpdatedBy]) VALUES (@StanderdDate, @CustomerID,@Buffalo,@Cow,@LineID,@CreatedOn,@CreatedBy,@UpdatedOn,@UpdatedBy)");
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@StanderdDate", DbType.DateTime, HomeDeliveryMilkIssue.MilkDeliveryDate);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@CustomerID", DbType.Int32, item.ID);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@LineID", DbType.Int32, item.LineID);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@Buffalo", DbType.Decimal, item.Buffalo);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@Cow", DbType.Decimal, item.Cow);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@CreatedOn", DbType.DateTime, HomeDeliveryMilkIssue.CreatedOn);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@CreatedBy", DbType.Decimal, HomeDeliveryMilkIssue.CreatedBy);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@UpdatedOn", DbType.DateTime, HomeDeliveryMilkIssue.UpdatedOn);
                       _db.AddInParameter(MonthlyMilkStanderdMilk, "@UpdatedBy", DbType.Decimal, HomeDeliveryMilkIssue.UpdatedBy);

                       _db.ExecuteNonQuery(MonthlyMilkStanderdMilk, transaction);
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
