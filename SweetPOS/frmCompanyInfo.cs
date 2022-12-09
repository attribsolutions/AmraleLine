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
    public partial class frmCompanyInfo : Form
    {
        #region Class level variables...

        InfoManager _infoManager = new InfoManager();
        string _value = string.Empty;

        #endregion

        public frmCompanyInfo()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Company Information";
        }

        private void frmCompanyInfo_Load(object sender, EventArgs e)
        {
            try
            {
                _value = _infoManager.GetInfo(1);     //Hardcode
                txtSystemTitle.Text = _value;

                _value = _infoManager.GetInfo(2);     //Hardcode
                txtCompanyName.Text = _value;

                _value = _infoManager.GetInfo(3);     //Hardcode
                txtCompanySubtitle.Text = _value;

                _value = _infoManager.GetInfo(4);     //Hardcode
                txtAddress1.Text = _value;

                _value = _infoManager.GetInfo(5);     //Hardcode
                txtAddress2.Text = _value;

                _value = _infoManager.GetInfo(6);     //Hardcode
                txtPhoneNo.Text = _value;

                _value = _infoManager.GetInfo(7);     //Hardcode
                txtVatTinNo.Text = _value;

                _value = _infoManager.GetInfo(8);     //Hardcode
                txtSubjectTo.Text = _value;

                _value = _infoManager.GetInfo(9);     //Hardcode
                txtFirstLine.Text = _value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Info." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            try
            {
                _infoManager.AddInfo(1, txtSystemTitle.Text);

                _infoManager.AddInfo(2, txtCompanyName.Text);

                _infoManager.AddInfo(3, txtCompanySubtitle.Text);

                _infoManager.AddInfo(4, txtAddress1.Text);

                _infoManager.AddInfo(5, txtAddress2.Text);

                _infoManager.AddInfo(6, txtPhoneNo.Text);

                _infoManager.AddInfo(7, txtVatTinNo.Text);

                _infoManager.AddInfo(8, txtSubjectTo.Text);

                _infoManager.AddInfo(9, txtFirstLine.Text);

                MessageBox.Show("Company Info updated successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Info." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}