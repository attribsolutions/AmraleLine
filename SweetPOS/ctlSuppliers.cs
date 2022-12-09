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
    public partial class ctlSuppliers : UserControl
    {
        SupplierManager _supplierManager = new SupplierManager();

        public ctlSuppliers()
        {
            InitializeComponent();
            if (Program.UserRole == "Cashier")      //Hardcode
            {
                btnDelete.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNewSupplier_Click(object sender, EventArgs e)
        {
            AddSupplier();
        }

        public bool SearchSuppliers()
        {
            BindingList<SupplierInfo> suppliers = new BindingList<SupplierInfo>();
            try
            {
                suppliers = _supplierManager.GetSuppliersByName(txtSearchText.Text, false);
                if (suppliers.Count > 0)
                {
                    grdResult.DataSource = suppliers;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = suppliers.Count.ToString() + " record(s) found.";
                suppliers = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Supplier by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["Address"].Visible = false;
            grdResult.Columns["State"].Visible = false;

            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;

            grdResult.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        void AddSupplier()
        {
            frmSupplier frm = new frmSupplier();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchSuppliers();

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

        bool EditSupplier()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                SupplierInfo supplier = (SupplierInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmSupplier frm = new frmSupplier(supplier);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchSuppliers();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (supplier.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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

        bool DeleteSupplier()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                string tableName = string.Empty;
                bool isUsed = true;
                try
                {
                    isUsed = _supplierManager.CheckSupplierUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Supplier is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _supplierManager.DeleteSupplier(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchSuppliers();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Supplier is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Supplier." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchSuppliers();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditSupplier();
        }

        private void btnModifySupplier_Click(object sender, EventArgs e)
        {
            EditSupplier();
        }

        private void btnDeleteSupplier_Click(object sender, EventArgs e)
        {
            DeleteSupplier();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchSuppliers();
            }
        }
    }
}