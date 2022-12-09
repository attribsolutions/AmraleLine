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

namespace SweetPOS
{
    public partial class frmInitializeCards : Form
    {
        #region Class level variables...

        Communication c = new Communication(Program.PORTNAME, Program.BAUDRATE, true);
        string key = Program.KEYSET;
        string _previousCardNo = string.Empty;
        int _cardCount = 0;
        SettingManager _settingManager = new SettingManager();
        bool _allowInitializeWithItem = false;
        List<string> _cards = new List<string>();

        #endregion

        public frmInitializeCards()
        {
            InitializeComponent();
            this.Text = Program.MESSAGEBOXTITLE + " - Card Initialization";
        }

        private void frmInitializeCards_Load(object sender, EventArgs e)
        {
            if (Program.ALLOWDIRECTINITIALIZATION)
                btnInitialize.Enabled = true;

            try
            {
                _allowInitializeWithItem = Convert.ToBoolean(_settingManager.GetSetting(24));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in getting setting." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            MessageText("Reading card, please wait...", Color.DarkOliveGreen);

            try
            {
                string data = c.ReadBlock(key, 1);
                Application.DoEvents();
                int itemCount = 0;
                if (data.Trim().Length >= 17)
                {
                    int i;
                    if (int.TryParse(data.Substring(15, 2), out i))
                    {
                        itemCount = Convert.ToInt32(data.Substring(15, 2));
                    }

                    if (itemCount > 0)
                    {
                        if (_allowInitializeWithItem)
                        {
                            DialogResult dr = MessageBox.Show("Items found, Continue Initialize?", Program.MESSAGEBOXTITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dr == DialogResult.No)
                            {
                                MessageText("Card not initialized.", Color.Violet);
                                return;
                            }
                        }
                        else
                        {
                            MessageText("Not allowed to Initialize cards with Items. Please check setting.", Color.Violet);
                            return;
                        }
                    }
                }

                string cardID = string.Empty;

                if (!c.ResetItems(key, out cardID, 0))
                {
                    MessageText("Card initialization failed... Keep card beside.", Color.Red);
                }
                else
                {
                    MessageText("Card initialized successfully.", Color.Green);

                    if (!CheckCardInitializedBefore(cardID))
                    {
                        _cards.Add(cardID);
                        _cardCount += 1;
                        lblCardCount.Text = _cardCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool CheckCardInitializedBefore(String s)
        {
            foreach (string str in _cards)
            {
                if (str == s)
                    return true;
            }

            return false;
        }

        void MessageText(string text, Color c)
        {
            lblMessages.BackColor = c;
            lblMessages.Text = text;
            lblMessages.Visible = true;
            Application.DoEvents();
        }

        private void btnMasterCard_Click(object sender, EventArgs e)
        {
            MessageText("Reading card, please wait...", Color.DarkOliveGreen);

            try
            {
                string masterKey = c.ReadBlock(key, 2);
                if (masterKey.Substring(5, 11) == "MASTER CARD")
                {
                    btnMasterCard.Enabled = false;
                    btnInitialize.Enabled = true;
                    btnInitialize.Focus();
                    MessageText("Master card validated successfully... Please place the cards one by one && press enter.", Color.Blue);
                }
                else
                {
                    MessageText("Not a master card, Try again.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while validating Master Card." + Environment.NewLine + Environment.NewLine + "(" + ex.Message + ")", Program.MESSAGEBOXTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}