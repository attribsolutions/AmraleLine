using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BusinessLogic;
using DataObjects;

namespace SweetPOS
{
    public partial class frmSaleManual : Form
    {
        #region Class level variables...

        SaleManager _saleManager = new SaleManager();
        SaleInfo _sale = new SaleInfo();
        BindingList<SaleItemInfo> _saleItems = new BindingList<SaleItemInfo>();
        OrderInfo _order = new OrderInfo();
        BindingList<OrderItemInfo> _orderItems = new BindingList<OrderItemInfo>();
        LineManager _lineManager = new LineManager();
        ItemManager _itemManager = new ItemManager();
        CustomerManager _customerManager = new CustomerManager();
        SettingManager _settingManager = new SettingManager();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        bool _roundAmount = false;
        decimal _round50 = 0;
        decimal _round1 = 0;
        bool _billFromOrder = false;

        #endregion

        public frmSaleManual()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Manual Sale Entry";
        }

        public frmSaleManual(OrderInfo order, BindingList<OrderItemInfo> orderItems)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Order to Sale";
            _order = order;
            _orderItems = orderItems;
            _billFromOrder = true;
        }

        private void ShowOrderInSaleFormat()
        {
            if (_order.OrderDate != DateTime.MinValue)
                dtBillDate.Value = _order.OrderDate;
            //dtBillDate.Enabled = false;
            cboCashCredit.SelectedIndex = 1;
            cboCustomer.SelectedValue = Convert.ToInt32(_order.CustomerID);
            //cboCustomer.Enabled = false;
            txtDiscountPercent.Text = "0";
            txtCardPaymentDetails.Text = string.Empty;
            txtDescription.Text = _sale.Description;

            foreach (OrderItemInfo orderItem in _orderItems)
            {
                ItemInfo item = GetItemInfo(orderItem.ItemID);
                int row = grdItems.Rows.Add();

                grdItems.Rows[row].Cells["ItemID"].Value = orderItem.ItemID;
                grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
                grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
                grdItems.Rows[row].Cells["Quantity"].Value = orderItem.Quantity;
                grdItems.Rows[row].Cells["UnitID"].Value = orderItem.UnitID;
                grdItems.Rows[row].Cells["Unit"].Value = orderItem.UnitName;
                grdItems.Rows[row].Cells["Vat"].Value = orderItem.Vat;
                grdItems.Rows[row].Cells["Rate"].Value = orderItem.Rate;
                grdItems.Rows[row].Cells["Amount"].Value = orderItem.Amount;
            }
            CalculateTotals();
            dtBillDate.Focus();
        }

        public frmSaleManual(SaleInfo sale, BindingList<SaleItemInfo> saleItems)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Modify Sale Entry";
            _sale = sale;
            _saleItems = saleItems;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmSaleModify_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                CloseForm();
            }
        }

        private void frmSaleModify_Load(object sender, EventArgs e)
        {
            FillLines();
            txtCustomerNo.Text = Convert.ToString(0);
           // GetCustomersByLineID();
            FillItems();
            cboCustomer.SelectedIndex = -1;
            cboItemName.SelectedIndex = -1;
            cboLine.SelectedIndex = -1;
            cboCashCredit.SelectedIndex = 1;
            cboTransactionType.SelectedIndex = 0;
           // txtCustomerNo.Text =Convert.ToString(0);

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                New();
                if (_order != null)
                    ShowOrderInSaleFormat();
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";
                //cboCustomer.Enabled = false;
                cboLine.SelectedValue = _sale.LineNumber;
                txtBillNo.Tag = _sale.ID;
                dtBillDate.Value = _sale.BillDate;
                //dtBillDate.Enabled = false;
                txtBillNo.Text = _sale.BillNo.ToString();
                cboCashCredit.SelectedIndex = _sale.CashCredit;
                cboCustomer.SelectedValue = Convert.ToInt32(_sale.CustomerID);
                txtDiscountPercent.Text = _sale.DiscountPercentage.ToString("0.00");
                txtCardPaymentDetails.Text = _sale.CardPaymentDetails;
                txtCustomerNo.Text = _sale.CustomerNumber.ToString();
               
                if (_sale.IsCouponSale)
                    cboCouponSale.Checked = true;
                else
                    cboCouponSale.Checked = false;

                txtDescription.Text = _sale.Description;
                if (_sale.RFIDTransaction)
                    cboTransactionType.SelectedIndex = 1;

                foreach (SaleItemInfo saleItem in _saleItems)
                {
                    ItemInfo item = GetItemInfo(saleItem.ItemID);
                    int row = grdItems.Rows.Add();
                    grdItems.Rows[row].Cells["ItemID"].Value = saleItem.ItemID;
                    grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
                    grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
                    grdItems.Rows[row].Cells["Quantity"].Value = saleItem.Quantity;
                    grdItems.Rows[row].Cells["UnitID"].Value = saleItem.UnitID;
                    grdItems.Rows[row].Cells["Unit"].Value = saleItem.UnitName;
                    grdItems.Rows[row].Cells["Gst"].Value = saleItem.Vat;
                    grdItems.Rows[row].Cells["Rate"].Value = saleItem.Rate;
                    grdItems.Rows[row].Cells["Amount"].Value = saleItem.Amount;
                }
                CalculateTotals();
                dtBillDate.Focus();
                //Desable Controls 2018/04/16 Akash
                cboLine.Enabled = false;
                txtCustomerNo.ReadOnly = true;
                cboCustomer.Enabled = false;
            }
            LoadRoundingDetails();
            
           
        }

        private void LoadRoundingDetails()
        {
            try
            {
                _roundAmount = Convert.ToBoolean(_settingManager.GetSetting(7));         //Hardcode

                _round50 = Convert.ToDecimal(_settingManager.GetSetting(8));            //Hardcode

                _round1 = Convert.ToDecimal(_settingManager.GetSetting(9));             //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Setting." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool FillItems()
        {
            BindingList<ItemInfo> items = new BindingList<ItemInfo>();
            try
            {
                items = _itemManager.GetItemsAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboItemName.DataSource = items;
            cboItemName.DisplayMember = "Name";
            cboItemName.ValueMember = "ID";

            items = null;
            return true;
        }

        bool FillLines()
        {
            
            BindingList<LineInfo> lines = new BindingList<LineInfo>();
            try
            {
                lines = _lineManager.GetLines();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Lines ." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cboLine.DataSource = lines;
            cboLine.DisplayMember = "ID";
            cboLine.ValueMember = "ID";
            cboLine.SelectedIndex = 1;

            lines = null;
            return true;
        }

        bool GetCustomersByIDs()
        {
            BindingList<CustomerInfo> customers = new BindingList<CustomerInfo>();
            try
            {
                customers = _customerManager.GetCustomersByIDs(Convert.ToInt32(cboLine.SelectedValue), Convert.ToInt32(txtCustomerNo.Text), Convert.ToInt32(cboCustomer.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
           
            
            cboCustomer.DataSource = customers;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "ID";
          //  customers.Insert(0, new CustomerInfo { ID = 0, Name = "Select" });
            customers = null;
            return true;
        }
        bool GetCustomersNumberByID()
        {
            Int32 CustomersNumber = 0;
            BindingList<CustomerInfo> customers = new BindingList<CustomerInfo>();
            try
            {
                CustomersNumber = _customerManager.GetCustomersNumberByID(Convert.ToInt32(cboLine.SelectedValue), Convert.ToInt32(cboCustomer.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }



            txtCustomerNo.Text = CustomersNumber.ToString();
            return true;
        }
        bool ValidateFields()
        {
            if (dtBillDate.Value.Date > DateTime.Now.Date)
            {
                dtBillDate.Focus();
                MessageBox.Show("Please enter valid Sale Date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtBillNo.Text.Trim() == string.Empty)
            {
                txtBillNo.Focus();
                MessageBox.Show("Please enter Sale Number.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboCashCredit.SelectedIndex == -1)
            {
                cboCashCredit.Focus();
                MessageBox.Show("Please select Pay Mode.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //if ((cboCustomer.SelectedIndex != -1 && cboCashCredit.Text == "Cash") || (cboCustomer.SelectedIndex == -1 && cboCashCredit.Text == "Credit"))
            //{
            //    cboCashCredit.Focus();
            //    MessageBox.Show("Please select valid pay mode OR customer.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}

            if (grdItems.Rows.Count == 0 || Convert.ToDecimal(txtTotalAmount.Text) == 0)
            {
                cboItemName.Focus();
                MessageBox.Show("Please add at least one item to proceed.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        SaleInfo SetValue(out BindingList<SaleItemInfo> saleItems)
        {
            SaleInfo retVal = new SaleInfo();
            BindingList<SaleItemInfo> saleItemss = new BindingList<SaleItemInfo>();

            retVal.BillDate = dtBillDate.Value.Date;
            retVal.BillNo = Convert.ToString(txtBillNo.Text);
            retVal.CashCredit = (byte)cboCashCredit.SelectedIndex;

            retVal.DivisionID = Program.DivisionID;
            retVal.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
            retVal.DiscountPercentage = Convert.ToDecimal(txtDiscountPercent.Text);
            retVal.DiscountAmount = Convert.ToDecimal(txtDiscountAmount.Text);
            retVal.NetAmount = Convert.ToDecimal(txtNetAmount.Text) + Convert.ToDecimal(lblRoundOff.Text);
            retVal.RoundedAmount = Convert.ToDecimal(txtNetAmount.Text);
            retVal.CardPaymentDetails = txtCardPaymentDetails.Text;

            if (cboCashCredit.Text == "Credit" || Convert.ToString(cboCustomer.SelectedValue)!="")
            {
                retVal.BalanceAmount = retVal.NetAmount;
                retVal.CustomerID = Convert.ToInt32(cboCustomer.SelectedValue);
            }
            else
                retVal.CustomerID = 0;

            retVal.Description = txtDescription.Text;
            retVal.IsCouponSale = (Boolean)cboCouponSale.Checked;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;
            retVal.BillFromOrder = _billFromOrder;
            if (_billFromOrder)
                retVal.OrderID = _order.ID;

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                SaleItemInfo saleItem = new SaleItemInfo();
                saleItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
                saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                saleItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                saleItem.Vat = Convert.ToDecimal(dr.Cells["Gst"].Value);
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

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                Int64 billNumber = 0;
                try
                {
                    sale.RFIDTransaction = false;
                    _newRecordID = _saleManager.AddSale(sale, saleItems, Program.CounterName, out billNumber);
                    _newRecordAdded = true;
                    
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    sale.ID = Convert.ToInt32(txtBillNo.Tag);
                    _saleManager.UpdateSale(sale, saleItems);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";
            cboCustomer.Enabled = true;

            dtBillDate.Enabled = true;
            dtBillDate.Value = DateTime.Today.Date;
            try
            {
                txtBillNo.Text = _saleManager.GetNextBillNumber().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting max bill number." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           // cboCashCredit.SelectedText = "Credit";
            cboCustomer.SelectedIndex = -1;
            grdItems.Rows.Clear();
            txtDiscountPercent.Text = txtDiscountAmount.Text = txtTaxAmount.Text = txtNetAmount.Text = txtTotalAmount.Text = "0.00";
            CalculateTotals();
            txtDescription.Text = string.Empty;
            chkBarCode.Checked = false;
            txtBarCode.Text = string.Empty;
            txtCardPaymentDetails.Text = string.Empty;
            dtBillDate.Focus();
        }

        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (Save())
                New();
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (Save())
                CloseForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        bool CheckDuplicateItem(ChallanItemInfo challanItem)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToInt32(dr.Cells["ItemId"].Value) == challanItem.ItemID)
                {
                    dr.Cells["Quantity"].Value = (Convert.ToDecimal(dr.Cells["Quantity"].Value) + challanItem.Quantity).ToString("0.000");
                    return true;
                }
            }
            return false;
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

        private void grdItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (grdItems.Columns[e.ColumnIndex].Name == "Quantity" || grdItems.Columns[e.ColumnIndex].Name == "Rate")
            {
                string s = e.FormattedValue.ToString();
                decimal d;
                if (!decimal.TryParse(s, out d))
                {
                    e.Cancel = true;
                }
            }
        }

        private void grdItems_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (grdItems.Columns[e.ColumnIndex].Name == "Quantity")
            {
                grdItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(grdItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("0.000");
            }
            if (grdItems.Columns[e.ColumnIndex].Name == "Rate")
            {
                grdItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(grdItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("0.00");
            }
            CalculateTotals();
        }

        void CalculateTotals()
        {
            txtTaxAmount.Text = txtNetAmount.Text = txtTotalAmount.Text = "0.00";
            decimal totalAmount = 0;
            decimal totalTax = 0;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                decimal amt = Convert.ToDecimal(dr.Cells["Quantity"].Value) * Convert.ToDecimal(dr.Cells["Rate"].Value);
                dr.Cells["Amount"].Value = amt.ToString("0.00");
                totalAmount += amt;

                decimal tax = amt - ((amt * 100) / (100 + Convert.ToDecimal(dr.Cells["Gst"].Value)));
                totalTax += tax;
            }
            txtTotalAmount.Text = totalAmount.ToString("0.00");
            txtTaxAmount.Text = totalTax.ToString("0.00");

            txtDiscountAmount.Text = ((totalAmount * Convert.ToDecimal(txtDiscountPercent.Text)) / 100).ToString("0.00");
            txtNetAmount.Text = (totalAmount - Convert.ToDecimal(txtDiscountAmount.Text)).ToString("0.00");

            if (_roundAmount)
                RoundTotalAmount(totalAmount);
        }

        private void RoundTotalAmount(decimal totalAmount)
        {
            decimal roundAmount;
            decimal roundOffAmount;

            int i = (int)totalAmount;
            decimal paise = totalAmount - i;

            if (paise > 0)
            {
                if (paise > _round1)
                {
                    roundAmount = decimal.Round(totalAmount, 0);
                    roundOffAmount = totalAmount - roundAmount;
                }
                else if (paise > _round50 && paise <= _round1)
                {
                    roundAmount = i + 0.50M;
                    roundOffAmount = totalAmount - roundAmount;
                }
                else
                {
                    roundAmount = (decimal)i;
                    roundOffAmount = totalAmount - roundAmount;
                }
                txtNetAmount.Text = roundAmount.ToString("0.00");
                lblRoundOff.Text = roundOffAmount.ToString("0.00");
            }
            else
            {
                txtNetAmount.Text = totalAmount.ToString("0.00");
                lblRoundOff.Text = "0.00";
            }
        }

        private void txtDiscountPercent_Validated(object sender, EventArgs e)
        {
            txtDiscountPercent.Text = Convert.ToDecimal(txtDiscountPercent.Text).ToString("0.00");
            CalculateTotals();
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (cboItemName.SelectedIndex == -1)
            {
                if (chkBarCode.Checked)
                    txtBarCode.Focus();
                else
                    cboItemName.Focus();

                return;
            }
            if (Convert.ToDecimal(txtQuantity.Text) == 0)
            {
                txtQuantity.Focus();
                return;
            }
            if (Convert.ToDecimal(txtRate.Text) == 0)
            {
                txtRate.Focus();
                return;
            }

            if (CheckItemExist(Convert.ToInt32(cboItemName.SelectedValue)))
                return;

            ItemInfo item = null;
            try
            {
                item = _itemManager.GetItem(Convert.ToInt32(cboItemName.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = grdItems.Rows.Add();
            grdItems.Rows[row].Cells["ItemID"].Value = cboItemName.SelectedValue;
            grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
            grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
            grdItems.Rows[row].Cells["Quantity"].Value = txtQuantity.Text;
            grdItems.Rows[row].Cells["UnitID"].Value = item.UnitID;
            grdItems.Rows[row].Cells["Unit"].Value = item.Unit;
            grdItems.Rows[row].Cells["Gst"].Value = item.Gst;
            grdItems.Rows[row].Cells["Rate"].Value = txtRate.Text;
            grdItems.Rows[row].Cells["Amount"].Value = (Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtRate.Text)).ToString("0.00");

            cboItemName.SelectedIndex = -1;
            txtQuantity.Text = "0.000";
            txtRate.Text = "0.00";
            txtBarCode.Text = string.Empty;

            if (chkBarCode.Checked)
                txtBarCode.Focus();
            else
                cboItemName.Focus();

            CalculateTotals();
        }

        bool CheckItemExist(int itemId)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToInt32(dr.Cells["ItemID"].Value) == itemId)
                {
                    MessageBox.Show("Item already exist in the grid.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboItemName.Focus();
                    return true;
                }
            }

            return false;
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtQuantity.Text, out d))
                e.Cancel = true;
            else
                txtQuantity.Text = Convert.ToDecimal(txtQuantity.Text).ToString("0.000");
        }

        private void txtRate_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtRate.Text, out d))
                e.Cancel = true;
            else
                txtRate.Text = Convert.ToDecimal(txtRate.Text).ToString("0.00");
        }

        private void cboItemName_Validated(object sender, EventArgs e)
        {
            if (cboItemName.SelectedIndex != -1)
            {
                try
                {
                    ItemInfo item = _itemManager.GetItem(Convert.ToInt32(cboItemName.SelectedValue));

                    txtRate.Text = item.Rate.ToString("0.00");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void grdItems_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateTotals();
        }

        private void txtBarCode_Validating(object sender, CancelEventArgs e)
        {
            ItemInfo item = null;
            try
            {
                item = _itemManager.GetItemByBarCode(txtBarCode.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting item by bar code." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (item.ID != 0)
            {
                cboItemName.SelectedValue = item.ID;
                txtRate.Text = item.Rate.ToString("0.00");
                txtQuantity.Focus();
            }
            else
            {
                cboItemName.SelectedIndex = -1;
                txtRate.Text = "0.00";
                txtQuantity.Focus();
            }
        }

        private void chkBarCode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBarCode.Checked)
            {
                txtBarCode.Visible = true;
                lblBarCode.Visible = true;
                cboItemName.Enabled = false;
                txtBarCode.Focus();
            }
            else
            {
                txtBarCode.Visible = false;
                lblBarCode.Visible = false;
                cboItemName.Enabled = true;
                cboItemName.Focus();
            }
        }


        //private void txtCustomerNo_TextChanged_1(object sender, EventArgs e)
        //{  
        //    if (txtCustomerNo.Text=="")
        //    {
        //        txtCustomerNo.Focus();
        //    }
        //    else
        //    {
        //        GetCustomersByIDs();
        //    }
        //}

        private int LastCustomerNoLineWise()
        {
            int CustomerNo = 0;

            try
            {
                CustomerNo = _customerManager.GetCustomersLastNoByLineID(Convert.ToInt32(cboLine.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customer No." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtCustomerNo.Text = Convert.ToString(CustomerNo);
            return CustomerNo;
        }

        private void cboLine_TextChanged(object sender, EventArgs e)
        {
            txtCustomerNo.Text = Convert.ToString(0);
          
           // GetCustomersByIDs();
        }

        private void cboLine_SelectionChangeCommitted(object sender, EventArgs e)
        {
           
            cboCustomer.SelectedIndex  = -1;
            GetCustomersByIDs();
            
           
        }

        private void txtCustomerNo_TextChanged(object sender, EventArgs e)
        {
                if (txtCustomerNo.Text=="")
                {
                    txtCustomerNo.Focus();
                }
                else
                {
                 GetCustomersByIDs();
                }
        }
      
        private void cboCustomer_SelectionChangeCommitted(object sender, EventArgs e)
        {
              GetCustomersNumberByID();
        }

        


    }
}