using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Licenser;

namespace SweetPOS
{
    public partial class Licenser : Form
    {
        public Licenser()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GetInfo g = new GetInfo();
            txtSerialNumber.Text = g.GenerateSerial(txtMachineID.Text,(object)dtValidTillDate.Value.Date);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
