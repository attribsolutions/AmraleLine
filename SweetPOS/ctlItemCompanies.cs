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
    public partial class ctlItemCompanies : UserControl
    {
        ItemCompanyManager _itemCompanyManager = new ItemCompanyManager();

        public ctlItemCompanies()
        {
            InitializeComponent();
            if (Program.UserRole == "Cashier")      //Hardcode
            {
                btnModify.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNewItemCompany_Click(object sender, EventArgs e)
        {
            AddItemCompany();
        }

        public bool SearchItemCompanys()
        {
            BindingList<ItemCompanyInfo> itemCompanys = new BindingList<ItemCompanyInfo>();
            try
            {
                itemCompanys = _itemCompanyManager.GetItemCompaniesAll(txtSearchText.Text, true, false);
                if (itemCompanys.Count > 0)
                {
                    grdResult.DataSource = itemCompanys;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = itemCompanys.Count.ToString() + " record(s) found.";
                itemCompanys = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Item Category by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }

        void AddItemCompany()
        {
            frmItemCompany frm = new frmItemCompany();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchItemCompanys();

                //Set focus in grid at position, where the new record added.
                for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                {
                    if (frm.NewRecordID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                    {
                        grdResult.Rows[i].Selected = true;
                        grdResult.CurrentCell = grdResult[2, i];
                        break;
                    }
                }
            }
        }

        bool EditItemCompany()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                ItemCompanyInfo itemCompany = (ItemCompanyInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmItemCompany frm = new frmItemCompany(itemCompany);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchItemCompanys();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (itemCompany.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[2, i];
                            break;
                        }
                    }
                }
            }
            return true;
        }

        bool DeleteItemCompany()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                bool isUsed = true;
                try
                {
                    isUsed = _itemCompanyManager.CheckItemCompanyUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Item Category is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _itemCompanyManager.DeleteItemCompany(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchItemCompanys();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Item Category is in use.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Item Category." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchItemCompanys();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditItemCompany();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteItemCompany();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddItemCompany();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditItemCompany();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchItemCompanys();
            }
        }
    }
}