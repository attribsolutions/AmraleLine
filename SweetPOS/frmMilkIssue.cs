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
    public partial class frmMilkIssue : Form
    {
        MilkIssueManager _MilkIssueManager = new MilkIssueManager();
        MilkIssueInfo _milkIssueInfo = new MilkIssueInfo();
        MilkIssueItemInfo _milkIssueItemInfo = new MilkIssueItemInfo();
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        Boolean isLinemanFilled = false;

        public frmMilkIssue()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateFields();
                grdResult.Rows.Clear();
                ShowItemsforMilkIssueEntry();

            }
            catch (Exception Ex)
            {

            }
        }

        bool ShowItemsforMilkIssueEntry()
        {
            BindingList<MilkIssueItemInfo> MilkIssueItems = new BindingList<MilkIssueItemInfo>();
            try
            {
                MilkIssueItems = _MilkIssueManager.ShowItemsforMilkIssueEntry();
                //grdResult.DataSource = MilkIssueItems;
                foreach (MilkIssueItemInfo orderItem in MilkIssueItems)
                {
                    int row = grdResult.Rows.Add();
                    grdResult.Rows[row].Cells["ItemCode"].Value = orderItem.ItemCode;
                    grdResult.Rows[row].Cells["ItemName"].Value = orderItem.Name;
                    grdResult.Rows[row].Cells["Quantity"].Value = 0;
                    grdResult.Rows[row].Cells["UnitID"].Value = orderItem.UnitID;
                    grdResult.Rows[row].Cells["Unit"].Value = orderItem.Unit;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
              
            }
            MilkIssueItems = null;
            return true;
            
        }


        void DisableFields()
        {

            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["ItemCode"].Visible = false;
            grdResult.Columns["ItemGroupID"].Visible = false;
            grdResult.Columns["ItemGroup"].Visible = false;
            grdResult.Columns["Quantity"].Visible = false;
            grdResult.Columns["ToDate"].Visible = false;
            grdResult.Columns["FromDate"].Visible = false;
            grdResult.Columns["LineManID"].Visible = false;
            grdResult.Columns["MainBranchCode"].Visible = false;
            grdResult.Columns["BarCode"].Visible = false;
            grdResult.Columns["DisplayName"].Visible = false;
            grdResult.Columns["DisplayIndex"].Visible = false;
            grdResult.Columns["UnitID"].Visible = false;
            grdResult.Columns["Rate"].Visible = false;
            grdResult.Columns["LastPurchaseRate"].Visible = false;
            grdResult.Columns["Gst"].Visible = false;
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

            BindingList<MilkIssueItemInfo> MilkIssueItems = new BindingList<MilkIssueItemInfo>();
            LinemanInfo MilkIssue = SetValue(out MilkIssueItems);

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = Convert.ToInt32(_MilkIssueManager.AddMilkIssueToLineMan(MilkIssue, MilkIssueItems));
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Issue Milk." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;

        }
        bool ValidateFields()
        {
            if (cboLineman.Text.Trim() == string.Empty)
            {
                cboLineman.Focus();
                MessageBox.Show("Please enter lineman.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        LinemanInfo SetValue(out BindingList<MilkIssueItemInfo> MilkIssueItemss)
        {
            LinemanInfo retVal = new LinemanInfo();
            retVal.ID=Convert.ToInt32(cboLineman.SelectedValue);
            retVal.FromDate = Convert.ToDateTime(dtMilkIssueDate.Value).Date;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;
            
            BindingList<MilkIssueItemInfo> MilkIssueItems = new BindingList<MilkIssueItemInfo>();
            foreach (DataGridViewRow row in grdResult.Rows)
            {
                MilkIssueItemInfo items = new MilkIssueItemInfo();
                if (Convert.ToDecimal(row.Cells["Quantity"].Value) > 0)
                {
                    items.ItemCode = Convert.ToInt32(row.Cells["ItemCode"].Value);
                    items.Name = Convert.ToString(row.Cells["ItemName"].Value);
                    items.UnitID = Convert.ToInt32(row.Cells["UnitID"].Value);
                    items.Quantity = Convert.ToDecimal(row.Cells["Quantity"].Value);
                }
                MilkIssueItems.Add(items);
            }
            MilkIssueItemss = MilkIssueItems;
            return retVal;
        }

        bool FillLineMans()
        {
            LinemanManager _linemanManager = new LinemanManager();
            BindingList<LinemanInfo> linemans = new BindingList<LinemanInfo>();
            try
            {
                linemans = _linemanManager.GetLinemans();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item Groups." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            cboLineman.DataSource = linemans;
            cboLineman.DisplayMember = "Name";
            cboLineman.ValueMember = "ID";
            linemans = null;
            cboLineman.SelectedIndex = -1;
            isLinemanFilled = true;
            return true;
        }

        private void frmMilkIssue_Load(object sender, EventArgs e)
        {
            FillLineMans();
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";
            cboLineman.SelectedValue = 0;
            grdResult.DataSource =null;
        }
        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (Save())
                CloseForm();

        }
    }
}
