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
    public partial class frmCounter : Form
    {
        CounterManager _counterManager = new CounterManager();
        CounterInfo _counter = new CounterInfo();

        EnumClass.FormViewMode _viewMode = EnumClass.FormViewMode.Addmode;
        bool _changesMade = false;

        public frmCounter()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Counter Master"; ;
        }

        public frmCounter(CounterInfo counter)
        {
            InitializeComponent();
            _counter = counter;
            _viewMode = EnumClass.FormViewMode.EditMode;
        }

        private void frmCounter_Load(object sender, EventArgs e)
        {
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                btnSave.Text = "&Update";
                txtCounterName.Tag = _counter.ID;
                txtCounterName.Text = _counter.Name;
                txtPassword.Text = _counter.Password;
            }
        }

        void ClearFields()
        {
            txtCounterName.Text = string.Empty;
            txtPassword.Text = string.Empty;

            txtCounterName.Focus();
        }

        bool ValidateFields()
        {
            if (txtCounterName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Counter name should not be empty.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCounterName.Focus();
                return false;
            }

            if (txtPassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("Password should not be empty.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    int cnt = _counterManager.GetSameNameCount(txtCounterName.Text);
                    if (cnt > 0)
                    {
                        MessageBox.Show("Counter name already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCounterName.Focus();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting same name count." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    int cnt = _counterManager.GetSameNameCountForEditMode(txtCounterName.Text, Convert.ToInt32(txtCounterName.Tag));
                    if (cnt > 0)
                    {
                        MessageBox.Show("Counter name already exist.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCounterName.Focus();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting same name count." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        CounterInfo SetValue()
        {
            CounterInfo retVal = new CounterInfo();

            retVal.Name = txtCounterName.Text;
            retVal.Password = txtPassword.Text;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            return retVal;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            CounterInfo counter = SetValue();

            if (_viewMode == EnumClass.FormViewMode.Addmode)
            {
                try
                {
                    _counterManager.AddCounter(counter);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in saving Counter." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Counter saved successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                _changesMade = true;
            }
            if (_viewMode == EnumClass.FormViewMode.EditMode)
            {
                try
                {
                    counter.ID = Convert.ToInt32(txtCounterName.Tag);
                    counter.OldName = _counter.Name;

                    _counterManager.UpdateCounter(counter);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating Counter." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Counter updated successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }

        void CloseForm()
        {
            if (_changesMade)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void lblClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CloseForm();
        }

        private void frmCounter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape) 
            CloseForm();
        }
    }
}