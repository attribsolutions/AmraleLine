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
    public partial class ctlSales : UserControl
    {
        SaleManager _saleManager = new SaleManager();
        SaleItemManager _saleItemManager = new SaleItemManager();
        bool _allowEdit = true;
        bool _fillingGrid = true;

        public ctlSales()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddSale();
        }

        public bool SearchSales()
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

            BindingList<SaleInfo> sales = new BindingList<SaleInfo>();
            try
            {
                if (cboSearchIn.SelectedIndex == 2 ||  cboSearchIn.SelectedIndex == 3)
                    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, txtSearchText.Text, null, null,Program.DivisionID,0);
                if (cboSearchIn.SelectedIndex == 1)
                    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, string.Empty, (object)dtStartDate.Value.Date, (object)dtEndDate.Value.Date, Program.DivisionID, cboSaleType.SelectedIndex);
                if (cboSearchIn.SelectedIndex == 0)
                    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, txtSearchText.Text, (object)dtStartDate.Value.Date, (object)dtEndDate.Value.Date, Program.DivisionID, 0);
                //if (cboSearchIn.SelectedIndex == 7 || cboSearchIn.SelectedIndex == 8)
                //    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, string.Empty, (object)dtStartDate.Value.Date, (object)dtEndDate.Value.Date, Program.DivisionID,0);

                //if (cboSearchIn.SelectedIndex == 4 || cboSearchIn.SelectedIndex == 9)
                //    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, string.Empty, null, null, Program.DivisionID,0);
                //if (cboSearchIn.SelectedIndex == 5)
                //    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, "1", null, null, Program.DivisionID,0);
                //if (cboSearchIn.SelectedIndex == 6)
                //    sales = _saleManager.GetSalesByFilter(cboSearchIn.SelectedIndex, "0", null, null, Program.DivisionID,0);

                if (sales.Count > 0)
                {
                    grdResult.DataSource = sales;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = sales.Count.ToString() + " record(s) found.";
                sales = null;
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
            if (cboSearchIn.SelectedIndex != 9)
                grdResult.Columns["Description"].Visible = false;
            grdResult.Columns["IsPrint"].Visible = false;
            grdResult.Columns["Select"].Visible = false;
            grdResult.Columns["CashCredit"].Visible = false;
            grdResult.Columns["DiscountPercentage"].Visible = false;
            grdResult.Columns["DiscountAmount"].Visible = false;
            grdResult.Columns["NetAmount"].Visible = false;
            grdResult.Columns["BalanceAmount"].Visible = false;
            grdResult.Columns["RoundedAmount"].Visible = false;
            grdResult.Columns["RFIDTransaction"].Visible = false;
            grdResult.Columns["TotalWeight"].Visible = false;
            grdResult.Columns["ActualWeight"].Visible = false;
            grdResult.Columns["BillFromOrder"].Visible = false;
            grdResult.Columns["OrderID"].Visible = false;
            grdResult.Columns["CardPaymentDetails"].Visible = false;
            grdResult.Columns["IsProcessed"].Visible = false;
            grdResult.Columns["CustomerNumber"].Visible = false;
            grdResult.Columns["DivisionID"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
             grdResult.Columns["LineNumber"].Visible = false;
            grdResult.Columns["IsCouponSale"].Visible = false;
            grdResult.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["DiscountPercentage"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["DiscountAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["NetAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["RoundedAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["BalanceAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            grdResult.Columns["BillDate"].Width = 200;

            //foreach (DataGridViewColumn col in grdResult.Columns)
            //{
            //    col.ReadOnly = true;
            //    if (col.Name == "Select")
            //        col.ReadOnly = false;
            //}
        }

        void CalculateTotals()
        {
            txtTotalCash.Text = txtTotalCredit.Text = txtTotalBalance.Text = "0.00";

            foreach (DataGridViewRow dr in grdResult.Rows)
            {
                if (dr.Cells["CashCredit"].Value.ToString() == "0")
                    txtTotalCash.Text = (Convert.ToDecimal(txtTotalCash.Text) + Convert.ToDecimal(dr.Cells["NetAmount"].Value)).ToString("0.00");
                else
                    txtTotalCredit.Text = (Convert.ToDecimal(txtTotalCredit.Text) + Convert.ToDecimal(dr.Cells["NetAmount"].Value)).ToString("0.00");

                txtTotalBalance.Text = (Convert.ToDecimal(txtTotalBalance.Text) + Convert.ToDecimal(dr.Cells["BalanceAmount"].Value)).ToString("0.00");
            }
        }

        void AddSale()
        {
            frmSaleManual frm = new frmSaleManual();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchSales();

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

        bool EditSale()
        {
            if (grdResult.SelectedRows.Count > 0 && _allowEdit)
            {
                if ((Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID) && !CheckMasterCard())
                {
                    MessageBox.Show("Master Card Not Found.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string tableName = string.Empty;
                try
                {
                    bool isUsed = _saleManager.CheckSaleUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                    if (isUsed)
                        return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Sale is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                SaleInfo sale = (SaleInfo)grdResult.SelectedRows[0].DataBoundItem;

                BindingList<SaleItemInfo> saleItems = GetSaleItemsBySaleId(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));

                frmSaleManual frm = new frmSaleManual(sale, saleItems);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchSales();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (sale.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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

        bool DeleteSale()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                if ((Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID) && !CheckMasterCard())
                {
                    MessageBox.Show("Master Card Not Found.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string tableName = string.Empty;
                bool isUsed = true;
                try
                {
                    isUsed = _saleManager.CheckSaleUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Sale is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _saleManager.DeleteSale(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value), false);
                            SearchSales();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Sale is in use.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        BindingList<SaleItemInfo> GetSaleItemsBySaleId(int saleId)
        {
            BindingList<SaleItemInfo> retVal = null;
            try
            {
                retVal = _saleItemManager.GetSaleItemsBySaleId(saleId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Sale Items by Sale Id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return retVal;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchSales();
            if (grdResult.Rows.Count > 0)
                grdResult_SelectionChanged(sender, e);
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditSale();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditSale();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSale();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchSales();
            }
        }

        private void ctlSales_Load(object sender, EventArgs e)
        {
            cboSearchIn.SelectedIndex = 1;
            cboSaleType.SelectedIndex = 0;
            if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID)
            {
                lblNeedMasterCard.Visible = true;
            }

            if (Program.UserRole == "Cashier")      //Hardcode
            {
                btnModify.Enabled = false;
                _allowEdit = false;
            }
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchIn.SelectedIndex == 0 ||  cboSearchIn.SelectedIndex == 3)     //Hardcode
            {
                txtSearchText.Visible = true;
                dtStartDate.Visible = true;
                lblTo.Visible = true;
                dtEndDate.Visible = true;
                cboSaleType.Visible = false;
                txtSearchText.Text = "";
            }        
         if (cboSearchIn.SelectedIndex == 2)
         {
             txtSearchText.Visible = true;
             dtStartDate.Visible = false;
             lblTo.Visible = false;
             dtEndDate.Visible = false;
             cboSaleType.Visible = false;
             txtSearchText.Text = "";
         }                                    
                             
            //if (cboSearchIn.SelectedIndex == 7 || cboSearchIn.SelectedIndex == 8)     //Hardcode
            //{
            //    txtSearchText.Visible = false;
            //    dtStartDate.Visible = true;
            //    lblTo.Visible = true;
            //    dtEndDate.Visible = true;
            //    cboSaleType.Visible = false;
            //}
            if (cboSearchIn.SelectedIndex == 1)     //Hardcode
            {
                txtSearchText.Visible = false;
                dtStartDate.Visible = true;
                lblTo.Visible = true;
                dtEndDate.Visible = true;
                //cboSaleType.Visible = true;
            }
            //if (cboSearchIn.SelectedIndex == 4 || cboSearchIn.SelectedIndex == 5 || cboSearchIn.SelectedIndex == 6 || cboSearchIn.SelectedIndex == 9)     //Hardcode
            //{
            //    txtSearchText.Visible = false;
            //    dtStartDate.Visible = false;
            //    lblTo.Visible = false;
            //    dtEndDate.Visible = false;
            //    cboSaleType.Visible = false;
            //}
        }

        private void grdResult_SelectionChanged(object sender, EventArgs e)
        {
            if (!_fillingGrid)
            {
                if (Convert.ToDecimal(grdResult.SelectedRows[0].Cells["BalanceAmount"].Value) > 0)
                    btnMakeReceipt.Visible = true;
                else
                    btnMakeReceipt.Visible = false;
            }
        }

        private void btnMakePayment_Click(object sender, EventArgs e)
        {
            if (grdResult.SelectedRows.Count > 0 && Convert.ToDecimal(grdResult.SelectedRows[0].Cells["BalanceAmount"].Value) > 0)
            {
                SaleInfo sale = null;
                try
                {
                    sale = _saleManager.GetSale(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting Sale by Sale Id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int partyID = sale.CustomerID;
                Int64 billID = sale.ID;
                decimal balanceAmount = sale.BalanceAmount;
                string customerName = grdResult.SelectedRows[0].Cells["CustomerName"].Value.ToString();
                string billNo = sale.BillNo.ToString();

                frmReceiptPayment frm = new frmReceiptPayment(partyID, billID, balanceAmount, customerName, billNo, false);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchSales();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (sale.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[2, i];
                            break;
                        }
                    }
                }
            }
            else
                btnMakeReceipt.Visible = false;
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                if ((Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID) && !CheckMasterCard())
                {
                    MessageBox.Show("Master Card Not Found.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ReportClass rpt = new ReportClass();
                if (Program.SHOWSHORTBILL)
                {
                    rpt.ShowBillChitale(Convert.ToInt32(grdResult.SelectedRows[0].Cells["BillNo"].Value), true, Convert.ToDecimal(grdResult.SelectedRows[0].Cells["DiscountAmount"].Value) > 0);
                }
                else
                {
                    rpt.ShowBill(Convert.ToInt32(grdResult.SelectedRows[0].Cells["BillNo"].Value), true, Convert.ToDecimal(grdResult.SelectedRows[0].Cells["DiscountAmount"].Value) > 0);
                }
            }
        }

        private void grdResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            grdResult_SelectionChanged(sender, e);
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

        private void lblNeedMasterCard_Click(object sender, EventArgs e)
        {
            DialogResult dlg = MessageBox.Show("Continue?", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Question);
            if (dlg == DialogResult.No)
            {
                return;
            }

            foreach (DataGridViewRow dr in grdResult.Rows)
            {
                if (!Convert.ToBoolean(dr.Cells["Select"].Value))
                    continue;

                try
                {
                    _saleManager.DeleteSale(Convert.ToInt32(dr.Cells["ID"].Value), false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in deleting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            MessageBox.Show("Deleted Successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            SearchSales();
        }
    }
}