using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using DevicesControlled;
using CommunicationMethods;

namespace CatvDevice
{
    public class CatvContemporaryResearchSerialImpl : ICatv
    {
        public event EventHandler<CatvEventArgs> CatvChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public Catv Catv { get; set; }

        public CatvContemporaryResearchSerialImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }

        public void ChannelDown()
        {
            CrestronConsole.PrintLine("Sending Chanel Down Command");
        }

        public void ChannelUp()
        {
            CrestronConsole.PrintLine("Sending Chanel Down Command");
        }

        public void Exit()
        {
            CrestronConsole.PrintLine("Sending Exit Command");
        }

        public void GetChannel()
        {
            CrestronConsole.PrintLine("Sending Get Channel Command");
        }
        public void SetChannel(string channel)
        {
            CrestronConsole.PrintLine("Sending Set Channel {0} Command", channel);
        }

        public void Info()
        {
            CrestronConsole.PrintLine("Sending Info Command");
        }

        public void List()
        {
            CrestronConsole.PrintLine("Sending List Command");
        }

        public void Menu()
        {
            CrestronConsole.PrintLine("Sending Menu Command");
        }

        public void MenuDown()
        {
            CrestronConsole.PrintLine("Sending Menu Down Command");
        }

        public void MenuEnter()
        {
            CrestronConsole.PrintLine("Sending Menu Enter Command");
        }

        public void MenuLeft()
        {
            CrestronConsole.PrintLine("Sending Menu Left Command");
        }

        public void MenuRight()
        {
            CrestronConsole.PrintLine("Sending Menu Right Command");
        }

        public void MenuUp()
        {
            CrestronConsole.PrintLine("Sending Menu Up Command");
        }

        public void PreviousChannel()
        {
            CrestronConsole.PrintLine("Sending Previous Channel Command");
        }

        public void RecallPreset(int preset)
        {
            CrestronConsole.PrintLine($"Recalling Preset {0} Channel {1}", preset, this.Catv.ChannelPresets[preset]);
        }
        public void SavePreset(int preset)
        {
            CrestronConsole.PrintLine($"Saving Preset {0}", preset);
            if (this.Catv.ChannelPresets.ContainsKey(preset))
            {
                this.Catv.ChannelPresets[preset] = "123";
            } else
            {
                this.Catv.ChannelPresets.Add(preset, "123");
            }
            CatvEventArgs catvEventArgs = new CatvEventArgs();

            Catv.Channel = "";
            catvEventArgs.EventName = "ChannelPresets";
            catvEventArgs.DeviceName = "Tuner1";
            catvEventArgs.ChannelPresets = this.Catv.ChannelPresets;

            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "Catv",
                CatvEventArgs = catvEventArgs

            });
        }
        public void GetPresets()
        {
            CatvEventArgs catvEventArgs = new CatvEventArgs();

            Catv.Channel = "";
            catvEventArgs.EventName = "ChannelPresets";
            catvEventArgs.DeviceName = "Tuner1";
            catvEventArgs.ChannelPresets = this.Catv.ChannelPresets;

            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "Catv",
                CatvEventArgs = catvEventArgs

            });
        }
        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            this.Catv = new Catv(name, ipAddress, 23);


            this.Catv.Client.Enable = true;
            this.Catv.Client.DataReceived += OnDataReceived;
            this.Catv.Client.DeviceConnected += Client_DeviceConnected;
            this.Catv.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to Catv at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            this.Catv = new Catv(name, serialPort, baudRate);

            switch (baudRate)
            {
                case "9600":
                    this.Catv.SerialPort.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                                                    ComPort.eComDataBits.ComspecDataBits8,
                                                    ComPort.eComParityType.ComspecParityNone,
                                                    ComPort.eComStopBits.ComspecStopBits1,
                                                    ComPort.eComProtocolType.ComspecProtocolRS232,
                                                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                                    false);
                    break;
            }

            CrestronConsole.PrintLine($"Connecting to CATV via serial");
        }

        public void Reconnect()
        {
            if (this.Catv.Client.IsConnected)
            {
                this.Catv.Client.Enable = false;
                this.Catv.Client.Enable = true;
            }
            else
            {
                this.Catv.Client.Enable = false;
                this.Catv.Client.Enable = true;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            CrestronConsole.PrintLine("From SIMPL# Module: raw device response: " + e.DataString + "\r");
            CatvEventArgs catvEventArgs = new CatvEventArgs();

            if (e.DataString.Contains("STB")) // Something
            {
                Catv.Channel = "";
                catvEventArgs.EventName = "Channel";
                catvEventArgs.DeviceName = "Catv";
                catvEventArgs.Channel = "";

            }
            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "Catv",
                CatvEventArgs = catvEventArgs

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
            Catv.Client.SendString("MV?\r\n");
        }
        protected void OnDspChangeEvent(CatvEventArgs e)
        {
            CatvChangeEvent?.Invoke(this, e);
        }
    }
}
