using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class StockDAL
    {
        public List<ItemInfo> GetAllItems()
        {
            List<ItemInfo> retVal = new List<ItemInfo>();
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT ID FROM Items");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adp.Fill(ds, "Items");

                    foreach (DataRow dr in ds.Tables["Items"].Rows)
                    {
                        ItemInfo item = new ItemInfo();
                        item.ID = Convert.ToInt32(dr["ID"]);

                        retVal.Add(item);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                return retVal;
            }
        }

        public StockInfo Process(DateTime stockDate, int itemID, int divisionID)
        {
            StockInfo stockRecord = new StockInfo();
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());

            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Stock_Process");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.BigInt));
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@StockDate"].Value = stockDate;
                    cmd.Parameters["@ItemID"].Value = itemID;
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    SqlDataReader objRead;
                    objRead = cmd.ExecuteReader();
                    if (objRead.HasRows == true)
                    {
                        objRead.Read();
                        stockRecord.StockDate = stockDate;
                        stockRecord.ItemID = itemID;
                        stockRecord.Opening = Convert.ToDecimal(objRead["ClBal"]);
                        stockRecord.Challan = Convert.ToDecimal(objRead["Challan"]);
                        stockRecord.IBInward = Convert.ToDecimal(objRead["IBInward"]);
                        stockRecord.IBOutward = Convert.ToDecimal(objRead["IBOutward"]);
                        stockRecord.Sale = Convert.ToDecimal(objRead["Sale"]);
                        stockRecord.DivisionID = divisionID;
                    }
                    else
                    {
                        stockRecord.StockDate = stockDate;
                        stockRecord.ItemID = itemID;
                        stockRecord.Closing = 0;
                        stockRecord.Challan = 0;
                        stockRecord.Sale = 0;
                        stockRecord.DivisionID = divisionID;
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return stockRecord;
        }

        public void StockInsert(StockInfo stock)
        {
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Stock(StockDate,ItemID,Opening,Challan,IBInward,IBOutward,Sale,Adjusted,DivisionID) VALUES(@StockDate,@ItemID,@OpBal,@Challan,@IBInward,@IBOutward,@WORelease,'False',@DivisionID) " +
                    "UPDATE Stock SET	Closing=@OpBal+@Challan-@WORelease-@IBInward-@IBOutward WHERE StockDate=@StockDate AND ItemID=@ItemID AND DivisionID=@DivisionID ");
                    
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.BigInt));
                    cmd.Parameters.Add(new SqlParameter("@OpBal", SqlDbType.Decimal));
                    cmd.Parameters.Add(new SqlParameter("@Challan", SqlDbType.Decimal));
                    cmd.Parameters.Add(new SqlParameter("@IBInward", SqlDbType.Decimal));
                    cmd.Parameters.Add(new SqlParameter("@IBOutward", SqlDbType.Decimal));
                    cmd.Parameters.Add(new SqlParameter("@WORelease", SqlDbType.Decimal));
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));


                    cmd.Parameters["@StockDate"].Value = stock.StockDate;
                    cmd.Parameters["@ItemID"].Value = stock.ItemID;
                    cmd.Parameters["@OpBal"].Value = stock.Opening;
                    cmd.Parameters["@Challan"].Value = stock.Challan;
                    cmd.Parameters["@IBInward"].Value = stock.IBInward;
                    cmd.Parameters["@IBOutward"].Value = stock.IBOutward;
                    cmd.Parameters["@WORelease"].Value = stock.Sale;
                    cmd.Parameters["@DivisionID"].Value = stock.DivisionID;

                    cmd.ExecuteScalar();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public void StockDelete(DateTime stockDate, int divisionID)
        {
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Stock WHERE StockDate = @StockDate AND DivisionID = @DivisionID DELETE FROM StockAdjustments WHERE AdjustmentDate = @StockDate");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters["@StockDate"].Value = stockDate;


                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    cmd.ExecuteScalar();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public List<StockInfo> GetByDate(DateTime stockDate, string itemName, int divisionID)
        {
            List<StockInfo> retVal = new List<StockInfo>();

            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = null;
                    if (itemName == string.Empty)
                        cmd = new SqlCommand("SELECT Stock.ID, Stock.StockDate, Stock.ItemID, Items.Name, Stock.Opening, Stock.Challan, Stock.IBInward, Stock.IBOutward, Stock.Sale, Stock.Closing, Stock.Adjusted , DivisionID  FROM Stock INNER JOIN Items ON Stock.ItemID=Items.ID WHERE Stock.StockDate = @StockDate  AND DivisionID = @DivisionID");
                    else
                        cmd = new SqlCommand("SELECT Stock.ID, Stock.StockDate, Stock.ItemID, Items.Name, Stock.Opening, Stock.Challan, Stock.IBInward, Stock.IBOutward, Stock.Sale, Stock.Closing, Stock.Adjusted , DivisionID  FROM Stock INNER JOIN Items ON Stock.ItemID=Items.ID WHERE Stock.StockDate = @StockDate AND Items.Name LIKE '" + itemName + "%'  AND DivisionID = @DivisionID");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters["@StockDate"].Value = stockDate;

                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adp.Fill(ds, "Stock");
                    foreach (DataRow dr in ds.Tables["Stock"].Rows)
                    {
                        StockInfo stock = new StockInfo();
                        stock.ID = Convert.ToInt32(dr["ID"]);
                        stock.StockDate = Convert.ToDateTime(dr["StockDate"]);
                        stock.ItemID = Convert.ToInt32(dr["ItemID"]);
                        stock.ItemName = Convert.ToString(dr["Name"]);
                        stock.Opening = Convert.ToDecimal(dr["Opening"]);
                        stock.Challan = Convert.ToDecimal(dr["Challan"]);
                        stock.IBInward = Convert.ToDecimal(dr["IBInward"]);
                        stock.IBOutward = Convert.ToDecimal(dr["IBOutward"]);
                        stock.Sale = Convert.ToDecimal(dr["Sale"]);
                        stock.Closing = Convert.ToDecimal(dr["Closing"]);
                        stock.Adjusted = Convert.ToBoolean(dr["Adjusted"]);
                        stock.DivisionID = Convert.ToInt32(dr["DivisionID"]);

                        retVal.Add(stock);
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                return retVal;
            }
        }

        public int GetPreviousDay(DateTime stockDate, int divisionID)
        {
            int retVal = 0;
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT	IsNull((SELECT COUNT(*) FROM Stock 	WHERE StockDate=@StockDate-1 AND DivisionID = @DivisionID),0)");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters["@StockDate"].Value = stockDate;

                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    retVal = Convert.ToInt32(cmd.ExecuteScalar());

                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return retVal;
        }

        public DateTime GetMaxDate(int divisionID)
        {
            DateTime retVal = DateTime.Today;
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(StockDate), 1) FROM Stock WHERE DivisionID = @DivisionID ");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    retVal = Convert.ToDateTime(cmd.ExecuteScalar());

                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return retVal;
        }

        public int IsAdjusted(DateTime stockDate, int divisionID)
        {
            int retVal = 0;
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Stock WHERE Adjusted='True' AND StockDate = @StockDate AND DivisionID = @DivisionID");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters["@StockDate"].Value = stockDate;

                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    retVal = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return retVal;
        }

        public bool CheckDayExist(DateTime stockDate, int divisionID)
        {
            SqlConnection conn = new SqlConnection(DBConn.GetConnectionString());
            using (conn)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(StockDate) FROM Stock WHERE StockDate = @StockDate AND DivisionID = @DivisionID ");
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@StockDate", SqlDbType.DateTime));
                    cmd.Parameters["@StockDate"].Value = stockDate;
                    cmd.Parameters.Add(new SqlParameter("@DivisionID", SqlDbType.Int));
                    cmd.Parameters["@DivisionID"].Value = divisionID;

                    int retVal = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    conn.Close();

                    if (retVal > 0)
                        return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return false;
        }
    }
}