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
    public partial class frmSaleRFIDMini : Form
    {
        #region Class level variables...

        SaleManager _saleManager = new SaleManager();
        ItemManager _itemManager = new ItemManager();

        int _newRecordID = 0;
        bool _newRecordAdded = false;

        #endregion

        Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
        string key = Program.KEYSET;

        public frmSaleRFIDMini()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - RFID Sale Entry (MINI)";
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

            retVal.BillDate = dtBillDate.Value.Date;
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

        bool Save()
        {
            if (!ValidateFields())
                return false;

            BindingList<SaleItemInfo> saleItems = null;
            SaleInfo sale = SetValue(out saleItems);

            try
            {
                //_newRecordID = _saleManager.AddSale(sale, saleItems, Program.CounterName);
                _newRecordAdded = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            label4.Text = "0";
            btnShowBill.Enabled = false;
            grdItems.Rows.Clear();
            txtTotalAmount.Text = "0.00";
            lblMessages.Visible = false;

            string data;
            try
            {
                data = c.ReadBlock(key, 1);

                MessageText("Reading card, please wait...", Color.DarkOliveGreen);

                int itemCount = 0;
                if (data.Trim().Length >= 17)
                {
                    int i;
                    if (!int.TryParse(data.Substring(15, 2), out i))
                    {
                        MessageText("Invalid data.", Color.Blue);
                        btnShowBill.Enabled = true;
                        return;
                    }
                    else
                        itemCount = Convert.ToInt32(data.Substring(15, 2));

                    if (itemCount == 0)
                    {
                        MessageText("No items.", Color.Green);
                        btnShowBill.Enabled = true;
                        return;
                    }
                }
                else
                {
                    MessageText("Bad or no card.", Color.Red);
                    btnShowBill.Enabled = true;
                    return;
                }

                BindingList<ReadItemInfo> readItems = c.ReadItems(key, 10);

                //if (readItems.Count != itemCount)
                //{
                //    MessageText("Missing items.", Color.Blue);
                //    return;
                //}

                bool goAhead = true;

                //foreach (ReadItemInfo uten in readItems)
                //{
                //    int row = grdItems.Rows.Add();
                //    grdItems.Rows[row].Cells["ItemID"].Value = uten.Data;
                //}

                foreach (ReadItemInfo item in readItems)
                {
                    if (item.Data.Trim().Length < 15)
                    {
                        MessageText("Invalid data in items. (" + item.Data.Trim() + ")", Color.Blue);
                        goAhead = false;
                        btnShowBill.Enabled = true;
                        break;
                    }

                    int itemId;
                    if (!int.TryParse(item.Data.Substring(5, 3), out itemId))
                    {
                        MessageText("Invalid item code. (" + item.Data.Substring(5, 3) + ")", Color.Blue);
                        goAhead = false;
                        btnShowBill.Enabled = true;
                        break;
                    }

                    decimal qty;
                    if (!decimal.TryParse(item.Data.Substring(8, 7), out qty))
                    {
                        MessageText("Invalid quantity. (" + item.Data.Substring(8, 7) + ")", Color.Blue);
                        goAhead = false;
                        btnShowBill.Enabled = true;
                        break;
                    }

                    ItemInfo i = _itemManager.GetItem(itemId);

                    int row = grdItems.Rows.Add();
                    grdItems.Rows[row].Cells["SrNo"].Value = (row + 1).ToString();
                    grdItems.Rows[row].Cells["ItemID"].Value = i.ID;
                    grdItems.Rows[row].Cells["ItemName"].Value = i.Name;
                    grdItems.Rows[row].Cells["Quantity"].Value = qty;
                    grdItems.Rows[row].Cells["UnitID"].Value = i.UnitID;
                    grdItems.Rows[row].Cells["Unit"].Value = i.Unit;
                    grdItems.Rows[row].Cells["Gst"].Value = i.Gst;
                    grdItems.Rows[row].Cells["Rate"].Value = i.Rate;
                    grdItems.Rows[row].Cells["Amount"].Value = (qty * i.Rate).ToString("0.00");

                    lblMessages.Visible = false;
                }

                CalculateTotals();

                if (!goAhead)
                {
                    btnShowBill.Enabled = true;
                    return;
                }

                string temp = string.Empty;
                if (!c.ResetItems(key, out temp, 0))
                    MessageBox.Show("Card initialization failed... Keep card beside.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                {
                    //Save & Print logic
                    Save();
                    Print();
                }

                btnShowBill.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                btnShowBill.Enabled = true;
                return;
            }
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