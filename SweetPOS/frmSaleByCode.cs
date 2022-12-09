using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;
using System.IO;
using ChitalePersonalzer;

namespace SweetPOS
{
    public partial class frmSaleByCode : Form
    {
        #region Class level variable declaration...

        ItemCompanyManager _itemCompanyManager = new ItemCompanyManager();
        ItemGroupManager _itemGroupManager = new ItemGroupManager();
        ItemManager _itemManager = new ItemManager();
        SaleItemManager _saleItemManager = new SaleItemManager();
        SaleManager _saleManager = new SaleManager();
        SettingManager _settingManager = new SettingManager();

        Timer tmrClearMessage = new Timer();
        Timer tmrClock = new Timer();

        int grpPageNo = 1;
        int grpPageCount = 5;
        int itmPageNo = 1;
        int itmPageCount = 25;
        int companyPageNo = 1;
        int companyPageCount = 7;

        int lastClickedCompanyID = 0;
        int lastClickedGroupID = 0;

        Padding _padding = new Padding(2);
        bool _fillingMixTopli = false;

        Int16 _amtPercent = 0;
        decimal _discountAmountPercent = 0;

        int _newRecordID = 0;
        string _cardID = string.Empty;

        Communication _c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
        string _key = Program.KEYSET;
        string _barCodeToReadWriteCard = string.Empty;

        bool _allowAddOnlyAfterRetrive;
        bool _showItemMultiple;
        bool _showShowFromCardButton;

        int _billNumber = 0;

        //For round off amount & Animation
        bool _roundAmount = false;
        decimal _round50 = 0;
        decimal _round1 = 0;
        bool _saveAndprint = false;
        bool _modifyRate = false;

        #endregion

        public frmSaleByCode()
        {
            InitializeComponent();
        }

        private void frmSale_Load(object sender, EventArgs e)
        {
            this.Text = Program.MESSAGEBOXTITLE + " - Sales Transactions";

            SetQuantityPadNumericPadPositionSize();

            tmrClearMessage.Interval = 1400;
            tmrClearMessage.Tick += new EventHandler(tmrClearMessage_Tick);
            tmrClock.Interval = 500;
            tmrClock.Enabled = true;
            tmrClock.Tick += new EventHandler(tmrClock_Tick);

            try
            {
                if (_settingManager.GetSetting(26) == "True")
                    _barCodeToReadWriteCard = _settingManager.GetSetting(27);

                _allowAddOnlyAfterRetrive = _settingManager.GetSetting(28) == "True" ? true : false;
                _showItemMultiple = _settingManager.GetSetting(29) == "True" ? true : false;
                _showShowFromCardButton = btnShowFromCard.Visible = _settingManager.GetSetting(30) == "True" ? true : false;

                _roundAmount = Convert.ToBoolean(_settingManager.GetSetting(7));         //Hardcode

                _round50 = Convert.ToDecimal(_settingManager.GetSetting(8));            //Hardcode

                _round1 = Convert.ToDecimal(_settingManager.GetSetting(9));             //Hardcode

                _saveAndprint = Convert.ToBoolean(_settingManager.GetSetting(32));             //Hardcode

                _modifyRate = Convert.ToBoolean(_settingManager.GetSetting(33));             //Hardcode

                if (_saveAndprint)
                {
                    btnWriteToCard.Text = "Save && Print";
                    lblBillNumber.Visible = txtBillNumber.Visible = true;
                }
                else
                {
                    btnWriteToCard.Text = "Save to Card";
                    lblBillNumber.Visible = txtBillNumber.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting bar code setting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ShowSearchByName();
            NewBill();
            txtCode.Focus();
        }

        private void SetQuantityPadNumericPadPositionSize()
        {
            Size s = pnlRightMain.Size;

            //tlpQuantityPad.Height = tlpNumericPad.Height = (s.Height / 2);
            //tlpQuantityPad.Top = 0;
            //tlpNumericPad.Top = tlpQuantityPad.Height;

            tlpQuantityPad.Height = 90;
            tlpNumericPad.Height = (s.Height / 2);
            tlpQuantityPad.Top = 284;
            tlpNumericPad.Top = tlpQuantityPad.Height + 284;
            pnlAmountForQuantity.Width = s.Width;
            pnlAmountForQuantity.Height = 290;
            pnlAmountForQuantity.Top = pnlAmountForQuantity.Left = 0;

            pnlSearchItemByName.Height = tlpNumericPad.Height = (s.Height / 2);
            pnlSearchItemByName.Top = 0;

            tlpQuantityPad.Width = tlpNumericPad.Width = pnlSearchItemByName.Width = s.Width;
            tlpQuantityPad.Left = tlpNumericPad.Left = pnlSearchItemByName.Left = 0;

            lblEnterRate.Top = s.Height / 4;
            lblEnterRate.Left = (tlpNumericPad.Width / 2) - (lblEnterRate.Width / 2);
        }

        void tmrClock_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.Date.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();

            if (_showShowFromCardButton)
            {
                if (grdItems.Rows.Count > 0)
                    btnShowFromCard.Visible = false;
                else
                    btnShowFromCard.Visible = true;
            }
        }

        void tmrClearMessage_Tick(object sender, EventArgs e)
        {
            lblMessages.Visible = false;
            tmrClearMessage.Enabled = false;
        }

        void NewBill()
        {
            pnlRightMain.SuspendLayout();

            if (Convert.ToDecimal(lblTotalAmount.Text) > 0)
                lblLastBillAmount.Text = "Last Bill Amt:   " + lblTotalAmount.Text;

            try
            {
                txtBillNumber.Text = _saleManager.GetNextBillNumber().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting max bill number." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnSave.Text = "Save Bill";
            btnPrint.Text = "Print Bill";

            itmPageNo = grpPageNo = 1;
            grdItems.Columns["Counter"].Visible = false;
            grdItems.Rows.Clear();
            lblTotalAmount.Text = "0.00";
            lblCode.Text = "Code:";
            tlpNumericPad.BackColor = Color.Teal;
            lblEnterRate.Visible = false;
            txtCode.Text = string.Empty;
            grdItems.Columns[1].HeaderText = "0 Items";

            btnSave.Enabled = true;
            _fillingMixTopli = pnlMixTopli.Visible = false;
            pnlMixTopli.Width = 377;
            btnHideQty.Enabled = true;

            _amtPercent = 0;
            _discountAmountPercent = 0;
            lblTotalAmount.Tag = null;
            lblDiscountDetails.Visible = false;
            if (_allowAddOnlyAfterRetrive)
                pnlSearchItemByName.Enabled = tlpNumericPad.Enabled = btnWriteToCard.Enabled = false;
            tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = false;
            tlpNumericPad.Visible = true;

            ReShowSearchByName();

            pnlRightMain.ResumeLayout(true);
            txtCode.Focus();
        }

        private void ReShowSearchByName()
        {
            pnlSearchItemByName.Visible = true;
            ((TextBox)((ctlSearchItemByName)pnlSearchItemByName.Controls.Find("ctlSearchItemByName", true)[0]).Controls.Find("txtItemName", true)[0]).Text = string.Empty;
            ((ctlSearchItemByName)pnlSearchItemByName.Controls.Find("ctlSearchItemByName", true)[0]).SearchItems();
        }

        private void ShowSearchByName()
        {
            ctlSearchItemByName ctl = new ctlSearchItemByName();

            ctl.Dock = DockStyle.Fill;
            pnlSearchItemByName.SuspendLayout();
            pnlSearchItemByName.Controls.Add(ctl);
            ctl.SearchItems();
            ctl.Focus();
            pnlSearchItemByName.ResumeLayout();
        }

        #region Groups & Items (Load & Click)

        private void MessageText(string message, bool successMessage)
        {
            lblMessages.BackColor = Color.Yellow;
            lblMessages.ForeColor = Color.Red;
            tmrClearMessage.Enabled = true;
            lblMessages.BringToFront();
            if (successMessage)
            {
                lblMessages.BackColor = Color.Green;
                lblMessages.ForeColor = Color.White;
            }

            lblMessages.Visible = true;
            lblMessages.Text = message;

            if (tlpNumericPad.Enabled)
                btnWriteToCard.Enabled = true;

            txtCode.Focus();
        }

        bool SearchIfExist(ItemInfo item)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToInt32(dr.Cells["ItemID"].Value) == item.ID)
                {
                    if (item.UnitID == 2)
                        dr.Cells["Quantity"].Value = Convert.ToDecimal(dr.Cells["Quantity"].Value) + 1;

                    grdItems.Rows[dr.Index].Selected = true;
                    grdItems.CurrentCell = grdItems[1, dr.Index];
                    CalculateTotal();
                    txtCode.Text = string.Empty;
                    txtCode.Focus();

                    return true;
                }
            }
            return false;
        }

        bool SearchIfExistInMix(ItemInfo item)
        {
            foreach (DataGridViewRow dr in grdMixTopli.Rows)
            {
                if (Convert.ToInt32(dr.Cells["MixItemID"].Value) == item.ID)
                {
                    grdMixTopli.Rows[dr.Index].Selected = true;
                    grdMixTopli.CurrentCell = grdMixTopli[1, dr.Index];

                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Quantity & Numeric Pad Click Events

        private void QuantityPadClick(object sender, EventArgs e)
        {
            if (grdItems.SelectedRows.Count > 0)
            {
                grdItems.SelectedRows[0].Cells["Quantity"].Value = Convert.ToDecimal(Convert.ToInt32(((Button)sender).Tag) / 1000M);
                grdItems.Focus();
                CalculateTotal();
            }
            if (_modifyRate)
            {
                if (lblCode.Text != "Rate:")
                {
                    lblCode.Text = "Rate:";
                    tlpNumericPad.BackColor = Color.Red;
                    lblEnterRate.Visible = true;
                    tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = false;
                }
            }
            else
                ResetForNewItem();

            lblMessages.Visible = false;
        }

        private void NumericPadClick(object sender, EventArgs e)
        {
            txtCode.Text += ((Button)sender).Text;
            txtCode.Select(txtCode.Text.Length, 0);

            if (txtCode.Text.StartsWith(".") && txtCode.Text.Length == 4)
                btnCode_Click(sender, e);

            if (txtCode.Text.Trim().Length > 1 && txtCode.Text.Substring(1, 1) == "." && txtCode.Text.Trim().Length == 5)
                btnCode_Click(sender, e);

            if (txtCode.Text.Trim().Length > 2 && txtCode.Text.Substring(2, 1) == "." && txtCode.Text.Trim().Length == 6)
                btnCode_Click(sender, e);

            lblMessages.Visible = false;
        }

        private void btnHideQty_Click(object sender, EventArgs e)
        {
            if (!grdMixTopli.Visible)
            {
                if (grdItems.Rows.Count > 0)
                {
                    tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = !tlpQuantityPad.Visible;
                }
            }
            else
            {
                if (grdItems.Rows.Count > 0)
                {
                    tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = !tlpQuantityPad.Visible;
                }
            }

            if (pnlAmountForQuantity.Visible == true)
                txtAmountForQuantity.Text = string.Empty;

            txtCode.Text = string.Empty;
            lblCode.Text = tlpQuantityPad.Visible ? "Qty:" : lblCode.Text = "Code:";
            pnlSearchItemByName.Visible = !tlpQuantityPad.Visible;
            tlpNumericPad.BackColor = Color.Teal;
            lblEnterRate.Visible = false;
            txtCode.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtCode.Text = string.Empty;
            txtCode.Focus();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            btnCode_Click(sender, e);
            txtCode.Focus();
        }

        #endregion

        void CalculateTotal()
        {
            decimal totalAmount = 0;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (dr.Cells["QuantityByAmount"].Value == null || dr.Cells["QuantityByAmount"].Value.ToString() != "1")
                    dr.Cells["Amount"].Value = Convert.ToDecimal(dr.Cells["Quantity"].Value) * Convert.ToDecimal(dr.Cells["Rate"].Value);
                totalAmount += Convert.ToDecimal(dr.Cells["Amount"].Value);
            }

            if (_discountAmountPercent == 0)
                lblTotalAmount.Text = totalAmount.ToString("0.00");
            else
            {
                if (_amtPercent == 1)
                    lblTotalAmount.Text = (totalAmount - _discountAmountPercent).ToString("0.00");
                else if (_amtPercent == 2)
                    lblTotalAmount.Text = (totalAmount - ((totalAmount * _discountAmountPercent) / 100)).ToString("0.00");
            }

            if (_roundAmount)
                lblTotalAmount.Text = Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero).ToString("0.00");

            lblTotalAmount.Tag = totalAmount;
            grdItems.Columns[1].HeaderText = grdItems.Rows.Count == 1 ? grdItems.Rows.Count.ToString() + " Item" : grdItems.Rows.Count.ToString() + " Items";
        }

        void ResetForNewItem()
        {
            //tlpQuantityPad.Visible = tlpNumericPad.Visible = false;
            tlpNumericPad.Visible = true;
            tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = false;
            txtCode.Text = string.Empty;
            lblCode.Text = "Code:";
            tlpNumericPad.BackColor = Color.Teal;
            lblEnterRate.Visible = false;
            CalculateTotal();
            txtCode.Focus();

            ReShowSearchByName();
        }

        #region Button Click Events

        private void btnNewBill_Click(object sender, EventArgs e)
        {
            NewBill();
            lblMessages.Visible = false;
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            if (_fillingMixTopli)
            {
                if (grdMixTopli.SelectedRows.Count > 0)
                {
                    int deleteRowIndex = grdMixTopli.SelectedRows[0].Index;
                    grdMixTopli.Rows.RemoveAt(deleteRowIndex);

                    if (grdMixTopli.Rows.Count > 0)
                    {
                        if (grdMixTopli.Rows.Count == deleteRowIndex)
                        {
                            grdMixTopli.Rows[deleteRowIndex - 1].Selected = true;
                        }
                        else
                        {
                            grdMixTopli.Rows[deleteRowIndex].Selected = true;
                        }
                    }

                    CalculateAverageRate();
                }
                return;
            }

            if (grdItems.SelectedRows.Count > 0)
            {
                int deleteRowIndex = grdItems.SelectedRows[0].Index;
                grdItems.Rows.RemoveAt(deleteRowIndex);

                if (grdItems.Rows.Count > 0)
                {
                    if (grdItems.Rows.Count == deleteRowIndex)
                    {
                        grdItems.Rows[deleteRowIndex - 1].Selected = true;
                    }
                    else
                    {
                        grdItems.Rows[deleteRowIndex].Selected = true;
                    }
                }

                ResetForNewItem();
            }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnCode_Click(sender, e);
            }
        }

        void ItemClickEvent(object sender, EventArgs e)
        {
            if (_fillingMixTopli)
            {
                FillMixTopli(sender, e);
                return;
            }

            if (grdItems.Rows.Count > 0)
            {
                if (Convert.ToDecimal(grdItems.SelectedRows[0].Cells["Quantity"].Value) == 0)
                {
                    tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = true;
                    txtAmountForQuantity.Text = string.Empty;
                    pnlSearchItemByName.Visible = false;
                    MessageText("Enter quantity for selected item.", false);
                    txtCode.Text = string.Empty;
                    return;
                }
            }

            ItemInfo item = null;
            try
            {
                if (sender.GetType().Name == "Button")
                {
                    item = _itemManager.GetItemByItemCode(Convert.ToInt32(((Button)sender).Tag));
                }
                else
                {
                    int i = 0;
                    if (int.TryParse(sender.ToString(), out i))
                    {
                        item = _itemManager.GetItemByItemCode(Convert.ToInt32(sender.ToString()));
                    }
                    else
                    {
                        item = (ItemInfo)sender;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item by Id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_showItemMultiple)
            {
                if (SearchIfExist(item))
                    return;
            }

            if (item.Name == string.Empty)
            {
                MessageText("Item not found.", false);
                txtCode.Text = string.Empty;
                return;
            }

            int row = grdItems.Rows.Add();
            grdItems.Rows[row].Cells["ItemID"].Value = item.ID;
            grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
            grdItems.Rows[row].Cells["ItemGroupID"].Value = item.ItemGroupID;
            grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
            grdItems.Rows[row].Cells["Rate"].Value = item.Rate;
            grdItems.Rows[row].Cells["UnitID"].Value = item.UnitID;
            grdItems.Rows[row].Cells["Gst"].Value = item.Gst;

            //if (item.UnitID == 2)  //Hardcode (No.)
            //{
            //    grdItems.Rows[row].Cells["Quantity"].Value = 1;
            //    txtCode.Text = string.Empty;
            //}
            //else
            //{
            tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = tlpNumericPad.Visible = true;
            txtAmountForQuantity.Text = string.Empty;
            pnlSearchItemByName.Visible = false;
                txtCode.Text = string.Empty;
                lblCode.Text = "Qty:";
                //txtCode.Text = "1";
                tlpNumericPad.BackColor = Color.Teal;
                lblEnterRate.Visible = false;
            //}

            grdItems.Rows[row].Selected = true;
            grdItems.CurrentCell = grdItems[1, row];

            CalculateTotal();

            lblMessages.Visible = false;
            txtCode.Focus();
        }

        private void btnCode_Click(object sender, EventArgs e)
        {
            if (_modifyRate)
            {
                if (lblCode.Text == "Code:")
                {
                    ShowItemByCodeORName(false);
                }
                else if (lblCode.Text != "Rate:")
                {
                    if (ChangeQuantity(false))
                    {
                        lblCode.Text = "Rate:";
                        tlpNumericPad.BackColor = Color.Red;
                        lblEnterRate.Visible = true;
                        tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = false;
                        txtCode.Text = string.Empty;
                    }
                }
                else
                {
                    if (grdItems.SelectedRows.Count > 0 && lblCode.Text == "Rate:")
                    {
                        ChangeRate();
                    }
                }

                return;
            }

            if (_barCodeToReadWriteCard != string.Empty && txtCode.Text == _barCodeToReadWriteCard)
            {
                //if (!tlpMostUsedItems.Enabled)
                //{
                //    btnShowFromCard_Click(sender, e);
                //}
                //else
                //{
                //    btnWriteToCard_Click(sender, e);
                //}
                txtCode.Text = string.Empty;
                return;
            }
            if (grdItems.SelectedRows.Count > 0 && lblCode.Text == "Qty:")
            {
                ChangeQuantity(true);
            }
            else
            {
                if (lblCode.Text == "Code:")
                {
                    ShowItemByCodeORName(false);
                }
                else
                {
                    MessageText("First click on Show Items from Card button.", false);
                    txtCode.SelectAll();
                }
            }
            txtCode.Focus();
        }

        private void ShowItemByCodeORName(bool Name)
        {
            if (btnSave.Text == "Save Coupon")
            {
                txtCode.Text = string.Empty;
                txtCode.Focus();
                return;
            }

            if (txtCode.Text.Trim().Length == 0)
            {
                txtCode.Focus();
                return;
            }

            EventArgs e = new EventArgs();
            int i = 0;
            if (txtCode.Text.Trim().Length <= 4 && int.TryParse(txtCode.Text, out i))   //Hardcode
            {
                ItemClickEvent((object)txtCode.Text, e);
            }
            else
            {
                ItemInfo item = _itemManager.GetItemByBarCode(txtCode.Text.Trim());
                if (item.Name != string.Empty)
                {
                    ItemClickEvent((object)item, e);
                }
                else
                {
                    MessageText("Item not found.", false);
                    txtCode.SelectAll();
                }
            }
        }

        private bool ChangeQuantity(bool resetForNewItem)
        {
            if (txtCode.Text.Trim().Length > 7)
            {
                MessageText("Enter valid quantity.", false);
                return false;
            }
            else
            {
                decimal qty;
                if (decimal.TryParse(txtCode.Text, out qty))
                {
                    grdItems.SelectedRows[0].Cells["Quantity"].Value = qty;

                    if (resetForNewItem)
                    {
                        ResetForNewItem();
                    }
                    else
                        CalculateTotal();
                }
                else
                {
                    if (!decimal.TryParse(Convert.ToString(grdItems.SelectedRows[0].Cells["Quantity"].Value), out qty))
                    {
                        MessageText("Enter valid quantity.", false);
                        return false;
                    }
                }
            }

            return true;
        }

        private void ChangeRate()
        {
            if (txtCode.Text.Trim().Length > 7)
            {
                MessageText("Enter valid rate.", false);
            }
            else
            {
                decimal rate;
                if (decimal.TryParse(txtCode.Text, out rate))
                {
                    grdItems.SelectedRows[0].Cells["Rate"].Value = rate;
                    ResetForNewItem();
                }
                else if (txtCode.Text.Trim() == string.Empty)
                {
                    ResetForNewItem();
                }
                else
                {
                    MessageText("Enter valid rate.", false);
                }
            }
        }

        #endregion

        BindingList<SaleItemInfo> SetValue(bool isPrint, out SaleInfo sales)
        {
            SaleInfo sale = new SaleInfo();

            sale.BillDate = DateTime.Today.Date;
            sale.BillNo = Convert.ToString(txtBillNumber.Text);
            sale.CashCredit = 0;    //Hardcode
            sale.TotalAmount = Convert.ToDecimal(lblTotalAmount.Tag);
            sale.NetAmount = Convert.ToDecimal(lblTotalAmount.Text);
            sale.IsPrint = isPrint;

            if (_discountAmountPercent > 0)
            {
                if (_amtPercent == 1)
                {
                    sale.DiscountPercentage = (_discountAmountPercent * 100) / Convert.ToDecimal(lblTotalAmount.Tag);
                    sale.DiscountAmount = _discountAmountPercent;
                }
                else if (_amtPercent == 2)
                {
                    sale.DiscountPercentage = _discountAmountPercent;
                    sale.DiscountAmount = Convert.ToDecimal(lblTotalAmount.Tag) * _discountAmountPercent / 100;
                }
            }
            sale.RFIDTransaction = false;
            sale.CreatedBy = Program.CURRENTUSER;
            sale.CreatedOn = DateTime.Now;
            sale.UpdatedBy = Program.CURRENTUSER;
            sale.UpdatedOn = DateTime.Now;

            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                SaleItemInfo saleItem = new SaleItemInfo();
                saleItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
                saleItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                saleItem.Vat = Convert.ToDecimal(dr.Cells["Vat"].Value);
                saleItem.Amount = Convert.ToDecimal(dr.Cells["Amount"].Value);

                retVal.Add(saleItem);
            }

            sales = sale;
            return retVal;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePrintBill(false, sender, e);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            SavePrintBill(true, sender, e);
        }

        private void SavePrintBill(bool isPrint, object sender, EventArgs e)
        {
            //Check whether items present in the grid
            if (grdItems.Rows.Count == 0)
            {
                MessageText("No items found.", false);
                return;
            }

            //Check for zero quantity
            bool validationFailed = false;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToDecimal(dr.Cells["Amount"].Value) == 0)
                {
                    dr.Selected = true;
                    validationFailed = true;
                    break;
                }
            }
            if (validationFailed)
            {
                MessageText("Quantity not entered.", false);
                return;
            }
            SaleInfo sale = new SaleInfo();
            BindingList<SaleItemInfo> saleItems = SetValue(isPrint, out sale);
            try
            {
                Int64 billNumber = 0;

                _newRecordID = _saleManager.AddSale(sale, saleItems, Program.CounterName, out billNumber);
                
                if (isPrint && saleItems.Count > 0)
                {
                    ReportClass rptClass = new ReportClass();
                    rptClass.ShowBill(billNumber, false, _discountAmountPercent > 0);
                }
                txtCode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale Items." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageText("Data written successfully...", true);
            lblLastBillAmount.Text = "Last Bill Amount:   " + lblTotalAmount.Text;

            btnNewBill_Click(sender, e);
        }

        private void lblExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnBackToRecent_Click(object sender, EventArgs e)
        {
            itmPageNo = 1;
            //LoadMostUsedItems();
        }

        #region Topli...

        void FillMixTopli(object sender, EventArgs e)
        {
            ItemInfo item = null;
            try
            {
                if (sender.GetType().Name == "Button")
                {
                    item = _itemManager.GetItemByItemCode(Convert.ToInt32(((Button)sender).Tag));
                }
                else
                {
                    int i = 0;
                    if (int.TryParse(sender.ToString(), out i))
                    {
                        item = _itemManager.GetItem(Convert.ToInt32(sender.ToString()));
                    }
                    else
                    {
                        item = (ItemInfo)sender;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item by Id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (SearchIfExistInMix(item))
                return;

            if (item.Name == string.Empty)
            {
                MessageText("Item not found.", false);
                txtCode.Text = string.Empty;
                return;
            }

            int row = grdMixTopli.Rows.Add();
            grdMixTopli.Rows[row].Cells["MixItemID"].Value = item.ID;
            grdMixTopli.Rows[row].Cells["MixItemGroupID"].Value = item.ItemGroupID;
            grdMixTopli.Rows[row].Cells["MixItemName"].Value = item.Name;
            grdMixTopli.Rows[row].Cells["MixRate"].Value = item.Rate;

            grdMixTopli.Rows[row].Selected = true;
            grdMixTopli.CurrentCell = grdMixTopli[1, row];

            CalculateAverageRate();

            lblMessages.Visible = false;
        }

        void CalculateAverageRate()
        {
            if (grdMixTopli.Rows.Count > 0)
            {
                decimal total = 0;
                int cnt = 0;
                foreach (DataGridViewRow dr in grdMixTopli.Rows)
                {
                    cnt += 1;
                    total += Convert.ToDecimal(dr.Cells["MixRate"].Value);
                }
                lblAverageRate.Text = (total / cnt).ToString("0.00");
            }
            else
                lblAverageRate.Text = "0.00";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _fillingMixTopli = false;
            pnlMixTopli.Visible = false;
            btnHideQty.Enabled = true;
            if (grdMixTopli.Rows.Count > 0)
            {
                int row = grdItems.Rows.Add();
                grdItems.Rows[row].Cells["ItemID"].Value = 107;   //Hardcode
                grdItems.Rows[row].Cells["ItemName"].Value = "Mix Dryfruit Mawa";   //Hardcode
                grdItems.Rows[row].Cells["Rate"].Value = lblAverageRate.Text;
                grdItems.Rows[row].Cells["Amount"].Value = "0.00";

                tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = true;
                txtAmountForQuantity.Text = string.Empty;
                pnlSearchItemByName.Visible = false;
                txtCode.Text = string.Empty;
                lblCode.Text = "Qty:";
                tlpNumericPad.BackColor = Color.Teal;
                lblEnterRate.Visible = false;
                grdItems.Rows[row].Selected = true;
                grdItems.CurrentCell = grdItems[1, row];
            }
        }

        #endregion

        private void btnGroupPrevious_Click(object sender, EventArgs e)
        {
            grpPageNo -= 1;
            //LoadGroups(lastClickedCompanyID);
        }

        private void btnGroupNext_Click(object sender, EventArgs e)
        {
            grpPageNo += 1;
            //LoadGroups(lastClickedCompanyID);
        }

        private void btnItemPrevious_Click(object sender, EventArgs e)
        {
            itmPageNo -= 1;
            //LoadItemsByGroupID(lastClickedGroupID);
        }

        private void btnItemNext_Click(object sender, EventArgs e)
        {
            itmPageNo += 1;
            //LoadItemsByGroupID(lastClickedGroupID);
        }

        private void btnWriteToCard_Click(object sender, EventArgs e)
        {
            #region New for Direct Print (Without RFID Card)

            //Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
            //bool isBlankCard = false;
            //string temp = string.Empty;
            //if (Save(out isBlankCard))
            //{
            //    MessageText("Saved successfully", true);
            //    Print();
            //    _billNumber = 0;
            //}
            //else
            //{
            //    MessageText("Error in saving.", false);
            //}
            //NewBill();

            #endregion

            if (_saveAndprint)
            {
                #region New for Direct Print

                Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
                bool isBlankCard = false;
                Int64 billNumber = 0;
                string temp = string.Empty;
                if (Save(out isBlankCard, out billNumber))
                {
                    MessageText("Saved successfully", true);
                    Print(billNumber);
                    _billNumber = 0;
                    if ((Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID) && !c.ResetItems(Program.KEYSET, out temp, 0))
                    {
                        MessageBox.Show("Show card again. Items not cleared in the card.");
                    }
                }
                //else if (isBlankCard)
                //{
                //    MessageText("Blank Card.", true);
                //    if (!c.ResetItems(Program.KEYSET, out temp, 0))
                //    {
                //        MessageBox.Show("Show card again. Items not cleared in the card.");
                //    }
                //}
                else
                {
                    MessageText("Error in saving.", false);
                }
                NewBill();

                #endregion
            }
            else
            {
                #region Old Save to Card

                Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
                string key = Program.KEYSET;
                btnWriteToCard.Enabled = false;

                Application.DoEvents();

                try
                {
                    if (_allowAddOnlyAfterRetrive && c.ReadCardID() != _cardID)
                    {
                        MessageText("Not the same card.", false);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageText(ex.Message, false);
                }
                try
                {
                    int itemCount = 0;
                    string data = c.ReadBlock(key, 1);
                    Application.DoEvents();
                    if (data.Trim().Substring(0, 1) == "1")
                    {
                        MessageText("Card not found.", false);
                        return;
                    }
                    else if (data.Trim().Length < 17)
                    {
                        MessageText("Invalid card.", false);
                        return;
                    }
                    else if (!_allowAddOnlyAfterRetrive)
                        itemCount = Convert.ToInt32(data.Substring(15, 2));

                    List<SaleItemInfo> saleItems = new List<SaleItemInfo>();

                    foreach (DataGridViewRow dr in grdItems.Rows)
                    {
                        SaleItemInfo saleItem = new SaleItemInfo();
                        saleItem.ItemID = Convert.ToInt32(dr.Cells["ItemCode"].Value);
                        saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                        saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);

                        saleItems.Add(saleItem);
                    }

                    int failedReason = 0;
                    if (c.WriteItems(Program.KEYSET, saleItems, itemCount, out failedReason))
                    {
                        if (failedReason == 2)
                        {
                            MessageText("Items saved successfully...", true);
                            lblLastBillAmount.Text = "Last Bill Amount:   " + lblTotalAmount.Text;
                            NewBill();
                        }
                        else
                        {
                            MessageText("Unexpected Response.", false);
                        }
                    }
                    else
                    {
                        if (failedReason == 1)
                        {
                            MessageText("Error occured while saving Items. 1", false);
                        }
                        else if (failedReason == 3)
                        {
                            MessageText("Error occured while saving Item Count. 3", false);
                        }
                        else
                        {
                            MessageText("Write failed.... Please try again.", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageText(ex.Message, false);
                }
                return;

                #endregion
            }
        }

        SaleInfo SetValue(out BindingList<SaleItemInfo> saleItems)
        {
            SaleInfo retVal = new SaleInfo();
            BindingList<SaleItemInfo> saleItemss = new BindingList<SaleItemInfo>();

            retVal.BillDate = DateTime.Today;
            try
            {
                retVal.BillNo = _saleManager.GetNextBillNumber().ToString();
                _billNumber = Convert.ToInt32(retVal.BillNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting bill number." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                saleItems = null;
                return null;
            }
            retVal.CashCredit = 0;

            retVal.TotalAmount = Convert.ToDecimal(lblTotalAmount.Text);
            retVal.DiscountPercentage = 0;
            retVal.DiscountAmount = 0;
            retVal.NetAmount = Convert.ToDecimal(lblTotalAmount.Text);
            retVal.RoundedAmount = Convert.ToDecimal(lblTotalAmount.Text);

            retVal.BalanceAmount = 0;
            retVal.CustomerID = 0;
            retVal.Description = string.Empty;
            retVal.RFIDTransaction = true;
            retVal.IsPrint = true;
            retVal.TotalWeight = retVal.ActualWeight = 0;

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

        bool Save(out bool isBlankCard, out Int64 billNo)
        {
            if (grdItems.Rows.Count == 0)
            {
                isBlankCard = true;
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

                isBlankCard = false; 
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                isBlankCard = false;
                billNo = 0;
                return false;
            }
        }

        void Print(Int64 billNumber)
        {
            ReportClass rptClass = new ReportClass();
            if (Program.SHOWSHORTBILL)
            {
                rptClass.ShowBillChitale(billNumber, false, false);
            }
            else
            {
                rptClass.ShowBill(billNumber, false, false);
            }
        }

        private void txtCode_Click(object sender, EventArgs e)
        {
            if (grdItems.Rows.Count > 0)
            {
                if (lblCode.Text == "Code:")
                {
                    lblCode.Text = "Qty:";
                    tlpNumericPad.BackColor = Color.Teal;
                    lblEnterRate.Visible = false;
                    tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = tlpNumericPad.Visible = true;
                    txtAmountForQuantity.Text = string.Empty;
                    pnlSearchItemByName.Visible = false;
                }
                else if (Convert.ToString(grdItems.SelectedRows[0].Cells["Quantity"].Value).Trim() != string.Empty)
                {
                    lblCode.Text = "Code:";
                    tlpNumericPad.BackColor = Color.Teal;
                    lblEnterRate.Visible = false;
                    tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = false;
                    tlpNumericPad.Visible = true;
                }
            }
            else
            {
                tlpNumericPad.Visible = true;
                lblCode.Text = "Code:";
                tlpNumericPad.BackColor = Color.Teal;
                lblEnterRate.Visible = false;
            }
        }

        private void btnShowFromCard_Click(object sender, EventArgs e)
        {
            string data = string.Empty;
            try
            {
                btnShowFromCard.Text = "Reading...";
                btnShowFromCard.Enabled = false;
                Application.DoEvents();

                _cardID = _c.ReadCardID();

                data = _c.ReadBlock(_key, 1);
                Application.DoEvents();
                int itemCount = 0;
                if (data.Trim().Length >= 17)
                {
                    int i;
                    if (!int.TryParse(data.Substring(15, 2), out i))
                    {
                        MessageText("Invalid data.", false);
                        if (_showShowFromCardButton && !_allowAddOnlyAfterRetrive)
                        {
                            btnWriteToCard.Enabled = false;
                        }
                        return;
                    }
                    else
                        itemCount = Convert.ToInt32(data.Substring(15, 2));

                    if (itemCount == 0)
                    {
                        MessageText("No items found.", true);
                        pnlSearchItemByName.Enabled = tlpNumericPad.Enabled = btnWriteToCard.Enabled = true;
                        return;
                    }
                }
                else
                {
                    MessageText("Card not found.", false);
                    return;
                }

                BindingList<ReadItemInfo> readItems = _c.ReadItems(_key, itemCount);
                if (readItems.Count != itemCount)
                {
                    MessageText("Missing items.", false);
                    return;
                }

                bool goAhead = true;

                foreach (ReadItemInfo item in readItems)
                {
                    if (item.Data.Trim().Length < 15)
                    {
                        MessageText("Invalid data in items. (" + item.Data.Trim() + ")", false);
                        goAhead = false;
                        break;
                    }

                     int itemId;
                    decimal qty;
                    if (item.Data.Trim().Substring(20, 1) == "B")
                    {
                        if (!int.TryParse(item.Data.Substring(5, 4), out itemId))
                        {
                            MessageText("Invalid item code. (" + item.Data.Substring(5, 4).Trim() + ")", false);
                            goAhead = false;
                            break;
                        }

                        //if (!decimal.TryParse(item.Data.Substring(9, 7), out qty))
                        //{
                        //    MessageText("Invalid quantity. (" + item.Data.Substring(9, 7).Trim() + ")", false);
                        //    goAhead = false;
                        //    break;
                        //}
                        if (!decimal.TryParse(item.Data.Substring(9, 3) + "." + item.Data.Substring(12, 3), out qty))
                        {
                            MessageText("Invalid quantity. (" + item.Data.Substring(9, 6).Trim() + ")", false);
                            goAhead = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!int.TryParse(item.Data.Substring(5, 3), out itemId))
                        {
                            MessageText("Invalid item code. (" + item.Data.Substring(5, 3).Trim() + ")", false);
                            goAhead = false;
                            break;
                        }

                        if (!decimal.TryParse(item.Data.Substring(8, 7), out qty))
                        {
                            MessageText("Invalid quantity. (" + item.Data.Substring(8, 7).Trim() + ")", false);
                            goAhead = false;
                            break;
                        }
                    }

                    ItemInfo i = _itemManager.GetItemByItemCode(itemId);
                    int row = grdItems.Rows.Add();

                    if (i.Name.Trim() == string.Empty)
                    {
                        grdItems.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                        grdItems.Rows[row].Cells["ItemID"].Value = itemId.ToString();
                        grdItems.Rows[row].Cells["ItemCode"].Value = "0";
                        grdItems.Rows[row].Cells["ItemGroupID"].Value = "0";
                        grdItems.Rows[row].Cells["ItemName"].Value = "Invalid Item Code";
                        grdItems.Rows[row].Cells["Quantity"].Value = qty;
                        grdItems.Rows[row].Cells["Rate"].Value = "0.00";
                    }
                    else
                    {
                        grdItems.Rows[row].DefaultCellStyle.ForeColor = Color.Maroon;
                        grdItems.Rows[row].DefaultCellStyle.SelectionForeColor = Color.White;
                        grdItems.Rows[row].Cells["ItemID"].Value = i.ID;
                        grdItems.Rows[row].Cells["ItemCode"].Value = i.ItemCode;
                        grdItems.Rows[row].Cells["ItemGroupID"].Value = i.ItemGroupID;
                        grdItems.Rows[row].Cells["ItemName"].Value = i.Name;
                        grdItems.Rows[row].Cells["UnitID"].Value = i.UnitID;
                        grdItems.Rows[row].Cells["Quantity"].Value = qty.ToString("0.000");
                        if (item.Data.Length > 19 && Convert.ToInt32(item.Data.Trim().Substring(15, 4)) > 0)
                        {
                            grdItems.Rows[row].Cells["Rate"].Value = Convert.ToDecimal(item.Data.Trim().Substring(15, 4) + "." + item.Data.Trim().Substring(19, 1)).ToString("0.00");
                        }
                        else
                        {
                            grdItems.Rows[row].Cells["Rate"].Value = i.Rate.ToString("0.00");
                        }
                        grdItems.Rows[row].Cells["Gst"].Value = i.Gst;
                    }

                    lblMessages.Visible = false;
                }

                if (_showShowFromCardButton && !_allowAddOnlyAfterRetrive)
                {
                    btnWriteToCard.Enabled = false;
                }
                else
                    btnWriteToCard.Enabled = true;

                if (!goAhead)
                {
                    return;
                }

                CalculateTotal();
                pnlSearchItemByName.Enabled = tlpNumericPad.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageText(ex.Message, false);
                if(_allowAddOnlyAfterRetrive)
                    pnlSearchItemByName.Enabled = tlpNumericPad.Enabled = btnWriteToCard.Enabled = false;
            }
            finally
            {
                btnShowFromCard.Text = "&Show Items From Card";
                btnShowFromCard.Enabled = true;
                txtCode.Focus();
            }
        }

        private void lblTotalAmount_Click(object sender, EventArgs e)
        {
            if (btnWriteToCard.Enabled)
                btnWriteToCard_Click(sender, e);
        }

        private void txtCode_Enter(object sender, EventArgs e)
        {
            txtCode.Select(txtCode.Text.Length, 0);
            //txtCode.Text = string.Empty;
        }

        private void grdItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtCode.Text = string.Empty;
            txtCode.Focus();
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (txtCode.Text.Length > 0)
            {
                txtCode.Text = txtCode.Text.Substring(0, txtCode.Text.Length - 1);
                txtCode.Select(txtCode.Text.Length, 0);
                txtCode.Focus();
            }
            else
                txtCode.Focus();
        }

        private void RatePadClick(object sender, EventArgs e)
        {
            txtAmountForQuantity.Text += ((Button)sender).Text;
            txtAmountForQuantity.Select(txtAmountForQuantity.Text.Length, 0);

            lblMessages.Visible = false;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            decimal d;
            if (decimal.TryParse(txtAmountForQuantity.Text, out d))
            {
                grdItems.SelectedRows[0].Cells["Quantity"].Value = (d / Convert.ToDecimal(grdItems.SelectedRows[0].Cells["Rate"].Value)).ToString("0.000");
                grdItems.SelectedRows[0].Cells["Amount"].Value = d.ToString("0.00");
                grdItems.SelectedRows[0].Cells["QuantityByAmount"].Value = "1";

                //if (_modifyRate)
                //{
                //    if (lblCode.Text != "Rate:")
                //    {
                //        lblCode.Text = "Rate:";
                //        tlpNumericPad.BackColor = Color.Red;
                //        lblEnterRate.Visible = true;
                //        tlpQuantityPad.Visible = pnlAmountForQuantity.Visible = false;
                //    }
                //}
                //else
                    ResetForNewItem();
            }
            else
            {
                txtAmountForQuantity.Text = string.Empty;
                txtAmountForQuantity.Focus();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (txtAmountForQuantity.Text.Length > 0)
            {
                txtAmountForQuantity.Text = txtAmountForQuantity.Text.Substring(0, txtAmountForQuantity.Text.Length - 1);
                txtAmountForQuantity.Select(txtAmountForQuantity.Text.Length, 0);
                txtAmountForQuantity.Focus();
            }
            else
                txtAmountForQuantity.Focus();
        }
    }
}