using System;
using System.Text;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharpPro.UI;
using Crestron.SimplSharp.CrestronIO;
using Physical;
using UserInteraction;
using MediaTransport;
using DevicesControlled;
using Newtonsoft.Json;
using System.Collections.Generic;




namespace MAVETesting
{
    public class ControlSystem : CrestronControlSystem
    {
        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        /// 
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        /// 
        /// You cannot send / receive data in the constructor
        /// </summary>
        /// 
        public List<XpanelForSmartGraphics> MyXPanels;
        public XpanelForSmartGraphics MyXPanel; // declare a global XPanel variable
        public IPhysical PhysicalImpl;
        public IUserInteraction UserInteractionImpl;
        public MediaTransportImplJson MediaTransportImpl;
        public DevicesControlledImplJson DevicesControlledImpl;

        public string physicalPillarFileName = "/user/PhysicalPillarJsonConfig.json";
        public string uiPillarFileName = "/user/UserInteractionPillarJsonConfig.json";
        public string mediaTransportPillarFileName = "/user/MediaTransportPillarJsonConfig.json";
        public string devicesControlledPillarFileName = "/user/DevicesControlledPillarJsonConfig.json";
        public string PhysicalPillarJsonConfig = "";
        public string UserInteractionPillarJsonConfig = "";
        public string MediaTransportPillarJsonConfig = "";
        public string DevicesControlledPillarJsonConfig = "";


        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                MyXPanels = new List<XpanelForSmartGraphics>();

                MyXPanel = new XpanelForSmartGraphics(0x31, this); // instantiate the XpanelForSmartGraphics object
                MyXPanel.SigChange += this.MyXPanel_SigChange;
                MyXPanel.OnlineStatusChange += this.MyXPanel_OnlineStatusChange;
                MyXPanels.Add(MyXPanel);
                

                string SGDFilePath = Path.Combine(Directory.GetApplicationDirectory(), "/user/V Corps G3 JOC 1.sgd");

                if (MyXPanel.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Error in registration on panel {0}", MyXPanel.RegistrationFailureReason);
                } else
                {
                    if (File.Exists(SGDFilePath))
                    {
                        CrestronConsole.PrintLine("\rSGD File Found!");
                        MyXPanel.LoadSmartObjects(SGDFilePath);

                        foreach ( KeyValuePair<uint, SmartObject> pair in MyXPanel.SmartObjects)
                        {
                            pair.Value.SigChange += MySmartObjectSigChange;
                        }
                    }
                    else
                    {
                        CrestronConsole.PrintLine("\rSGD File Not Found!");
                    }
                }

                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {
                // Instantiate Pillar Implementation Classes
                PhysicalImpl = new PhysicalImplJson();
                //UserInteractionImpl = new UserInteractionImplJson(MyXPanels);
                MediaTransportImpl = new MediaTransportImplJson();
                DevicesControlledImpl = new DevicesControlledImplJson();

                //Read Pillar JSON files as raw text and assign to global Pillar JSON configuration string variables
                PhysicalPillarJsonConfig = ReadJSONConfig(physicalPillarFileName);
                UserInteractionPillarJsonConfig = ReadJSONConfig(uiPillarFileName);
                MediaTransportPillarJsonConfig = ReadJSONConfig(mediaTransportPillarFileName);
                DevicesControlledPillarJsonConfig = ReadJSONConfig(devicesControlledPillarFileName);

                // Call GetPillarConfigValues method for each Pillar and pass raw JSON configuration text from above
                //PhysicalImpl.GetPillarConfigValues(PhysicalPillarJsonConfig);
                //UserInteractionImpl.GetPillarConfigValues(UserInteractionPillarJsonConfig);
                //MediaTransportImpl.InitializeMediaTransportValues(MediaTransportPillarJsonConfig);
                DevicesControlledImpl.InitializeDevicesControlledValues(DevicesControlledPillarJsonConfig);

                // Each Pillar subscribes to each other Pillar's Event publisher
                
                //Subscriptions to Physical Pillar Event publisher
                //Physical.Physical.PhysicalChangeEvent += UserInteractionImpl.OnPhysicalChangeEvent;
                //Physical.Physical.PhysicalChangeEvent += MediaTransportImpl.OnPhysicalChangeEvent;
                Physical.Physical.PhysicalChangeEvent += DevicesControlledImpl.OnPhysicalChangeEvent;

                //Subscriptions to MediaTransport Pillar Event publisher
                //MediaTransport.MediaTransport.MediaTransportChangeEvent += UserInteractionImpl.OnMediaTransportChangeEvent;
                MediaTransport.MediaTransport.MediaTransportChangeEvent += DevicesControlledImpl.OnMediaTransportChangeEvent;
                //MediaTransport.MediaTransport.MediaTransportChangeEvent += PhysicalImpl.OnMediaTransportChangeEvent;

                //Subscriptions to DevicesControlled Pillar Event publisher
                //DevicesControlled.DevicesControlled.DevicesControlledChangeEvent += UserInteractionImpl.OnDevicesControlledChangeEvent;
                //DevicesControlled.DevicesControlled.DevicesControlledChangeEvent += PhysicalImpl.OnDevicesControlledChangeEvent;
                //DevicesControlled.DevicesControlled.DevicesControlledChangeEvent += MediaTransportImpl.OnDevicesControlledChangeEvent;


            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        public string ReadJSONConfig(string incoming)
        {
            string json = "";
            try
            {
                if ( File.Exists(incoming))
                {
                    using (StreamReader sr = new StreamReader(incoming, System.Text.Encoding.Default))
                        json = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("\rError attempting to open and read {0}. Error code {1}", incoming, e.Message);
            }
            return json;
        }


        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        /// 
        private void MyXPanel_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            if (currentDevice == MyXPanel)
            {
                if (args.DeviceOnLine)
                {
                    ErrorLog.Error("myXpanel is ONLINE");
                }
                else
                {
                    ErrorLog.Error("myXpanel is OFFLINE");
                }
            }
        }

        public Dictionary<uint, ButtonHoldCheck> regularHoldCheck = new Dictionary<uint, ButtonHoldCheck>();
        private void MyXPanel_SigChange(BasicTriList currentDevice, SigEventArgs args)
        {
            if (currentDevice == MyXPanel)
            {
                CrestronConsole.PrintLine("Touch Panel params: number = {0}, name = {1}",  args.Sig.Number, args.Sig.Name);
                switch (args.Sig.Type)
                {
                    case eSigType.NA:
                        break;
                    case eSigType.Bool:
                        DigitalState value = new DigitalState();
                        if (args.Sig.BoolValue == true)
                        {
                            value = DigitalState.On;
                            

                        }
                        else
                        {
                            value = DigitalState.Off;
                        }
                        UserInteractionButtonEventArgs booleanButtonArgs = new UserInteractionButtonEventArgs()
                        {
                            SmartGraphic = ESmartGraphicID.NA,
                            Button = args.Sig.Number,
                            Type = ButtonType.Digital,
                            DigitalValue = value
                        };
                        UserInteractionEventArgs UIEventArgs = new UserInteractionEventArgs()
                        {
                            TouchPanel = "Admin",
                            EventType = "Button",
                            ButtonArgs = booleanButtonArgs
                        };

                        //UserInteraction.UserInteraction.OnUserInteractionChangeEvent(UIEventArgs);
                        if (regularHoldCheck.ContainsKey(args.Sig.Number))
                        {
                            CrestronConsole.PrintLine("Button is in Dicionary");
                            CheckForButtonHold(regularHoldCheck[args.Sig.Number], UIEventArgs);
                        } else
                        {
                            ButtonHoldCheck newButton = new ButtonHoldCheck(UIEventArgs);
                            //newButton.Held += UserInteractionImpl.OnUserInteractionChangeHoldEvent;
                            //newButton.Held += DevicesControlledImpl.OnUserInteractionChangeHoldEvent;

                            //newButton.MomentaryPress += UserInteractionImpl.OnUserInteractionChangeMomentaryEvent;
                            //newButton.MomentaryPress += MediaTransportImpl.OnUserInteractionChangeMomentaryEvent;
                            //newButton.MomentaryPress += PhysicalImpl.OnUserInteractionChangeMomentaryEvent;
                            //newButton.MomentaryPress += DevicesControlledImpl.OnUserInteractionChangeMomentaryEvent;
                            regularHoldCheck.Add(args.Sig.Number, newButton);
                            CrestronConsole.PrintLine("Button is being added to Dicionary");
                            CheckForButtonHold(regularHoldCheck[args.Sig.Number], UIEventArgs);
                        }

                        
                        break;
                    case eSigType.UShort:
                        UserInteractionButtonEventArgs analogButtonArgs = new UserInteractionButtonEventArgs()
                        {
                            Button = args.Sig.Number,
                            Type = ButtonType.Analog,
                            AnalogValue = args.Sig.UShortValue
                        };
                        UserInteraction.UserInteraction.OnUserInteractionChangeEvent(new UserInteractionEventArgs()
                        {
                            TouchPanel = "Admin",
                            EventType = "Button",
                            ButtonArgs = analogButtonArgs
                        });
                        break;
                    case eSigType.String:
                        UserInteractionButtonEventArgs serialButtonArgs = new UserInteractionButtonEventArgs()
                        {
                            Button = args.Sig.Number,
                            Type = ButtonType.Serial,
                        };
                        UserInteraction.UserInteraction.OnUserInteractionChangeEvent(new UserInteractionEventArgs()
                        {
                            TouchPanel = "Admin",
                            EventType = "Button",
                            ButtonArgs = serialButtonArgs
                        });
                        break;
                }
            }
        }

        private void CheckForButtonHold(ButtonHoldCheck btn, UserInteractionEventArgs args)
        {   
            if (args.ButtonArgs.DigitalValue == DigitalState.On)
            {
                btn.ButtonState = true;
                CrestronConsole.PrintLine("Turning on button {0}", args.ButtonArgs.Button);
            } else
            {
                btn.ButtonState = false;
                CrestronConsole.PrintLine("Turning off button {0}", args.ButtonArgs.Button);
            }
        }


        public Dictionary<String, ButtonHoldCheck> smartGraphicHoldCheck = new Dictionary<String, ButtonHoldCheck>();
        private void MySmartObjectSigChange(GenericBase currentDevice, SmartObjectEventArgs args)
        {
            CrestronConsole.PrintLine("Smart Object params: signal = {0}, number = {1}, name = {2}", args.SmartObjectArgs.ID, args.Sig.Number, args.Sig.Name);


            DigitalState value = new DigitalState();
            if (args.Sig.BoolValue == true)
            {
                value = DigitalState.On;
            }
            else
            {
                value = DigitalState.Off;
                
            }
            UserInteractionButtonEventArgs booleanButtonArgs = new UserInteractionButtonEventArgs()
            {
                SmartGraphic = (ESmartGraphicID)args.SmartObjectArgs.ID,
                Name = args.Sig.Name,
                Button = args.Sig.Number,
                Type = ButtonType.Digital,
                DigitalValue = value
            };
            UserInteractionEventArgs UIEventArgs = new UserInteractionEventArgs()
            {
                TouchPanel = "Admin",
                EventType = "Button",
                ButtonArgs = booleanButtonArgs
            };

            //UserInteraction.UserInteraction.OnUserInteractionChangeEvent(UIEventArgs);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(args.SmartObjectArgs.ID.ToString());
            sb.Append(args.Sig.Number.ToString());
            

            if (smartGraphicHoldCheck.ContainsKey(sb.ToString()))
            {
                CrestronConsole.PrintLine("Button is in Dicionary");
                CheckForButtonHold(smartGraphicHoldCheck[sb.ToString()], UIEventArgs);
            }
            else
            {
                ButtonHoldCheck newButton = new ButtonHoldCheck(UIEventArgs);
                //newButton.Held += UserInteractionImpl.OnUserInteractionChangeHoldEvent;
                //newButton.Held += DevicesControlledImpl.OnUserInteractionChangeHoldEvent;
                //newButton.MomentaryPress += UserInteractionImpl.OnUserInteractionChangeMomentaryEvent;
                //newButton.MomentaryPress += MediaTransportImpl.OnUserInteractionChangeMomentaryEvent;
                //newButton.MomentaryPress += PhysicalImpl.OnUserInteractionChangeMomentaryEvent;
                //newButton.MomentaryPress += DevicesControlledImpl.OnUserInteractionChangeMomentaryEvent;

                smartGraphicHoldCheck.Add(sb.ToString(), newButton);
                CrestronConsole.PrintLine("Button is being added to Dicionary");
                CheckForButtonHold(smartGraphicHoldCheck[sb.ToString()], UIEventArgs);
            }
        }
        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }
    }
}