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
    public partial class frmSaleProcessing : Form
    {
        #region Class level variables...

        SaleProcessingManager _salesProcessingManager = new SaleProcessingManager();
        CustomerManager _customerManager = new CustomerManager();
        EnumClass.SaleProcessingMode _viewMode = EnumClass.SaleProcessingMode.ProcessingMode;

        #endregion

        #region Load

        public frmSaleProcessing()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Sale Processing";
        }

        public frmSaleProcessing(int Mode)
        {
            InitializeComponent();

            if (Mode == 1)
            {
                _viewMode = EnumClass.SaleProcessingMode.ProcessingMode;
                this.Text = Program.MESSAGEBOXTITLE + " - Sale Processing";
                dtBillDate.Value = DateTime.Now.AddMonths(-1);
            }
            else if (Mode == 2)
            {
                _viewMode = EnumClass.SaleProcessingMode.PrintMode;
                this.Text = Program.MESSAGEBOXTITLE + " - Sale Printing";
                lblHeading.Text = "Sale Printing";
                dtBillDate.Value = DateTime.Now.AddMonths(-1);
            }
            else
            {
                _viewMode = EnumClass.SaleProcessingMode.PaymentMode;
                this.Text = Program.MESSAGEBOXTITLE + " - Sale Payment";
                lblHeading.Text = "Sale Payment";
                dtBillDate.Value = DateTime.Now.AddMonths(-1);
            }
        }

        private void frmSaleModify_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }

        private void frmSaleModify_Load(object sender, EventArgs e)
        {
            FillCustomers(0);
            cboCustomer.SelectedIndex = -1;
            cboSearchIn.SelectedIndex = 0;
            FillGridItems();
            if (_viewMode == EnumClass.SaleProcessingMode.ProcessingMode)
            {
                btnProcess.Text = "Process";               
            }
            else if (_viewMode == EnumClass.SaleProcessingMode.PrintMode)
            {
                btnProcess.Text = "Print";                
            }
            else
            {
                btnProcess.Text = "Pay";                
            }
        }

        private void FillGridItems()
        {
            grdSaleProcessing.DataSource = _salesProcessingManager.GetAllLSalesProcessingList(cboSearchIn.SelectedIndex, cboCustomer.Text ,dtBillDate.Value.Date,0,0);

            DisableFields();
        }

        private void DisableFields()
        {
            grdSaleProcessing.Columns["ID"].Visible = false;
            grdSaleProcessing.Columns["Month"].Visible = false;
            grdSaleProcessing.Columns["Year"].Visible = false;
            grdSaleProcessing.Columns["CustomerID"].Visible = false;
            grdSaleProcessing.Columns["LineNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdSaleProcessing.Columns["CustomerNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdSaleProcessing.Columns["OpeningBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdSaleProcessing.Columns["PaidAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdSaleProcessing.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdSaleProcessing.Columns["ClosingBalance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            txtOpeningTotal.Text = txtAmountTotal.Text = txtPaidTotal.Text = txtClosingTotal.Text = "0.00";

            foreach (DataGridViewRow row in grdSaleProcessing.Rows)
            {
                txtOpeningTotal.Text = (Convert.ToDecimal(txtOpeningTotal.Text) + Convert.ToDecimal(row.Cells["OpeningBalance"].Value)).ToString("0.00");
                txtAmountTotal.Text = (Convert.ToDecimal(txtAmountTotal.Text) + Convert.ToDecimal(row.Cells["Amount"].Value)).ToString("0.00");
                txtPaidTotal.Text = (Convert.ToDecimal(txtPaidTotal.Text) + Convert.ToDecimal(row.Cells["PaidAmount"].Value)).ToString("0.00");
                txtClosingTotal.Text = (Convert.ToDecimal(txtClosingTotal.Text) + Convert.ToDecimal(row.Cells["ClosingBalance"].Value)).ToString("0.00");
            }
        }

        bool FillCustomers(Int32 LineID)
        {
            BindingList<CustomerInfo> customers = new BindingList<CustomerInfo>();
            try
            {
                customers = _customerManager.GetCustomersByLineID(LineID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //cboCustomer.Items.Insert(0, "ALL");
            cboCustomer.DataSource = customers;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "ID";

            customers = null;


            return true;
        }
        #endregion

        #region Process
        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (btnProcess.Text == "Process")
            {
                Process(false);               
            }
            else if (btnProcess.Text == "Print")
            {
                Print();               
            }
            else
            {
                Payment();                              
            }
            FillGridItems();
        }

        private void Payment()
        {
            if (grdSaleProcessing.SelectedRows.Count > 0)
            {
                SaleProcessingInfo saleProcessingInfo = (SaleProcessingInfo)grdSaleProcessing.SelectedRows[0].DataBoundItem;
                frmSalePayment frm = new frmSalePayment(saleProcessingInfo);
                frm.ShowDialog();
            }  
        }

        private void Process(Boolean isPaidAmount)
        {
            //if (Convert.ToInt32(cboCustomer.SelectedValue) == 0)
            if (grdSaleProcessing.Rows.Count > 0 || grdSaleProcessing.Rows.Count == 0)
            {
                int errorType = 0;
                bool result = false;

                result = _salesProcessingManager.ProcessSalesData(dtBillDate.Value.Date, Convert.ToInt32(cboCustomer.SelectedValue), out errorType, GetCutomerIDOnGrid(), isPaidAmount);

                if (result == true)
                {
                    MessageBox.Show("Successfull Processed");
                }
                else
                {
                    if (errorType == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Paid Amout Present Do You Want To Proceed ?", "", MessageBoxButtons.OKCancel);
                        if (dialogResult == DialogResult.OK)
                            Process(true);
                    }
                    else
                        MessageBox.Show("Not Processed");
                }
            }
            else
            {
                SaleProcessingInfo retVal = new SaleProcessingInfo();
                int mode = _salesProcessingManager.CheckIfDataPresentInSalesProcessing(dtBillDate.Value.Date, Convert.ToInt32(cboCustomer.SelectedValue), out retVal);

                if (mode == 1)
                {
                    DialogResult result = MessageBox.Show("Do You Want to Overwrite?", "", MessageBoxButtons.OKCancel);
                    switch (result)
                    {
                        case DialogResult.OK:
                            {
                                bool result1 = _salesProcessingManager.ProcessSalesDataOfSingleCustomers(1, dtBillDate.Value.Date, retVal);
                                break;
                            }
                        case DialogResult.Cancel:
                            {
                                this.Close();
                                break;
                            }
                    }
                }
                else if (mode == 2)
                {
                    bool result1 = _salesProcessingManager.ProcessSalesDataOfSingleCustomers(2, dtBillDate.Value.Date, retVal);
                }
                else if (mode == 3)
                {
                    MessageBox.Show("Process ALL Customers For the First Time");
                }
            }
        }
        #endregion

        #region Close Form
        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        void CloseForm()
        {
            this.DialogResult = DialogResult.OK;
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region Print
        private void Print()
        {
            try
            {
                ReportClass report = new ReportClass();
                GetCutomerIDOnGrid();
                string CustomerIDs = GetCutomerIDOnGrid();
                if (CustomerIDs != "")
                {
                    report.ShowSalesProcessingReport(CustomerIDs, dtBillDate.Value.Date, cboSearchIn.SelectedIndex);
                }
                else
                {
                    MessageBox.Show("Their Is No Record To Print");
                }
                //if (cboSearchIn.SelectedIndex == 1 && txtCustomerNo.Text!="" && Convert.ToInt32(txtCustomerNo.Text) > 0)
                //    { report.ShowSalesProcessingReport(Convert.ToInt32(txtCustomerNo.Text), dtBillDate.Value.Date, cboSearchIn.SelectedIndex); }
                //else
                //    report.ShowSalesProcessingReport(Convert.ToInt32(cboCustomer.SelectedValue), dtBillDate.Value.Date, cboSearchIn.SelectedIndex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetCutomerIDOnGrid()
        {
            string customerIDs = "";
            foreach (DataGridViewRow row in grdSaleProcessing.Rows)
            {
                customerIDs += row.Cells["CustomerID"].Value.ToString() + ",";
            }
            customerIDs = customerIDs.TrimEnd(',');
            return customerIDs;
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindingList<SaleProcessingInfo> retVal = new BindingList<SaleProcessingInfo>();
            if (cboSearchIn.SelectedIndex == 0)
            {
                retVal = _salesProcessingManager.GetAllLSalesProcessingList(cboSearchIn.SelectedIndex, cboCustomer.Text , dtBillDate.Value.Date, txtCustomerNo.Text!="" ? Convert.ToInt32(txtCustomerNo.Text) : 0,0);
            }
            else if (cboSearchIn.SelectedIndex == 1)
            {
                retVal = _salesProcessingManager.GetAllLSalesProcessingList(cboSearchIn.SelectedIndex, (Convert.ToString(cboCustomer.Text) != "") ? cboCustomer.Text : "0", dtBillDate.Value.Date, txtCustomerNo.Text != "" ? Convert.ToInt32(txtCustomerNo.Text) : 0,0);
            }
            else if (cboSearchIn.SelectedIndex == 2)
            {
                retVal = _salesProcessingManager.GetAllLSalesProcessingList(cboSearchIn.SelectedIndex, (Convert.ToString(cboCustomer.Text) != "") ? cboCustomer.Text : "0", dtBillDate.Value.Date, txtCustomerNo.Text != "" ? Convert.ToInt32(txtCustomerNo.Text) : 0,Convert.ToInt32(cboLines.SelectedValue));
            }
            grdSaleProcessing.DataSource = retVal;
            DisableFields();
        }
        #endregion

        #region Control Event
        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchIn.SelectedIndex == 0)
            {
                lblCustomer.Visible = true;
                cboCustomer.Visible = true;
                lblPaidAmount.Visible = false;
                txtCustomerNo.Visible = false;
                cboLines.Visible = false;
                cboCustomer.SelectedIndex = -1;
            }
             if (cboSearchIn.SelectedIndex == 1)
            {
                lblCustomer.Visible = false;
                cboCustomer.Visible = false;
                lblPaidAmount.Visible = true;
                txtCustomerNo.Visible = true;
                cboLines.Visible = false;
                txtCustomerNo.Text = "0";
                lblPaidAmount.Text = (_viewMode == EnumClass.SaleProcessingMode.PrintMode) ? "Customer No" : "Customer No";
            }
            else if (cboSearchIn.SelectedIndex == 2)
            {
                FillLines();
                //lblCustomer.Visible = true;
                cboCustomer.Visible = false;
                lblPaidAmount.Visible = true;
                txtCustomerNo.Visible = true;
                cboLines.Visible = true;
                txtCustomerNo.Text = "0";
                lblPaidAmount.Text = "Customer No";
            }
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

        private void grdSaleProcessing_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (btnProcess.Text == "Payment")
            {
                Payment();
            }
        }
        #endregion

        private void cboLines_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillCustomers(Convert.ToInt32(cboLines.SelectedValue));
            
        }

    }
}
