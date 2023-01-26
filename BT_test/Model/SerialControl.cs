using System;
using System.IO.Ports;


namespace BT_test.Model
{
    class SerialControl
    {

        private const char STX = '\x02';
        private const char ETX = '\x03';


        private TopLog _log;
        private string _com = "";
        private SerialPort _port;
        private string _msg;

        public SerialControl(TopLog log)
        {
            _log = log;
        }

        public SerialControl(TopLog log, string com)
        {
            _log = log;
            SetCom(com);
        }

        public void SetCom(string com)
        {
            _com = com;
        }

        public void Start()
        {
            if (_com == "")
            {
                _log.Log = "[Error] Bad COM port '" + _com + "'";
                return;
            }

            _port = new SerialPort()
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Handshake = Handshake.None,
                DtrEnable = false,
                RtsEnable = false,
                PortName = _com,
                ReadTimeout = -1,
                WriteTimeout = -1,
            };
            _port.DataReceived += this.DataReceived;
            _msg = string.Empty;
            _port.Open();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int read = _port.BytesToRead;
            if(read > 0)
            {
                byte[] data = new byte[read];
                _port.Read(data, 0, read);

                bool isStx = false;
                bool isEtx = false;
                int posStart = 0;
                int posEnd = read - 1;
                for(int i=0; i<data.Length; i++)
                {
                    if(!isStx && (data[i] == STX))
                    {
                        if (isEtx) isEtx = false;
                        posStart = i + 1;
                        isStx = true;
                    }
                    if(!isEtx && (data[i] == ETX))
                    {
                        posEnd = i - 1;
                        isEtx = true;
                    }
                }
                if (isStx) _msg = "";
                if (!isEtx) posEnd = read - 1;
                if((posStart < read) || (posEnd>=0))
                {
                    byte[] tmp = new byte[posEnd - posStart + 1];
                    Array.Copy(data, posStart, tmp, 0, tmp.Length);
                    _msg += System.Text.Encoding.ASCII.GetString(tmp);
                }
                if(isEtx)
                {
                    this.Received(_msg);
                }
            }
        }

        private void Received(string msg)
        {
            _log.Log = "Data received.\n" + msg + "\n"; 
        }



    }
}
