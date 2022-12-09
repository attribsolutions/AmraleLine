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
using ChitalePersonalzer;

namespace SweetPOS
{
    public partial class ctlSalesPayment : UserControl
    {
        #region Form variables
        SalePaymentManager _salePaymentManager = new SalePaymentManager();
        SaleManager _saleManager = new SaleManager();
        SaleItemManager _saleItemManager = new SaleItemManager();
        bool _fillingGrid = true;
        #endregion 

        #region From intialize and Load
        public ctlSalesPayment()
        {
            InitializeComponent();
        }

        private void ctlSales_Load(object sender, EventArgs e)
        {
            cboSearchIn.SelectedIndex = 1;
            SearchSalesPayment();
            btnNew.Visible = false;
            btnModify.Visible = false;
            //btnDelete.Visible = false;
        }
        #endregion

        #region Close Form
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion

        #region Add Form
        private void btnNew_Click(object sender, EventArgs e)
        {
            //AddSale();
        }

        void AddSale()
        {
            frmSalePayment frm = new frmSalePayment();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchSalesPayment();

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

        #region Delete Sale Payment

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeletePayment();
        }

        bool DeletePayment()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                try
                {
                    DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        _salePaymentManager.DeletePayment(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value), Convert.ToInt32(grdResult.SelectedRows[0].Cells["ProcessingID"].Value));
                        SearchSalesPayment();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Sale Payment." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region Modify Sale Payment

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditSale();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditSale();
        }

        bool EditSale()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                //if ((Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID) && !CheckMasterCard())
                //{
                //    MessageBox.Show("Master Card Not Found.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}

                //string tableName = string.Empty;
                //try
                //{
                //    bool isUsed = _saleManager.CheckSaleUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                //    if (isUsed)
                //        return false;
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Error in checking whether Sale is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return false;
                //}

                //SaleInfo sale = (SaleInfo)grdResult.SelectedRows[0].DataBoundItem;

                //BindingList<SaleItemInfo> saleItems = GetSaleItemsBySaleId(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));

                //frmSaleManual frm = new frmSaleManual(sale, saleItems);
                //if (frm.ShowDialog() == DialogResult.OK)
                //{
                //    SearchSales();

                //    //Set focus in grid at position, where it was previously before edit.
                //    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                //    {
                //        if (sale.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                //        {
                //            grdResult.Rows[i].Selected = true;
                //            grdResult.CurrentCell = grdResult[2, i];
                //            break;
                //        }
                //    }
                //}
            }
            return true;
        }
        #endregion

        #region Search Sale Payment

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchSalesPayment();
        }

        public bool SearchSalesPayment()
        {
            _fillingGrid = true;

            if (cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3)
            {
                decimal d;
                if (!decimal.TryParse(txtSearchText.Text, out d))
                {
                    txtSearchText.Focus();
                    MessageBox.Show("Please enter valid amount.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            BindingList<SalePaymentInfo> salesPayment = new BindingList<SalePaymentInfo>();            
            try
            {

                if (cboSearchIn.SelectedIndex == 0 || cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3 || cboSearchIn.SelectedIndex == 4)
                    salesPayment = _salePaymentManager.GetSalesPaymentByFilter(cboSearchIn.SelectedIndex, txtSearchText.Text, null, null);                    
                if (cboSearchIn.SelectedIndex == 1)
                    salesPayment = _salePaymentManager.GetSalesPaymentByFilter(cboSearchIn.SelectedIndex, string.Empty, (object)dtStartDate.Value.Date, (object)dtEndDate.Value.Date);
                        

                if (salesPayment.Count > 0)
                {
                    grdResult.DataSource = salesPayment;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                
                lblRecordsFound.Text = salesPayment.Count.ToString() + " record(s) found.";
                salesPayment = null;
                txtSearchText.Focus();

                _fillingGrid = false;

                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Sale by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _fillingGrid = false;
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["CustomerID"].Visible = false;
            grdResult.Columns["ProcessingID"].Visible = false;
            grdResult.Columns["PaymentMode"].Visible = false;

            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["ChequeNo"].Visible = false;
            grdResult.Columns["LineNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdResult.Columns["CustomerNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdResult.Columns["OpeningBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["PaidAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["AdjustmentAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["ClosingBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            

            foreach (DataGridViewColumn col in grdResult.Columns)
            {
                col.ReadOnly = true;
                if (col.Name == "Select")
                    col.ReadOnly = false;
            }
        }

        void CalculateTotals()
        {
            txtTotalCash.Text = txtTotalCredit.Text = txtTotalBalance.Text = "0.00";

            foreach (DataGridViewRow dr in grdResult.Rows)
            {
                if (dr.Cells["PaymentModeName"].Value.ToString() == "CASH")
                    txtTotalCash.Text = (Convert.ToDecimal(txtTotalCash.Text) + Convert.ToDecimal(dr.Cells["PaidAmount"].Value)).ToString("0.00");
                else
                    txtTotalCredit.Text = (Convert.ToDecimal(txtTotalCredit.Text) + Convert.ToDecimal(dr.Cells["PaidAmount"].Value)).ToString("0.00");

                txtTotalBalance.Text = (Convert.ToDecimal(txtTotalBalance.Text) + Convert.ToDecimal(dr.Cells["ClosingBalance"].Value)).ToString("0.00");
            }
        }       

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchSalesPayment();
            }
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchIn.SelectedIndex == 0 || cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3 || cboSearchIn.SelectedIndex == 4)     //Hardcode
            {
                txtSearchText.Visible = true;
                dtStartDate.Visible = false;
                lblTo.Visible = false;
                dtEndDate.Visible = false;
            }
            if (cboSearchIn.SelectedIndex == 1 )     //Hardcode
            {
                txtSearchText.Visible = false;
                dtStartDate.Visible = true;
                lblTo.Visible = true;
                dtEndDate.Visible = true;
            }            
        }

        bool CheckMasterCard()
        {
            Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
            string key = Program.KEYSET;

            try
            {
                string masterKey = c.ReadBlock(key, 2);
                if (masterKey.Length > 16 && masterKey.Substring(5, 11) == "MASTER CARD")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            c = null;
            return false;
        }

        #endregion

        #region PrintBill
        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                ReportClass report = new ReportClass();
                report.ShowPaymentReceipt(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
            }
        }
        #endregion
    }
}