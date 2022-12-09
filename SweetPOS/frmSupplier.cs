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
    public partial class frmSupplier : Form
    {
        #region Class level variables...
        
        SupplierManager _supplierManager = new SupplierManager();
        SupplierInfo _supplier = new SupplierInfo();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmSupplier()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Supplier Master";
        }

        public frmSupplier(SupplierInfo supplier)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Supplier Master";
            _supplier = supplier;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmSupplier_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmSupplier_Load(object sender, EventArgs e)
        {

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                txtName.Tag = _supplier.ID;
                txtName.Text = _supplier.Name;
                txtContactPerson.Text = _supplier.ContactPerson;
                txtAddress.Text = _supplier.Address;
                txtCity.Text = _supplier.City;
                txtState.Text = _supplier.State;
                txtPhone.Text = _supplier.Phone;
                txtMobile.Text = _supplier.Mobile;
                txtEMail.Text = _supplier.EMail;
                txtVatTin.Text = _supplier.VatTinNo;
                txtBalance.Text = _supplier.Balance.ToString("0.00");
                chkIsActive.Checked = _supplier.IsActive;

                txtName.Focus();
            }
        }

        bool ValidateFields()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                txtName.Focus();
                MessageBox.Show("Please enter Supplier Name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtContactPerson.Text.Trim() == string.Empty)
            {
                txtContactPerson.Focus();
                MessageBox.Show("Please enter Contact person name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        SupplierInfo SetValue()
        {
            SupplierInfo retVal = new SupplierInfo();
            retVal.Name = txtName.Text;
            retVal.ContactPerson = txtContactPerson.Text;
            retVal.Address = txtAddress.Text;
            retVal.City = txtCity.Text;
            retVal.State = txtState.Text;
            retVal.Phone = txtPhone.Text;
            retVal.Mobile = txtMobile.Text;
            retVal.EMail = txtEMail.Text;
            retVal.VatTinNo = txtVatTin.Text;
            retVal.Balance = Convert.ToDecimal(txtBalance.Text);
            retVal.IsActive = chkIsActive.Checked;

            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            return retVal;
        }

        bool Save()
        {
            if (!ValidateFields())
                return false;

            SupplierInfo supplier = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _supplierManager.AddSupplier(supplier);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Supplier." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    supplier.ID = Convert.ToInt32(txtName.Tag);
                    _supplierManager.UpdateSupplier(supplier);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Supplier." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtName.Text = string.Empty;
            lblHeading.Text = "New Supplier Details";
            txtContactPerson.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtState.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtMobile.Text = string.Empty;
            txtEMail.Text = string.Empty;
            txtVatTin.Text = string.Empty;
            txtBalance.Text = "0.00";
            chkIsActive.Checked = true;

            txtName.Focus();
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

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() != string.Empty)
                lblHeading.Text = txtName.Text;
            else
                lblHeading.Text = "New Supplier Details";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }
    }
}