using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;


namespace VtcDevice
{
    public class Vtc
    {
       
        public string HostName { get; set; }
        public string UiName { get; set; }
        public string IpAddress { get; set; }
        public int DeviceId { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        
        public Dictionary<String, int> CallStatus { get; set; }
        public int PresentationStatus { get; set; }
        public Dictionary<int, VTCSpeedDialElement> StoredNumbers { get; set; }
        public int DTMF { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }
        public Vtc(string hostName, string ipAddress, int deviceID, int port)
        {
            CallStatus = new Dictionary<string, int>();

            StoredNumbers = new Dictionary<int, VTCSpeedDialElement>();

            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.DeviceId = deviceID;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, port);
        }

        public Vtc(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate)
        {
            CallStatus = new Dictionary<string, int>();

            StoredNumbers = new Dictionary<int, VTCSpeedDialElement>();

            this.HostName = hostName;
            this.SerialPort = serialPort;
            
        }
    }
}
