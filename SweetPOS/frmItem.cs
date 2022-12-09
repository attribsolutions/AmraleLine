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
    public partial class frmItem : Form
    {
        #region Class level variables...
        
        ItemManager _itemManager = new ItemManager();
        ItemInfo _item = new ItemInfo();
        ItemGroupManager _itemGroupManager = new ItemGroupManager();
        UnitManager _unitManager = new UnitManager();
        CounterManager _counterManager = new CounterManager();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmItem()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Item Master";
        }

        public frmItem(ItemInfo item)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Item Master - " + item.ID.ToString();
            _item = item;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmItem_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmItem_Load(object sender, EventArgs e)
        {
            AutoCompleteForItemName();
            FillItemGroups();
            FillUnits();
            cboItemGroup.SelectedIndex = -1;
            cboUnit.SelectedIndex = -1;
            cboCounter.SelectedIndex = -1;

            if (Program.IsItemCodeAsID)
                chkAsPerID.Checked = true;
            else
                CheckChanged();

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                txtBarCode.Tag = _item.ID;
                txtBarCode.Text = _item.BarCode;
                txtItemCode.Text = _item.ItemCode.ToString("0");
                txtMainBranchCode.Text = _item.MainBranchCode.ToString("0");
                txtName.Text = _item.Name;
                txtDisplayName.Text = _item.DisplayName;
                txtDisplayIndex.Text = _item.DisplayIndex.ToString();
                cboItemGroup.SelectedValue = _item.ItemGroupID;
                cboUnit.SelectedValue = _item.UnitID;
                txtVat.Text = _item.Gst.ToString("0.00");
                txtRate.Text = _item.Rate.ToString("0.00");
                
                if (cboUnit.Text.Trim() == "No.")      //Hardcode
                {
                    lblUnitWeight.Visible = true;
                    txtUnitWeight.Visible = true;
                    txtUnitWeight.Text = _item.UnitWeight.ToString("0.000");
                }
                chkIsActive.Checked = _item.IsActive;
                chkShowFixed.Checked = _item.ShowFixed;
                txtFixedDisplayIndex.Text = _item.FixedDisplayIndex.ToString();
                cboCounter.SelectedValue = _item.CounterID;
                chkRoundOff.Checked = _item.IsRoundOff;
                txtBarCode.Focus();
            }
        }

        private void AutoCompleteForItemName()
        {
            BindingList<ItemInfo> items = null;
            try
            {
                items = _itemManager.GetItemsAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AutoCompleteStringCollection autoSource = new AutoCompleteStringCollection();
            foreach (ItemInfo item in items)
            {
                autoSource.Add(item.Name);
            }
            txtName.AutoCompleteCustomSource = autoSource;
        }

        private void AutoComplete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
        }

        bool FillItemGroups()
        {
            BindingList<ItemGroupInfo> itemGroups = new BindingList<ItemGroupInfo>();
            try
            {
                itemGroups = _itemGroupManager.GetItemGroupsByName(string.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item Groups." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboItemGroup.DataSource = itemGroups;
            cboItemGroup.DisplayMember = "Name";
            cboItemGroup.ValueMember = "ID";

            itemGroups = null;
            return true;
        }

        bool FillUnits()
        {
            BindingList<UnitInfo> units = new BindingList<UnitInfo>();
            try
            {
                units = _unitManager.GetUnitsAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Units." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboUnit.DataSource = units;
            cboUnit.DisplayMember = "Name";
            cboUnit.ValueMember = "ID";

            units = null;
            return true;
        }

        bool FillCounters()
        {
            BindingList<CounterInfo> counters = new BindingList<CounterInfo>();
            try
            {
                counters = _counterManager.GetCountersAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Couners." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboCounter.DataSource = counters;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "ID";

            counters = null;
            return true;
        }

        bool ValidateFields()
        {
            int id = 0;

            if (!int.TryParse(txtItemCode.Text, out id))
            {
                txtItemCode.Focus();
                MessageBox.Show("Please enter Item Code.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        id = _itemManager.GetSameNameCount(txtItemCode.Text, 2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Code count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0 && txtItemCode.Text.Trim() != "0")
                    {
                        MessageBox.Show("Item Code already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtItemCode.Focus();
                        return false;
                    }
                }
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                {
                    try
                    {
                        id = _itemManager.GetSameNameCountForEditMode(txtItemCode.Text, Convert.ToInt32(txtBarCode.Tag), 2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Code count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0 && txtItemCode.Text.Trim() != "0")
                    {
                        MessageBox.Show("Item Code already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtItemCode.Focus();
                        return false;
                    }
                }
            }

            if (txtName.Text.Trim() == string.Empty)
            {
                txtName.Focus();
                MessageBox.Show("Please enter Item Name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        id = _itemManager.GetSameNameCount(txtName.Text, 1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Name count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Item Name already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        return false;
                    }
                }
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                {
                    try
                    {
                        id = _itemManager.GetSameNameCountForEditMode(txtName.Text, Convert.ToInt32(txtBarCode.Tag), 1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Item Name count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Item Name already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        return false;
                    }
                }
            }

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    id = _itemManager.GetSameNameCount(txtBarCode.Text, 4);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting same Barcode count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (id > 0 && txtBarCode.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("Bar Code already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarCode.Focus();
                    return false;
                }
            }
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    id = _itemManager.GetSameNameCountForEditMode(txtBarCode.Text, Convert.ToInt32(txtBarCode.Tag), 4);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting same Barcode count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (id > 0 && txtBarCode.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("Bar code already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarCode.Focus();
                    return false;
                }
            }

            int displayIndex;
            if (txtDisplayIndex.Text.Trim() == string.Empty)
                txtDisplayIndex.Text = "0";

            if (!int.TryParse(txtDisplayIndex.Text, out displayIndex) || displayIndex < 0)
            {
                txtDisplayIndex.Focus();
                MessageBox.Show("Please enter display index numeric only.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboItemGroup.SelectedIndex == -1)
            {
                cboItemGroup.Focus();
                MessageBox.Show("Please select group name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (displayIndex != 0)
            {
                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        id = _itemManager.GetSameNameCount(displayIndex.ToString() + "," + Convert.ToString(cboItemGroup.SelectedValue), 5);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Display index count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0 && (txtDisplayIndex.Text.Trim() != "0" && txtDisplayIndex.Text.Trim() != "99"))
                    {
                        MessageBox.Show("Display index already exist in selected Item Group.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDisplayIndex.Focus();
                        //return false;
                    }
                }
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                {
                    try
                    {
                        id = _itemManager.GetSameNameCountForEditMode(displayIndex.ToString() + "," + Convert.ToString(cboItemGroup.SelectedValue), Convert.ToInt32(txtBarCode.Tag), 5);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Display index count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0 && (txtDisplayIndex.Text.Trim() != "0" && txtDisplayIndex.Text.Trim() != "99"))
                    {
                        MessageBox.Show("Display index already exist in selected Item Group.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDisplayIndex.Focus();
                        //return false;
                    }
                }
            }

            if (cboUnit.SelectedIndex == -1)
            {
                cboUnit.Focus();
                MessageBox.Show("Please select unit.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (chkShowFixed.Checked)
            {
                try
                {
                    if (_itemManager.CheckFixedItemCount(Program.FixedItemCount))    //Hardcode      As per form design
                    {
                        chkShowFixed.Checked = false;
                        MessageBox.Show("Fixed item count exceeds limit.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking fixed item count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                int fixedDisplayIndex;
                if (txtFixedDisplayIndex.Text.Trim() == string.Empty)
                    txtFixedDisplayIndex.Text = "0";

                if (!int.TryParse(txtFixedDisplayIndex.Text, out fixedDisplayIndex) || fixedDisplayIndex < 0)
                {
                    txtFixedDisplayIndex.Focus();
                    MessageBox.Show("Please enter fixed index numeric only.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (fixedDisplayIndex != 0)
                {
                    if (_viewMode == EnumClass.FormViewMode.Addmode)
                    {
                        try
                        {
                            id = _itemManager.GetSameNameCount(fixedDisplayIndex.ToString(), 6);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error in getting same Fixed index count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        if (id > 0)
                        {
                            MessageBox.Show("Fixed index already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtFixedDisplayIndex.Focus();
                            return false;
                        }
                    }
                    if (_viewMode == EnumClass.FormViewMode.EditMode)
                    {
                        try
                        {
                            id = _itemManager.GetSameNameCountForEditMode(fixedDisplayIndex.ToString(), Convert.ToInt32(txtBarCode.Tag), 6);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error in getting same Fixed index count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        if (id > 0)
                        {
                            MessageBox.Show("Fixed index already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtFixedDisplayIndex.Focus();
                            return false;
                        }
                    }
                }
            }

            try
            {
                ItemGroupInfo itemGroup = _itemGroupManager.GetItemGroup(Convert.ToInt32(cboItemGroup.SelectedValue));
                if (itemGroup.CouponGroup && chkShowFixed.Checked)
                {
                    MessageBox.Show("Not possible to fix the item from coupon group type.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting itemgroup by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        ItemInfo SetValue()
        {
            ItemInfo retVal = new ItemInfo();

            retVal.ItemCode = Convert.ToInt32(txtItemCode.Text);
            retVal.MainBranchCode = Convert.ToInt32(txtMainBranchCode.Text);
            retVal.BarCode = txtBarCode.Text;
            retVal.Name = txtName.Text;
            retVal.DisplayName= txtDisplayName.Text;
            retVal.DisplayIndex = Convert.ToInt32(txtDisplayIndex.Text);
            retVal.ItemGroupID = Convert.ToInt32(cboItemGroup.SelectedValue);
            retVal.UnitID = Convert.ToInt32(cboUnit.SelectedValue);
            
            if (cboUnit.Text.Trim() == "No.")      //Hardcode
            {
                retVal.UnitWeight = Convert.ToDecimal(txtUnitWeight.Text);
            }
            else
            {
                retVal.UnitWeight = 0;
            }
            retVal.IsActive = chkIsActive.Checked;
            retVal.IsRoundOff = chkRoundOff.Checked;
            retVal.ShowFixed = chkShowFixed.Checked;
            retVal.CounterID = Convert.ToInt32(cboCounter.SelectedValue);
            retVal.FixedDisplayIndex = Convert.ToInt32(txtFixedDisplayIndex.Text);
            retVal.Gst = Convert.ToDecimal(txtVat.Text);
            retVal.Rate = Convert.ToDecimal(txtRate.Text);
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

            ItemInfo item = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _itemManager.AddItem(item);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Item." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    item.ID = Convert.ToInt32(txtBarCode.Tag);
                    _itemManager.UpdateItem(item);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Item." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        void GetMaxItemID(byte codeId)
        {
            try
            {
                txtItemCode.Text = (_itemManager.GetMaxItemID(codeId)).ToString("0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Max of Item Code/Id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";

            txtMainBranchCode.Text = "0";
            txtDisplayIndex.Text = "99";
            txtBarCode.Text = string.Empty;
            txtName.Text = string.Empty;
            lblHeading.Text = "New Item Details";
            txtDisplayName.Text = string.Empty;
            cboItemGroup.SelectedIndex = -1;
            cboUnit.SelectedIndex = -1;
            //txtVat.Text = "0.00";
            //txtRate.Text = "0.00";
            
            txtUnitWeight.Text = "0.000";
            lblUnitWeight.Visible = txtUnitWeight.Visible = false;
            chkIsActive.Checked = true;
            chkShowFixed.Checked = false;
            lblFixedDisplayIndex.Visible = txtFixedDisplayIndex.Visible = cboCounter.Visible = lblSelectCounter.Visible = false;
            cboCounter.SelectedIndex = -1;

            AutoCompleteForItemName();
            CheckChanged();
            txtItemCode.Focus();
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
                lblHeading.Text = "New Item Details";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        private void txtDisplayName_Enter(object sender, EventArgs e)
        {
            txtDisplayName.Text = txtName.Text;
        }

        private void Numeric_Validating(object sender, CancelEventArgs e)
        {
            if (((TextBox)sender).Text.Trim() != string.Empty)
            {
                decimal d = 0.0M;
                if (!decimal.TryParse(((TextBox)sender).Text, out d))
                    e.Cancel = true;
                else
                {
                    if (((TextBox)sender).Name == "txtVat" || ((TextBox)sender).Name == "txtRate" || ((TextBox)sender).Name == "txtLastPurchaseRate")
                        ((TextBox)sender).Text = d.ToString("0.00");
                    else if (((TextBox)sender).Name == "txtReorderLevel" || ((TextBox)sender).Name == "txtReorderQuantity" || ((TextBox)sender).Name == "txtUnitWeight")
                        ((TextBox)sender).Text = d.ToString("0.000");
                    else
                        ((TextBox)sender).Text = d.ToString("0");
                }
            }
            else
            {
                if (((TextBox)sender).Name == "txtVat" || ((TextBox)sender).Name == "txtRate" || ((TextBox)sender).Name == "txtLastPurchaseRate")
                    ((TextBox)sender).Text = "0.00";
                else if (((TextBox)sender).Name == "txtReorderQuantity" || ((TextBox)sender).Name == "txtReorderQuantity" || ((TextBox)sender).Name == "txtUnitWeight")
                    ((TextBox)sender).Text = "0.000";
                else
                    ((TextBox)sender).Text = "0";
            }
        }

        private void chkAsPerID_CheckedChanged(object sender, EventArgs e)
        {
            CheckChanged();
        }

        private void CheckChanged()
        {
            if (chkAsPerID.Checked)
            {
                txtItemCode.Enabled = false;
                GetMaxItemID(1);
            }
            else
            {
                txtItemCode.Enabled = true;
                GetMaxItemID(2);
            }
        }

        private void cboUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboUnit.Text.Trim() == "No.")      //Hardcode
            {
                lblUnitWeight.Visible = txtUnitWeight.Visible = true;
            }
            else
            {
                lblUnitWeight.Visible = txtUnitWeight.Visible = false;
            }
        }

        private void chkShowFixed_CheckedChanged(object sender, EventArgs e)
        {
            lblFixedDisplayIndex.Visible = txtFixedDisplayIndex.Visible = lblSelectCounter.Visible = cboCounter.Visible = chkShowFixed.Checked;
        }

    }
}