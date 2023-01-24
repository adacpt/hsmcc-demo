using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharpPro;

namespace DspDevice
{
    public class Dsp
    {
        public String Make { get; set; }
        public String Model { get; set; }
        public int DeviceID { get; set; }
        public String HostName { get; set; }
        public String IpAddress { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        public float Volume { get; set; }
        public int VolumeMute { get; set; }
        public int MicMute { get; set; }
        public int ActiveCall { get; set; }
        public Dictionary<int, ATCSpeedDialElement> StoredNumbers { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }

        public Dsp(string hostName, string ipAddress, int deviceId, int port)
        {
            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.DeviceID = deviceId;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, Port);

            StoredNumbers = new Dictionary<int, ATCSpeedDialElement>();
        }
        public Dsp(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate)
        {
            this.HostName = hostName;
            this.SerialPort = serialPort;

            StoredNumbers = new Dictionary<int, ATCSpeedDialElement>();
        }
    }
}
