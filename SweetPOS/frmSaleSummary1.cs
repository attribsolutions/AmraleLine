using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;

namespace SweetPOS
{
    public partial class frmSaleSummary1 : Form
    {
        SaleItemManager _saleItemManager = new SaleItemManager();
        UserManager _userManager = new UserManager();
        SettingManager _settingManager = new SettingManager();
        DivisionManager _divisionManager = new DivisionManager();
        ItemManager _itemManager = new ItemManager();
        LineManager _lineManager = new LineManager();
        CustomerManager _customerManager = new CustomerManager();
        EnumClass.ReportFormMode _reportMode;
        Boolean isLineFilled = false;

        public frmSaleSummary1(EnumClass.ReportFormMode reportMode)
        {
            InitializeComponent();
            _reportMode = reportMode;
            this.Text = Program.MESSAGEBOXTITLE + " - Report Form";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void frmSaleSummary_Load(object sender, EventArgs e)
        {

            

            if (_reportMode == EnumClass.ReportFormMode.iSaleSummary)
            {
                lblHeading.Text = "Sale Summary ";
                FillCashier();
                FillDivision();

                cboCashier.SelectedIndex = -1;
                cboDivision.SelectedIndex = -1;
            }
            if (_reportMode == EnumClass.ReportFormMode.TimewiseSaleSummary)
            {
                lblHeading.Text = "Sale Summary ";
                lblDivision.Visible = cboDivision.Visible = false;
                FillSaleType();
                dtSDate.Format = dtEDate.Format = DateTimePickerFormat.Custom;
                dtSDate.CustomFormat = dtEDate.CustomFormat = "dd/MM/yyyy hh:mm tt";
                dtSDate.Width = dtEDate.Width = 170;
                cboDivision.SelectedIndex = -1;
                lblCashier.Text = "Sale Type";
            }
            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryByBill)
            {
                lblHeading.Text = "Sale Summary By Bill";
                FillCashier();
                FillDivision();
                cboCashier.SelectedIndex = -1;
                cboDivision.SelectedIndex = -1;
                lblFromDate.Text = "From Bill No";
                lblToDate.Text = "To Bill No";
                dtSDate.Visible = false;
                dtEDate.Visible = false;
                txtFromBillNo.Visible = true;
                txtToBillNo.Visible = true;
            }
            if (_reportMode == EnumClass.ReportFormMode.StockReport)
            {
                FillDivision();
                cboCashier.Visible = lblCashier.Visible = false;
                lblHeading.Text = "Stock Report";
                //lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = lblToDate.Visible = dtEDate.Visible = false;
                lblFromDate.Text = "Date";
            }
            if (_reportMode == EnumClass.ReportFormMode.MaterialRegister)
            {
                lblHeading.Text = "Material Register ";
                FillItems();
                FillDivision();
                lblCashier.Text = "Item";
                cboCashier.SelectedIndex = -1;
                cboDivision.SelectedIndex = -1;
            }
            if (_reportMode == EnumClass.ReportFormMode.SalePayment)
            {
                lblHeading.Text = "Payment Receipt ";
               
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = true;
                dtEDate.Visible = true;
                lblToDate.Visible = true;
            }
            if (_reportMode == EnumClass.ReportFormMode.SaleTypeWise)
            {
                lblHeading.Text = "Sale Type Summary";
                lblCashier.Text = "Sale Type";
                lblDivision.Visible = cboDivision.Visible = false;
                FillSaleType();
            }
            if (_reportMode == EnumClass.ReportFormMode.CustomerMessages)
            {
                lblHeading.Text = "Customer Messages";
                lblCashier.Text = "Select Lines";
                FillLines();
                lblDivision.Visible = cboDivision.Visible = false;
                dtEDate.Visible = false;
                lblToDate.Visible = false;
            }

            if (_reportMode == EnumClass.ReportFormMode.MilkSummary)
            {
                lblHeading.Text = "Customer Milk Summary";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = true;
                dtEDate.Visible = true;
                lblToDate.Visible = true;
            }
            if (_reportMode == EnumClass.ReportFormMode.ProductSummary)
            {
                lblHeading.Text = "Customer Product Summary";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = true;
                dtEDate.Visible = true;
                lblToDate.Visible = true;
            }
            if (_reportMode == EnumClass.ReportFormMode.CustomerBalance)
            {
                lblHeading.Text = "Customer Balance Summary";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = true;
                dtEDate.Visible = false;
                lblToDate.Visible = false;
                dtSDate.Visible = true;
                lblFromDate.Visible = true;
                lblFromDate.Text = "Month";
                dtSDate.CustomFormat = "MM/yyyy";
            }
            if (_reportMode == EnumClass.ReportFormMode.CustomerBalanceWithMilkDetails)
            {
                lblHeading.Text = "Customer Balance With Milk Summary";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = false;
                dtEDate.Visible = false;
                lblToDate.Visible = false;
                dtSDate.Visible = true;
                lblFromDate.Visible = true;
                lblFromDate.Text = "Month";
                dtSDate.CustomFormat ="MM/yyyy";
            }

            if (_reportMode == EnumClass.ReportFormMode.LineWiseOutStanding)
            {
                lblHeading.Text = "Line Wise Balance Summary";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = false;
                cboCashier.Visible = true;
                dtEDate.Visible = false;
                lblToDate.Visible = false;
                dtSDate.Visible = true;
                lblFromDate.Visible = true;
                lblFromDate.Text = "Month";
                dtSDate.CustomFormat = "MM/yyyy";
            } if (_reportMode == EnumClass.ReportFormMode.CustomerOutStanding)
            {
                lblHeading.Text = "Line Wise Customers OutStanding";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = false;
                cboCashier.Visible = true;
                dtEDate.Visible = false;
                lblToDate.Visible = false;
                dtSDate.Visible = true;
                lblFromDate.Visible = true;
                lblFromDate.Text = "Month";
                dtSDate.CustomFormat = "MM/yyyy";
            }
            if (_reportMode == EnumClass.ReportFormMode.CowBuffloMilkQuantity)
            {
                cboCashier.TabIndex = 3;
                cboDivision.TabIndex = 4;
                lblHeading.Text = "Cow Bufflo Milk Quantity ";
                lblCashier.Text = "Line";
                FillLines();
                lblDivision.Text = "Customer";
                lblDivision.Visible = cboDivision.Visible = true;
                cboCashier.Visible = true;
                dtEDate.Visible = true;
                lblToDate.Visible = true;
                dtSDate.Visible = true;
                lblFromDate.Visible = true;
                lblFromDate.Text = "From Date";
                dtSDate.CustomFormat = "dd/MM/yyyy";
                lblToDate.Text = "To Date";
                dtEDate.CustomFormat = "dd/MM/yyyy";
            }
        }
       
        bool FillLines()
        {
            BindingList<LineInfo> lines = new BindingList<LineInfo>();
            try
            {
                lines = _lineManager.GetLines();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item Groups." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboCashier.DataSource = lines;
            cboCashier.DisplayMember = "LineNumber";
            cboCashier.ValueMember = "ID";
            lines = null;
            cboCashier.SelectedIndex = -1;
    
            isLineFilled = true;

            return true;

        }

        bool FillCustomersByLineID()
        {
            BindingList<CustomerInfo> customer = new BindingList<CustomerInfo>();
            CustomerManager _customerManager = new CustomerManager();
            try
            {
                customer = _customerManager.GetCustomersByLineID(Convert.ToInt32(cboCashier.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cboDivision.DataSource = customer;
            cboDivision.DisplayMember = "Name";
            cboDivision.ValueMember = "ID";
            customer = null;
            cboDivision.SelectedIndex = -1;

            return true;
        }
        
        private void FillSaleType()
        {
            cboCashier.Items.Add("All");
            cboCashier.Items.Add("Card Sale");
            cboCashier.Items.Add("Coupon Sale");
            cboCashier.Items.Add("Cash Sale");
            cboCashier.Items.Add("Coupon Issue");
            cboCashier.SelectedIndex = 0;
        }
           
        bool FillCashier()
        {
            BindingList<UserInfo> users = new BindingList<UserInfo>();
            try
            {
                users = _userManager.GetAllSystemUsers(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Users." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboCashier.DataSource = users;
            cboCashier.DisplayMember = "Name";
            cboCashier.ValueMember = "ID";

            users = null;
            return true;
        }

        bool FillDivision()
        {
            BindingList<DivisionInfo> division = new BindingList<DivisionInfo>();
            try
            {
                division = _divisionManager.GetAllDivision();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboDivision.DataSource = division;
            cboDivision.DisplayMember = "DivisionName";
            cboDivision.ValueMember = "DivisionID";

            division = null;
            return true;
        }

        bool FillItems()
        {
            BindingList<ItemInfo> items = new BindingList<ItemInfo>();
            try
            { items = _itemManager.GetItemsAll(); }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cboCashier.DataSource = items;
            cboCashier.DisplayMember = "Name";
            cboCashier.ValueMember = "ID";
            return true;
        }

        private void frmSaleSummary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                this.DialogResult = DialogResult.Cancel;
            }
        }        

        private DateTime GetReportTime(DateTime sessionStartDateTime, DateTime sessionEndDateTime)
        {
            try
            {
                return _settingManager.GetStartTimeBySessionStartEndTime(sessionStartDateTime, sessionEndDateTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting report Time." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DateTime.Now;
            }
        }

        private void dtEDate_Enter(object sender, EventArgs e)
        {
            dtEDate.Value = dtSDate.Value.Date;
        }

        private void dtSDate_ValueChanged(object sender, EventArgs e)
        {
            if (_reportMode == EnumClass.ReportFormMode.CashierwiseSale)
            {
                dtEDate.Value = dtSDate.Value;
            }
        }

        private void btnShowSummary_Click(object sender, EventArgs e)
        {
            if (_reportMode == EnumClass.ReportFormMode.iSaleSummary)
                ShowServerSaleSummaryVatANDTime(false);
            if (_reportMode == EnumClass.ReportFormMode.TimewiseSaleSummary)
                ShowServerSaleSummaryVatANDTime(true);
            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryByBill)
                ShowSaleSummaryByBill();
            if (_reportMode == EnumClass.ReportFormMode.StockReport)
                ShowDaywiseStockReport();
            if (_reportMode == EnumClass.ReportFormMode.MaterialRegister)
                ShowMaterialResgister();
            if (_reportMode == EnumClass.ReportFormMode.SalePayment)
                ShowSalePayment();
            if (_reportMode == EnumClass.ReportFormMode.SaleTypeWise)
                showSaleTypeWiseReport();
            if (_reportMode == EnumClass.ReportFormMode.CustomerMessages)
            ShowCustomerMessages();
            if (_reportMode == EnumClass.ReportFormMode.MilkSummary)
                MilkSummary();
            if (_reportMode == EnumClass.ReportFormMode.ProductSummary)
                ProductSummary();
            if (_reportMode == EnumClass.ReportFormMode.CustomerBalance)
                CustomerBalance();
            if (_reportMode == EnumClass.ReportFormMode.CustomerBalanceWithMilkDetails)
                CustomerBalanceWithMilkDetails();
            if (_reportMode == EnumClass.ReportFormMode.LineWiseOutStanding)
                LineWiseOutStanding();
            if (_reportMode == EnumClass.ReportFormMode.CustomerOutStanding)
                CustomerOutStanding();
            if (_reportMode == EnumClass.ReportFormMode.CowBuffloMilkQuantity)
                CowBuffloMilkQuantity();
            
        }

        private void CowBuffloMilkQuantity()
        {
            ReportClass rpt = new ReportClass();
            rpt.CowBuffloMilkQuantitySummary(dtSDate.Value.Date, dtEDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToString(cboCashier.Text), Convert.ToInt32(cboDivision.SelectedValue), Convert.ToString(cboDivision.Text));
        }

        private void showSaleTypeWiseReport()
        {
            ReportClass rpt = new ReportClass();
            rpt.ShowSaleTypeWiseReport(dtSDate.Value.Date, dtEDate.Value.Date, cboCashier.SelectedIndex, cboCashier.SelectedIndex <0 ? "All" : cboCashier.Text);
        }

        private void ShowServerSaleSummaryVatANDTime(Boolean IsTimeWise)
        {
            ReportClass rpt = new ReportClass();
            if (!IsTimeWise)
            {
                if (cboCashier.SelectedIndex == -1 && cboDivision.SelectedIndex == -1)
                {
                    rpt.ShowServerSales(1, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, 0, 0, IsTimeWise);
                }
                else if (cboCashier.SelectedIndex >= 0 && cboDivision.SelectedIndex == -1)
                {
                    rpt.ShowServerSales(2, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), 0, IsTimeWise);
                }
                else if (cboCashier.SelectedIndex == -1 && cboDivision.SelectedIndex >= 0)
                {
                    rpt.ShowServerSales(3, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, 0, Convert.ToInt32(cboDivision.SelectedValue), IsTimeWise);
                }
                else
                {
                    rpt.ShowServerSales(4, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToInt32(cboDivision.SelectedValue), IsTimeWise);
                }
            }
            else
            {
                if (cboCashier.SelectedIndex == -1)
                {
                    rpt.ShowServerSales(1, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, 0, 0, IsTimeWise);
                }
                else if (cboCashier.SelectedIndex >= 0)
                {
                    rpt.ShowServerSales(2, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, cboCashier.SelectedIndex , 0, IsTimeWise);
                }
                //else if (cboCashier.SelectedIndex == -1 && cboDivision.SelectedIndex >= 0)
                //{
                //    rpt.ShowServerSales(3, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, 0, Convert.ToInt32(cboDivision.SelectedValue), IsTimeWise);
                //}
                //else
                //{
                //    rpt.ShowServerSales(4, IsTimeWise ? dtSDate.Value : dtSDate.Value.Date, IsTimeWise ? dtEDate.Value : dtEDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToInt32(cboDivision.SelectedValue), IsTimeWise);
                //}
            }
        }

        private void ShowSaleSummaryByBill()
        {
            ReportClass rpt = new ReportClass();
            Int32 frombill = txtFromBillNo.Text == string.Empty ? 0 : Convert.ToInt32(txtFromBillNo.Text);
            Int32 toBill = txtToBillNo.Text == string.Empty ? 0 : Convert.ToInt32(txtToBillNo.Text);
            if (cboCashier.SelectedIndex == -1 && cboDivision.SelectedIndex == -1)
            {
                rpt.ShowSummarySaleByBill(1, frombill, toBill, 0, 0);
            }
            else if (cboCashier.SelectedIndex >= 0 && cboDivision.SelectedIndex == -1)
            {
                rpt.ShowSummarySaleByBill(2, frombill, toBill, Convert.ToInt32(cboCashier.SelectedValue), 0);
            }
            else if (cboCashier.SelectedIndex == -1 && cboDivision.SelectedIndex >= 0)
            {
                rpt.ShowSummarySaleByBill(3, frombill, toBill, 0, Convert.ToInt32(cboDivision.SelectedValue));
            }
            else
            {
                rpt.ShowSummarySaleByBill(4, frombill, toBill, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToInt32(cboDivision.SelectedValue));
            }
        }

        void ShowDaywiseStockReport()
        {
            ReportClass rpt = new ReportClass();
            rpt.DayWiseStockReport(dtSDate.Value.Date, dtSDate.Value.Date, Convert.ToInt32(cboDivision.SelectedValue), cboDivision.Text);
        }

        private void ShowMaterialResgister()
        {
            if (MaterialRegisterValidation())
            {
                ReportClass rpt = new ReportClass();
                rpt.ShowMaterialRegister(dtSDate.Value.Date, dtEDate.Value.Date, cboCashier.SelectedIndex >= 0 ? Convert.ToInt32(cboCashier.SelectedValue) : 0, cboDivision.SelectedIndex >= 0 ? Convert.ToInt32(cboDivision.SelectedValue) : 0, cboCashier.SelectedIndex >= 0 ? cboCashier.Text : null, cboDivision.SelectedIndex >= 0 ? cboDivision.Text : null);
            }
        }

        private void ShowSalePayment()
        {
            ReportClass rpt = new ReportClass();
            rpt.ShowSalePayment(dtSDate.Value.Date, dtEDate.Value.Date, cboCashier.SelectedIndex >= 0 ? Convert.ToInt32(cboCashier.SelectedValue) : 0, cboCashier.SelectedIndex >= 0 ? cboCashier.Text : "All");
        }

        private void ShowCustomerMessages()
        {
            ReportClass rpt = new ReportClass();
            rpt.ShowCustomerMessages(dtSDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToString(cboCashier.Text));
                //(dtSDate.Value.Date,dtEDate.Value.Date,cboCashier.SelectedIndex>=0?Convert.ToInt32(cboCashier.SelectedValue):0,cboCashier.);

        
        }
        private void MilkSummary()
        {
            if (cboCashier.Text != "" && cboDivision.Text != "")
            {
                ReportClass rpt = new ReportClass();
                rpt.MilkSummary(dtSDate.Value.Date, dtEDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToString(cboCashier.Text), Convert.ToInt32(cboDivision.SelectedValue));

                //(dtSDate.Value.Date,dtEDate.Value.Date,cboCashier.SelectedIndex>=0?Convert.ToInt32(cboCashier.SelectedValue):0,cboCashier.);
            }
            else
            {
                cboDivision.Focus();
            }


        }
        private void CustomerBalanceWithMilkDetails()
        {

            ReportClass rpt = new ReportClass();
            rpt.CustomerBalanceWithMilkDetails(dtSDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToInt32(cboDivision.SelectedValue));

        }
        private void LineWiseOutStanding()
        {

            ReportClass rpt = new ReportClass();
            rpt.LineWiseOutStanding(dtSDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToInt32(cboDivision.SelectedValue));

        }
        private void CustomerOutStanding()
        {

            ReportClass rpt = new ReportClass();
            rpt.CustomerOutStanding(dtSDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToInt32(cboDivision.SelectedValue));

        }
        private void ProductSummary()
        {
            
            //if (cboCashier.Text != "" && cboDivision.Text != "")
            //{
                ReportClass rpt = new ReportClass();
                rpt.ProductSummary(dtSDate.Value.Date, dtEDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToString(cboCashier.Text), Convert.ToInt32(cboDivision.SelectedValue));
                //(dtSDate.Value.Date,dtEDate.Value.Date,cboCashier.SelectedIndex>=0?Convert.ToInt32(cboCashier.SelectedValue):0,cboCashier.);
            //}
            //else
            //{
            //    cboDivision.Focus();
            //}

        }
        private void CustomerBalance()
        {
            ReportClass rpt = new ReportClass();
            rpt.CustomerBalance(dtSDate.Value.Date, Convert.ToInt32(cboCashier.SelectedValue), Convert.ToString(cboCashier.Text), Convert.ToInt32(cboDivision.SelectedValue));
            //(dtSDate.Value.Date,dtEDate.Value.Date,cboCashier.SelectedIndex>=0?Convert.ToInt32(cboCashier.SelectedValue):0,cboCashier.);


        }

        private bool MaterialRegisterValidation()
        {
            if (!dateValidation())
            { return false; }
            if (cboCashier.SelectedValue == null)
            {
                MessageBox.Show("Please Enter The Valid Item.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboDivision.SelectedValue == null)
            {
                MessageBox.Show("Please Enter The Valid Division.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private bool dateValidation()
        {
            if (dtSDate.Value.Date > System.DateTime.Now.Date || dtEDate.Value.Date > System.DateTime.Now.Date)
            {
                MessageBox.Show("Futer Date Is Not Valid.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtSDate.Value.Date > dtEDate.Value.Date)
            {
                MessageBox.Show("From Date Should Not Be Greater Than To Date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void cboCashier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLineFilled)
            { 
                cboDivision.SelectedIndex = -1;
                cboDivision.Text = string.Empty;
            }
            //FillCustomersByLineID();
        }

        private void cboCashier_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillCustomersByLineID();
        }

    }
}
