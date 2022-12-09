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
    public partial class frmDispatch : Form
    {
        #region Class level variables...

        //ChallanManager _challanManager = new ChallanManager();
        ChallanInfo _challan = new ChallanInfo();
        BindingList<ChallanItemInfo> _challanItems = new BindingList<ChallanItemInfo>();
        ItemManager _itemManager = new ItemManager();        
        DivisionManager _divisionManager = new DivisionManager();
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        SettingManager _settingManager = new SettingManager();
        DispatchManager _dispatchManager = new DispatchManager();
        StockManager _stockManager = new StockManager();

        int _newRecordID = 0;

        bool _newRecordAdded = false;        

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        #endregion

        public frmDispatch()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Dispatch Entry";
        }        

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmDispatch_KeyPress(object sender, KeyPressEventArgs e)
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
         
        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void frmDispatch_Load(object sender, EventArgs e)
        {
            FillFromDivisions();
            FillToDivisions();
            SearchStock();
        }

        private void FillToDivisions()
        {
            try
            {
                BindingList<DivisionInfo> division = _divisionManager.GetAllDivision();
                cboToDivision.DataSource = division;
                cboToDivision.DisplayMember = "DivisionName";
                cboToDivision.ValueMember = "DivisionID";

                division = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting To Divisions." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchStock();
        }

        void FillFromDivisions()
        {
            try
            {
                BindingList<DivisionInfo> division = _divisionManager.GetAllDivision();
                cboFromDivision.DataSource = division;
                cboFromDivision.DisplayMember = "DivisionName";
                cboFromDivision.ValueMember = "DivisionID";

                division = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting From Divisions." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;

            dtToDate.Value = DateTime.Today.Date;

            grdItems.Rows.Clear();

            dtToDate.Focus();

            cboFromDivision.SelectedIndex = 0;
        }      

        private void SearchStock()
        {
            try
            {
                if (_stockManager.CheckDayExist(dtToDate.Value.Date, Convert.ToInt32(cboFromDivision.SelectedValue)))
                {
                    New();
                    List<StockInfo> stocks = _stockManager.GetByDate(dtToDate.Value.Date, null, Convert.ToInt32(cboFromDivision.SelectedValue));

                    if (stocks.Count > 0)
                    {
                        foreach (StockInfo stock in stocks)
                        {
                            int row = grdItems.Rows.Add();

                            grdItems.Rows[row].Cells["ID"].Value = stock.ID;
                            grdItems.Rows[row].Cells["ItemID"].Value = stock.ItemID;
                            grdItems.Rows[row].Cells["StockDate"].Value = stock.StockDate.Date;
                            grdItems.Rows[row].Cells["ItemName"].Value = stock.ItemName;
                            grdItems.Rows[row].Cells["StockQuantity"].Value = stock.Closing;                            
                        }
                        
                        DisableFields();
                        grdItems.Focus();
                    }
                    else
                    {
                        grdItems.DataSource = null;                        
                    }
                    stocks = null;
                }
                else
                {
                    MessageBox.Show("Todays stock not present. Please Generate It." + Environment.NewLine , Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in checking stock by date." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }            
        }

        private void DisableFields()
        {
            grdItems.Columns["ID"].Visible = false;            
            grdItems.Columns["ItemID"].Visible = false;            
        }

        private void grdItems_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CalculateRowsCount();
        }

        private void grdItems_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateRowsCount();
        }

        void CalculateRowsCount()
        {
            if (grdItems.Rows.Count > 0)
                grdItems.Columns["ItemName"].HeaderText = "Item Name - " + grdItems.Rows.Count.ToString();
            else
                grdItems.Columns["ItemName"].HeaderText = "Item Name";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }

        private void grdItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (grdItems.Columns[e.ColumnIndex].Name != "Quantity")
                return;

            try
            {
                if (e.FormattedValue.ToString().Trim() != string.Empty)
                {
                    decimal d;
                    if (!decimal.TryParse(e.FormattedValue.ToString(), out d))
                        e.Cancel = true;
                }
                else
                    grdItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void grdItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (grdItems.Columns[e.ColumnIndex].Name == "Quantity")
                grdItems.Rows[e.RowIndex].Cells["Quantity"].Value = Convert.ToDecimal(grdItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("0.000");
        }       

        private void btnDispatch_Click(object sender, EventArgs e)
        {

        }

        //bool ValidateFields()
        //{
        //    if (dtChallanDate.Value.Date > DateTime.Now.Date)
        //    {
        //        dtChallanDate.Focus();
        //        MessageBox.Show("Please enter valid challan date.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return false;
        //    } 

        //    return true;
        //}

        //ChallanInfo SetValue(out BindingList<ChallanItemInfo> challanItems)
        //{
        //    ChallanInfo retVal = new ChallanInfo();
        //    BindingList<ChallanItemInfo> challanItemss = new BindingList<ChallanItemInfo>();

        //    //retVal.ChallanDate = dtChallanDate.Value.Date;
        //    //retVal.ChallanNo = txtChallanNo.Text;
        //    //retVal.SupplierID = Convert.ToInt32(cboSupplier.SelectedValue);
        //    //retVal.ReceivedDate = dtReceivedDate.Value.Date;
        //    //retVal.VehicleNo = txtVehicleNo.Text;
        //    //retVal.DeliveredBy = txtDeliveredBy.Text;
        //    //retVal.ReceivedBy = Convert.ToInt32(cboReceivedBy.SelectedValue);
        //    //retVal.Description = txtDescription.Text;

        //    //if (Program.UserRole == "Administrator")
        //    //{
        //    //    retVal.DivisionID = Convert.ToInt16(cboDivision.SelectedValue);
        //    //}
        //    //else
        //    //{
        //    //    retVal.DivisionID = Program.DivisionID;
        //    //}
        //    //retVal.CreatedBy = Program.CURRENTUSER;
        //    //retVal.CreatedOn = DateTime.Now;
        //    //retVal.UpdatedBy = Program.CURRENTUSER;
        //    //retVal.UpdatedOn = DateTime.Now;

        //    //foreach (DataGridViewRow dr in grdItems.Rows)
        //    //{
        //    //    ChallanItemInfo challanItem = new ChallanItemInfo();
        //    //    challanItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
        //    //    challanItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
        //    //    challanItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                
        //    //    challanItemss.Add(challanItem);
        //    //}

        //    challanItems = challanItemss;
        //    return retVal;
        //}

        //bool Save()
        //{
        //    if (!ValidateFields())
        //        return false;

        //    BindingList<ChallanItemInfo> challanItems = null;
        //    ChallanInfo challan = SetValue(out challanItems);

        //    if (_viewMode == EnumClass.FormViewMode.Addmode)
        //    {
        //        try
        //        {
        //            _newRecordID = _challanManager.AddChallan(challan, challanItems);
        //            _newRecordAdded = true;
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error in saving Challan." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return false;
        //        }
        //    }

        //    if (_viewMode == EnumClass.FormViewMode.EditMode)
        //    {
        //        try
        //        {
        //            //challan.ID = Convert.ToInt32(txtChallanNo.Tag);
        //            _challanManager.UpdateChallan(challan, challanItems);
        //            _newRecordAdded = true;
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error in updating Challan." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return false;
        //        }
        //    }

        //    return false;
        //}        
    }
}