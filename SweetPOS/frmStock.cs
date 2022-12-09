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
    public partial class frmStock : Form
    {
        StockManager _stockManager = new StockManager();
        DivisionManager _divisionManager = new DivisionManager();
        Boolean IsDivisionFilled = false;

        public frmStock()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Stock";
        }

        private void frmStock_Load(object sender, EventArgs e)
        {
            try
            {
                if (_stockManager.CheckDayExist(dtStockDate.Value.Date,Program.DivisionID))
                    btnShowStock.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in checking stock by date." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Program.LoginName == "Admin")
            {
                cboDivision.Visible = true;
                FillDivisions();
            }
            else
            {
                cboDivision.Visible = false;
            }
            dtStockDate.Value = _stockManager.GetMaxDate(Convert.ToInt32(cboDivision.SelectedValue));
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
                IsDivisionFilled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting all Divisions." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            StockInfo stock = new StockInfo();
            List<ItemInfo> items = new List<ItemInfo>();

            int divisionID = 0;
            if (cboDivision.Visible == true)
            {
                divisionID = Convert.ToInt32(cboDivision.SelectedValue);
            }
            else
            {
                divisionID = Program.DivisionID;
            }

            int cont;
            int isAdj;
            DialogResult dr1 = DialogResult.No;
            DialogResult dr2 = DialogResult.No;
            try
            {
                cont = _stockManager.GetPreviousDay(dtStockDate.Value.Date, divisionID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting previous day stock." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                isAdj = Convert.ToInt32(_stockManager.IsAdjusted(dtStockDate.Value.Date, divisionID));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting adjusted stock." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cont == 0)
                dr1 = MessageBox.Show("Previous day record NOT FOUND, Please generate for previous day first OR Continue with ZERO(0) OPENING for above Date...?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            else
                dr1 = DialogResult.Yes;

            if (dr1 == DialogResult.Yes && isAdj > 0)
                dr2 = MessageBox.Show("Stock Adjusted Manually, Do you want to overwrite the adjusted stock...?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            else
                dr2 = DialogResult.Yes;            
           

            if (cont > 0 || dr1 == DialogResult.Yes)
            {
                if (dr2 == DialogResult.Yes)
                {
                    items = _stockManager.GetAllItems();

                    _stockManager.StockDelete(dtStockDate.Value.Date,divisionID);

                    foreach (ItemInfo newItem in items)
                    {
                        stock = _stockManager.Process(dtStockDate.Value.Date, newItem.ID, divisionID);
                        _stockManager.StockInsert(stock);
                    }
                    MessageBox.Show("Stock Generation Completed Successfully...", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnShowStock.Enabled = true;
                    ShowForSelectedDate(dtStockDate.Value.Date,divisionID);
                }
            }
        }

        void ShowForSelectedDate(DateTime stockDate,int DivisionID)
        {
            List<StockInfo> stocks = _stockManager.GetByDate(dtStockDate.Value.Date, string.Empty, DivisionID);

            grdItems.DataSource = stocks;

            grdItems.Columns["ID"].Visible = false;
            grdItems.Columns["StockDate"].Visible = false;
            grdItems.Columns["ItemID"].Visible = false;
            grdItems.Columns["DivisionID"].Visible = false;
            grdItems.Columns["CreatedBy"].Visible = false;
            grdItems.Columns["CreatedOn"].Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void frmStock_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dtStockDate_ValueChanged(object sender, EventArgs e)
        {
            btnShowStock.Enabled = false;
        }

        private void btnShowStock_Click(object sender, EventArgs e)
        {
            ReportClass rpt = new ReportClass();
            rpt.ShowStockReport(dtStockDate.Value.Date, dtStockDate.Value.Date,Program.DivisionID);
        }

        private void cboDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsDivisionFilled)
            { dtStockDate.Value = _stockManager.GetMaxDate(Convert.ToInt32(cboDivision.SelectedValue)); }
        }
    }
}