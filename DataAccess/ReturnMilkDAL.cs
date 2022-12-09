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
    public class ReturnMilkDAL
    {
        SqlDatabase _db = DBConn.CreateDB();

        public ReturnMilkDAL()
        {

        }


        public BindingList<MilkIssueInfo> GetLinemanByFilter(string LinemanName, int count)
        {
            BindingList<MilkIssueInfo> retval = new BindingList<MilkIssueInfo>();
            DbCommand command = _db.GetSqlStringCommand(@"Select MilkIssueDate ,Milkissue.LinemanID,Linemans.Name LinemanName,Lines.LineNumber LineNumber,Lines.Name LineName,count (MilkIssueItems.ItemId)TotalItems from  Milkissue  
                                                            join MilkIssueItems on MilkIssueItems.MilkIssueID=Milkissue.ID
                                                            Join Linemans on Linemans.ID=Milkissue.LinemanID 
                                                            join lines on Lines.ID =Linemans.LineID Group by Milkissue.LinemanID,MilkIssueDate,Linemans.Name,Lines.LineNumber,Lines.Name");
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    MilkIssueInfo MilkIssue = new MilkIssueInfo();
                    MilkIssue.MilkIssueDate = Convert.ToDateTime(dataReader["MilkIssueDate"]);
                    MilkIssue.ID = Convert.ToInt32(dataReader["LinemanID"]);
                    MilkIssue.Name = Convert.ToString(dataReader["LinemanName"]);
                    MilkIssue.LineID = Convert.ToString(dataReader["LineNumber"]);
                    MilkIssue.LineName = Convert.ToString(dataReader["LineName"]);
                    MilkIssue.TotalItems = Convert.ToInt32(dataReader["TotalItems"]);
                    retval.Add(MilkIssue);
                }
            }
            return retval;
        }

        public BindingList<MilkIssueItemInfo> ShowItemsforMilkReturnEntry(MilkIssueInfo milkIssuelinemanInfo)
        {
            BindingList<MilkIssueItemInfo> retval = new BindingList<MilkIssueItemInfo>();

            DbCommand command = _db.GetSqlStringCommand(@"SELECT MilkIssue.LinemanID, Items.ItemCode,Items.Name, Items.ItemGroupID, ItemGroups.Name ItemGroup, UnitID,MilkIssueItems.Quantity, Units.Name Unit, Items.Gst, Items.Rate  FROM MilkIssue
                                                        INNER JOIN MilkIssueItems on MilkIssueItems.MilkIssueID=MilkIssue.ID
                                                        INNER JOIN Items on MilkIssueItems.itemID= Items.ID
                                                        INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID 
                                                        INNER JOIN Units ON Items.UnitID = Units.ID 
                                                        Where Milkissue.MilkIssueDate=@ReturnDate AND  LinemanID=@LinemanID");
                _db.AddInParameter(command, "@ReturnDate", DbType.DateTime, milkIssuelinemanInfo.MilkIssueDate);
                _db.AddInParameter(command, "@LinemanID", DbType.Int32, milkIssuelinemanInfo.ID);
            using (IDataReader dataReader = _db.ExecuteReader(command))
            {
                while (dataReader.Read())
                {
                    MilkIssueItemInfo MilkIssueItems = new MilkIssueItemInfo();
                    MilkIssueItems.LineManID = Convert.ToInt32(dataReader["LinemanID"]);
                    //MilkIssueItems.ID = Convert.ToInt32(dataReader["ID"]);
                    MilkIssueItems.ItemCode = Convert.ToInt32(dataReader["ItemCode"]);
                    MilkIssueItems.ItemGroupID = Convert.ToInt32(dataReader["ItemGroupID"]);
                    MilkIssueItems.Name = Convert.ToString(dataReader["Name"]);
                    MilkIssueItems.ItemGroup = Convert.ToString(dataReader["ItemGroup"]);
                    MilkIssueItems.UnitID = Convert.ToInt32(dataReader["UnitID"]);
                    MilkIssueItems.Unit = Convert.ToString(dataReader["Unit"]);
                    MilkIssueItems.Quantity = Convert.ToDecimal(dataReader["Quantity"]);
                    MilkIssueItems.Gst = Convert.ToDecimal(dataReader["Gst"]);
                    MilkIssueItems.Rate = Convert.ToDecimal(dataReader["Rate"]);
                    retval.Add(MilkIssueItems);

                }
            }
            return retval;
        }


        public int ReturnMilkByLineman(LinemanInfo MilkIssue, BindingList<MilkIssueItemInfo> MilkIssueItems)
        {
            int retval = 0;
            DbTransaction transaction = null;
            try
            {
                DbCommand command = _db.GetSqlStringCommand(@"INSERT INTO ReturnMilk ([ReturnDate],[LinemanID],[CreatedBy],[CreatedOn],[UpdatedBy],[UpdatedOn])
                                                          VALUES(@ReturnDate,@LinemanID,@CreatedBy,@CreatedOn,@UpdatedBy,@UpdatedOn) SELECT IDENT_CURRENT('ReturnMilk')");
                _db.AddInParameter(command, "@ReturnDate", DbType.DateTime, MilkIssue.FromDate);
               
                _db.AddInParameter(command, "@LinemanID", DbType.Int32, MilkIssue.ID);
                _db.AddInParameter(command, "@CreatedBy", DbType.Byte, MilkIssue.CreatedBy);
                _db.AddInParameter(command, "@CreatedOn", DbType.DateTime, MilkIssue.CreatedOn);
                _db.AddInParameter(command, "@UpdatedBy", DbType.Byte, MilkIssue.UpdatedBy);
                _db.AddInParameter(command, "@UpdatedOn", DbType.DateTime, MilkIssue.UpdatedOn);

                using (DbConnection conn = _db.CreateConnection())
                {

                    conn.Open();
                    transaction = conn.BeginTransaction();
                    retval = Convert.ToInt32(_db.ExecuteScalar(command, transaction));

                    foreach (MilkIssueItemInfo item in MilkIssueItems)
                    {
                        if (item.ReturnQuantity > 0)
                        {
                            DbCommand commandMilkIssueItems = _db.GetSqlStringCommand("INSERT INTO ReturnMilkItems ([ReturnMilkID], [ItemID], [Quantity],[ReturnQuantity]) VALUES (@ReturnMilkID, @ItemID, @Quantity,@ReturnQuantity)");
                            _db.AddInParameter(commandMilkIssueItems, "@ReturnMilkID", DbType.Int32, retval);
                            _db.AddInParameter(commandMilkIssueItems, "@ItemID", DbType.Int32, item.ItemCode);
                            _db.AddInParameter(commandMilkIssueItems, "@Quantity", DbType.Decimal, item.Quantity);
                            _db.AddInParameter(commandMilkIssueItems, "@ReturnQuantity", DbType.Decimal, item.ReturnQuantity);

                            _db.ExecuteNonQuery(commandMilkIssueItems, transaction);
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

        public void DeleteMilkReturn(int MilkIssueID)
        {

            DbCommand command = _db.GetSqlStringCommand("DELETE FROM ReturnMilk WHERE LinemanID = @ID");
            _db.AddInParameter(command, "ID", DbType.Int32, MilkIssueID);
            using (DbConnection con = _db.CreateConnection())
            {
                con.Open();
                _db.ExecuteNonQuery(command);
            }
        }
    }
}
