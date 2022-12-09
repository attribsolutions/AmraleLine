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
    public partial class ctlOrders : UserControl
    {
        OrderManager _orderManager = new OrderManager();
        bool _fillingGrid = true;

        public ctlOrders()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddOrder();
        }

        public bool SearchOrders()
        {
            _fillingGrid = true;

            if (cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3)
            {
                decimal d;
                if (!decimal.TryParse(txtSearchText.Text, out d))
                {
                    txtSearchText.Focus();
                    MessageBox.Show("Please enter valid amount.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            BindingList<OrderInfo> orders = new BindingList<OrderInfo>();
            try
            {
                if (cboSearchIn.SelectedIndex == 0 || cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3)
                    orders = _orderManager.GetOrdersByFilter(cboSearchIn.SelectedIndex, txtSearchText.Text, null, null);
                if (cboSearchIn.SelectedIndex == 1 || cboSearchIn.SelectedIndex == 5)
                    orders = _orderManager.GetOrdersByFilter(cboSearchIn.SelectedIndex, string.Empty, (object)dtStartDate.Value.Date, (object)dtEndDate.Value.Date);
                if (cboSearchIn.SelectedIndex == 4)
                    orders = _orderManager.GetOrdersByFilter(cboSearchIn.SelectedIndex, string.Empty, null, null);

                if (orders.Count > 0)
                {
                    grdResult.DataSource = orders;
                    DisableFields();
                    grdResult.Focus();
                }
                else
                {
                    grdResult.DataSource = null;
                    txtSearchText.Focus();
                }
                lblRecordsFound.Text = orders.Count.ToString() + " record(s) found.";
                orders = null;
                txtSearchText.Focus();

                _fillingGrid = false;

                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Order by name." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _fillingGrid = false;
                return false;
            }
            return true;
        }

        void CalculateTotals()
        {
            txtTotalCash.Text = txtTotalBalance.Text = "0.00";
            decimal totalAmount = 0;
            decimal advance = 0;
            decimal deliveryPayments = 0;

            foreach (DataGridViewRow dr in grdResult.Rows)
            {
                totalAmount += Convert.ToDecimal(dr.Cells["TotalAmount"].Value);
                advance += Convert.ToDecimal(dr.Cells["Advance"].Value);
                deliveryPayments += Convert.ToDecimal(dr.Cells["DeliveryPayment"].Value);
            }

            txtTotalCash.Text = totalAmount.ToString("0.00");
            txtTotalBalance.Text = (totalAmount - advance - deliveryPayments).ToString("0.00");
        }

        void DisableFields()
        {
            grdResult.Columns["ID"].Visible = false;
            grdResult.Columns["BillID"].Visible = false;
            grdResult.Columns["CustomerID"].Visible = false;

            grdResult.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["Advance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grdResult.Columns["DeliveryPayment"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        void AddOrder()
        {
            frmOrder frm = new frmOrder();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SearchOrders();

                //Set focus in grid at position, where the new record added.
                for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                {
                    if (frm.NewRecordID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                    {
                        grdResult.Rows[i].Selected = true;
                        grdResult.CurrentCell = grdResult[1, i];
                        break;
                    }
                }
            }
        }

        bool EditOrder()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                OrderInfo order = (OrderInfo)grdResult.SelectedRows[0].DataBoundItem;

                BindingList<OrderItemInfo> orderItems = GetOrderItemsByOrderId(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));

                frmOrder frm = new frmOrder(order, orderItems);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchOrders();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (order.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[1, i];
                            break;
                        }
                    }
                }
            }
            return true;
        }

        bool DeleteOrder()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                string tableName = string.Empty;
                bool isUsed = false;
                try
                {
                    isUsed = _orderManager.CheckOrderUsed(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value), out tableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in checking whether Order is used or not." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    if (!isUsed)
                    {
                        DialogResult dr = MessageBox.Show("Confirm Delete?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            _orderManager.DeleteOrder(Convert.ToInt32(grdResult.SelectedRows[0].Cells["ID"].Value));
                            SearchOrders();
                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot delete, Order is in use.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting Order." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        BindingList<OrderItemInfo> GetOrderItemsByOrderId(int orderId)
        {
            BindingList<OrderItemInfo> retVal = null;
            try
            {
                retVal = _orderManager.GetOrderItemsByOrderId(orderId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Order Items by Order Id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return retVal;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            SearchOrders();
        }

        private void grdResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditOrder();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            EditOrder();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteOrder();
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchOrders();
            }
        }

        private void ctlOrders_Load(object sender, EventArgs e)
        {
            cboSearchIn.SelectedIndex = 0;
        }

        private void cboSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchIn.SelectedIndex == 0 || cboSearchIn.SelectedIndex == 2 || cboSearchIn.SelectedIndex == 3)     //Hardcode
            {
                txtSearchText.Visible = true;
                dtStartDate.Visible = false;
                lblTo.Visible = false;
                dtEndDate.Visible = false;
            }
            if (cboSearchIn.SelectedIndex == 1 || cboSearchIn.SelectedIndex == 5)     //Hardcode
            {
                txtSearchText.Visible = false;
                dtStartDate.Visible = true;
                lblTo.Visible = true;
                dtEndDate.Visible = true;
            }
            if (cboSearchIn.SelectedIndex == 4)     //Hardcode
            {
                txtSearchText.Visible = false;
                dtStartDate.Visible = false;
                lblTo.Visible = false;
                dtEndDate.Visible = false;
            }
        }

        private void btnPrintOrder_Click(object sender, EventArgs e)
        {
            try
            {
                ReportClass rpt = new ReportClass();
                rpt.OrderForm(Convert.ToInt32(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value)));
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt64(grdResult.SelectedRows[0].Cells["BillID"].Value) > 0)
                MessageBox.Show("Bill already created.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                OrderConvertToSales();
        }

        private void OrderConvertToSales()
        {
            if (grdResult.SelectedRows.Count > 0)
            {
                OrderInfo order = (OrderInfo)grdResult.SelectedRows[0].DataBoundItem;

                BindingList<OrderItemInfo> orderItems = GetOrderItemsByOrderId(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value));

                frmSaleManual frm = new frmSaleManual(order, orderItems);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SearchOrders();

                    //Set focus in grid at position, where it was previously before edit.
                    for (int i = 0; i <= grdResult.Rows.Count - 1; i++)
                    {
                        if (order.ID == Convert.ToInt32(grdResult.Rows[i].Cells["ID"].Value))
                        {
                            grdResult.Rows[i].Selected = true;
                            grdResult.CurrentCell = grdResult[1, i];
                            break;
                        }
                    }
                }
            }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                ReportClass rpt = new ReportClass();
                rpt.ShowInvoice(Convert.ToInt32(Convert.ToInt32(grdResult.SelectedRows[0].Cells["Id"].Value)));
            }
            catch (Exception ex)
            {

            }
        }
    }
}