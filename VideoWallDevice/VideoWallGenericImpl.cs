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
    public class VideoWallGenericImpl : IVideoWall
    {
        
        public event EventHandler<VideoWallEventArgs> VideoWallDeviceChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public VideoWall VideoWall { get; set; }

        public VideoWallGenericImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
            PollingTimer.Enabled = true;
        }

        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            this.VideoWall = new VideoWall(name, ipAddress, deviceID, 23);


            this.VideoWall.Client.Enable = true;
            this.VideoWall.Client.DataReceived += OnDataReceived;
            this.VideoWall.Client.DeviceConnected += Client_DeviceConnected;
            this.VideoWall.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to VideoWall at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
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
            VideoWall.Client.SendString("MV?\r\n");
        }
        protected void OnVideoWallChangeEvent(VideoWallEventArgs e)
        {
            VideoWallDeviceChangeEvent?.Invoke(this, e);
            CrestronConsole.PrintLine($"From SIMPL# Module: invoking {VideoWall.HostName} VideoWallChangeEvent\r");
        }
        
    }
}
