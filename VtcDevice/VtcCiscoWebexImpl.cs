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

namespace VtcDevice
{
    public class VtcCiscoWebexImpl : IVtc
    {
        
        Vtc Vtc { get; set; }

        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public VtcCiscoWebexImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }

        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            this.Vtc = new Vtc(name, ipAddress, deviceID, 23);

            if (deviceID == 0)
            {
                Vtc.DeviceId = 1;
            }
            else
            {
                Vtc.DeviceId = deviceID;
            }

            this.Vtc.Client.Enable = true;
            this.Vtc.Client.DataReceived += OnDataReceived;
            this.Vtc.Client.DeviceConnected += Client_DeviceConnected;
            this.Vtc.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to VTC at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
        }

        public void Reconnect()
        {
            if (this.Vtc.Client.IsConnected)
            {
                this.Vtc.Client.Enable = false;
                this.Vtc.Client.Enable = true;
            }
            else
            {
                this.Vtc.Client.Enable = false;
                this.Vtc.Client.Enable = true;
            }
        }

        public void GetDtmf()
        {
            CrestronConsole.PrintLine("GET VTC Codec DTMF Status");
        }

        public void GetPresentation()
        {
            CrestronConsole.PrintLine("GET VTC Codec Presentation Status");
        }

        public void GetSelfview()
        {
            CrestronConsole.PrintLine("GET VTC Codec Selfview Status");
        }

        public void HangupCall(string callName)
        {
            CrestronConsole.PrintLine("Hangup VTC Call {0}", callName);
        }

        public void MakeCall(string number)
        {
            CrestronConsole.PrintLine("Make VTC Call {0}", number);
        }

        public void MovePipScreen()
        {
            CrestronConsole.PrintLine("Move PiP Screen");
        }

        public void SetDtmf(int dtmf)
        {
            if( dtmf == 0 )
            {
                CrestronConsole.PrintLine("Turn DTMF Status OFF");
            } else if ( dtmf == 1 )
            {
                CrestronConsole.PrintLine("Turn DTMF Status ON");
            }
        }

        public void SetPresentation(int presentation)
        {
            if (presentation == 0)
            {
                CrestronConsole.PrintLine("Turn VTC Presentation Status OFF");
            }
            else if (presentation == 1)
            {
                CrestronConsole.PrintLine("Turn VTC Presentation Status ON");
            }
        }

        public void SetSelfview(int selfview)
        {
            if (selfview == 0)
            {
                CrestronConsole.PrintLine("Turn VTC Selfview Status OFF");
            }
            else if (selfview == 1)
            {
                CrestronConsole.PrintLine("Turn VTC Selfview Status ON");
            }
        }

        public void StoreNumber(int slot, string number)
        {
            CrestronConsole.PrintLine($"Calling StoredNumber slot {slot} and {number}");
            if (this.Vtc.StoredNumbers.ContainsKey(slot))
            {
                this.Vtc.StoredNumbers[slot].StoredNumber = number;
            }
            else
            {
                VTCSpeedDialElement element = new VTCSpeedDialElement();
                element.StoredNumber = number;

                this.Vtc.StoredNumbers.Add(slot, element);
            }
            VtcEventArgs vtcEventArgs = new VtcEventArgs();

            vtcEventArgs.EventName = "StoredNumber";
            vtcEventArgs.DeviceName = "VTC";
            vtcEventArgs.PresetSlot = slot;
            vtcEventArgs.PresetNumber = number;

            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "VTC",
                VtcEventArgs = vtcEventArgs

            });
        }
        public void StoreName(int slot, string name)
        {
            CrestronConsole.PrintLine($"Seeing StoreName for slot {slot} and {name}");
            if (this.Vtc.StoredNumbers.ContainsKey(slot))
            {
                this.Vtc.StoredNumbers[slot].StoredName = name;
            }
            else
            {
                VTCSpeedDialElement element = new VTCSpeedDialElement();
                element.StoredName = name;

                this.Vtc.StoredNumbers.Add(slot, element);
            }
            VtcEventArgs vtcEventArgs = new VtcEventArgs();

            vtcEventArgs.EventName = "StoredName";
            vtcEventArgs.DeviceName = "VTC";
            vtcEventArgs.PresetSlot = slot;
            vtcEventArgs.PresetNumber = name;

            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "VTC",
                VtcEventArgs = vtcEventArgs

            });
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            CrestronConsole.PrintLine("From SIMPL# Module: raw device response: " + e.DataString + "\r");

            if (e.DataString.Contains("")) // 
            {
                // Call Status
                Vtc.CallStatus["Call1"] = 1;

                DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlled.DevicesControlledEventArgs()
                {
                    DeviceCategory = "VTC",
                    VtcEventArgs = new VtcEventArgs()
                    {
                        DeviceName = Vtc.HostName,
                        EventName = "Call Status",
                        CallStatus = Vtc.CallStatus
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
            Vtc.Client.SendString("\r\n");
        }
    }
}
