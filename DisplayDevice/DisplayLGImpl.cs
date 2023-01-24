using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using CommunicationMethods;
using System.Text.RegularExpressions;

namespace DisplayDevice
{
    public class DisplayLGImpl : IDisplay
    {
        Display Display { get; set; }

        public event EventHandler<DisplayEventArgs> DisplayChangeEvent;
        public event EventHandler<EventArgs> DeviceConnectionEvent;
        public event EventHandler<EventArgs> DeviceDisconnectionEvent;
                
        public DisplayLGImpl()
        {

        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            CrestronConsole.PrintLine($"From SIMPL# Module: raw device response: {e.DataString}\r" );

            string deviceResponse = "";

            foreach (var b in e.DataBytes)
            {
                deviceResponse = deviceResponse + b.ToString("X2") + " ";
            }

            CrestronConsole.PrintLine($"From SIMPL# Module: converted string response: {deviceResponse}");

            // VOLUME MUTE

            if (deviceResponse.StartsWith("e"))
            {

                var valueDict = new Dictionary<string, int>()
                {
                    { "00", 0 },
                    { "01", 1 }                    
                };

                var keyValue = e.DataString.Substring(9, 2);

                if (valueDict.ContainsKey(keyValue))
                {

                    Display.VolumeMute = valueDict[keyValue];
                    this.OnDisplayChangeEvent(new DisplayEventArgs()
                    {
                        EventName = "VolumeMute",
                        DeviceName = Display.HostName,
                        VolumeMute = Display.VolumeMute
                    });
                }
                else 
                {
                    CrestronConsole.PrintLine("There has been an error in parsing the MUTE STATUS response.");
                }
            }

            // INPUT
            if (deviceResponse.StartsWith("b"))
            {
                var valueDict = new Dictionary<string, int>()
                {
                    { "90", 1 },
                    { "91", 2 },
                    { "92", 3 },
                    { "00", 4 }
                    
                };

                var keyValue = e.DataString.Substring(9, 2);

                if (valueDict.ContainsKey(keyValue))
                {

                    Display.Input = valueDict[keyValue];
                    this.OnDisplayChangeEvent(new DisplayEventArgs()
                    {
                        EventName = "Input",
                        DeviceName = Display.HostName,
                        Input = Display.Input
                    });
                }
                else
                {
                    CrestronConsole.PrintLine("There has been an error in parsing the INPUT STATUS response.");
                }
            }

            // MULTIVIEW
            if (deviceResponse.StartsWith("c"))
            {
                string[] words = deviceResponse.Split(' ');
                string multiScreenPhrase = words[3];
                string multiScreenInputs = multiScreenPhrase.Remove(multiScreenPhrase.Length - 1);
                int multiScreenInt = multiScreenInputs.Length / 2;
                
                if (multiScreenInt > 0)
                {

                    Display.MultiView = multiScreenInt;
                    this.OnDisplayChangeEvent(new DisplayEventArgs()
                    {
                        EventName = "MultiView",
                        DeviceName = Display.HostName,
                        MultiView = Display.MultiView
                    });
                }
                else
                {
                    CrestronConsole.PrintLine("There has been an error in parsing the MULTIVIEW STATUS response.");
                }
            }

            // POWER
            if (deviceResponse.StartsWith("a"))
            {
                var valueDict = new Dictionary<string, int>()
                {
                    { "00", 0 },
                    { "01", 1 }                    
                };

                var keyValue = e.DataString.Substring(9, 2);

                if (valueDict.ContainsKey(keyValue))
                {

                    Display.Power = valueDict[keyValue];
                    this.OnDisplayChangeEvent(new DisplayEventArgs()
                    {
                        EventName = "Power",
                        DeviceName = Display.HostName,
                        Power = Display.Power
                    });
                }
                else
                {
                    CrestronConsole.PrintLine("There has been an error in parsing the POWER STATUS response.");
                }
            }

            // VOLUME
            if (deviceResponse.StartsWith("f"))
            {
                string strVol = e.DataString.Substring(9, 2);

                try
                {
                    int intVol = int.Parse(strVol);
                    Display.Volume = intVol;
                    this.OnDisplayChangeEvent(new DisplayEventArgs()
                    {
                        EventName = "VolumeMute",
                        DeviceName = Display.HostName,
                        VolumeMute = Display.VolumeMute
                    });
                }
                catch
                {
                    CrestronConsole.PrintLine("There has been an error in parsing the VOLUME response.");
                }
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

        


        public void AdjustVolume(int volume)
        {
            if (volume >= 0 & volume <= 100)
            {
                Display.Client.SendString($"kf {Display.DeviceId} {volume:X2}\r"); // I need a two digit number at all times
            }
        }

        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Display.Client.SendString("\n");
        }

        public void ConnectRawSocket(string name, string ipAddress, int deviceID)
        {
            this.Display = new Display(name, ipAddress, deviceID, 41794); // the port comes from page 26 of manual

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
            CrestronConsole.PrintLine($"Connecting to display {Display.DeviceId} at {ipAddress}");
        }

        public void ConnectSerial(string name, ComPort serialPort, string baudRate)
        {
        }

        public void GetInput()
        {
            // the documentation says that this only works for serial communications
            Display.Client.SendString($"xb {Display.DeviceId} FF\r");
        }

        public void GetMultiView()
        {
            throw new NotImplementedException();
            //Display.Client.SendString($"xc {Display.DeviceId} FF \r"); 
            // There is no command to get multiview, not even in serial comms, but the call should create a response, which we can read.
            // add a onmultiviewchangeevent thing
        }

        public void GetPower()
        {
            // the documentation says that this only works for serial communications
            Display.Client.SendString($"ka {Display.DeviceId} FF\r");
        }

        public void GetVolume()
        {
            // the documentation says that this only works for serial communications
            Display.Client.SendString($"kf {Display.DeviceId} FF\r");
        }

        public void GetVolumeMute()
        {
            // the documentation says that this only works for serial communications
            Display.Client.SendString($"ke {Display.DeviceId} FF\r");
        }

        public void Reconnect()
        {
            this.Display.Client.Enable = true;
        }

        public void SetInput(int input)
        {
            var inputDict = new Dictionary<int, string>()
            {
                { 1, "90" },
                { 2, "91" },
                { 3, "92"}
            };

            if (inputDict.ContainsKey(input))
            {
                Display.Client.SendString($"xb {Display.DeviceId} {inputDict[input]}\r");
            }
            else
            {
                CrestronConsole.PrintLine($"The value you have entered, {input}, is not a valid input.");
            }            
        }

        public void SetMultiView(int multiview)
        {
            // write a getmultiview reader
            // add a onmultiviewchangeevent thing
            var inputDict = new Dictionary<int, string>()
            {
                { 1, "90" },
                { 2, "91" }
            };

            if (multiview == 2)
            {
                Display.Client.SendString($"xc {Display.DeviceId} 22 {inputDict[1]} {inputDict[multiview]} \r");
            }
            else if (multiview == 1)
            {
                Display.Client.SendString($"xb {Display.DeviceId} {inputDict[multiview]}\r");
            }
            else
            {
                CrestronConsole.PrintLine("The value you have entered for the multiview source is invalid.");
            }
        }

        public void SetPower(int power)
        {
            var powerDict = new Dictionary<int, string>()
            {
                {0, "00" },
                {1, "01" }
            };

            if(powerDict.ContainsKey(power))
            {
                Display.Client.SendString($"ka {Display.DeviceId} {powerDict[power]}\r");
            }
            else
            {
                CrestronConsole.PrintLine("You have entered an invalid power command value.");
            }
        }

        public void SetVolume(int volume)
        {
            if (volume >= 0 & volume <= 100)
            {
                string hexVolume = volume.ToString("X");

                Display.Client.SendString($"kf {Display.DeviceId} {hexVolume}\r"); // I need a two digit number at all times
            }
            else
            {
                CrestronConsole.PrintLine($"The value you have entered, {volume}, is not a valid input.");
            }
        }

        public void SetVolumeMute(int mute)
        {

            var muteDict = new Dictionary<int, string>()
            {
                { 0, "00" },
                { 1, "01" }
            };

            if (muteDict.ContainsKey(mute))
            {
                Display.Client.SendString($"ke {Display.DeviceId} {muteDict[mute]}\r");
            }

            else
            {
                CrestronConsole.PrintLine($"The value you have entered, {mute}, is invalid.");
            }            
        }
        
    }
}
