using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharpPro;

namespace SignagePlayerDevice
{
    public class SignagePlayer
    {
        
        public string HostName { get; set; }
        public string UiName { get; set; }
        public string IpAddress { get; set; }
        public int DeviceId { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        public int Classification { get; set; }
        public int MicMute { get; set; }
        public int SpeakerMute { get; set; }
        public int VtcActive { get; set; }
        public List<String> ZoneNames { get; set; }
        public List<int> ZoneHourOffsets { get; set; }
        public List<int> ZoneMinuteOffsets { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }

        public SignagePlayer(string hostName, string ipAddress, int port, int timeZones)
        {
            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, port);

            ZoneNames = new List<String>();
            ZoneHourOffsets = new List<int>();
            ZoneMinuteOffsets = new List<int>();

            for (int i = 0; i <= (timeZones-1); i++)
            {
                ZoneNames.Add("");
                ZoneHourOffsets.Add(0);
                ZoneMinuteOffsets.Add(0);
            }
        }

        public SignagePlayer(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate, int timeZones)
        {

            this.HostName = hostName;
            this.SerialPort = serialPort;

            ZoneNames = new List<String>();
            ZoneHourOffsets = new List<int>();
            ZoneMinuteOffsets = new List<int>();

            for (int i = 0; i <= (timeZones - 1); i++)
            {
                ZoneNames.Add("");
                ZoneHourOffsets.Add(0);
                ZoneMinuteOffsets.Add(0);
            }

        }

    }
}
