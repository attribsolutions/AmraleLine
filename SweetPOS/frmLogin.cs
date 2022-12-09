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
using Microsoft.Win32;
using Licenser;
using ChitalePersonalzer;
using System.Configuration;

namespace SweetPOS
{
    public partial class frmLogin : Form
    {
        #region Declaration
        UserManager _UserManager = new UserManager();
        SettingManager _settingManager = new SettingManager();
        DivisionManager _divisionManager = new DivisionManager();
        Communication c;
        string key = Program.KEYSET;
        int _disableTimer = 0;
        bool _fillingdropdownValues = true;
        #endregion

        #region PageLoadEvent
        public frmLogin()
        {
            InitializeComponent();
            this.Text = "User Login";
        }
        #endregion
        
        #region LoadEvent
        private void frmUserLogin_Load(object sender, EventArgs e)
        {
            this.Height = 334;
            FillUsers();
            _fillingdropdownValues = false;

            cboUser.SelectedIndex = -1;
            SetSystemStartTime();
            
            if (Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID)
            {
                
                string portNumber = string.Empty;

                try
                {
                    portNumber = _settingManager.GetSetting(5);
                    c = new Communication(portNumber, Program.BAUDRATE, true);     //Hardcode (PORT Number)
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Opening port. (" + portNumber + ")" + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void SetSystemStartTime()
        {
            SettingInfo setting = new SettingInfo();
            try
            {
                setting.Value = _settingManager.GetSetting(23);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Getting system start time setting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Program.SHOWSHORTBILL = Convert.ToBoolean(_settingManager.GetSetting(25));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Getting report format setting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Convert.ToBoolean(setting.Value))
            {
                try
                {
                    _settingManager.SaveSystemStartTime(DateTime.Now);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Saving system start time setting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        void FillUsers()
        {
            try
            {
                BindingList<UserInfo> users = _UserManager.GetAllSystemUsers(true);

                cboUser.DataSource = users;
                cboUser.DisplayMember = "Name";
                cboUser.ValueMember = "ID";

                users = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting all Users." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void FillDivisions()
        {
            if (!_fillingdropdownValues)
            {
                try
                {
                    BindingList<DivisionInfo> division = _divisionManager.GetAllDivisionByUserID(Convert.ToInt32(cboUser.SelectedValue));
                    cboDivision.DataSource = division;
                    cboDivision.DisplayMember = "DivisionName";
                    cboDivision.ValueMember = "DivisionID";
                    if (division.Count > 0)
                    {
                        lblDivision.Visible = true;
                        cboDivision.Visible = true;
                        cboDivision.Enabled = true;
                        if (division.Count == 1)
                        {
                            Program.DivisionID = division[0].DivisionID;
                            cboDivision.Enabled = false;
                        }
                    }
                    division = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in getting all Divisions." + Environment.NewLine + Environment.NewLine + ex.InnerException.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        #endregion
        
        #region PublicEvent
        bool ValidateFields()
        {
            if (cboUser.SelectedIndex == -1)
            {
                MessageBox.Show("Select user name first.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboUser.Focus();
                return false;
            }

            if (txtPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Please enter password first.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (cboDivision.SelectedIndex == -1)
            {
                MessageBox.Show("Select Division first.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboUser.Focus();
                return false;
            }

            return true;
        }

        bool ValidateRegistry()
        {
            return true;
            //MessageBox.Show("1");
            GetInfo g = new GetInfo();
          //  MessageBox.Show("2");

            string processorSerial = g.GetCPUId();
            //MessageBox.Show("3");

            string serialNumber = g.GenerateSerial(processorSerial, null);
           // MessageBox.Show("4");

            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(Program.REGKEY, false);
            //MessageBox.Show("5");

            if (regKey != null)
            {
               // MessageBox.Show("6");

                object objSerialNumber = regKey.GetValue("SerialNumber");
                //MessageBox.Show("7:" + objSerialNumber.ToString());

                if (objSerialNumber != null)
                {
                  //  MessageBox.Show("8");

                    if (CheckForValidLicenseDate(serialNumber, objSerialNumber.ToString()))
                    {
                   //     MessageBox.Show("9");

                        Program.SERIALKEY = objSerialNumber.ToString();
                     //   MessageBox.Show("10");

                        return true;
                    }
                   // MessageBox.Show("11");

                    if (serialNumber == objSerialNumber.ToString())
                    {
                     //   MessageBox.Show("12");

                        Program.SERIALKEY = objSerialNumber.ToString();
                        return true;
                    }
                   // MessageBox.Show("13");

                    string tempSerialNumber = DateTime.Now.AddMonths(1).ToString("ddMMyyyyhh");
                    string block1 = tempSerialNumber.Substring(0, 5);
                    string block2 = tempSerialNumber.Substring(1, 5);
                    string block3 = tempSerialNumber.Substring(2, 5);
                    string block4 = tempSerialNumber.Substring(3, 5);
                    string block5 = tempSerialNumber.Substring(4, 5);
                    tempSerialNumber = block1 + "-" + block2 + "-" + block3 + "-" + block4 + "-" + block5;
                    if (txtSerialNumber.Text == tempSerialNumber)
                    {
                        Program.TEMPSERIAL = tempSerialNumber;
                        Program.SERIALKEY = tempSerialNumber;
                        return true;
                    }
                    else
                        txtSerialNumber.Text = objSerialNumber.ToString();
                    //MessageBox.Show("15");

                }
                regKey.Close();
            }
           // MessageBox.Show("16");

            return false;
        }
        
        private bool CheckForValidLicenseDate(string srNo, string regNo)
        {
            return true;
            //Check whether rest of the key is valid or not (Other than date logic)
         //   MessageBox.Show("17");

            string tempSrNo = srNo;
            tempSrNo = tempSrNo.Remove(26, 1);      //5
            tempSrNo = tempSrNo.Remove(20, 1);      //1
            tempSrNo = tempSrNo.Remove(14, 1);      //2
            tempSrNo = tempSrNo.Remove(8, 1);       //3
            tempSrNo = tempSrNo.Remove(2, 1);       //4
         //   MessageBox.Show("18");

            string tempRegNo = regNo;
            tempRegNo = tempRegNo.Remove(26, 1);     //5
            tempRegNo = tempRegNo.Remove(20, 1);     //1
            tempRegNo = tempRegNo.Remove(14, 1);     //2
            tempRegNo = tempRegNo.Remove(8, 1);      //3
            tempRegNo = tempRegNo.Remove(2, 1);      //4
          //  MessageBox.Show("19");

            if (tempSrNo == tempRegNo)
            {
           //     MessageBox.Show("20");

                //String in yyMMd format.
                string stringValidDate = regNo.Substring(20, 1) + regNo.Substring(14, 1) + regNo.Substring(8, 1) + regNo.Substring(2, 1) + regNo.Substring(26, 1);
           //     MessageBox.Show("21");

                DateTime validDate = new DateTime(2000 + Convert.ToInt32(stringValidDate.Substring(0, 2)), Convert.ToInt32(stringValidDate.Substring(2, 2)), 1);
           //     MessageBox.Show("22");

                if (DateTime.Today < validDate)
                    return true;
            }
         //   MessageBox.Show("23");

            return false;
        }

        private bool ValidateRegistryAndSetAccordingly()
        {
            return true;
            if (!ValidateRegistry())
            {
                this.Height = 434;
                GetInfo g = new GetInfo();
                txtMachineID.Text = g.GetCPUId();
                pnlRegister.Visible = true;
                MessageText("License validation failed, Get the Serial Number by providing following Machine ID.", Color.Red);
                txtSerialNumber.Focus();

                return false;
            }

            return true;
        }

        private void SetCompanyDetailVariables()
        {
            try
            {
                InfoManager infoManager = new InfoManager();

                Program.MESSAGEBOXTITLE = infoManager.GetInfo(1);

                Program.COMPANYNAME = infoManager.GetInfo(2);
                Program.COMPANYSUBTITLE = infoManager.GetInfo(3);
                Program.ADDRESS1 = infoManager.GetInfo(4);
                Program.ADDRESS2 = infoManager.GetInfo(5);
                Program.PHONENUMBER = infoManager.GetInfo(6);
                Program.VATNO = infoManager.GetInfo(7);
                Program.SUBJECTTO = infoManager.GetInfo(8);

                try
                {
                    Program.CLIENTNAME = SweetPOS.Properties.Settings.Default.ClientName;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Object reference not set to an instance of an object.")
                    {
                        MessageBox.Show("ClientName value not set in config file. Taking default value.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting info value by id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                SettingManager settingManager = new SettingManager();

                Program.PORTNAME = settingManager.GetSetting(5);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting setting by id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void KeyboardClick(object sender, EventArgs e)
        {
            txtPassword.Text += ((Button)sender).Text;
            txtPassword.Select(txtPassword.Text.Length, 0);
            txtPassword.Focus();
        }

        private void SetUserRole(UserInfo user)
        {
            if (user.UserRole.Contains("Admin"))
                Program.UserType = EnumClass.UserRoles.Administrator;
            else if (user.UserRole.Contains("Cash"))
                Program.UserType = EnumClass.UserRoles.Cashier;
            else if (user.UserRole.Contains("Sale"))
                Program.UserType = EnumClass.UserRoles.SalesCounter;
        }

        void ShowMainForm()
        {
            this.Hide();
            if (Program.UserType == EnumClass.UserRoles.SalesCounter)
            {
                frmSaleByCode frm = new frmSaleByCode();
                frm.ShowDialog();
            }
            else
            {
                int a = Convert.ToInt32(cboDivision.SelectedValue);
                frmMain frm = new frmMain(a);
                frm.ShowDialog();
            }
            this.Close();
        }

        UserInfo GetUserByCardID()
        {
            string cardID = c.ReadCardID();

            UserInfo user = null;
            try
            {
                user = _UserManager.GetUsersByCardID(cardID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting User by CardID." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return user;
        }

        void MessageText(string text, Color c)
        {
            _disableTimer = 0;
            tmrDisableLabel.Enabled = true;

            Application.DoEvents();
        }

        #endregion

        #region ControlEvent
        private void lblClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void cboUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPassword.Text = string.Empty;
            txtPassword.Focus();
            FillDivisions();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateRegistryAndSetAccordingly())
               return;

            if (!ValidateFields())
                return;
            
            UserInfo user = null;
            try
            {
                user = _UserManager.GetUser(Convert.ToInt32(cboUser.SelectedValue));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting User by name." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (user.Password.ToUpper() == txtPassword.Text.ToUpper())
            {
                SetCompanyDetailVariables();
                SetUserRole(user);

                Program.CURRENTUSER = (byte)user.ID;
                Program.LoginName = cboUser.Text;
                Program.DivisionID = Convert.ToInt32(cboDivision.SelectedValue);

                ShowMainForm();
            }
            else
            {
                txtPassword.Text = string.Empty;
                txtPassword.Focus();
            }
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

        private void frmLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                btnEnter_Click(sender, e);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(Program.REGKEY, true);
            if (regKey == null)
            {
                regKey = Registry.LocalMachine.CreateSubKey(Program.REGKEY);
            }
            regKey.SetValue("SerialNumber", (object)txtSerialNumber.Text);

            MessageBox.Show("Serial Number successfully saved to registry.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnLogin_Click(sender, e);
        }

        private void tmrDisableLabel_Tick(object sender, EventArgs e)
        {
            _disableTimer += 1;

            if (_disableTimer == 7)
            {

                _disableTimer = 0;
                tmrDisableLabel.Enabled = false;
            }
        }

        private void lblMessages_Click(object sender, EventArgs e)
        {

        }

        #endregion              
        
    }
}