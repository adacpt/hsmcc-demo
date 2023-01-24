using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharpPro;

namespace CatvDevice
{
    public class Catv
    {
        public String Make { get; set; }
        public String Model { get; set; }
        public int DeviceID { get; set; }
        public String HostName { get; set; }
        public String IpAddress { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        public String Channel { get; set; }
        public Dictionary<int, String> ChannelPresets { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }

        public Catv(string hostName, string ipAddress, int port)
        {
            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, Port);

            ChannelPresets = new Dictionary<int, string>();
        }
        public Catv(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate)
        {

            this.HostName = hostName;
            this.SerialPort = serialPort;

            ChannelPresets = new Dictionary<int, string>();

        }
    }
}
