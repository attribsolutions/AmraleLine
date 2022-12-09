using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using BusinessLogic;

namespace SweetPOS
{
    public partial class frmMain : Form
    {
        string _picFileName = string.Empty;
        ctlSales _ctlSale = null;
        SettingManager _settingManager = new SettingManager();
        UserManager _userManager = new UserManager();
        int _DivisionID = 0;

        public frmMain()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Main Screen";
        }

        public frmMain(int DivisionID)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Main Screen";
            Program.DivisionID = DivisionID;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!CheckLicense())
            {
                MessageBox.Show("License validation failed.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Dispose();
            }           

            CheckLoginRoleAndHideMenus();

            Classes.UserRights u = new SweetPOS.Classes.UserRights(EnumClass.UserRoles.Cashier, (Control)sender);
            u = null;

            mnuLoggedInAs.Text = "Logged in as " + Program.LoginName + " at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " in Division: " + Program.DivisionID;
            lblFirmName.Text = Program.COMPANYNAME;
            lblAddress1.Text = Program.ADDRESS1;
            lblAddress2.Text = Program.ADDRESS2;

            LoadPicture();

            //Hide Menu Items 
            itemGroupMasterToolStripMenuItem.Visible = false;
            itemCompaniesToolStripMenuItem.Visible = false;
            supplierToolStripMenuItem.Visible = false;
            //userMasterToolStripMenuItem.Visible = false;
            stockToolStripMenuItem.Visible = false;
            
            //Hide Reports Menu
            saleToolStripMenuItem.Visible = false;
             timeWiseSummaryToolStripMenuItem.Visible=false;
             saleSummaryByBillToolStripMenuItem.Visible=false;
             stockReportToolStripMenuItem.Visible=false;
             materialRegisterToolStripMenuItem.Visible=false;
             salePaymentToolStripMenuItem.Visible=false;
             customerSaleToolStripMenuItem.Visible = false;

            //List Menu
           //  homeDeliveryProductsToolStripMenuItem.Visible = false;
             homeDeliveryMilkToolStripMenuItem1.Visible = false;
            // rateMasterToolStripMenuItem.Visible = false;
        }


        private void CheckLoginRoleAndHideMenus()
        {
            try
            {
                System.ComponentModel.BindingList<DataObjects.UserInfo> user = _userManager.GetUsersByName(Program.LoginName);
                Program.UserRole = user[0].UserRole.Trim();
                if (Program.UserRole == "Cashier")      //Hardcode
                {
                    mnuToolsDatabaseUtilitiesRestore.Visible = false;
                    mnuToolsSettings.Visible = false;                    
                    mnuHelp.Visible = false;
                    salePaymentToolStripMenuItem1.Visible = false;
                   
                    saleProcessingToolStripMenuItem1.Visible = false;
                    saleProcessingPrintToolStripMenuItem1.Visible = false;
                    stockToolStripMenuItem.Visible = false;
                    userMasterToolStripMenuItem.Visible = false;
                    viewToolStripMenuItem.Visible = false;
                    userMasterToolStripMenuItem.Visible = false;
                    try
                    {                       
                        Program.ALLOWDIRECTINITIALIZATION = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool CheckLicense()
        {
            return true;
            if (Program.TEMPSERIAL == Program.SERIALKEY && Program.SERIALKEY != string.Empty)
                return true;

            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Program.REGKEY, false);
            if (regKey != null)
            {
                object objSerialNumber = regKey.GetValue("SerialNumber");

                if (objSerialNumber != null)
                {
                    if (objSerialNumber.ToString() == Program.SERIALKEY)
                        return true;
                }
            }

            return false;
        }

        void LoadPicture()
        {
            string defaultBackImage = Application.StartupPath + "\\Images\\Back.Jpg";
            if (System.IO.File.Exists(defaultBackImage))
            {
                picBack.BackgroundImage = Image.FromFile(defaultBackImage);
                picBack.Visible = true;
                picBack.Size = new Size(pnlControls.Width, pnlControls.Height);
                picBack.Location = new Point(242, 36);
                grpInfo.Parent = picBack;
                grpInfo.Location = new Point(picBack.Width - grpInfo.Width - 4, picBack.Height - grpInfo.Height - 4);
            }

            try
            {
                _picFileName = _settingManager.GetSetting(3);       //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string extension = _picFileName.Substring(_picFileName.Length - 3, 3);
            if (extension.ToUpper() == "JPG")
            {
                if (System.IO.File.Exists(_picFileName))
                {
                    picBack.BackgroundImage = Image.FromFile(_picFileName);
                    picBack.Visible = true;
                    picBack.Size = new Size(pnlControls.Width, pnlControls.Height);
                    picBack.Location = new Point(242, 36);
                    grpInfo.Parent = picBack;
                    grpInfo.Location = new Point(picBack.Width - grpInfo.Width - 4, picBack.Height - grpInfo.Height - 4);
                }
                else
                {
                    if (!System.IO.File.Exists(defaultBackImage))
                        picBack.BackgroundImage = null;
                }
            }
            else
            {
                if (!System.IO.File.Exists(defaultBackImage))
                    picBack.BackgroundImage = null;
            }
        }        

        bool CheckFormInstanceExist(string formName)
        {
            bool disposeUserControl;
            try
            {
                disposeUserControl = Convert.ToBoolean(_settingManager.GetSetting(4));     //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            foreach (Control c in pnlControls.Controls)
            {
                if (disposeUserControl)
                    c.Dispose();
                else
                    c.Hide();
            }

            //foreach (Control c in pnlControls.Controls)
            //{
            //    if (c.Name == formName)
            //    {
            //        if (formName == "ctlChallans")
            //        {
            //            ctlChallans ctrl = (ctlChallans)c;
            //            if (Program.ChallanFormMode == ctrl.ChallanFormMode)
            //            {
            //                c.Show();
            //                picBack.Visible = false;
            //                grpInfo.Visible = false;
            //                c.Focus();
            //                return false;
            //            }
            //            else
            //                return true;
            //        }
            //        else
            //        {
            //            c.Show();
            //            picBack.Visible = false;
            //            grpInfo.Visible = false;
            //            c.Focus();
            //            return false;
            //        }
            //    }
            //}

            return true;
        }        

        private void pnlControls_ControlAdded(object sender, ControlEventArgs e)
        {
            grpInfo.Visible = false;
            picBack.Visible = false;
        }

        private void pnlControls_ControlRemoved(object sender, ControlEventArgs e)
        {
            grpInfo.Visible = true;
            picBack.Visible = true;
            if (System.IO.File.Exists(_picFileName))
            {
                picBack.Visible = true;
                picBack.Size = new Size(pnlControls.Width, pnlControls.Height);
                picBack.Location = new Point(242, 41);
                grpInfo.Parent = picBack;
                grpInfo.Location = new Point(picBack.Width - grpInfo.Width - 4, picBack.Height - grpInfo.Height - 4);
            }
        }               

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Confirm exit?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
                e.Cancel = true;
        }

        void ShowControl(Control control)
        {
            if (!CheckFormInstanceExist(control.Name))
                return;

            pnlControls.SuspendLayout();
            control.Dock = DockStyle.Fill;
            pnlControls.Controls.Add(control);
            control.Focus();
            pnlControls.ResumeLayout();
        }

        //#region Button Click...

        //private void btnSupplier_Click(object sender, EventArgs e)
        //{
        //    ctlSuppliers ctl = new ctlSuppliers();
        //    ShowControl(ctl);
        //    ctl.SearchSuppliers();
        //}

        //private void btnItemGroup_Click(object sender, EventArgs e)
        //{
        //    ctlItemGroups ctl = new ctlItemGroups();
        //    ShowControl(ctl);
        //    ctl.SearchItemGroups();
        //}

        //private void btnItem_Click(object sender, EventArgs e)
        //{
        //    ctlItems ctl = new ctlItems();
        //    ShowControl(ctl);
        //    ctl.SearchItems();
        //}

        //private void btnCustomer_Click(object sender, EventArgs e)
        //{
        //    ctlCustomers ctl = new ctlCustomers();
        //    ShowControl(ctl);
        //    ctl.SearchCustomers();
        //}

        //private void btnUser_Click(object sender, EventArgs e)
        //{
        //    ctlUsers ctl = new ctlUsers();
        //    ShowControl(ctl);
        //    ctl.SearchUsers();
        //}

        //private void btnChallan_Click(object sender, EventArgs e)
        //{
        //    frmChallan frm = new frmChallan();
        //    frm.ShowDialog();
        //    frm.Dispose();
        //    frm = null;
        //}

        //private void btnInvoice_Click(object sender, EventArgs e)
        //{
        //    frmInvoice frm = new frmInvoice();
        //    frm.ShowDialog();
        //    frm.Dispose();
        //    frm = null;
        //}

        //private void btnSale_Click(object sender, EventArgs e)
        //{
        //    if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID)
        //    {
        //        string _animationImage = string.Empty;
        //        try { _animationImage = _settingManager.GetSetting(11); }
        //        catch (Exception ex) { MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        //        frmSaleRFID frm = new frmSaleRFID();
        //        frm.WindowState = FormWindowState.Maximized;
        //        if (System.IO.File.Exists(_animationImage))
        //            frm.AnimatedImage = new Bitmap(_animationImage);
        //        frm.ShowDialog();
        //    }
        //    else if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithNetworking)
        //    {
        //        //New screen for showing bill by card number
        //    }
        //    else if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenAtCounterWithBarCode)
        //    {
        //        frmSale frm = new frmSale();
        //        frm.ShowDialog();
        //    }
        //    else if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ManualBilling)
        //    {
        //        frmSaleManual frm = new frmSaleManual();
        //        frm.ShowDialog();
        //    }
        //}

        //private void btnReceiptPayment_Click(object sender, EventArgs e)
        //{
        //    frmReceiptPayment frm = new frmReceiptPayment();
        //    frm.ShowDialog();
        //    frm.Dispose();
        //    frm = null;
        //}

        //private void btnOrders_Click(object sender, EventArgs e)
        //{
        //    ctlOrders ctl = new ctlOrders();
        //    ShowControl(ctl);
        //    ctl.SearchOrders();
        //}

        //private void btnStock_Click(object sender, EventArgs e)
        //{
        //    frmStock frm = new frmStock();
        //    frm.ShowDialog();
        //}

        //private void btnStockAdjustment_Click(object sender, EventArgs e)
        //{
        //    ctlStock ctl = new ctlStock();
        //    ShowControl(ctl);
        //    ctl.SearchStockAdjustment();
        //}

        //#endregion

        #region Menu Click...

        private void mnuViewSaleSummary_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.SaleSummaryVat);
            frm.ShowDialog();
        }

        private void mnuViewTimeWiseSaleSummary_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.TimewiseSaleSummary);
            frm.ShowDialog();
        }

        private void mnuViewPeriodicalSaleSummary_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.SaleSummaryPeriodical);
            frm.ShowDialog();
        }

        private void mnuViewCashierWiseSale_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.CashierwiseSale);
            frm.ShowDialog();
        }

        private void mnuViewShowSingleBill_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.ShowBill);
            frm.ShowDialog();
        }

        private void mnuViewStockReport_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.StockReport);
            frm.ShowDialog();
        }

        private void mnuViewChallanReport_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.ChallanReport);
            frm.ShowDialog();
        }

        private void mnuViewInvoiceReport_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.PurchaseReport);
            frm.ShowDialog();
        }

        private void mnuViewReceiptPaymentReport_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.CashBankTransaction);
            frm.ShowDialog();
        }

        private void mnuListReceiptPayments_Click(object sender, EventArgs e)
        {
            ctlReceiptPayments ctl = new ctlReceiptPayments(_ctlSale);
            ShowControl(ctl);
            ctl.SearchReceiptPayments();
        }

        private void mnuListSales_Click(object sender, EventArgs e)
        {
            _ctlSale = new ctlSales();
            ShowControl(_ctlSale);
            _ctlSale.SearchSales();
        }

        private void mnuViewItemwiseSaleReport_Click(object sender, EventArgs e)
        {
            frmSaleSummary frm = new frmSaleSummary(EnumClass.ReportFormMode.SaleSummaryPeriodicalItemwise);
            frm.ShowDialog();
        }

        #endregion

        #region File
        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region List
        //private void mnuListChallans_Click(object sender, EventArgs e)
        //{
        //    ctlChallans ctl = new ctlChallans(1);
        //    ShowControl(ctl);
        //    ctl.SearchChallans();
        //}
        private void salesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ctlSales ctl = new ctlSales();
            ShowControl(ctl);
            ctl.SearchSales();
        }
        private void salePaymentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ctlSalesPayment ctl = new ctlSalesPayment();
            ShowControl(ctl);
        }
        private void saleSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Sale Menu
        private void saleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSale frm = new frmSale();
            frm.ShowDialog();
        }

        private void saleProcessingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmSaleProcessing frm = new frmSaleProcessing(1);
            frm.ShowDialog();
        }

        private void saleProcessingPrintToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmSaleProcessing frm = new frmSaleProcessing(2);
            frm.ShowDialog();
        }

        private void salePaymentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmSaleProcessing frm = new frmSaleProcessing(3);
            frm.ShowDialog();
        }
        #endregion

        #region Master
        private void customerMasterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ctlCustomers ctl = new ctlCustomers();
            ShowControl(ctl);
            ctl.SearchCustomers();
        }

        private void itemMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlItems ctl = new ctlItems();
            ShowControl(ctl);
            ctl.SearchItems();
        }

        private void lineMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlLines ctl = new ctlLines();
            ShowControl(ctl);
            ctl.SearchLines();
        }

        private void linemanMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlLineman ctl = new ctlLineman();
            ShowControl(ctl);
            ctl.SearchLinemans();
        }

        private void rateMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlRates ctl = new ctlRates();
            ShowControl(ctl);
            ctl.SearchRates();
        }

        private void customerMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlCustomerMessages ctl = new ctlCustomerMessages();
            ShowControl(ctl);
            ctl.SearchCustomerMessages();
        }

        private void itemGroupMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlItemGroups ctl = new ctlItemGroups();
            ShowControl(ctl);
            ctl.SearchItemGroups();
        }

        private void itemCompaniesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlItemCompanies ctl = new ctlItemCompanies();
            ShowControl(ctl);
            ctl.SearchItemCompanys();
        }

        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlSuppliers ctl = new ctlSuppliers();
            ShowControl(ctl);
            ctl.SearchSuppliers();
        }

        private void userMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlUsers ctl = new ctlUsers();
            ShowControl(ctl);
            ctl.SearchUsers();
        }
        #endregion

        #region Stock
        private void generateStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStock frm = new frmStock();
            frm.ShowDialog();
        }

        private void stockAdjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlStock ctl = new ctlStock();
            ShowControl(ctl);
            ctl.SearchStockAdjustment();
        }
        #endregion

        #region Help
        private void mnuHelpCompanyInfo_Click(object sender, EventArgs e)
        {
            frmCompanyInfo frm = new frmCompanyInfo();
            frm.ShowDialog();
        }
        #endregion

        #region Tools
        private void mnuToolsDatabaseUtilitiesBackup_Click(object sender, EventArgs e)
        {
            frmDatabaseBackup frm = new frmDatabaseBackup();
            frm.ShowDialog();
        }

        private void mnuToolsDatabaseUtilitiesRestore_Click(object sender, EventArgs e)
        {
            frmDatabaseRestore frm = new frmDatabaseRestore();
            frm.ShowDialog();
        }

        private void mnuToolsSettings_Click(object sender, EventArgs e)
        {
            frmSettings frm = new frmSettings();
            if (frm.ShowDialog() == DialogResult.OK)
                LoadPicture();
        }
        #endregion

        #region Inward
        //private void challansToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ctlChallans ctl = new ctlChallans(1);
        //    Program.ChallanFormMode = 1;
        //    ShowControl(ctl);
        //    ctl.SearchChallans();
        //}
        #endregion

        #region Report
        private void saleSummaryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.iSaleSummary);
            frm.ShowDialog();
        }
        private void timeWiseSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.TimewiseSaleSummary);
            frm.ShowDialog();
        }
        private void saleSummaryByBillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.SaleSummaryByBill);
            frm.ShowDialog();
        }
        private void stockReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.StockReport);
            frm.ShowDialog();
        }

        private void materialRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.MaterialRegister);
            frm.ShowDialog();
        }
        private void salePaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.SalePayment);
            frm.ShowDialog();
        }
        private void customerSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.SaleTypeWise);
            frm.ShowDialog();
        }
        #endregion

        //private void dispatchToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ctlChallans ctl = new ctlChallans(2);
        //    Program.ChallanFormMode = 2; 
        //    ShowControl(ctl);
        //    ctl.SearchChallans();
        //}

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void customerMessageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ctlMessages ctl = new ctlMessages();
            ShowControl(ctl);
        }

        private void customerMessagesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.CustomerMessages);
            frm.ShowDialog(); 

        }

        private void lineManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlLinemanMilkIssue ctl = new ctlLinemanMilkIssue();
            ShowControl(ctl);
        }

        private void returnMilkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlReturnMilk ctl = new ctlReturnMilk();
            ShowControl(ctl);
        }

        private void homeDeliveryMilkToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ctlHomeDeliveryMilk ctl= new ctlHomeDeliveryMilk();
            ShowControl(ctl);
        }

        private void homeDeliveryProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // ctlHomeDeliveryPruduct ctl = new ctlHomeDeliveryPruduct();
            ctlSales ctl = new ctlSales();
            ShowControl(ctl);
        }

        private void customerMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctlCustomerMessages ctl = new ctlCustomerMessages();
            ShowControl(ctl);
            ctl.SearchCustomerMessages();
        }

        private void calculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("calc.exe");
        }

        private void monthlyStanderdOfMilkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMonthlyMilkStanderd frm = new frmMonthlyMilkStanderd();
            frm.ShowDialog();
            
        }

        private void milkSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.MilkSummary);
            frm.ShowDialog(); 
        }

        private void productSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.ProductSummary);
            frm.ShowDialog(); 
        }

        private void homeDeliveryMilkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHomeDeliveryMilk frm = new frmHomeDeliveryMilk();
            frm.ShowDialog();
        }

        private void homeDeliveryProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleManual frm = new frmSaleManual();
            frm.ShowDialog();
        }

        private void customerBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.CustomerBalance);
            frm.ShowDialog();
        }

        private void changeCustomerLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLineChange frm = new frmLineChange();
            frm.ShowDialog();
        }

        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void customerBalanceWithMilkDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.CustomerBalanceWithMilkDetails);
            frm.ShowDialog();
        }

        private void lineWiseOutstandingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.LineWiseOutStanding);
            frm.ShowDialog();
        }

        private void salePaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.SalePayment);
            frm.ShowDialog();
        }

        private void customerOutstandingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.CustomerOutStanding);
            frm.ShowDialog();
        }

        private void cowBuffloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSaleSummary1 frm = new frmSaleSummary1(EnumClass.ReportFormMode.CowBuffloMilkQuantity);
            frm.ShowDialog();
        }


    }
}