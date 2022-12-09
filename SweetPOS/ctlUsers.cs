using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;

namespace SweetPOS
{
    public partial class ctlUsers : UserControl
    {
        UserManager _userManager = new UserManager();

        public ctlUsers()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNewUser_Click(object sender, EventArgs e)
        {
            AddUser();
        }

        public bool SearchUsers()
        {
            BindingList<UserInfo> users = new BindingList<UserInfo>();
            try
            {
                users = _userManager.GetUsersByName(txtSearchText.Text);
                if (users.Count > 0)
                {
                    grdResult.DataSource = users;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = users.Count.ToString() + " record(s) found.";
                users = null;
                txtSearchText.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting User by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["Password"].Visible = false;

            grdResult.Columns["CreatedBy"].Visible = false;
            grdResult.Columns["CreatedOn"].Visible = false;
            grdResult.Columns["UpdatedBy"].Visible = false;
            grdResult.Columns["UpdatedOn"].Visible = false;
        }

        void AddUser()
        {
            frmUser frm = new frmUser();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchUsers();

                //Set focus in grid at position, where the new record added.
                for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                {
                    if (frm.NewRecordID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                    {
                        grdResult.Rows[i].Selected = true;
                        grdResult.CurrentCell = grdResult[2, i];
                        break;
                    }
                }
            }
        }

        bool EditUser()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                UserInfo user = (UserInfo)grdResult.SelectedRows[0].DataBoundItem;

                frmUser frm = new frmUser(user);
                if (user.ID == 1)      //Hardcode   (Reserved User)
                    frm.AdminModification = true;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchUsers();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (user.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[2, i];
                            break;
                        }
                    }
                }
            }
            return true;
        }

        bool DeleteUser()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                if (Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value) == 1)      //Hardcode   (Reserved User)
                    return false;

                string tableName = string.Empty;
                bool isUsed = true;
                try
                {
                    isUsed = _userManager.CheckUserUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether User is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _userManager.DeleteUser(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchUsers();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, User is in use. (Table Name: " + tableName + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting User." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchUsers();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditUser();
        }

        private void btnModifyUser_Click(object sender, EventArgs e)
        {
            EditUser();
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchUsers();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddUser();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditUser();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }
    }
}