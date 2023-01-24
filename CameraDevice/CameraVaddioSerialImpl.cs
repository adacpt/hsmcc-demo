using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using CommunicationMethods;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;


namespace CameraDevice
{
    public class CameraVaddioSerialImpl : ICamera
    {
        
        public Camera Camera;
        private int CameraMode;
        
        public event EventHandler<CameraEventArgs> PresetSavedEvent;
        public event EventHandler<CameraEventArgs> PresetRecallEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;
        public CameraVaddioSerialImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
            PollingTimer.Enabled = true;
        }

        // this constructor should be used by SIMPL# Pro in order to instantiate a new camera and socket when instantiating the implementation class
        // the SIMPL+ module can only use the default constructor, so it must call the Connect( string name, string ipAddress) method to instantiate a camera and socket
        
        public CameraVaddioSerialImpl(Camera camera)
        {
            this.Camera = camera;
            // check for ip address
            if( camera.IpAddress != null && camera.IpAddress.Length > 0 )
            {
                this.Camera.Client.Enable = true;
                this.Camera.Client.DataReceived += OnDataReceived;
            }
            else
            {
                CrestronConsole.PrintLine("The IP Address for the camera is not defined");
            }
            
        }

        public void ConnectRawSocket(string name, string ipAddress)
        {
            // check for ip address value

            this.Camera = new Camera(name, ipAddress, 100);
            this.Camera.Client.Enable = true;
            this.Camera.Client.DataReceived += OnDataReceived;
            PollingTimer.Enabled = true;
            CrestronConsole.PrintLine($"Connecting to camera at {ipAddress}");
        }
        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
            this.Camera = new Camera(name, serialPort, baudRate);

            switch (baudRate)
            {
                case "9600":
                    this.Camera.SerialPort.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                                                    ComPort.eComDataBits.ComspecDataBits8,
                                                    ComPort.eComParityType.ComspecParityNone,
                                                    ComPort.eComStopBits.ComspecStopBits1,
                                                    ComPort.eComProtocolType.ComspecProtocolRS232,
                                                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                                    false);
                    break;
            }

            CrestronConsole.PrintLine($"Connecting to VTcameraC via serial");
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            CrestronConsole.PrintLine("this is coming from the device: " + e.DataString);

            if (e.DataString.Contains("OK"))
            {
                if( CameraMode == 1 ) // preset recall mode
                {
                    this.OnPresetRecallEvent(new CameraEventArgs()
                    {
                        EventName = "Preset Recalled",
                        DeviceName = Camera.HostName,
                        PresetRecalled = (ushort)this.Camera.CurrentPreset
                    });
                }

                if (CameraMode == 2) // preset save mode
                {
                    this.OnPresetSavedEvent(new CameraEventArgs()
                    {
                        EventName = "Preset Saved",
                        DeviceName = Camera.HostName,
                        PresetRecalled = (ushort)this.Camera.CurrentPreset
                    });
                }

            }
        }

        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Camera.SerialPort.Send("MV?\r\n");
        }

        protected void OnPresetSavedEvent( CameraEventArgs e )
        {
            PresetSavedEvent?.Invoke(this, e);
            CrestronConsole.PrintLine("invoking PresetSavedEvent");
        }

        protected void OnPresetRecallEvent( CameraEventArgs e )
        {
            PresetRecallEvent?.Invoke(this, e);
            CrestronConsole.PrintLine("invoking PresetRecallEvent");
        }

        public void Pan(int direction, int speed )
        {
            switch (direction)
            {
                case 0: // pan left
                    this.Camera.SerialPort.Send($"camera pan left {speed.ToString()}\r" );
                    break;

                case 1: // pan right
                    this.Camera.SerialPort.Send($"camera pan right {speed.ToString()}\r");
                    break;
            }
        }

        public void Tilt(int direction, int speed)
        {
            switch (direction)
            {
                case 0: // tilt up
                    this.Camera.SerialPort.Send($"camera tilt up {speed.ToString()}\r");
                    break;

                case 1: // tilt down
                    this.Camera.SerialPort.Send($"camera pan down {speed.ToString()}\r");
                    break;
            }
        }

        public void Zoom(int direction, int speed)
        {
            switch (direction)
            {
                case 0: // zoom in
                    this.Camera.SerialPort.Send($"camera zoom in {speed.ToString()}\r");
                    break;

                case 1: // zoom out
                    this.Camera.SerialPort.Send($"camera zoom out {speed.ToString()}\r");
                    break;
            }
        }

        public void RecallPreset(int preset)
        {
            this.Camera.SerialPort.Send($"camera preset recall {preset.ToString()}\r");
            this.Camera.CurrentPreset = preset;
            CameraMode = 1;
        }

        public void SavePreset(int preset)
        {
            this.Camera.SerialPort.Send($"camera preset store {preset.ToString()}\r");
            this.Camera.CurrentPreset = preset;
            CameraMode = 2;
        }

        public void Stop()
        {
            this.Camera.SerialPort.Send($"camera stop\r");
        }

        public void Pan(int direction, int speed, int action, string controlName)
        {
            this.Camera.SerialPort.Send($"camera pan {direction}\r");
        }

        public void Tilt(int direction, int speed, int action, string controlName)
        {
            this.Camera.SerialPort.Send($"camera tilt {direction}\r");
        }

        public void Zoom(int direction, int speed, int action, string controlName)
        {
            this.Camera.SerialPort.Send($"camera zoom {direction}\r");
        }

        public void Stop(string controlName)
        {
            this.Camera.SerialPort.Send($"camera {controlName} stop\r");
        }

        public void RecallPreset(int preset, string controlName)
        {
            throw new NotImplementedException();
        }

        public void SavePreset(int preset, string controlName)
        {
            throw new NotImplementedException();
        }

        public void Power(int power, string controlName)
        {
            throw new NotImplementedException();
        }

        public void Home(string controlName)
        {
            throw new NotImplementedException();
        }

        public void RecallPosition(int pan, int tilt, int zoom)
        {
            throw new NotImplementedException();
        }
        
    }
}
