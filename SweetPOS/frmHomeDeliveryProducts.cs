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
    public partial class frmHomeDeliveryProducts: Form
    {
        HomeDeliveryMilkManagaer _HomeDeliveryMilkManagaer = new HomeDeliveryMilkManagaer();
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        HomeDeliveryMilkInfo _homedeliverymilkinfo = new HomeDeliveryMilkInfo();
        BindingList<HomeDeliveryMilkInfo> _saleItems = new BindingList<HomeDeliveryMilkInfo>();
        int _newRecordID = 0;
        bool _newRecordAdded = false;

        public frmHomeDeliveryProducts()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Home Delivery Milk";
        }

        public frmHomeDeliveryProducts(HomeDeliveryMilkInfo homedeliverymilkinfo)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Home Delivery Milk - " + homedeliverymilkinfo.ID.ToString();
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
            FillLines();

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                cboLineman.SelectedValue = _homedeliverymilkinfo.ID;
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
                MessageBox.Show("Error in getting Item Groups." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboLineman.DataSource = lines;
            cboLineman.DisplayMember = "ID";
            cboLineman.ValueMember = "ID";
            lines = null;
            cboLineman.SelectedIndex = -1;
            return true;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            grdResult.Rows.Clear();
            ShowCustomerForDomedeliveryMilkEntry(); 
        }

            bool ShowCustomerForDomedeliveryMilkEntry()
            {
                BindingList<HomeDeliveryMilkInfo> homedeliverymilkinfo = new BindingList<HomeDeliveryMilkInfo>();
                try
                {
                    HomeDeliveryMilkInfo homedeliverycustomerinfo = new HomeDeliveryMilkInfo();
                    homedeliverymilkinfo = _HomeDeliveryMilkManagaer.ShowCustomerForHomedeliveryMilkEntry(Convert.ToInt32(cboLineman.SelectedValue),Convert.ToDateTime(dtMilkDeliveryDate.Value),true);
                    foreach (HomeDeliveryMilkInfo orderItem in homedeliverymilkinfo)
                    {
                        int row = grdResult.Rows.Add();
                        grdResult.Rows[row].Cells["CustomerName"].Value = orderItem.Name;
                        grdResult.Rows[row].Cells["CustomerNumber"].Value = orderItem.CustomerNumber;
                        grdResult.Rows[row].Cells["CustomerID"].Value = orderItem.ID;
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

                BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo = new BindingList<HomeDeliveryMilkInfo>();
                HomeDeliveryMilkInfo HomeDeliveryMilkIssue = SetValue(out HomeDeliveryMilkInfo);

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
                return false;

            }

            HomeDeliveryMilkInfo SetValue(out BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkInfo)
            {
                HomeDeliveryMilkInfo retVal = new HomeDeliveryMilkInfo();
                retVal.MilkDeliveryDate = Convert.ToDateTime(dtMilkDeliveryDate.Value).Date;
                retVal.CreatedBy = Program.CURRENTUSER;
                retVal.CreatedOn = DateTime.Now;
                //retVal.UpdatedBy = Program.CURRENTUSER;
                //retVal.UpdatedOn = DateTime.Now;
                BindingList<HomeDeliveryMilkInfo> MilkIssue = new BindingList<HomeDeliveryMilkInfo>();
                foreach (DataGridViewRow row in grdResult.Rows)
                {
                    HomeDeliveryMilkInfo items = new HomeDeliveryMilkInfo();
                    if (Convert.ToDecimal(row.Cells["Buffalo"].Value) > 0 || Convert.ToDecimal(row.Cells["Cow"].Value) > 0)
                    {
                        items.ID = Convert.ToInt32(row.Cells["CustomerID"].Value);
                        items.Name = Convert.ToString(row.Cells["CustomerName"].Value);
                        items.Buffalo = Convert.ToInt32(row.Cells["Buffalo"].Value);
                        items.Cow = Convert.ToDecimal(row.Cells["Cow"].Value);
                    }
                    MilkIssue.Add(items);
                }
                HomeDeliveryMilkInfo = MilkIssue;
                return retVal;
            }

            bool ValidateFields()
            {
                if (cboLineman.Text.Trim() == string.Empty)
                {
                    cboLineman.Focus();
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
                cboLineman.SelectedValue = 0;
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
    }
}
