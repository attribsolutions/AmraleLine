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
    public partial class frmCustomerMessages : Form
    {
        #region Class level variables...

        CustomerMessageManager _messageManger = new CustomerMessageManager();
        CustomerMessageInfo _messageInfo = new CustomerMessageInfo();
        Boolean isLineFilled = false;

        LinemanManager _linemanManager = new LinemanManager();
        LinemanInfo _linemanInfo = new LinemanInfo();

        LineManager _lineManager = new LineManager();
        MessageManager _messageManager = new MessageManager();
        
        
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmCustomerMessages()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Lineman Master";
        }

        public frmCustomerMessages(CustomerMessageInfo customerMessage)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Lineman Master - " + customerMessage.ID.ToString();
            txtCustomerMessage.Tag = customerMessage.ID.ToString();
            _messageInfo = customerMessage;
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
            FillMessages();
            FillCustomersByLineID(0);

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";
                chkIsComplated.Visible = true;

                dtSDate.Value = Convert.ToDateTime(_messageInfo.FromDate);
                dtEDate.Value = Convert.ToDateTime(_messageInfo.ToDate);
                cboLine.SelectedValue = _messageInfo.LineID;
                cboCustomer.SelectedValue = _messageInfo.CustomerID;
                txtCustomerMessage.Text = _messageInfo.Message;
                cboMessage.Text = _messageInfo.DefaultMessage;
                if (_messageInfo.IsComplated == "Yes")
                {
                    chkIsComplated.Checked = true;
                }
                else { chkIsComplated.Checked = false; }
            }
        }
        
        bool FillCustomersByLineID(int LineID)
        {
            BindingList<CustomerInfo> customer = new BindingList<CustomerInfo>();
            CustomerManager _customerManager = new CustomerManager();
            try
            {
                customer = _customerManager.GetCustomersByLineID(LineID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cboCustomer.DataSource = customer;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "ID";
            customer = null;
            cboCustomer.SelectedIndex = -1;

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
                MessageBox.Show("Error in getting Item Groups." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboLine.DataSource = lines;
            cboLine.DisplayMember = "LineNumber";
            cboLine.ValueMember = "ID";
            lines = null;
            cboLine.SelectedIndex = -1;
            isLineFilled = true;
            return true;
        }

        bool FillMessages()
        {
            BindingList<MessageInfo> message = new BindingList<MessageInfo>();
           
            try
            {
                message = _messageManager.GetMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Messages." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboMessage.DataSource = message;
            cboMessage.DisplayMember = "MESSAGE";
            cboMessage.ValueMember = "ID";
            message = null;
            cboMessage.SelectedIndex = -1;
            //isLineFilled = true;
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
            if (cboLine.Text.Trim() == string.Empty)
            {
                cboLine.Focus();
                MessageBox.Show("Please enter line.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //if (cboCustomer.Text.Trim() == string.Empty)
            //{
            //    cboCustomer.Focus();
            //    MessageBox.Show("Please enter customer.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}

            if (txtCustomerMessage.Text.Trim() == string.Empty)
            {
                txtCustomerMessage.Focus();
                MessageBox.Show("Please enter customer message.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboMessage.Text.Trim() == string.Empty)
            {
                cboMessage.Focus();
                MessageBox.Show("Please enter select message.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            
            }
            //if ( dtSDate.Value>dtEDate.Value)
            //{
            //    dtEDate.Focus();
            //    MessageBox.Show("Please Select End Date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            return true;
        }

        CustomerMessageInfo SetValue()
        {
            CustomerMessageInfo retVal = new CustomerMessageInfo();

            retVal.FromDate = Convert.ToDateTime(dtSDate.Value).Date;
            retVal.ToDate = Convert.ToDateTime(dtEDate.Value).Date;
            retVal.CustomerID = Convert.ToInt32(cboCustomer.SelectedValue);
            retVal.DefaultMessage = Convert.ToString(cboMessage.Text);
            retVal.Message = Convert.ToString(txtCustomerMessage.Text);
            if (chkIsComplated.Checked)
            {   retVal.IsComplated = "Yes";}
            else
            { retVal.IsComplated = "No"; }
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

            CustomerMessageInfo CustMsg = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _messageManger.AddCustomerMessage(CustMsg);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Customer Message." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    CustMsg.ID = Convert.ToInt32(txtCustomerMessage.Tag);
                    _messageManger.UpdateCustomerMessage(CustMsg);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Customer Message." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtCustomerMessage.Text = string.Empty;
            cboLine.SelectedValue = cboCustomer.SelectedValue = 0;
            //txtLinemanName.Text = txtMobile.Text = txtCity.Text = txtAddress.Text =txtCommission.Text = string.Empty;
            //cboLine.SelectedValue = 0;
            //chkIsActive.Checked = false;
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

        private void cboLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLineFilled)
            {
                FillCustomersByLineID(Convert.ToInt32(cboLine.SelectedValue));
                cboCustomer.SelectedIndex = -1;
                cboCustomer.Text = string.Empty;
            }
        }

        private void dtEDate_ValueChanged(object sender, EventArgs e)
        {
            //if (dtEDate.Value < dtSDate.Value)
            //{
            //    dtEDate.Focus();
            //    MessageBox.Show("Please Select End Date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
            //}
        }
        private void cboMessage_TextChanged(object sender, EventArgs e)
        {
            txtCustomerMessage.Text = cboMessage.Text;
        }

        private void dtSDate_ValueChanged(object sender, EventArgs e)
        {
            dtEDate.Value = dtSDate.Value;
        }

       

    }
}