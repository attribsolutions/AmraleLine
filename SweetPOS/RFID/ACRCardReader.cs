using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SweetPOS
{
    internal class ACRCardReader : CardReadWrapper
    {
        private static int g_rHandle;
        private static short g_retCode;
        private static short retCode;
        private static byte g_Sec;
        private static byte g_SID = 1;
        private byte[] g_pKey = new byte[6];
        private static bool g_isConnected = false;

        private static int _portNo;

        public ACRCardReader(int portNo)
        {
            _portNo = portNo;
        }

        private static bool Connect()
        {
            //=====================================================================
            // This function opens the port(connection) to ACR120 reader
            //=====================================================================

            // Variable declarations
            int ctr = 0;
            byte[] FirmwareVer = new byte[31];
            byte[] FirmwareVer1 = new byte[20];
            byte infolen = 0x00;
            string FirmStr;
            ACR120U.tReaderStatus ReaderStat = new ACR120U.tReaderStatus();

            if (g_isConnected)
            {
                return true;
            }

            ACR120U.ACR120_Close(0);

            g_rHandle = ACR120U.ACR120_Open(_portNo);
            if (g_rHandle != 0)

                throw new ApplicationException("Invalid Handle." + String.Format("{0}", g_rHandle));

            else
            {
                g_isConnected = true;

                //Get the DLL version the program is using
                g_retCode = ACR120U.ACR120_RequestDLLVersion(ref infolen, ref FirmwareVer[0]);
                if (g_retCode < 0)

                    throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));

                else
                {
                    FirmStr = "";
                    for (ctr = 0; ctr < Convert.ToInt16(infolen) - 1; ctr++)
                        FirmStr = FirmStr + char.ToString((char)(FirmwareVer[ctr]));
                    //DisplayMessage("DLL Version ." + FirmStr);
                }

                //Routine to get the firmware version.
                g_retCode = ACR120U.ACR120_Status(g_rHandle, ref FirmwareVer1[0], ref ReaderStat);
                if (g_retCode < 0)

                    throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));

                else
                {
                    FirmStr = "";
                    for (ctr = 0; ctr < Convert.ToInt16(infolen); ctr++)
                        if ((FirmwareVer1[ctr] != 0x00) && (FirmwareVer1[ctr] != 0xFF))
                            FirmStr = FirmStr + char.ToString((char)(FirmwareVer1[ctr]));
                    //DisplayMessage("Firmware Version ." + FirmStr);
                }
            }

            return true;
        }

        private string Select()
        {
            //=====================================================================
            // This function selects a single card in range and return the Serial No.
            //=====================================================================

            //Variable Declarations
            byte[] ResultSN = new byte[11];
            byte ResultTag = 0x00;
            byte[] TagType = new byte[51];
            int ctr = 0;
            string SN = "";


            //Select specific card based from serial number	
            g_retCode = ACR120U.ACR120_Select(g_rHandle, ref TagType[0], ref ResultTag, ref ResultSN[0]);
            if (g_retCode < 0)

                throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));

            else
            {
                //DisplayMessage("Select Success");
                //get serial number and convert to hex

                if ((TagType[0] == 4) || (TagType[0] == 5))
                {

                    SN = "";
                    for (ctr = 0; ctr < 7; ctr++)
                    {
                        SN = SN + string.Format("{0:X2} ", ResultSN[ctr]);
                    }
                }
                else
                {

                    SN = "";
                    for (ctr = 0; ctr < ResultTag; ctr++)
                    {
                        SN = SN + string.Format("{0:X2} ", ResultSN[ctr]);
                    }
                }

                //Display Serial Number
                //DisplayMessage("( i ) Card Serial Number." + SN + " ( " + ACR120U.GetTagType1(TagType[0]) + " )");
            }
            return SN;
        }

        private void Login(int sectorNo)
        {
            //=====================================================================
            // This function is for the authentication to access one sector of a card.
            // Only one sector at a time can be accessed.
            //=====================================================================

            long sto = 0;
            byte vKeyType = 0x00;
            int ctr, tmpInt, PhysicalSector = 0;


            vKeyType = ACR120U.ACR120_LOGIN_KEYTYPE_DEFAULT_F;


            for (ctr = 0; ctr < 6; ctr++)
            {
                g_pKey[ctr] = 0xFF;
                break;
            }

            tmpInt = Convert.ToInt16(sectorNo);
            g_Sec = Convert.ToByte(tmpInt);

            //Computation for obtaining the actual Physical Sector.
            if (Convert.ToInt16(g_Sec) > 31)
                PhysicalSector = Convert.ToInt16(g_Sec) + ((Convert.ToInt16(g_Sec) - 32) * 3);
            else
                PhysicalSector = Convert.ToInt16(g_Sec);

            g_retCode = ACR120U.ACR120_Login(g_rHandle, Convert.ToByte(PhysicalSector), Convert.ToInt16(vKeyType),
                Convert.ToByte(sto), ref g_pKey[0]);
            if (g_retCode < 0)

                throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));

            //else
            //{
            //    DisplayMessage("Login Success");
            //    DisplayMessage("Log at Logical Sector." + String.Format("{0}", Convert.ToInt16(g_Sec)));
            //    DisplayMessage("Log at Physical Sector." + String.Format("{0}", PhysicalSector));
            //    DisplayMessage("Login Type index." + string.Format("{0}", loginForm.cbLoginType.SelectedIndex));
            //}
        }

        private string Read(int blockNo)
        {
            //=======================================================================
            // This function reads a block within the sector where you login
            //=======================================================================

            //Variable Declarations
            byte[] dataRead = new byte[16];
            string dstr;
            int ctr, tmpInt = 0;
            byte Blck = 0;

            Blck = Convert.ToByte(blockNo);

            //To access the exact block on the card you must Multiply Sector where you Login by 4
            //and add the Block.

            //Computation for exact block to Access
            tmpInt = Convert.ToInt16(Blck);
            if (Convert.ToInt16(g_Sec) > 31)
                tmpInt = tmpInt + ((Convert.ToInt16(g_Sec) - 32) * 16) + 128;
            else
                tmpInt = tmpInt + Convert.ToInt16(g_Sec) * 4;
            Blck = Convert.ToByte(tmpInt);

            retCode = ACR120U.ACR120_Read(g_rHandle, Blck, ref dataRead[0]);
            if (retCode < 0)

                throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));

            else
            {
                //DisplayMessage("Read Block Success");
                // convert bytes read to chosen option (e.g. AS HEX, AS ASCII)
                dstr = "";
                for (ctr = 0; ctr < 16; ctr++)
                {
                    dstr = dstr + char.ToString((char)(dataRead[ctr]));
                }

                return dstr;
            }
        }

        private void Write(int blockNo, string data)
        {
            byte Blck = Convert.ToByte(blockNo);
            byte[] dout = new byte[16];
            char[] charArray = new char[16];

            charArray = data.ToCharArray();

            for (int ctr = 0; ctr < data.Length; ctr++)
            {
                dout[ctr] = Convert.ToByte(charArray[ctr]);
            }

            g_retCode = ACR120U.ACR120_Write(g_rHandle, Blck, ref dout[0]);
            if (g_retCode != 0)
            {
                throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));
            }
        }

        public override string ReadBlock()
        {
            string retVal = string.Empty;
            Connect();
            Select();

            Login(1);
            retVal = Read(2);

            return retVal;
        }

        public override string GetCardSerial()
        {
            Connect();

            return Select();
        }

        public override void WriteBlock(int blockNo, string data)
        {
            Connect();
            Select();

            Login(1);
            Write(blockNo, data);

            ////=======================================================================
            //// This function writes a block within the sector where you login
            ////=======================================================================

            ////Variable Declarations
            //int ctr, tmpInt = 0;
            //byte Blck = 0;
            //byte[] dout = new byte[16];
            //char[] charArray = new char[16];
            //string inStr;

            ////To access the exact block on the card 
            //// For Sectors 0 - 31, you must Multiply Sector where you Login by 4 and add Block
            //// For Sectors 32 - 38, you must Multiply Sector where you Login by 16 and add Block + 128
            //Blck = Convert.ToByte(blockNo);

            ////Computation for exact block to Access
            ////This is for Trapping Accidental write to block 3 (Sector < 32) or block 15 which is the Trailer Block
            ////If your not sure on how to setup Access Bit or what to write on Trailer Block
            //tmpInt = Convert.ToInt16(Blck);
            //if (Convert.ToInt16(g_Sec) > 31)
            //{
            //    tmpInt = tmpInt + ((Convert.ToInt16(g_Sec) - 32) * 16) + 128;
            //}
            //else
            //{
            //    tmpInt = tmpInt + Convert.ToInt16(g_Sec) * 4;
            //}
            //Blck = Convert.ToByte(tmpInt);

            //    charArray = data.ToCharArray();
            //    inStr = data;
            //    for (ctr = 0; ctr < data.Length; ctr++)
            //    {
            //        dout[ctr] = Convert.ToByte(charArray[ctr]);
            //    }

            //g_retCode = ACR120U.ACR120_Write(g_rHandle, Blck, ref dout[0]);

            //if (g_retCode < 0)

            //    throw new ApplicationException(ACR120U.GetErrMsg(g_retCode));

            //else
            //{
            //    throw new ApplicationException("Write Block Success.");
            //}
        }
    }
}