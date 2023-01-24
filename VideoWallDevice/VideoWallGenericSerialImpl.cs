using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace VideoWallDevice
{
    public class VideoWallGenericSerialImpl : IVideoWall
    {
        
        public event EventHandler<VideoWallEventArgs> VideoWallDeviceChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public VideoWall VideoWall { get; set; }

        public VideoWallGenericSerialImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
            PollingTimer.Enabled = true;
        }

        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
        }
        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            this.VideoWall = new VideoWall(name, serialPort, baudRate);

            switch (baudRate)
            {
                case "9600":
                    this.VideoWall.SerialPort.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                                                    ComPort.eComDataBits.ComspecDataBits8,
                                                    ComPort.eComParityType.ComspecParityNone,
                                                    ComPort.eComStopBits.ComspecStopBits1,
                                                    ComPort.eComProtocolType.ComspecProtocolRS232,
                                                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                                    false);
                    break;
            }

            CrestronConsole.PrintLine($"Connecting to VTC via serial");
        }

        public void GetLayout()
        {
            CrestronConsole.PrintLine("GET the current video wall layout");
        }

        public void SetLayout(int layout)
        {
            CrestronConsole.PrintLine($"SET the current video wall layout to layout {layout}");
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            CrestronConsole.PrintLine("From Video Wall: raw device response: " + e.DataString + "\r");

            if (e.DataString.Contains("")) // 
            {
                VideoWall.Layout = 1;
                this.OnVideoWallChangeEvent(new VideoWallEventArgs()
                {
                    EventName = "Layout",
                    Layout = 1
                });
            }
        }

        public void Reconnect()
        {
            if (this.VideoWall.Client.IsConnected)
            {
                this.VideoWall.Client.Enable = false;
                this.VideoWall.Client.Enable = true;
            }
            else
            {
                this.VideoWall.Client.Enable = false;
                this.VideoWall.Client.Enable = true;
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
            VideoWall.SerialPort.Send("MV?\r\n");
        }
        protected void OnVideoWallChangeEvent(VideoWallEventArgs e)
        {
            VideoWallDeviceChangeEvent?.Invoke(this, e);
            CrestronConsole.PrintLine($"From SIMPL# Module: invoking {VideoWall.HostName} VideoWallChangeEvent\r");
        }
        
    }
}
