using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SweetPOS
{
    class CardReadWrapper
    {
        public virtual string GetCardSerial() { return ""; }

        public virtual string ReadBlock() { return ""; }

        /// <summary>
        /// Write data to specified location.
        /// </summary>
        /// <param name="writeType">Enum of write types.</param>
        /// <param name="data">Data to be written (must be equal or less than 16 chars).</param>
        public virtual void WriteBlock(int blockNo, string data) { }

        private static CardReadWrapper _reader;

        public static CardReadWrapper GetInstance(string readerType, int portNo)
        {
            if (_reader != null)
            {
                return _reader;
            }
            switch (readerType)
            {
                case "ACR":
                    _reader = new ACRCardReader(portNo);
                    break;
            }
            return _reader;
        }
    }
}