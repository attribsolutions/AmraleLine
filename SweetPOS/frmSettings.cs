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
    public partial class frmSettings : Form
    {
        #region Class level variables...
        DivisionManager _divisionManager = new DivisionManager();
        SettingManager _settingManager = new SettingManager();
        string _value = string.Empty;

        #endregion

        public frmSettings()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Settings";
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {

            if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithNetworking || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenAtCounterWithBarCode || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ManualBilling)
            {
                label16.Visible = label7.Visible = cboPortName.Visible = false;
            }
            try
            {
                _value = _settingManager.GetSetting(1);     //Hardcode
                txtDBPath.Text = _value;

                _value = _settingManager.GetSetting(2);     //Hardcode
                cboShowBill.Text = _value;

                _value = _settingManager.GetSetting(3);     //Hardcode
                txtBackImage.Text = _value;

                _value = _settingManager.GetSetting(4);     //Hardcode
                cboDisposeUserControl.Text = _value;

                _value = _settingManager.GetSetting(5);     //Hardcode
                cboPortName.Text = _value;

                _value = _settingManager.GetSetting(6);     //Hardcode
                chkResetBillNumberEachDay.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(7);     //Hardcode
                chkRoundAmounts.Checked = Convert.ToBoolean(_value);

                if (chkRoundAmounts.Checked)
                    txtRound50.Enabled = txtRound1.Enabled = true;

                _value = _settingManager.GetSetting(8);     //Hardcode
                txtRound50.Text = _value;

                _value = _settingManager.GetSetting(9);     //Hardcode
                txtRound1.Text = _value;

                _value = _settingManager.GetSetting(10);     //Hardcode
                chkShowAnimation.Checked = Convert.ToBoolean(_value);

                if (chkShowAnimation.Checked)
                    txtGIFImage.Enabled = btnAminationImage.Enabled = true;

                _value = _settingManager.GetSetting(11);     //Hardcode
                txtGIFImage.Text = _value;

                _value = _settingManager.GetSetting(12);     //Hardcode
                chkShowPrintDirectlyButton.Checked = Convert.ToBoolean(_value);

                if (chkShowPrintDirectlyButton.Checked)
                    cboPrintDirectDefault.Enabled = true;

                _value = _settingManager.GetSetting(13);     //Hardcode
                cboPrintDirectDefault.Text = _value;

                _value = _settingManager.GetSetting(14);     //Hardcode
                chkIntlzAccesstoCashier.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(15);     //Hardcode
                chkShowQuantityTotalOnBill.Checked = Convert.ToBoolean(_value);
                grpCOMPort.Enabled = chkShowQuantityTotalOnBill.Checked;

                _value = _settingManager.GetSetting(16);     //Hardcode
                chkCardPaymentDetails.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(17);     //Hardcode
                txtDifferenceBetweenWeight.Text = _value;

                _value = _settingManager.GetSetting(18);     //Hardcode
                cboCOMPortForWeighingScale.Text = _value;

                _value = _settingManager.GetSetting(19);     //Hardcode
                cboBaudRate.Text = _value;

                _value = _settingManager.GetSetting(20);     //Hardcode
                chkDontSaveIfNoWeight.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(21);     //Hardcode
                txtMorningFrom.Text = _value.Substring(0, 5);
                txtMorningTo.Text = _value.Substring(6, 5);

                _value = _settingManager.GetSetting(22);     //Hardcode
                txtEveningFrom.Text = _value.Substring(0, 5);
                txtEveningTo.Text = _value.Substring(6, 5);

                _value = _settingManager.GetSetting(23);     //Hardcode
                cboSystemStartTimeOnSummary.Text = _value;

                _value = _settingManager.GetSetting(24);     //Hardcode
                chkInitializeCardsWithItems.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(25);     //Hardcode
                chkShowShortBill.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(26);     //Hardcode
                chkCodeToReadCard.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(27);     //Hardcode
                txtBarCode.Text = _value;

                _value = _settingManager.GetSetting(28);     //Hardcode
                chkAddOnlyAfterRetrive.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(29);     //Hardcode
                chkShowItemMultiple.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(30);     //Hardcode
                chkShowShowFromCardButton.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(32);     //Hardcode
                chkSaveAndPrint.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(33);     //Hardcode
                chkModifyRates.Checked = Convert.ToBoolean(_value);

                _value = _settingManager.GetSetting(34);     //Hardcode
                txtConfirmInward.Text = _value;

                _value = _settingManager.GetSetting(35);    //HardCode
                cboPrintBIll.Text = _value;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            if (txtDBPath.Text.Trim() != string.Empty)
                f.SelectedPath = txtDBPath.Text;

            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.SelectedPath.Length == 3)
                {
                    MessageBox.Show("Please select a folder.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtDBPath.Text = f.SelectedPath + @"\";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (chkShowQuantityTotalOnBill.Checked && cboCOMPortForWeighingScale.Text == cboPortName.Text)
            {
                MessageBox.Show("Both (RFID & Weighing Scale) ports cannot be same.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                tabMain.SelectedTab = tabSale;
                cboCOMPortForWeighingScale.Focus();
                return;
            }

            decimal d;
            if (!decimal.TryParse(txtRound1.Text, out d) || !decimal.TryParse(txtRound50.Text, out d))
            {
                MessageBox.Show("Show valid values in rounding details.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                tabMain.SelectedTab = tabSale;
                txtRound50.Focus();
                return;
            }

            if (!decimal.TryParse(txtDifferenceBetweenWeight.Text, out d))
            {
                MessageBox.Show("Show valid value in difference percentage.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                tabMain.SelectedTab = tabSale;
                txtDifferenceBetweenWeight.Focus();
                return;
            }

            //Session time validation
            int morStartHr = Convert.ToInt32(txtMorningFrom.Text.Substring(0, 2));
            int morStartMn = Convert.ToInt32(txtMorningFrom.Text.Substring(3, 2));
            int morEndHr = Convert.ToInt32(txtMorningTo.Text.Substring(0, 2));
            int morEndMn = Convert.ToInt32(txtMorningTo.Text.Substring(3, 2));
            int eveStartHr = Convert.ToInt32(txtEveningFrom.Text.Substring(0, 2));
            int eveStartMn = Convert.ToInt32(txtEveningFrom.Text.Substring(3, 2));
            int eveEndHr = Convert.ToInt32(txtEveningTo.Text.Substring(0, 2));
            int eveEndMn = Convert.ToInt32(txtEveningTo.Text.Substring(3, 2));
            DateTime sessionTime;
            try
            {
                sessionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, morStartHr, morStartMn, 0);
                sessionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, morEndHr, morEndMn, 0);
                sessionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, eveStartHr, eveStartMn, 0);
                sessionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, eveEndHr, eveEndMn, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Session time entered." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SettingInfo setting = new SettingInfo();
            try
            {
                setting.ID = 1;     //Hardcode
                setting.Value = txtDBPath.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 2;     //Hardcode
                setting.Value = cboShowBill.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 3;     //Hardcode
                setting.Value = txtBackImage.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 4;     //Hardcode
                setting.Value = cboDisposeUserControl.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 5;     //Hardcode
                setting.Value = cboPortName.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 6;     //Hardcode
                setting.Value = chkResetBillNumberEachDay.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 7;     //Hardcode
                setting.Value = chkRoundAmounts.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 8;     //Hardcode
                setting.Value = txtRound50.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 9;     //Hardcode
                setting.Value = txtRound1.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 10;     //Hardcode
                setting.Value = chkShowAnimation.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 11;     //Hardcode
                setting.Value = txtGIFImage.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 12;     //Hardcode
                setting.Value = chkShowPrintDirectlyButton.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 13;     //Hardcode
                setting.Value = cboPrintDirectDefault.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 14;     //Hardcode
                setting.Value = chkIntlzAccesstoCashier.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 15;     //Hardcode
                setting.Value = chkShowQuantityTotalOnBill.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 16;     //Hardcode
                setting.Value = chkCardPaymentDetails.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 17;     //Hardcode
                setting.Value = txtDifferenceBetweenWeight.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 18;     //Hardcode
                setting.Value = cboCOMPortForWeighingScale.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 19;     //Hardcode
                setting.Value = cboBaudRate.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 20;     //Hardcode
                setting.Value = chkDontSaveIfNoWeight.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 21;     //Hardcode
                setting.Value = txtMorningFrom.Text + "-" + txtMorningTo.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 22;     //Hardcode
                setting.Value = txtEveningFrom.Text + "-" + txtEveningTo.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 23;     //Hardcode
                setting.Value = cboSystemStartTimeOnSummary.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 24;     //Hardcode
                setting.Value = chkInitializeCardsWithItems.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 25;     //Hardcode
                setting.Value = chkShowShortBill.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 26;
                if (txtBarCode.Text.Trim().Length > 0 && chkCodeToReadCard.Checked)
                    setting.Value = chkCodeToReadCard.Checked.ToString();
                else
                    setting.Value = "False";
                _settingManager.UpdateSetting(setting);

                setting.ID = 27;     //Hardcode
                setting.Value = txtBarCode.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 28;     //Hardcode
                setting.Value = chkAddOnlyAfterRetrive.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 29;     //Hardcode
                setting.Value = chkShowItemMultiple.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 30;     //Hardcode
                setting.Value = chkShowShowFromCardButton.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 32;     //Hardcode
                setting.Value = chkSaveAndPrint.Checked.ToString();
                _settingManager.UpdateSetting(setting);

                setting.ID = 33;     //Hardcode
                setting.Value = chkModifyRates.Checked.ToString();
                _settingManager.UpdateSetting(setting);


                setting.ID = 34;     //Hardcode
                setting.Value = txtConfirmInward.Text;
                _settingManager.UpdateSetting(setting);

                setting.ID = 35;    //Hardcode
                setting.Value = cboPrintBIll.Text;
                _settingManager.UpdateSetting(setting);

                MessageBox.Show("Settings updated successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Settings." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnBrowseBackImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "JPEG Files (*.jpg)|*.jpg";

            if (txtBackImage.Text.Trim() != string.Empty)
                f.FileName = txtBackImage.Text;

            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.FileName.Length == 3)
                {
                    MessageBox.Show("Please select a file.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtBackImage.Text = f.FileName;
            }
        }

        private void btnAminationImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "GIF Images (*.gif)|*.gif";

            if (txtBackImage.Text.Trim() != string.Empty)
                f.FileName = txtBackImage.Text;

            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.FileName.Length == 3)
                {
                    MessageBox.Show("Please select a file.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtBackImage.Text = f.FileName;
            }
        }

        private void NumericValidating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(((TextBox)sender).Text, out d))
                e.Cancel = true;
            else
                ((TextBox)sender).Text = Convert.ToDecimal(((TextBox)sender).Text).ToString("0.00");
        }

        

        #region Check Changed...

        private void chkRoundAmounts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRoundAmounts.Checked)
                txtRound50.Enabled = txtRound1.Enabled = true;
            else
                txtRound50.Enabled = txtRound1.Enabled = false;
        }

        private void chkShowAnimation_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowAnimation.Checked)
                txtGIFImage.Enabled = btnAminationImage.Enabled = true;
            else
                txtGIFImage.Enabled = btnAminationImage.Enabled = false;
        }

        private void chkShowPrintDirectlyButton_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPrintDirectlyButton.Checked)
                cboPrintDirectDefault.Enabled = true;
            else
            {
                cboPrintDirectDefault.Enabled = false;
                cboPrintDirectDefault.Text = "True";
            }
        }

        private void chkShowQuantityTotalOnBill_CheckedChanged(object sender, EventArgs e)
        {
            grpCOMPort.Enabled = chkShowQuantityTotalOnBill.Checked;
        }

        private void chkCodeToReadCard_CheckedChanged(object sender, EventArgs e)
        {
            txtBarCode.Enabled = chkCodeToReadCard.Checked;
        }

        private void chkAddOnlyAfterRetrive_CheckedChanged(object sender, EventArgs e)
        {
            chkShowShowFromCardButton.Enabled = !chkAddOnlyAfterRetrive.Checked;
            chkShowShowFromCardButton.Checked = chkAddOnlyAfterRetrive.Checked;
        }

        private void chkShowShowFromCardButton_CheckedChanged(object sender, EventArgs e)
        {
            chkCodeToReadCard.Enabled = chkShowShowFromCardButton.Checked;
            if (chkShowShowFromCardButton.Checked && chkCodeToReadCard.Checked)
                txtBarCode.Enabled = true;
            else
                txtBarCode.Enabled = false;
        }

        #endregion
    }
}
