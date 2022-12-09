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
    public partial class frmItemCompany : Form
    {
        #region Class level variables...

        ItemCompanyManager _itemCompanyManager = new ItemCompanyManager();
        ItemCompanyInfo _itemCompany = new ItemCompanyInfo();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmItemCompany()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Item Category Master";
        }

        public frmItemCompany(ItemCompanyInfo itemCompany)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Item Category Master - " + itemCompany.ID.ToString();
            _itemCompany = itemCompany;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmItemCompany_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmItemCompany_Load(object sender, EventArgs e)
        {
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                txtName.Tag = _itemCompany.ID;
                txtName.Text = _itemCompany.Name;
                txtDisplayName.Text = _itemCompany.DisplayName;
                txtDisplayIndex.Text = _itemCompany.DisplayIndex.ToString();
                chkIsActive.Checked = _itemCompany.IsActive;

                txtName.Focus();
            }
        }

        bool ValidateFields()
        {
            int id;
            if (txtName.Text.Trim() == string.Empty)
            {
                txtName.Focus();
                MessageBox.Show("Please enter Item Category Name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        id = _itemCompanyManager.GetSameNameCount(txtName.Text, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Category count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Item Category already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        return false;
                    }
                }
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                {
                    try
                    {
                        id = _itemCompanyManager.GetSameNameCountForEditMode(txtName.Text, Convert.ToInt32(txtName.Tag), false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Category count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Item Category already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        return false;
                    }
                }
            }

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    id = _itemCompanyManager.GetSameNameCount(txtDisplayIndex.Text, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting same Display Index count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (id > 0)
                {
                    MessageBox.Show("Display Index already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDisplayIndex.Focus();
                    return false;
                }
            }
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    id = _itemCompanyManager.GetSameNameCountForEditMode(txtDisplayIndex.Text, Convert.ToInt32(txtName.Tag), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting same Display Index count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (id > 0)
                {
                    MessageBox.Show("Display Index already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDisplayIndex.Focus();
                    return false;
                }
            }

            int i;
            if (!int.TryParse(txtDisplayIndex.Text, out i) && (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithNetworking || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenAtCounterWithBarCode))
            {
                txtDisplayIndex.Focus();
                MessageBox.Show("Please enter display index in figures only." + Environment.NewLine + Environment.NewLine + "You can keep it 0 (zero) if you don't want to use this.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        ItemCompanyInfo SetValue()
        {
            ItemCompanyInfo retVal = new ItemCompanyInfo();

            retVal.Name = txtName.Text;
            retVal.DisplayName = txtDisplayName.Text;

            retVal.DisplayIndex = Convert.ToByte(txtDisplayIndex.Text);
            
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

            ItemCompanyInfo itemCompany = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _itemCompanyManager.AddItemCompany(itemCompany);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Item Category." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    itemCompany.ID = Convert.ToInt32(txtName.Tag);
                    _itemCompanyManager.UpdateItemCompany(itemCompany);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Item Category." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            lblHeading.Text = "New Item Category Details";
            txtDisplayName.Text = string.Empty;
            txtDisplayIndex.Text = string.Empty;
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
                lblHeading.Text = "New Item Category Details";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        private void txtDisplayName_Enter(object sender, EventArgs e)
        {
            txtDisplayName.Text = txtName.Text;
        }

        private void txtDisplayIndex_Validating(object sender, CancelEventArgs e)
        {
            int i;
            if (txtDisplayIndex.Text.Trim().Length > 0)
            {
                if (!int.TryParse(txtDisplayIndex.Text, out i))
                    txtDisplayIndex.Text = "0";
            }
            else
                txtDisplayIndex.Text = "0";
        }
    }
}