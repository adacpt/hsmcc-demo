using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace DisplayDevice
{
    public class DisplaySonySerialImpl : IDisplay
    {
        
        public Display Display { get; set; }
        

        public event EventHandler<DisplayEventArgs> DisplayChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;


        public DisplaySonySerialImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }

        // This is for SIMPL+
        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            this.Display = new Display(name, serialPort, baudRate);

            switch (baudRate)
            {
                case "9600":
                    this.Display.SerialPort.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                                                    ComPort.eComDataBits.ComspecDataBits8,
                                                    ComPort.eComParityType.ComspecParityNone,
                                                    ComPort.eComStopBits.ComspecStopBits1,
                                                    ComPort.eComProtocolType.ComspecProtocolRS232,
                                                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                                    false);
                    break;
            }

            CrestronConsole.PrintLine($"Connecting to display via serial");
        }


        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //CrestronConsole.PrintLine("From SIMPL# Module: raw device response: " + e.DataString + "\r");

            if (e.DataString.Contains("POWR0000000000000001")) // Power On
            {
                Display.Power = 1;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Power",
                    DeviceName = Display.HostName,
                    Power = Display.Power
                });
            }
            if (e.DataString.Contains("POWR0000000000000000")) // Power Off
            {
                Display.Power = 0;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Power",
                    DeviceName = Display.HostName,
                    Power = Display.Power
                });
            }
            if (e.DataString.Contains("INPT0000000100000001")) // Input 1
            {
                Display.Input = 1;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (e.DataString.Contains("INPT0000000100000002")) // Input 2
            {
                Display.Input = 2;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (e.DataString.Contains("INPT0000000100000003")) // Input 3
            {
                Display.Input = 3;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (e.DataString.Contains("INPT0000000100000004")) // Input 4
            {
                Display.Input = 4;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Input",
                    DeviceName = Display.HostName,
                    Input = Display.Input
                });
            }
            if (e.DataString.Contains("AMUT0000000000000001")) // Mute On
            {
                Display.VolumeMute = 1;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "VolumeMute",
                    DeviceName = Display.HostName,
                    VolumeMute = Display.VolumeMute
                });
            }
            if (e.DataString.Contains("AMUT0000000000000000")) // Mute Off
            {
                Display.VolumeMute = 0;
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "VolumeMute",
                    DeviceName = Display.HostName,
                    VolumeMute = Display.VolumeMute
                });
            }
            if (e.DataString.Contains("VOLU")) // Volume
            {
                string newVol = e.DataString.Remove(0, 20);
                
                Display.Volume = (int)double.Parse(newVol);
                this.OnDisplayChangeEvent(new DisplayEventArgs()
                {
                    EventName = "Volume",
                    DeviceName = Display.HostName,
                    Volume = Display.Volume
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
            Display.Client.SendString("*SEMADReth0############\n");
        }

        public void GetInput()
        {
            Display.Client.SendString("*SEINPT################\n");
        }

        public void GetMultiView()
        {
            throw new NotImplementedException();
        }

        public void GetPower()
        {
            Display.Client.SendString("*SEPOWR################\n");
        }

        public void GetVolume()
        {
            Display.Client.SendString("*SEVOLU################\n");
        }

        public void GetVolumeMute()
        {
            Display.Client.SendString("*SEAMUT################\n");
        }

        public void SetInput(int input)
        {
            switch ( input )
            {
                case 1:
                    Display.Client.SendString("*SCINPT0000000100000001\x0A");
                    break;
                case 2:
                    Display.Client.SendString("*SCINPT0000000100000002\x0A");
                    break;
                case 3:
                    Display.Client.SendString("*SCINPT0000000100000003\x0A");
                    break;
                case 4:
                    Display.Client.SendString("*SCINPT0000000100000004\x0A");
                    break;

            }
        }

        public void SetMultiView(int multiview)
        {
            throw new NotImplementedException();
        }

        public void SetPower(int power)
        {
            switch (power)
            {
                case 0:
                    Display.Client.SendString("*SCPOWR0000000000000000\x0A");
                    break;

                case 1:
                    Display.Client.SendString("*SCPOWR0000000000000001\x0A");
                    break;
            }
        }

        public void AdjustVolume(int volume)
        {
            switch (volume)
            {
                case 0:
                    Display.Client.SendString("*SCIRCC0000000000000030\x0A");
                    break;

                case 1:
                    Display.Client.SendString("*SCIRCC0000000000000031\x0A");
                    break;
            }
        }

        public void SetVolume(int volume)
        {
            if (volume < 10)
            {
                Display.Client.SendString($"*SCVOLU000000000000000{volume.ToString()}\x0A");
            }
            if (volume < 100)
            {
                Display.Client.SendString($"*SCVOLU00000000000000{volume.ToString()}\x0A");
            }
            if( volume == 100)
            {
                Display.Client.SendString("*SCVOLU0000000000000100\x0A");
            }
            
        }

        public void SetVolumeMute(int mute)
        {
            switch (mute)
            {
                case 0:
                    Display.Client.SendString("*SCAMUT0000000000000000\x0A");
                    break;

                case 1:
                    Display.Client.SendString("*SCAMUT0000000000000001\x0A");
                    break;
            }
        }

        public void Reconnect()
        {
            if (this.Display.Client.IsConnected)
            {
                this.Display.Client.Enable = false;
                this.Display.Client.Enable = true;
            }
            else
            {
                this.Display.Client.Enable = false;
                this.Display.Client.Enable = true;
            }
            
        }
    }
}
