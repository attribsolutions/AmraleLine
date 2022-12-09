using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;



namespace SweetPOS
{
    public partial class ctlMessages : UserControl
    {
        
        MessageManager _messageManager = new MessageManager();
        bool _allowEdit = true;


        public ctlMessages()
        {
            InitializeComponent();

            if (Program.UserRole == "Cashier")      //Hardcode
            {
                btnModify.Enabled = false;
                btnDelete.Enabled = false;

                _allowEdit = false;
            }


        }

        private void btnMsgClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchMessages();
        }

        private void SearchMessages()
        {
            BindingList<MessageInfo> messages = new BindingList<MessageInfo>();
            try
            {
                messages = _messageManager.GetAllMessages(txtSearchText.Text,1000);
                if (messages.Count > 0)
                {
                    grdResult.DataSource = messages;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = messages.Count.ToString() + " record(s) found.";
                messages = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting  messages." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

        }

        private void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
           
        }

        private void btnNewMessage_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        private void AddItem()
        {
            frmMessage frm = new frmMessage();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchMessages();
                
                // i'll wriete here = Set focus in grid at position, where the new record added.
            
            
            }
        }

        private void btnMessageModify_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        
       bool EditItem()
        {
            if (grdResult.SelectedRows.Count > 0 && _allowEdit)
            {
                MessageInfo Msg = (MessageInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmMessage frm = new frmMessage(Msg);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchMessages();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (Msg.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[1, i];
                            break;
                        }
                    }
                }
            }
            return true;
        
        }

            private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
            {
                EditItem();
            }

            private void btnMessageDelete_Click(object sender, EventArgs e)
            {
                DeleteItem();
            }

           bool DeleteItem()
            {
                        if (grdResult.SelectedRows.Count > 0)
                     {
                         string tableName = string.Empty;
                        bool isUsed = false;
                        try
                        {
                            if (!isUsed)
                            {
                                DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (dr == DialogResult.Yes)
                                {
                                    _messageManager.DeleteMessage(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                                    SearchMessages();
                                    return true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot delete, message is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting message." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    return false;
             }
    }
}
