using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.ComponentModel;
using DataObjects;

namespace ChitalePersonalzer
{
    public class ReadItemInfo
    {
        public ReadItemInfo()
        {
            
        }

        System.String _data = string.Empty;

        public System.String Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }

    public class Communication
    {
        SerialPort _serialPort = null;
        string _portName = string.Empty;
        int _baudRate = 0;
        bool _isOneKCard = true;

        public Communication(string portName, int baudRate, bool isOneKCard)
        {
            _isOneKCard = isOneKCard;
            _portName = portName;
            _baudRate = baudRate;
            _serialPort = new SerialPort(portName);
        }
        
        private void CheckPortConnection()
        {
            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;

            try
            {
                _serialPort.Open();
                _serialPort.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error opening port " + _portName + ".");
            }
        }

        void LoadKeySet(string key)
        {
            CheckPortConnection();

            if (!ValidateKey(key))
                throw new ApplicationException("Key validation failed.");

            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;


            _serialPort.Open();
            _serialPort.Write("L001" + key + "\r");
            string abc = _serialPort.ReadExisting();
            _serialPort.Close();
        }

        string IntToString(int intBlockSector)
        {
            return intBlockSector.ToString().PadLeft(2, '0');
        }

        public bool CheckReaderWorking()
        {
            _serialPort.Open();
            _serialPort.Write("t\r");
            string retVal = _serialPort.ReadExisting();
            _serialPort.Close();

            if (retVal.Trim() == "0")
                return true;
            else
                return false;
        }

        public string ReadBlock(string key, int blockNo)
        {
            if (!ValidateBlockNumber(blockNo))
                throw new ApplicationException("Invalid block.");

            LoadKeySet(key);

            string block = IntToString(blockNo);

            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;
            _serialPort.Open();
            _serialPort.ReadExisting();

            string str = "R1001" + block + "\r";

            _serialPort.Write(str);
            
            string retVal = _serialPort.ReadLine();

            _serialPort.Close();
            
            return retVal;
        }

        bool ValidateKey(string key)
        {
            if (key.Length != 12)
                return false;

            return true;
        }

        bool ValidateBlockNumber(int blockNo)
        {
            if (_isOneKCard)
            {
                if (blockNo > 62 || blockNo == 0)
                    return false;

                int rem;
                int r = Math.DivRem(blockNo, 3, out rem);
                if (rem == 3)
                    return false;
            }

            return true;
        }

        public BindingList<ReadItemInfo> ReadItems(string key, int noOfItems)
        {
            BindingList<ReadItemInfo> retVal = new BindingList<ReadItemInfo>();

            LoadKeySet(key);

            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;
            _serialPort.Open();
            _serialPort.ReadExisting();

            string cmd = "R1001";

            int blockNo = 4;

            for (int i = 0; i < noOfItems; )
            {
                ReadItemInfo readItem = new ReadItemInfo();
                int rem;
                int r = Math.DivRem(blockNo, 4, out rem);
                if (rem != 3)
                {
                    _serialPort.Write(cmd + blockNo.ToString().PadLeft(2, '0') + "\r");
            
                    readItem.Data = _serialPort.ReadLine();
                    //readItem.Data = _serialPort.ReadExisting();

                    if (readItem.Data != "\r")
                    {
                        retVal.Add(readItem);
                        i++;
                    }
                }
                blockNo++;
            }

            _serialPort.ReadExisting();
            _serialPort.Close();

            return retVal;
        }

        public bool WriteItems(string key, List<SaleItemInfo> saleItems, int itemCount, out int itemsFailed)
        {
            LoadKeySet(key);

            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;
            _serialPort.Open();
            _serialPort.ReadExisting();

            string readLine = string.Empty;
            int blockNo = 4;
            int rem;
            if (itemCount > 0)
            {
                for (int i = 0; i < saleItems.Count; )
                {
                    ReadItemInfo readItem = new ReadItemInfo();
                    int r = Math.DivRem(blockNo, 4, out rem);
                    if (rem != 3)
                    {
                        i++;
                    }
                    blockNo++;
                }
            }

            for (int i = 0; i < saleItems.Count; )
            {
                ReadItemInfo readItem = new ReadItemInfo();
                int r = Math.DivRem(blockNo, 4, out rem);
                if (rem != 3)
                {
                    if (saleItems[i].ItemID.ToString().Length == 4)
                    {
                        string itemNumber = saleItems[i].ItemID.ToString().PadLeft(4, '0');
                        string quantity = saleItems[i].Quantity.ToString("000.000").Split('.')[0] + saleItems[i].Quantity.ToString("000.000").Split('.')[1];
                        
                        string rate = string.Empty;
                        string cmd = string.Empty;

                        if (saleItems[i].Rate > 0)
                        {
                            rate = saleItems[i].Rate.ToString("0000.0").Split('.')[0] + saleItems[i].Rate.ToString("0000.0").Split('.')[1];
                            cmd = "W1000" + blockNo.ToString().PadLeft(2, '0') + itemNumber + quantity + rate + "B\r";
                        }
                        else
                        {
                            cmd = "W1000" + blockNo.ToString().PadLeft(2, '0') + itemNumber + quantity + "0000B\r";
                        }
                        _serialPort.Write(cmd);
                    }
                    else
                    {
                        string itemNumber = saleItems[i].ItemID.ToString().PadLeft(3, '0');
                        string quantity = saleItems[i].Quantity.ToString("000.000");

                        string rate = string.Empty;
                        string cmd = string.Empty;

                        if (saleItems[i].Rate > 0)
                        {
                            rate = saleItems[i].Rate.ToString("0000.0").Split('.')[0] + saleItems[i].Rate.ToString("0000.0").Split('.')[1];
                            cmd = "W1000" + blockNo.ToString().PadLeft(2, '0') + itemNumber + quantity + rate + "0\r";
                        }
                        else
                        {
                            cmd = "W1000" + blockNo.ToString().PadLeft(2, '0') + itemNumber + quantity + "000000\r";
                        }
                        _serialPort.Write(cmd);
                    }
                    readLine = _serialPort.ReadLine();

                    if (readLine.Trim().Substring(0, 1) != "0")
                    {
                        itemsFailed = 1;
                        return false;
                    }

                    i++;
                }
                blockNo++;
            }

            string Dt = DateTime.Now.Date.Day.ToString().PadLeft(2, '0') + DateTime.Now.Date.Month.ToString().PadLeft(2, '0');
            string Tm = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
            string cmd1 = "W100001n" + Dt + Tm + "#" + (itemCount + saleItems.Count).ToString().Trim().PadLeft(2, '0') + "0000\r";
            _serialPort.Write(cmd1);

            readLine = _serialPort.ReadLine();
            _serialPort.Close();

            if (readLine.Trim().Substring(0, 1) == "0")
            {
                itemsFailed = 2;
                return true;
            }
            else
            {
                itemsFailed = 3;
                return false;
            }
        }

        public bool ResetItems(string key, out string cardID, int noOfItems)
        {
            LoadKeySet(key);
            string cardNo = string.Empty;
            cardNo = ReadCardID();

            string Dt = DateTime.Now.Date.Day.ToString().PadLeft(2, '0') + DateTime.Now.Date.Month.ToString().PadLeft(2, '0');
            string Tm = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;

            _serialPort.Open();
            _serialPort.ReadExisting();

            _serialPort.Write("W100001n" + Dt + Tm + "#0" + noOfItems.ToString().Trim() + "00000\r");

            string retVal = _serialPort.ReadLine();

            _serialPort.Close();

            cardID = cardNo;
            return retVal.Trim().Substring(0, 1) == "0";
        }

        public string ReadCardID()
        {
            _serialPort = new SerialPort(_portName);
            _serialPort.BaudRate = _baudRate;

            _serialPort.Open();
            _serialPort.ReadExisting();

            _serialPort.Write("R100101\r");

            string retVal = string.Empty;
            int i = _serialPort.ReadByte();
            if (i == 48)
            {
                retVal = _serialPort.ReadByte().ToString() + " " + _serialPort.ReadByte().ToString() + " " + _serialPort.ReadByte().ToString() + " " + _serialPort.ReadByte().ToString();
            }

            _serialPort.Close();

            return retVal;
        }
    }
}