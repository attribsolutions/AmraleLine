using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace Licenser
{
    public class GetInfo
    {
        /// <summary>
        /// return Volume Serial Number from hard drive
        /// </summary>
        /// <param name="strDriveLetter">[optional] Drive letter</param>
        /// <returns>[string] VolumeSerialNumber</returns>
        public string GetVolumeSerial(string strDriveLetter)
        {
            if (strDriveLetter == "" || strDriveLetter == null) strDriveLetter = "C";
            ManagementObject disk =
                new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
            disk.Get();
            return disk["VolumeSerialNumber"].ToString();
        }


        /// <summary>
        /// Returns MAC Address from first Network Card in Computer
        /// </summary>
        /// <returns>[string] MAC Address</returns>
        public string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)  // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        /// <returns>[string] ProcessorId</returns>
        public string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {// only return cpuInfo from first CPU
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
            }
            return cpuInfo;
        }

        /// <summary>
        /// Generates serial number from the machineID provided.
        /// </summary>
        /// <param name="machineID"> ComputerID showing on the login form.</param>
        /// <param name="dateTill">License validation date.</param>
        /// <returns>[string] SerialKey</returns>
        public string GenerateSerial(string machineID, object dateTill)
        {
            //Converts number into numbers & till 25 characters.

            string fixedNumbers = "71250658460211457";
            string numericSerial = string.Empty;

            foreach (char c in machineID)
            {
                if (!char.IsNumber(c))
                {
                    int i = (int)c;
                    numericSerial += i;
                }
                else
                {
                    numericSerial += c;
                }
            }
            if (numericSerial.Length < 25)
            {
                int charsNeeded = 25 - numericSerial.Length;
                numericSerial = fixedNumbers.Substring(0, charsNeeded) + numericSerial;
            }

            //Date logic

            string dateLogic;
            if (dateTill == null)
                dateLogic = DateTime.Today.Month.ToString().PadLeft(2, '0') + "1" + DateTime.Today.Year.ToString().Substring(2, 2);
            else
                dateLogic = ((DateTime)dateTill).Month.ToString().PadLeft(2, '0') + "1" + ((DateTime)dateTill).Year.ToString().Substring(2, 2);

            //Divide the number in sets of five in descending order per block

            string b1 = numericSerial.Substring(4, 1) + numericSerial.Substring(3, 1) + dateLogic.Substring(0, 1) + numericSerial.Substring(1, 1) + numericSerial.Substring(0, 1);
            string b2 = numericSerial.Substring(9, 1) + numericSerial.Substring(8, 1) + dateLogic.Substring(1, 1) + numericSerial.Substring(6, 1) + numericSerial.Substring(5, 1);
            string b3 = numericSerial.Substring(14, 1) + numericSerial.Substring(13, 1) + dateLogic.Substring(2, 1) + numericSerial.Substring(11, 1) + numericSerial.Substring(10, 1);
            string b4 = numericSerial.Substring(19, 1) + numericSerial.Substring(18, 1) + dateLogic.Substring(3, 1) + numericSerial.Substring(16, 1) + numericSerial.Substring(15, 1);
            string b5 = numericSerial.Substring(24, 1) + numericSerial.Substring(23, 1) + dateLogic.Substring(4, 1) + numericSerial.Substring(21, 1) + numericSerial.Substring(20, 1);

            string serialNumber = b2 + "-" + b1 + "-" + b5 + "-" + b4 + "-" + b3;

            return serialNumber;
        }
    }
}
