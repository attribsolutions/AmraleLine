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
    public partial class ctlRates : UserControl
    {
        LineManager _lineManager = new LineManager();
        RateManager _rateManager = new RateManager();

        bool _allowEdit = true;

        public ctlRates()
        {
            InitializeComponent();
            //cboSearchIn.SelectedIndex = 0;
            if (Program.UserRole == "Cashier")      //Hardcode
            {
                btnModify.Enabled = false;
                btnDelete.Enabled = false;

                _allowEdit = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            AddRate();
        }

        public bool SearchRates()
        {
            BindingList<RatesInfo> rates = new BindingList<RatesInfo>();
            try
            {
                rates = _rateManager.GetRatesByFilter(txtSearchText.Text, 1000);
                if (rates.Count > 0)
                {
                    grdResult.DataSource = rates;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = rates.Count.ToString() + " record(s) found.";
                rates = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting rates by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["IsLastUpdated"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
        }

        void AddRate()
        {
            frmRate frm = new frmRate();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchRates();

                //Set focus in grid at position, where the new record added.
                for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                {
                    if (frm.NewRecordID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                    {
                        grdResult.Rows[i].Selected = true;
                        grdResult.CurrentCell = grdResult[1, i];
                        break;
                    }
                }
            }
        }

        bool EditRate()
        {
            if (grdResult.SelectedRows.Count > 0 && _allowEdit)
            {
                RatesInfo rate = (RatesInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmRate frm = new frmRate(rate);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchRates();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (rate.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[1, i];
                            break;
                        }
                    }
                }
            }
            return true;
        }

        bool DeleteRate()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                string tableName = string.Empty;
                //bool isUsed = true;
                bool isUsed = false;
                //try
                //{
                //    isUsed = _lineManager.CheckLineUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Error in checking whether Line is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return false;
                //}
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _rateManager.DeleteRate(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchRates();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Line is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Line." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchRates();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditRate();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRate();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddRate();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditRate();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchRates();
            }
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearchText.Text = string.Empty;
        }

    }
}