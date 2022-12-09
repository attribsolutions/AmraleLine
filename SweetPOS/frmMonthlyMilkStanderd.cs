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
    public partial class frmMonthlyMilkStanderd: Form
    {
        MonthlyMilkStanderdManagaer _MonthlyMilkStanderdManagaer = new MonthlyMilkStanderdManagaer();
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        MonthlyMilkStanderdInfo _homedeliverymilkinfo = new MonthlyMilkStanderdInfo();
        BindingList<MonthlyMilkStanderdInfo> _saleItems = new BindingList<MonthlyMilkStanderdInfo>();
        int _newRecordID = 0;
        bool _newRecordAdded = false;

        public frmMonthlyMilkStanderd()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Monthaly Milk Standerd - ";
        }

        public frmMonthlyMilkStanderd(MonthlyMilkStanderdInfo homedeliverymilkinfo)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " -  Monthaly Milk Standerd - " + homedeliverymilkinfo.ID.ToString();
            //txtLinemanName.Tag = homedeliverymilkinfo.ID.ToString();
            _homedeliverymilkinfo = homedeliverymilkinfo;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmHomeDeliveryMilk_Load(object sender, EventArgs e)
        {
            this.grdResult.DefaultCellStyle.Font = new Font("Tahoma", 13);
            this.dtMilkDeliveryDate.Enabled = false;
            FillLines();
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";
                cboLines.SelectedValue = _homedeliverymilkinfo.ID;
                dtMilkDeliveryDate.Value = _homedeliverymilkinfo.MilkDeliveryDate;
                foreach (MonthlyMilkStanderdInfo saleItem in _saleItems)
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
                MessageBox.Show("Error in getting Line Names." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ShowCustomerForDomedeliveryMilkEntry();
        }

            bool ShowCustomerForDomedeliveryMilkEntry()
            {
                BindingList<MonthlyMilkStanderdInfo> homedeliverymilkinfo = new BindingList<MonthlyMilkStanderdInfo>();
                try
                {
                    MonthlyMilkStanderdInfo homedeliverycustomerinfo = new MonthlyMilkStanderdInfo();
                    homedeliverymilkinfo = _MonthlyMilkStanderdManagaer.ShowCustomerForHomedeliveryMilkEntry(Convert.ToInt32(cboLines.SelectedValue));

                    foreach (MonthlyMilkStanderdInfo orderItem in homedeliverymilkinfo)
                    {
                        int row = grdResult.Rows.Add();
                        grdResult.Rows[row].Cells["CustomerName"].Value = orderItem.Name;
                        grdResult.Rows[row].Cells["CustomerNumber"].Value = orderItem.CustomerNumber;
                        grdResult.Rows[row].Cells["CustomerID"].Value = orderItem.ID;
                        grdResult.Rows[row].Cells["Buffalo"].Value = orderItem.Buffalo;
                        grdResult.Rows[row].Cells["Cow"].Value = orderItem.Cow;
                        
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
                if (!ValidateFields())
                    return true;

                BindingList<MonthlyMilkStanderdInfo> HomeDeliveryMilkInfo = new BindingList<MonthlyMilkStanderdInfo>();
                MonthlyMilkStanderdInfo HomeDeliveryMilkIssue = SetValue(out HomeDeliveryMilkInfo);
                if (!CheckTodaysMonthlyStanderedEntryExist())
                {
                    if (_viewMode == EnumClass.FormViewMode.Addmode)
                    {
                        try
                        {
                            _newRecordID = Convert.ToInt32(_MonthlyMilkStanderdManagaer.HomedeliveryMilkByLineman(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo));
                            _newRecordAdded = true;
                            MessageBox.Show("Monthly Milk Standerd Saved. " + Environment.NewLine + Environment.NewLine, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.None);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error in saving  Monthly Milk Standerd Saved " + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        _newRecordID = Convert.ToInt32(_MonthlyMilkStanderdManagaer.UpdateMonthlyStandered(HomeDeliveryMilkIssue, HomeDeliveryMilkInfo));
                        _newRecordAdded = true;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in Updating  Home delivery Milk " + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return false;
            }
            private bool CheckTodaysMonthlyStanderedEntryExist()
            {
                bool retVal;
                int exist;
                try
                {
                    exist = _MonthlyMilkStanderdManagaer.CheckTodaysMonthlyStanderedEntryExist(dtMilkDeliveryDate.Value.Date, Convert.ToInt32(cboLines.SelectedValue));
                    if (exist > 0)
                        retVal = true;
                    else
                        retVal = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking todays sale entry." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", "asdf", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return retVal;
            }
            MonthlyMilkStanderdInfo SetValue(out BindingList<MonthlyMilkStanderdInfo> HomeDeliveryMilkInfo)
            {
                MonthlyMilkStanderdInfo retVal = new MonthlyMilkStanderdInfo();
                retVal.MilkDeliveryDate = Convert.ToDateTime(dtMilkDeliveryDate.Value).Date;
                retVal.LineID = Convert.ToInt32(cboLines.SelectedValue);
                retVal.CreatedBy = Program.CURRENTUSER;
                retVal.CreatedOn = DateTime.Now;
                retVal.UpdatedBy = Program.CURRENTUSER;
                retVal.UpdatedOn = DateTime.Now;

                BindingList<MonthlyMilkStanderdInfo> MilkIssue = new BindingList<MonthlyMilkStanderdInfo>();
                foreach (DataGridViewRow row in grdResult.Rows)
                {
                    MonthlyMilkStanderdInfo items = new MonthlyMilkStanderdInfo();
                    if (Convert.ToDecimal(row.Cells["Buffalo"].Value) >= 0 || Convert.ToDecimal(row.Cells["Cow"].Value) >= 0)
                    {
                        items.ID = Convert.ToInt32(row.Cells["CustomerID"].Value);
                        items.Name = Convert.ToString(row.Cells["CustomerName"].Value);
                        items.Buffalo = Convert.ToDecimal(row.Cells["Buffalo"].Value);
                        items.Cow = Convert.ToDecimal(row.Cells["Cow"].Value);
                        
                    }
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
                    MessageBox.Show("Please Select Lineman.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            private void grdResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
            {
                decimal TotalBuffalo = 0.00m;
                decimal TotalCow = 0.00m;
                foreach (DataGridViewRow row in grdResult.Rows)
                {
                  
                    if (Convert.ToDecimal(row.Cells["Buffalo"].Value) >= 0 || Convert.ToDecimal(row.Cells["Cow"].Value) >= 0)
                    {

                        TotalBuffalo += Convert.ToDecimal(row.Cells["Buffalo"].Value);
                        TotalCow += Convert.ToDecimal(row.Cells["Cow"].Value);

                    }
                    lblTotalBuffalo.Text = Convert.ToString(TotalBuffalo);
                    lblTotalCow.Text = Convert.ToString(TotalCow);
                    lblTotal.Text = Convert.ToString(TotalBuffalo + TotalCow);
                }
 
            }

            private void btnClear_Click(object sender, EventArgs e)
            {
                cboLines.Enabled = true;
                grdResult.Rows.Clear();
            }

    }
}
