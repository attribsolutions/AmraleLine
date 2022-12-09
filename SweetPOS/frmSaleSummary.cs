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
    public partial class frmSaleSummary : Form
    {
        SaleItemManager _saleItemManager = new SaleItemManager();
        EnumClass.ReportFormMode _reportMode;
        SupplierManager _supplierManager = new SupplierManager();
        ItemManager _itemManager = new ItemManager();
        UserManager _userManager = new UserManager();
        SettingManager _settingManager = new SettingManager();

        public frmSaleSummary(EnumClass.ReportFormMode reportMode)
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
            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryVat)
            {
                lblHeading.Text = "Sale Summary (Vatwise)";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = lblItem.Visible = cboItems.Visible = false;
            }
            if (_reportMode == EnumClass.ReportFormMode.TimewiseSaleSummary)
            {
                lblHeading.Text = "Timewise Sale Summary";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = false;
                dtSDate.Format = dtEDate.Format = DateTimePickerFormat.Custom;
                dtSDate.CustomFormat = dtEDate.CustomFormat = "dd/MM/yyyy hh:mm:ss tt";
                dtSDate.Width = dtEDate.Width = 170;
            }
            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryPeriodical)
            {
                lblHeading.Text = "Periodical Sale Summary";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = false;
            }
            if (_reportMode == EnumClass.ReportFormMode.ShowBill)
            {
                lblHeading.Text = "Show Single Bill";
                dtSDate.Enabled = dtEDate.Enabled = lblSupplier.Visible = cboSupplier.Visible = false;
            }
            if (_reportMode == EnumClass.ReportFormMode.CashierwiseSale)
            {
                FillCashier();
                cboCashier.SelectedIndex = -1;
                lblHeading.Text = "Cashierwise Sale Summary";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = false;
                lblCashier.Visible = cboCashier.Visible = lblMorEve.Visible = cboMorEve.Visible = true;
                dtEDate.Enabled = false;
            }
            if (_reportMode == EnumClass.ReportFormMode.StockReport)
            {
                FillDivisions();
                lblHeading.Text = "Stock Report";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = lblToDate.Visible = dtEDate.Visible = false;
                lblFromDate.Text = "Date";
            }
            if (_reportMode == EnumClass.ReportFormMode.ChallanReport)
            {
                lblHeading.Text = "Challan Report";
                lblBillNo.Visible = txtBillNumber.Visible = false;
                FillSuppliers();
                cboSupplier.SelectedIndex = -1;
            }
            if (_reportMode == EnumClass.ReportFormMode.PurchaseReport)
            {
                lblHeading.Text = "Invoice Report";
                lblBillNo.Visible = txtBillNumber.Visible = false;
                FillSuppliers();
                cboSupplier.SelectedIndex = -1;
            }
            if (_reportMode == EnumClass.ReportFormMode.CashBankTransaction)
            {
                lblHeading.Text = "Receipt Payment Report";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = false;
            }

            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryPeriodicalItemwise)
            {
                FillItems();
                cboItems.SelectedIndex = -1;
                lblItem.Visible = cboItems.Visible = true;
                lblHeading.Text = "Item wise Sale Report";
                lblBillNo.Visible = txtBillNumber.Visible = lblSupplier.Visible = cboSupplier.Visible = false;
            }
        }

        bool FillSuppliers()
        {
            BindingList<SupplierInfo> suppliers = new BindingList<SupplierInfo>();
            try
            {
                suppliers = _supplierManager.GetSuppliersAll(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Suppliers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboSupplier.DataSource = suppliers;
            cboSupplier.DisplayMember = "Name";
            cboSupplier.ValueMember = "ID";

            suppliers = null;
            return true;
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

        bool FillItems()
        {
            BindingList<ItemInfo> items = new BindingList<ItemInfo>();
            try
            {
                items = _itemManager.GetItemsAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboItems.DataSource = items;
            cboItems.DisplayMember = "Name";
            cboItems.ValueMember = "ID";

            items = null;
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

        void FillDivisions()
        {
            DivisionManager _divisionManager = new DivisionManager();
            try
            {
                BindingList<DivisionInfo> division = _divisionManager.GetAllDivisionByUserID(Convert.ToInt32(Program.CURRENTUSER));
                cboDivision.DataSource = division;
                cboDivision.DisplayMember = "DivisionName";
                cboDivision.ValueMember = "DivisionID";
                if (division.Count > 0)
                {
                    lblDivision.Visible = true;
                    cboDivision.Visible = true;
                    cboDivision.Enabled = true;
                    if (division.Count == 1)
                    {
                        cboCashier.Enabled = false;
                    }
                }
                division = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting all Divisions." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnShowSummary_Click(object sender, EventArgs e)
        {
            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryVat)
                ShowSaleSummaryVat();

            if (_reportMode == EnumClass.ReportFormMode.TimewiseSaleSummary)
                ShowTimewiseSaleSummary();

            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryPeriodical)
                ShowSaleSummaryPeriodical();

            if (_reportMode == EnumClass.ReportFormMode.CashierwiseSale)
                ShowSaleSummaryCashier();

            if (_reportMode == EnumClass.ReportFormMode.ShowBill)
                ShowSingleBill();

            if (_reportMode == EnumClass.ReportFormMode.StockReport)
                ShowDaywiseStockReport();

            if (_reportMode == EnumClass.ReportFormMode.ChallanReport)
                ShowChallanReport();

            if (_reportMode == EnumClass.ReportFormMode.PurchaseReport)
                ShowInvoiceReport();

            if (_reportMode == EnumClass.ReportFormMode.CashBankTransaction)
                ShowReceiptPaymentReport();

            if (_reportMode == EnumClass.ReportFormMode.SaleSummaryPeriodicalItemwise)
                SaleSummaryPeriodicalItemwise();
        }

        void ShowSaleSummaryVat()
        {
            try
            {
                _saleItemManager.ProcessSummaryNew(dtSDate.Value.Date, dtEDate.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in processing new summary." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportClass rpt = new ReportClass();
            rpt.SaleSummaryVatwise(dtSDate.Value.Date, dtEDate.Value.Date);
        }

        void ShowTimewiseSaleSummary()
        {
            try
            {
                _saleItemManager.ProcessSummaryNew(dtSDate.Value.Date, dtEDate.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in processing new summary." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportClass rpt = new ReportClass();
            rpt.SaleSummaryGroupWise(dtSDate.Value, dtEDate.Value, true);
        }

        void SaleSummaryPeriodicalItemwise()
        {
            try
            {
                _saleItemManager.ProcessSummaryNew(dtSDate.Value.Date, dtEDate.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in processing new summary." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportClass rpt = new ReportClass();
            rpt.SaleSummaryItemwise(Convert.ToInt32(cboItems.SelectedValue), dtSDate.Value.Date, dtEDate.Value.Date);
        }

        void ShowSaleSummaryPeriodical()
        {
            try
            {
                _saleItemManager.ProcessSummaryNew(dtSDate.Value.Date, dtEDate.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in processing new summary." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportClass rpt = new ReportClass();
                
            rpt.SaleSummaryGroupWise(dtSDate.Value.Date, dtEDate.Value.Date, false);
        }

        void ShowSaleSummaryCashier()
        {
            string morningSessionTime = string.Empty;
            string eveningSessionTime = string.Empty;
            bool showSystemStartTime = false;
            try
            {
                morningSessionTime = _settingManager.GetSetting(21);
                eveningSessionTime = _settingManager.GetSetting(22);
                showSystemStartTime = Convert.ToBoolean(_settingManager.GetSetting(23));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Getting system start time setting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int morStartHr = Convert.ToInt32(morningSessionTime.Substring(0, 2));
            int morStartMn = Convert.ToInt32(morningSessionTime.Substring(3, 2));
            int morEndHr = Convert.ToInt32(morningSessionTime.Substring(6, 2));
            int morEndMn = Convert.ToInt32(morningSessionTime.Substring(9, 2));
            int eveStartHr = Convert.ToInt32(eveningSessionTime.Substring(0, 2));
            int eveStartMn = Convert.ToInt32(eveningSessionTime.Substring(3, 2));
            int eveEndHr = Convert.ToInt32(eveningSessionTime.Substring(6, 2));
            int eveEndMn = Convert.ToInt32(eveningSessionTime.Substring(9, 2));

            DateTime morningSessionStartDateTime;
            DateTime morningSessionEndDateTime;
            DateTime eveningSessionStartDateTime;
            DateTime eveningSessionEndDateTime;

            try
            {
                morningSessionStartDateTime = new DateTime(dtSDate.Value.Year, dtSDate.Value.Month, dtSDate.Value.Day, morStartHr, morStartMn, 0);
                morningSessionEndDateTime = new DateTime(dtEDate.Value.Year, dtEDate.Value.Month, dtEDate.Value.Day, morEndHr, morEndMn, 0);
                eveningSessionStartDateTime = new DateTime(dtSDate.Value.Year, dtSDate.Value.Month, dtSDate.Value.Day, eveStartHr, eveStartMn, 0);
                eveningSessionEndDateTime = new DateTime(dtEDate.Value.Year, dtEDate.Value.Month, dtEDate.Value.Day, eveEndHr, eveEndMn, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Session time entered in settings." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (cboMorEve.Text == "Morning")
                {
                    _saleItemManager.ProcessSummaryNew(morningSessionStartDateTime, morningSessionEndDateTime);
                }
                else if (cboMorEve.Text == "Evening")
                {
                    _saleItemManager.ProcessSummaryNew(eveningSessionStartDateTime, eveningSessionEndDateTime);
                }
                else
                {
                    _saleItemManager.ProcessSummaryNew(dtSDate.Value.Date, dtEDate.Value.Date);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in processing new summary." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ReportClass rpt = new ReportClass();

            if (cboCashier.SelectedIndex != -1)
            {
                if (cboMorEve.Text == "Morning")
                {
                    rpt.reportTime = GetReportTime(morningSessionStartDateTime, morningSessionEndDateTime);
                    rpt.SaleSummaryCashierwise(morningSessionStartDateTime, morningSessionEndDateTime, Convert.ToInt32(cboCashier.SelectedValue));
                }
                else if (cboMorEve.Text == "Evening")
                {
                    rpt.reportTime = GetReportTime(eveningSessionStartDateTime, eveningSessionEndDateTime);
                    rpt.SaleSummaryCashierwise(eveningSessionStartDateTime, eveningSessionEndDateTime, Convert.ToInt32(cboCashier.SelectedValue));
                }
                else
                {
                    rpt.reportTime = GetReportTime(morningSessionStartDateTime, eveningSessionEndDateTime);
                    rpt.SaleSummaryCashierwise(dtSDate.Value.Date, new DateTime(dtEDate.Value.Year, dtEDate.Value.Month, dtEDate.Value.Day).AddDays(1).AddSeconds(-1), Convert.ToInt32(cboCashier.SelectedValue));
                }
            }
            else
            {
                if (cboMorEve.Text == "Morning")
                {
                    rpt.reportTime = GetReportTime(morningSessionStartDateTime, morningSessionEndDateTime);
                    rpt.SaleSummaryCashierwise(morningSessionStartDateTime, morningSessionEndDateTime, 0);
                }
                else if (cboMorEve.Text == "Evening")
                {
                    rpt.reportTime = GetReportTime(eveningSessionStartDateTime, eveningSessionEndDateTime);
                    rpt.SaleSummaryCashierwise(eveningSessionStartDateTime, eveningSessionEndDateTime, 0);
                }
                else
                {
                    rpt.reportTime = GetReportTime(morningSessionStartDateTime, eveningSessionEndDateTime);
                    rpt.SaleSummaryCashierwise(dtSDate.Value.Date, new DateTime(dtEDate.Value.Year, dtEDate.Value.Month, dtEDate.Value.Day).AddDays(1).AddSeconds(-1), 0);
                }
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

        void ShowSingleBill()
        {
            int i = 0;
            if (!int.TryParse(txtBillNumber.Text, out i))
            {
                txtBillNumber.Text = string.Empty;
                txtBillNumber.Focus();
            }
            ReportClass rpt = new ReportClass();
            rpt.ShowBill(Convert.ToInt32(txtBillNumber.Text), true, false);
        }

        void ShowDaywiseStockReport()
        {
            ReportClass rpt = new ReportClass();
            rpt.DayWiseStockReport(dtSDate.Value.Date, dtSDate.Value.Date, Convert.ToInt32(cboDivision.SelectedValue),cboDivision.Text);
        }

        void ShowChallanReport()
        {
            ReportClass rpt = new ReportClass();
            if (cboSupplier.SelectedIndex == -1)
                rpt.ChallanReport(dtSDate.Value.Date, dtEDate.Value.Date, 0);
            else
                rpt.ChallanReport(dtSDate.Value.Date, dtEDate.Value.Date, Convert.ToInt32(cboSupplier.SelectedValue));
        }

        void ShowInvoiceReport()
        {
            ReportClass rpt = new ReportClass();
            if (cboSupplier.SelectedIndex == -1)
                rpt.InvoiceReport(dtSDate.Value.Date, dtEDate.Value.Date, 0);
            else
                rpt.InvoiceReport(dtSDate.Value.Date, dtEDate.Value.Date, Convert.ToInt32(cboSupplier.SelectedValue));
        }

        void ShowReceiptPaymentReport()
        {
            ReportClass rpt = new ReportClass();
            rpt.ReceiptPaymentReport(dtSDate.Value.Date, dtEDate.Value.Date);
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
    }
}
