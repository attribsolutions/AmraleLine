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
    public partial class frmShowReport : Form
    {
        public frmShowReport(string reportTitle)
        {
            InitializeComponent();
            this.Text = reportTitle;
        }

        private void frmShowReport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.DialogResult = DialogResult.OK;
        }
    }
}