using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using System.Text.RegularExpressions;

namespace DisplayDevice
{
    public class DisplaySamsungImpl : IDisplay
    {
        
        Display Display { get; set; }

        public event EventHandler<DisplayEventArgs> DisplayChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public DisplaySamsungImpl()
        {
            // default constructor
        }

        // commands and responses in this implementation assume the default device ID of 00
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            string deviceResponse = "";

            foreach (var b in e.DataBytes)
            {
                deviceResponse = deviceResponse + b.ToString("X2") + " ";
            }
            
            CrestronConsole.PrintLine($"From SIMPL# Module: raw {Display.HostName} device response: {deviceResponse} \r");

            if (deviceResponse.Contains("AA FF 00 03 41 11 01 55")) // Power On
            {
                Display.Power = 1;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Power",
                    DeviceName = Display.HostName,
                    Power = Display.Power
                });
                Task.Delay(10000); // wait 10 seconds to reconnect the socket (Samsung specific)
                Reconnect();
            }
            if (deviceResponse.Contains("AA FF 00 03 41 11 00 54")) // Power Off
            {
                Display.Power = 0;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Power",
                    DeviceName = Display.HostName,
                    Power = Display.Power
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 21 78") || deviceResponse.Contains("AA FF 00 0341 14 22 79")) // Input 1: HDMI1 or HDMI1-PC
            {
                Display.Input = 1;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 23 7A") || deviceResponse.Contains("AA FF 00 03 41 14 24 7B")) // Input 2: HDMI2 or HDMI2-PC
            {
                Display.Input = 2;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 31 88") || deviceResponse.Contains("AA FF 00 03 41 14 32 89")) // Input 3: HDMI3 or HDMI3-PC
            {
                Display.Input = 3;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 33 8A") || deviceResponse.Contains("AA FF 00 03 41 14 34 8B")) // Input 4: HDMI4 or HDMI4-PC
            {
                Display.Input = 4;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 25 7C")) // Input 5: DisplayPort1
            {
                Display.Input = 5;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 26 7D")) // Input 6: DisplayPort2
            {
                Display.Input = 6;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 14 18 6F") || deviceResponse.Contains("AA FF 00 03 41 14 1F 76")) // Input 7: DVI or DVI_video
            {
                Display.Input = 7;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 12")) // Volume Level - this string is appended by the volume level and checksum
            {
                string dataVolume;
                int convertedVolume;

                dataVolume = deviceResponse.Substring(18, 2); // remove all but volume data from the data received
                convertedVolume = int.Parse(dataVolume, System.Globalization.NumberStyles.HexNumber); //convert hex string to int

                Display.Volume = convertedVolume;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Volume",
                    DeviceName = Display.HostName,
                    Volume = Display.Volume
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 13 01 57")) // Volume Mute On
            {
                Display.VolumeMute = 1;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "VolumeMute",
                    DeviceName = Display.HostName,
                    VolumeMute = Display.VolumeMute
                });
            }
            if (deviceResponse.Contains("AA FF 00 03 41 13 00 56")) // Volume Mute Off
            {
                Display.VolumeMute = 0;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "VolumeMute",
                    DeviceName = Display.HostName,
                    VolumeMute = Display.VolumeMute
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
        protected void OnDisplayChangeEvent(DisplayEventArgs e)
        {
            DisplayChangeEvent?.Invoke(this, e);
            CrestronConsole.PrintLine($"From SIMPL# Module: invoking {Display.HostName} DisplayChangeEvent\r");
        }

        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Display.Client.SendString("\n");
        }

        // This is for SIMPL+
        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            this.Display = new Display(name, ipAddress, deviceID, 1515);

            if (deviceID == 0)
            {
                Display.DeviceId = 1;
            }
            else
            {
                Display.DeviceId = deviceID;
            }

            this.Display.Client.Enable = true;
            this.Display.Client.DataReceived += OnDataReceived;
            this.Display.Client.DeviceConnected += Client_DeviceConnected;
            this.Display.Client.DeviceDisconnected += Client_DeviceDisconnected;
            //this.Display.Client.pollingTimer.Elapsed += this.OnPollingTimerElapsed;

            CrestronConsole.PrintLine($"Connecting to {Display.HostName} at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
        }

        public void GetInput()
        {
            byte[] command = { 0xAA, 0x14, 0x00, 0x00, 0x14 };
            Display.Client.SendBytes(command);
        }

        public void GetMultiView()
        {
            throw new NotImplementedException(); // Not on Samsung QM Series documentation
        }

        public void GetPower()
        {
            byte[] command = { 0xAA, 0x11, 0x00, 0x00, 0x11 };
            Display.Client.SendBytes(command);
        }

        public void GetVolume()
        {
            byte[] command = { 0xAA, 0x12, 0x00, 0x00, 0x12 };
            Display.Client.SendBytes(command);
        }

        public void GetVolumeMute()
        {
            byte[] command = { 0xAA, 0x13, 0x00, 0x00, 0x13 };
            Display.Client.SendBytes(command);
        }

        public void SetInput(int input)
        {
            switch(input)
            {
                case 1: // Set Input 1: HDMI1
                    byte[] command1 = { 0xAA, 0x14, 0x00, 0x01, 0x21, 0x36 };
                    Display.Client.SendBytes(command1);
                    break;

                case 2: // Set Input 2: HDMI2
                    byte[] command2 = { 0xAA, 0x14, 0x00, 0x01, 0x23, 0x38 };
                    Display.Client.SendBytes(command2);
                    break;

                case 3: // Set Input 3: HDMI3
                    byte[] command3 = { 0xAA, 0x14, 0x00, 0x01, 0x31, 0x46 };
                    Display.Client.SendBytes(command3);
                    break;

                case 4: // Set Input 4: HDMI4
                    byte[] command4 = { 0xAA, 0x14, 0x00, 0x01, 0x33, 0x48 };
                    Display.Client.SendBytes(command4);
                    break;

                case 5: // Set Input 5: DisplayPort1
                    byte[] command5 = { 0xAA, 0x14, 0x00, 0x01, 0x25, 0x3A };
                    Display.Client.SendBytes(command5);
                    break;

                case 6: // Set Input 6: DisplayPort2
                    byte[] command6 = { 0xAA, 0x14, 0x00, 0x01, 0x26, 0x3B };
                    Display.Client.SendBytes(command6);
                    break;

                case 7: // Set Input 7: DVI
                    byte[] command7 = { 0xAA, 0x14, 0x00, 0x01, 0x18, 0x33 };
                    Display.Client.SendBytes(command7);
                    break;

                default:
                    CrestronConsole.PrintLine($"From SIMPL# Module: {Display.HostName} Display Input not valid");
                    break;
            }
        }

        public void SetMultiView(int multiview)
        {
            throw new NotImplementedException(); // Not on Samsung QM Series documentation
        }

        public void SetPower(int power)
        {
            switch (power)
            {
                case 0: // Set Power Off
                    byte[] command0 = { 0xAA, 0x11, 0x00, 0x01, 0x00, 0x12 };
                    Display.Client.SendBytes(command0);
                    break;

                case 1: // Set Power On
                    byte[] command1 = { 0xAA, 0x11, 0x00, 0x01, 0x01, 0x13 };
                    Display.Client.SendBytes(command1);
                    Task.Delay(2000); // wait 2 seconds and if there was no ACK, send the command again
                    if (Display.Power == 0)
                    {
                        Display.Client.SendBytes(command1);
                    }
                    Task.Delay(2000); // wait 2 seconds and if there was no ACK, send the command again
                    if (Display.Power == 0)
                    {
                        Display.Client.SendBytes(command1);
                    }
                    break;
            }
        }

        public void AdjustVolume(int volume)
        {
            GetVolume();
            switch(volume)
            {
                case 0: // Adjust Volume Up
                    //byte[] command0 = { 0xAA, 0x62, 0x00, 0x01, 0x00, 0x63 };
                    //Display.Client.SendBytes(command0);
                    if(Display.Volume <= 95)
                    {
                        var newVolumeUp = Display.Volume + 5;
                        SetVolume(newVolumeUp);
                    }
                    else
                    {
                        var maxVolume = 100;
                        SetVolume(maxVolume);
                    }
                    break;

                case 1: // Adjust Volume Down
                    //byte[] command1 = { 0xAA, 0x62, 0x00, 0x01, 0x01, 0x64 };
                    //Display.Client.SendBytes(command1);
                    if (Display.Volume >= 5)
                    {
                        var newVolumeDown = Display.Volume - 5;
                        SetVolume(newVolumeDown);
                    }
                    else
                    {
                        var minVolume = 0;
                        SetVolume(minVolume);
                    }
                    break;
            }
        }

        public void SetVolume(int volume)
        {
            if (volume <= 100 && volume >= 0)
            {
                byte[] command = { 0xAA, 0x12, 0x00, 0x01, (byte)volume, (byte)(19 + volume) };
                Display.Client.SendBytes(command);
            }
            else
            {
                CrestronConsole.PrintLine($"Unable to set volume of {volume} on {Display.HostName} at {Display.IpAddress}");
            }
        }

        public void SetVolumeMute(int mute)
        {
            switch (mute)
            {
                case 0: // Set Volume Mute Off
                    byte[] command0 = { 0xAA, 0x13, 0x00, 0x01, 0x00, 0x14 };
                    Display.Client.SendBytes(command0);
                    break;

                case 1: // Set Volume Mute On
                    byte[] command1 = { 0xAA, 0x13, 0x00, 0x01, 0x01, 0x15 };
                    Display.Client.SendBytes(command1);
                    break;
            }
        }

        public void Reconnect()
        {
            this.Display.Client.Enable = false;
            this.Display.Client.Enable = true;
        }
        
    }
}
