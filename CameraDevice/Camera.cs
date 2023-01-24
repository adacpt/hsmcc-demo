using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharpPro;

namespace CameraDevice
{
    public class Camera
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string HostName { get; set; }
        public string UiName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        public int CurrentPreset { get; set; }
        public int Orientation { get; set; }
        public int Power { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }

        public Camera(string hostName, string ipAddress, int port)
        {
            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, Port);
        }
        public Camera(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate)
        {

            this.HostName = hostName;
            this.SerialPort = serialPort;

        }
    }
}