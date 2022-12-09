using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace SweetPOS.Classes
{
    public static class CustomerDisplay
    {
        public static bool ShowMessage(string message)
        {
            if (message.Length != 40)
                return false;

            try
            {
                SerialPort port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
                port.Open();
                port.Write(message);
                port.Close();
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("Error Customer Display: " + ex.Message);
                return false;
            }

            return true;
        }
    }
}
