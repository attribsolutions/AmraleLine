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
    public partial class ctlCustomers : UserControl
    {
        CustomerManager _customerManager = new CustomerManager();

        public ctlCustomers()
        {
            InitializeComponent();            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cboSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchBy.SelectedIndex == 1)
            {
                lblCustomerName.Text = "Customer Number";
                cboLines.Visible = false;
                txtSearchText.Visible = true;
            }
            else if (cboSearchBy.SelectedIndex == 2)
            {

                lblCustomerName.Text = "Line Number";
                FillLines();
                txtSearchText.Visible = false;
                cboLines.Visible = true;
                
            }
            else 
            {
                lblCustomerName.Text = "Customer Name";
                cboLines.Visible = false;
                txtSearchText.Visible = true;
            }
        }

        private void ctlCustomers_Load(object sender, EventArgs e)
        {
            cboSearchBy.SelectedIndex = 0;
        }

        #region Search Customer
        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchCustomers();
        }       

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchCustomers();
            }
        }

        public bool SearchCustomers()
        {
            BindingList<CustomerInfo> customers = new BindingList<CustomerInfo>();
            try
            {
                customers = _customerManager.GetCustomersByName(cboSearchBy.SelectedIndex, txtSearchText.Text,Convert.ToInt32(cboLines.SelectedValue));
                if (customers.Count > 0)
                {
                    grdResult.DataSource = customers;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = customers.Count.ToString() + " record(s) found.";
                customers = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Customer by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["Address"].Visible = false;
            grdResult.Columns["VatTinNumber"].Visible = false;
            grdResult.Columns["Quantity"].Visible = false;
            grdResult.Columns["ItemID"].Visible = false;
            grdResult.Columns["CustomerType"].Visible = false;
            grdResult.Columns["ItemCode"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
            grdResult.Columns["EMail"].Visible = false;
            grdResult.Columns["IsActive"].Visible = false;
            grdResult.Columns["MemberSince"].Visible = false;
            grdResult.Columns["City"].Visible = false;
            grdResult.Columns["ContactPerson"].Visible = false;
            grdResult.Columns["NewCustomerNumber"].Visible = false;
            grdResult.Columns["Mobile"].Visible = false;
            grdResult.Columns["LineID"].Visible = false;
            grdResult.Columns["Barcode"].Visible = false;
            grdResult.Columns["ItemName"].Visible = false;
            grdResult.Columns["CustomerNameMarathi"].Visible = false;
            grdResult.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["Deposit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["CustomerNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdResult.Columns["LineNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
        }
        bool FillLines()
        {
            LineManager _lineManager = new LineManager();
            BindingList<LineInfo> lines = new BindingList<LineInfo>();
            try
            {
                lines = _lineManager.GetLines();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Line ID." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboLines.DataSource = lines;
            cboLines.DisplayMember = "LineNumber";
            cboLines.ValueMember = "ID";
            lines = null;
            cboLines.SelectedIndex = -1;
            return true;
        }
        #endregion

        #region Add Customer
        private void btnNew_Click(object sender, EventArgs e)
        {
            AddCustomer();
        }

        void AddCustomer()
        {
            frmCustomer frm = new frmCustomer();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchCustomers();

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
        #endregion

        #region Modify Customer
        private void btnModify_Click(object sender, EventArgs e)
        {
            EditCustomer();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditCustomer();
        }

        bool EditCustomer()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                CustomerInfo customer = (CustomerInfo)grdResult.SelectedRows[0].DataBoundItem;
                BindingList<CustomerItemsInfo> customerItems = _customerManager.GetCustomerItemsForSelectedCustomer(customer.ID);
                frmCustomer frm = new frmCustomer(customer, customerItems);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchCustomers();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (customer.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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
        #endregion

        #region Delete Customer
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteCustomer();
        }

        bool DeleteCustomer()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                string tableName = string.Empty;
                bool isUsed = true;
                try
                {
                    isUsed = _customerManager.CheckCustomerUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Customer is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _customerManager.DeleteCustomer(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchCustomers();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Customer is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Customer." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }
        #endregion

        
    }
}