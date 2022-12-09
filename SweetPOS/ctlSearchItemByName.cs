using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BusinessLogic;
using DataObjects;

namespace SweetPOS
{
    public partial class ctlSearchItemByName : UserControl
    {
        ItemManager _itemManager = new ItemManager();
        bool _fillingGrid = true;

        public ctlSearchItemByName()
        {
            InitializeComponent();
        }

        public bool SearchItems()
        {
            _fillingGrid = true;

            grdItems.Rows.Clear();
            
            BindingList<ItemInfo> items = new BindingList<ItemInfo>();
            try
            {
                if (txtItemName.Text.Trim().Length > 0)
                {
                    items = _itemManager.GetItemsByFilter(11, txtItemName.Text, 11);
                    if (items.Count > 0)
                    {
                        foreach (ItemInfo item in items)
                        {
                            int row = grdItems.Rows.Add();

                            grdItems.Rows[row].Cells["ItemID"].Value = item.ItemCode;
                            grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
                            grdItems.Rows[row].Cells["Rate"].Value = item.Rate;
                        }
                        grdItems.Focus();
                    }
                    else
                    {
                        grdItems.DataSource = null;
                        txtItemName.Focus();
                    }
                    items = null;
                    txtItemName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Item by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            _fillingGrid = false;
            object o = new object();
            EventArgs e = new EventArgs();
            grdItems_SelectionChanged(o, e);

            return true;
        }

        private void KeyboardClick(object sender, EventArgs e)
        {
            txtItemName.Text += ((Button)sender).Text;
            txtItemName.Select(txtItemName.Text.Length, 0);
            txtItemName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (txtItemName.Text.Length > 0)
            {
                txtItemName.Text = txtItemName.Text.Substring(0, txtItemName.Text.Length - 1);
                txtItemName.Select(txtItemName.Text.Length, 0);
                txtItemName.Focus();
            }
            else
                txtItemName.Focus();
        }

        private void grdItems_SelectionChanged(object sender, EventArgs e)
        {
            Control[] c = this.Parent.Parent.Parent.Controls.Find("txtCode", true);
            if (grdItems.Rows.Count > 0 && !_fillingGrid)
            {
                ((TextBox)c[0]).Text = grdItems.SelectedRows[0].Cells["ItemID"].Value.ToString();
            }
            else
            {
                ((TextBox)c[0]).Text = string.Empty;
            }
        }

        private void btnSpace_Click(object sender, EventArgs e)
        {
            txtItemName.Text += " ";
            txtItemName.Select(txtItemName.Text.Length, 0);
            txtItemName.Focus();
        }

        private void txtItemName_TextChanged(object sender, EventArgs e)
        {
            SearchItems();
        }
    }
}
