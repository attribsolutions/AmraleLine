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
    public partial class frmItemGroup : Form
    {
        #region Class level variables...

        ItemCompanyManager _itemCompanyManager = new ItemCompanyManager();
        ItemGroupManager _itemGroupManager = new ItemGroupManager();
        ItemGroupInfo _itemGroup = new ItemGroupInfo();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmItemGroup()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Item Group Master";
        }

        public frmItemGroup(ItemGroupInfo itemGroup)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Item Group Master - " + itemGroup.ID.ToString();
            _itemGroup = itemGroup;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmItemGroup_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmItemGroup_Load(object sender, EventArgs e)
        {
            FillItemCompanies();

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                cboItemCompany.SelectedValue = _itemGroup.ItemCompanyID;
                txtName.Tag = _itemGroup.ID;
                txtName.Text = _itemGroup.Name;
                txtDisplayName.Text = _itemGroup.DisplayName;
                txtDisplayIndex.Text = _itemGroup.DisplayIndex.ToString();
                chkCouponGroup.Checked = _itemGroup.CouponGroup;
                chkIsActive.Checked = _itemGroup.IsActive;

                txtName.Focus();
            }
        }

        private bool FillItemCompanies()
        {
            BindingList<ItemCompanyInfo> itemCompanies = new BindingList<ItemCompanyInfo>();
            try
            {
                itemCompanies = _itemCompanyManager.GetItemCompaniesAll(string.Empty, false, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Companies." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboItemCompany.DataSource = itemCompanies;
            cboItemCompany.DisplayMember = "Name";
            cboItemCompany.ValueMember = "ID";

            itemCompanies = null;
            return true;
        }

        bool ValidateFields()
        {
            int id;
            if (cboItemCompany.SelectedIndex == -1)
            {
                cboItemCompany.Focus();
                MessageBox.Show("Please select Category.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (txtName.Text.Trim() == string.Empty)
            {
                txtName.Focus();
                MessageBox.Show("Please enter Item Group Name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        id = _itemGroupManager.GetSameNameCount(txtName.Text, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Group count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Item Group already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        return false;
                    }
                }
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                {
                    try
                    {
                        id = _itemGroupManager.GetSameNameCountForEditMode(txtName.Text, Convert.ToInt32(txtName.Tag), false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Group count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Item Group already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        return false;
                    }
                }
            }

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    id = _itemGroupManager.GetSameNameCount(txtDisplayIndex.Text, true);
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
                    id = _itemGroupManager.GetSameNameCountForEditMode(txtDisplayIndex.Text, Convert.ToInt32(txtName.Tag), true);
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

            if (chkCouponGroup.Checked)
            {
                try
                {
                    int cnt = _itemGroupManager.CheckCouponGroupCount();
                    if (cnt > 0)
                    {
                        chkCouponGroup.Checked = false;
                        MessageBox.Show("Only one group can be assigned as coupon group.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error checking coupon group count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        ItemGroupInfo SetValue()
        {
            ItemGroupInfo retVal = new ItemGroupInfo();

            retVal.ItemCompanyID = Convert.ToInt32(cboItemCompany.SelectedValue);
            retVal.Name = txtName.Text;
            retVal.DisplayName = txtDisplayName.Text;

            retVal.DisplayIndex = Convert.ToByte(txtDisplayIndex.Text);
            
            retVal.CouponGroup = chkCouponGroup.Checked;
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

            ItemGroupInfo itemGroup = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _itemGroupManager.AddItemGroup(itemGroup);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving ItemGroup." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    itemGroup.ID = Convert.ToInt32(txtName.Tag);
                    _itemGroupManager.UpdateItemGroup(itemGroup);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating ItemGroup." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            cboItemCompany.SelectedIndex = -1;
            txtName.Text = string.Empty;
            lblHeading.Text = "New Item Group Details";
            txtDisplayName.Text = string.Empty;
            txtDisplayIndex.Text = string.Empty;
            chkIsActive.Checked = true;

            cboItemCompany.Focus();
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
                lblHeading.Text = "New Item Group Details";
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