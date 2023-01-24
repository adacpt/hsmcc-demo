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
    public class CameraVaddioImpl : ICamera
    {
        
        public Camera Camera;
        private int CameraMode;
        
        public event EventHandler<CameraEventArgs> PresetSavedEvent;
        public event EventHandler<CameraEventArgs> PresetRecallEvent;

        public Timer PollingTimer;
        public int PollingTime { get; set; } = 5000;
        public CameraVaddioImpl()
        {
            PollingTimer = new Timer(PollingTime);
            PollingTimer.Elapsed += this.OnPollingTimerElapsed;
            PollingTimer.AutoReset = true;
        }

        // this constructor should be used by SIMPL# Pro in order to instantiate a new camera and socket when instantiating the implementation class
        // the SIMPL+ module can only use the default constructor, so it must call the Connect( string name, string ipAddress) method to instantiate a camera and socket
        
        public CameraVaddioImpl(Camera camera)
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
            this.Camera.Client.SendString("MV?\r\n");
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
                    CrestronConsole.PrintLine($"camera pan left {speed.ToString()}\r");
                    this.Camera.Client.SendString($"camera pan left {speed.ToString()}\r" );
                    break;

                case 1: // pan right
                    CrestronConsole.PrintLine($"camera pan right {speed.ToString()}\r");
                    this.Camera.Client.SendString($"camera pan right {speed.ToString()}\r");
                    break;
            }
        }

        public void Tilt(int direction, int speed)
        {
            switch (direction)
            {
                case 0: // tilt up
                    CrestronConsole.PrintLine($"camera tilt up {speed.ToString()}\r");
                    this.Camera.Client.SendString($"camera tilt up {speed.ToString()}\r");
                    break;

                case 1: // tilt down
                    CrestronConsole.PrintLine($"camera pan down {speed.ToString()}\r");
                    this.Camera.Client.SendString($"camera pan down {speed.ToString()}\r");
                    break;
            }
        }

        public void Zoom(int direction, int speed)
        {
            switch (direction)
            {
                case 0: // zoom in
                    CrestronConsole.PrintLine($"camera zoom in {speed.ToString()}\r");
                    this.Camera.Client.SendString($"camera zoom in {speed.ToString()}\r");
                    break;

                case 1: // zoom out
                    CrestronConsole.PrintLine($"camera zoom out {speed.ToString()}\r");
                    this.Camera.Client.SendString($"camera zoom out {speed.ToString()}\r");
                    break;
            }
        }

        public void RecallPreset(int preset)
        {
            CrestronConsole.PrintLine($"camera preset recall {preset.ToString()}\r");
            this.Camera.Client.SendString($"camera preset recall {preset.ToString()}\r");
            this.Camera.CurrentPreset = preset;
            CameraMode = 1;
        }

        public void SavePreset(int preset)
        {
            CrestronConsole.PrintLine($"camera preset store {preset.ToString()}\r");
            this.Camera.Client.SendString($"camera preset store {preset.ToString()}\r");
            this.Camera.CurrentPreset = preset;
            CameraMode = 2;
        }

        public void Stop()
        {
            CrestronConsole.PrintLine($"camera stop\r");
            this.Camera.Client.SendString($"camera stop\r");
        }

        public void Pan(int direction, int speed, int action, string controlName)
        {
            throw new NotImplementedException();
        }

        public void Tilt(int direction, int speed, int action, string controlName)
        {
            throw new NotImplementedException();
        }

        public void Zoom(int direction, int speed, int action, string controlName)
        {
            throw new NotImplementedException();
        }

        public void Stop(string controlName)
        {
            throw new NotImplementedException();
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
