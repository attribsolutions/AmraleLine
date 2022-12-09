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
    public partial class frmCustomer : Form
    {
        #region Class level variables...
        
        CustomerManager _customerManager = new CustomerManager();
        CustomerInfo _customer = new CustomerInfo();
        BindingList<CustomerItemsInfo> _custometItemsInfo = new BindingList<CustomerItemsInfo>();
        ItemManager _itemManager = new ItemManager(); 
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        ulong CustomerName;
        #endregion

        #region Form Events
        public frmCustomer()
        {
            InitializeComponent();
            BindItems();
            this.Text = Program.MESSAGEBOXTITLE + " - Customer Master";
        }

        public frmCustomer(CustomerInfo customer,BindingList<CustomerItemsInfo> customerItems)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Customer Master";
            _viewMode = EnumClass.FormViewMode.EditMode;
            _customer = customer;
            _custometItemsInfo = customerItems;
            BindItems();            
        }

        public int NewRecordID
        {
            get { return _newRecordID; } 
            set { _newRecordID = value; } 
        }        

        private void frmCustomer_KeyPress(object sender, KeyPressEventArgs e)
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
        #endregion

        #region Add Items
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtQuantity.Text == "")
            {
                MessageBox.Show("Please Enter The Item Quantity.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (Convert.ToDecimal(txtQuantity.Text) == 0)
            {
                txtQuantity.Focus();
                return;
            }

            for (int i = 0; i <= grdItems.Rows.Count - 1; i++)
            {
                if (Convert.ToInt32(cboItems.SelectedValue) == Convert.ToInt32(grdItems.Rows[i].Cells["ItemID"].Value))
                {
                    MessageBox.Show("Item already exist in the grid.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboItems.Focus();
                    return;
                }
            }

            ItemInfo item = null;
            try
            {
                item = _itemManager.GetItem(Convert.ToInt32(cboItems.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = grdItems.Rows.Add();
            grdItems.Rows[row].Cells["ItemID"].Value = cboItems.SelectedValue;
            grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
            grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
            grdItems.Rows[row].Cells["Quantity"].Value = txtQuantity.Text;
            grdItems.Rows[row].Cells["UnitID"].Value = item.UnitID;
            grdItems.Rows[row].Cells["Unit"].Value = item.Unit;
            grdItems.Rows[row].Cells["Gst"].Value = item.Gst;
            grdItems.Rows[row].Cells["Rate"].Value = item.Rate;
            grdItems.Rows[row].Cells["Amount"].Value = (Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(item.Rate)).ToString("0.00");

            DisableFields();
            cboItems.SelectedIndex = -1;
            txtQuantity.Text = string.Empty;
            cboItems.Focus();
        }

        private void DisableFields()
        {
            grdItems.Columns["ItemID"].Visible = false;
            grdItems.Columns["ItemCode"].Visible = false;
            grdItems.Columns["UnitID"].Visible = false;
            grdItems.Columns["Unit"].Visible = false;
            grdItems.Columns["Gst"].Visible = false;
            grdItems.Columns["Rate"].Visible = false;
            grdItems.Columns["Amount"].Visible = false;
        }

        #endregion

        #region Load Events

        private void frmCustomer_Load(object sender, EventArgs e)
        {
            FillLines();
            int LineID = Convert.ToInt32(cboLine.SelectedValue);
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";
                txtName.Tag = _customer.ID;
                txtName.Text = _customer.Name;
                txtContactPerson.Text = _customer.ContactPerson;
                txtAddress.Text = _customer.Address;
                txtCity.Text = _customer.City;
                txtMobile.Text = _customer.Mobile;
                txtEMail.Text = _customer.EMail;
                txtCustomerNumber.Text = _customer.CustomerNumber.ToString();
                //txtBarcode.Text = _customer.Barcode;
                //cboCustomerType.SelectedIndex = _customer.CustomerType;
                cboLine.SelectedValue = _customer.LineID;
                cboLine.Enabled = false;
                dtMemberSince.Value = _customer.MemberSince;
                txtBalance.Text = _customer.Balance.ToString("0.00");
                txtDeposit.Text = _customer.Deposit.ToString("0.00");
                chkIsActive.Checked = _customer.IsActive;
                txtCustomerNameMarathi.Text = _customer.CustomerNameMarathi;

                BindGrid();
                txtName.Focus();
                BindItems();
            }
            else
            {
                DisableFields();                
                txtCustomerNumber.Text = _customerManager.GetMaxCustomerNumber().ToString();
                txtName.Focus();
            }
        }
        bool FillLines()
        {
            LineManager _lineManager = new LineManager();
            BindingList<LineInfo> lines = new BindingList<LineInfo>();
            try
            {
                lines = _lineManager.GetLines();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Line ID." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboLine.DataSource = lines;
            cboLine.DisplayMember = "ID";
            cboLine.ValueMember = "ID";
            lines = null;
            cboLine.SelectedIndex = -1;
            return true;
        }
        private void BindGrid()
        {
            foreach (CustomerItemsInfo customerItem in _custometItemsInfo)
            {
                ItemInfo item = null;
                try
                {
                    item = _itemManager.GetItem(Convert.ToInt32(customerItem.ItemID));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int row = grdItems.Rows.Add();
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                    grdItems.Rows[row].Cells["ItemID"].Value = item.ID;
                else
                    grdItems.Rows[row].Cells["ItemID"].Value = cboItems.SelectedValue;
                grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
                grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
                grdItems.Rows[row].Cells["Quantity"].Value = customerItem.Quantity;
                grdItems.Rows[row].Cells["UnitID"].Value = item.UnitID;
                grdItems.Rows[row].Cells["Unit"].Value = item.Unit;
                grdItems.Rows[row].Cells["Gst"].Value = Convert.ToDecimal(item.Gst);
                grdItems.Rows[row].Cells["Rate"].Value = item.Rate;
                grdItems.Rows[row].Cells["Amount"].Value = (Convert.ToDecimal(customerItem.Quantity) * Convert.ToDecimal(item.Rate)).ToString("0.00");
            }
            DisableFields();
        }

        private bool BindItems()
        {
            BindingList<ItemInfo> retval = new BindingList<ItemInfo>();
                     
            try
            {
                retval = _itemManager.GetItemsAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cboItems.DataSource = retval;
            cboItems.DisplayMember = "Name";
            cboItems.ValueMember = "ID";
            retval = null;
            return true;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() != string.Empty)
                lblHeading.Text = txtName.Text;
            else
                lblHeading.Text = "New Customer Details";
        }
        #endregion

        #region Save and Clear

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
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

        bool Save()
        {
            if (!ValidateFields())
                return false;

            BindingList<CustomerItemsInfo> customerItems = new BindingList<CustomerItemsInfo>();

            CustomerInfo customer = SetValue(out customerItems);

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    if (!_customerManager.CheckCustomerNumber(customer.CustomerNumber))
                    {
                        //if (!_customerManager.CheckBarCode(txtBarcode.Text.Trim()))
                        //{
                            _newRecordID = _customerManager.AddCustomer(customer, customerItems);
                            _newRecordAdded = true;
                            return true;
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Customer Barcode Is Already Exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return false;
                        //}
                    }
                    else
                    {
                        MessageBox.Show("Customer Number Is Already Exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Customer." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    customer.ID = Convert.ToInt32(txtName.Tag);
                    _customerManager.UpdateCustomer(customer, customerItems);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Customer." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        bool ValidateFields()
        {
            if (txtCustomerNumber.Text.Trim() == string.Empty)
            {
                txtCustomerNumber.Focus();
                MessageBox.Show("Please enter Customer Number.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtName.Text.Trim() == string.Empty)
            {
                txtName.Focus();
                MessageBox.Show("Please enter Customer Name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtContactPerson.Text.Trim() == string.Empty)
            {
                txtContactPerson.Focus();
                MessageBox.Show("Please enter Contact person name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboLine.Text.Trim() == string.Empty)
            {
                cboLine.Focus();
                MessageBox.Show("Please select line name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            decimal d;
            if (!decimal.TryParse(txtBalance.Text, out d))
            {
                txtBalance.Focus();
                MessageBox.Show("Please enter balance in figures only.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        CustomerInfo SetValue(out BindingList<CustomerItemsInfo> customerItemss)
        {  
             
            CustomerInfo retVal = new CustomerInfo();
            retVal.CustomerNumber = Convert.ToInt16(txtCustomerNumber.Text);
            retVal.Name = txtName.Text;
            retVal.ContactPerson = txtContactPerson.Text;
            retVal.Address = txtAddress.Text;
            retVal.City = txtCity.Text;
            retVal.Mobile = txtMobile.Text;
            retVal.EMail = txtEMail.Text;
            retVal.Balance = Convert.ToDecimal(txtBalance.Text);
            retVal.Deposit = Convert.ToString(txtDeposit.Text) != "" ? Convert.ToDecimal(txtDeposit.Text) : 0;
            retVal.IsActive = chkIsActive.Checked;
            //retVal.CustomerType = Convert.ToInt32(cboCustomerType.SelectedIndex);
            retVal.LineID = Convert.ToInt32(cboLine.SelectedValue);
            //retVal.Barcode = txtBarcode.Text;
            retVal.MemberSince = dtMemberSince.Value;
            retVal.CustomerNameMarathi = txtCustomerNameMarathi.Text;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            BindingList<CustomerItemsInfo> customerItems = new BindingList<CustomerItemsInfo>();

            foreach (DataGridViewRow rows in grdItems.Rows)
            {
                CustomerItemsInfo items = new CustomerItemsInfo();
                items.ItemID = Convert.ToInt32(rows.Cells["ItemID"].Value);
                items.Quantity = Convert.ToDecimal(rows.Cells["Quantity"].Value);

                customerItems.Add(items);
            }
            customerItemss = customerItems;
            return retVal;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";
            txtCustomerNumber.Text = string.Empty;
            txtName.Text = string.Empty;
            lblHeading.Text = "New Customer Details";
            txtContactPerson.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtMobile.Text = string.Empty;
            txtEMail.Text = string.Empty;
            //txtBarcode.Text = string.Empty;
            //cboCustomerType.SelectedIndex = cboLine.SelectedIndex =  -1;
            cboItems.SelectedIndex = -1;
            txtQuantity.Text = string.Empty;
            txtBalance.Text = "0.00";
            txtDeposit.Text = "0.00";
            chkIsActive.Checked = true;
            grdItems.DataSource = null;
            grdItems.Rows.Clear();
            txtName.Focus();
            cboLine.SelectedIndex = -1;
        }

        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }
        #endregion        

        

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
            txtCustomerNumber.Text =Convert.ToString(CustomerNo);
            return CustomerNo;
        }

        private void cboLine_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LastCustomerNoLineWise();
        }

        private void txtCustomerNameMarathi_TextChanged(object sender, EventArgs e)
        {
                
        }
    }
}