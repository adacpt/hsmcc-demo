using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharpPro;

namespace VideoWallDevice
{
    public class VideoWall
    {
        
        public string HostName { get; set; }
        public string UiName { get; set; }
        public string IpAddress { get; set; }
        public int DeviceId { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        public int Layout { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }

        public VideoWall(string hostName, string ipAddress, int deviceID, int port)
        {
            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.DeviceId = deviceID;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, port);
        }
        public VideoWall(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate)
        {

            this.HostName = hostName;
            this.SerialPort = serialPort;

        }

    }
}
