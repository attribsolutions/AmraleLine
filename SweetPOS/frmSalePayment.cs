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
    public partial class frmSalePayment : Form
    {
        #region Class level variables...

        SaleProcessingInfo _saleProcessingInfo = new SaleProcessingInfo();
        SalePaymentManager _salePaymentManager = new SalePaymentManager();
        

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        bool _roundAmount = false;
        decimal _round50 = 0;
        decimal _round1 = 0;
        bool _billFromOrder = false;

        #endregion

        public frmSalePayment()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Sale Payment";
        }

        public frmSalePayment(SaleProcessingInfo saleProcessingInfo)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Sale Payment";
            _saleProcessingInfo = saleProcessingInfo;
            ShowSalePaymentData();            
        }

        private void ShowSalePaymentData()
        {
            txtCustomerName.Text = _saleProcessingInfo.CustomerName;
            txtCutomerID.Text = Convert.ToString(_saleProcessingInfo.CustomerNumber);
            txtOpeningBalance.Text = _saleProcessingInfo.ClosingBalance.ToString(); 
        }

        private void ShowOrderInSaleFormat()
        {           
            dtPaymentDate.Enabled = false;
            CalculateTotals();
            dtPaymentDate.Focus();
        }
        
        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }        
         
        SalePaymentInfo SetValue()
        {
            SalePaymentInfo retVal = new SalePaymentInfo();
            
            retVal.PaymentDate = dtPaymentDate.Value;
            retVal.ProcessingID = _saleProcessingInfo.ID;
            retVal.CustomerID = _saleProcessingInfo.CustomerID;
            retVal.PaymentMode = Convert.ToInt32(cboPaymentMode.SelectedIndex); //INDEX
            retVal.OpeningBalance = Convert.ToString(txtOpeningBalance.Text)!="" ? Convert.ToDecimal(txtOpeningBalance.Text) : 0;
            retVal.PaidAmount = Convert.ToString(txtAmount.Text)!="" ? Convert.ToDecimal(txtAmount.Text) : 0;
            retVal.ClosingBalance = Convert.ToString(txtClosingAmount.Text)!="" ? Convert.ToDecimal(txtClosingAmount.Text) : 0;
            retVal.AdjustmentAmount = Convert.ToString(txtAdjustment.Text)!="" ? Convert.ToDecimal(txtAdjustment.Text) : 0;
            retVal.ReceiptNo = Convert.ToInt32(txtReceiptNo.Text);
            retVal.Comment = txtComment.Text;
            retVal.ChequeNo = txtChequeno.Text;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;

            return retVal;
        }

        bool Save(Boolean isPrint)
        {
            if (!ValidateFields())
                return false;
            
            SalePaymentInfo salePayment = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {                
                try
                {
                    _newRecordID = _salePaymentManager.AddSalePayment(salePayment);
                    _newRecordAdded = true;
                    if(isPrint)
                    {
                        ReportClass report = new ReportClass();
                        report.ShowPaymentReceipt(_newRecordID);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Sale Payment." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private bool ValidateFields()
        {
            if (txtReceiptNo.Text == string.Empty)
            {
                txtReceiptNo.Focus();
                MessageBox.Show("Please Enter Receipt No.", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return false;
            }
            else 
            {
                if (!Convert.ToBoolean(_salePaymentManager.CheckReceiptNoExists(Convert.ToInt32(txtReceiptNo.Text))))
                {
                    txtReceiptNo.Focus();
                     DialogResult dr = MessageBox.Show("Receipt No. Already Exists...! Do you want to Continue?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                     if(dr== DialogResult.No)
                         return false;
                }
            }
            

            if (txtAmount.Text.Trim() == string.Empty || Convert.ToDecimal(txtAmount.Text) <= 0)
            {
                txtAmount.Focus();
                MessageBox.Show("Please enter Amount.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            cboPaymentMode.SelectedIndex = 0;
            btnSaveNew.Text = "Save && Pr&int";
            btnSaveClose.Text = "Save && Cl&ose";            
            dtPaymentDate.Value = DateTime.Today;
            dtPaymentDate.Focus();
            cboPaymentMode.SelectedValue = 0;
        }

        void CloseForm()
        {
            if (_newRecordID > 0)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (Save(true))
                CloseForm();
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (Save(false))
                CloseForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }
        
        void CalculateTotals()
        {
            try
            {
                decimal differenceAmount = ((Convert.ToString(txtOpeningBalance.Text)!="" ? Convert.ToDecimal(txtOpeningBalance.Text) : 0) - (Convert.ToString(txtAmount.Text)!="" ? Convert.ToDecimal(txtAmount.Text) : 0) - (Convert.ToString(txtAdjustment.Text)!="" ? Convert.ToDecimal(txtAdjustment.Text) : 0));
                if (differenceAmount > 0 && differenceAmount <= 5)
                { 
                    txtAdjustment.Text = Convert.ToString(differenceAmount);
                    txtClosingAmount.Text = "0.0";
                }
                else
                { 
                    txtClosingAmount.Text = Convert.ToString(differenceAmount); 
                    txtAdjustment.Text = "0.0";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void txtAdjustment_TextChanged(object sender, EventArgs e)
        {
            txtClosingAmount.Text = ((Convert.ToString(txtOpeningBalance.Text)!="" ? Convert.ToDecimal(txtOpeningBalance.Text) : 0 )- (Convert.ToString(txtAmount.Text)!="" ? Convert.ToDecimal(txtAmount.Text) : 0) - (Convert.ToString(txtAdjustment.Text) != "" ? Convert.ToDecimal(txtAdjustment.Text) : 0)).ToString();
        }

        private void frmSalePayment_Load(object sender, EventArgs e)
        {
            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                New();
            }            
        }

        private void frmSalePayment_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cboPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPaymentMode.SelectedIndex == 1)
            {
                lblChequeNo.Visible = true;
                txtChequeno.Visible = true;
            }
            else
            {
                lblChequeNo.Visible = false;
                txtChequeno.Visible = false;
            }
        }   
    }
}