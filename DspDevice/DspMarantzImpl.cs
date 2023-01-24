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


namespace DspDevice
{
    public class DspMarantzImpl : IDsp
    {
        public event EventHandler<DspEventArgs> DspChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;

        public Dsp Dsp { get; set; }

        public DspMarantzImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }

        public void AdjustVolume( String levelName, int volume )
        {
            if ( volume == 1 ) // raise volume
            {
                int newVolume = (int) Dsp.Volume + 2;
                Dsp.Client.SendString($"MV{newVolume.ToString()}\n\r");
                Dsp.Client.SendString("MVUP\n\r");
            }
            if ( volume == 0 ) // lower volume
            {
                int newVolume = (int)Dsp.Volume - 2;
                Dsp.Client.SendString($"MV{newVolume.ToString()}\n\r");
                Dsp.Client.SendString("MVDOWN\n\r");
            }
        }

        public void GetMicMute( String levelName)
        {
            CrestronConsole.PrintLine($"Getting DSP mic mute {levelName} status");
        }

        public void GetVolume( String levelName)
        {
            Dsp.Client.SendString("MV?\r\n");
        }

        public void GetVolumeMute( String levelName )
        {
            Dsp.Client.SendString("MU?\r\n");
        }

        public void ToggleMicMute(string level)
        {
            if (this.Dsp.MicMute == 1) // mute speakers
            {
                CrestronConsole.PrintLine($"Setting DSP {level} to mic mute to off");
            }
            if (this.Dsp.MicMute == 0) // unmute speakers
            {
                CrestronConsole.PrintLine($"Setting DSP {level} to mic mute to off");
            }
        }

        public void SetMicMute( String levelName, int mute )
        {
            CrestronConsole.PrintLine($"Setting DSP {levelName} to {mute}");
        }

        public void SetVolume( String levelName, int volume )
        {
            Dsp.Client.SendString($"MV{volume.ToString()}\n\r");
        }

        public void SetVolumeMute(String levelName, int mute)
        {
            if (mute == 1) // mute speakers
            {
                Dsp.Client.SendString("MUON\r\n");
            }
            if (mute == 0) // unmute speakers
            {
                Dsp.Client.SendString("MUOFF\r\n");
            }
        }

        public void ToggleVolumeMute( String levelName )
        {
            if ( Dsp.VolumeMute == 1 ) // mute speakers
            {
                Dsp.Client.SendString("MUOFF\r\n");
            }
            if (Dsp.VolumeMute == 0 ) // unmute speakers
            {
                Dsp.Client.SendString("MUON\r\n");
            }
        }
        public void MakeCall(string number)
        {
            CrestronConsole.PrintLine($"Making ATC Call to {number}");
        }

        public void HangupCall()
        {
            CrestronConsole.PrintLine($"Hanging up ATC Call");
        }

        public void StoreNumber(int slot, string number)
        {
            if (this.Dsp.StoredNumbers.ContainsKey(slot))
            {
                this.Dsp.StoredNumbers[slot].StoredNumber = number;
            } else
            {
                ATCSpeedDialElement element = new ATCSpeedDialElement();
                element.StoredNumber = number;

                this.Dsp.StoredNumbers.Add(slot, element);
            }
            DspEventArgs dspEventArgs = new DspEventArgs();

            dspEventArgs.EventName = "StoredNumber";
            dspEventArgs.DeviceName = "DSP";
            dspEventArgs.PresetSlot = slot;
            dspEventArgs.PresetNumber = number;

            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "DSP",
                DspEventArgs = dspEventArgs

            });
        }
        public void StoreName(int slot, string name)
        {
            if (this.Dsp.StoredNumbers.ContainsKey(slot))
            {
                this.Dsp.StoredNumbers[slot].StoredName = name;
            }
            else
            {
                ATCSpeedDialElement element = new ATCSpeedDialElement();
                element.StoredName = name;

                this.Dsp.StoredNumbers.Add(slot, element);
            }
            DspEventArgs dspEventArgs = new DspEventArgs();

            dspEventArgs.EventName = "StoredName";
            dspEventArgs.DeviceName = "DSP";
            dspEventArgs.PresetSlot = slot;
            dspEventArgs.PresetNumber = name;

            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "DSP",
                DspEventArgs = dspEventArgs

            });
        }

        public void ConnectRawSocket( string name, string ipAddress, int deviceID )
        {
            this.Dsp = new Dsp(name, ipAddress, deviceID, 23);


            this.Dsp.Client.Enable = true;
            this.Dsp.Client.DataReceived += OnDataReceived;
            this.Dsp.Client.DeviceConnected += Client_DeviceConnected;
            this.Dsp.Client.DeviceDisconnected += Client_DeviceDisconnected;
            PollingTimer.Enabled = true;


            CrestronConsole.PrintLine($"Connecting to Dsp at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            
        }

        public void Reconnect()
        {
            if (this.Dsp.Client.IsConnected)
            {
                this.Dsp.Client.Enable = false;
                this.Dsp.Client.Enable = true;
            }
            else
            {
                this.Dsp.Client.Enable = false;
                this.Dsp.Client.Enable = true;
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //CrestronConsole.PrintLine("From SIMPL# Module: raw device response: " + e.DataString + "\r");
            DspEventArgs dspEventArgs = new DspEventArgs();

            if (e.DataString.Contains("MUON")) // Mute On
            {
                Dsp.VolumeMute = 1;
                dspEventArgs.EventName = "VolumeMute";
                dspEventArgs.DeviceName = "DSP";
                dspEventArgs.VolumeMute = 1;

            }
            if (e.DataString.Contains("MUOFF")) // Mute Off
            {
                Dsp.VolumeMute = 0;
                dspEventArgs.EventName = "VolumeMute";
                dspEventArgs.DeviceName = "DSP";
                dspEventArgs.VolumeMute = 0;
            }
            if (e.DataString.Contains("MV") && !e.DataString.Contains("MVMAX"))
            {
                String volume = e.DataString.Substring(2,2);
                Dsp.Volume = Int32.Parse(volume);
                dspEventArgs.EventName = "Volume";
                dspEventArgs.DeviceName = "DSP";
                dspEventArgs.Volume = Int32.Parse(volume);
                
            }
            DevicesControlled.DevicesControlled.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "DSP",
                DspEventArgs = dspEventArgs

            });

        }

        private void Client_DeviceConnected(object sender, EventArgs e)
        {
            DeviceConnectionEvent?.Invoke(this, e);
            PollingTimer.Start();
        }

        private void Client_DeviceDisconnected(object sender, EventArgs e)
        {
            DeviceDisconnectionEvent?.Invoke(this, e);
        }

        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dsp.Client.SendString("MV?\r\n");
        }
        protected void OnDspChangeEvent(DspEventArgs e)
        {
            DspChangeEvent?.Invoke(this, e);
        }

        
    }
}
