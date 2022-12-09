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
    public partial class frmLineman : Form
    {
        #region Class level variables...

        LinemanManager _linemanManager = new LinemanManager();
        LineManager _lineManager = new LineManager();
        LinemanInfo _linemanInfo = new LinemanInfo();
        
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmLineman()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Lineman Master";
        }

        public frmLineman(LinemanInfo lineman)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Lineman Master - " + lineman.ID.ToString();
            txtLinemanName.Tag = lineman.ID.ToString();
            _linemanInfo = lineman;
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
            FillLines();

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                txtLinemanName.Text = _linemanInfo.Name;
                cboLine.SelectedValue = _linemanInfo.LineId;
                txtMobile.Text = _linemanInfo.Mobile;
                txtCity.Text = _linemanInfo.City;
                txtAddress.Text = _linemanInfo.Address;
                txtCommission.Text = Convert.ToString(_linemanInfo.Commission);
                chkIsActive.Checked = _linemanInfo.IsActive;
                txtLinemanName.Focus();
            }
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
                MessageBox.Show("Error in getting Lines." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboLine.DataSource = lines;
            cboLine.DisplayMember = "Name";
            cboLine.ValueMember = "ID";
            lines = null;
            cboLine.SelectedIndex = -1;
            return true;
        }

        private void AutoComplete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
        }

        bool ValidateFields()
        {
            
            if (txtLinemanName.Text.Trim() == string.Empty)
            {
                txtLinemanName.Focus();
                MessageBox.Show("Please enter lineman name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboLine.Text.Trim() == string.Empty)
            {
                cboLine.Focus();
                MessageBox.Show("Please select line name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            int val;
            if (txtCommission.Text.Trim() == string.Empty)
            {
                txtCommission.Focus();
                MessageBox.Show("Please enter commission.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        LinemanInfo SetValue()
        {
            LinemanInfo retVal = new LinemanInfo();

            retVal.Name = txtLinemanName.Text;
            retVal.LineId = Convert.ToInt32(cboLine.SelectedValue);
            retVal.Mobile = Convert.ToString(txtMobile.Text);
            retVal.City = txtCity.Text;
            retVal.Address = txtAddress.Text;
            retVal.Commission = Convert.ToString(txtCommission.Text) != "" ? Convert.ToDecimal(txtCommission.Text) : 0;
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

            LinemanInfo lineman = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _linemanManager.AddLineman(lineman);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Lineman." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    lineman.ID = Convert.ToInt32(txtLinemanName.Tag);
                    _linemanManager.UpdateLineman(lineman);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Lineman." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtLinemanName.Text = txtMobile.Text = txtCity.Text = txtAddress.Text =txtCommission.Text = string.Empty;
            cboLine.SelectedValue = 0;
            chkIsActive.Checked = false;
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

        private void txtCommission_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

    }
}