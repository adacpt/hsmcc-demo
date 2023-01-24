using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace SignagePlayerDevice
{
    public class SignagePlayerSpinetixImpl : ISignagePlayer
    {
        
        public event EventHandler<SignagePlayerEventArgs> SignagePlayerDeviceChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public SignagePlayer SignagePlayer { get; set; }

        int timeZones = 0;

        public SignagePlayerSpinetixImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }

        public void ConnectRawSocket(string name, string ipAddress)
        {
            this.SignagePlayer = new SignagePlayer(name, ipAddress, 23, timeZones);


            this.SignagePlayer.Client.Enable = true;
            this.SignagePlayer.Client.DataReceived += OnDataReceived;
            this.SignagePlayer.Client.DeviceConnected += Client_DeviceConnected;
            this.SignagePlayer.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to SignagePlayer at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            
        }

        public void Reconnect()
        {
            if (this.SignagePlayer.Client.IsConnected)
            {
                this.SignagePlayer.Client.Enable = false;
                this.SignagePlayer.Client.Enable = true;
            }
            else
            {
                this.SignagePlayer.Client.Enable = false;
                this.SignagePlayer.Client.Enable = true;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            CrestronConsole.PrintLine($"From {SignagePlayer.HostName}: raw device response: " + e.DataString + "\r");

            if (e.DataString.Contains("")) // 
            {
                
            }
        }

        public void UpdateClassification(int classification)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating classification to {classification} \r");
        }

        public void UpdateMicMuteStatus(int micMute)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating mic mute status to {micMute} \r"); ;
        }

        public void UpdateSpeakerMuteStatus(int speakerMute)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating speaker mute to {speakerMute} \r");
        }

        public void UpdateVTCActiveStatus(int vtcActive)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating VTC Active status to {vtcActive} \r");
        }

        public void UpdateZoneHourOffset(int zoneHour, String activeZone)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating {activeZone} to {zoneHour} \r");
        }

        public void UpdateZoneMinuteOffset(int zoneMinute, String activeZone)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating {activeZone} to {zoneMinute} \r");
        }

        public void UpdateZoneName(string zoneName, String activeZone)
        {
            CrestronConsole.PrintLine($"{SignagePlayer.HostName} updating {activeZone} to {zoneName} \r");
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
            SignagePlayer.Client.SendString("MV?\r\n");
        }
        protected void OnSignagePlayerChangeEvent(SignagePlayerEventArgs e)
        {
            SignagePlayerDeviceChangeEvent?.Invoke(this, e);
            CrestronConsole.PrintLine($"From SIMPL# Module: invoking {SignagePlayer.HostName} SignagePlayerChangeEvent\r");
        }
        
    }
}
