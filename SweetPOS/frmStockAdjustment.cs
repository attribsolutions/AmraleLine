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
    public partial class frmStockAdjustment : Form
    {
        #region Class level variables...

        StockAdjustmentManager _stockAdjustmentManager = new StockAdjustmentManager();
        StockAdjustmentInfo _stockAdjustment = new StockAdjustmentInfo();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        int _divisionID = 0;

        #endregion

        public frmStockAdjustment(StockAdjustmentInfo stockAdjustment, EnumClass.FormViewMode viewMode)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Stock Adjustment";
            _stockAdjustment = stockAdjustment;
            _viewMode = viewMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmStockAdjustment_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmStockAdjustment_Load(object sender, EventArgs e)
        {
            dtAdjustmentDate.Value = _stockAdjustment.AdjustmentDate;
            txtItemName.Tag = _stockAdjustment.ItemID;
            txtItemName.Text = _stockAdjustment.Description;
            txtSystemQuantity.Text = _stockAdjustment.SystemQty.ToString("0.000");
            _divisionID = _stockAdjustment.DivisionID;

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                StockAdjustmentInfo stockAdj = _stockAdjustmentManager.GetStockAdjustment(_stockAdjustment.AdjustmentDate, _stockAdjustment.ItemID, _stockAdjustment.DivisionID);

                dtAdjustmentDate.Tag = stockAdj.ID;
                txtSystemQuantity.Text = stockAdj.SystemQty.ToString("0.000");
                txtAdjustedQuantity.Text = stockAdj.AdjustedQty.ToString("0.000");
                txtDescription.Text = stockAdj.Description;

                //txtAdjustedQuantity.Text = _stockAdjustment.SystemQty.ToString("0.000");

                txtAdjustedQuantity.Focus();
            }
        }

        bool ValidateFields()
        {
            decimal d;
            if (!decimal.TryParse(txtAdjustedQuantity.Text, out d))
            {
                txtAdjustedQuantity.Focus();
                MessageBox.Show("Please enter valid Stock Adjustment Quantity.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        StockAdjustmentInfo SetValue()
        {
            StockAdjustmentInfo retVal = new StockAdjustmentInfo();

            retVal.AdjustmentDate = dtAdjustmentDate.Value.Date;
            retVal.ItemID = (int)txtItemName.Tag;
            retVal.SystemQty = Convert.ToDecimal(txtSystemQuantity.Text);
            retVal.AdjustedQty = Convert.ToDecimal(txtAdjustedQuantity.Text);
            retVal.Description = txtDescription.Text;
            retVal.DivisionID = _divisionID;

            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            return retVal;
        }

        bool Save()
        {
            if (!ValidateFields())
                return false;

            StockAdjustmentInfo stockAdjustment = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _newRecordID = _stockAdjustmentManager.AddStockAdjustment(stockAdjustment);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Stock Adjustment." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    stockAdjustment.ID = Convert.ToInt32(dtAdjustmentDate.Tag);
                    _stockAdjustmentManager.UpdateStockAdjustment(stockAdjustment);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Stock Adjustment." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return false;
        }

        void New()
        {
            _viewMode = EnumClass.FormViewMode.Addmode;
            btnSaveNew.Text = "Save && Ne&w";
            btnSaveClose.Text = "Save && Cl&ose";

            dtAdjustmentDate.Value = DateTime.Today.Date;

            txtDescription.Text = string.Empty;

            txtAdjustedQuantity.Text = "0.000";

            dtAdjustmentDate.Focus();
        }

        void CloseForm()
        {
            if (_newRecordAdded)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (Save())
                New();
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (Save())
                CloseForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
        }
    }
}