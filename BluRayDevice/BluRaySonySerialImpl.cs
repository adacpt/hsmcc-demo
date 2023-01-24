using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using DevicesControlled;
using CommunicationMethods;



namespace BluRayDevice
{
    public class BluRaySonySerialImpl : IBluRay
    {
        
        public event EventHandler<BluRayEventArgs> BluRayDeviceChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public BluRay BluRay { get; set; }

        public BluRaySonySerialImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
            PollingTimer.Enabled = true;
        }

        public void Play()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Play Command");
        }

        public void Pause()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Pause Command");
        }

        public void Stop()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Stop Command");
        }

        public void NextChapter()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending NextChapter Command");
        }

        public void LastChapter()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending LastChapter Command");
        }

        public void FastForward()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending FastForward Command");
        }

        public void Rewind()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Rewind Command");
        }

        public void Number(int number)
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Number Command");
        }

        public void MenuUp()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending MenuUp Command");
        }

        public void MenuDown()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending MenuDown Command");
        }

        public void MenuLeft()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending MenuLeft Command");
        }

        public void MenuRight()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending MenuRight Command");
        }

        public void MenuEnter()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending MenuEnter Command");
        }

        public void Home()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Home Command");
        }

        public void Setup()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Setup Command");
        }

        public void Options()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Options Command");
        }

        public void Info()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Info Command");
        }

        public void Return()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending Return Command");
        }

        public void PopupMenu()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending PopupMenu Command");
        }

        public void TopMenu()
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending TopMenu Command");
        }

        public void SpecialKey(int key)
        {
            CrestronConsole.PrintLine($"{BluRay.HostName} Sending SpecialKey Command");
        }


        public void ConnectRawSocket(string name, string ipAddress)
        {
        }
        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            this.BluRay = new BluRay(name, serialPort, baudRate);

            switch (baudRate)
            {
                case "9600":
                    this.BluRay.SerialPort.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                                                    ComPort.eComDataBits.ComspecDataBits8,
                                                    ComPort.eComParityType.ComspecParityNone,
                                                    ComPort.eComStopBits.ComspecStopBits1,
                                                    ComPort.eComProtocolType.ComspecProtocolRS232,
                                                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                                    false);
                    break;
            }

            CrestronConsole.PrintLine($"Connecting to BluRay via serial");
        }

        public void Reconnect()
        {
            if (this.BluRay.Client.IsConnected)
            {
                this.BluRay.Client.Enable = false;
                this.BluRay.Client.Enable = true;
            }
            else
            {
                this.BluRay.Client.Enable = false;
                this.BluRay.Client.Enable = true;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //CrestronConsole.PrintLine("From SIMPL# Module: raw device response: " + e.DataString + "\r");
            BluRayEventArgs bluRayEventArgs = new BluRayEventArgs();

            if (e.DataString.Contains("MUON")) // Mute On
            {
                

            }
            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlled.DevicesControlledEventArgs()
            {
                DeviceCategory = "BluRay",
                BluRayEventArgs = bluRayEventArgs
            });

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
            BluRay.Client.SendString("MV?\r\n");
        }
        protected void OnBluRayChangeEvent(BluRayEventArgs e)
        {
            BluRayDeviceChangeEvent?.Invoke(this, e);
        }
    }
}

