using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharpPro;

namespace MatrixSwitchDevice
{
    public class MatrixSwitch
    {
       
        public string HostName { get; set; }
        public string UiName { get; set; }
        public string IpAddress { get; set; }
        public int DeviceId { get; set; }
        public int Port { get; set; }
        public String BaudRate { get; set; }
        public List<InputNode> Inputs { get; set; }
        public List<OutputNode> Outputs { get; set; }
        public TcpClient Client { get; set; }
        public ComPort SerialPort { get; set; }

        public MatrixSwitch(string hostName, string ipAddress, int deviceID, int port, int inputCount, int outputCount)
        {
            this.HostName = hostName;
            this.IpAddress = ipAddress;
            this.DeviceId = deviceID;
            this.Port = port;
            this.Client = new TcpClient(HostName, IpAddress, port);

            Inputs = new List<InputNode>();
            Outputs = new List<OutputNode>();

            // The 0 index is for the Blank Source
            for ( int i = 0; i <= inputCount; i++ )
            {
                Inputs.Add(new InputNode { Index = i, Input = i });
            }

            // The 0 index is for the Blank Destination
            for (int o = 0; o <= outputCount; o++)
            {
                Outputs.Add(new OutputNode { Index = o, Output = o });
            }
        }
        public MatrixSwitch(string hostName, Crestron.SimplSharpPro.ComPort serialPort, string baudRate, int inputCount, int outputCount)
        {
            this.HostName = hostName;
            this.SerialPort = serialPort;

            Inputs = new List<InputNode>();
            Outputs = new List<OutputNode>();

            // The 0 index is for the Blank Source
            for (int i = 0; i <= inputCount; i++)
            {
                Inputs.Add(new InputNode { Index = i, Input = i });
            }

            // The 0 index is for the Blank Destination
            for (int o = 0; o <= outputCount; o++)
            {
                Outputs.Add(new OutputNode { Index = o, Output = o });
            }
        }
    }
}
