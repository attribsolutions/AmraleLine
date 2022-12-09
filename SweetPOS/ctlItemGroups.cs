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
    public partial class ctlItemGroups : UserControl
    {
        ItemGroupManager _itemGroupManager = new ItemGroupManager();

        public ctlItemGroups()
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

        private void btnNewItemGroup_Click(object sender, EventArgs e)
        {
            AddItemGroup();
        }

        public bool SearchItemGroups()
        {
            BindingList<ItemGroupInfo> itemGroups = new BindingList<ItemGroupInfo>();
            try
            {
                itemGroups = _itemGroupManager.GetItemGroupsByName(txtSearchText.Text);
                if (itemGroups.Count > 0)
                {
                    grdResult.DataSource = itemGroups;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = itemGroups.Count.ToString() + " record(s) found.";
                itemGroups = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting ItemGroup by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["ItemCompanyID"].Visible = false;

            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
        }

        void AddItemGroup()
        {
            frmItemGroup frm = new frmItemGroup();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchItemGroups();

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

        bool EditItemGroup()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                ItemGroupInfo itemGroup = (ItemGroupInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmItemGroup frm = new frmItemGroup(itemGroup);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchItemGroups();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (itemGroup.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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

        bool DeleteItemGroup()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                bool isUsed = true;
                try
                {
                    isUsed = _itemGroupManager.CheckItemGroupUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether ItemGroup is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _itemGroupManager.DeleteItemGroup(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchItemGroups();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, ItemGroup is in use.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting ItemGroup." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchItemGroups();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditItemGroup();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteItemGroup();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddItemGroup();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditItemGroup();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchItemGroups();
            }
        }
    }
}