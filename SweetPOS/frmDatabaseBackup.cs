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
using System.IO;
using System.Configuration;

namespace SweetPOS
{
    public partial class frmDatabaseBackup : Form
    {
        //#region Variable Decleration...
        //SettingManager _settingManager = new SettingManager();
        //#endregion

        //public frmDatabaseBackup()
        //{
        //    InitializeComponent();
        //    this.Text = Program.MESSAGEBOXTITLE + " - Restore Database";
        //}

        ////take database back
        //private void DatabaseBackup()
        //{
        //    SettingInfo setting = new SettingInfo();
        //    int settingId = 1;      //Backup path ID    //Hardcode
        //    string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
        //    string[] strTime = DateTime.Now.TimeOfDay.ToString().Split(':');
        //    string strDateTime = strDate + " " + strTime[0].PadLeft(2, '0') + strTime[1].PadLeft(2, '0');
        //    setting.Value = _settingManager.GetSetting(settingId);
        //    try
        //    {
        //        string backupPath = setting.Value + strDateTime + ".bak";
        //        int retVal = 0;
        //        retVal = _settingManager.DatabaseBackup(backupPath);
        //        if (retVal != 0)
        //        {
        //            MessageBox.Show("Database Backup completed successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            lblStatus.Text = "Database Backup successful.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error in Database Backup." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        lblStatus.Text = "Error: " + ex.Message;
        //        return;
        //    }
        //    setting = null;
        //}

        //private void frmDatabaseBackup_Load(object sender, EventArgs e)
        //{
        //    SettingInfo setting = new SettingInfo();
        //    setting.Value = _settingManager.GetSetting(1);      //Hardcode
        //    if (setting.Value.Trim().Length <= 3)
        //    {
        //        lblStatus.Text = "Backup path is not valid...";
        //        return;
        //    }
        //    tmrStart.Enabled = true;
        //}

        //private void tmrStart_Tick(object sender, EventArgs e)
        //{
        //    tmrStart.Enabled = false;
        //    lblStatus.Text = "Processing, please wait...";
        //    DatabaseBackup();
        //}

        //private void btnClose_Click(object sender, EventArgs e)
        //{
        //    this.DialogResult = DialogResult.OK;
        //}
        #region Class level variables...

        SettingManager _settingManager = new SettingManager();
        SettingInfo _setting = new SettingInfo();

        int _backupSettingId = 1;       //Hardcode

        #endregion

        public frmDatabaseBackup()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Database Backup";
        }

        private void DatabaseBackup()
        {
            //string Database=SweetPOS.Properties.Settings.Default.DatabaseName;//database name from app.config

            //Added by prajakta 
            string dbName = ConfigurationManager.ConnectionStrings["SweetPOS.Properties.Settings.ConnectionString"].ToString();
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(dbName);

            string server = builder.DataSource;
            string DatabaseName = builder.InitialCatalog;

            //end

            string strDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
            string[] strTime = DateTime.Now.TimeOfDay.ToString().Split(':');
            string strDateTime = DatabaseName + strDate + " " + strTime[0].PadLeft(2, '0') + strTime[1].PadLeft(2, '0');
            try
            {
                string backupPath = txtBackupLocation.Text.Trim() + "\\" + strDateTime + ".bak";
                int retVal = 0;
                retVal = _settingManager.DatabaseBackup(backupPath, DatabaseName);
                if (retVal != 0)
                {
                    btnBackupDatabase.Enabled = false;
                    lblBackupStatus.Text = "Database Backup successful.";
                    MessageBox.Show("The backup of database completed successfully." + Environment.NewLine + backupPath, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnClose.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error [Database Backup]: " + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblBackupStatus.Text = "Database backup failed";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void frmDatabaseBackup_Load(object sender, EventArgs e)
        {
            if (_settingManager.GetSetting(_backupSettingId).Trim().Length <= 3)
            {
                lblBackupStatus.Text = "Backup path is not valid...";
                return;
            }
            txtBackupLocation.Text = _settingManager.GetSetting(_backupSettingId);

            ValidateBackupLocation();
        }

        bool ValidateBackupLocation()
        {
            if (!Directory.Exists(txtBackupLocation.Text))
            {
                //MessageBox.Show("Backup path not valid, Please verify.", Program.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblBackupStatus.Text = "Backup path is not valid...";
                return false;
            }
            else
                lblBackupStatus.Text = string.Empty;

            return true;
        }

        private void btnBackupDatabase_Click(object sender, EventArgs e)
        {
            if (!ValidateBackupLocation())
                return;

            lblBackupStatus.Text = "Processing, please wait...";
            DatabaseBackup();
        }

        private void lnkChangeBackupPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            if (txtBackupLocation.Text.Trim() != string.Empty)
                f.SelectedPath = txtBackupLocation.Text;

            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.SelectedPath.Length == 3)
                {
                    MessageBox.Show("Please select folder.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                txtBackupLocation.Text = f.SelectedPath;
                DialogResult dr = MessageBox.Show("Do you want to set this bakup location as default?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    SettingManager settingManager = new SettingManager();
                    try
                    {
                        SettingInfo setting = new SettingInfo();
                        setting.ID = _backupSettingId;
                        setting.Value = txtBackupLocation.Text;

                        settingManager.UpdateSetting(setting);
                        MessageBox.Show("The backup location saved successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ValidateBackupLocation();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in saving setting value." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }
    }
}