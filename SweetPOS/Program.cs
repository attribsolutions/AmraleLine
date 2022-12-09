using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Configuration;

namespace SweetPOS
{
    static class Program
    {
        public static bool IsItemCodeAsID = false;

        public static EnumClass.SystemOperatingMode SystemOperatingMode = EnumClass.SystemOperatingMode.TouchScreenWithoutRFID;

        public static string KEYSET = "FFFFFFFFFFFF";
        public static Int32 BAUDRATE = 115200;

        public static string SERIALKEY = string.Empty;
        public static string TEMPSERIAL = string.Empty;

        public static string MESSAGEBOXTITLE = string.Empty;
        public static string COMPANYNAME = string.Empty;
        public static string COMPANYSUBTITLE = string.Empty;
        public static string ADDRESS1 = string.Empty;
        public static string ADDRESS2 = string.Empty;
        public static string PHONENUMBER = string.Empty;
        public static string VATNO = string.Empty;
        public static string SUBJECTTO = string.Empty;
        public static bool SHOWSHORTBILL = false;

        public static byte CURRENTUSER = 1;
        public static int CounterID = 0;
        public static EnumClass.UserRoles UserType = EnumClass.UserRoles.Administrator;
        public static string CounterName = "Counter1";
        public static string LoginName = "";
        public static string UserRole = "";

        public static string PORTNAME = string.Empty;
        public static bool ALLOWDIRECTINITIALIZATION = false;
        public static int DivisionID = 0;

        public static int FixedItemCount = 25;
        public static string CLIENTNAME = string.Empty;

        public static int ChallanFormMode = 0;

        internal const string REGKEY = "Software\\Company\\SweetPOS";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //if (SweetPOS.Properties.Settings.Default.CounterMode)
                Application.Run(new frmLogin());
        }
    }
}