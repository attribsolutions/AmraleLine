using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;

namespace SweetPOS
{
    public partial class ctlStock : UserControl
    {
        StockManager _stockManager = new StockManager();
        StockAdjustmentManager _stockAdjManager = new StockAdjustmentManager();
        DivisionManager _divisionManager = new DivisionManager();

        public ctlStock()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddStockAdjustment();
        }

        public bool SearchStockAdjustment()
        {
            try
            {
                List<StockInfo> stocks = _stockManager.GetByDate(dtStockDate.Value.Date, txtSearchText.Text,Convert.ToInt32(cboDivision.SelectedValue));

                if (stocks.Count > 0)
                {
                    grdResult.DataSource = stocks;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = stocks.Count.ToString() + " record(s) found.";
                stocks = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Stock Adjustments." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;

            grdResult.Columns["ItemID"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
        }

        void AddStockAdjustment()
        {
            StockAdjustmentInfo stockAdj = new StockAdjustmentInfo();
            stockAdj.ID = Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value);
            stockAdj.ItemID = Convert.ToInt32(grdResult.SelectedRows[0].Cells["ItemId"].Value);
            stockAdj.AdjustmentDate = Convert.ToDateTime(grdResult.SelectedRows[0].Cells["StockDate"].Value);
            stockAdj.Description = grdResult.SelectedRows[0].Cells["ItemName"].Value.ToString();    //Temp used as itemname
            stockAdj.SystemQty = Convert.ToInt32(grdResult.SelectedRows[0].Cells["Closing"].Value);
            stockAdj.DivisionID = Convert.ToInt32(grdResult.SelectedRows[0].Cells["DivisionID"].Value);

            EnumClass.FormViewMode viewMode =  EnumClass.FormViewMode.Addmode;
            if (Convert.ToBoolean(grdResult.SelectedRows[0].Cells["Adjusted"].Value))
                viewMode = EnumClass.FormViewMode.EditMode;
            else
                viewMode = EnumClass.FormViewMode.Addmode;
            frmStockAdjustment frm = new frmStockAdjustment(stockAdj, viewMode);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchStockAdjustment();

                //Set focus in grid at position, where the new record added.
                for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                {
                    if (frm.NewRecordID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                    {
                        grdResult.Rows[i].Selected = true;
                        grdResult.CurrentCell = grdResult[3, i];
                        break;
                    }
                }
            }
        }

        bool EditStockAdjustment()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                StockAdjustmentInfo stockAdj = new StockAdjustmentInfo();
                stockAdj.ID = Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value);
                stockAdj.ItemID = Convert.ToInt32(grdResult.SelectedRows[0].Cells["ItemId"].Value);
                stockAdj.AdjustmentDate = Convert.ToDateTime(grdResult.SelectedRows[0].Cells["StockDate"].Value);
                stockAdj.Description = grdResult.SelectedRows[0].Cells["ItemName"].Value.ToString();    //Temp used as itemname
                stockAdj.SystemQty = Convert.ToDecimal(grdResult.SelectedRows[0].Cells["Closing"].Value);
                stockAdj.DivisionID = Convert.ToInt32(grdResult.SelectedRows[0].Cells["DivisionID"].Value);


                EnumClass.FormViewMode viewMode = EnumClass.FormViewMode.Addmode;
                if (Convert.ToBoolean(grdResult.SelectedRows[0].Cells["Adjusted"].Value))
                    viewMode = EnumClass.FormViewMode.EditMode;
                else
                    viewMode = EnumClass.FormViewMode.Addmode;
                frmStockAdjustment frm = new frmStockAdjustment(stockAdj, viewMode);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchStockAdjustment();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (stockAdj.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[3, i];
                            break;
                        }
                    }
                }
            }
            return true;
        }

        bool DeleteStockAdjustment()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                if (!Convert.ToBoolean(grdResult.SelectedRows[0].Cells["Adjusted"].Value))
                {
                    MessageBox.Show("Entry not found, cannot delete.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                try
                {
                    DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        int stockId = Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value);
                        int divisionId = Convert.ToInt32(grdResult.SelectedRows[0].Cells["DivisionID"].Value);

                        StockAdjustmentInfo stockAdj = _stockAdjManager.GetStockAdjustment(Convert.ToDateTime(grdResult.SelectedRows[0].Cells["StockDate"].Value), Convert.ToInt32(grdResult.SelectedRows[0].Cells["ItemID"].Value), divisionId);

                        _stockAdjManager.DeleteStockAdjustment(stockAdj.ID);
                        
                        SearchStockAdjustment();
                        //Set focus in grid at position, where it was previously before edit.
                        for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                        {
                            if (stockId == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                            {
                                grdResult.Rows[i].Selected = true;
                                grdResult.CurrentCell = grdResult[3, i];
                                break;
                            }
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Stock Adjustment." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchStockAdjustment();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditStockAdjustment();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditStockAdjustment();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteStockAdjustment();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchStockAdjustment();
            }
        }

        private void ctlStock_Load(object sender, EventArgs e)
        {
            FillDivisions();
            dtStockDate.Value = _stockManager.GetMaxDate(Convert.ToInt32(Program.DivisionID));
        }

        private void lblRecordsFound_Click(object sender, EventArgs e)
        {

        }

        void FillDivisions()
        {
            try
            {
                BindingList<DivisionInfo> division = _divisionManager.GetAllDivision();
                cboDivision.DataSource = division;
                cboDivision.DisplayMember = "DivisionName";
                cboDivision.ValueMember = "DivisionID";

                division = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting all Divisions." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}