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
    public partial class frmOrder : Form
    {
        #region Class level variables...

        OrderManager _orderManager = new OrderManager();
        OrderInfo _order = new OrderInfo();
        BindingList<OrderItemInfo> _orderItems = new BindingList<OrderItemInfo>();
        ItemManager _itemManager = new ItemManager();
        CustomerManager _customerManager = new CustomerManager();
        UserManager _userManager = new UserManager();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        bool _fillingValues = true;

        #endregion

        public frmOrder()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Order Entry";
        }

        public frmOrder(OrderInfo order, BindingList<OrderItemInfo> orderItems)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Order Entry";
            _order = order;
            _orderItems = orderItems;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmChallan_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmChallan_Load(object sender, EventArgs e)
        {
            FillCustomers();
            FillItems();
            _fillingValues = false;

            cboCustomerName.SelectedIndex = -1;
            cboItem.SelectedIndex = -1;

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                dtOrderDate.Tag = _order.ID;
                dtOrderDate.Value = _order.OrderDate;
                cboCustomerName.SelectedValue = Convert.ToInt32(_order.CustomerID);
                CustomerInfo customer = new CustomerInfo();
                try { customer = _customerManager.GetCustomer(_order.CustomerID); }
                catch (Exception ex) { MessageBox.Show("Error in getting customer." + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                txtContactNumber.Text = customer.Mobile;
                dtDeliveryDate.Value = _order.DeliveryDate;
                txtAdvance.Text = _order.Advance.ToString("0.00");
                txtDeliveryPayment.Text = _order.DeliveryPayment.ToString("0.00");
                txtComments.Text = _order.Comments;
                chkOrderCompleted.Checked = _order.IsCompleted;

                foreach (OrderItemInfo orderItem in _orderItems)
                {
                    int row = grdItems.Rows.Add();

                    grdItems.Rows[row].Cells["ItemID"].Value = orderItem.ItemID;
                    grdItems.Rows[row].Cells["UnitID"].Value = orderItem.UnitID;
                    grdItems.Rows[row].Cells["ItemName"].Value = orderItem.ItemName;
                    grdItems.Rows[row].Cells["Quantity"].Value = orderItem.Quantity.ToString("0.000");
                    grdItems.Rows[row].Cells["Rate"].Value = orderItem.Rate.ToString("0.00");
                    grdItems.Rows[row].Cells["Vat"].Value = orderItem.Vat.ToString("0.00");
                    grdItems.Rows[row].Cells["Amount"].Value = orderItem.Amount.ToString("0.00");
                }

                CalculateTotals();

                dtOrderDate.Focus();
            }
        }

        bool FillCustomers()
        {
            BindingList<CustomerInfo> customers = new BindingList<CustomerInfo>();
            try
            {
                customers = _customerManager.GetCustomersAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboCustomerName.DataSource = customers;
            cboCustomerName.DisplayMember = "Name";
            cboCustomerName.ValueMember = "ID";

            customers = null;
            return true;
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

            cboItem.DataSource = items;
            cboItem.DisplayMember = "Name";
            cboItem.ValueMember = "ID";

            items = null;
            return true;
        }

        private void cboCustomerName_Leave(object sender, EventArgs e)
        {
            try
            {
                CustomerInfo customer = _customerManager.GetCustomer(Convert.ToInt32(cboCustomerName.SelectedValue));
                txtContactNumber.Text = customer.Mobile;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting customer." + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void cboItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_fillingValues)
                return;

            try
            {
                ItemInfo item = _itemManager.GetItem(Convert.ToInt32(cboItem.SelectedValue));
                txtRate.Text = item.Rate.ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item." + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        bool ValidateFields()
        {
            if (cboCustomerName.Text.Trim() == string.Empty)
            {
                cboCustomerName.Focus();
                MessageBox.Show("Please select customer name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtContactNumber.Text.Trim() == string.Empty)
            {
                txtContactNumber.Focus();
                MessageBox.Show("Please enter contact number.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtDeliveryDate.Value.Date < dtOrderDate.Value.Date)
            {
                dtDeliveryDate.Focus();
                MessageBox.Show("Please enter valid delivery date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (grdItems.Rows.Count == 0)
            {
                cboItem.Focus();
                MessageBox.Show("Please enter atleast one order item.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        OrderInfo SetValue(out BindingList<OrderItemInfo> orderItems)
        {
            OrderInfo retVal = new OrderInfo();
            BindingList<OrderItemInfo> orderItemss = new BindingList<OrderItemInfo>();

            retVal.OrderDate = dtOrderDate.Value.Date;
            if (cboCustomerName.SelectedIndex != -1)
                retVal.CustomerID = Convert.ToInt32(cboCustomerName.SelectedValue);
            else
                retVal.CustomerID = 0;
            retVal.DeliveryDate = dtDeliveryDate.Value;
            retVal.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
            retVal.Advance = Convert.ToDecimal(txtAdvance.Text);
            retVal.DeliveryPayment = Convert.ToDecimal(txtDeliveryPayment.Text);
            retVal.IsCompleted = chkOrderCompleted.Checked;
            retVal.Comments = txtComments.Text;

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                OrderItemInfo orderItem = new OrderItemInfo();
                orderItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
                orderItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                orderItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                orderItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                orderItem.Vat = Convert.ToDecimal(dr.Cells["Vat"].Value);
                orderItem.Amount = Convert.ToDecimal(dr.Cells["Amount"].Value);
                
                orderItemss.Add(orderItem);
            }

            orderItems = orderItemss;
            return retVal;
        }

        bool Save()
        {
            if (!ValidateFields())
                return false;

            BindingList<OrderItemInfo> orderItems = null;
            OrderInfo order = SetValue(out orderItems);

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    if (cboCustomerName.SelectedIndex != -1)
                    {
                        CustomerInfo customer = new CustomerInfo();
                        customer.Name = customer.ContactPerson = cboCustomerName.Text;
                        customer.Mobile = txtContactNumber.Text;
                        customer.CreatedBy = Program.CURRENTUSER;
                        customer.CreatedOn = DateTime.Now;

                        _newRecordID = _orderManager.AddOrder(order, orderItems, customer);
                    }
                    else
                    {
                        _newRecordID = _orderManager.AddOrder(order, orderItems, null);
                    }

                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Order." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    order.ID = Convert.ToInt32(dtOrderDate.Tag);
                    _orderManager.UpdateOrder(order, orderItems);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Challan." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            dtOrderDate.Value = DateTime.Today.Date;
            cboCustomerName.Text = string.Empty;
            txtContactNumber.Text = string.Empty;
            dtDeliveryDate.Value = DateTime.Today.Date;
            txtAdvance.Text = "0.00";
            txtDeliveryPayment.Text = "0.00";
            txtComments.Text = string.Empty;

            txtItemNo.Text = string.Empty;
            cboItem.SelectedIndex = -1;
            txtQuantity.Text = "0.000";
            txtRate.Text = "0.00";
            txtAmount.Text = "0.00";

            chkOrderCompleted.Checked = false;

            grdItems.Rows.Clear();
            dtOrderDate.Focus();
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

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtQuantity.Text, out d))
                e.Cancel = true;
            else
                txtQuantity.Text = Convert.ToDecimal(txtQuantity.Text).ToString("0.000");
        }

        private void Amount_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(((TextBox)sender).Text, out d))
                e.Cancel = true;
            else
            {
                ((TextBox)sender).Text = Convert.ToDecimal(((TextBox)sender).Text).ToString("0.00");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboItem.Text.Trim() == string.Empty)
            {
                cboItem.Focus();
                return;
            }
            if (Convert.ToDecimal(txtQuantity.Text) <= 0)
            {
                txtQuantity.Focus();
                return;
            }
            if (Convert.ToDecimal(txtRate.Text) <= 0)
            {
                txtRate.Focus();
                return;
            }

            if (CheckItemExist(Convert.ToInt32(cboItem.SelectedValue)))
                return;

            ItemInfo item = null;
            try
            {
                item = _itemManager.GetItem(Convert.ToInt32(cboItem.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int row = grdItems.Rows.Add();

            grdItems.Rows[row].Cells["ItemID"].Value = cboItem.SelectedValue;
            grdItems.Rows[row].Cells["UnitID"].Value = item.UnitID;
            grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
            grdItems.Rows[row].Cells["Quantity"].Value = txtQuantity.Text;
            grdItems.Rows[row].Cells["Rate"].Value = txtRate.Text;
            grdItems.Rows[row].Cells["Gst"].Value = item.Gst;
            grdItems.Rows[row].Cells["Amount"].Value = (Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtRate.Text)).ToString("0.00");

            grdItems.CurrentCell = grdItems.Rows[row].Cells["ItemName"];

            cboItem.SelectedIndex = -1;
            txtQuantity.Text = "0.000";
            txtRate.Text = "0.00";
            txtAmount.Text = "0.00";

            CalculateTotals();

            cboItem.Focus();
        }

        private void CalculateTotals()
        {
            decimal totalAmount = 0;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                totalAmount += Convert.ToDecimal(dr.Cells["Amount"].Value);
            }
            txtTotalAmount.Text = totalAmount.ToString("0.00");
            txtBalanceAmount.Text = (Convert.ToDecimal(txtTotalAmount.Text) - Convert.ToDecimal(txtAdvance.Text) - Convert.ToDecimal(txtDeliveryPayment.Text)).ToString("0.00");
        }

        bool CheckItemExist(int itemId)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToInt32(dr.Cells["ItemID"].Value) == itemId)
                {
                    MessageBox.Show("Item already exist in the grid.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboItem.Focus();
                    return true;
                }
            }

            return false;
        }

        private void grdItems_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateTotals();
        }

        private void Numeric_Validated(object sender, EventArgs e)
        {
            txtAmount.Text = (Convert.ToDecimal(txtRate.Text) * Convert.ToDecimal(txtQuantity.Text)).ToString("0.00");
            txtBalanceAmount.Text = (Convert.ToDecimal(txtTotalAmount.Text) - Convert.ToDecimal(txtAdvance.Text) - Convert.ToDecimal(txtDeliveryPayment.Text)).ToString("0.00");
        }
    }
}