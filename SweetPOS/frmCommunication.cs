using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;

namespace SweetPOS
{
    public partial class frmCommunication : Form
    {
        SerialPort serialPort = null;

        public frmCommunication()
        {
            InitializeComponent();
        }

        private void frmCommunication_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort("COM9", 9600);
            serialPort.BaudRate = 9600;
            serialPort.Close();

            serialPort.Open();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text += Environment.NewLine + serialPort.ReadExisting();
        }

        private void frmCommunication_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort.Close();
        }
    }
}