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
    public partial class frmReceiptPayment : Form
    {
        #region Class level variables...

        ReceiptPaymentManager _receiptPaymentManager = new ReceiptPaymentManager();
        ReceiptPaymentInfo _receiptPayment = new ReceiptPaymentInfo();
        UserManager _userManager = new UserManager();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        bool _partyTransaction = false;
        int _partyId = 0;
        Int64 _partyBillNo = 0;
        decimal _oldAmountPaid = 0;

        #endregion

        public frmReceiptPayment()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Receipt/Payment Entry";
        }

        public frmReceiptPayment(int partyID, Int64 billID, decimal dueAmount, string partyName, string invoiceNo, bool payment)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Receipt/Payment Entry";
            _partyTransaction = true;
            _partyId = partyID;
            _partyBillNo = billID;

            if (payment)
            {
                txtDueAmount.Text = dueAmount.ToString("0.00");
                cboReceiptPayment.Text = "Payment";
                cboReceiptPayment.Enabled = false;
                pnlDueBalance.Visible = true;
                txtParticulars.Text = "Payment made to " + partyName + " against invoice no " + invoiceNo;
            }
            else
            {
                txtDueAmount.Text = dueAmount.ToString("0.00");
                cboReceiptPayment.Text = "Receipt";
                cboReceiptPayment.Enabled = false;
                pnlDueBalance.Visible = true;
                txtParticulars.Text = "Payment received from " + partyName + " against sale bill no " + invoiceNo;
            }
        }

        public frmReceiptPayment(ReceiptPaymentInfo receiptPayment)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Receipt/Payment Entry";
            _receiptPayment = receiptPayment;
            _partyTransaction = receiptPayment.PartyTransaction;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public frmReceiptPayment(ReceiptPaymentInfo receiptPayment, int partyID, Int64 partyBillId)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Receipt/Payment Entry";
            _receiptPayment = receiptPayment;
            _partyTransaction = receiptPayment.PartyTransaction;
            _partyId = partyID;
            _partyBillNo = partyBillId;
            _oldAmountPaid = receiptPayment.Amount;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        void CalculateTotal()
        {
            txtBalanceAmount.Text = (Convert.ToDecimal(txtDueAmount.Text) - Convert.ToDecimal(txtAmount.Text)).ToString("0.00");
        }

        private void frmReceiptPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                CloseForm();
            }
        }

        private void frmReceiptPayment_Load(object sender, EventArgs e)
        {
            FillUsers();

            cboReceivedBy.SelectedIndex = -1;

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                dtTransactionDate.Tag = _receiptPayment.ID;
                dtTransactionDate.Value = _receiptPayment.TransactionDate;
                cboReceiptPayment.Text = _receiptPayment.ReceiptPayment;
                txtParticulars.Text = _receiptPayment.Particulars;
                cboPayMode.Text = _receiptPayment.PayMode;
                txtChequeNo.Text = _receiptPayment.ChequeNo;
                txtBankName.Text = _receiptPayment.BankName;
                txtAmount.Text = _receiptPayment.Amount.ToString("0.00");
                txtDescription.Text = _receiptPayment.Description;
                cboReceivedBy.SelectedValue = _receiptPayment.ReceivedPaidBy;

                if (_receiptPayment.PartyTransaction)
                {
                    pnlDueBalance.Visible = true;
                    txtDueAmount.Text = _receiptPayment.DueAmount.ToString("0.00");
                    txtBalanceAmount.Text = _receiptPayment.BalanceAmount.ToString("0.00");
                }

                dtTransactionDate.Focus();
            }
        }

        bool FillUsers()
        {
            BindingList<UserInfo> users = new BindingList<UserInfo>();
            try
            {
                users = _userManager.GetUsersAll(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Users." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboReceivedBy.DataSource = users;
            cboReceivedBy.DisplayMember = "Name";
            cboReceivedBy.ValueMember = "ID";

            users = null;
            return true;
        }

        bool ValidateFields()
        {
            if (dtTransactionDate.Value.Date > DateTime.Now.Date)
            {
                dtTransactionDate.Focus();
                MessageBox.Show("Please enter valid Transaction Date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboReceiptPayment.SelectedIndex == -1)
            {
                cboReceiptPayment.Focus();
                MessageBox.Show("Please select transaction type.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtParticulars.Text.Trim() == string.Empty)
            {
                txtParticulars.Focus();
                MessageBox.Show("Please enter particulars for this transaction.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboPayMode.SelectedIndex == -1)
            {
                cboPayMode.Focus();
                MessageBox.Show("Please select Pay Mode.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboPayMode.SelectedIndex != 0)
            {
                if (txtChequeNo.Text.Trim() == string.Empty)
                {
                    txtChequeNo.Focus();
                    MessageBox.Show("Please enter cheque number.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (txtBankName.Text.Trim() == string.Empty)
                {
                    txtBankName.Focus();
                    MessageBox.Show("Please enter bank name.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            decimal d;
            if (!decimal.TryParse(txtAmount.Text, out d))
            {
                txtAmount.Focus();
                MessageBox.Show("Please enter valid amount.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
                if (Convert.ToDecimal(txtAmount.Text) == 0)
                {
                    txtAmount.Focus();
                    MessageBox.Show("Amount must be greater than zero.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

            if (cboReceivedBy.SelectedIndex == -1)
            {
                cboReceivedBy.Focus();
                MessageBox.Show("Please select received by.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        ReceiptPaymentInfo SetValue()
        {
            ReceiptPaymentInfo retVal = new ReceiptPaymentInfo();

            retVal.TransactionDate = dtTransactionDate.Value.Date;
            retVal.ReceiptPayment = cboReceiptPayment.Text;
            retVal.Particulars = txtParticulars.Text;
            retVal.PayMode = cboPayMode.Text;
            retVal.BankName = txtBankName.Text;
            retVal.ChequeNo = txtChequeNo.Text;
            retVal.Amount = Convert.ToDecimal(txtAmount.Text);
            retVal.ReceivedPaidBy = Convert.ToInt32(cboReceivedBy.SelectedValue);

            retVal.PartyTransaction = _partyTransaction;
            if (_partyTransaction)
            {
                retVal.PartyID = _partyId;
                retVal.BillID = _partyBillNo;
                retVal.DueAmount = Convert.ToDecimal(txtDueAmount.Text);
                retVal.BalanceAmount = Convert.ToDecimal(txtBalanceAmount.Text);
            }

            retVal.Description = txtDescription.Text;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            return retVal;
        }

        bool Save()
        {
            if (!ValidateFields())
                return false;

            ReceiptPaymentInfo receiptPayment = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _receiptPaymentManager.AddReceiptPayment(receiptPayment);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Cash Transaction." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    receiptPayment.ID = Convert.ToInt64(dtTransactionDate.Tag);
                    _receiptPaymentManager.UpdateReceiptPayment(receiptPayment, _oldAmountPaid);

                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Cash Transaction." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";

            cboReceiptPayment.SelectedIndex = -1;
            txtParticulars.Text = string.Empty;
            cboPayMode.SelectedIndex = -1;
            txtBankName.Text = string.Empty;
            txtChequeNo.Text = string.Empty;
            txtAmount.Text = "0.00";
            txtDueAmount.Text = "0.00";
            txtBalanceAmount.Text = "0.00";
            cboReceivedBy.SelectedIndex = -1;
            txtDescription.Text = string.Empty;
            
            _partyTransaction = false;
            cboReceiptPayment.Enabled = true;
            pnlDueBalance.Visible = false;

            dtTransactionDate.Focus();
        }

        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (Save())
                New();
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (Save())
                CloseForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        private void cboPayMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPayMode.SelectedIndex == 0)
            {
                txtChequeNo.Enabled = false;
                txtBankName.Enabled = false;
            }
            else
            {
                txtChequeNo.Enabled = true;
                txtBankName.Enabled = true;
            }
        }

        private void txtAmount_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtAmount.Text, out d))
                e.Cancel = true;
            else
                CalculateTotal();
        }
    }
}