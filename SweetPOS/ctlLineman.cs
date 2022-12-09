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
    public partial class ctlLineman : UserControl
    {
        LinemanManager _linemanManager = new LinemanManager();
        bool _allowEdit = true;

        public ctlLineman()
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

        public bool SearchLinemans()
        {
            BindingList<LinemanInfo> lines = new BindingList<LinemanInfo>();
            try
            {
                lines = _linemanManager.GetLinemanByFilter(txtSearchText.Text, 10000);
                if (lines.Count > 0)
                {
                    grdResult.DataSource = lines;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = lines.Count.ToString() + " record(s) found.";
                lines = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Lineman by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["FromDate"].Visible = false;
            grdResult.Columns["MilkIssueID"].Visible = false;
            grdResult.Columns["LineID"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
            grdResult.Columns["LineNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdResult.Columns["LineName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
        }

        void AddItem()
        {
            frmLineman frm = new frmLineman();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchLinemans();

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
                LinemanInfo lineman = (LinemanInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmLineman frm = new frmLineman(lineman);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchLinemans();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (lineman.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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
                            _linemanManager.DeleteLineman(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchLinemans();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Lineman is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Lineman." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchLinemans();
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
                SearchLinemans();
            }
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearchText.Text = string.Empty;
        }

        private void grdResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}