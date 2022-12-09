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
    public partial class frmRate : Form
    {
        #region Class level variables...

        //LineManager _lineManager = new LineManager();
        //LineInfo _lineInfo = new LineInfo();
        //LineInfo _line = new LineInfo();

        RateManager _rateManager = new RateManager();
        RatesInfo _rateInfo = new RatesInfo();
        Boolean IsItemFilled = false;
        
        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        int _newRecordID = 0;
        bool _newRecordAdded = false;
        
        #endregion

        public frmRate()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Rate Master";
        }

        public frmRate(RatesInfo rate)
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Rate Master - " + rate.ID.ToString();
            txtpreviousRate.Tag = rate.ID.ToString();
            _rateInfo = rate;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        public int NewRecordID
        { get { return _newRecordID; } set { _newRecordID = value; } }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmItem_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmRate_Load(object sender, EventArgs e)
        {
            FillItems();
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSaveNew.Text = "Update && Ne&w";
                btnSaveClose.Text = "Update && Cl&ose";

                cboItem.SelectedValue = _rateInfo.ItemID;
                txtpreviousRate.Text = txtNewRate.Text = _rateInfo.Rate.ToString("0.00");
                txtPreviousVat.Text = txtNewVat.Text = _rateInfo.VAT.ToString("0.00");
                //txtNewRate.Text = txtNewVat.Text = "0.00";
                
                txtNewRate.Focus();
            }
        }

        private void FillItems()
        {
            BindingList<ItemInfo> items = new BindingList<ItemInfo>();
            try
            {
                ItemManager _itemManager = new ItemManager();
                items = _itemManager.GetItemsAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cboItem.DataSource = items;
            cboItem.DisplayMember = "Name";
            cboItem.ValueMember = "ID";
            cboItem.SelectedIndex = -1;
            IsItemFilled = true;
            items = null;
        }

        private void AutoComplete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
        }

        bool ValidateFields()
        {
            return true;
        }

        RatesInfo SetValue()
        {
            RatesInfo retVal = new RatesInfo();

            retVal.EffectiveFrom = Convert.ToDateTime(dtSDate.Value).Date;
            retVal.ItemID = Convert.ToInt32(cboItem.SelectedValue);
            retVal.Rate = Convert.ToDecimal(txtNewRate.Text);
            retVal.VAT = Convert.ToDecimal(txtNewVat.Text);
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

            RatesInfo rate = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    //_newRecordID = _lineManager.AddLine(line);
                    _newRecordID = _rateManager.AddRate(rate);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Rate." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    _rateManager.AddRate(rate);
                    _newRecordAdded = true;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Rate." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtNewVat.Text = txtpreviousRate.Text = txtPreviousVat.Text = txtNewRate.Text = "0.00";
            cboItem.SelectedIndex = -1;
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

        private void Numeric_Validating(object sender, CancelEventArgs e)
        {
            if (((TextBox)sender).Text.Trim() != string.Empty)
            {
                decimal d = 0.0M;
                if (!decimal.TryParse(((TextBox)sender).Text, out d))
                    e.Cancel = true;
                else
                {
                    if (((TextBox)sender).Name == "txtNewVat" || ((TextBox)sender).Name == "txtNewRate" || ((TextBox)sender).Name == "txtPreviousVat" || ((TextBox)sender).Name == "txtPreviousRate")
                        ((TextBox)sender).Text = d.ToString("0.00");
                    else
                        ((TextBox)sender).Text = d.ToString("0");
                }
            }
            else
            {
                if (((TextBox)sender).Name == "txtNewVat" || ((TextBox)sender).Name == "txtNewRate" || ((TextBox)sender).Name == "txtPreviousVat" || ((TextBox)sender).Name == "txtPreviousRate")
                    ((TextBox)sender).Text = "0.00";
                else
                    ((TextBox)sender).Text = "0";
            }
        }

        private void string_Keypress(object sender, KeyPressEventArgs e)
        {
            // allows 0-9, backspace, and decimal
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private void cboItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsItemFilled)
            {
                RatesInfo rate = new RatesInfo();
                rate = _rateManager.GetRateByItemCode(Convert.ToInt32(cboItem.SelectedValue));
                txtpreviousRate.Text = txtNewRate.Text = rate.Rate.ToString("0.00");
                txtPreviousVat.Text = txtNewVat.Text = rate.VAT.ToString("0.00");
            }
        }


    }
}