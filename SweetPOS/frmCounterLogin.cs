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
    public partial class frmCounterLogin : Form
    {
        CounterManager _counterManager = new CounterManager();

        public frmCounterLogin()
        {
            InitializeComponent();
        }

        private void lblClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void frmCounterLogin_Load(object sender, EventArgs e)
        {
            FillCounters();
            cboCounter.SelectedIndex = -1;
        }

        void FillCounters()
        {
            try
            {
                BindingList<CounterInfo> counters = _counterManager.GetCountersAll();

                cboCounter.DataSource = counters;
                cboCounter.DisplayMember = "Name";
                cboCounter.ValueMember = "ID";

                counters = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting all Counters." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPassword.Text = string.Empty;
            txtPassword.Focus();
        }

        bool ValidateFields()
        {
            if (cboCounter.SelectedIndex == -1)
            {
                MessageBox.Show("Select counter name first.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCounter.Focus();
                return false;
            }

            if (txtPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Please enter password first.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            CounterInfo counter = null;
            try
            {
                counter = _counterManager.GetCounter(Convert.ToInt32(cboCounter.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Counter by name." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;   
            }

            if (counter.Password.ToUpper() == txtPassword.Text.ToUpper())
            {
                Program.CounterID = Convert.ToInt32(cboCounter.SelectedValue);
                Program.CounterName = cboCounter.Text;

                this.Hide();
                frmSale frm = new frmSale();
                if (!this.ShowInTaskbar)
                    frm.ShowInTaskbar = false;
                frm.ShowDialog();
                this.Close();
            }
            else
            {
                txtPassword.Text = string.Empty;
                txtPassword.Focus();
            }
        }

        private void KeyboardClick(object sender, EventArgs e)
        {
            txtPassword.Text += ((Button)sender).Text;
            txtPassword.Select(txtPassword.Text.Length, 0);
            txtPassword.Focus();
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length > 0)
            {
                txtPassword.Text = txtPassword.Text.Substring(0, txtPassword.Text.Length - 1);
                txtPassword.Select(txtPassword.Text.Length, 0);
                txtPassword.Focus();
            }
            else
                txtPassword.Focus();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            btnLogin_Click(sender, e);
        }

        private void frmCounterLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnEnter_Click(sender, e);
            }
        }
    }
}