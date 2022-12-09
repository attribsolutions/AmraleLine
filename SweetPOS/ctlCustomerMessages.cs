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
    public partial class ctlCustomerMessages : UserControl
    {
        CustomerMessageManager _messageManager = new CustomerMessageManager();
        LineManager _lineManager = new LineManager();
        bool _allowEdit = true;

        public ctlCustomerMessages()
        {
            InitializeComponent();
            //cboSearchIn.SelectedIndex = 0;
            if (Program.UserRole == "Cashier")      //Hardcode
            {
                btnModify.Enabled = false;
                btnDelete.Enabled = false;

                _allowEdit = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        public bool SearchCustomerMessages()
        {
            BindingList<CustomerMessageInfo> messages = new BindingList<CustomerMessageInfo>();
            try
            {
                messages = _messageManager.GetCustomerMessagesByFilter(txtSearchText.Text, 1000);
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
                MessageBox.Show("Error getting Messages." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
               return false;
            }

            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
            grdResult.Columns["CustomerID"].Visible = false;
            grdResult.Columns["LineID"].Visible = false;
        }

        void AddItem()
        {
            frmCustomerMessages frm = new frmCustomerMessages();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchCustomerMessages();

                //Set focus in grid at position, where the new record added.
                for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                {
                    if (frm.NewRecordID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                    {
                        grdResult.Rows[i].Selected = true;
                        grdResult.CurrentCell = grdResult[1, i];
                        break;
                    }
                }
            }
        }

        bool EditItem()
        {
            if (grdResult.SelectedRows.Count > 0 && _allowEdit)
            {
                CustomerMessageInfo CustomerMsg = (CustomerMessageInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmCustomerMessages frm = new frmCustomerMessages(CustomerMsg);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchCustomerMessages();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (CustomerMsg.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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

        bool DeleteItem()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                string tableName = string.Empty;
                //bool isUsed = true;
                bool isUsed = false;
                //try
                //{
                //    isUsed = _itemManager.CheckItemUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Error in checking whether Item is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return false;
                //}
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _messageManager.DeleteCustomerMessage(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchCustomerMessages();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, customer message is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting customer message." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchCustomerMessages();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditItem();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchCustomerMessages();
            }
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearchText.Text = string.Empty;
        }

        private void grdResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panelEx7_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblRecordsFound_Click(object sender, EventArgs e)
        {

        }

        private void lblHeading_Click(object sender, EventArgs e)
        {

        }

        private void panelEx5_Click(object sender, EventArgs e)
        {

        }

        private void panelEx6_Click(object sender, EventArgs e)
        {

        }

        private void panelEx3_Click(object sender, EventArgs e)
        {

        }

        private void panelEx4_Click(object sender, EventArgs e)
        {

        }

        private void panelEx1_Click(object sender, EventArgs e)
        {

        }

        private void txtSearchText_TextChanged(object sender, EventArgs e)
        {

        }

        private void panelEx2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

    }
}