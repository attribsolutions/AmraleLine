using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using BusinessLogic;
using DataObjects;
using System.Windows.Forms;

namespace SweetPOS
{
    public class FigureToWord
    {
        private static string[] onesMapping = new string[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static string[] tensMapping = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        private static string[] groupMapping = new string[] { "Hundred", "Thousand", "Lac", "Crore" };
        private static int[] groupNumber = new int[] { 1, 1000, 100000, 10000000, 1000, 100000, 10000000, 1000000000 };

        public FigureToWord()
        {

        }

        public static string ConvertToWord(decimal number)
        {
            string retVal = null;
            for (int i = 0; i < 4; i++)
            {
                int numberToProcess = (int)(number % groupNumber[i + 4] / groupNumber[i]);

                string groupDescription = ProcessGroup(numberToProcess);
                if (groupDescription != null)
                {
                    if (i > 0)
                    {
                        retVal = groupMapping[i] + " " + retVal;
                    }
                    retVal = groupDescription + " " + retVal;
                }
            }

            // Processing for Paise 

            decimal paiseD = number - (int)number;
            string paiseS = paiseD.ToString().Replace(".", "");
            int paiseI = Convert.ToInt32(paiseS);
            int tmp1 = Convert.ToString(paiseI).Length;
            if (tmp1 == 1)
            {
                paiseI = Convert.ToInt32(paiseS + "0");
            }
            string paiseW = ProcessGroup(paiseI);

            if (paiseI > 0)
            {
                return "Rupees " + retVal + "Paise " + paiseW + " Only";
            }
            else
            {
                return "Rupees " + retVal + "Only";
            }
        }

        private static string ProcessGroup(int number)
        {
            int tens = number % 100;
            int hundreds = number / 100;

            string retVal = null;
            if (hundreds > 0)
            {
                retVal = onesMapping[hundreds] + " " + groupMapping[0];
            }
            if (tens > 0)
            {
                if (tens < 20)
                {
                    retVal += ((retVal != null) ? " " : "") + onesMapping[tens];
                }
                else
                {
                    int ones = tens % 10;
                    tens = (tens / 10) - 2; // 20's offset

                    retVal += ((retVal != null) ? " " : "") + tensMapping[tens];

                    if (ones > 0)
                    {
                        retVal += ((retVal != null) ? " " : "") + onesMapping[ones];
                    }
                }
            }
            return retVal;
        }
    }

    public class ReportClass
    {

        string strConn = ConfigurationManager.ConnectionStrings["SweetPOS.Properties.Settings.ConnectionString"].ToString();
        SettingManager _settingManager = new SettingManager();
        InfoManager _infoManager = new InfoManager();

        public void BillWiseSale(DateTime saleDate)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT ID, BillDate, BillNo, TotalAmount FROM Sales WHERE BillDate = @SaleDate";
            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime);
            cmd.Parameters["@SaleDate"].Value = saleDate;
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSales");

            cmd.CommandText = "SELECT SaleItems.SaleID, Items.Name ItemName, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount FROM SaleItems " +
                "INNER JOIN Items ON SaleItems.ItemID = Items.ID INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillDate = @SaleDate";
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleItems");

            //Checks if summary created or not
            conn.Open();
            cmd.CommandText = "SELECT COUNT(ID) FROM SaleTotal WHERE SaleDate = @SaleDate";
            int cnt = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();

            if (cnt == 0)
                cmd.CommandText = "SELECT SUM(Amount) TotalAmount FROM SaleItems WHERE SaleDate = @SaleDate";
            else
                cmd.CommandText = "SELECT SUM(Amount) TotalAmount FROM SaleSummary WHERE SaleDate = @SaleDate";
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblTotal");

            SweetPOS.Reports.rptBillWiseSales reportBillWiseSales = new SweetPOS.Reports.rptBillWiseSales();
            reportBillWiseSales.SetDataSource(ds);

            frmShowReport frmReport = new frmShowReport("Sale Bill (All)");
            frmReport.crystalReportViewer1.ReportSource = reportBillWiseSales;
            frmReport.ShowDialog();
        }

        public void SaleSummaryVatwise(DateTime saleDate, DateTime eSaleDate)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT SaleSummary.SaleDate, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount, Items.Vat FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate ORDER BY Items.Vat";

            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime);
            cmd.Parameters["@SaleDate"].Value = saleDate;
            cmd.Parameters.Add("@ESaleDate", SqlDbType.DateTime);
            cmd.Parameters["@ESaleDate"].Value = eSaleDate;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleSummary");

            SweetPOS.Reports.rptSaleSummaryVat report = new SweetPOS.Reports.rptSaleSummaryVat();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
            report.SetParameterValue("FromTime", saleDate.ToShortTimeString());
            report.SetParameterValue("ToTime", eSaleDate.ToShortTimeString());

            frmShowReport frmReport = new frmShowReport("Sale Summary (Vatwise)");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void SaleSummaryGroupWise(DateTime saleDate, DateTime eSaleDate, bool timeWise)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (!timeWise)
                cmd.CommandText = "SELECT SaleSummary.SaleDate, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate ORDER BY ItemGroups.DisplayIndex";
            else
                cmd.CommandText = "SELECT SaleSummary.SaleDate, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE SaleSummary.CreatedOn >= @SaleDate AND SaleSummary.CreatedOn <= @ESaleDate ORDER BY ItemGroups.DisplayIndex";

            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime);
            cmd.Parameters["@SaleDate"].Value = saleDate;
            cmd.Parameters.Add("@ESaleDate", SqlDbType.DateTime);
            cmd.Parameters["@ESaleDate"].Value = eSaleDate;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleSummary");

            SweetPOS.Reports.rptSaleSummary reportSaleSummary = new SweetPOS.Reports.rptSaleSummary();
            reportSaleSummary.SetDataSource(ds);
            reportSaleSummary.SetParameterValue("Firm", Program.COMPANYNAME);
            reportSaleSummary.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
            reportSaleSummary.SetParameterValue("FromTime", saleDate.ToShortTimeString());
            reportSaleSummary.SetParameterValue("ToTime", eSaleDate.ToShortTimeString());

            frmShowReport frmReport = new frmShowReport("Sale Summary");
            frmReport.crystalReportViewer1.ReportSource = reportSaleSummary;
            frmReport.ShowDialog();
        }

        public DateTime reportTime = DateTime.Now;

        public void SaleSummaryCashierwise(DateTime saleDate, DateTime eSaleDate, int userId)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            ////Sale Summary from Summary table
            //cmd.CommandText = "SELECT SaleSummary.SaleDate, Users.Name Cashier, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID INNER JOIN Users ON SaleSummary.CreatedBy = Users.ID WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate ORDER BY SaleSummary.CreatedBy, ItemGroups.DisplayIndex";
            //cmd.CommandText = "SELECT SaleSummary.SaleDate, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount, Items.Vat FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate ORDER BY Items.Vat";

            ////Sale Summary from Sales table
            if (userId != 0)
            {
                cmd.CommandText = "SELECT BillDate SaleDate, Users.Name Cashier, Items.Name ItemName, SUM(SaleItems.Quantity) Quantity, SUM(SaleItems.Amount) Amount, SaleItems.Vat, SUM(SaleItems.Amount) - ((100 * SUM(SaleItems.Amount))/(100 + SaleItems.Vat)) VatAmount FROM Sales JOIN SaleItems ON SaleItems.SaleID = Sales.ID JOIN Users ON Sales.CreatedBy = Users.ID JOIN Items ON SaleItems.ItemID = Items.ID WHERE Sales.CreatedOn >= @SaleDate AND Sales.CreatedOn <= @ESaleDate AND Sales.CreatedBy = @CreatedBy GROUP BY BillDate, Users.Name, Items.Name, SaleItems.Vat";
                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int);
                cmd.Parameters["@CreatedBy"].Value = userId;
            }
            else
            {
                cmd.CommandText = "SELECT BillDate SaleDate, Users.Name Cashier, Items.Name ItemName, SUM(SaleItems.Quantity) Quantity, SUM(SaleItems.Amount) Amount, SaleItems.Vat, SUM(SaleItems.Amount) - ((100 * SUM(SaleItems.Amount))/(100 + SaleItems.Vat)) VatAmount FROM Sales JOIN SaleItems ON SaleItems.SaleID = Sales.ID JOIN Users ON Sales.CreatedBy = Users.ID JOIN Items ON SaleItems.ItemID = Items.ID WHERE Sales.CreatedOn >= @SaleDate AND Sales.CreatedOn <= @ESaleDate GROUP BY BillDate, Users.Name, Items.Name, SaleItems.Vat";
            }

            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime);
            cmd.Parameters["@SaleDate"].Value = saleDate;
            cmd.Parameters.Add("@ESaleDate", SqlDbType.DateTime);
            cmd.Parameters["@ESaleDate"].Value = eSaleDate;


            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleSummary");

            SweetPOS.Reports.rptSaleCashier report = new SweetPOS.Reports.rptSaleCashier();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Firm1", Program.COMPANYSUBTITLE);
            report.SetParameterValue("SummaryTime", reportTime.ToString("hh:mm:ss tt"));

            frmShowReport frmReport = new frmShowReport("Cashier wise sale");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void SaleSummaryItemwise(int itemId, DateTime saleDate, DateTime eSaleDate)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (itemId == 0)
                cmd.CommandText = "SELECT SaleSummary.SaleDate, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate ORDER BY Items.Name";
            else
            {
                cmd.CommandText = "SELECT SaleSummary.SaleDate, Items.Name ItemName, ItemGroups.Name GroupName, SaleSummary.Quantity, Units.Name Unit, SaleSummary.Rate, SaleSummary.Amount FROM SaleSummary INNER JOIN Items ON Items.ID = SaleSummary.ItemID INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID WHERE SaleDate >= @SaleDate AND SaleDate <= @ESaleDate AND ItemID = @ItemID ORDER BY Items.Name";
                cmd.Parameters.Add("@ItemId", SqlDbType.Int);
                cmd.Parameters["@ItemId"].Value = itemId;
            }
            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime);
            cmd.Parameters["@SaleDate"].Value = saleDate;
            cmd.Parameters.Add("@ESaleDate", SqlDbType.DateTime);
            cmd.Parameters["@ESaleDate"].Value = eSaleDate;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleSummary");

            SweetPOS.Reports.rptSaleSummaryItemwise reportSaleSummaryItemwise = new SweetPOS.Reports.rptSaleSummaryItemwise();
            reportSaleSummaryItemwise.SetDataSource(ds);
            reportSaleSummaryItemwise.SetParameterValue("Firm", Program.COMPANYNAME);

            frmShowReport frmReport = new frmShowReport("Itemwise Sale Summary");
            frmReport.crystalReportViewer1.ReportSource = reportSaleSummaryItemwise;
            frmReport.ShowDialog();
        }

        public void ShowBillChitale(Int64 billNumber, bool singleBill, bool discountBill)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT Sales.ID, Customers.Name AS CustomersName, BillDate SaleDate, CashCredit, BillNo BillNumber, TotalAmount, DiscountAmount, RoundedAmount AS NetAmount FROM Sales JOIN Customers ON Customers.ID= Sales.CustomerID WHERE BillNo = @BillNumber";
            cmd.Parameters.Add("@BillNumber", SqlDbType.BigInt);
            cmd.Parameters["@BillNumber"].Value = billNumber;
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSales");

            cmd.CommandText = "SELECT SaleItems.SaleID, Items.Name ItemName, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount FROM SaleItems " +
                "INNER JOIN Items ON SaleItems.ItemID = Items.ID " +
                "INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillNo = @BillNumber";
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleItems");

            conn.Open();

            decimal totalQuantity = 0;
            cmd.CommandText = "SELECT (SELECT ISNULL(SUM(Quantity), 0) FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID JOIN Items ON SaleItems.ItemID = Items.ID JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.BillNo = @BillNo AND Units.Name != 'No.') + (SELECT ISNULL(SUM(Quantity), 0) * ISNULL(SUM(Items.UnitWeight), 0) FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID JOIN Items ON SaleItems.ItemID = Items.ID JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.BillNo = @BillNo AND Units.Name = 'No.') Quantity";
            cmd.Parameters.Add("@BillNo", SqlDbType.BigInt);
            cmd.Parameters["@BillNo"].Value = billNumber;
            totalQuantity = Convert.ToDecimal(cmd.ExecuteScalar());

            int packItemCount = 0;
            cmd.CommandText = "SELECT ISNULL(SUM(Quantity), 0) AS Quantity FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID JOIN Items ON SaleItems.ItemID = Items.ID JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.BillNo = @BillNo1 AND Units.Name = 'No.'";
            cmd.Parameters.Add("@BillNo1", SqlDbType.BigInt);
            cmd.Parameters["@BillNo1"].Value = billNumber;
            packItemCount = Convert.ToInt32(cmd.ExecuteScalar());

            int itemCount = 0;
            cmd.CommandText = "SELECT ISNULL(COUNT(*), 0) FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillNo = @BillNo2";
            cmd.Parameters.Add("@BillNo2", SqlDbType.BigInt);
            cmd.Parameters["@BillNo2"].Value = billNumber;
            itemCount = Convert.ToInt32(cmd.ExecuteScalar());

            conn.Close();

            bool showBill;
            bool showQuantityTotal;
            string firstLine;
            try
            {
                showBill = Convert.ToBoolean(_settingManager.GetSetting(2));      //Hardcode
                showQuantityTotal = Convert.ToBoolean(_settingManager.GetSetting(15));      //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                firstLine = Convert.ToString(_infoManager.GetInfo(9));      //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting First Line." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SweetPOS.Reports.rptChitale rpt = new SweetPOS.Reports.rptChitale();
            rpt.SetDataSource(ds);
            rpt.SetParameterValue("FirstLine", firstLine);
            rpt.SetParameterValue("Firm", Program.COMPANYNAME);
            rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);


            if (!singleBill)
            {
                if (showBill)
                {
                    frmShowReport frmReport = new frmShowReport("Sale Bill");
                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
                else
                    rpt.PrintToPrinter(1, false, 1, 1);
            }
            else
            {
                frmShowReport frmReport = new frmShowReport("Sale Bill");
                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
            }
        }

        public void ShowBill(Int64 billNumber, bool singleBill, bool discountBill)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT Sales.ID, Customers.Name AS CustomerName,Customers.CustomerNumber, Sales.CreatedOn SaleDate, CashCredit, BillNo BillNumber, TotalAmount, DiscountAmount, RoundedAmount AS NetAmount FROM Sales LEFT JOIN Customers ON Customers.ID= Sales.CustomerID WHERE BillNo = @BillNumber";
            cmd.Parameters.Add("@BillNumber", SqlDbType.BigInt);
            cmd.Parameters["@BillNumber"].Value = billNumber;
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSales");

            cmd.CommandText = "SELECT SaleItems.SaleID, Items.Name ItemName, SaleItems.Quantity, SaleItems.Rate, SaleItems.Amount FROM SaleItems " +
                "INNER JOIN Items ON SaleItems.ItemID = Items.ID " +
                "INNER JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillNo = @BillNumber";
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblSaleItems");

            conn.Open();

            decimal totalQuantity = 0;
            cmd.CommandText = "SELECT (SELECT ISNULL(SUM(Quantity), 0) FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID JOIN Items ON SaleItems.ItemID = Items.ID JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.BillNo = @BillNo AND Units.Name != 'No.') + (SELECT ISNULL(SUM(Quantity), 0) * ISNULL(SUM(Items.UnitWeight), 0) FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID JOIN Items ON SaleItems.ItemID = Items.ID JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.BillNo = @BillNo AND Units.Name = 'No.') Quantity";
            cmd.Parameters.Add("@BillNo", SqlDbType.BigInt);
            cmd.Parameters["@BillNo"].Value = billNumber;
            totalQuantity = Convert.ToDecimal(cmd.ExecuteScalar());

            int packItemCount = 0;
            cmd.CommandText = "SELECT ISNULL(SUM(Quantity), 0) AS Quantity FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID JOIN Items ON SaleItems.ItemID = Items.ID JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.BillNo = @BillNo1 AND Units.Name = 'No.'";
            cmd.Parameters.Add("@BillNo1", SqlDbType.BigInt);
            cmd.Parameters["@BillNo1"].Value = billNumber;
            packItemCount = Convert.ToInt32(cmd.ExecuteScalar());

            int itemCount = 0;
            cmd.CommandText = "SELECT ISNULL(COUNT(*), 0) FROM SaleItems JOIN Sales ON SaleItems.SaleID = Sales.ID WHERE Sales.BillNo = @BillNo2";
            cmd.Parameters.Add("@BillNo2", SqlDbType.BigInt);
            cmd.Parameters["@BillNo2"].Value = billNumber;
            itemCount = Convert.ToInt32(cmd.ExecuteScalar());

            conn.Close();

            bool showBill;
            bool showQuantityTotal;
            string firstLine;
            try
            {
                showBill = Convert.ToBoolean(_settingManager.GetSetting(2));      //Hardcode
                showQuantityTotal = Convert.ToBoolean(_settingManager.GetSetting(15));      //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                firstLine = Convert.ToString(_infoManager.GetInfo(9));      //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting First Line." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!discountBill)
            {
                if (Program.CLIENTNAME == string.Empty)
                {
                    #region Default Bill Report
                    SweetPOS.Reports.rptReceipt rpt = new SweetPOS.Reports.rptReceipt();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("FirstLine", firstLine);
                    rpt.SetParameterValue("Firm", Program.COMPANYNAME);
                    rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
                    rpt.SetParameterValue("Address1", Program.ADDRESS1);
                    rpt.SetParameterValue("Address2", Program.ADDRESS2);
                    rpt.SetParameterValue("VatNo", Program.VATNO);
                    rpt.SetParameterValue("SubjectTo", Program.SUBJECTTO);
                    rpt.SetParameterValue("ItemCount", itemCount.ToString());
                    if (showQuantityTotal)
                        rpt.SetParameterValue("QuantityTotal", packItemCount.ToString() + "#    " + totalQuantity.ToString("0.000"));
                    else
                        rpt.SetParameterValue("QuantityTotal", string.Empty);

                    if (!singleBill)
                    {
                        if (showBill)
                        {
                            frmShowReport frmReport = new frmShowReport("Sale Bill");
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                        else
                            rpt.PrintToPrinter(1, false, 1, 1);
                    }
                    else
                    {
                        frmShowReport frmReport = new frmShowReport("Sale Bill");
                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    #endregion
                }
                else if (Program.CLIENTNAME.ToUpper() == "MITHAS")
                {
                    #region MITHAS Bill Report
                    SweetPOS.Reports.rptReceiptMithas rpt = new SweetPOS.Reports.rptReceiptMithas();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("FirstLine", firstLine);
                    rpt.SetParameterValue("Firm", Program.COMPANYNAME);
                    rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
                    rpt.SetParameterValue("Address1", Program.ADDRESS1);
                    rpt.SetParameterValue("Address2", Program.ADDRESS2);
                    rpt.SetParameterValue("VatNo", Program.VATNO);
                    rpt.SetParameterValue("SubjectTo", Program.SUBJECTTO);
                    rpt.SetParameterValue("ItemCount", itemCount.ToString());
                    if (showQuantityTotal)
                        rpt.SetParameterValue("QuantityTotal", packItemCount.ToString() + "#    " + totalQuantity.ToString("0.000"));
                    else
                        rpt.SetParameterValue("QuantityTotal", string.Empty);

                    if (!singleBill)
                    {
                        if (showBill)
                        {
                            frmShowReport frmReport = new frmShowReport("Sale Bill");
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                        else
                            rpt.PrintToPrinter(1, false, 1, 1);
                    }
                    else
                    {
                        frmShowReport frmReport = new frmShowReport("Sale Bill");
                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    #endregion
                }
                else if (Program.CLIENTNAME.ToUpper() == "MOHNISH")
                {
                    #region MOHNISH Bill Report
                    SweetPOS.Reports.rptReceiptMohnish rpt = new SweetPOS.Reports.rptReceiptMohnish();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("FirstLine", firstLine);
                    rpt.SetParameterValue("Firm", Program.COMPANYNAME);
                    rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
                    rpt.SetParameterValue("Address1", Program.ADDRESS1);
                    rpt.SetParameterValue("Address2", Program.ADDRESS2);
                    rpt.SetParameterValue("VatNo", Program.VATNO);
                    rpt.SetParameterValue("SubjectTo", Program.SUBJECTTO);
                    rpt.SetParameterValue("ItemCount", itemCount.ToString());
                    if (showQuantityTotal)
                        rpt.SetParameterValue("QuantityTotal", packItemCount.ToString() + "#    " + totalQuantity.ToString("0.000"));
                    else
                        rpt.SetParameterValue("QuantityTotal", string.Empty);

                    if (!singleBill)
                    {
                        if (showBill)
                        {
                            frmShowReport frmReport = new frmShowReport("Sale Bill");
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                        else
                            rpt.PrintToPrinter(1, false, 1, 1);
                    }
                    else
                    {
                        frmShowReport frmReport = new frmShowReport("Sale Bill");
                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    #endregion
                }
                else if (Program.CLIENTNAME.ToUpper() == "INDEX")
                {
                    #region INDEX International Bill Report
                    SweetPOS.Reports.rptReceiptIndex rpt = new SweetPOS.Reports.rptReceiptIndex();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("FirstLine", firstLine);
                    rpt.SetParameterValue("Firm", Program.COMPANYNAME);
                    rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
                    rpt.SetParameterValue("Address1", Program.ADDRESS1);
                    rpt.SetParameterValue("Address2", Program.ADDRESS2);
                    rpt.SetParameterValue("VatNo", Program.VATNO);
                    rpt.SetParameterValue("SubjectTo", Program.SUBJECTTO);
                    rpt.SetParameterValue("ItemCount", itemCount.ToString());
                    if (showQuantityTotal)
                        rpt.SetParameterValue("QuantityTotal", packItemCount.ToString() + "#    " + totalQuantity.ToString("0.000"));
                    else
                        rpt.SetParameterValue("QuantityTotal", string.Empty);

                    if (!singleBill)
                    {
                        if (showBill)
                        {
                            frmShowReport frmReport = new frmShowReport("Sale Bill");
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                        else
                            rpt.PrintToPrinter(1, false, 1, 1);
                    }
                    else
                    {
                        frmShowReport frmReport = new frmShowReport("Sale Bill");
                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    #endregion
                }
                else if (Program.CLIENTNAME.ToUpper() == "ATHARV" || Program.CLIENTNAME.ToUpper() == "SHREEVEDANG")
                {
                    #region Atharv International Bill Report
                    SweetPOS.Reports.rptReceiptAtharv rpt = new SweetPOS.Reports.rptReceiptAtharv();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("FirstLine", firstLine);
                    rpt.SetParameterValue("Firm", Program.COMPANYNAME);
                    rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
                    rpt.SetParameterValue("Address1", Program.ADDRESS1);
                    rpt.SetParameterValue("Address2", Program.ADDRESS2);
                    rpt.SetParameterValue("VatNo", Program.VATNO);
                    rpt.SetParameterValue("SubjectTo", Program.SUBJECTTO);
                    rpt.SetParameterValue("ItemCount", itemCount.ToString());
                    if (showQuantityTotal)
                        rpt.SetParameterValue("QuantityTotal", packItemCount.ToString() + "#    " + totalQuantity.ToString("0.000"));
                    else
                        rpt.SetParameterValue("QuantityTotal", string.Empty);

                    if (!singleBill)
                    {
                        if (showBill)
                        {
                            frmShowReport frmReport = new frmShowReport("Sale Bill");
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                        else
                            rpt.PrintToPrinter(1, false, 1, 1);
                    }
                    else
                    {
                        frmShowReport frmReport = new frmShowReport("Sale Bill");
                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    #endregion
                }
            }
            else
            {
                SweetPOS.Reports.rptReceiptDiscount rpt = new SweetPOS.Reports.rptReceiptDiscount();
                rpt.SetDataSource(ds);
                rpt.SetParameterValue("Firm", Program.COMPANYNAME);
                rpt.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
                rpt.SetParameterValue("Address1", Program.ADDRESS1);
                rpt.SetParameterValue("Address2", Program.ADDRESS2);
                rpt.SetParameterValue("VatNo", Program.VATNO);
                rpt.SetParameterValue("SubjectTo", Program.SUBJECTTO);

                if (!singleBill)
                {
                    if (showBill)
                    {
                        frmShowReport frmReport = new frmShowReport("Sale Bill");
                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    else
                        rpt.PrintToPrinter(1, false, 1, 1);
                }
                else
                {
                    frmShowReport frmReport = new frmShowReport("Sale Bill");
                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
            }
        }

        public void ShowItems(bool groupWise, string groupName, bool sortByCode)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT ItemGroups.Name ItemGroup, Items.ItemCode, Items.Name, Items.Rate, Units.Name Unit FROM Items INNER JOIN ItemGroups ON Items.ItemGroupID = ItemGroups.ID INNER JOIN Units ON Items.UnitID = Units.ID";

            if (groupWise)
            {
                cmd.CommandText += " WHERE ItemGroups.Name LIKE '" + groupName + "%' ";
                cmd.CommandText += " ORDER BY ItemGroups.DisplayIndex";
            }
            else
            {
                if (sortByCode)
                    cmd.CommandText += " ORDER BY Items.ItemCode";
                else
                    cmd.CommandText += " ORDER BY Items.Name";
            }

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblItems");

            SweetPOS.Reports.rptItems report = new SweetPOS.Reports.rptItems();
            report.SetDataSource(ds);

            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
            report.SetParameterValue("Address1", Program.ADDRESS1);
            report.SetParameterValue("Address2", Program.ADDRESS2);

            frmShowReport frmReport = new frmShowReport("Items List");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ShowStockReport(DateTime sDate, DateTime eDate, int divisionID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsStock ds = new SweetPOS.Reports.dsStock();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT Stock.StockDate, Items.Name ItemName, Stock.Opening, Stock.Challan + Stock.IBInward AS Challan, Stock.IBOutward, Stock.Sale, Stock.Closing, Stock.Adjusted FROM Stock INNER JOIN Items ON Stock.ItemID = Items.ID WHERE StockDate >= @SDate AND StockDate <= @EDate AND DivisionID = @DivisionID ORDER BY Stock.StockDate";

            cmd.Parameters.Add("@SDate", SqlDbType.DateTime);
            cmd.Parameters["@SDate"].Value = sDate;
            cmd.Parameters.Add("@EDate", SqlDbType.DateTime);
            cmd.Parameters["@EDate"].Value = eDate;
            cmd.Parameters.Add("@DivisionID", SqlDbType.Int);
            cmd.Parameters["@DivisionID"].Value = divisionID;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblStock");

            SweetPOS.Reports.rptStock report = new SweetPOS.Reports.rptStock();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("SDate", sDate.ToShortDateString());
            report.SetParameterValue("EDate", eDate.ToShortDateString());

            frmShowReport frmReport = new frmShowReport("Stock Report");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void DayWiseStockReport(DateTime sDate, DateTime eDate, int divisionID, string DivisionName)
        {
            ///New Mini Report
            SqlConnection conn1 = new SqlConnection(strConn);
            SqlCommand cmd1 = new SqlCommand();
            SqlDataAdapter adp1 = new SqlDataAdapter();
            SweetPOS.Reports.dsStock ds1 = new SweetPOS.Reports.dsStock();

            cmd1.Connection = conn1;
            cmd1.CommandType = CommandType.Text;

            cmd1.CommandText = "SELECT Stock.StockDate, Items.Name ItemName, Stock.Opening, Stock.Challan + Stock.IBInward AS  Challan, Stock.IBOutward, Stock.Sale, Stock.Closing, Stock.Adjusted FROM Stock INNER JOIN Items ON Stock.ItemID = Items.ID WHERE StockDate >= @SDate AND StockDate <= @EDate AND DivisionID = @DivisionID ORDER BY Stock.StockDate ";

            cmd1.Parameters.Add("@SDate", SqlDbType.DateTime);
            cmd1.Parameters["@SDate"].Value = sDate;
            cmd1.Parameters.Add("@EDate", SqlDbType.DateTime);
            cmd1.Parameters["@EDate"].Value = eDate;
            cmd1.Parameters.Add("@DivisionID", SqlDbType.Int);
            cmd1.Parameters["@DivisionID"].Value = divisionID;

            adp1.SelectCommand = cmd1;
            adp1.Fill(ds1, "tblStock");

            SweetPOS.Reports.rptStock1 report1 = new SweetPOS.Reports.rptStock1();
            report1.SetDataSource(ds1);
            report1.SetParameterValue("Firm", Program.COMPANYNAME);
            report1.SetParameterValue("DivisionName", DivisionName);

            frmShowReport frmReport = new frmShowReport("Stock Report");
            frmReport.crystalReportViewer1.ReportSource = report1;
            frmReport.ShowDialog();
            ShowStockReport(sDate, eDate, divisionID);
        }

        public void ChallanReport(DateTime sDate, DateTime eDate, int supplierID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (supplierID == 0)
                cmd.CommandText = "SELECT Challans.ID, Challans.ChallanDate, Challans.ChallanNo, Suppliers.Name SupplierName, Challans.ReceivedDate, Challans.VehicleNo, Challans.DeliveredBy, Users.Name ReceivedBy, Challans.Completed, Challans.Description FROM Challans INNER JOIN Suppliers ON Challans.SupplierID = Suppliers.ID Left JOIN Users ON Challans.ReceivedBy = Users.ID WHERE Challans.ChallanDate BETWEEN @SDate AND @EDate";
            else
                cmd.CommandText = "SELECT Challans.ID, Challans.ChallanDate, Challans.ChallanNo, Suppliers.Name SupplierName, Challans.ReceivedDate, Challans.VehicleNo, Challans.DeliveredBy, Users.Name ReceivedBy, Challans.Completed, Challans.Description FROM Challans INNER JOIN Suppliers ON Challans.SupplierID = Suppliers.ID Left JOIN Users ON Challans.ReceivedBy = Users.ID WHERE Challans.ChallanDate BETWEEN @SDate AND @EDate AND Suppliers.ID = @SupplierID";

            cmd.Parameters.Add("@SDate", SqlDbType.DateTime);
            cmd.Parameters["@SDate"].Value = sDate;
            cmd.Parameters.Add("@EDate", SqlDbType.DateTime);
            cmd.Parameters["@EDate"].Value = eDate;
            cmd.Parameters.Add("@SupplierID", SqlDbType.Int);
            cmd.Parameters["@SupplierID"].Value = supplierID;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblChallans");

            cmd.CommandText = "SELECT ChallanItems.ChallanID, Items.Name ItemName, ChallanItems.Quantity, Units.Name UnitName FROM ChallanItems INNER JOIN Items ON ChallanItems.ItemID = Items.ID INNER JOIN Units ON ChallanItems.UnitID = Units.ID";

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblChallanItems");

            SweetPOS.Reports.rptChallan report = new SweetPOS.Reports.rptChallan();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("FromDate", sDate);
            report.SetParameterValue("ToDate", eDate);

            frmShowReport frmReport = new frmShowReport("Challan Report");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void InvoiceReport(DateTime sDate, DateTime eDate, int supplierID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (supplierID == 0)
                cmd.CommandText = "SELECT Invoices.ID, Invoices.InvoiceDate, Invoices.InvoiceNo, Suppliers.Name SupplierName, Invoices.CashCredit, Invoices.TotalAmount, Invoices.TaxAmount, Invoices.OtherCharges, Invoices.DiscountAmount, Invoices.NetAmount, Invoices.BalanceAmount, Invoices.DeliveredBy FROM Invoices INNER JOIN Suppliers ON Invoices.SupplierID = Suppliers.ID WHERE Invoices.InvoiceDate BETWEEN @SDate AND @EDate";
            else
                cmd.CommandText = "SELECT Invoices.ID, Invoices.InvoiceDate, Invoices.InvoiceNo, Suppliers.Name SupplierName, Invoices.CashCredit, Invoices.TotalAmount, Invoices.TaxAmount, Invoices.OtherCharges, Invoices.DiscountAmount, Invoices.NetAmount, Invoices.BalanceAmount, Invoices.DeliveredBy FROM Invoices INNER JOIN Suppliers ON Invoices.SupplierID = Suppliers.ID WHERE Invoices.InvoiceDate BETWEEN @SDate AND @EDate AND Suppliers.ID = @SupplierID";

            cmd.Parameters.Add("@SDate", SqlDbType.DateTime);
            cmd.Parameters["@SDate"].Value = sDate;
            cmd.Parameters.Add("@EDate", SqlDbType.DateTime);
            cmd.Parameters["@EDate"].Value = eDate;
            cmd.Parameters.Add("@SupplierID", SqlDbType.Int);
            cmd.Parameters["@SupplierID"].Value = supplierID;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblPurchases");

            cmd.CommandText = "SELECT InvoiceItems.InvoiceID, Items.Name ItemName, InvoiceItems.Quantity, Units.Name UnitName, InvoiceItems.Vat, InvoiceItems.Rate, InvoiceItems.Amount FROM InvoiceItems INNER JOIN Items ON InvoiceItems.ItemID = Items.ID INNER JOIN Units ON InvoiceItems.UnitID = Units.ID";

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblPurchaseItems");

            SweetPOS.Reports.rptInvoice report = new SweetPOS.Reports.rptInvoice();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("FromDate", sDate);
            report.SetParameterValue("ToDate", eDate);

            frmShowReport frmReport = new frmShowReport("Invoice Report");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ReceiptPaymentReport(DateTime sDate, DateTime eDate)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsStock ds = new SweetPOS.Reports.dsStock();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT TransactionDate, ReceiptPayment, Particulars, PayMode, Amount FROM ReceiptPayments WHERE TransactionDate >= @SDate AND TransactionDate <= @EDate ORDER BY TransactionDate, ReceiptPayment";

            cmd.Parameters.Add("@SDate", SqlDbType.DateTime);
            cmd.Parameters["@SDate"].Value = sDate;
            cmd.Parameters.Add("@EDate", SqlDbType.DateTime);
            cmd.Parameters["@EDate"].Value = eDate;

            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblReceiptPayments");

            SweetPOS.Reports.rptReceiptPayment report = new SweetPOS.Reports.rptReceiptPayment();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("SDate", sDate.ToShortDateString());
            report.SetParameterValue("EDate", eDate.ToShortDateString());

            frmShowReport frmReport = new frmShowReport("Receipt Payment Report");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void OrderForm(int orderNumber)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsStock ds = new SweetPOS.Reports.dsStock();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT Orders.ID OrderNumber, OrderDate, DeliveryDate DeliveryDateTime, Customers.Name CustomerName, [Address] " +
                "FROM Orders JOIN Customers ON CustomerID = Customers.ID WHERE Orders.ID = " + orderNumber;
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblOrder");

            cmd.CommandText = "SELECT Orders.ID OrderNumber, Items.Name ItemName, Quantity, Units.Name UnitName, OrderItems.Rate, Amount, Items.Vat " +
                              ",(Amount * Items.Vat)/100 VatAmount, Amount + (Amount * Items.Vat)/100 AmountWithVat " +
                              "FROM OrderItems JOIN Orders ON OrderID = Orders.ID JOIN Items ON ItemID = Items.ID " +
                              "JOIN Units ON OrderItems.UnitID = Units.ID WHERE Orders.ID = " + orderNumber;
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblOrderDetails");

            cmd.Connection.Open();
            cmd.CommandText = "SELECT SUM(Amount + (Amount * Items.Vat)/100) " +
                              "FROM OrderItems JOIN Orders ON OrderID = Orders.ID JOIN Items ON ItemID = Items.ID " +
                              "WHERE Orders.ID = " + orderNumber;
            decimal totalAmt = Convert.ToDecimal(cmd.ExecuteScalar());

            SweetPOS.Reports.rptOrderForm1 report = new SweetPOS.Reports.rptOrderForm1();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("AmountInWords", FigureToWord.ConvertToWord(totalAmt));

            frmShowReport frmReport = new frmShowReport("Order Form");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ShowInvoice(int orderNumber)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsStock ds = new SweetPOS.Reports.dsStock();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT BillNo OrderNumber, BillDate OrderDate, Customers.Name CustomerName, IsNull([Address], '') [Address], IsNull('Buyers TIN Number: '+Customers.VatTinNumber,'') BuyerTINNumber " +
                              "FROM Sales JOIN Customers ON CustomerID = Customers.ID WHERE Sales.ID  = (SELECT BillID FROM Orders WHERE ID = " + orderNumber + ")";
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblOrder");

            cmd.CommandText = "SELECT BillNo OrderNumber, Items.Name ItemName, Quantity, Units.Name UnitName, SaleItems.Rate, Amount, Items.Vat " +
                              ",(Amount * Items.Vat)/100 VatAmount, Amount + (Amount * Items.Vat)/100 AmountWithVat "+
                              "FROM SaleItems JOIN Sales ON SaleID = Sales.ID JOIN Items ON ItemID = Items.ID "+
                              "JOIN Units ON SaleItems.UnitID = Units.ID WHERE Sales.ID = (SELECT BillID FROM Orders WHERE ID = " + orderNumber + ")";
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tblOrderDetails");

            cmd.Connection.Open();
            cmd.CommandText = "SELECT SUM(Amount + (Amount * Items.Vat)/100) " +
                              "FROM SaleItems JOIN Sales ON SaleID = Sales.ID JOIN Items ON ItemID = Items.ID " +
                              "WHERE Sales.ID = (SELECT BillID FROM Orders WHERE ID = " + orderNumber + ")";
            decimal totalAmt = Convert.ToDecimal(cmd.ExecuteScalar());

            SweetPOS.Reports.rptInvoice1 report = new SweetPOS.Reports.rptInvoice1();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("AmountInWords", FigureToWord.ConvertToWord(totalAmt));

            frmShowReport frmReport = new frmShowReport("Tax Invoice");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        //Below Functions added by Jitendra

        //public void ShowSalesProcessingReport(int CustomerID, DateTime BillDate, int SearchIN)
        public void ShowSalesProcessingReport(string CustomerIDs, DateTime BillDate, int SearchIN)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                string commandText = "SELECT SaleProcessing.ID, CustomerID,Customers.CustomerNumber,CASE WHEN Customers.NameMarathi IS NULL THEN Customers.Name ELSE Customers.NameMarathi END Name ,Customers.LineID LineNumber, SaleProcessing.Amount,Customers.Deposit,SaleProcessing.OpeningBalance From SaleProcessing JOIN Customers On SaleProcessing.CustomerID = Customers.ID ";
                commandText += " WHERE Month=" + BillDate.Month + " AND Year=" + BillDate.Year + " AND CustomerID IN ( " + CustomerIDs + " ) ";
                commandText += (SearchIN == 0) ? " ORDER BY Name " : " ORDER BY CustomerNumber ";

                //if (CustomerID > 0)
                //{
                //    commandText += " WHERE Month=" + BillDate.Month + " AND Year=" + BillDate.Year + " AND CustomerID = " + CustomerID + " ";
                //}
                //else
                //{
                //    commandText += " WHERE Month=" + BillDate.Month + " AND Year=" + BillDate.Year ;
                //    commandText += (SearchIN == 0) ? " ORDER BY Name " : " ORDER BY CustomerID ";
                //}
                cmd.CommandText = commandText;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "SaleProcessing");
                string commandTextItems = " SELECT SaleProcessingItems.SaleProcessingID, Items.Name ItemName, SaleProcessingItems.Details, SaleProcessingItems.Amount, SaleProcessingItems.ItemQuantity FROM SaleProcessingItems JOIN Items ON SaleProcessingItems.ItemID = Items.ID JOIN SaleProcessing ON SaleProcessing.ID = SaleProcessingItems.SaleProcessingID ";
                commandTextItems += " WHERE Month=" + BillDate.Month + " AND Year=" + BillDate.Year + " AND CustomerID IN ( " + CustomerIDs + " ) order BY Items.Name ";

                //if (CustomerID > 0)
                //{
                //    commandTextItems += " WHERE Month=" + BillDate.Month + " AND Year=" + BillDate.Year + " AND CustomerID = " + CustomerID + " ";
                //}
                //else
                //{
                //    commandTextItems += " WHERE Month=" + BillDate.Month + " AND Year=" + BillDate.Year + " ORDER BY CustomerID ";
                //}
                cmd.CommandText = commandTextItems;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "SaleProcessingItems");

            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptLineReceipt report = new SweetPOS.Reports.rptLineReceipt();
            //SweetPOS.Reports.rptSaleProcessing report = new SweetPOS.Reports.rptSaleProcessing();
            report.SetDataSource(ds);


            //#region Assign custom Paper Size & Printer Name...

            //string paperSize = "A5";
            //System.Drawing.Printing.PrintDocument p = new System.Drawing.Printing.PrintDocument();

            //System.Drawing.Printing.PaperSize pSize = new System.Drawing.Printing.PaperSize();
            //for (int i = 0; i < p.PrinterSettings.PaperSizes.Count; i++)
            //{
            //    if (p.PrinterSettings.PaperSizes[i].PaperName == paperSize)
            //    {
            //        pSize = p.PrinterSettings.PaperSizes[i];
            //        break;
            //    }
            //}

            //report.PrintOptions.PaperSize = (CrystalDecisions.Shared.PaperSize)pSize.RawKind;
            //report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;

            //#endregion

            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("FromDate", BillDate.ToString("MMMM") + " " + BillDate.Year);

            frmShowReport frmReport = new frmShowReport("Sale Processed");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ShowPaymentReceipt(int ID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                string commandText = " SELECT Customers.Name CustomerName, CASE WHEN(SalePayment.PaymentMode = 1 ) then 'CHEQUE' else 'CASH'  END AS PaymentMode, SalePayment.PaymentDate,SalePayment.OpeningBalance, SalePayment.PaidAmount, SalePayment.ClosingBalance, SalePayment.AdjustmentAmount, SalePayment.Comment, SalePayment.ChequeNo FROM SalePayment JOIN Customers " +
                                     " ON SalePayment.CustomerID = Customers.ID  WHERE SalePayment.ID = " + ID + " ";

                cmd.CommandText = commandText;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "SalePayment");               
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptSalePayment report = new SweetPOS.Reports.rptSalePayment();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);

            frmShowReport frmReport = new frmShowReport("Sale Payment");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ShowServerSales(int mode, DateTime fromDate, DateTime toDate, int cashierID, int divisionID, Boolean IsTimeWise)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                string dateString = IsTimeWise ? " Sales.CreatedOn " : " Sales.BillDate ";
                string commandText = " SELECT Items.Name ItemName, SUM(SaleItems.Quantity) Quantity, SUM (SaleItems.Amount) Amount FROM SaleItems JOIN Items ON SaleItems.ItemID = Items.ID JOIN Sales ON SaleItems.SaleID = Sales.ID ";
                string cmdCouponIssue = " SELECT Items.Name ItemName, SUM(TC_CouponsItems.Quantity) Quantity, SUM(TC_CouponsItems.Amount) Amount FROM T_Coupons JOIN TC_CouponsItems ON TC_CouponsItems.CouponID = T_Coupons.ID JOIN Items ON Items.ID = TC_CouponsItems.ItemID WHERE T_Coupons.CreatedOn BETWEEN @fromDate and @todate ";

                if (!IsTimeWise)
                {
                    if (mode == 1)
                        commandText += " WHERE Sales.BillDate BETWEEN @fromDate and @todate ";
                    else if (mode == 2)
                        commandText += " WHERE Sales.BillDate BETWEEN @fromDate and @todate AND Sales.CreatedBy = @CreatedBy ";
                    else if (mode == 3)
                        commandText += " WHERE Sales.BillDate BETWEEN @fromDate and @todate AND Sales.DivisionID = @DivisionID ";
                    else
                        commandText += " WHERE Sales.CreatedOn BETWEEN @fromDate and @todate AND Sales.CreatedBy = @CreatedBy AND Sales.DivisionID = @DivisionID  ";
                }
                else
                {
                    if (mode == 1)
                        commandText += " WHERE Sales.CreatedOn BETWEEN @fromDate and @todate ";
                    else if (mode == 2)
                    {
                        commandText += " WHERE Sales.CreatedOn BETWEEN @fromDate and @todate ";
                        if (cashierID == 0)
                        {
                            commandText += " GROUP BY Items.Name UNION " + cmdCouponIssue;
                        }
                        if (cashierID == 1) //Card Sale
                            commandText += " AND Sales.CustomerID != 0 ";
                        else if (cashierID == 2) // Coupon Sale
                            commandText += " AND Sales.IsCouponSale = 1 ";
                        else if (cashierID == 3) // Cash Sale
                            commandText += " AND (Sales.CustomerID = 0 AND Sales.IsCouponSale = 0) ";
                        else if (cashierID == 4)
                            commandText = cmdCouponIssue;
                    }
                }
                commandText += " GROUP BY Items.Name ";

                cmd.CommandText = commandText;

                cmd.Parameters.Add("@fromDate", SqlDbType.DateTime);
                cmd.Parameters["@FromDate"].Value = fromDate;

                cmd.Parameters.Add("@toDate", SqlDbType.DateTime);
                cmd.Parameters["@ToDate"].Value = toDate;

                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int);
                cmd.Parameters["@CreatedBy"].Value = cashierID;

                cmd.Parameters.Add("@DivisionID", SqlDbType.Int);
                cmd.Parameters["@DivisionID"].Value = divisionID;

                adp.SelectCommand = cmd;
                adp.Fill(ds, "ServerSales");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptServerSales report = new SweetPOS.Reports.rptServerSales();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
            string Date = " From " + fromDate.ToShortDateString() + " To " + toDate.ToShortDateString();
            report.SetParameterValue("DateString", Date.ToString());

            if (!IsTimeWise)
            {
                if (cashierID == 0)
                {
                    report.SetParameterValue("Cashier", " Both Cashier ");
                }
                else
                {
                    UserManager _manager = new UserManager();
                    UserInfo user = _manager.GetUser(cashierID);
                    report.SetParameterValue("Cashier", user.Name);
                }
            }
            else
            {
                if (mode == 1)
                    report.SetParameterValue("Cashier", " All Sale ");
                else
                {
                    if (cashierID == 0)
                        report.SetParameterValue("Cashier", " All Sale ");
                    if (cashierID == 1)
                        report.SetParameterValue("Cashier", " Card Sale ");
                    else if (cashierID == 2)
                        report.SetParameterValue("Cashier", " Coupon Sale ");
                    else if (cashierID == 3)
                        report.SetParameterValue("Cashier", " Cash Sale ");
                    else if (cashierID == 4)
                        report.SetParameterValue("Cashier", " Coupon Issue ");
                }
            }

            if (!IsTimeWise)
            {
                if (divisionID == 0)
                    report.SetParameterValue("Division", " Both Division ");
                else
                    report.SetParameterValue("Division", " Division : " + divisionID);
            }
            else
            {
                report.SetParameterValue("Division", " Time Wise Sale Summary ");
            }
            frmShowReport frmReport = new frmShowReport("Server Sales");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ShowSummarySaleByBill(int mode, int fromBill, int toBill, int cashierID, int divisionID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                string commandText = " SELECT Items.Name ItemName, SUM(SaleItems.Quantity) Quantity, SUM (SaleItems.Amount) Amount FROM SaleItems JOIN Items ON SaleItems.ItemID = Items.ID JOIN Sales ON SaleItems.SaleID = Sales.ID ";

                if (mode == 1)
                    commandText += " WHERE Sales.BillNo >= @fromBill AND Sales.BillNo <= @toBill ";
                else if (mode == 2)
                    commandText += " WHERE Sales.BillNo >= @fromBill AND Sales.BillNo <= @toBill AND Sales.CreatedBy = @CreatedBy ";
                else if (mode == 3)
                    commandText += " WHERE Sales.BillNo >= @fromBill AND Sales.BillNo <= @toBill AND Sales.DivisionID = @DivisionID ";
                else
                    commandText += " WHERE Sales.BillNo >= @fromBill AND Sales.BillNo <= @toBill AND Sales.CreatedBy = @CreatedBy AND Sales.DivisionID = @DivisionID  ";

                commandText += " GROUP BY Items.Name ";

                cmd.CommandText = commandText;

                cmd.Parameters.Add("@fromBill", SqlDbType.Int);
                cmd.Parameters["@FromBill"].Value = fromBill;

                cmd.Parameters.Add("@toBill", SqlDbType.Int);
                cmd.Parameters["@ToBill"].Value = toBill;

                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int);
                cmd.Parameters["@CreatedBy"].Value = cashierID;

                cmd.Parameters.Add("@DivisionID", SqlDbType.Int);
                cmd.Parameters["@DivisionID"].Value = divisionID;

                adp.SelectCommand = cmd;
                adp.Fill(ds, "ServerSales");


            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptServerSales report = new SweetPOS.Reports.rptServerSales();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
            string Date = " Bill No From " + fromBill + " To " + toBill;
            report.SetParameterValue("DateString", Date.ToString());


            if (cashierID == 0)
            {
                report.SetParameterValue("Cashier", " Both Cashier ");
            }
            else
            {
                UserManager _manager = new UserManager();
                UserInfo user = _manager.GetUser(cashierID);
                report.SetParameterValue("Cashier", user.Name);
            }


            if (divisionID == 0)
                report.SetParameterValue("Division", " Both Division ");
            else
                report.SetParameterValue("Division", " Division : " + divisionID);

            frmShowReport frmReport = new frmShowReport("Server Sales");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }

        public void ShowMaterialRegister(DateTime fromDate, DateTime toDate,int itemID ,int divisionID, string ItemName,string DivisinName)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                string commandText = "SELECT LEFT(convert(varchar, ISNULL(ChallanDate,ISNULL(IBInwardDate,ISNULL(IBOutDate,SaleDate))),103),5) AS Date, ISNULL(ChallanQty,0) AS ChallanQty,ISNULL(IBInwardQty,0) " +
                    "AS IBInwardQty,ISNULL(IBOutQty,0) AS IBOutQty,ISNULL(SaleQty,0) AS SaleQty FROM ( "+
                    "(SELECT Challans.ChallanDate AS ChallanDate, SUM(ISNULL (ChallanItems.Quantity,0)) AS ChallanQty FROM Challans "+
                    "JOIN ChallanItems ON ChallanItems.ChallanID=Challans.ID JOIN Items ON Items.ID=ChallanItems.ItemID "+
                    "WHERE ChallanDate BETWEEN @FromDate AND @ToDate AND ChallanItems.ItemID=@ItemID AND SupplierID <> 0 AND ToDivisionID=@DivisionID "+
                    "GROUP BY Challans.ChallanDate,Items.Name)a "+
                    "FULL OUTER JOIN "+
                    "(SELECT Challans.ChallanDate AS IBInwardDate, SUM(ISNULL(ChallanItems.Quantity,0)) AS IBInwardQty FROM Challans "+
                    "JOIN ChallanItems ON ChallanItems.ChallanID=Challans.ID JOIN Items ON Items.ID=ChallanItems.ItemID "+
                    "WHERE ChallanDate BETWEEN @FromDate AND @ToDate AND ChallanItems.ItemID=@ItemID AND SupplierID = 0 AND Challans.ToDivisionID=@DivisionID "+
                    "GROUP BY Challans.ChallanDate,Items.Name)b ON a.ChallanDate = b.IBInwardDate "+
                    "FULL OUTER JOIN "+
                    "(SELECT Challans.ChallanDate AS IBOutDate, SUM(ISNULL(ChallanItems.Quantity,0)) AS IBOutQty FROM Challans "+
                    "JOIN ChallanItems ON ChallanItems.ChallanID=Challans.ID JOIN Items ON Items.ID=ChallanItems.ItemID "+
                    "WHERE ChallanDate BETWEEN @FromDate AND @ToDate AND ChallanItems.ItemID=@ItemID AND SupplierID = 0 AND Challans.FromDivisionID=@DivisionID "+
                    "GROUP BY Challans.ChallanDate,Items.Name)c ON a.ChallanDate = c.IBOutDate "+
                    "FULL OUTER JOIN "+
                    "(SELECT Sales.BillDate AS SaleDate, SUM(SaleItems.Quantity) AS SaleQty FROM Sales "+
                    "JOIN SaleItems ON SaleItems.SaleID=Sales.ID JOIN Items ON Items.ID=SaleItems.ItemID "+
                    "WHERE Sales.BillDate BETWEEN @FromDate AND @ToDate AND SaleItems.ItemID=@ItemID GROUP BY Sales.BillDate, Items.Name)d ON d.SaleDate=a.ChallanDate ) Order By SaleDate";

                cmd.CommandText = commandText;
                cmd.Parameters.Add("@fromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;

                cmd.Parameters.Add("@toDate", SqlDbType.Date);
                cmd.Parameters["@ToDate"].Value = toDate;

                cmd.Parameters.Add("@itemID", SqlDbType.Int);
                cmd.Parameters["@ItemID"].Value = itemID;

                cmd.Parameters.Add("@DivisionID", SqlDbType.Int);
                cmd.Parameters["@DivisionID"].Value = divisionID;

                adp.SelectCommand = cmd;
                adp.Fill(ds, "MaterialRegister");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptMaterialRegister report = new SweetPOS.Reports.rptMaterialRegister();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Item", ItemName);
            report.SetParameterValue("Division", DivisinName);
            string Date = " From " + fromDate.ToShortDateString() + " To " + toDate.ToShortDateString();
            report.SetParameterValue("DateString", Date.ToString());


            frmShowReport frmReport = new frmShowReport("Material Register");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }


        public void ShowSalePayment(DateTime fromDate, DateTime toDate, int LineID, string CashierName)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                string commandText = "SELECT SalePayment.ID, SalePayment.ProcessingID, SalePayment.CustomerID,Customers.CustomerNumber, Customers.Name CustomerName, " +
                                         " CASE WHEN (SalePayment.PaymentMode = 0 ) then 'CASH'  "+                                  
                                        " WHEN(SalePayment.PaymentMode = 1 ) then 'CHEQUE' "+
                                       " WHEN(SalePayment.PaymentMode = 2 ) then 'CARD' "+
                                       " WHEN(SalePayment.PaymentMode = 3 ) then 'PAYTM' "+
                                       " WHEN(SalePayment.PaymentMode = 4 ) then 'ONLINE'  else '' END  AS PaymentModeName, SalePayment.PaymentDate," +
                    "SalePayment.OpeningBalance,SalePayment.PaidAmount,SalePayment.ClosingBalance,SalePayment.AdjustmentAmount,SalePayment.Comment,"+
                    "SalePayment.ChequeNo FROM SalePayment JOIN Customers On SalePayment.CustomerID = Customers.ID "+
                    "WHERE SalePayment.PaymentDate BETWEEN @FromDate AND @ToDate ";
                if (LineID > 0)
                    commandText += " AND Customers.LineID = @LineID ";

                cmd.CommandText = commandText;
                cmd.Parameters.Add("@fromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;

                cmd.Parameters.Add("@toDate", SqlDbType.Date);
                cmd.Parameters["@ToDate"].Value = toDate;

                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;

                adp.SelectCommand = cmd;
                adp.Fill(ds, "SalePayments");
                ds.AcceptChanges();
                string commandText1 = @"SELECT sum (SalePayment.PaidAmount) TotalAmountModeWise, CASE WHEN (SalePayment.PaymentMode = 0 ) then 'CASH' WHEN(SalePayment.PaymentMode = 1 ) then 'CHEQUE'  WHEN(SalePayment.PaymentMode = 2 ) then 'CARD'  WHEN(SalePayment.PaymentMode = 3 ) then 'PAYTM'  
                                       WHEN(SalePayment.PaymentMode = 4 ) then 'ONLINE'  else '' END  AS Paymentmode  FROM SalePayment JOIN Customers On SalePayment.CustomerID = Customers.ID WHERE SalePayment.PaymentDate BETWEEN @FromDate AND @ToDate ";
                if (LineID > 0)
                    commandText1 += " AND Customers.LineID = @LineID GROUP BY SalePayment.PaymentMode ";
                else
                    commandText1 += " GROUP BY SalePayment.PaymentMode ";
                cmd.CommandText = commandText1;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "SalePayments");
                ds.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptSalePayments report = new SweetPOS.Reports.rptSalePayments();
            report.SetDataSource(ds);
            string Date = " From " + fromDate.ToShortDateString() + " To " + toDate.ToShortDateString();
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("FromDate", Date);
            report.SetParameterValue("Cashier", CashierName);
            
            frmShowReport frmReport = new frmShowReport("Sale Payments");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }

        public void ShowSaleTypeWiseReport(DateTime fromDate, DateTime toDate, int cashierID, string SaleType)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsBillWiseSale ds = new SweetPOS.Reports.dsBillWiseSale();

            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                string commandText = "SELECT Items.Name AS ItemName, SUM(SaleItems.Quantity) AS Quantity, SUM(SaleItems.Amount) AS Amount  FROM Sales JOIN SaleItems ON SaleItems.SaleID = Sales.ID JOIN Items ON Items.ID = SaleItems.ItemID ";
                string cmdCouponIssue = " SELECT Items.Name ItemName, SUM(TC_CouponsItems.Quantity) Quantity, SUM(TC_CouponsItems.Amount) Amount FROM T_Coupons JOIN TC_CouponsItems ON TC_CouponsItems.CouponID = T_Coupons.ID JOIN Items ON Items.ID = TC_CouponsItems.ItemID WHERE T_Coupons.IssueDate BETWEEN @fromDate and @todate ";
                //if (cashierID < 0)
                //    commandText += " WHERE Sales.BillDate Between @FromDate AND @ToDate ";
                if (cashierID == 0) // All
                    commandText += " WHERE Sales.BillDate Between @FromDate AND @ToDate GROUP BY Items.Name UNION " + cmdCouponIssue;
                if(cashierID == 1)
                    commandText += " WHERE Sales.BillDate Between @FromDate AND @ToDate AND Sales.CustomerID != 0 "; // card sale
                if(cashierID == 2)
                    commandText += " WHERE Sales.BillDate Between @FromDate AND @ToDate AND Sales.IsCouponSale = 1 "; // Coupon Sale
                if(cashierID == 3)
                    commandText += " WHERE Sales.BillDate Between @FromDate AND @ToDate AND (Sales.CustomerID = 0 AND Sales.IsCouponSale = 0) "; //cash sale
                if (cashierID == 4) // Coupon Isuue
                    commandText = cmdCouponIssue;

                commandText += " GROUP BY Items.Name ";

                cmd.CommandText = commandText;

                cmd.Parameters.Add("@fromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;

                cmd.Parameters.Add("@toDate", SqlDbType.Date);
                cmd.Parameters["@ToDate"].Value = toDate;

                adp.SelectCommand = cmd;
                adp.Fill(ds, "ServerSales");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptServerSales report = new SweetPOS.Reports.rptServerSales();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Firm2", Program.COMPANYSUBTITLE);
            string Date = " From " + fromDate.ToShortDateString() + " To " + toDate.ToShortDateString();
            report.SetParameterValue("DateString", Date.ToString());

            report.SetParameterValue("Cashier", " Sale Type ");
            report.SetParameterValue("Division", SaleType);
            frmShowReport frmReport = new frmShowReport("Server Sales");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
        }



        public void ShowCustomerMessages(DateTime fromDate,  int LineID,string LineName) 
        {
           // Var IsCompleted = "No";
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds= new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                string commandText = @"select [FromDate],[Todate],Customers.Name,[Message],[IsComplated],Customers.Mobile ,Lines.Name,Customers.LineID from CustomerMessages join Customers on Customers.ID=CustomerMessages.CustomerID join Lines on Lines.ID=Customers.LineID
                                      Where Customers.LineID=@LineID AND IsComplated='NO'  AND @FromDate BETWEEN FromDate AND Todate ";
                cmd.CommandText = commandText;
                cmd.Parameters.Add("@FromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblCustomerMessages");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptCustomerMessages report = new SweetPOS.Reports.rptCustomerMessages();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("LineName", LineName);
            report.SetParameterValue("fromDate", fromDate);

            frmShowReport frmReport = new frmShowReport("Customer Messages");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();
           
        }


        public void MilkSummary(DateTime fromDate, DateTime toDate, int LineID, string LineName, int CustomerID)
        {

            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                string commandText = @"Select MilkDeliveryDate,Customers.CustomerNumber, Customers.Name,Buffalo,Cow,HomeDeliveryMilk.LineID from HomeDeliveryMilk join Customers on CustomerID=Customers.ID 
                                        where MilkDeliveryDate BETWEEN @FromDate AND @ToDate AND CustomerID=@CustomerID AND HomeDeliveryMilk.LineID=@LineID Order by MilkDeliveryDate";
                cmd.CommandText = commandText;
                cmd.Parameters.Add("@FromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;
                cmd.Parameters.Add("@toDate", SqlDbType.Date);
                cmd.Parameters["@ToDate"].Value = toDate;
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int);
                cmd.Parameters["@CustomerID"].Value = CustomerID;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblMilkSummary");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptMilkSummary report = new SweetPOS.Reports.rptMilkSummary();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("LineName", LineName);
            report.SetParameterValue("fromDate", fromDate);
            report.SetParameterValue("toDate", toDate);
            frmShowReport frmReport = new frmShowReport("Milk Summary");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }
        public void CowBuffloMilkQuantitySummary(DateTime fromDate, DateTime toDate, int LineID, string LineName, int CustomerID,string Name)
        {
            string whereclause = string.Empty;
            string commandText = string.Empty;
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                if (CustomerID > 0 && LineID == 0)
                    whereclause = " AND CustomerID=@CustomerID ";
                else if (LineID > 0 && CustomerID == 0)
                    whereclause = " AND HomeDeliveryMilk.LineID=@LineID ";
                else if (LineID > 0 && CustomerID > 0)
                    whereclause = " AND CustomerID=@CustomerID AND HomeDeliveryMilk.LineID=@LineID ";


                commandText = @"SELECT SUM(Buffalo) Buffalo,SUM(Cow) Cow,HomeDeliveryMilk.LineID 
                                    FROM HomeDeliveryMilk 
                                    JOIN Customers ON CustomerID=Customers.ID 
                                    WHERE MilkDeliveryDate BETWEEN @FromDate AND @ToDate "+whereclause+"GROUP BY HomeDeliveryMilk.LineID";
                cmd.CommandText = commandText;
                cmd.Parameters.Add("@FromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;
                cmd.Parameters.Add("@toDate", SqlDbType.Date);
                cmd.Parameters["@ToDate"].Value = toDate;
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int);
                cmd.Parameters["@CustomerID"].Value = CustomerID;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblMilkSummary");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptCowBuffloMilkSummary report = new SweetPOS.Reports.rptCowBuffloMilkSummary();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("LineName", LineName);
            report.SetParameterValue("fromDate", fromDate);
            report.SetParameterValue("toDate", toDate);
            report.SetParameterValue("Name", Name==null?"":Name);
            frmShowReport frmReport = new frmShowReport("Milk Summary");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }
        public void CustomerBalanceWithMilkDetails(DateTime fromDate,int LineID, int CustomerID)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

//                string commandText = @"  SELECT LineNumber,CustomerNumber,Name,OpeningBalance,isnull([1],0) Cow,isnull([2],0) Buffalo,isnull([3],0) Products,Amounts GrandTotal,Balance
//                                        FROM (
//                                            Select Lines.LineNumber,Customers.CustomerNumber, Customers.Name,OpeningBalance,CASE WHEN SaleProcessingItems.itemid =1 THEN SaleProcessingItems.itemid ELSE CASE WHEN SaleProcessingItems.itemid =2 then SaleProcessingItems.itemid else 3 end END ItemID,Sum(SaleProcessingItems.amount) Amount,SaleProcessing.Amount Amounts,ClosingBalance Balance
//                                         From SaleProcessing 
//                                           join SaleProcessingItems on SaleProcessing.ID=SaleProcessingItems.SaleProcessingID
//                                           join Customers ON Customers.ID = SaleProcessing.CustomerID 
//                                           Join Lines on Lines.ID = Customers.LineID 
//                                         Where month=@Month and Year=@Year and Customers.LineID=@LineID
//                                           GROUP bY Lines.LineNumber,Customers.CustomerNumber, Customers.Name,OpeningBalance ,SaleProcessing.Amount,ClosingBalance,LineID ,CASE WHEN SaleProcessingItems.itemid =1 THEN SaleProcessingItems.itemid ELSE CASE WHEN SaleProcessingItems.itemid =2 then SaleProcessingItems.itemid else 3 end END, SaleProcessingItems.Amount 
//                                        ) as s
//                                        PIVOT
//                                        (
//                                            SUM(Amount)
//                                            FOR [ItemID] IN ([1], [2], [3])
//                                        )AS pvt";
                string commandText = @"SELECT LineNumber,CustomerNumber,Name,OpeningBalance,Sum(isnull(CowQuantity,0))CowQuantity,sum(isnull([1],0)) Cow,sum(isnull(BuffaloQuantity,0))BuffaloQuantity ,sum(isnull([2],0)) Buffalo,sum(isnull([3],0) )Products,Amounts GrandTotal,Balance
                                        FROM (
                                            Select Lines.LineNumber,Customers.CustomerNumber, Customers.Name,OpeningBalance,CASE WHEN SaleProcessingItems.itemid =1 THEN SaleProcessingItems.itemid ELSE CASE WHEN SaleProcessingItems.itemid =2 then SaleProcessingItems.itemid else 3 end END ItemID,Sum(SaleProcessingItems.amount) Amount,SaleProcessing.Amount Amounts,ClosingBalance Balance,CASE WHEN SaleProcessingItems.itemid =1 THEN Sum(ItemQuantity)End CowQuantity ,CASE WHEN SaleProcessingItems.itemid =2 THEN Sum(ItemQuantity)End BuffaloQuantity
                                         From SaleProcessing 
                                           join SaleProcessingItems on SaleProcessing.ID=SaleProcessingItems.SaleProcessingID
                                           join Customers ON Customers.ID = SaleProcessing.CustomerID 
                                           Join Lines on Lines.ID = Customers.LineID 
                                         Where month=@Month and Year=@Year  and Customers.LineID=@LineID
                                           GROUP bY itemid,Lines.LineNumber,Customers.CustomerNumber, Customers.Name,OpeningBalance ,SaleProcessing.Amount,ClosingBalance,LineID ,CASE WHEN SaleProcessingItems.itemid =1 THEN SaleProcessingItems.itemid ELSE CASE WHEN SaleProcessingItems.itemid =2 then SaleProcessingItems.itemid else 3 end END, SaleProcessingItems.Amount 
                                        ) as s
                                        PIVOT
                                        (
                                            SUM(Amount)
                                            FOR [ItemID] IN ([1], [2], [3])
                                        )AS pvt group by LineNumber,CustomerNumber,Name,OpeningBalance,Amounts,Balance";

                    cmd.CommandText = commandText;
                
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters["@Month"].Value = fromDate.Month;
                cmd.Parameters.Add("@Year", SqlDbType.Int);
                cmd.Parameters["@Year"].Value = fromDate.Year;
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int);
                cmd.Parameters["@CustomerID"].Value = CustomerID;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblCustomerBalance");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptCustomerBalanceAndMilkDetails report = new SweetPOS.Reports.rptCustomerBalanceAndMilkDetails();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("Month", fromDate.Month);
            report.SetParameterValue("Year", fromDate.Year);

           // report.SetParameterValue("LineName", LineName);
            //report.SetParameterValue("fromDate", fromDate);
            //report.SetParameterValue("toDate", toDate);
            //report.SetParameterValue("LineID", LineID);
            frmShowReport frmReport = new frmShowReport("Customer Balance with Milk Details");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }
        public void ProductSummary(DateTime fromDate, DateTime toDate, int LineID, string LineName, int CustomerID)
        {

            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                string commandText = string.Empty;
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                if (CustomerID > 0)
                    commandText = @"  Select Billdate,Items.name ItemName,Quantity,Amount ,Customers.name,Customers.CustomerNumber from SaleItems join SaleS on SaleID=sales.ID join Items on ItemID=Items.ID join Customers on CustomerID=Customers.ID where BillDate between @FromDate and @toDate and Sales.CustomerID=@CustomerID Order by Billdate";
                else
                {
                    commandText = @"SELECT Billdate,Items.name ItemName,SUM(Quantity) Quantity,SUM(Amount) Amount ,Customers.name,Customers.CustomerNumber 
                                    FROM SaleItems 
                                    JOIN SaleS ON SaleID=sales.ID 
                                    JOIN Items ON ItemID=Items.ID 
                                    JOIN Customers ON CustomerID=Customers.ID
                                    WHERE BillDate BETWEEN @FromDate and @toDate
                                    GROUP BY Billdate,Items.name,Customers.name,Customers.CustomerNumber
                                    ORDER BY Billdate";
                }
                
                cmd.CommandText = commandText;
                cmd.Parameters.Add("@FromDate", SqlDbType.Date);
                cmd.Parameters["@FromDate"].Value = fromDate;
                cmd.Parameters.Add("@toDate", SqlDbType.Date);
                cmd.Parameters["@ToDate"].Value = toDate;
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int);
                cmd.Parameters["@CustomerID"].Value = CustomerID;
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblMilkSummary");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptProductSummary report = new SweetPOS.Reports.rptProductSummary();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("LineName", LineName);
            report.SetParameterValue("fromDate", fromDate);
            report.SetParameterValue("toDate", toDate);
            report.SetParameterValue("LineID", LineID);
            frmShowReport frmReport = new frmShowReport("Product Summary");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }
        public void CustomerBalance(DateTime fromDate, int LineID, string LineName, int CustomerID)
        {

            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                if (CustomerID == 0)
                {
                    string commandText = @" Select Customers.CustomerNumber, Customers.Name,OpeningBalance,Amount,PaidAmount,ClosingBalance Balance,Lines.LineNumber From SaleProcessing 
                                        join Customers ON Customers.ID = SaleProcessing.CustomerID
                                        Join Lines on Lines.ID = Customers.LineID Where month=@Month and Year=@Year and Customers.LineID=@LineID Order by  CustomerNumber";

                    cmd.CommandText = commandText;
                }
                else
                {
                    string commandText = @" Select Customers.CustomerNumber, Customers.Name,OpeningBalance,Amount,PaidAmount,ClosingBalance Balance,Lines.LineNumber From SaleProcessing 
                                        join Customers ON Customers.ID = SaleProcessing.CustomerID
                                        Join Lines on Lines.ID = Customers.LineID Where month=@Month and Year=@Year and Customers.LineID=@LineID And CustomerID =@CustomerID Order by  CustomerNumber";

                    cmd.CommandText = commandText;
                }
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters["@Month"].Value = fromDate.Month;
                cmd.Parameters.Add("@Year", SqlDbType.Int);
                cmd.Parameters["@Year"].Value = fromDate.Year;
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int);
                cmd.Parameters["@CustomerID"].Value = CustomerID;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblCustomerBalance");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptCustomerBalance report = new SweetPOS.Reports.rptCustomerBalance();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            report.SetParameterValue("LineName", LineName);
            //report.SetParameterValue("fromDate", fromDate);
            //report.SetParameterValue("toDate", toDate);
            //report.SetParameterValue("LineID", LineID);
            frmShowReport frmReport = new frmShowReport("Customer Balance");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }

        public void CustomerOutStanding(DateTime fromDate, int LineID, int CustomerID)
        {

            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                if (LineID == 0)
                {
                    string commandText = @" Select Lines.LineNumber,Customers.CustomerNumber, Customers.Name,OpeningBalance,Amount,ClosingBalance Balance From SaleProcessing 
                                            join Customers ON Customers.ID = SaleProcessing.CustomerID
                                            Join Lines on Lines.ID = Customers.LineID where month=@Month and Year=@Year And PaidAmount=0  Order by  Customers.CustomerNumber ";

                    cmd.CommandText = commandText;
                }
                else
                {
                    string commandText = @"  Select   Lines.LineNumber,Customers.CustomerNumber, Customers.Name,OpeningBalance,Amount,PaidAmount,ClosingBalance Balance From SaleProcessing 
                                            join Customers ON Customers.ID = SaleProcessing.CustomerID
                                            Join Lines on Lines.ID = Customers.LineID where month=@Month and Year=@Year and Customers.LineID=@LineID  Order by  Customers.CustomerNumber";

                    cmd.CommandText = commandText;
                }
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters["@Month"].Value = fromDate.Month;
                cmd.Parameters.Add("@Year", SqlDbType.Int);
                cmd.Parameters["@Year"].Value = fromDate.Year;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblCustomerBalance");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptLineWiseCustomerOutStanding report = new SweetPOS.Reports.rptLineWiseCustomerOutStanding();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            frmShowReport frmReport = new frmShowReport("Line Wise Customer OutStanding");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }
        public void LineWiseOutStanding(DateTime fromDate, int LineID, int CustomerID)
        {
            string whereClause=string.Empty;
            string whereClause1=string.Empty;
            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adp = new SqlDataAdapter();
            SweetPOS.Reports.dsReportTables ds = new SweetPOS.Reports.dsReportTables();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                string month = fromDate.ToString("MM"); 

                if (LineID > 0)
                {
                    whereClause = " AND HomeDeliveryMilk.LineID=@LineID ";
                    whereClause1 = " and Customers.LineID=@LineID ";
                }
                //ADDED SUB QUERY TO GET PAID AMOUNT DIRECTLY FROM SALEPAYMENT TABLE 
                //SUGGESTED BY KIRAN SIR AND CHANGED BY BHUSHAN JADHAV ON 30/11/2022

//                string commandText = @" SELECT * FROM (
//                SELECT  SUM(Buffalo*BuffaloRate) Buffalo,SUM(Cow*CowRate) Cow,HomeDeliveryMilk.LineID 
//                FROM HomeDeliveryMilk 
//                JOIN Customers ON CustomerID=Customers.ID 
//                WHERE Convert(varchar, Month( MilkDeliveryDate))=@Month  AND   Convert(varchar, Year( MilkDeliveryDate))=@Year   " + whereClause +
//                " GROUP BY HomeDeliveryMilk.LineID )" +
//                " A JOIN " +
//                " (SELECT  Lines.LineNumber,SUM(OpeningBalance) OpeningBalance,Sum(Amount) Amount,SUM(PaidAmount) PaidAmount,sum(ClosingBalance) Balance " +
//                " From SaleProcessing " +
//                " JOIN Customers ON Customers.ID = SaleProcessing.CustomerID " +
//                " JOIN Lines on Lines.ID = Customers.LineID where SaleProcessing.Month=@Month and SaleProcessing.Year=@Year   " +
//                " GROUP BY LineNumber ,Lines.ID " +
//                " ) B ON A.LineID=B.LineNumber  " +
//                " LEFT JOIN (SELECT LineID,SUM(RoundedAmount) Products FROM Sales " +
//                " JOIN Customers ON CustomerID=Customers.ID " +
//                " WHERE Convert(varchar, Month( BillDate))=@Month  AND   Convert(varchar, Year( BillDate))=@Year " + whereClause1 +
//                " GROUP BY LineID) C ON C.LineID=B.LineNumber " +
//                " ORDER BY A.LineID ";

                string commandText = @" SELECT * FROM (
                SELECT  SUM(Buffalo*BuffaloRate) Buffalo,SUM(Cow*CowRate) Cow,HomeDeliveryMilk.LineID 
                FROM HomeDeliveryMilk 
                JOIN Customers ON CustomerID=Customers.ID 
                WHERE Convert(varchar, Month( MilkDeliveryDate))=@Month  AND   Convert(varchar, Year( MilkDeliveryDate))=@Year    GROUP BY HomeDeliveryMilk.LineID ) A  JOIN  (
                SELECT  Lines.LineNumber,SUM(OpeningBalance) OpeningBalance,Sum(Amount) Amount, (SELECT SUM(PAIDAMOUNT) PaidAmount FROM SalePayment
                JOIN CUSTOMERS ON CUSTOMERID=CUSTOMERS.ID
                WHERE  SalePayment.PaymentDate BETWEEN DATEADD(month, 1,CONVERT(VARCHAR,@Year)+CONVERT(VARCHAR,@Month)+'01') AND DATEADD(month, 1,cONVERT(dATE,DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0, CONVERT(VARCHAR,@Year)+CONVERT(VARCHAR,@Month)+'15') + 1, 0)))) AND LINEID=Lines.LineNumber) PaidAmount,sum(ClosingBalance) Balance  
                From SaleProcessing " +
               " JOIN Customers ON Customers.ID = SaleProcessing.CustomerID " +
               " JOIN Lines on Lines.ID = Customers.LineID where SaleProcessing.Month=@Month and SaleProcessing.Year=@Year   " +
               " GROUP BY LineNumber ,Lines.ID " +
               " ) B ON A.LineID=B.LineNumber  " +
               " LEFT JOIN (SELECT LineID,SUM(RoundedAmount) Products FROM Sales " +
               " JOIN Customers ON CustomerID=Customers.ID " +
               " WHERE Convert(varchar, Month( BillDate))=@Month  AND   Convert(varchar, Year( BillDate))=@Year " + whereClause1 +
               " GROUP BY LineID) C ON C.LineID=B.LineNumber " +
               " ORDER BY A.LineID ";

                cmd.CommandText = commandText;
                cmd.Parameters.Add("@LineID", SqlDbType.Int);
                cmd.Parameters["@LineID"].Value = LineID;
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters["@Month"].Value = month;
                cmd.Parameters.Add("@Month1", SqlDbType.Text);
                cmd.Parameters["@Month1"].Value = month;
                cmd.Parameters.Add("@Year", SqlDbType.Int);
                cmd.Parameters["@Year"].Value = fromDate.Year;
                adp.SelectCommand = cmd;
                adp.Fill(ds, "tblCustomerBalance");
            }
            catch (Exception ex)
            {
                throw;
            }
            SweetPOS.Reports.rptLineWiseOutStanding report = new SweetPOS.Reports.rptLineWiseOutStanding();
            report.SetDataSource(ds);
            report.SetParameterValue("Firm", Program.COMPANYNAME);
            frmShowReport frmReport = new frmShowReport("Line Wise OutStanding");
            frmReport.crystalReportViewer1.ReportSource = report;
            frmReport.ShowDialog();

        }
    }
}