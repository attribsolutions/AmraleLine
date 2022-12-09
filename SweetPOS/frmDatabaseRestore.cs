using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using DataObjects;
using BusinessLogic;
using System.Configuration;
using System.Data.SqlClient;

namespace SweetPOS
{
    public partial class frmDatabaseRestore : Form
    {
        #region Class level variables...

        SettingManager settingManager = new SettingManager();
        SettingInfo setting = new SettingInfo();

        #endregion

        public frmDatabaseRestore()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Restore Database";
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SweetPOS.Properties.Settings.SweetPOSAttachConnectionString"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            try
            {
                string sqlText = "";
                if (lsvBackupFile.SelectedItems.Count > 0)
                {
                    sqlText = "USE Master";
                    SqlCommand cmd = new SqlCommand(sqlText, con);
                    con.Open();
                    int retVal = 0;
                    retVal = cmd.ExecuteNonQuery();
                    con.Close();

                    string restorePath = lsvBackupFile.SelectedItems[0].SubItems[2].Text.Trim();
                    sqlText = @"USE MASTER RESTORE DATABASE SweetPOS FROM DISK = '" + restorePath.Trim() + "' WITH REPLACE";
                    SqlCommand cmd1 = new SqlCommand(sqlText, con);
                    con.Open();
                    int retVal1 = 0;
                    retVal1 = cmd1.ExecuteNonQuery();

                    if (retVal1 != 0)
                    {
                        MessageBox.Show("Database restored successfully.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Database Restore." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                con.Close();
            }
        }

        private void frmDatabaseRestore_Load(object sender, EventArgs e)
        {
            FillBackupFile();
        }

        //filled listview with backup database path
        private void FillBackupFile()
        {
            int settingId = 1;      //Hardcode
            setting.Value = settingManager.GetSetting(settingId);
            DirectoryInfo dir = new DirectoryInfo(setting.Value);
            FileInfo[] files = dir.GetFiles("*.Bak");
            lsvBackupFile.Items.Clear();
            int i = 0;
            foreach (FileInfo f in files)
            {
                ListViewItem item = new ListViewItem();
                string[] str = f.Name.Split('.');
                string str1 = str[0].Substring(0, 4);
                string str2 = str[0].Substring(4, 2);
                string str3 = str[0].Substring(6, 2);
                string str4 = str[0].Substring(9, 2);
                string str5 = str[0].Substring(11, 2);
                string s1 = str4 + ":" + str5;
                DateTime dt = new DateTime(Convert.ToInt16(str1), Convert.ToInt16(str2), Convert.ToInt16(str3));
                string d = dt.ToLongDateString();
                i += 1;
                item.Text = Convert.ToString(i);
                item.SubItems.Add(d + " at " + s1);
                item.SubItems.Add(f.FullName);
                lsvBackupFile.Items.Add(item);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}