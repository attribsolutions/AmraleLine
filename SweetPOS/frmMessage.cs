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
    public partial class frmMessage : Form

    {
        MessageManager _messageManger = new MessageManager();
        MessageInfo _messageInfo = new MessageInfo();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
         int _newRecordID = 0;
        bool _newRecordAdded = false;

        public frmMessage()
        {
            InitializeComponent();
              this.Text = Program.MESSAGEBOXTITLE + " - Message Master";
        }


        public frmMessage(MessageInfo Message)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Message Master - " + Message.ID.ToString();
            txtMessage.Tag = Message.ID.ToString();
            _messageInfo = Message;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }
        private void lblMessage_Click(object sender, EventArgs e)
        {

        }

        private void txtCustomerMessage_TextChanged(object sender, EventArgs e)
        {

        }

      
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (Save())
                New();

        }

        private void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";

            txtMessage.Text = string.Empty;
            
        }

        bool ValidateFields()
        {
            if (txtMessage.Text.Trim() == string.Empty)
            {
                txtMessage.Focus();
                MessageBox.Show("Please enter Mesage.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

          MessageInfo SetValue()
        {
            MessageInfo retVal = new MessageInfo();

            retVal.Message = Convert.ToString(txtMessage.Text);
            
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

              MessageInfo newMsg = SetValue();

              if (_viewMode == EnumClass.FormViewMode.Addmode)
              {
                  try
                  {
                      _newRecordID = _messageManger.AddMessage(newMsg);
                      _newRecordAdded = true;
                      return true;
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show("Error in saving Message." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                      return false;
                  }
              }


              if (_viewMode == EnumClass.FormViewMode.EditMode)
              {
                  try
                  {
                      newMsg.ID = Convert.ToInt32(txtMessage.Tag);
                      _messageManger.UpdateMessage(newMsg);
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

          private void frmMessage_Load(object sender, EventArgs e)
          {
              if (_viewMode == EnumClass.FormViewMode.EditMode)
              {
                  btnSaveNew.Text = "Update && Ne&w";
                  btnSaveClose.Text = "Update && Cl&ose";
                  txtMessage.Text = _messageInfo.Message;
              }
          }

          private void btnClear_Click(object sender, EventArgs e)
          {
              New();
          }
    }
}
