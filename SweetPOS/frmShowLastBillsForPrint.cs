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
    public partial class frmShowLastBillsForPrint : Form
    {
        #region Class level variables...

        SaleManager _saleManager = new SaleManager();

        #endregion

        public frmShowLastBillsForPrint()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmChallan_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmChallan_Load(object sender, EventArgs e)
        {
            BindingList<SaleInfo> sales = new BindingList<SaleInfo>();
            try
            {
                sales = _saleManager.GetSalesByFilterForPrintCardDetails();

                if (sales.Count > 0)
                {
                    grdResult.Rows.Clear();

                    foreach (SaleInfo sale in sales)
                    {
                        int row = grdResult.Rows.Add();

                        grdResult.Rows[row].Cells["CustomerName"].Value = sale.CustomerName;
                        grdResult.Rows[row].Cells["BillID"].Value = sale.ID;
                        grdResult.Rows[row].Cells["BillNumber"].Value = sale.BillNo;
                        grdResult.Rows[row].Cells["BillDate"].Value = sale.BillDate;
                        grdResult.Rows[row].Cells["Amount"].Value = sale.RoundedAmount;
                        grdResult.Rows[row].Cells["User"].Value = sale.UserName;
                        grdResult.Rows[row].Cells["Items"].Value = "(" + Convert.ToString(sale.UpdatedBy) + ") " + sale.Description;
                        grdResult.Rows[row].Cells["IsPrinted"].Value = sale.IsPrint;

                        if (sale.IsPrint)
                            grdResult.Rows[row].DefaultCellStyle.ForeColor = grdResult.Rows[row].DefaultCellStyle.SelectionForeColor = Color.Brown;
                    }

                    grdResult.Rows[0].Selected = true;
                }
                sales = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Sale by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void CloseForm()
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                try
                {
                    ReportClass rpt = new ReportClass();
                    if (Program.SHOWSHORTBILL)
                    {
                        rpt.ShowBillChitale(Convert.ToInt32(grdResult.SelectedRows[0].Cells["BillNumber"].Value), false, false);
                    }
                    else
                    {
                        rpt.ShowBill(Convert.ToInt32(grdResult.SelectedRows[0].Cells["BillNumber"].Value), false, false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in printing bill." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    if (!Convert.ToBoolean(grdResult.SelectedRows[0].Cells["IsPrinted"].Value))
                    {
                        _saleManager.UpdateSalePrint(Convert.ToInt32(grdResult.SelectedRows[0].Cells["BillNumber"].Value));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating bill printed." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
        }
    }
}