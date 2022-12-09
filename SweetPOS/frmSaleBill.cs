using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;

namespace SweetPOS
{
    public partial class frmSaleBill : Form
    {
        SaleManager _saleManager = new SaleManager();
        SaleItemManager _saleItemManager = new SaleItemManager();
        Timer tmrClock = new Timer();
        CardReadWrapper _reader = null;
        Timer tmrClearMessage = new Timer();

        bool showOnly = false;

        public frmSaleBill()
        {
            InitializeComponent();
        }

        private void frmSaleBill_Load(object sender, EventArgs e)
        {
            //int portNo = Convert.ToInt32(SweetPOS.Properties.Settings.Default.PortNumber);
            int portNo = 0;
            _reader = CardReadWrapper.GetInstance("ACR", portNo);
            tmrClearMessage.Interval = 2800;
            tmrClearMessage.Tick += new EventHandler(tmrClearMessage_Tick);
            tmrClock.Enabled = true;
            tmrClock.Interval = 100;
            tmrClock.Tick += new EventHandler(tmrClock_Tick);

            NewBill();
        }

        void tmrClock_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToLongTimeString().Trim();
        }

        void tmrClearMessage_Tick(object sender, EventArgs e)
        {
            lblMessages.Visible = false;
            tmrClearMessage.Enabled = false;
        }

        void ShowBill(int counterID)
        {
            grdItems.Rows.Clear();

            //Check whether card present
            string cardSerialNumber = string.Empty;
            try
            {
                cardSerialNumber = _reader.GetCardSerial();
            }
            catch
            {
                tmrClearMessage.Enabled = true;
                lblMessages.Visible = true;
                lblMessages.Text = "Card not found.";
                return;
            }

            BindingList<SaleItemInfo> saleItems = null;
            try
            {
                //saleItems = _saleItemManager.GetSaleItemsByCardAndCounter(cardSerialNumber.Trim(), counterID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Sale Items by CardNumber & Counter." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (saleItems.Count == 0)
            {
                tmrClearMessage.Enabled = true;
                lblMessages.Visible = true;
                lblMessages.Text = "Items not found for selected card.";
                return;
            }
            else
            {
                int count = 0;
                decimal total = 0;
                foreach (SaleItemInfo saleItem in saleItems)
                {
                    int row = grdItems.Rows.Add();

                    grdItems.Rows[row].Cells["SrNo"].Value = (row + 1).ToString();
                    grdItems.Rows[row].Cells["ItemID"].Value = saleItem.ItemID;
                    //grdItems.Rows[row].Cells["ItemName"].Value = saleItem.ItemName;
                    grdItems.Rows[row].Cells["Quantity"].Value = saleItem.Quantity;
                    grdItems.Rows[row].Cells["Rate"].Value = saleItem.Rate;
                    grdItems.Rows[row].Cells["Amount"].Value = saleItem.Amount;
                    //grdItems.Rows[row].Cells["Counter"].Value = saleItem.CounterID;
                    grdItems.Rows[row].Cells["SaleItemID"].Value = saleItem.ID;

                    count += 1;
                    total += Convert.ToDecimal(saleItem.Amount);
                }
                txtTotalAmount.Text = total.ToString("0.00");
                lblItemCount.Text = count.ToString().Trim() + "  Items";
                lblMessages.Visible = false;
            }
        }

        SaleInfo SetValue(out BindingList<SaleItemInfo> saleItems)
        {
            SaleInfo retVal = new SaleInfo();
            
            //retVal.SaleDate = dtDate.Value;
            //retVal.BillNumber = _saleManager.GetNextBillNumber(DateTime.Today.Date);
            retVal.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);

            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            BindingList<SaleItemInfo> items = new BindingList<SaleItemInfo>();
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                SaleItemInfo saleItem = new SaleItemInfo();
                saleItem.ID = Convert.ToInt32(dr.Cells["SaleItemID"].Value);
                saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                saleItem.Amount = Convert.ToDecimal(dr.Cells["Amount"].Value);
                saleItem.UpdatedBy = Program.CURRENTUSER;
                saleItem.UpdatedOn = DateTime.Now;

                items.Add(saleItem);
            }

            saleItems = items;
            return retVal;
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            if (!showOnly)
            {
                NewBill();
                ShowBill(0);
            }

            if (grdItems.Rows.Count == 0 || Convert.ToDecimal(txtTotalAmount.Text) == 0)
                return;

            BindingList<SaleItemInfo> saleItems = new BindingList<SaleItemInfo>();
            SaleInfo sale = SetValue(out saleItems);
            try
            {
                //_saleManager.AddSale(sale, saleItems);
                showOnly = false;
                //SweetPOS.ReportsClass reportClass = new ReportsClass();
                //reportClass.ShowBill(Convert.ToInt32(txtBillNumber.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale Bill." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void lblExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        void NewBill()
        {
            dtDate.Value = DateTime.Now.Date;
            //txtBillNumber.Text = _saleManager.GetNextBillNumber(DateTime.Today.Date).ToString();
            grdItems.Rows.Clear();
            txtTotalAmount.Text = "0.00";
            lblMessages.Visible = false;
            lblItemCount.Text = "0  Items";
            showOnly = false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            NewBill();
        }

        private void btnShowOnly_Click(object sender, EventArgs e)
        {
            ShowBill(0);
            showOnly = true;
        }

        private void grdItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                decimal d;
                if (!decimal.TryParse(e.FormattedValue.ToString(), out d))
                {
                    e.Cancel = true;
                }
                else
                {
                    grdItems.Rows[e.RowIndex].Cells["Amount"].Value = Convert.ToDecimal(e.FormattedValue) * Convert.ToDecimal(grdItems.Rows[e.RowIndex].Cells["Rate"].Value);
                }
            }
        }

        void CalculateTotal()
        {
            decimal itemCount = 0;
            decimal totalAmount = 0;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                itemCount += 1;
                totalAmount += Convert.ToDecimal(dr.Cells["Amount"].Value);
            }
            txtTotalAmount.Text = totalAmount.ToString("0.00");
            lblItemCount.Text = itemCount.ToString().Trim() + "  Items";
        }

        private void grdItems_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            grdItems.Rows[e.RowIndex].Cells["Quantity"].Value = Convert.ToDecimal(grdItems.Rows[e.RowIndex].Cells["Quantity"].Value).ToString("0.000");
            CalculateTotal();
        }
    }
}