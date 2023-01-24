using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using CommunicationMethods;
using DevicesControlled;

namespace PowerControllerDevice
{
    public class PowerControllerGenericImpl : IPowerController
    {
        public event EventHandler<PowerControllerEventArgs> PowerControllerDeviceChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public PowerController PowerController { get; set; }

        public PowerControllerGenericImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }


        public void GetOutletPowerState(int outlet)
        {
            CrestronConsole.PrintLine("Getting the power state from Outlet {0}", outlet);
        }

        public void SetOutletPowerState(int outlet, int powerState)
        {
            CrestronConsole.PrintLine("Setting the power state for Outlet {0} to {1}", outlet, powerState);
        }

        public void ConnectRawSocket(string name, string ipAddress)
        {
            this.PowerController = new PowerController(name, ipAddress, 23);


            this.PowerController.Client.Enable = true;
            this.PowerController.Client.DataReceived += OnDataReceived;
            this.PowerController.Client.DeviceConnected += Client_DeviceConnected;
            this.PowerController.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to PowerController at {ipAddress}");
        }
        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            
        }

        public void Reconnect()
        {
            if (this.PowerController.Client.IsConnected)
            {
                this.PowerController.Client.Enable = false;
                this.PowerController.Client.Enable = true;
            }
            else
            {
                this.PowerController.Client.Enable = false;
                this.PowerController.Client.Enable = true;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //CrestronConsole.PrintLine("From SIMPL# Module: raw device response: " + e.DataString + "\r");
            PowerControllerEventArgs powerControllerEventArgs = new PowerControllerEventArgs();

            if (e.DataString.Contains("")) // 
            {
                

            }
            
            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlled.DevicesControlledEventArgs()
            {
                DeviceCategory = "Powercontroller",
                //PowerControllerEventArgs = powerControllerEventArgs
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
            PowerController.Client.SendString("MV?\r\n");
        }
        protected void OnDspChangeEvent(PowerControllerEventArgs e)
        {
            PowerControllerDeviceChangeEvent?.Invoke(this, e);
        }
    }
}
