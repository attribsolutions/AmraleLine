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
    public partial class frmLineChange: Form
    {
        HomeDeliveryMilkManagaer _HomeDeliveryMilkManagaer = new HomeDeliveryMilkManagaer();
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        HomeDeliveryMilkInfo _homedeliverymilkinfo = new HomeDeliveryMilkInfo();
        CustomerManager _customerManager = new CustomerManager();
        CustomerInfo _customer = new CustomerInfo();
        BindingList<HomeDeliveryMilkInfo> _saleItems = new BindingList<HomeDeliveryMilkInfo>();
        BindingList<CustomerItemsInfo> _custometItemsInfo = new BindingList<CustomerItemsInfo>();

        int _newRecordID = 0;
        bool _newRecordAdded = false;

        public frmLineChange()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Customer Line Change";
        }

        public frmLineChange(HomeDeliveryMilkInfo homedeliverymilkinfo)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Customer Line Change - " + homedeliverymilkinfo.ID.ToString();
            _homedeliverymilkinfo = homedeliverymilkinfo;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frmLineChange_Load(object sender, EventArgs e)
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
                MessageBox.Show("Error in getting Lines ." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            grdResult.Rows.Clear();
            ShowCustomerForLineChange();
            
        }
            bool ShowCustomerForLineChange()
            {
                BindingList<CustomerInfo> customerInfo = new BindingList<CustomerInfo>();
                try
                {
                   // CustomerInfo customerInfo = new CustomerInfo();
                    customerInfo = _customerManager.GetCustomersByLineID(Convert.ToInt32(cboLines.SelectedValue));

                    foreach (CustomerInfo customers in customerInfo)
                    {
                        int row = grdResult.Rows.Add();
                        grdResult.Rows[row].Cells["CustomerName"].Value = customers.Name;
                        grdResult.Rows[row].Cells["CustomerNumber"].Value = customers.CustomerNumber;
                        grdResult.Rows[row].Cells["CustomerID"].Value = customers.ID;
                       // grdResult.Rows[row].Cells["LineNumber"].Value = customers.LineID;
                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting Customers." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                customerInfo = null;
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

                BindingList<CustomerInfo> customers = new BindingList<CustomerInfo>();
                CustomerInfo customer = SetValue(out customers);

                if (_viewMode == EnumClass.FormViewMode.Addmode)
                {
                    try
                    {
                        _customerManager.UpdateCustomerForLineChange(customer, customers);
                        _newRecordAdded = true;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in saving  customer line Number" + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return false;

            }

            private bool ValidateFields()
            {
                foreach (DataGridViewRow row in grdResult.Rows)
                {
                    if (Convert.ToDecimal(row.Cells["CustomerNumber"].Value) ==
                         Convert.ToDecimal(row.Cells["NewCustomerNumber"].Value))
                    {
                        MessageBox.Show("Customer Number Already in Grid");
                    }

                }
                return true;
            }

            CustomerInfo SetValue(out BindingList<CustomerInfo> customers)
            {
                CustomerInfo retVal = new CustomerInfo();
                retVal.CreatedBy = Program.CURRENTUSER;
                retVal.CreatedOn = DateTime.Now;
                retVal.UpdatedBy = Program.CURRENTUSER;
                retVal.UpdatedOn = DateTime.Now;

                BindingList<CustomerInfo> Customers = new BindingList<CustomerInfo>();
                foreach (DataGridViewRow row in grdResult.Rows)
                {
                    if (Convert.ToInt32(row.Cells["NewCustomerNumber"].Value) > 0)
                    {
                        CustomerInfo items = new CustomerInfo();
                        items.ID = Convert.ToInt32(row.Cells["CustomerID"].Value);
                        items.Name = Convert.ToString(row.Cells["CustomerName"].Value);
                        items.CustomerNumber = Convert.ToInt32(row.Cells["CustomerNumber"].Value);
                        items.NewCustomerNumber = Convert.ToInt32(row.Cells["NewCustomerNumber"].Value);
                        // items.LineID = Convert.ToInt32(cboLines.SelectedValue);
                        Customers.Add(items);
                    }
                }
                customers = Customers;
                return retVal;
            }

            //bool ValidateFields()
            //{
            //    if (cboLines.Text.Trim() == string.Empty)
            //    {
            //        cboLines.Focus();
            //        MessageBox.Show("Please Select Line.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return false;
            //    }
            //    return true;
            //}

            //CustomerInfo SetValue()
            //{
            //    CustomerInfo retVal = new CustomerInfo();
            //    retVal.CustomerNumber = Convert.ToInt16(txtCustomerNumber.Text);
            //    retVal.Name = txtName.Text;
            //    retVal.CreatedBy = Program.CURRENTUSER;
            //    retVal.CreatedOn = DateTime.Now;
            //    retVal.UpdatedBy = Program.CURRENTUSER;
            //    retVal.UpdatedOn = DateTime.Now;
            //    return retVal;
            //}

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
                if (Save ())
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
                   
                }
            }
    }
}
