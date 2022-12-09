using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

using BusinessLogic;
using DataObjects;
using ChitalePersonalzer;
using System.Drawing.Printing;

namespace SweetPOS
{
    public partial class frmSaleRFIDTest : Form
    {
        #region Class level variables...

        SaleManager _saleManager = new SaleManager();
        ItemManager _itemManager = new ItemManager();

        int _newRecordID = 0;
        bool _newRecordAdded = false;

        #endregion

        Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
        string key = Program.KEYSET;

        public frmSaleRFIDTest()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - RFID Sale Entry (TEST)";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmSaleRFID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
        }

        private void frmSaleRFID_Load(object sender, EventArgs e)
        {
            New();
        }

        bool ValidateFields()
        {


            return true;
        }

        SaleInfo SetValue(out BindingList<SaleItemInfo> saleItems)
        {
            SaleInfo retVal = new SaleInfo();
            BindingList<SaleItemInfo> saleItemss = new BindingList<SaleItemInfo>();

            retVal.BillNo = Convert.ToString(txtBillNo.Text);
            retVal.CashCredit = 0;

            retVal.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
            retVal.DiscountPercentage = 0;
            retVal.DiscountAmount = 0;
            retVal.NetAmount = Convert.ToDecimal(lblNetAmount.Text);

            retVal.BalanceAmount = 0;
            retVal.CustomerID = 0;

            retVal.Description = string.Empty;

            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                SaleItemInfo saleItem = new SaleItemInfo();
                saleItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
                saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                saleItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                saleItem.Vat = Convert.ToDecimal(dr.Cells["Vat"].Value);
                saleItem.Amount = Convert.ToDecimal(dr.Cells["Amount"].Value);

                saleItemss.Add(saleItem);
            }

            saleItems = saleItemss;
            return retVal;
        }

        bool Save(out Int64 billNo)
        {
            if (!ValidateFields())
            {
                billNo = 0;
                return false;
            }

            BindingList<SaleItemInfo> saleItems = null;
            SaleInfo sale = SetValue(out saleItems);

            Int64 billNumber = 0;
            try
            {
                _newRecordID = _saleManager.AddSale(sale, saleItems, Program.CounterName, out billNumber);
                billNo = billNumber;

                _newRecordAdded = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                billNo = 0;
                return false;
            }
        }

        void New()
        {
            txtPreviousBill.Text = lblNetAmount.Text;

            grdItems.Rows.Clear();
            lblMessages.Visible = false;

            try
            {
                txtBillNo.Text = _saleManager.GetNextBillNumber().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting next bill number." + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalculateTotals();

            btnShowBill.Focus();
        }

        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        ItemInfo GetItemInfo(int itemID)
        {
            ItemInfo retVal = null;
            try
            {
                retVal = _itemManager.GetItem(itemID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return retVal;
        }

        void CalculateTotals()
        {
            txtTaxAmount.Text = lblNetAmount.Text = txtTotalAmount.Text = "0.00";
            decimal totalAmount = 0;
            decimal totalTax = 0;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                decimal amt = Convert.ToDecimal(dr.Cells["Quantity"].Value) * Convert.ToDecimal(dr.Cells["Rate"].Value);
                dr.Cells["Amount"].Value = amt.ToString("0.00");
                totalAmount += amt;

                decimal tax = amt - ((amt * 100) / (100 + Convert.ToDecimal(dr.Cells["Vat"].Value)));
                totalTax += tax;
            }
            txtTotalAmount.Text = totalAmount.ToString("0.00");
            txtTaxAmount.Text = totalTax.ToString("0.00");
            lblNetAmount.Text = (totalAmount + totalTax).ToString("0.00");
        }

        private void btnShowBill_Click(object sender, EventArgs e)
        {
            //New();

            //CheckPortConnection()
            SerialPort _serialPort = new SerialPort("COM4");
            _serialPort.BaudRate = 115200;

            try
            {
                _serialPort.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Error opening port.");
            }

            //LoadKeySet()
            try
            {
                _serialPort.Write("L001" + key + "\r");
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading keyset.");
            }
            string abc = _serialPort.ReadLine();

            //ReadBlock()
            _serialPort.ReadExisting();

            string str = "R1001" + "02" + "\r";

            _serialPort.Write(str);
            string retVal = _serialPort.ReadLine();
            _serialPort.Close();
        }

        void Print()
        {
            //System.Drawing.Printing.PrinterSettings.StringCollection str = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
            //System.Drawing.Printing.PrinterSettings pSetting = new System.Drawing.Printing.PrinterSettings();
            //pSetting.PrinterName = "Epson 300+";

            //int i = 0;
            //foreach(System.Drawing.Printing.PaperSize p in pSetting.PaperSizes)
            //{
            //    i += 1;
            //    string n = p.PaperName;
            //}

            ReportClass rptClass = new ReportClass();
            rptClass.ShowBill(Convert.ToInt64(txtBillNo.Text), false, false);
        }

        void MessageText(string text, Color c)
        {
            lblMessages.BackColor = c;
            lblMessages.Text = text;
            lblMessages.Visible = true;
        }
    }
}