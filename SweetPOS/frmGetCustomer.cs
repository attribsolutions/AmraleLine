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
    public partial class frmGetCustomer : Form
    {
        #region Declaration
        CustomerManager _CustomerManager = new CustomerManager();
        frmSale _frm;
        #endregion

        #region Load

        public frmGetCustomer(frmSale frm)
        {
            InitializeComponent();
            _frm = frm; 
        }

        private void frmSaleModify_Load(object sender, EventArgs e)
        {
            //AutoCompleteForName();
            txtName.Focus();
        }

        //private void AutoCompleteForName()
        //{
        //    BindingList<CustomerInfo> Customer = null;
        //    try
        //    {
        //        Customer = _CustomerManager.GetCustomersAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    AutoCompleteStringCollection autoSource = new AutoCompleteStringCollection();
        //    foreach (CustomerInfo iCustomer in Customer)
        //    {
        //        autoSource.Add(iCustomer.Barcode + " " + iCustomer.Name);
        //    }
        //    txtName.AutoCompleteCustomSource = autoSource;
        //}

        #endregion

        #region Close Form
        void CloseForm()
        {
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Search
        private void btnGo_Click(object sender, EventArgs e)
        {
            if (grdCustomer.SelectedRows.Count > 0)
            {
                CustomerInfo customer = (CustomerInfo)grdCustomer.SelectedRows[0].DataBoundItem;
                string str = "AMAR" + string.Format("{0:D3}", customer.CustomerNumber);
                _frm.CustomerNumber = str;
                _frm.CodeClickEvent(sender, e);
            }
            CloseForm();
        }
        #endregion

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnGo_Click(sender, e);
                txtName.Focus();
            }
            if (e.KeyCode == Keys.Down)
            {
                grdCustomer.Focus();
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            BindingList<CustomerInfo> Customer = null;
            try
            {
                Customer = _CustomerManager.GetCustomersByName(0, txtName.Text,0);
                grdCustomer.DataSource = Customer;
                DisableFields();
                //grdCustomer.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void DisableFields()
        {
            grdCustomer.Columns["ID"].Visible = false;
            grdCustomer.Columns["ContactPerson"].Visible = false;
            grdCustomer.Columns["Address"].Visible = false;
            grdCustomer.Columns["City"].Visible = false;
            grdCustomer.Columns["Mobile"].Visible = false;
            grdCustomer.Columns["EMail"].Visible = false;
            grdCustomer.Columns["Balance"].Visible = false;
            grdCustomer.Columns["IsActive"].Visible = false;
            grdCustomer.Columns["CustomerType"].Visible = false;
            grdCustomer.Columns["Barcode"].Visible = false;
            grdCustomer.Columns["MemberSince"].Visible = false;
            grdCustomer.Columns["VatTinNumber"].Visible = false;
            grdCustomer.Columns["ItemID"].Visible = false;
            grdCustomer.Columns["ItemCode"].Visible = false;
            grdCustomer.Columns["Quantity"].Visible = false;
            grdCustomer.Columns["CreatedBy"].Visible = false;
            grdCustomer.Columns["CreatedOn"].Visible = false;
            grdCustomer.Columns["UpdatedBy"].Visible = false;
            grdCustomer.Columns["UpdatedOn"].Visible = false;
            //grdResult.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void KeyboardClick(object sender, EventArgs e)
        {
            txtName.Text += ((Button)sender).Text;
            txtName.Select(txtName.Text.Length, 0);
            txtName.Focus();
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length > 0)
            {
                txtName.Text = txtName.Text.Substring(0, txtName.Text.Length - 1);
                txtName.Select(txtName.Text.Length, 0);
                txtName.Focus();
            }
            else
                txtName.Focus();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            btnGo_Click(sender, e);
        }

        private void grdCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == (Char)Keys.Enter)
            //{
            //    btnGo_Click(sender, e);
            //}
        }

    }
}
