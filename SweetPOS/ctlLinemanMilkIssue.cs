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
    public partial class ctlLinemanMilkIssue : UserControl
    {
        MilkIssueManager _milkIsssueManager = new MilkIssueManager();
        bool _allowEdit = true;

        public ctlLinemanMilkIssue()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

            AddItem();
        }

        private void AddItem()
        {
            frmMilkIssue frm = new frmMilkIssue();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchLineManMilkIssue();

                // i'll wriete here = Set focus in grid at position, where the new record added.


            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchLineManMilkIssue();
        }

        public bool SearchLineManMilkIssue()
        {
            BindingList<MilkIssueInfo> MilkIssue = new BindingList<MilkIssueInfo>();
            try
            {
                MilkIssue = _milkIsssueManager.GetLinemanByFilter(txtSearchText.Text, 10000);
                if (MilkIssue.Count > 0)
                {
                    grdResult.DataSource = MilkIssue;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = MilkIssue.Count.ToString() + " record(s) found.";
                MilkIssue = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Lineman by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        void DisableFields()
        {

            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["Mobile"].Visible = false;
            grdResult.Columns["Address"].Visible = false;
            grdResult.Columns["City"].Visible = false;
            grdResult.Columns["Commission"].Visible = false;
            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["IsActive"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
           
        }

        private void grdResult_DoubleClick(object sender, EventArgs e)
        {
        
          //  EditItem();
        
        }

        //bool EditItem()
        //{
        //    if (grdResult.SelectedRows.Count > 0 && _allowEdit)
        //    {
        //        MilkIssueInfo MilkIssueInfo = (MilkIssueInfo)grdResult.SelectedRows[0].DataBoundItem;
        //        MilkIssueItemInfo MilkIssueitems=new MilkIssueItemInfo();
        //        //=(MilkIssueItemInfo)grdResult.SelectedRows[0].DataBoundItem;

        //         frmMilkIssue frm = new frmMilkIssue(LinmenorderNumber);
        //        if (frm.ShowDialog() == DialogResult.OK)
        //        {
        //            SearchLineManMilkIssue();

        //            //Set focus in grid at position, where it was previously before edit.
        //            for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
        //            {
        //                if (MilkIssueitems.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
        //                {
        //                    grdResult.Rows[i].Selected = true;
        //                    grdResult.CurrentCell = grdResult[1, i];
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}

        private void btnModify_Click(object sender, EventArgs e)
        {
         //EditItem();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }


        bool DeleteItem()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                try
                {
                  
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _milkIsssueManager.DeleteMilkIssue(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchLineManMilkIssue();
                            return true;
                        }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Milk Issue ." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }
        
    }
}
