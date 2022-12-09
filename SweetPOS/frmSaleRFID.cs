using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

using BusinessLogic;
using DataObjects;
using ChitalePersonalzer;
using System.Drawing.Printing;
using System.Threading;

namespace SweetPOS
{
    public partial class frmSaleRFID : Form
    {
        #region Class level variables...

        SerialPort _serialPort;

        SaleManager _saleManager = new SaleManager();
        ItemManager _itemManager = new ItemManager();
        SettingManager _settingManager = new SettingManager();

        int _newRecordID = 0;

        long _hideMessageCounter = 6;

        //For round off amount & Animation
        bool _roundAmount = false;
        decimal _round50 = 0;
        decimal _round1 = 0;
        bool _showAnimation = false;

        bool _showWeight = false;
        int _allowWeightDifference = 0;

        bool _dontSaveIfNoWeight = false;

        string _amountForChange = string.Empty;

        #endregion

        Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
        string key = Program.KEYSET;

        string _oldTotal = "0";

        public frmSaleRFID()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - RFID Sale Entry";
            lblHeading.Text = Program.COMPANYNAME;
            LoadSettings();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void frmSaleRFID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{Tab}");
            }
        }

        private void frmSaleRFID_KeyDown(object sender, KeyEventArgs e)
        {
            if (chkPrintDirectly.Visible && e.KeyValue == 109)
            {
                if (grdItems.Rows.Count > 0)
                    Print(Convert.ToInt64(txtBillNo.Text));
                else
                    MessageText("No Items found to print.", Color.Maroon);
            }
            if (e.KeyValue == 107)
            {
                if (!lblSumPrevious.Visible)
                {
                    lblSumPrevious.Visible = true;
                    lblSumPrevious.Text = (Convert.ToDecimal(lblNetAmount.Text) + Convert.ToDecimal(txtPreviousBill.Text)).ToString("0.00");
                }
                else if (!lblSumPrevious1.Visible)
                {
                    lblSumPrevious1.Visible = true;
                    lblSumPrevious1.Text = (Convert.ToDecimal(lblSumPrevious.Text) + Convert.ToDecimal(txtPreviousBill1.Text)).ToString("0.00");
                }
                else if (!lblSumPrevious2.Visible)
                {
                    lblSumPrevious2.Visible = true;
                    lblSumPrevious2.Text = (Convert.ToDecimal(lblSumPrevious1.Text) + Convert.ToDecimal(txtPreviousBill2.Text)).ToString("0.00");
                }
            }
            if (e.KeyCode == Keys.F9)
            {
                e.Handled = true;
                pnlCardPaymentDetails.Visible = !pnlCardPaymentDetails.Visible;
                if (pnlCardPaymentDetails.Visible)
                    txtCardPaymentDetails.Focus();
                else
                    btnShowBill.Focus();
            }
        }

        private void frmSaleRFID_Load(object sender, EventArgs e)
        {
            try
            {
                _serialPort.Open();
                tmrWeight.Enabled = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error opening weighing scale port." + Environment.NewLine + ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            New();
            btnShowBill.Focus();
        }

        private void LoadSettings()
        {
            try
            {
                _roundAmount = Convert.ToBoolean(_settingManager.GetSetting(7));         //Hardcode

                _round50 = Convert.ToDecimal(_settingManager.GetSetting(8));            //Hardcode

                _round1 = Convert.ToDecimal(_settingManager.GetSetting(9));             //Hardcode

                _showAnimation = Convert.ToBoolean(_settingManager.GetSetting(10));     //Hardcode

                _showWeight = Convert.ToBoolean(_settingManager.GetSetting(15));     //Hardcode

                chkPrintDirectly.Visible = Convert.ToBoolean(_settingManager.GetSetting(12));   //Hardcode

                chkPrintDirectly.Checked = Convert.ToBoolean(_settingManager.GetSetting(13));   //Hardcode

                _allowWeightDifference = Convert.ToInt32(_settingManager.GetSetting(17));         //HardCode

                string weighingCOMPort = _settingManager.GetSetting(18);         //HardCode

                string weighingCOMPortBaudRate = _settingManager.GetSetting(19);     //HardCode

                _serialPort = new SerialPort(weighingCOMPort, Convert.ToInt32(weighingCOMPortBaudRate));

                _dontSaveIfNoWeight = Convert.ToBoolean(_settingManager.GetSetting(20));   //Hardcode
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting Setting." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!_showWeight)
            {
                pnlShowWeight.Visible = false;
                pnlNetAmount.Location = new Point(504, 268);
            }
            if (_roundAmount)
            {
                lblRoundOff.Visible = true;
            }
        }

        bool ValidateFields()
        {


            return true;
        }

        SaleInfo SetValue(out BindingList<SaleItemInfo> saleItems)
        {
            SaleInfo retVal = new SaleInfo();
            BindingList<SaleItemInfo> saleItemss = new BindingList<SaleItemInfo>();

            retVal.BillDate = dtBillDate.Value.Date;
            retVal.BillNo = Convert.ToString(txtBillNo.Text);
            retVal.CashCredit = 0;

            retVal.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text);
            retVal.DiscountPercentage = 0;
            retVal.DiscountAmount = 0;
            retVal.NetAmount = Convert.ToDecimal(lblNetAmount.Text) + Convert.ToDecimal(lblRoundOff.Text);
            retVal.RoundedAmount = Convert.ToDecimal(lblNetAmount.Text);

            retVal.BalanceAmount = 0;
            retVal.CustomerID = 0;

            retVal.Description = string.Empty;

            retVal.RFIDTransaction = true;

            retVal.IsPrint = chkPrintDirectly.Checked;

            retVal.TotalWeight = Convert.ToDecimal(lblTotalWeight.Text);
            retVal.ActualWeight = Convert.ToDecimal(lblActualWeight.Text);

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

        bool Save(out Int64 billNo)
        {
            if (!ValidateFields())
            {
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

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in saving Sale." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                billNo = 0;
                return false;
            }
        }

        bool New()
        {
            ChangePreviousBill();

            grdItems.Rows.Clear();
            lblMessages.Visible = lblCardBlank.Visible = false;
            lblSumPrevious.Visible = lblSumPrevious1.Visible = lblSumPrevious2.Visible = false;
            lblSumPrevious.Text = lblSumPrevious1.Text = lblSumPrevious2.Text = "0.00";
            pnlPreviousBillItems.Visible = false;

            try
            {
                txtBillNo.Text = _saleManager.GetNextBillNumber().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting next bill number." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            CalculateTotals();

            lblTotalWeight.Text = lblActualWeight.Text = "0.000";
            _amountForChange = string.Empty;

            return true;
        }

        private void ChangePreviousBill()
        {
            if (Convert.ToDecimal(lblNetAmount.Text) > 0)
            {
                txtPreviousBill2.Text = txtPreviousBill1.Text;
                txtPreviousBill2.Tag = txtPreviousBill1.Tag;

                txtPreviousBill1.Text = txtPreviousBill.Text;
                txtPreviousBill1.Tag = txtPreviousBill.Tag;

                txtPreviousBill.Text = lblNetAmount.Text;
                txtPreviousBill.Tag = SavePreviousBillToShowOnHover();
            }
        }

        private BindingList<PreviousBillItems> SavePreviousBillToShowOnHover()
        {
            BindingList<PreviousBillItems> pbItems = new BindingList<PreviousBillItems>();

            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                PreviousBillItems pbItem = new PreviousBillItems();
                pbItem.SrNo = dr.Cells["SrNo"].Value.ToString();
                pbItem.Code = dr.Cells["ItemCode"].Value.ToString();
                pbItem.ItemName = dr.Cells["ItemName"].Value.ToString();
                pbItem.Quantity = dr.Cells["Quantity"].Value.ToString();
                pbItem.Unit = dr.Cells["Unit"].Value.ToString();
                pbItem.Rate = dr.Cells["Rate"].Value.ToString();
                pbItem.Amount = dr.Cells["Amount"].Value.ToString();

                pbItems.Add(pbItem);
            }

            return pbItems;
        }

        void CloseForm()
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            New();
            btnShowBill.Focus();
        }

        ItemInfo GetItemInfo(int itemID)
        {
            ItemInfo retVal = null;
            try
            {
                retVal = _itemManager.GetItem(itemID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting item by id." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return retVal;
        }

        void CalculateTotals()
        {
            try
            {
                txtTaxAmount.Text = lblNetAmount.Text = txtTotalAmount.Text = "0.00";
                decimal totalAmount = 0;
                decimal totalTax = 0;
                foreach (DataGridViewRow dr in grdItems.Rows)
                {
                    decimal amt = Convert.ToDecimal(dr.Cells["Quantity"].Value) * Convert.ToDecimal(dr.Cells["Rate"].Value);
                    dr.Cells["Amount"].Value = amt.ToString("0.00");
                    totalAmount += amt;

                    decimal tax = amt - ((amt * 100) / (100 + Convert.ToDecimal(dr.Cells["Vat"].Value)));
                    totalTax += tax;
                }
                txtTotalAmount.Text = (totalAmount - totalTax).ToString("0.00");
                txtTaxAmount.Text = totalTax.ToString("0.00");
                lblNetAmount.Text = (totalAmount).ToString("0.00");

                if (_roundAmount)
                {
                    //RoundTotalAmount(totalAmount);
                    lblNetAmount.Text = Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero).ToString("0.00");
                }

                if (_showWeight)
                    ShowQuantityAndActualQuantity();
            }
            catch (Exception ex)
            {
                MessageText("Error in calculating totals.", Color.Red);
            }
        }

        private void ShowQuantityAndActualQuantity()
        {
            if (grdItems.Rows.Count == 0)
                return;

            decimal totalWeight = 0;
            foreach (DataGridViewRow r in grdItems.Rows)
            {
                if (r.Cells["Unit"].Value.ToString().Trim() == "No.")
                {
                    decimal unitWeight = 0;
                    try
                    {
                        ItemManager itemManager = new ItemManager();
                        ItemInfo item = new ItemInfo();
                        item = itemManager.GetItem(Convert.ToInt32(r.Cells["ItemID"].Value));
                        unitWeight = item.UnitWeight;
                    }
                    catch (Exception ex)
                    {
                        MessageText(ex.Message, Color.Red);
                        return;
                    }
                    totalWeight += Convert.ToDecimal(r.Cells["Quantity"].Value) * unitWeight;
                }
                else
                {
                    totalWeight += Convert.ToDecimal(r.Cells["Quantity"].Value);
                }
            }
            lblTotalWeight.Text = totalWeight.ToString("0.000");

            decimal actualWeight = Convert.ToDecimal(lblCurrentWeight.Text);

            decimal lessPercentAllowed = Convert.ToDecimal(totalWeight) - ((Convert.ToDecimal(totalWeight) * _allowWeightDifference) / 100);
            decimal morePercentAllowed = Convert.ToDecimal(totalWeight) + ((Convert.ToDecimal(totalWeight) * _allowWeightDifference) / 100);

            lblActualWeight.Text = lblCurrentWeight.Text;
            //if (actualWeight >= lessPercentAllowed && actualWeight <= morePercentAllowed)
            if (actualWeight <= morePercentAllowed)
                lblActualWeight.BackColor = Color.DarkGreen;
            else
                lblActualWeight.BackColor = Color.Red;
        }

        private void RoundTotalAmount(decimal totalAmount)
        {
            decimal roundAmount;
            decimal roundOffAmount;

            int i = (int)totalAmount;
            decimal paise = totalAmount - i;

            if (paise > 0)
            {
                if (paise > _round1)
                {
                    roundAmount = decimal.Round(totalAmount, 0);
                    roundOffAmount = totalAmount - roundAmount;
                }
                else if (paise > _round50 && paise <= _round1)
                {
                    roundAmount = i + 0.50M;
                    roundOffAmount = totalAmount - roundAmount;
                }
                else
                {
                    roundAmount = (decimal)i;
                    roundOffAmount = totalAmount - roundAmount;
                }
                lblNetAmount.Text = roundAmount.ToString("0.00");
                lblRoundOff.Text = roundOffAmount.ToString("0.00");
            }
            else
            {
                lblNetAmount.Text = totalAmount.ToString("0.00");
                lblRoundOff.Text = "0.00";
            }
        }

        bool ShowAndReset()
        {
            btnShowBill.Enabled = false;
            Application.DoEvents();

            if (!New())
                return false;

            MessageText("Reading card, please wait...", Color.DarkOliveGreen);

            string data = string.Empty;
            try
            {
                data = c.ReadBlock(key, 1);
                Application.DoEvents();
                int itemCount = 0;
                if (data.Trim().Length >= 17)
                {
                    int i;
                    if (!int.TryParse(data.Substring(15, 2), out i))
                    {
                        MessageText("Invalid data.", Color.OrangeRed);
                        return false;
                    }
                    else
                        itemCount = Convert.ToInt32(data.Substring(15, 2));

                    if (itemCount == 0)
                    {
                        MessageText("Blank Card", Color.Green);
                        return false;
                    }
                }
                else
                {
                    MessageText("Card not found.", Color.Red);
                    return false;
                }

                BindingList<ReadItemInfo> readItems = c.ReadItems(key, itemCount);
                if (readItems.Count != itemCount)
                {
                    MessageText("Missing items.", Color.OrangeRed);
                    return false;
                }

                bool goAhead = true;

                foreach (ReadItemInfo item in readItems)
                {
                    if (item.Data.Trim().Length < 15)
                    {
                        MessageText("Invalid data in items. (" + item.Data.Trim() + ")", Color.OrangeRed);
                        goAhead = false;
                        break;
                    }

                    int itemId;
                    decimal qty;
                    if (item.Data.Length > 20 && item.Data.Substring(20, 1) == "B")
                    {
                        if (!int.TryParse(item.Data.Substring(5, 4), out itemId))
                        {
                            MessageText("Invalid item code. (" + item.Data.Substring(5, 4).Trim() + ")", Color.OrangeRed);
                            goAhead = false;
                            break;
                        }

                        //if (!decimal.TryParse(item.Data.Substring(9, 7), out qty))
                        //{
                        //    MessageText("Invalid quantity. (" + item.Data.Substring(9, 7).Trim() + ")", Color.OrangeRed);
                        //    goAhead = false;
                        //    break;
                        //}
                        if (!decimal.TryParse(item.Data.Substring(9, 3) + "." + item.Data.Substring(12, 3), out qty))
                        {
                            MessageText("Invalid quantity. (" + item.Data.Substring(9, 6).Trim() + ")", Color.OrangeRed);
                            goAhead = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!int.TryParse(item.Data.Substring(5, 3), out itemId))
                        {
                            MessageText("Invalid item code. (" + item.Data.Substring(5, 3).Trim() + ")", Color.OrangeRed);
                            goAhead = false;
                            break;
                        }

                        if (!decimal.TryParse(item.Data.Substring(8, 7), out qty))
                        {
                            MessageText("Invalid quantity. (" + item.Data.Substring(8, 7).Trim() + ")", Color.OrangeRed);
                            goAhead = false;
                            break;
                        }
                    }                    

                    ItemInfo i = _itemManager.GetItemByItemCode(itemId);
                    int row = grdItems.Rows.Add();
                    grdItems.Rows[row].Cells["SrNo"].Value = (row + 1).ToString();
                    if (i.Name.Trim() == string.Empty)
                    {
                        grdItems.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                        grdItems.Rows[row].Cells["ItemCode"].Value = itemId.ToString();
                        grdItems.Rows[row].Cells["ItemName"].Value = "Invalid Item Code";
                        grdItems.Rows[row].Cells["Quantity"].Value = qty;
                        grdItems.Rows[row].Cells["Unit"].Value = string.Empty; 
                        grdItems.Rows[row].Cells["Vat"].Value = "0.00";
                        grdItems.Rows[row].Cells["Rate"].Value = "0.00";
                        grdItems.Rows[row].Cells["Amount"].Value = "0.00";
                    }
                    else
                    {
                        grdItems.Rows[row].Cells["ItemID"].Value = i.ID;
                        grdItems.Rows[row].Cells["ItemCode"].Value = i.ItemCode;
                        grdItems.Rows[row].Cells["ItemName"].Value = i.Name;
                        grdItems.Rows[row].Cells["Quantity"].Value = qty;
                        grdItems.Rows[row].Cells["UnitID"].Value = i.UnitID;
                        grdItems.Rows[row].Cells["Unit"].Value = i.Unit;
                        grdItems.Rows[row].Cells["Gst"].Value = i.Gst;

                        if (item.Data.Length > 19 && item.Data.Substring(15, 4).Trim() != string.Empty && Convert.ToInt32(item.Data.Substring(15, 4)) > 0)
                        {
                            grdItems.Rows[row].Cells["Rate"].Value = Convert.ToDecimal(item.Data.Substring(15, 4) + "." + item.Data.Substring(19, 1)).ToString("0.00");
                            //grdItems.Rows[row].Cells["Rate"].Value = Convert.ToInt32(item.Data.Trim().Substring(16, 4));
                        }
                        else
                        {
                            grdItems.Rows[row].Cells["Rate"].Value = i.Rate;
                        }
                        grdItems.Rows[row].Cells["Amount"].Value = (qty * i.Rate).ToString("0.00");
                    }

                    lblMessages.Visible = false;
                }

                if (!goAhead)
                {
                    return false;
                }

                CalculateTotals();

                ////Commented on 22 Jan 2012
                //Application.DoEvents();

                if (_showWeight && _dontSaveIfNoWeight)
                {
                    if (Convert.ToDecimal(lblCurrentWeight.Text) == 0)
                    {
                        _hideMessageCounter = 0;
                        lblCardPaymentErrorMessage.Text = "Weight must be greater than zero. Record not saved.";
                        lblCardPaymentErrorMessage.Visible = true;
                        return false;
                    }
                }

                string temp = string.Empty;
                if (!c.ResetItems(key, out temp, 0))
                {
                    MessageText("Show card again.", Color.Red);

                    //Added on 15 Jan 2012
                    grdItems.Rows.Clear();
                    
                    ////Commented & added below line on 22 Jan 2012 - Because blank bills are printing & saving zero amount in Sales & no items in SaleItems
                    //CalculateTotals();
                    txtTotalAmount.Text = txtTaxAmount.Text = lblNetAmount.Text = "0.00";
                    Application.DoEvents();

                    return false;
                }
                else
                {
                    lblCardBlank.Visible = true;
                    pnlMessages.Visible = false;
                    Application.DoEvents();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageText(ex.Message, Color.Red);
                btnShowBill.Enabled = true;
                Application.DoEvents();
                return false;
            }
        }

        private void btnShowBill_Click(object sender, EventArgs e)
        {         
            if (ShowAndReset())
            {
                Int64 billNumber = 0;
                if (!Save(out billNumber))
                {
                    MessageText("Save failed.", Color.Red);
                }
                else
                {
                    if (chkPrintDirectly.Checked)
                    {
                        Print(billNumber);
                    }
                } 
                //Added on 26 Jan 2012
                btnShowBill.Enabled = true;
                Application.DoEvents();
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

        void MessageText(string text, Color c)
        {
            lblMessages.Visible = true;
            lblMessages.BackColor = c;
            lblMessages.Text = text;
            if (text == "Reading card, please wait...")
            {
                if (_showAnimation)
                    pnlMessages.Visible = lblMessages.Visible = false;
                else
                    pnlMessages.Visible = lblMessages.Visible = true;
            }
            else
            {
                btnShowBill.Enabled = true;
            }
            Application.DoEvents();
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
            lblTime.TextAlign = ContentAlignment.MiddleCenter;

            _hideMessageCounter += 1;
            if (_hideMessageCounter == 5)
                lblCardPaymentErrorMessage.Visible = false;
        }

        #region GIF Animation

        Bitmap animatedImage; // = new Bitmap("E:\\POS\\GIF\\Cricket.gif");

        public Bitmap AnimatedImage { set { animatedImage = value; } }

        bool currentlyAnimating = false;

        private void pnlMessages_Paint(object sender, PaintEventArgs e)
        {
            if (!_showAnimation)
                return;

            //Begin the animation.
            AnimateImage();
            Application.DoEvents();

            //Get the next frame ready for rendering.
            ImageAnimator.UpdateFrames();

            //Draw the next frame in the animation.
            e.Graphics.DrawImage(this.animatedImage, new Point((pnlMessages.Width / 2) - (this.animatedImage.Width / 2), (pnlMessages.Height / 2) - (this.animatedImage.Height / 2)));
            Application.DoEvents();
        }

        //This method begins the animation.
        public void AnimateImage()
        {
            if (!currentlyAnimating)
            {
                //Begin the animation only once.
                ImageAnimator.Animate(animatedImage, new EventHandler(this.OnFrameChanged));
                currentlyAnimating = true;
                Application.DoEvents();
            }
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            //Force a call to the Paint event handler.
            pnlMessages.Invalidate();
            Application.DoEvents();
        }

        #endregion

        #region For Devesh

        private void lblBlank_Click(object sender, EventArgs e)
        {
            ShowAndReset();
        }

        #endregion

        private void chkPrintDirectly_CheckedChanged(object sender, EventArgs e)
        {
            btnShowBill.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_newRecordID > 0)
            {
                try
                {
                    _saleManager.UpdateCardPaymentDetails(_newRecordID, txtCardPaymentDetails.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in updating card details." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _hideMessageCounter = 0;
                lblCardPaymentErrorMessage.Text = "Previous transaction not found, Payment details not saved.";
                lblCardPaymentErrorMessage.Visible = true;
            }
            pnlCardPaymentDetails.Visible = false;
        }

        #region Weighing Scale Read Routine...

        private void tmrWeight_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();

            if (!_serialPort.IsOpen)
                return;
            string weighString = _serialPort.ReadExisting();
            int i = 0;
            decimal d;
            foreach (char c in weighString)
            {
                if (c == '+' || c == '-')
                {
                    if (weighString.Length >= i + 9)
                    {
                        if (weighString.Substring(i + 8, 1).ToUpper() == "K" || weighString.Substring(i + 8, 1).ToUpper() == "G")
                        {
                            if (decimal.TryParse(weighString.Substring(i, 8), out d))
                            {
                                lblCurrentWeight.Text = d.ToString("0.000");
                                break;
                            }
                        }
                        else
                            lblCurrentWeight.Text = "0.000";
                    }
                }
                i += 1;
            }
        }

        private void frmSaleRFID_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrWeight.Enabled = false;
            _serialPort.Close();
        }

        #endregion

        private void txtPreviousBill_MouseLeave(object sender, EventArgs e)
        {
            pnlPreviousBillItems.Visible = false;
            if (_oldTotal != "0")
            {
                lblNetAmount.Text = _oldTotal;
                _oldTotal = "0";
            }
            btnShowBill.Focus();
        }

        private void txtPreviousBill_Click(object sender, EventArgs e)
        {
            _oldTotal = lblNetAmount.Text;

            pnlPreviousBillItems.Size = pnlItemsGrid.Size;
            pnlPreviousBillItems.Location = pnlItemsGrid.Location;

            BindingList<PreviousBillItems> pbItems = (BindingList<PreviousBillItems>)((TextBox)sender).Tag;
            grdPreviousBillItems.DataSource = pbItems;

            Application.DoEvents();

            if (pbItems != null && pbItems.Count > 0)
            {
                grdPreviousBillItems.Columns[0].Width = grdItems.Columns["SrNo"].Width;
                grdPreviousBillItems.Columns[1].Width = grdItems.Columns["ItemCode"].Width;
                grdPreviousBillItems.Columns[2].Width = grdItems.Columns["ItemName"].Width;
                grdPreviousBillItems.Columns[3].Width = grdItems.Columns["Quantity"].Width;
                grdPreviousBillItems.Columns[4].Width = grdItems.Columns["Unit"].Width;
                grdPreviousBillItems.Columns[5].Width = grdItems.Columns["Rate"].Width;
                grdPreviousBillItems.Columns[6].Width = grdItems.Columns["Amount"].Width;

                grdPreviousBillItems.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grdPreviousBillItems.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grdPreviousBillItems.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grdPreviousBillItems.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grdPreviousBillItems.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                pnlPreviousBillItems.Visible = true;

                decimal netAmount = 0;
                foreach (DataGridViewRow dr in grdPreviousBillItems.Rows)
                {
                    netAmount += Convert.ToDecimal(dr.Cells[6].Value);
                }
                lblNetAmount.Text = netAmount.ToString("0.00");
            }

            btnShowBill.Focus();
        }

        private void btnShowBill_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.NumPad1 || e.KeyCode == Keys.NumPad2 || e.KeyCode == Keys.NumPad3 || e.KeyCode == Keys.NumPad4 || e.KeyCode == Keys.NumPad5 || e.KeyCode == Keys.NumPad6 || e.KeyCode == Keys.NumPad7 || e.KeyCode == Keys.NumPad8 || e.KeyCode == Keys.NumPad9)
            {
                _amountForChange += e.KeyValue.ToString();
                
            }
        }
    }

    class PreviousBillItems
    {
        string srNo = string.Empty;
        string code = string.Empty;
        string itemName = string.Empty;
        string quantity = string.Empty;
        string unit = string.Empty;
        string rate = string.Empty;
        string amount = string.Empty;

        public string SrNo
        {
            get { return srNo; }
            set { srNo = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public string Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public string Rate
        {
            get { return rate; }
            set { rate = value; }
        }

        public string Amount
        {
            get { return amount; }
            set { amount = value; }
        }
    }
}