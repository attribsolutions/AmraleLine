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
    public partial class ctlHomeDeliveryMilk : UserControl
         
    {
           HomeDeliveryMilkManagaer _HomeDeliveryMilkManagaer = new HomeDeliveryMilkManagaer();
           EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
           int _newRecordID = 0;
           bool _newRecordAdded = false;
           bool _allowEdit = true;

        public ctlHomeDeliveryMilk()
        {
            InitializeComponent();
        }
        private void AddNewDelivery()
        {
            frmHomeDeliveryMilk frm = new frmHomeDeliveryMilk();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchMilkDeliveryEnteries();
                // i'll wriete here = Set focus in grid at position, where the new record added.
                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchMilkDeliveryEnteries();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddNewDelivery();

        }
        public bool SearchMilkDeliveryEnteries()
        {
            BindingList<HomeDeliveryMilkInfo> milkdeliveries = new BindingList<HomeDeliveryMilkInfo>();
            try
            {
                milkdeliveries = _HomeDeliveryMilkManagaer.SearchMilkDeliveryEnteries(txtSearchText.Text, 10000);
                if (milkdeliveries.Count > 0)
                {
                    grdResult.DataSource = milkdeliveries;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = milkdeliveries.Count.ToString() + " record(s) found.";
                milkdeliveries = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting milkDelivery by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["LinemanID"].Visible = false;
            grdResult.Columns["CustomerNumber"].Visible = false;
            grdResult.Columns["ItemID"].Visible = false;
            grdResult.Columns["ItemCode"].Visible = false;
            grdResult.Columns["Quantity"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
            grdResult.Columns["ItemName"].Visible = false;
            grdResult.Columns["LineID"].Visible = false;
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteDelivery();
        }
        bool DeleteDelivery()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                try
                {
                    DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        _HomeDeliveryMilkManagaer.DeleteDelivery(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                        SearchMilkDeliveryEnteries();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Milk Delivery." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void ctlHomeDeliveryMilk_Load(object sender, EventArgs e)
        {
            SearchMilkDeliveryEnteries();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditDeliveryMilk();
        }

        bool EditDeliveryMilk()
        {
            if (grdResult.SelectedRows.Count > 0 && _allowEdit)
            {
                HomeDeliveryMilkInfo homedeliverymilkinfo = (HomeDeliveryMilkInfo)grdResult.SelectedRows[0].DataBoundItem;
                //BindingList<HomeDeliveryMilkInfo> HomeDeliveryMilkItems = GetSaleItemsBySaleId(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));
                frmHomeDeliveryMilk frm = new frmHomeDeliveryMilk(homedeliverymilkinfo);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchMilkDeliveryEnteries();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (homedeliverymilkinfo.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
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
    }
}
