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
    public partial class ctlReceiptPayments : UserControl
    {
        ReceiptPaymentManager _receiptPaymentManager = new ReceiptPaymentManager();
        ctlSales _ctlSale = null;

        public ctlReceiptPayments(ctlSales ctlSale)
        {
            InitializeComponent();
            _ctlSale = ctlSale;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddReceiptPayment();
        }

        public bool SearchReceiptPayments()
        {
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

            BindingList<ReceiptPaymentInfo> receiptPayments = new BindingList<ReceiptPaymentInfo>();
            try
            {
                if (cboSearchIn.SelectedIndex == 0 || cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3)
                    receiptPayments = _receiptPaymentManager.GetReceiptPaymentsByFilter(cboSearchIn.SelectedIndex, txtSearchText.Text, null, null);
                if (cboSearchIn.SelectedIndex == 1)
                    receiptPayments = _receiptPaymentManager.GetReceiptPaymentsByFilter(cboSearchIn.SelectedIndex, string.Empty, (object)dtStartDate.Value.Date, (object)dtEndDate.Value.Date);

                if (receiptPayments.Count > 0)
                {
                    grdResult.DataSource = receiptPayments;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = receiptPayments.Count.ToString() + " record(s) found.";
                receiptPayments = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting ReceiptPayment by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["PartyTransaction"].Visible = false;
            grdResult.Columns["PartyID"].Visible = false;
            grdResult.Columns["BillID"].Visible = false;
            grdResult.Columns["ReceivedPaidBy"].Visible = false;
            grdResult.Columns["Description"].Visible = false;

            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;

            grdResult.Columns["Particulars"].FillWeight = 200;

            grdResult.Columns["BalanceAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["DueAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        void AddReceiptPayment()
        {
            frmReceiptPayment frm = new frmReceiptPayment();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchReceiptPayments();
                if (_ctlSale != null)
                    _ctlSale.SearchSales();

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

        bool EditReceiptPayment()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                ReceiptPaymentInfo receiptPayment = (ReceiptPaymentInfo)grdResult.SelectedRows[0].DataBoundItem;
                
                frmReceiptPayment frm;
                if (receiptPayment.PartyTransaction)
                    frm = new frmReceiptPayment(receiptPayment, receiptPayment.PartyID, receiptPayment.BillID);
                else
                    frm = new frmReceiptPayment(receiptPayment);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchReceiptPayments();
                    if (_ctlSale != null)
                        _ctlSale.SearchSales();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (receiptPayment.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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

        bool DeleteReceiptPayment()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                bool isUsed = false;
                try
                {
                    //isUsed = _receiptPaymentManager.CheckReceiptPaymentUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether ReceiptPayment is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _receiptPaymentManager.DeleteReceiptPayment((ReceiptPaymentInfo)grdResult.SelectedRows[0].DataBoundItem);
                            SearchReceiptPayments();
                            if (_ctlSale != null)
                                _ctlSale.SearchSales();

                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, ReceiptPayment is in use.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting ReceiptPayment." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchReceiptPayments();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditReceiptPayment();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditReceiptPayment();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteReceiptPayment();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchReceiptPayments();
            }
        }

        private void ctlReceiptPayments_Load(object sender, EventArgs e)
        {
            cboSearchIn.SelectedIndex = 0;
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchIn.SelectedIndex == 0)     //Hardcode
            {
                txtSearchText.Visible = true;
                dtStartDate.Visible = false;
                lblTo.Visible = false;
                dtEndDate.Visible = false;
            }
            if (cboSearchIn.SelectedIndex == 1)     //Hardcode
            {
                txtSearchText.Visible = false;
                dtStartDate.Visible = true;
                lblTo.Visible = true;
                dtEndDate.Visible = true;
            }
        }
    }
}