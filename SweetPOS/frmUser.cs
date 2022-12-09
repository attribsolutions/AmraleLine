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
using ChitalePersonalzer;

namespace SweetPOS
{
    public partial class frmUser : Form
    {
        #region Class level variables...
        
        UserManager _userManager = new UserManager();
        UserInfo _user = new UserInfo();
        SettingManager _settingManager = new SettingManager();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        bool _adminModification = false;

        Communication c;

        #endregion

        #region PageLoad
        public frmUser()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - User Master";
        }

        public frmUser(UserInfo user)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - User Master";
            _user = user;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        public bool AdminModification
        { get { return _adminModification; } set { _adminModification = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmUser_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmUser_Load(object sender, EventArgs e)
        {
            cboLoginRole.SelectedIndex = -1;
            FillDivision();
            try
            {
                c = new Communication(_settingManager.GetSetting(5), Program.BAUDRATE, true);     //Hardcode (PORT Number)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting all Users." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (AdminModification)
            {
                txtName.Enabled = false;
                txtLoginName.Enabled = false;
                cboLoginRole.Enabled = false;
                chkIsActive.Enabled = false;
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                txtName.Tag = _user.ID;
                txtName.Text = _user.Name;
                txtMobile.Text = _user.Mobile;
                txtEMail.Text = _user.EMail;
                chkIsSystemUser.Checked = _user.IsSystemUser;
                if (chkIsSystemUser.Checked)
                {
                    txtLoginName.Text = _user.LoginName;
                    txtPassword.Text = _user.Password;
                    cboLoginRole.Text = _user.UserRole;
                }
                else
                    pnlLoginInfo.Enabled = false;

                //if (_user.CardID != string.Empty)
                //{
                //    //chkIsCardPersonalized.Checked = true;
                //    //txtCardID.Text = _user.CardID;
                //}
                
                chkIsActive.Checked = _user.IsActive;
                BindingList<DivisionInfo> divisions = new BindingList<DivisionInfo>();
                DivisionManager _DivisionManager = new DivisionManager();
                divisions = _DivisionManager.GetAllDivisionByUserID(_user.ID);
                FillDivisionGrid(divisions);
                txtName.Focus();
            }
        }
        private void FillDivisionGrid(BindingList<DivisionInfo> Divisions)
        {
            foreach (DivisionInfo Division in Divisions)
            {
                int row = grdDivision.Rows.Add();
                grdDivision.Rows[row].Cells["DivisionID"].Value = Division.DivisionID;
                grdDivision.Rows[row].Cells["DivisionName"].Value = Division.DivisionName;
            }
        }
        #endregion

        #region Public
        bool FillDivision()
        {
            DivisionManager _divisionManager = new DivisionManager();
            BindingList<DivisionInfo> division = new BindingList<DivisionInfo>();
            try
            {
                division = _divisionManager.GetAllDivision();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ddlDivision.DataSource = division;
            ddlDivision.DisplayMember = "DivisionName";
            ddlDivision.ValueMember = "DivisionID";
           
            division = null;
            return true;
        }

        bool ValidateFields()
        {
            int id;
            if (txtName.Text.Trim() == string.Empty)
            {
                txtName.Focus();
                MessageBox.Show("Please enter User Name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        id = _userManager.GetSameNameCount(txtLoginName.Text, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Login Name count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Login Name already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtLoginName.Focus();
                        return false;
                    }
                    if (grdDivision.Rows.Count < 1)
                    {
                        MessageBox.Show("Please Add Division.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ddlDivision.Focus();
                        return false;
                    }
                }
                if (_viewMode == EnumClass.FormViewMode.EditMode)
                {
                    try
                    {
                        id = _userManager.GetSameNameCountForEditMode(txtLoginName.Text, Convert.ToInt32(txtName.Tag), false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in getting same Login Name count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (id > 0)
                    {
                        MessageBox.Show("Login Name already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtLoginName.Focus();
                        return false;
                    }
                    if (grdDivision.Rows.Count < 1)
                    {
                        MessageBox.Show("Please Add Division.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ddlDivision.Focus();
                        return false;
                    }
                }
            }

            #region Card
            //if (chkIsCardPersonalized.Checked && txtCardID.Text == string.Empty)
            //{
            //    btnPersonalize.Focus();
            //    MessageBox.Show("Card ID cannot be blank.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            //else
            //{
            //    if (Convert.ToString(txtCardID.Tag) == "MASTER CARD" && txtLoginName.Text != "Admin")      //Hardcode
            //    {
            //        btnPersonalize.Focus();
            //        MessageBox.Show("Master Card cannot be personalized to other users.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return false;
            //    }

            //    if (_viewMode == EnumClass.FormViewMode.Addmode)
            //    {
            //        try
            //        {
            //            id = _userManager.GetSameNameCount(txtCardID.Text, true);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show("Error in getting same CardID count." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return false;
            //        }
            //        if (id > 0)
            //        {
            //            MessageBox.Show("This card is issued for another user.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            btnPersonalize.Focus();
            //            return false;
            //        }
            //    }
            //    if (_viewMode == EnumClass.FormViewMode.EditMode)
            //    {
            //        try
            //        {
            //            id = _userManager.GetSameNameCountForEditMode(txtCardID.Text, Convert.ToInt32(txtName.Tag), true);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show("Error in getting same CardID count in edit mode." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return false;
            //        }
            //        if (id > 0)
            //        {
            //            MessageBox.Show("CardID already exits.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            btnPersonalize.Focus();
            //            return false;
            //        }
            //    }
            //}

            
            #endregion

            if (chkIsSystemUser.Checked)
            {
                if (txtLoginName.Text.Trim() == string.Empty)
                {
                    txtLoginName.Focus();
                    MessageBox.Show("Please enter login name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (txtPassword.Text.Trim() == string.Empty)
                {
                    txtPassword.Focus();
                    MessageBox.Show("Please enter password.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    txtPassword.Focus();
                    MessageBox.Show("Password dosen't match.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Text = txtConfirmPassword.Text = string.Empty;
                    txtPassword.Focus();
                    return false;
                }

                if (cboLoginRole.SelectedIndex == -1)
                {
                    cboLoginRole.Focus();
                    MessageBox.Show("Please select user role.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        UserInfo SetValue(out BindingList<DivisionInfo> divisions)
        {
            UserInfo retVal = new UserInfo();
            retVal.Name = txtName.Text;
            retVal.Mobile = txtMobile.Text;
            retVal.EMail = txtEMail.Text;
            retVal.IsSystemUser = chkIsSystemUser.Checked;
            if (chkIsSystemUser.Checked)
            {
                retVal.LoginName = txtLoginName.Text;
                retVal.Password = txtPassword.Text;
                retVal.UserRole = cboLoginRole.Text;
            }
            else
            {
                retVal.LoginName = string.Empty;
                retVal.Password = string.Empty;
                retVal.UserRole = string.Empty;
            }
            
            retVal.IsActive = chkIsActive.Checked;
            
            //if (chkIsCardPersonalized.Checked)
            //    retVal.CardID = txtCardID.Text;

            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            BindingList<DivisionInfo> division = new BindingList<DivisionInfo>();

            foreach (DataGridViewRow rows in grdDivision.Rows)
            {
                DivisionInfo divi = new DivisionInfo();
                divi.DivisionID = Convert.ToInt32(rows.Cells["DivisionID"].Value);
                divi.DivisionName = Convert.ToString(rows.Cells["DivisionName"].Value);

                division.Add(divi);
            }
            divisions = division;
            return retVal;
        }

        bool Save()
        {
            if (!ValidateFields())
                return false;
            BindingList<DivisionInfo> division = new BindingList<DivisionInfo>();
            UserInfo user = SetValue(out division);

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _userManager.AddUser(user, division);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving User." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    user.ID = Convert.ToInt32(txtName.Tag);
                    _userManager.UpdateUser(user, division);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating User." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtName.Enabled = true;
            txtLoginName.Enabled = true;
            cboLoginRole.Enabled = true;
            chkIsActive.Enabled = true;

            txtName.Text = string.Empty;
            lblHeading.Text = "New User Details";
            txtMobile.Text = string.Empty;
            txtEMail.Text = string.Empty;
            chkIsSystemUser.Checked = false;
            pnlLoginInfo.Enabled = false;
            txtLoginName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            cboLoginRole.SelectedIndex = -1;
            chkIsActive.Checked = true;
            //chkIsCardPersonalized.Checked = false;
            
            txtName.Focus();
        }

        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        bool CheckItemExist(int DivID)
        {
            foreach (DataGridViewRow dr in grdDivision.Rows)
            {
                if (Convert.ToInt32(dr.Cells["DivisionID"].Value) == DivID)
                {
                    MessageBox.Show("Division already exist in the grid.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ddlDivision.Focus();
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region ControlEvent
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
                lblHeading.Text = "New User Details";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        private void chkIsSystemUser_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsSystemUser.Checked)
                pnlLoginInfo.Enabled = true;
            else
                pnlLoginInfo.Enabled = false;
        }

        private void btnAddDivision_Click_1(object sender, EventArgs e)
        {
            if (CheckItemExist(Convert.ToInt32(ddlDivision.SelectedValue)))
                return;
            int row = grdDivision.Rows.Add();
            grdDivision.Rows[row].Cells["DivisionID"].Value = ddlDivision.SelectedValue;
            grdDivision.Rows[row].Cells["DivisionName"].Value = ddlDivision.Text;
        }
        #endregion

        #region CardCode
        //private void chkIsCardPersonalized_CheckedChanged(object sender, EventArgs e)
        //{
        //    //if (chkIsCardPersonalized.Checked)
        //    //{
        //    //    txtCardID.Text = string.Empty;
        //    //    txtCardID.Tag = string.Empty;
        //    //    pnlPersonalize.Enabled = true;
        //    //}
        //    //else
        //    //    pnlPersonalize.Enabled = false;
        //}

        //private void btnPersonalize_Click(object sender, EventArgs e)
        //{
        //    txtCardID.Text = c.ReadCardID();
        //    string s = c.ReadBlock(Program.KEYSET, 2);
        //    if (s.Length >= 17 && s.Substring(5, 11) == "MASTER CARD")
        //    {
        //        txtCardID.Tag = "MASTER CARD";
        //    }
        //    else
        //        txtCardID.Tag = string.Empty;
        //}
        #endregion
    }
}