using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DataObjects;
using BusinessLogic;
using System.IO;
using ChitalePersonalzer;

namespace SweetPOS
{
    public partial class frmSale : Form
    {
        #region Class level variable declaration...

        ItemCompanyManager _itemCompanyManager = new ItemCompanyManager();
        ItemGroupManager _itemGroupManager = new ItemGroupManager();
        ItemManager _itemManager = new ItemManager();
        SaleItemManager _saleItemManager = new SaleItemManager();
        SaleManager _saleManager = new SaleManager();
        SettingManager _settingManager = new SettingManager();
        CustomerManager _customerManager = new CustomerManager();
        Timer tmrClearMessage = new Timer();
        Timer tmrClock = new Timer();
        Timer tmrCustomerDisplay = new Timer();
        CustomerInfo _retVal = new CustomerInfo();
        SaleProcessingManager _processingManager = new SaleProcessingManager();

        int grpPageNo = 1;
        int grpPageCount = 5;
        int itmPageNo = 1;
        int itmPageCount = 25;
        int companyPageNo = 1;
        int companyPageCount = 7;

        int lastClickedCompanyID = 0;
        int lastClickedGroupID = 0;

        Padding _padding = new Padding(2);
        bool _fillingMixTopli = false;

        Int16 _amtPercent = 0;
        decimal _discountAmountPercent = 0;

        int _newRecordID = 0;
        string _cardID = string.Empty;

        Communication _c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
        string _key = Program.KEYSET;
        string _barCodeToReadWriteCard = string.Empty;

        bool _allowAddOnlyAfterRetrive;
        bool _showItemMultiple;
        bool _showShowFromCardButton;

        int _billNumber = 0;

        //For round off amount & Animation
        bool _roundAmount = false;
        decimal _round50 = 0;
        decimal _round1 = 0;
        bool _saveAndprint = false;
        bool _modifyRate = false;

        public int FormSession { get; set; }

        private string _customerNumber = string.Empty;
        public string CustomerNumber
        {
            get
            { return _customerNumber; }
            set
            { txtCode.Text = value.ToString(); }
        }

        string _customerDisplayMessage = string.Empty;

        #endregion

        public frmSale()
        {
            InitializeComponent();
        }

        public frmSale(int session)
        {
            InitializeComponent();
            FormSession = session;

            if (session == 1)
            {
                btnSession1.BackColor = Color.Yellow;
                btnSession2.BackColor = Color.White;
            }
            else
            {
                btnSession1.BackColor = Color.White;
                btnSession2.BackColor = Color.Yellow;
            }


            btnSession1.Visible = false;
            btnSession2.Visible = false;
        }

        private void frmSale_Load(object sender, EventArgs e)
        {
            this.Text = Program.MESSAGEBOXTITLE + " - Sales Transactions";

            btnSession1.Visible = false;
            btnSession2.Visible = false;
            SetQuantityPadNumericPadPositionSize();

            tmrClearMessage.Interval = 1400;
            tmrClearMessage.Tick += new EventHandler(tmrClearMessage_Tick);

            tmrClock.Interval = 500;
            tmrClock.Enabled = true;
            tmrClock.Tick += new EventHandler(tmrClock_Tick);

            tmrCustomerDisplay.Interval = 30000;

            try
            {
                if (_settingManager.GetSetting(26) == "True")
                    _barCodeToReadWriteCard = _settingManager.GetSetting(27);

                _allowAddOnlyAfterRetrive = _settingManager.GetSetting(28) == "True" ? true : false;    //Hardcode

                _showItemMultiple = _settingManager.GetSetting(29) == "True" ? true : false;            //Hardcode

                _showShowFromCardButton = btnShowFromCard.Visible = _settingManager.GetSetting(30) == "True" ? true : false;

                _roundAmount = Convert.ToBoolean(_settingManager.GetSetting(7));        //Hardcode

                _round50 = Convert.ToDecimal(_settingManager.GetSetting(8));            //Hardcode

                _round1 = Convert.ToDecimal(_settingManager.GetSetting(9));             //Hardcode

                _saveAndprint = Convert.ToBoolean(_settingManager.GetSetting(32));      //Hardcode

                _modifyRate = Convert.ToBoolean(_settingManager.GetSetting(33));        //Hardcode

                if (_saveAndprint)
                {
                    lblBillNumber.Visible = txtBillNumber.Visible = true;
                    btnSave.Visible = btnCoupon.Visible = btnPrint.Visible = true;
                }
                else
                {

                    lblBillNumber.Visible = txtBillNumber.Visible = false;
                }


                AutoCompleteForCustomer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting bar code setting." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            NewBill();
            txtCode.Focus();
        }

        private void AutoCompleteForCustomer()
        {
            BindingList<CustomerInfo> customerInfo = null;
            try
            {
                customerInfo = _customerManager.GetCustomersAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AutoCompleteStringCollection autoSource = new AutoCompleteStringCollection();
            foreach (CustomerInfo customer in customerInfo)
            {
                autoSource.Add(customer.Name);
            }
            txtCode.AutoCompleteCustomSource = autoSource;
        }

        private void SetQuantityPadNumericPadPositionSize()
        {
            Size s = pnlRightMain.Size;
            tlpQuantityPad.Height = tlpNumericPad.Height = (s.Height / 2);
            tlpQuantityPad.Top = 0;
            tlpNumericPad.Top = tlpQuantityPad.Height;

            tlpQuantityPad.Width = tlpNumericPad.Width = s.Width;
            tlpQuantityPad.Left = tlpNumericPad.Left = 0;
        }

        void tmrClock_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.Date.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();

            if (_showShowFromCardButton)
            {
                if (grdItems.Rows.Count > 0)
                    btnShowFromCard.Visible = false;
                else
                    btnShowFromCard.Visible = true;
            }
        }

        void tmrClearMessage_Tick(object sender, EventArgs e)
        {
            lblMessages.Visible = false;
            tmrClearMessage.Enabled = false;
        }

        void NewBill()
        {
            pnlRightMain.SuspendLayout();

            try
            {
                txtBillNumber.Text = _saleManager.GetNextBillNumber().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting max bill number." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tlpItemGroups.Enabled = true;
            tlpFixed.Enabled = true;

            itmPageNo = grpPageNo = 1;
            grdItems.Columns["Counter"].Visible = false;
            LoadFixedItems();
            grdItems.Rows.Clear();
            lblTotalAmount.Text = "0.00";
            lblCode.Text = "Code:";
            txtCode.Text = string.Empty;
            grdItems.Columns[1].HeaderText = "0 Items";

            btnSave.Enabled = true;
            _fillingMixTopli = pnlMixTopli.Visible = false;
            pnlMixTopli.Width = 377;
            btnHideQty.Enabled = true;

            _amtPercent = 0;
            _discountAmountPercent = 0;
            lblTotalAmount.Tag = null;
            lblDiscountDetails.Visible = false;

            tlpQuantityPad.Visible = tlpNumericPad.Visible = false;

            pnlRightMain.ResumeLayout(true);
            lblCustomerName.Text = string.Empty;
            lblLastBillAmount.Text = string.Empty;
            btnCoupon.Enabled = true;
            txtCode.Focus();
        }

        #region Groups & Items (Load & Click)

        void HighlightCompanyGroup()
        {
            foreach (Control c in tlpItemCompanies.Controls)
            {
                Button b = (Button)c;
                b.Font = new Font("Arial", 11, FontStyle.Regular);
                b.ForeColor = Color.White;
                if (Convert.ToInt32(b.Tag) == lastClickedCompanyID)
                {
                    b.Font = new Font("Arial", 12, FontStyle.Bold);
                    b.ForeColor = Color.Black;
                }
            }

            foreach (Control c in tlpItemGroups.Controls)
            {
                Button b = (Button)c;
                b.Font = new Font("Arial", 11, FontStyle.Regular);
                b.ForeColor = Color.White;
                if (Convert.ToInt32(b.Tag) == lastClickedGroupID)
                {
                    b.Font = new Font("Arial", 12, FontStyle.Bold);
                    b.ForeColor = Color.Black;
                }
            }

            txtCode.Focus();
        }

        void LoadCompanies()
        {
            tlpItemCompanies.Controls.Clear();

            BindingList<ItemCompanyInfo> itemCompanies = null;
            try
            {
                itemCompanies = _itemCompanyManager.GetItemCompaniesAll(string.Empty, false, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item Categories." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int firstDisplayIndex = 999;
            int cnt = 1;
            foreach (ItemCompanyInfo itemCompany in itemCompanies)
            {
                if (cnt > companyPageNo * companyPageCount)
                {
                    break;
                }

                if (companyPageNo == 1 || cnt > ((companyPageNo - 1) * companyPageCount))
                {
                    Button groupBtn = new Button();
                    groupBtn.Text = itemCompany.DisplayName;
                    groupBtn.Dock = DockStyle.Fill;
                    groupBtn.Tag = itemCompany.ID;
                    groupBtn.UseMnemonic = false;
                    groupBtn.ForeColor = Color.White;
                    groupBtn.FlatStyle = FlatStyle.Flat;
                    groupBtn.Margin = _padding;
                    groupBtn.FlatAppearance.MouseOverBackColor = Color.LightSteelBlue;
                    groupBtn.FlatAppearance.MouseDownBackColor = Color.SteelBlue;

                    groupBtn.Click += new EventHandler(CompanyClickEvent);

                    string imgPath = Application.StartupPath + "\\Images\\Categories\\" + itemCompany.ID.ToString().Trim() + ".jpg";
                    if (File.Exists(imgPath))
                        groupBtn.BackgroundImage = System.Drawing.Image.FromFile(imgPath);

                    groupBtn.TextAlign = ContentAlignment.BottomCenter;
                    groupBtn.BackgroundImageLayout = ImageLayout.Zoom;

                    tlpItemCompanies.Controls.Add(groupBtn);

                    if (itemCompany.DisplayIndex < firstDisplayIndex)
                    {
                        firstDisplayIndex = itemCompany.DisplayIndex;
                        lastClickedCompanyID = itemCompany.ID;
                    }
                }
                cnt += 1;
            }
            LoadGroups(firstDisplayIndex);
        }

        void LoadGroups(int itemCompanyId)
        {
            tlpItemGroups.Controls.Clear();

            BindingList<ItemGroupInfo> itemGroups = null;
            try
            {
                itemGroups = _itemGroupManager.GetItemGroupsAll(itemCompanyId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting All Item Groups." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int firstDisplayIndex = 999;
            int cnt = 1;
            foreach (ItemGroupInfo itemGroup in itemGroups)
            {
                if (cnt > grpPageNo * grpPageCount)
                {
                    break;
                }

                if (grpPageNo == 1 || cnt > ((grpPageNo - 1) * grpPageCount))
                {
                    Button groupBtn = new Button();
                    groupBtn.Text = itemGroup.DisplayName;
                    groupBtn.Dock = DockStyle.Fill;
                    groupBtn.Tag = itemGroup.ID;
                    groupBtn.UseMnemonic = false;
                    groupBtn.ForeColor = Color.White;
                    groupBtn.FlatStyle = FlatStyle.Flat;
                    groupBtn.Margin = _padding;
                    groupBtn.FlatAppearance.MouseOverBackColor = Color.LightSteelBlue;
                    groupBtn.FlatAppearance.MouseDownBackColor = Color.SteelBlue;

                    groupBtn.Click += new EventHandler(GroupClickEvent);

                    string imgPath = Application.StartupPath + "\\Images\\SubCategories\\" + itemGroup.ID.ToString().Trim() + ".jpg";
                    if (File.Exists(imgPath))
                        groupBtn.BackgroundImage = System.Drawing.Image.FromFile(imgPath);

                    groupBtn.TextAlign = ContentAlignment.BottomCenter;
                    groupBtn.BackgroundImageLayout = ImageLayout.Zoom;

                    tlpItemGroups.Controls.Add(groupBtn);

                    if (itemGroup.DisplayIndex < firstDisplayIndex)
                    {
                        firstDisplayIndex = itemGroup.DisplayIndex;
                        lastClickedGroupID = itemGroup.ID;
                    }
                }
                cnt += 1;
            }

            LoadItemsByGroupID(lastClickedGroupID);
        }

        void LoadFixedItems()
        {
            LoadCompanies();
            lastClickedCompanyID = 0;
            lastClickedGroupID = 0;
            HighlightCompanyGroup();

            tlpMostUsedItems.Controls.Clear();
            BindingList<ItemInfo> items = null;
            try
            {
                items = _itemManager.GetItemsByFilter(102, string.Empty, 10000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Fixed Items." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int cnt = 0;
            foreach (ItemInfo item in items)
            {
                cnt += 1;
                Button itemBtn = new Button();
                itemBtn.Text = item.DisplayName;
                itemBtn.Dock = DockStyle.Fill;
                itemBtn.Tag = item.ItemCode;
                itemBtn.UseMnemonic = false;
                itemBtn.ForeColor = Color.Black;
                itemBtn.Margin = _padding;
                itemBtn.FlatStyle = FlatStyle.Flat;

                itemBtn.Click += new EventHandler(ItemClickEvent);

                //string imgPath = @"E:\POS\Code\Images\Items\" + item.ID.ToString().Trim() + ".jpg";   //Hardcode
                string imgPath = Application.StartupPath + "\\Images\\Items\\" + item.ID.ToString().Trim() + ".jpg";
                if (File.Exists(imgPath))
                    itemBtn.BackgroundImage = System.Drawing.Image.FromFile(imgPath);

                itemBtn.TextAlign = ContentAlignment.BottomCenter;
                itemBtn.BackgroundImageLayout = ImageLayout.Stretch;

                tlpMostUsedItems.Controls.Add(itemBtn);

                if (cnt == itmPageCount)
                    break;
            }
        }

        void LoadMostUsedItems()
        {
            tlpMostUsedItems.SuspendLayout();
            tlpMostUsedItems.Controls.Clear();
            BindingList<ItemInfo> items = null;
            try
            {
                items = _itemManager.GetItemsByFilter(103, Program.CounterName, 10000);      //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Most Used Items." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int cnt = 0;
            foreach (ItemInfo item in items)
            {
                cnt += 1;
                Button itemBtn = new Button();
                itemBtn.Text = item.DisplayName;
                itemBtn.Dock = DockStyle.Fill;
                itemBtn.Tag = item.ItemCode;
                itemBtn.UseMnemonic = false;
                itemBtn.ForeColor = Color.Black;
                itemBtn.Margin = _padding;
                itemBtn.FlatStyle = FlatStyle.Flat;
                itemBtn.FlatAppearance.MouseOverBackColor = Color.LightSteelBlue;
                itemBtn.FlatAppearance.MouseDownBackColor = Color.SteelBlue;

                itemBtn.Click += new EventHandler(ItemClickEvent);

                //string imgPath = @"E:\POS\Code\Images\Items\" + item.ID.ToString().Trim() + ".jpg";   //Hardcode
                string imgPath = Application.StartupPath + "\\Images\\Items\\" + item.ID.ToString().Trim() + ".jpg";
                if (File.Exists(imgPath))
                    itemBtn.BackgroundImage = System.Drawing.Image.FromFile(imgPath);

                itemBtn.TextAlign = ContentAlignment.BottomCenter;
                itemBtn.BackgroundImageLayout = ImageLayout.Stretch;

                tlpMostUsedItems.Controls.Add(itemBtn);

                if (cnt == itmPageCount)
                    break;
            }
            tlpMostUsedItems.ResumeLayout(true);
        }

        void CompanyClickEvent(object sender, EventArgs e)
        {
            itmPageNo = 1;

            lastClickedCompanyID = Convert.ToInt32(((Button)sender).Tag);

            try
            {
                ItemCompanyInfo itemCompany = _itemCompanyManager.GetItemCompany(lastClickedCompanyID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Category by Id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadGroups(lastClickedCompanyID);
        }

        void GroupClickEvent(object sender, EventArgs e)
        {
            itmPageNo = 1;

            lastClickedGroupID = Convert.ToInt32(((Button)sender).Tag);
            if (lastClickedCompanyID == 0)
                lastClickedCompanyID = _itemGroupManager.GetItemGroup(lastClickedGroupID).ItemCompanyID;

            //try
            //{
            //    ItemGroupInfo itemGroup = _itemGroupManager.GetItemGroup(lastClickedGroupID);

            //    if (!CheckCouponGroup(itemGroup))
            //        return;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error in getting Group by Id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            LoadItemsByGroupID(lastClickedGroupID);
        }

        bool CheckCouponGroup(ItemGroupInfo itemGroup)
        {
            if (itemGroup.CouponGroup)
            {
                if (grdItems.Rows.Count > 0)
                {
                    MessageText("Invalid selection, First clear non-coupon items.", false);
                    return false;
                }
                else
                {
                    tlpItemGroups.Enabled = false;
                    tlpFixed.Enabled = false;

                    btnSave.Text = "Save Coupon";
                    btnCoupon.Text = "Print Coupon";
                    return true;
                }
            }

            return true;
        }

        private void MessageText(string message, bool successMessage)
        {
            lblMessages.BackColor = Color.Yellow;
            lblMessages.ForeColor = Color.Red;
            tmrClearMessage.Enabled = true;
            lblMessages.BringToFront();
            if (successMessage)
            {
                lblMessages.BackColor = Color.Green;
                lblMessages.ForeColor = Color.White;
            }

            lblMessages.Visible = true;
            lblMessages.Text = message;



            txtCode.Focus();
        }

        void LoadItemsByGroupID(int groupId)
        {
            tlpMostUsedItems.Controls.Clear();

            //System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            //stopWatch.Start();

            BindingList<ItemInfo> items = null;
            try
            {
                items = _itemManager.GetItemsAllByGroupID(groupId, Program.CounterName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Items by GroupId." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int cnt = 1;
            foreach (ItemInfo item in items)
            {
                if (cnt > itmPageNo * itmPageCount)
                {
                    break;
                }
                if (itmPageNo == 1 || cnt > ((itmPageNo - 1) * itmPageCount))
                {
                    Button itemBtn = new Button();
                    itemBtn.Text = item.DisplayName;
                    itemBtn.Dock = DockStyle.Fill;
                    itemBtn.Tag = item.ItemCode;
                    itemBtn.UseMnemonic = false;
                    itemBtn.ForeColor = Color.Black;
                    itemBtn.Margin = _padding;
                    itemBtn.FlatStyle = FlatStyle.Flat;
                    itemBtn.FlatAppearance.MouseOverBackColor = Color.LightSteelBlue;
                    itemBtn.FlatAppearance.MouseDownBackColor = Color.SteelBlue;

                    itemBtn.Click += new EventHandler(ItemClickEvent);

                    //string imgPath = @"E:\POS\Code\Images\Items\" + item.ID.ToString().Trim() + ".jpg";
                    string imgPath = Application.StartupPath + "\\Images\\Items\\" + item.ID.ToString().Trim() + ".jpg";
                    if (File.Exists(imgPath))
                        itemBtn.BackgroundImage = System.Drawing.Image.FromFile(imgPath);

                    itemBtn.TextAlign = ContentAlignment.BottomCenter;
                    itemBtn.BackgroundImageLayout = ImageLayout.Stretch;

                    tlpMostUsedItems.Controls.Add(itemBtn);
                }
                cnt += 1;
            }

            //stopWatch.Stop();
            //// Get the elapsed time as a TimeSpan value.
            //TimeSpan ts = stopWatch.Elapsed;

            //// Format and display the TimeSpan value.
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //    ts.Hours, ts.Minutes, ts.Seconds,
            //    ts.Milliseconds / 10);
            //lblBillNumber.Text = elapsedTime;

            ////tlpMostUsedItems.ResumeLayout(true);
            ////pnlItemsBack.ResumeLayout();
            ////pnlRightMain.ResumeLayout();

            HighlightCompanyGroup();
        }

        void ItemClickEvent(object sender, EventArgs e)//Add Items To GridView
        {
            if (_fillingMixTopli)
            {
                FillMixTopli(sender, e);
                return;
            }

            if (grdItems.Rows.Count > 0)
            {
                if (Convert.ToDecimal(grdItems.SelectedRows[0].Cells["Quantity"].Value) == 0)
                {
                    tlpQuantityPad.Visible = true;
                    MessageText("Enter quantity for selected item.", false);
                    txtCode.Text = string.Empty;
                    return;
                }
            }

            ItemInfo item = null;
            try
            {
                if (sender.GetType().Name == "Button")
                {
                    item = _itemManager.GetItemByItemCode(Convert.ToInt32(((Button)sender).Tag));
                }
                else
                {
                    int i = 0;
                    if (int.TryParse(sender.ToString(), out i))
                    {
                        item = _itemManager.GetItemByItemCode(Convert.ToInt32(sender.ToString()));
                    }
                    else
                    {
                        item = (ItemInfo)sender;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item by Id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_showItemMultiple)
            {
                if (SearchIfExist(item))
                    return;
            }

            if (item.Name == string.Empty)
            {
                MessageText("Item not found.", false);
                txtCode.Text = string.Empty;
                return;
            }

            int row = grdItems.Rows.Add();
            grdItems.Rows[row].Cells["ItemID"].Value = item.ID;
            grdItems.Rows[row].Cells["ItemCode"].Value = item.ItemCode;
            grdItems.Rows[row].Cells["ItemGroupID"].Value = item.ItemGroupID;
            grdItems.Rows[row].Cells["ItemName"].Value = item.Name;
            grdItems.Rows[row].Cells["Rate"].Value = item.Rate;
            grdItems.Rows[row].Cells["UnitID"].Value = item.UnitID;
            grdItems.Rows[row].Cells["Gst"].Value = item.Gst;

            if (item.UnitID == 2)  //Hardcode (No.)
            {
                grdItems.Rows[row].Cells["Quantity"].Value = 1;
                txtCode.Text = string.Empty;

                CalculateTotal();
            }
            else
            {
                tlpQuantityPad.Visible = tlpNumericPad.Visible = true;
                txtCode.Text = string.Empty;
                lblCode.Text = "Qty:";
            }

            grdItems.Rows[row].Selected = true;
            grdItems.CurrentCell = grdItems[1, row];

            lblMessages.Visible = false;
            txtCode.Focus();
        }

        bool SearchIfExist(ItemInfo item)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToInt32(dr.Cells["ItemID"].Value) == item.ID)
                {
                    if (item.UnitID == 2)
                        dr.Cells["Quantity"].Value = Convert.ToDecimal(dr.Cells["Quantity"].Value) + 1;

                    grdItems.Rows[dr.Index].Selected = true;
                    grdItems.CurrentCell = grdItems[1, dr.Index];
                    CalculateTotal();
                    txtCode.Text = string.Empty;
                    txtCode.Focus();

                    return true;
                }
            }
            return false;
        }

        bool SearchIfExistInMix(ItemInfo item)
        {
            foreach (DataGridViewRow dr in grdMixTopli.Rows)
            {
                if (Convert.ToInt32(dr.Cells["MixItemID"].Value) == item.ID)
                {
                    grdMixTopli.Rows[dr.Index].Selected = true;
                    grdMixTopli.CurrentCell = grdMixTopli[1, dr.Index];

                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Quantity & Numeric Pad Click Events

        private void QuantityPadClick(object sender, EventArgs e)
        {
            if (grdItems.SelectedRows.Count > 0)
            {
                grdItems.SelectedRows[0].Cells["Quantity"].Value = Convert.ToDecimal(Convert.ToInt32(((Button)sender).Tag) / 1000M);
                grdItems.Focus();
            }
            if (_modifyRate)
            {
                if (lblCode.Text != "Rate:")
                {
                    lblCode.Text = "Rate:";
                    tlpQuantityPad.Visible = false;
                }
            }
            else
                ResetForNewItem();

            lblMessages.Visible = false;
        }

        private void NumericPadClick(object sender, EventArgs e)
        {
            txtCode.Text += ((Button)sender).Text;
            txtCode.Select(txtCode.Text.Length, 0);

            if (txtCode.Text.StartsWith(".") && txtCode.Text.Length == 4)
                btnCode_Click(sender, e);

            if (txtCode.Text.Trim().Length > 1 && txtCode.Text.Substring(1, 1) == "." && txtCode.Text.Trim().Length == 5)
                btnCode_Click(sender, e);

            if (txtCode.Text.Trim().Length > 2 && txtCode.Text.Substring(2, 1) == "." && txtCode.Text.Trim().Length == 6)
                btnCode_Click(sender, e);

            lblMessages.Visible = false;
        }

        private void btnHideQty_Click(object sender, EventArgs e)
        {
            if (!grdMixTopli.Visible)
            {
                if (grdItems.Rows.Count == 0)
                    tlpNumericPad.Visible = !tlpNumericPad.Visible;
                else
                    tlpQuantityPad.Visible = tlpNumericPad.Visible = !tlpNumericPad.Visible;
            }
            else
            {
                if (grdMixTopli.Rows.Count == 0)
                    tlpNumericPad.Visible = !tlpNumericPad.Visible;
                else
                    tlpQuantityPad.Visible = tlpNumericPad.Visible = !tlpNumericPad.Visible;
            }

            txtCode.Text = string.Empty;
            lblCode.Text = tlpQuantityPad.Visible ? "Qty:" : lblCode.Text = "Code:";
            txtCode.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtCode.Text = string.Empty;
            txtCode.Focus();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            btnCode_Click(sender, e);
            txtCode.Focus();
        }

        #endregion

        void CalculateTotal()
        {
            decimal totalAmount = 0;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                dr.Cells["Amount"].Value = Convert.ToDecimal(dr.Cells["Quantity"].Value) * Convert.ToDecimal(dr.Cells["Rate"].Value);
                totalAmount += Convert.ToDecimal(dr.Cells["Amount"].Value);
            }

            if (_discountAmountPercent == 0)
                lblTotalAmount.Text = totalAmount.ToString("0.00");
            else
            {
                if (_amtPercent == 1)
                    lblTotalAmount.Text = (totalAmount - _discountAmountPercent).ToString("0.00");
                else if (_amtPercent == 2)
                    lblTotalAmount.Text = (totalAmount - ((totalAmount * _discountAmountPercent) / 100)).ToString("0.00");
            }

            if (_roundAmount)
                lblTotalAmount.Text = Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero).ToString("0.00");

            lblTotalAmount.Tag = totalAmount;
            grdItems.Columns[1].HeaderText = grdItems.Rows.Count == 1 ? grdItems.Rows.Count.ToString() + " Item" : grdItems.Rows.Count.ToString() + " Items";

            if (FormSession == 1)
            {
                if (grdItems.Rows.Count > 0)
                    btnSession1.Text = "Session " + FormSession.ToString() + Environment.NewLine + grdItems.Rows.Count.ToString() + " - " + lblTotalAmount.Text;
                else
                    btnSession1.Text = "Session " + FormSession.ToString();
            }
            else
            {
                if (grdItems.Rows.Count > 0)
                    btnSession2.Text = "Session " + FormSession.ToString() + Environment.NewLine + grdItems.Rows.Count.ToString() + " - " + lblTotalAmount.Text;
                else
                    btnSession2.Text = "Session " + FormSession.ToString();
            }

            //Display Items on Customer Display Code
            _customerDisplayMessage = string.Empty;
            foreach (DataGridViewRow row in grdItems.Rows)
            {
                string itemName = string.Empty;
                if (row.Cells["ItemName"].Value.ToString().Length < 12)
                    itemName = row.Cells["ItemName"].Value.ToString().PadRight(12, ' ');
                else if (row.Cells["ItemName"].Value.ToString().Length > 12)
                    itemName = row.Cells["ItemName"].Value.ToString().Substring(0, 12);
                else
                    itemName = row.Cells["ItemName"].Value.ToString();

                _customerDisplayMessage += itemName + " " + Convert.ToDecimal(row.Cells["Quantity"].Value).ToString("000.000");
            }

            if (_customerDisplayMessage.Length == 0)
            {
                _customerDisplayMessage = _customerDisplayMessage + "   NO ITEMS FOUND   ".PadRight(40, ' ');
            }
            if (_customerDisplayMessage.Length == 20)
            {
                _customerDisplayMessage = _customerDisplayMessage.PadRight(40, ' ');
            }
            else if (_customerDisplayMessage.Length > 40)
            {
                _customerDisplayMessage = _customerDisplayMessage.Substring((grdItems.Rows.Count - 2) * 20, 40);
            }

            Classes.CustomerDisplay.ShowMessage(_customerDisplayMessage);

            tmrClearMessage.Enabled = false;
        }

        void ResetForNewItem()
        {
            tlpQuantityPad.Visible = tlpNumericPad.Visible = false;
            txtCode.Text = string.Empty;
            lblCode.Text = "Code:";
            CalculateTotal();
            txtCode.Focus();
        }

        #region Button Click Events

        private void btnNewBill_Click(object sender, EventArgs e)
        {
            NewBill();
            lblMessages.Visible = false;

            _customerDisplayMessage = "  Welcome to SHREE  " + " KRISHNA DAIRY FARMS";
            Classes.CustomerDisplay.ShowMessage(_customerDisplayMessage);
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            if (_fillingMixTopli)
            {
                if (grdMixTopli.SelectedRows.Count > 0)
                {
                    int deleteRowIndex = grdMixTopli.SelectedRows[0].Index;
                    grdMixTopli.Rows.RemoveAt(deleteRowIndex);

                    if (grdMixTopli.Rows.Count > 0)
                    {
                        if (grdMixTopli.Rows.Count == deleteRowIndex)
                        {
                            grdMixTopli.Rows[deleteRowIndex - 1].Selected = true;
                        }
                        else
                        {
                            grdMixTopli.Rows[deleteRowIndex].Selected = true;
                        }
                    }

                    CalculateAverageRate();
                }
                return;
            }

            if (grdItems.SelectedRows.Count > 0)
            {
                int deleteRowIndex = grdItems.SelectedRows[0].Index;
                grdItems.Rows.RemoveAt(deleteRowIndex);

                if (grdItems.Rows.Count > 0)
                {
                    if (grdItems.Rows.Count == deleteRowIndex)
                    {
                        grdItems.Rows[deleteRowIndex - 1].Selected = true;
                    }
                    else
                    {
                        grdItems.Rows[deleteRowIndex].Selected = true;
                    }
                }

                ResetForNewItem();
            }
        }

        private void btnCode_Click(object sender, EventArgs e) // FillItems from textbox
        {
            CodeClickEvent(sender, e);
        }

        public void CodeClickEvent(object sender, EventArgs e)
        {
            if (_modifyRate)
            {
                if (lblCode.Text != "Rate:")
                {
                    ChangeQuantity(false);
                    lblCode.Text = "Rate:";
                    tlpQuantityPad.Visible = false;
                    txtCode.Text = string.Empty;
                }
                else
                {
                    if (grdItems.SelectedRows.Count > 0 && lblCode.Text == "Rate:")
                    {
                        ChangeRate();
                    }
                }

                return;
            }

            if (_barCodeToReadWriteCard != string.Empty && txtCode.Text == _barCodeToReadWriteCard)
            {
                if (!tlpMostUsedItems.Enabled)
                {
                    btnShowFromCard_Click(sender, e);
                }
                else
                {

                }
                txtCode.Text = string.Empty;
                return;
            }
            if (grdItems.SelectedRows.Count > 0 && lblCode.Text == "Qty:")
            {
                ChangeQuantity(true);
            }
            else
            {
                if (lblCode.Text == "Code:" && tlpMostUsedItems.Enabled)
                {
                    if (btnSave.Text == "Save Coupon")
                    {
                        txtCode.Text = string.Empty;
                        txtCode.Focus();
                        return;
                    }

                    if (txtCode.Text.Trim().Length == 0)
                    {
                        txtCode.Focus();
                        return;
                    }

                    int i = 0;
                    if (txtCode.Text.Length > 4 && txtCode.Text.Substring(0, 4) == "AMAR")                      //Barcode String
                    {
                        if (CheckItemInGrid())
                        {
                            _retVal = _customerManager.GetCustomerForSales(Convert.ToInt32(txtCode.Text.Substring(4, 3))); //Get Customer Code
                            if (_retVal.ID > 0)
                            {
                                BindingList<CustomerItemsInfo> customerItems = _customerManager.GetCustomerItemsForSelectedCustomer(_retVal.ID);

                                lblCustomerName.Text = _retVal.Name + "  Balance :" + _processingManager.GetClosingBalanceofCustomer(Convert.ToInt32(txtCode.Text.Substring(4, 3)));
                                lblLastBillAmount.Text = "No : " + txtCode.Text.Substring(txtCode.Text.Length - 3);
                                foreach (CustomerItemsInfo items in customerItems)
                                {
                                    ItemClickEvent((object)items.ItemCode, e);
                                    grdItems.SelectedRows[0].Cells["Quantity"].Value = items.Quantity;//ItemAdd to grid  
                                }

                                _customerDisplayMessage = _retVal.Name.Length <= 40 ? _retVal.Name.PadRight(40, ' ') : _retVal.Name.Substring(0, 40);
                                Classes.CustomerDisplay.ShowMessage(_customerDisplayMessage);
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(1500);

                                CalculateTotal();

                                tlpQuantityPad.Visible = tlpNumericPad.Visible = btnCoupon.Enabled = false;
                            }
                            else
                            {
                                MessageText("Customer Not Found...", false);
                                txtCode.Text = string.Empty;
                            }
                        }
                        lblCode.Text = "Code:";
                    }
                    else
                    {
                        if (txtCode.Text.Trim().Length <= 4 && int.TryParse(txtCode.Text, out i))   //Hardcode
                        {
                            ItemClickEvent((object)txtCode.Text, e);
                        }
                        else
                        {
                            ItemInfo item = _itemManager.GetItemByBarCode(txtCode.Text.Trim());
                            if (item.Name != string.Empty)
                            {
                                ItemClickEvent((object)item, e);
                            }
                            else
                            {
                                MessageText("Item not found.", false);
                                txtCode.SelectAll();
                            }
                        }
                    }
                }
                else
                {
                    MessageText("First click on Show Items from Card button.", false);
                    txtCode.SelectAll();
                }
            }
            txtCode.Focus();
        }

        //public bool ShowMessage(string message)
        //{
        //    if (message.Length != 40)
        //        return false;

        //    try
        //    {
        //        lblTempMessages.Text = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        //System.Windows.Forms.MessageBox.Show("Error Customer Display: " + ex.Message);
        //        return false;
        //    }

        //    return true;
        //}

        private void ChangeQuantity(bool resetForNewItem)
        {
            if (txtCode.Text.Trim().Length > 7)
            {
                MessageText("Enter valid quantity.", false);
            }
            else
            {
                decimal qty;
                if (decimal.TryParse(txtCode.Text, out qty))
                {
                    if (resetForNewItem)
                    {
                        grdItems.SelectedRows[0].Cells["Quantity"].Value = qty;
                        ResetForNewItem();
                    }
                    else
                        CalculateTotal();
                }
                else
                {
                    MessageText("Enter valid quantity.", false);
                }
            }
        }

        private void ChangeRate()
        {
            if (txtCode.Text.Trim().Length > 7)
            {
                MessageText("Enter valid rate.", false);
            }
            else
            {
                decimal rate;
                if (decimal.TryParse(txtCode.Text, out rate))
                {
                    grdItems.SelectedRows[0].Cells["Rate"].Value = rate;
                    ResetForNewItem();
                }
                else if (txtCode.Text.Trim() == string.Empty)
                {
                    ResetForNewItem();
                }
                else
                {
                    MessageText("Enter valid rate.", false);
                }
            }
        }

        #endregion

        private void lblExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnBackToRecent_Click(object sender, EventArgs e)
        {
            itmPageNo = 1;
            LoadMostUsedItems();
        }

        #region Topli...

        void FillMixTopli(object sender, EventArgs e)
        {
            ItemInfo item = null;
            try
            {
                if (sender.GetType().Name == "Button")
                {
                    item = _itemManager.GetItemByItemCode(Convert.ToInt32(((Button)sender).Tag));
                }
                else
                {
                    int i = 0;
                    if (int.TryParse(sender.ToString(), out i))
                    {
                        item = _itemManager.GetItem(Convert.ToInt32(sender.ToString()));
                    }
                    else
                    {
                        item = (ItemInfo)sender;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Item by Id." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (SearchIfExistInMix(item))
                return;

            if (item.Name == string.Empty)
            {
                MessageText("Item not found.", false);
                txtCode.Text = string.Empty;
                return;
            }

            int row = grdMixTopli.Rows.Add();
            grdMixTopli.Rows[row].Cells["MixItemID"].Value = item.ID;
            grdMixTopli.Rows[row].Cells["MixItemGroupID"].Value = item.ItemGroupID;
            grdMixTopli.Rows[row].Cells["MixItemName"].Value = item.Name;
            grdMixTopli.Rows[row].Cells["MixRate"].Value = item.Rate;

            grdMixTopli.Rows[row].Selected = true;
            grdMixTopli.CurrentCell = grdMixTopli[1, row];

            CalculateAverageRate();

            lblMessages.Visible = false;
        }

        void CalculateAverageRate()
        {
            if (grdMixTopli.Rows.Count > 0)
            {
                decimal total = 0;
                int cnt = 0;
                foreach (DataGridViewRow dr in grdMixTopli.Rows)
                {
                    cnt += 1;
                    total += Convert.ToDecimal(dr.Cells["MixRate"].Value);
                }
                lblAverageRate.Text = (total / cnt).ToString("0.00");
            }
            else
                lblAverageRate.Text = "0.00";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _fillingMixTopli = false;
            pnlMixTopli.Visible = false;
            btnHideQty.Enabled = true;
            if (grdMixTopli.Rows.Count > 0)
            {
                int row = grdItems.Rows.Add();
                grdItems.Rows[row].Cells["ItemID"].Value = 107;   //Hardcode
                grdItems.Rows[row].Cells["ItemName"].Value = "Mix Dryfruit Mawa";   //Hardcode
                grdItems.Rows[row].Cells["Rate"].Value = lblAverageRate.Text;
                grdItems.Rows[row].Cells["Amount"].Value = "0.00";

                tlpQuantityPad.Visible = true;
                txtCode.Text = string.Empty;
                lblCode.Text = "Qty:";
                grdItems.Rows[row].Selected = true;
                grdItems.CurrentCell = grdItems[1, row];
            }
        }

        #endregion

        private void btnGroupPrevious_Click(object sender, EventArgs e)
        {
            grpPageNo -= 1;
            LoadGroups(lastClickedCompanyID);
        }

        private void btnGroupNext_Click(object sender, EventArgs e)
        {
            grpPageNo += 1;
            LoadGroups(lastClickedCompanyID);
        }

        private void btnItemPrevious_Click(object sender, EventArgs e)
        {
            itmPageNo -= 1;
            LoadItemsByGroupID(lastClickedGroupID);
        }

        private void btnItemNext_Click(object sender, EventArgs e)
        {
            itmPageNo += 1;
            LoadItemsByGroupID(lastClickedGroupID);
        }

        private bool ValidateControls()
        {
            foreach (DataGridViewRow row in grdItems.Rows)
            {
                if (Convert.ToDecimal(row.Cells["Amount"].Value) == 0)
                {
                    MessageText("Invalid item in the list.", false);
                    return false;
                }
            }

            return true;
        }

        SaleInfo SetValue(out BindingList<SaleItemInfo> saleItems)
        {
            SaleInfo retVal = new SaleInfo();
            BindingList<SaleItemInfo> saleItemss = new BindingList<SaleItemInfo>();

            retVal.BillDate = DateTime.Today.Date;
            try
            {
                retVal.BillNo = _saleManager.GetNextBillNumber().ToString();
                _billNumber = Convert.ToInt32(retVal.BillNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting bill number." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                saleItems = null;
                return null;
            }
            retVal.CashCredit = 0;

            retVal.TotalAmount = Convert.ToDecimal(lblTotalAmount.Text);
            retVal.DiscountPercentage = 0;
            retVal.DiscountAmount = 0;
            retVal.NetAmount = Convert.ToDecimal(lblTotalAmount.Text);
            retVal.RoundedAmount = Convert.ToDecimal(lblTotalAmount.Text);

            retVal.BalanceAmount = 0;
            retVal.CustomerID = _retVal.ID;
            retVal.Description = string.Empty;
            retVal.RFIDTransaction = true;
            retVal.IsPrint = true;
            retVal.TotalWeight = retVal.ActualWeight = 0;
            retVal.DivisionID = Program.DivisionID;
            retVal.CreatedBy = Program.CURRENTUSER;
            retVal.CreatedOn = DateTime.Now;
            retVal.UpdatedBy = Program.CURRENTUSER;
            retVal.UpdatedOn = DateTime.Now;

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                SaleItemInfo saleItem = new SaleItemInfo();
                saleItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
                saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                saleItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                saleItem.Vat = Convert.ToDecimal(dr.Cells["Vat"].Value);
                saleItem.Amount = Convert.ToDecimal(dr.Cells["Amount"].Value);

                saleItemss.Add(saleItem);
            }

            saleItems = saleItemss;
            return retVal;
        }

        bool Save(out bool isBlankCard, out Int64 billNo)
        {
            if (grdItems.Rows.Count == 0)
            {
                isBlankCard = true;
                billNo = 0;
                return false;
            }

            BindingList<SaleItemInfo> saleItems = null;
            SaleInfo sale = SetValue(out saleItems);

            Int64 billNumber = 0;
            try
            {
                _newRecordID = _saleManager.AddSale(sale, saleItems, Program.CounterName, out billNumber);
                billNo = billNumber;

                isBlankCard = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                isBlankCard = false;
                billNo = 0;
                return false;
            }
        }

        void Print(Int64 billNumber)
        {
            ReportClass rptClass = new ReportClass();
            if (Program.SHOWSHORTBILL)
            {
                rptClass.ShowBillChitale(billNumber, false, false);
            }
            else
            {
                rptClass.ShowBill(billNumber, false, false);
            }
        }

        private void txtCode_Click(object sender, EventArgs e)
        {
            if (grdItems.Rows.Count > 0)
            {
                if (lblCode.Text == "Code:")
                {
                    lblCode.Text = "Qty:";
                    tlpQuantityPad.Visible = tlpNumericPad.Visible = true;
                }
                else if (Convert.ToString(grdItems.SelectedRows[0].Cells["Quantity"].Value).Trim() != string.Empty)
                {
                    lblCode.Text = "Code:";
                    tlpQuantityPad.Visible = false;
                    tlpNumericPad.Visible = true;
                }
            }
            else
            {
                tlpNumericPad.Visible = true;
                lblCode.Text = "Code:";
            }
        }

        private void btnShowFixed_Click(object sender, EventArgs e)
        {
            LoadFixedItems();
        }

        private void btnShowFromCard_Click(object sender, EventArgs e)
        {
            string data = string.Empty;
            try
            {
                btnShowFromCard.Text = "Reading...";
                btnShowFromCard.Enabled = false;
                Application.DoEvents();

                _cardID = _c.ReadCardID();

                data = _c.ReadBlock(_key, 1);
                Application.DoEvents();
                int itemCount = 0;
                if (data.Trim().Length >= 17)
                {
                    int i;
                    if (!int.TryParse(data.Substring(15, 2), out i))
                    {
                        MessageText("Invalid data.", false);
                        return;
                    }
                    else
                        itemCount = Convert.ToInt32(data.Substring(15, 2));

                    if (itemCount == 0)
                    {
                        MessageText("No items found.", true);
                        tlpMostUsedItems.Enabled = true;
                        return;
                    }
                }
                else
                {
                    MessageText("Card not found.", false);
                    return;
                }

                BindingList<ReadItemInfo> readItems = _c.ReadItems(_key, itemCount);
                if (readItems.Count != itemCount)
                {
                    MessageText("Missing items.", false);
                    return;
                }

                bool goAhead = true;

                foreach (ReadItemInfo item in readItems)
                {
                    if (item.Data.Trim().Length < 15)
                    {
                        MessageText("Invalid data in items. (" + item.Data.Trim() + ")", false);
                        goAhead = false;
                        break;
                    }

                    int itemId;
                    if (!int.TryParse(item.Data.Substring(5, 3), out itemId))
                    {
                        MessageText("Invalid item code. (" + item.Data.Substring(5, 3).Trim() + ")", false);
                        goAhead = false;
                        break;
                    }

                    decimal qty;
                    if (!decimal.TryParse(item.Data.Substring(8, 7), out qty))
                    {
                        MessageText("Invalid quantity. (" + item.Data.Substring(8, 7).Trim() + ")", false);
                        goAhead = false;
                        break;
                    }

                    ItemInfo i = _itemManager.GetItemByItemCode(itemId);
                    int row = grdItems.Rows.Add();

                    if (i.Name.Trim() == string.Empty)
                    {
                        grdItems.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                        grdItems.Rows[row].Cells["ItemID"].Value = itemId.ToString();
                        grdItems.Rows[row].Cells["ItemCode"].Value = "0";
                        grdItems.Rows[row].Cells["ItemGroupID"].Value = "0";
                        grdItems.Rows[row].Cells["ItemName"].Value = "Invalid Item Code";
                        grdItems.Rows[row].Cells["Quantity"].Value = qty;
                        grdItems.Rows[row].Cells["Rate"].Value = "0.00";
                    }
                    else
                    {
                        grdItems.Rows[row].DefaultCellStyle.ForeColor = Color.Maroon;
                        grdItems.Rows[row].DefaultCellStyle.SelectionForeColor = Color.White;
                        grdItems.Rows[row].Cells["ItemID"].Value = i.ID;
                        grdItems.Rows[row].Cells["ItemCode"].Value = i.ItemCode;
                        grdItems.Rows[row].Cells["ItemGroupID"].Value = i.ItemGroupID;
                        grdItems.Rows[row].Cells["ItemName"].Value = i.Name;
                        grdItems.Rows[row].Cells["UnitID"].Value = i.UnitID;
                        grdItems.Rows[row].Cells["Quantity"].Value = qty.ToString("0.000");
                        grdItems.Rows[row].Cells["Rate"].Value = i.Rate.ToString("0.00");
                        grdItems.Rows[row].Cells["Gst"].Value = i.Gst;
                    }

                    lblMessages.Visible = false;
                }

                if (!goAhead)
                {
                    return;
                }

                CalculateTotal();
                tlpMostUsedItems.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageText(ex.Message, false);
            }
            finally
            {
                btnShowFromCard.Text = "&Show Items From Card";
                btnShowFromCard.Enabled = true;
                txtCode.Focus();
            }
        }

        private void lblTotalAmount_Click(object sender, EventArgs e)
        {

        }

        private void txtCode_Enter(object sender, EventArgs e)
        {
            txtCode.Text = string.Empty;
        }

        private void grdItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtCode.Text = string.Empty;
            txtCode.Focus();
        }

        private void grdItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnShowLastBills_Click(object sender, EventArgs e)
        {
            frmShowLastBillsForPrint frm = new frmShowLastBillsForPrint();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                txtCode.Focus();
            }
        }

        private void lblMessages_Click(object sender, EventArgs e)
        {
            lblMessages.Visible = false;
        }

        private void lblLastBillAmount_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (_billNumber > 0)
                {
                    Print(_billNumber);
                    _saleManager.UpdateSalePrint(_billNumber);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing sale bill." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void frmSale_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (grdItems.Rows.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Items exist in grid, Are you sure want to exit?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            bool formSession2Open = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "frmSale")
                {
                    if (((frmSale)form).FormSession == 2)
                    {
                        if (((frmSale)form).grdItems.Rows.Count > 0)
                            formSession2Open = true;
                    }
                }
            }
            if (((frmSale)sender).FormSession == 1 && formSession2Open)
            {
                MessageBox.Show("Close Session 2 first.", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        private void btnSession1_Click(object sender, EventArgs e)
        {
            string btnText2 = string.Empty;
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "frmSale")
                {
                    if (((frmSale)form).FormSession == 2)
                    {
                        btnText2 = ((frmSale)form).btnSession2.Text;
                    }
                }
            }

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "frmSale")
                {
                    if (((frmSale)form).FormSession == 1)
                    {
                        form.Activate();
                        ((frmSale)form).btnSession2.Text = btnText2;
                        return;
                    }
                }
            }
        }

        private void btnSession2_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "frmSale")
                {
                    if (((frmSale)form).FormSession == 1)
                    {
                        btnSession1.Text = ((frmSale)form).btnSession1.Text;
                    }
                }
            }

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "frmSale")
                {
                    if (((frmSale)form).FormSession == 2)
                    {
                        form.Activate();
                        ((frmSale)form).CalculateTotal();
                        return;
                    }
                }
            }

            frmSale frm = new frmSale(2);
            frm.Show();
        }

        #region Save and Print

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePrintBill(false, false, sender, e);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            SavePrintBill(true, false, sender, e);
        }

        private void btnCoupon_Click(object sender, EventArgs e)
        {
            SavePrintBill(Convert.ToBoolean(_settingManager.GetSetting(35)), true, sender, e);
        }

        private void SavePrintBill(bool isPrint, bool isCouponSale, object sender, EventArgs e)
        {
            tmrCustomerDisplay.Enabled = false;

            //Check whether items present in the grid
            if (grdItems.Rows.Count == 0)
            {
                MessageText("No items found.", false);
                return;
            }

            //Check for zero quantity
            bool validationFailed = false;
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (Convert.ToDecimal(dr.Cells["Amount"].Value) == 0)
                {
                    dr.Selected = true;
                    validationFailed = true;
                    break;
                }
            }
            if (validationFailed)
            {
                MessageText("Quantity not entered.", false);
                return;
            }
            SaleInfo sale = new SaleInfo();
            BindingList<SaleItemInfo> saleItems = SetValue(isPrint, isCouponSale, out sale);
            try
            {
                Int64 billNumber = 0;

                _newRecordID = _saleManager.AddSale(sale, saleItems, Program.CounterName, out billNumber);

                if (isPrint)
                    MessageText("Saved & Printed Successfully.", true);
                else
                    MessageText("Saved successfully.", true);

                //lblLastBillAmount.Text = "Last Bill Amt:   " + lblTotalAmount.Text;

                if (isPrint && saleItems.Count > 0)
                {
                    ReportClass rptClass = new ReportClass();
                    rptClass.ShowBill(billNumber, false, _discountAmountPercent > 0);
                }

                txtCode.Focus();
                lblCustomerName.Text = string.Empty;

                tmrCustomerDisplay.Enabled = true;
                tmrCustomerDisplay.Tick += new EventHandler(tmrCustomerDisplay_Tick);
                Classes.CustomerDisplay.ShowMessage("Bill Amount:" + lblTotalAmount.Text.Trim().PadLeft(8, ' ') + "Thank U Visit Again!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale Items." + Environment.NewLine + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
            string temp = string.Empty;
            if ((Program.SystemOperatingMode == EnumClass.SystemOperatingMode.ChitaleRFID || Program.SystemOperatingMode == EnumClass.SystemOperatingMode.TouchScreenWithRFID) && !c.ResetItems(Program.KEYSET, out temp, 0))
            {
                MessageBox.Show("Show card again. Items not cleared in the card.");
            }
            else
            {
                NewBill();
            }
        }
        void tmrCustomerDisplay_Tick(object sender, EventArgs e)
        {
            Classes.CustomerDisplay.ShowMessage("  Welcome to SHREE  " + " KRISHNA DAIRY FARMS");
        }

        BindingList<SaleItemInfo> SetValue(bool isPrint, bool isCouponSale, out SaleInfo sales)
        {
            SaleInfo sale = new SaleInfo();

            sale.BillDate = DateTime.Today.Date;
            try
            {
                sale.BillNo = _saleManager.GetNextBillNumber().ToString();
                _billNumber = Convert.ToInt32(sale.BillNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting bill number." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sales = null;
                return null;
            }
            sale.CashCredit = 0;    //Hardcode
            //sale.CustomerID = (lblLastBillAmount.Text.Length > 4) ? Convert.ToInt32(lblLastBillAmount.Text.Substring(4)) :0;
            sale.CustomerNumber = (lblLastBillAmount.Text.Length > 4) ? Convert.ToInt32(lblLastBillAmount.Text.Substring(4)) : 0;
            sale.TotalAmount = Convert.ToDecimal(lblTotalAmount.Tag);
            sale.NetAmount = Convert.ToDecimal(lblTotalAmount.Text);
            sale.RoundedAmount = Convert.ToDecimal(lblTotalAmount.Text);
            sale.IsPrint = isPrint;
            sale.IsCouponSale = isCouponSale;
            if (_discountAmountPercent > 0)
            {
                if (_amtPercent == 1)
                {
                    sale.DiscountPercentage = (_discountAmountPercent * 100) / Convert.ToDecimal(lblTotalAmount.Tag);
                    sale.DiscountAmount = _discountAmountPercent;
                }
                else if (_amtPercent == 2)
                {
                    sale.DiscountPercentage = _discountAmountPercent;
                    sale.DiscountAmount = Convert.ToDecimal(lblTotalAmount.Tag) * _discountAmountPercent / 100;
                }
            }
            sale.RFIDTransaction = true;
            sale.DivisionID = Program.DivisionID;
            sale.IsProcessed = false;

            sale.CreatedBy = Program.CURRENTUSER;
            sale.CreatedOn = DateTime.Now;
            sale.UpdatedBy = Program.CURRENTUSER;
            sale.UpdatedOn = DateTime.Now;

            BindingList<SaleItemInfo> retVal = new BindingList<SaleItemInfo>();

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                SaleItemInfo saleItem = new SaleItemInfo();
                saleItem.ItemID = Convert.ToInt32(dr.Cells["ItemID"].Value);
                saleItem.UnitID = Convert.ToInt32(dr.Cells["UnitID"].Value);
                saleItem.Quantity = Convert.ToDecimal(dr.Cells["Quantity"].Value);
                saleItem.Rate = Convert.ToDecimal(dr.Cells["Rate"].Value);
                saleItem.Vat = Convert.ToDecimal(dr.Cells["Vat"].Value);
                saleItem.Amount = Convert.ToDecimal(dr.Cells["Amount"].Value);

                retVal.Add(saleItem);
            }

            sales = sale;
            return retVal;
        }

        #endregion

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
            if (e.KeyCode == Keys.F10)
            {
                if (CheckItemInGrid())
                {
                    NewBill();
                    tmrClock.Enabled = false;
                    frmGetCustomer frm = new frmGetCustomer(this);
                    if (frm.ShowDialog() == DialogResult.OK)
                        tmrClock.Enabled = true;
                }
            }
            if (e.KeyCode == Keys.F11)
            {
                frmSaleProcessing frm = new frmSaleProcessing(3);
                frm.ShowDialog();
            }
        }
        private bool CheckItemInGrid()
        {
            if (grdItems.Rows.Count > 0)
            {
                MessageText("Previous Items Not Saved.", false);
                txtCode.Text = string.Empty;
                return false;
            }
            else
                return true;
        }
    }
}