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
    public partial class frmHomeDeliveryMilk: Form
    {
        HomeDeliveryMilkManagaer _HomeDeliveryMilkManagaer = new HomeDeliveryMilkManagaer();
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        HomeDeliveryMilkInfo _homedeliverymilkinfo = new HomeDeliveryMilkInfo();
        BindingList<HomeDeliveryMilkInfo> _saleItems = new BindingList<HomeDeliveryMilkInfo>();
        ItemManager items = new ItemManager();

        int _newRecordID = 0;
        int _UpdatedRecordID = 0;
        bool _newRecordAdded = false;
        bool _UpdatedRecordAdded = false;

        public frmHomeDeliveryMilk()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Home Delivery Milk";
        }

        public frmHomeDeliveryMilk(HomeDeliveryMilkInfo homedeliverymilkinfo)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Home Delivery Milk - " + homedeliverymilkinfo.ID.ToString();
            _homedeliverymilkinfo = homedeliverymilkinfo;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmHomeDeliveryMilk_Load(object sender, EventArgs e)
        {
            FillLines();
            this.grdResult.DefaultCellStyle.Font = new Font("Tahoma", 13);
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                cboLines.SelectedValue = _homedeliverymilkinfo.ID;
                dtMilkDeliveryDate.Value = _homedeliverymilkinfo.MilkDeliveryDate;

                foreach (HomeDeliveryMilkInfo saleItem in _saleItems)
                {
                    int row = grdResult.Rows.Add();
                    grdResult.Rows[row].Cells["ItemCode"].Value = saleItem.ItemCode;
                    grdResult.Rows[row].Cells["CustomerName"].Value = saleItem.Name;
                }
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
                MessageBox.Show("Error in getting Lines." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            cboLines.DataSource = lines;
            cboLines.DisplayMember = "LineNumber";
            cboLines.ValueMember = "ID";
            lines = null;
            cboLines.SelectedIndex = -1;
            return true;
        }

            private void btnGo_Click(object sender, EventArgs e)
            {
                cboLines.Enabled = false;
                grdResult.Rows.Clear();
                System.Threading.Thread.Sleep(500);
               
                if (!CheckTodaysSaleEntryExist())
                {
                    ShowCustomerForDomedeliveryMilkEntry(false);
                }
                else
                { 
                    ShowCustomerForDomedeliveryMilkEntry(true);
                }
            }
            bool ShowCustomerForDomedeliveryMilkEntry(bool DataExist)
            {
                BindingList<HomeDeliveryMilkInfo> homedeliverymilkinfo = new BindingList<HomeDeliveryMilkInfo>();
                ItemInfo CowRate = items.GetItem(1);
                ItemInfo BuffaloRate = items.GetItem(2);
                


                try
                {
                    HomeDeliveryMilkInfo homedeliverycustomerinfo = new HomeDeliveryMilkInfo();
                    homedeliverymilkinfo = _HomeDeliveryMilkManagaer.ShowCustomerForHomedeliveryMilkEntry(Convert.ToInt32(cboLines.SelectedValue), Convert.ToDateTime(dtMilkDeliveryDate.Value).Date, DataExist);
                   
                    foreach (HomeDeliveryMilkInfo orderItem in homedeliverymilkinfo)
                    {
                        int row = grdResult.Rows.Add();
                        grdResult.Rows[row].Cells["CustomerName"].Value = orderItem.Name;
                        grdResult.Rows[row].Cells["CustomerNumber"].Value = orderItem.CustomerNumber;
                        grdResult.Rows[row].Cells["CustomerID"].Value = orderItem.ID;
                        grdResult.Rows[row].Cells["Buffalo"].Value = orderItem.Buffalo;
                        grdResult.Rows[row].Cells["BuffaloRate"].Value = BuffaloRate.Rate;
                        grdResult.Rows[row].Cells["Cow"].Value = orderItem.Cow;
                        grdResult.Rows[row].Cells["CowRate"].Value = CowRate.Rate;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                homedeliverymilkinfo = null;
                return true;
            }

            private void btnSaveNew_Click(object sender, EventArgs e)
            {
                if (Save())
                   New();
            }

            bool Save()
            {
                btnSaveClose.Enabled = false;
                if (!ValidateFields())
                    return true;

                BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo = new BindingList<HomeDeliveryMilkInfo>();
                HomeDeliveryMilkInfo HomeDeliveryMilkIssue = SetValue(out HomeDeliveryMilkInfo);

                if (!CheckTodaysSaleEntryExist())
                {
                    if (_viewMode == EnumClass.FormViewMode.Addmode)
                    {
                        try
                        {
                            _newRecordID = Convert.ToInt32(_HomeDeliveryMilkManagaer.HomedeliveryMilkByLineman(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo));
                            _newRecordAdded = true;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error in saving  Home delivery Milk " + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        _UpdatedRecordID = Convert.ToInt32(_HomeDeliveryMilkManagaer.UpdateHomedeliveryMilkByLineman(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo));
                        _UpdatedRecordAdded = true;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in Updating  Home delivery Milk " + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                btnSaveClose.Enabled = true;
                return false;
               

            }
            private bool CheckTodaysSaleEntryExist()
            {
                bool retVal;
                int exist;
               
                //System.Timers.Timer stopWatch;
                //int timeout = 300000;
                try
                {
                    exist = _HomeDeliveryMilkManagaer.CheckTodaysMilkEntryExist(dtMilkDeliveryDate.Value.Date, Convert.ToInt32(cboLines.SelectedValue));
                    //stopWatch = new System.Timers.Timer(timeout);
                    //stopWatch.Start();
                    if (exist > 0)
                    {
                        retVal = true;
                    }
                    else
                    {
                        retVal = false;
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking todays sale entry." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", "asdf", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return retVal;
            }

            HomeDeliveryMilkInfo SetValue(out BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo)
            {
                HomeDeliveryMilkInfo retVal = new HomeDeliveryMilkInfo();
                retVal.MilkDeliveryDate = Convert.ToDateTime(dtMilkDeliveryDate.Value).Date;
                retVal.LineID = Convert.ToInt32(cboLines.SelectedValue);
                retVal.CreatedBy = Program.CURRENTUSER;
                retVal.CreatedOn = DateTime.Now;
                retVal.UpdatedBy = Program.CURRENTUSER;
                retVal.UpdatedOn = DateTime.Now;
               
                BindingList<HomeDeliveryMilkInfo> MilkIssue = new BindingList<HomeDeliveryMilkInfo>();
                foreach (DataGridViewRow row in grdResult.Rows)
                {
                        HomeDeliveryMilkInfo items = new HomeDeliveryMilkInfo();
                        items.ID = Convert.ToInt32(row.Cells["CustomerID"].Value);
                        items.Name = Convert.ToString(row.Cells["CustomerName"].Value);
                        items.Buffalo = Convert.ToDecimal(row.Cells["Buffalo"].Value);
                        items.BuffaloRate = Convert.ToDecimal(row.Cells["BuffaloRate"].Value);
                        items.Cow = Convert.ToDecimal(row.Cells["Cow"].Value);
                        items.CowRate = Convert.ToDecimal(row.Cells["CowRate"].Value);
                        items.LineID = Convert.ToInt32(cboLines.SelectedValue);
                        MilkIssue.Add(items);
                }
                HomeDeliveryMilkInfo = MilkIssue;
                return retVal;
            }

            bool ValidateFields()
            {
                if (cboLines.Text.Trim() == string.Empty)
                {
                    cboLines.Focus();
                    MessageBox.Show("Please Select Line.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }

            void New()
            {
                _viewMode = EnumClass.FormViewMode.Addmode;
                btnSaveNew.Text = "Save && Ne&w";
                btnSaveClose.Text = "Save && Cl&ose";
                cboLines.SelectedValue = 0;
                grdResult.Rows.Clear();
            }

            private void btnSaveClose_Click(object sender, EventArgs e)
            {
                if (Save())
                   CloseForm();
            }

            void CloseForm()
            {
                if (_newRecordAdded)
                    this.DialogResult = DialogResult.OK;
                else
                    this.DialogResult = DialogResult.Cancel;
            }


            private void grdResult_CellLeave(object sender, DataGridViewCellEventArgs e)
            {
                Decimal totalBuffalo = 0.00m;
                Decimal totalCow = 0.00m;
                if (e.ColumnIndex == 6 || e.ColumnIndex == 7)
                {
                    foreach (DataGridViewRow row in grdResult.Rows)
                    {   
                        if (Convert.ToDecimal(row.Cells["Buffalo"].Value) > 0 || Convert.ToDecimal(row.Cells["Cow"].Value) > 0)
                        {
                            totalBuffalo += Convert.ToDecimal(row.Cells["Buffalo"].Value);
                            totalCow += Convert.ToDecimal(row.Cells["Cow"].Value);
                        }
                        lblTotalBuffalo.Text = Convert.ToString(totalBuffalo);
                        lblTotalCow.Text = Convert.ToString(totalCow);
                        lblTotal.Text = Convert.ToString(totalBuffalo + totalCow);
                    }
                }
            }

            private void grdResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
            {
                Decimal totalBuffalo = 0.00m;
                Decimal totalCow = 0.00m;
                if (e.ColumnIndex == 6 || e.ColumnIndex == 7)
                {
                    foreach (DataGridViewRow row in grdResult.Rows)
                    {
                        totalBuffalo += Convert.ToDecimal(row.Cells["Buffalo"].Value);
                        totalCow += Convert.ToDecimal(row.Cells["Cow"].Value);
                    }
                    lblTotalBuffalo.Text = Convert.ToString(totalBuffalo);
                    lblTotalCow.Text = Convert.ToString(totalCow);
                    lblTotal.Text = Convert.ToString(totalBuffalo + totalCow);
                }
            }

            private void lblHeading_Click(object sender, EventArgs e)
            {

            }

            private void btnClear_Click(object sender, EventArgs e)
            {
                cboLines.Enabled = true;
                grdResult.Rows.Clear();
            }
    }
}
