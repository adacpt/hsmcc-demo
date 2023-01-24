using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using DevicesControlled;

namespace MatrixSwitchDevice
{
    public class MatrixSwitchGenericSerialImpl : IMatrixSwitch
    {
        
        MatrixSwitch MatrixSwitch { get; set; }

        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public MatrixSwitchGenericSerialImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
            PollingTimer.Enabled = true;
        }

        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            this.MatrixSwitch = new MatrixSwitch(name, ipAddress, deviceID, 40, 16, 16);

            if (deviceID == 0)
            {
                MatrixSwitch.DeviceId = 1;
            }
            else
            {
                MatrixSwitch.DeviceId = deviceID;
            }

            this.MatrixSwitch.Client.Enable = true;
            this.MatrixSwitch.Client.DataReceived += OnDataReceived;
            this.MatrixSwitch.Client.DeviceConnected += Client_DeviceConnected;
            this.MatrixSwitch.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to Matrix Switch at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            this.MatrixSwitch = new MatrixSwitch(name, serialPort, baudRate, 16, 16);

            switch (baudRate)
            {
                case "9600":
                    this.MatrixSwitch.SerialPort.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                                                    ComPort.eComDataBits.ComspecDataBits8,
                                                    ComPort.eComParityType.ComspecParityNone,
                                                    ComPort.eComStopBits.ComspecStopBits1,
                                                    ComPort.eComProtocolType.ComspecProtocolRS232,
                                                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                                    false);
                    break;
            }

            CrestronConsole.PrintLine($"Connecting to Matrix Switch via serial");
        }

        public void Reconnect()
        {
            if (this.MatrixSwitch.Client.IsConnected)
            {
                this.MatrixSwitch.Client.Enable = false;
                this.MatrixSwitch.Client.Enable = true;
            }
            else
            {
                this.MatrixSwitch.Client.Enable = false;
                this.MatrixSwitch.Client.Enable = true;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //CrestronConsole.PrintLine("From Matrix Switch SIMPL# Module: raw device response: " + e.DataString + "\r");

            if (e.DataString.Contains("")) // 
            {
                // Call Status
                Dictionary<int, int> Route = new Dictionary<int, int>();
                Route[1] = 1;

                DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
                {
                    DeviceCategory = "Matrix Switch",
                    
                    MatrixSwitchEventArgs = new MatrixSwitchEventArgs()
                    {
                        DeviceName = MatrixSwitch.HostName,
                        EventName = "Route Complete",
                        Route = Route
                    }
                    
                });
            }
        }

        private void Client_DeviceConnected(object sender, EventArgs e)
        {
            DeviceConnectionEvent?.Invoke(this, e);
        }

        private void Client_DeviceDisconnected(object sender, EventArgs e)
        {
            DeviceDisconnectionEvent?.Invoke(this, e);
        }

        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            MatrixSwitch.SerialPort.Send("GET MAC\r\n");
        }

        public void MakeRoute(int inputIndex, int outputIndex)
        {
            CrestronConsole.PrintLine("Making the route");
            String command = String.Format("SET OUT{0} VS IN{1}\r\n", outputIndex, inputIndex);
            CrestronConsole.PrintLine("Route command {0}", command);
            MatrixSwitch.Client.SendString(command);
        }

        public void DisconnectSource(int inputIndex)
        {
            throw new NotImplementedException();
        }

        public void DisconnectDestination(int outputIndex)
        {
            throw new NotImplementedException();
        }
        
    }
}
