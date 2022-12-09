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
    public partial class ctlItems : UserControl
    {
        ItemManager _itemManager = new ItemManager();
        bool _allowEdit = true;

        public ctlItems()
        {
            InitializeComponent();
            cboSearchIn.SelectedIndex = 0;
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

        public bool SearchItems()
        {
            BindingList<ItemInfo> items = new BindingList<ItemInfo>();
            try
            {
                items = _itemManager.GetItemsByFilter(cboSearchIn.SelectedIndex + 1, txtSearchText.Text, 10000);
                if (items.Count > 0)
                {
                    grdResult.DataSource = items;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = items.Count.ToString() + " record(s) found.";
                items = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Item by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].FillWeight = 30;
            grdResult.Columns["ID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            grdResult.Columns["MainBranchCode"].Visible = false;
            grdResult.Columns["DisplayName"].Visible = false;
            grdResult.Columns["ItemGroupID"].Visible = false;
            grdResult.Columns["UnitID"].Visible = false;
            grdResult.Columns["ReorderLevel"].Visible = false;
            grdResult.Columns["ReorderQuantity"].Visible = false;
            grdResult.Columns["LastPurchaseRate"].Visible = false;
            //grdResult.Columns["Rate"].Visible = false;
            //grdResult.Columns["Gst"].Visible = false;
            grdResult.Columns["IsActive"].Visible = false;
            grdResult.Columns["ShowFixed"].Visible = false;
            grdResult.Columns["CounterID"].Visible = false;
            grdResult.Columns["FixedDisplayIndex"].Visible = false;

            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;

            grdResult.Columns["ItemGroup"].DisplayIndex = 6;
            grdResult.Columns["Unit"].DisplayIndex = 7;

            grdResult.Columns["ItemCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["Gst"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["Rate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["ReorderQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            grdResult.Columns["ItemCode"].HeaderText = "Code";
            grdResult.Columns["ItemCode"].FillWeight = 40;
            grdResult.Columns["Unit"].FillWeight = 40;
            grdResult.Columns["Gst"].FillWeight = 40;
            grdResult.Columns["Rate"].FillWeight = 50;
            grdResult.Columns["ReorderQuantity"].FillWeight = 60;
            grdResult.Columns["ReorderQuantity"].HeaderText = "Reorder";
            grdResult.Columns["IsActive"].FillWeight = 50;
            grdResult.Columns["ShowFixed"].FillWeight = 50;
        }

        void AddItem()
        {
            frmItem frm = new frmItem();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchItems();

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
                ItemInfo item = (ItemInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmItem frm = new frmItem(item);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchItems();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (item.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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
                bool isUsed = true;
                try
                {
                    isUsed = _itemManager.CheckItemUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Item is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _itemManager.DeleteItem(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchItems();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Item is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Item." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchItems();
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
                SearchItems();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ReportClass rptClass = new ReportClass();
            if (cboSearchIn.SelectedIndex == 1)    //Hardcode
                rptClass.ShowItems(true, txtSearchText.Text, false);
            else
            {
                rptClass.ShowItems(false, string.Empty, false);
                rptClass.ShowItems(false, string.Empty, true);
            }
        }

        private void txtSearchText_TextChanged(object sender, EventArgs e)
        {
            if (cboSearchIn.SelectedIndex == 1)        //Hardcode
                btnPrint.Text = "&Print for group starts with " + txtSearchText.Text;
            else
                btnPrint.Text = "&Print All";
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearchText.Text = string.Empty;
        }
    }
}