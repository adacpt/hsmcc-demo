using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Crestron.SimplSharp;
using DisplayDevice;
using VtcDevice;
using VideoWallDevice;
using MatrixSwitchDevice;
using CameraDevice;
using SignagePlayerDevice;
using BluRayDevice;
using DspDevice;
using CatvDevice;
using PowerControllerDevice;
using MAVETesting;
using MediaTransport;
using Physical;
using UserInteraction;
using Crestron.SimplSharpPro;


namespace DevicesControlled
{
    public class DevicesControlledImplJson: IDevicesControlled
    {
        event EventHandler<DevicesControlledEventArgs> DeviceChangeEvent;

        // DeviceSelectionMap values <{TouchPanelName}, {DeviceName}>
        public Dictionary<String, String> CurrentVtcDevice = new Dictionary<string, string>();
        public Dictionary<String, String> CurrentTunerDevice = new Dictionary<string, string>();
        public Dictionary<String, String> CurrentCameraDevice = new Dictionary<string, string>();

        // CommandParameterSelectionMap values <{TouchPanelName}, {ParameterValue}>
        public Dictionary<String, String> CurrentTimezone = new Dictionary<string, string>();
        public Dictionary<String, String> CurrentVideoWallLayout = new Dictionary<string, string>();
        public Dictionary<String, String> CurrentATCPreset = new Dictionary<string, string>();
        public Dictionary<String, String> CurrentVTCPreset = new Dictionary<string, string>();

        public List<ComPort> SerialPorts;

        public int RoomClassification = 0;

        public DevicesControlledImplJson(List<ComPort> serialPorts)
        {
            SerialPorts = new List<ComPort>();
            SerialPorts = serialPorts;
        }

        String DevicesControlledPillarConfig = "";

        public void GetPillarConfigValues(String json)
        {
            DevicesControlledPillarConfig = json;
            InitializeDevicesControlledValues();
        }


        public void InitializeDevicesControlledValues()
        {
            //CrestronConsole.PrintLine(json);
            
            RootObject rootObject = new RootObject();
            rootObject = JsonConvert.DeserializeObject<RootObject>(DevicesControlledPillarConfig);

            DevicesControlled.DeviceSelectionMap = rootObject.DevicesControlledPillar.DeviceSelectionMap;
            DevicesControlled.CommandParameterSelectionMap = rootObject.DevicesControlledPillar.CommandParameterSelectionMap;
            DevicesControlled.ButtonActions = rootObject.DevicesControlledPillar.ButtonActions;


            if (rootObject.DevicesControlledPillar.Displays.Count > 0)
            {
                DevicesControlled.Displays= rootObject.DevicesControlledPillar.Displays;
                InitializeDeviceConnections("Display");
            }

            if (rootObject.DevicesControlledPillar.DSPs.Count > 0)
            {
                DevicesControlled.DSPs = rootObject.DevicesControlledPillar.DSPs;
                InitializeDeviceConnections("DSP");
            }

            if (rootObject.DevicesControlledPillar.MatrixSwitches.Count > 0)
            {
                DevicesControlled.MatrixSwitches = rootObject.DevicesControlledPillar.MatrixSwitches;
                InitializeDeviceConnections("MatrixSwitch");
            }

            if (rootObject.DevicesControlledPillar.VTCs.Count > 0)
            {
                DevicesControlled.VTCs = rootObject.DevicesControlledPillar.VTCs;
                InitializeDeviceConnections("VTC");
            }

            if (rootObject.DevicesControlledPillar.VideoWalls.Count > 0)
            {
                DevicesControlled.VideoWalls = rootObject.DevicesControlledPillar.VideoWalls;
                InitializeDeviceConnections("VideoWall");
            }
            if (rootObject.DevicesControlledPillar.Cameras.Count > 0)
            {
                DevicesControlled.Cameras = rootObject.DevicesControlledPillar.Cameras;
                InitializeDeviceConnections("Camera");
            }
            if (rootObject.DevicesControlledPillar.SignagePlayers.Count > 0)
            {
                DevicesControlled.SignagePlayers = rootObject.DevicesControlledPillar.SignagePlayers;
                InitializeDeviceConnections("SignagePlayer");
            }
            if (rootObject.DevicesControlledPillar.PowerControllers.Count > 0)
            {
                DevicesControlled.PowerControllers = rootObject.DevicesControlledPillar.PowerControllers;
                InitializeDeviceConnections("PowerController");
            }
            if (rootObject.DevicesControlledPillar.BluRays.Count > 0)
            {
                DevicesControlled.BluRays = rootObject.DevicesControlledPillar.BluRays;
                InitializeDeviceConnections("BluRay");
            }
            if (rootObject.DevicesControlledPillar.Catvs.Count > 0)
            {
                DevicesControlled.Catvs = rootObject.DevicesControlledPillar.Catvs;
                InitializeDeviceConnections("Catv");
            }
        }

        public void InitializeDeviceConnections(String deviceCategory)
        {
            switch (deviceCategory)
            {
                case "Display":
                    foreach (KeyValuePair<String, Display> display in DevicesControlled.Displays)
                    {
                        Display disp = display.Value;
                        switch (disp.Manufacturer)
                        {
                            case "Sony":
                                DisplayDevice.Display d;
                                switch (disp.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        d = new DisplayDevice.Display(disp.RawSocketConnectionInfo.HostName, disp.RawSocketConnectionInfo.IpAddress, disp.RawSocketConnectionInfo.DeviceId, disp.RawSocketConnectionInfo.Port);
                                        disp.DisplayImpl = new DisplaySonyImpl();
                                        disp.DisplayImpl.ConnectRawSocket(disp.RawSocketConnectionInfo.HostName, disp.RawSocketConnectionInfo.IpAddress, disp.RawSocketConnectionInfo.DeviceId);
                                        break;
                                    case EConnectionType.Serial:
                                        d = new DisplayDevice.Display(disp.SerialConnectionInfo.HostName, SerialPorts[0], disp.SerialConnectionInfo.BaudRate);
                                        disp.DisplayImpl = new DisplaySonyImpl();
                                        disp.DisplayImpl.ConnectSerial(disp.SerialConnectionInfo.HostName, SerialPorts[0], disp.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case "DSP":
                    foreach (KeyValuePair<String, Dsp> dsp in DevicesControlled.DSPs)
                    {
                        Dsp d = dsp.Value;
                        switch (d.Manufacturer)
                        {
                            case "Marantz":
                                DspDevice.Dsp audioDSP;
                                switch (d.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        audioDSP = new DspDevice.Dsp(d.RawSocketConnectionInfo.HostName, d.RawSocketConnectionInfo.IpAddress, d.RawSocketConnectionInfo.DeviceId, d.RawSocketConnectionInfo.Port);
                                        d.DspImpl = new DspMarantzImpl();
                                        d.DspImpl.ConnectRawSocket(d.RawSocketConnectionInfo.HostName, d.RawSocketConnectionInfo.IpAddress, d.RawSocketConnectionInfo.DeviceId);

                                        break;
                                    case EConnectionType.Serial:
                                        audioDSP = new DspDevice.Dsp(d.SerialConnectionInfo.HostName, SerialPorts[0], d.SerialConnectionInfo.BaudRate);
                                        d.DspImpl = new DspMarantzSerialImpl();
                                        d.DspImpl.ConnectSerial(d.SerialConnectionInfo.HostName, SerialPorts[0], d.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case "MatrixSwitch":
                    foreach (KeyValuePair<String, MatrixSwitch> matrixSwitch in DevicesControlled.MatrixSwitches)
                    {
                        MatrixSwitch ms = matrixSwitch.Value;
                        switch (ms.Manufacturer)
                        {
                            case "AVPro":
                                MatrixSwitchDevice.MatrixSwitch matrix;
                                switch (ms.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        matrix = new MatrixSwitchDevice.MatrixSwitch(ms.RawSocketConnectionInfo.HostName, ms.RawSocketConnectionInfo.IpAddress, ms.RawSocketConnectionInfo.DeviceId, ms.RawSocketConnectionInfo.Port, 16, 16);
                                        ms.MatrixSwitchImpl = new MatrixSwitchGenericImpl();
                                        ms.MatrixSwitchImpl.ConnectRawSocket(ms.RawSocketConnectionInfo.HostName, ms.RawSocketConnectionInfo.IpAddress, ms.RawSocketConnectionInfo.DeviceId);

                                        break;
                                    case EConnectionType.Serial:
                                        matrix = new MatrixSwitchDevice.MatrixSwitch(ms.SerialConnectionInfo.HostName, SerialPorts[0], ms.SerialConnectionInfo.BaudRate, 16, 16);
                                        ms.MatrixSwitchImpl = new MatrixSwitchGenericSerialImpl();
                                        ms.MatrixSwitchImpl.ConnectSerial(ms.SerialConnectionInfo.HostName, SerialPorts[0], ms.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;

                case "VTC":
                    foreach (KeyValuePair<String, Vtc> VTC in DevicesControlled.VTCs)
                    {
                        Vtc VidConf = VTC.Value;
                        switch (VidConf.Manufacturer)
                        {
                            case "Cisco":
                                VtcDevice.Vtc vidConf;
                                switch (VidConf.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        vidConf = new VtcDevice.Vtc(VidConf.RawSocketConnectionInfo.HostName, VidConf.RawSocketConnectionInfo.IpAddress, VidConf.RawSocketConnectionInfo.DeviceId, VidConf.RawSocketConnectionInfo.Port);
                                        VidConf.VtcImpl = new VtcCiscoWebexImpl();
                                        VidConf.VtcImpl.ConnectRawSocket(VidConf.RawSocketConnectionInfo.HostName, VidConf.RawSocketConnectionInfo.IpAddress, VidConf.RawSocketConnectionInfo.DeviceId);

                                        break;
                                    case EConnectionType.Serial:
                                        vidConf = new VtcDevice.Vtc(VidConf.SerialConnectionInfo.HostName, SerialPorts[0], VidConf.SerialConnectionInfo.BaudRate);
                                        VidConf.VtcImpl = new VtcCiscoWebexSerialImpl();
                                        VidConf.VtcImpl.ConnectSerial(VidConf.SerialConnectionInfo.HostName, SerialPorts[0], VidConf.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case "VideoWall":
                    foreach (KeyValuePair<String, VideoWall> VideoWall in DevicesControlled.VideoWalls)
                    {
                        VideoWall VidWall = VideoWall.Value;
                        switch (VidWall.Manufacturer)
                        {
                            case "RGB Spectrum":
                                VideoWallDevice.VideoWall vidWall;
                                switch (VidWall.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        vidWall = new VideoWallDevice.VideoWall(VidWall.RawSocketConnectionInfo.HostName, VidWall.RawSocketConnectionInfo.IpAddress, VidWall.RawSocketConnectionInfo.DeviceId, VidWall.RawSocketConnectionInfo.Port);
                                        VidWall.VideoWallImpl = new VideoWallGenericImpl();
                                        VidWall.VideoWallImpl.ConnectRawSocket(VidWall.RawSocketConnectionInfo.HostName, VidWall.RawSocketConnectionInfo.IpAddress, VidWall.RawSocketConnectionInfo.DeviceId);

                                        break;
                                    case EConnectionType.Serial:
                                        vidWall = new VideoWallDevice.VideoWall(VidWall.SerialConnectionInfo.HostName, SerialPorts[0], VidWall.SerialConnectionInfo.BaudRate);
                                        VidWall.VideoWallImpl = new VideoWallGenericSerialImpl();
                                        VidWall.VideoWallImpl.ConnectSerial(VidWall.SerialConnectionInfo.HostName, SerialPorts[0], VidWall.SerialConnectionInfo.BaudRate);
                                        break;
                                }

                                break;
                        }
                    }
                    break;
                case "Camera":
                    foreach (KeyValuePair<String, Camera> Camera in DevicesControlled.Cameras)
                    {
                        Camera Cam = Camera.Value;
                        switch (Cam.Manufacturer)
                        {
                            case "Vaddio":
                                CameraDevice.Camera c;

                                switch (Cam.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        c = new CameraDevice.Camera(Cam.RawSocketConnectionInfo.HostName, Cam.RawSocketConnectionInfo.IpAddress, Cam.RawSocketConnectionInfo.Port);
                                        Cam.CameraImpl = new CameraVaddioImpl();
                                        Cam.CameraImpl.ConnectRawSocket(Cam.RawSocketConnectionInfo.HostName, Cam.RawSocketConnectionInfo.IpAddress);

                                        break;
                                    case EConnectionType.Serial:
                                        c = new CameraDevice.Camera(Cam.SerialConnectionInfo.HostName, SerialPorts[0], Cam.SerialConnectionInfo.BaudRate);
                                        Cam.CameraImpl = new CameraVaddioSerialImpl();
                                        Cam.CameraImpl.ConnectSerial(Cam.SerialConnectionInfo.HostName, SerialPorts[0], Cam.SerialConnectionInfo.BaudRate);
                                        break;
                                }



                                

                                break;
                        }
                    }
                    break;
                case "SignagePlayer":
                    foreach (KeyValuePair<String, SignagePlayer> SignagePlayer in DevicesControlled.SignagePlayers)
                    {
                        SignagePlayer Player = SignagePlayer.Value;
                        switch (Player.Manufacturer)
                        {
                            
                            case "Spinetix":
                                SignagePlayerDevice.SignagePlayer signagePlayer;

                                switch (Player.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        signagePlayer = new SignagePlayerDevice.SignagePlayer(Player.RawSocketConnectionInfo.HostName, Player.RawSocketConnectionInfo.IpAddress, Player.RawSocketConnectionInfo.Port, Player.TimeZones);
                                        Player.SignagePlayerImpl = new SignagePlayerSpinetixImpl();
                                        Player.SignagePlayerImpl.ConnectRawSocket(Player.RawSocketConnectionInfo.HostName, Player.RawSocketConnectionInfo.IpAddress);

                                        break;
                                    case EConnectionType.Serial:
                                        signagePlayer = new SignagePlayerDevice.SignagePlayer(Player.SerialConnectionInfo.HostName, SerialPorts[0], Player.SerialConnectionInfo.BaudRate, Player.TimeZones);
                                        Player.SignagePlayerImpl = new SignagePlayerSpinetixSerialImpl();
                                        Player.SignagePlayerImpl.ConnectSerial(Player.SerialConnectionInfo.HostName, SerialPorts[0], Player.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case "PowerController":
                    foreach (KeyValuePair<String, PowerController> PowerController in DevicesControlled.PowerControllers)
                    {
                        PowerController Controller = PowerController.Value;
                        switch (Controller.Manufacturer)
                        {
                            case "CyberPower":
                                PowerControllerDevice.PowerController pc;
                                switch (Controller.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        pc = new PowerControllerDevice.PowerController(Controller.RawSocketConnectionInfo.HostName, Controller.RawSocketConnectionInfo.IpAddress, Controller.RawSocketConnectionInfo.Port);
                                        Controller.PowerControllerImpl = new PowerControllerGenericImpl();
                                        Controller.PowerControllerImpl.ConnectRawSocket(Controller.RawSocketConnectionInfo.HostName, Controller.RawSocketConnectionInfo.IpAddress);

                                        break;
                                    case EConnectionType.Serial:
                                        pc = new PowerControllerDevice.PowerController(Controller.SerialConnectionInfo.HostName, SerialPorts[0], Controller.SerialConnectionInfo.BaudRate);
                                        Controller.PowerControllerImpl = new PowerControllerGenericSerialImpl();
                                        Controller.PowerControllerImpl.ConnectSerial(Controller.SerialConnectionInfo.HostName, SerialPorts[0], Controller.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case "BluRay":
                    foreach (KeyValuePair<String, BluRay> BluRay in DevicesControlled.BluRays)
                    {
                        BluRay BR = BluRay.Value;
                        switch (BR.Manufacturer)
                        {
                            case "Sony":
                                BluRayDevice.BluRay br;
                                switch (BR.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        br = new BluRayDevice.BluRay(BR.RawSocketConnectionInfo.HostName, BR.RawSocketConnectionInfo.IpAddress, BR.RawSocketConnectionInfo.Port);
                                        BR.BluRayImpl = new BluRaySonyImpl();
                                        BR.BluRayImpl.ConnectRawSocket(BR.RawSocketConnectionInfo.HostName, BR.RawSocketConnectionInfo.IpAddress);

                                        break;
                                    case EConnectionType.Serial:
                                        br = new BluRayDevice.BluRay(BR.SerialConnectionInfo.HostName, SerialPorts[0], BR.SerialConnectionInfo.BaudRate);
                                        BR.BluRayImpl = new BluRaySonySerialImpl();
                                        BR.BluRayImpl.ConnectSerial(BR.SerialConnectionInfo.HostName, SerialPorts[0], BR.SerialConnectionInfo.BaudRate);
                                        break;
                                }





                                BluRayDevice.BluRay pc = new BluRayDevice.BluRay(BR.RawSocketConnectionInfo.HostName, BR.RawSocketConnectionInfo.IpAddress, BR.RawSocketConnectionInfo.Port);
                                BR.BluRayImpl = new BluRayDevice.BluRaySonyImpl();
                                BR.BluRayImpl.ConnectRawSocket(BR.RawSocketConnectionInfo.HostName, BR.RawSocketConnectionInfo.IpAddress);

                                break;
                        }
                    }
                    break;
                case "Catv":
                    foreach (KeyValuePair<String, Catv> Catv in DevicesControlled.Catvs)
                    {
                        Catv catv = Catv.Value;
                        switch (catv.Manufacturer)
                        {
                            case "Contemporary Research":
                                CatvDevice.Catv c;
                                switch (catv.ConnectionType)
                                {
                                    case EConnectionType.RawSocket:
                                        c = new CatvDevice.Catv(catv.RawSocketConnectionInfo.HostName, catv.RawSocketConnectionInfo.IpAddress, catv.RawSocketConnectionInfo.Port);
                                        catv.CatvImpl = new CatvDevice.CatvContemporaryResearchImpl();
                                        catv.CatvImpl.ConnectRawSocket(catv.RawSocketConnectionInfo.HostName, catv.RawSocketConnectionInfo.IpAddress, catv.RawSocketConnectionInfo.Port);

                                        break;
                                    case EConnectionType.Serial:
                                        c = new CatvDevice.Catv(catv.SerialConnectionInfo.HostName, SerialPorts[0], catv.SerialConnectionInfo.BaudRate);
                                        catv.CatvImpl = new CatvContemporaryResearchSerialImpl();
                                        catv.CatvImpl.ConnectSerial(catv.SerialConnectionInfo.HostName, SerialPorts[0], catv.SerialConnectionInfo.BaudRate);
                                        break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        /*
        private void OnDisplayChangeEvent(object sender, DisplayEventArgs e)
        {
            this.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "Display",
                DisplayEventArgs = e,
                
            });
        }

        private void OnDspChangeEvent(object sender, DspEventArgs e)
        {
            this.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "DSP",
                DspEventArgs = e,

            });
        }

        private void OnVtcChangeEvent(object sender, VtcEventArgs e)
        {
            this.OnDevicesControlledChangeEvent(new DevicesControlledEventArgs()
            {
                DeviceCategory = "VTC",
                VtcEventArgs = e,

            });
        }
        */

        public void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args)
        {
            this.DeviceChangeEvent?.Invoke(this, args);
        }


        public void SetDeviceSelections(TouchPanelEventArgs args)
        {
            foreach (KeyValuePair<String, List<DeviceSelectionMap>> deviceSelectionMap in DevicesControlled.DeviceSelectionMap){
                if ( deviceSelectionMap.Key == args.TouchPanel)
                {
                    foreach (DeviceSelectionMap devSelMap in deviceSelectionMap.Value)
                    {
                        if ( ( devSelMap.SmartGraphic == (uint) args.ButtonArgs.SmartGraphic ) && ( devSelMap.Join == args.ButtonArgs.Button ) )
                        {
                            // THIS REQUIRES MANUAL INTERVENTION TO VERIFY THE REQUIRED DEVICE CATEGORIES ARE CORRECT AND MATCH THE JSON
                            switch (devSelMap.DeviceCategory)
                            {
                                case "Catv":
                                    if (CurrentTunerDevice.ContainsKey(args.TouchPanel))
                                    {
                                        CurrentTunerDevice[args.TouchPanel] = devSelMap.DeviceName;
                                    } else
                                    {
                                        CurrentTunerDevice.Add(args.TouchPanel, devSelMap.DeviceName);
                                    }
                                    break;
                                case "Cameras":
                                    if (CurrentCameraDevice.ContainsKey(args.TouchPanel))
                                    {
                                        CurrentCameraDevice[args.TouchPanel] = devSelMap.DeviceName;
                                    }
                                    else
                                    {
                                        CurrentCameraDevice.Add(args.TouchPanel, devSelMap.DeviceName);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        
        
        public void SetCommandParameterSelections(TouchPanelEventArgs args)
        {
            foreach (KeyValuePair<String, List<CommandParameterSelectionMap>> parameterSelectionMap in DevicesControlled.CommandParameterSelectionMap)
            {
                if (parameterSelectionMap.Key == args.TouchPanel)
                {
                    foreach (CommandParameterSelectionMap paramSelMap in parameterSelectionMap.Value)
                    {
                        if ((paramSelMap.SmartGraphic == (uint)args.ButtonArgs.SmartGraphic) && (paramSelMap.Join == args.ButtonArgs.Button))
                        {
                            switch (paramSelMap.CommandParameterCategory)
                            {
                                case "Timezones":
                                    if (CurrentTimezone.ContainsKey(args.TouchPanel))
                                    {
                                        CurrentTimezone[args.TouchPanel] = paramSelMap.ParameterValue;
                                    }
                                    else
                                    {
                                        CurrentTimezone.Add(args.TouchPanel, paramSelMap.ParameterValue);
                                    }
                                    break;
                            
                                case "VideoWall Layouts":
                                    if (CurrentVideoWallLayout.ContainsKey(args.TouchPanel))
                                    {
                                        CurrentVideoWallLayout[args.TouchPanel] = paramSelMap.ParameterValue;
                                    }
                                    else
                                    {
                                        CurrentVideoWallLayout.Add(args.TouchPanel, paramSelMap.ParameterValue);
                                    }
                                    break;
                                case "ATC SpeedDial Slot":
                                    if (CurrentATCPreset.ContainsKey(args.TouchPanel))
                                    {
                                        CurrentATCPreset[args.TouchPanel] = paramSelMap.ParameterValue;
                                    }
                                    else
                                    {
                                        CurrentATCPreset.Add(args.TouchPanel, paramSelMap.ParameterValue);
                                    }
                                    break;
                                case "VTC SpeedDial Slot":
                                    if (CurrentVTCPreset.ContainsKey(args.TouchPanel))
                                    {
                                        CurrentVTCPreset[args.TouchPanel] = paramSelMap.ParameterValue;
                                    }
                                    else
                                    {
                                        CurrentVTCPreset.Add(args.TouchPanel, paramSelMap.ParameterValue);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }


        public void ProcessUserInteractionEvents(TouchPanelEventArgs args)
        {
            
            foreach (ButtonAction button in DevicesControlled.ButtonActions)
            {
                if (button.Join == (ushort)args.ButtonArgs.Button && button.ButtonState == args.ButtonArgs.ButtonState.ToString() && args.ButtonArgs.SmartGraphic == (ESmartGraphicID) button.SmartGraphicId)
                {
                    foreach (Command cmd in button.Commands)
                    {
                        switch (cmd.DeviceCategory)
                        {
                            case "Display":
                                ProcessDisplayCommands(cmd, args);
                                break;

                            case "DSP":
                                ProcessDspCommands(cmd, args);
                                break;

                            case "VTC":
                                ProcessVtcCommands(cmd, args);
                                break;

                            case "VideoWall":
                                ProcessVideoWallCommands(cmd, args);
                                break;

                            case "Camera":
                                ProcessCameraCommands(cmd, args);
                                break;

                            case "SignagePlayer":
                                ProcessSignagePlayerCommands(cmd, args);
                                break;

                            case "PowerController":
                                ProcessPowerControllerCommands(cmd, args);
                                break;

                            case "BluRay":
                                ProcessBluRayCommands(cmd, args);
                                break;

                            case "Catv":
                                ProcessCatvCommands(cmd, args);
                                break;
                        }
                    }
                }
            }
        }

        public void ProcessDisplayCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "Power":
                    DevicesControlled.Displays[cmd.DeviceName].DisplayImpl.SetPower(cmd.DisplayCommandParameters.PowerParam);
                    break;
                case "Input":
                    DevicesControlled.Displays[cmd.DeviceName].DisplayImpl.SetInput(cmd.DisplayCommandParameters.InputParam);
                    break;
                case "AdjustVolume":
                    DevicesControlled.Displays[cmd.DeviceName].DisplayImpl.AdjustVolume(cmd.DisplayCommandParameters.VolumeAdjustParam);
                    break;
                case "SetVolume":
                    DevicesControlled.Displays[cmd.DeviceName].DisplayImpl.SetVolume(cmd.DisplayCommandParameters.VolumeSetParam);
                    break;
                case "VolumeMute":
                    DevicesControlled.Displays[cmd.DeviceName].DisplayImpl.SetVolumeMute(cmd.DisplayCommandParameters.MuteParam);
                    break;
            }
        }
        public KeyboardJoinMap GetKeyBoardJoinMap(String keyboardName)
        {
            KeyboardJoinMap selectedKb = new KeyboardJoinMap();
            foreach (KeyboardJoinMap keyboardMap in UserInteraction.UserInteraction.KeyboardJoinMaps)
            {
                if (keyboardMap.Name == keyboardName)
                {
                    selectedKb = keyboardMap;
                }
            }
            return selectedKb;
        }

        public void ProcessDspCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "AdjustVolume":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.AdjustVolume(cmd.DspCommandParameters.LevelNameParam, cmd.DspCommandParameters.VolumeAdjustParam);
                    break;
                case "VolumeSet":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.AdjustVolume(cmd.DspCommandParameters.LevelNameParam, cmd.DspCommandParameters.VolumeSetParam);
                    break;
                case "ToggleVolumeMute":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.ToggleVolumeMute(cmd.DspCommandParameters.LevelNameParam);
                    break;
                case "SetVolumeMute":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.SetVolumeMute(cmd.DspCommandParameters.LevelNameParam, cmd.DspCommandParameters.SpeakerMuteParam);
                    break;
                case "ToggleMicMute":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.ToggleMicMute(cmd.DspCommandParameters.LevelNameParam);
                    break;
                case "SetMicMute":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.SetMicMute(cmd.DspCommandParameters.LevelNameParam, cmd.DspCommandParameters.MicMuteParam);
                    break;
                case "StoreNumber":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.StoreNumber(cmd.DspCommandParameters.PresetSlotParam, GetKeyBoardJoinMap("ATCKeyboard").TypedString);
                    break;
                case "StoreName":
                    DevicesControlled.DSPs[cmd.DeviceName].DspImpl.StoreName(Int32.Parse(CurrentATCPreset[args.TouchPanel]), GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString);
                    break;
            }
        }

        public void ProcessVtcCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "MakeCall":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.MakeCall(cmd.VtcCommandParameters.MakeCallParam);
                    break;
                case "HangupCall":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.HangupCall(cmd.VtcCommandParameters.HangupCallParam);
                    break;
                case "SetPresentation":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.SetPresentation(cmd.VtcCommandParameters.PresentationParam);
                    break;
                case "SetSelfview":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.SetSelfview(cmd.VtcCommandParameters.SelfviewParam);
                    break;
                case "MovePipScreen":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.MovePipScreen();
                    break;
                case "SetDtmf":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.SetDtmf(cmd.VtcCommandParameters.DtmfParam);
                    break;
                case "StoreNumber":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.StoreNumber(cmd.VtcCommandParameters.PresetSlotParam, GetKeyBoardJoinMap("VTCKeyboard").TypedString);
                    break;
                case "StoreName":
                    DevicesControlled.VTCs[cmd.DeviceName].VtcImpl.StoreName(Int32.Parse(CurrentVTCPreset[args.TouchPanel]), GetKeyBoardJoinMap("VTCKeyboard").TypedString);
                    break;
            }
        }

        public void ProcessVideoWallCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "SetLayout":
                    DevicesControlled.VideoWalls[cmd.DeviceName].VideoWallImpl.SetLayout(cmd.VideoWallCommandParameters.LayoutParam);
                    break;
            }
        }

        public void ProcessCameraCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "Pan":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.Pan(cmd.CameraCommandParameters.PanParam, cmd.CameraCommandParameters.SpeedParam, cmd.CameraCommandParameters.ActionParam, cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "Tilt":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.Pan(cmd.CameraCommandParameters.TiltParam, cmd.CameraCommandParameters.SpeedParam, cmd.CameraCommandParameters.ActionParam, cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "Zoom":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.Pan(cmd.CameraCommandParameters.ZoomParam, cmd.CameraCommandParameters.SpeedParam, cmd.CameraCommandParameters.ActionParam, cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "Stop":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.Stop(cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "RecallPreset":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.RecallPreset(cmd.CameraCommandParameters.RecallPresetParam, cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "SavePreset":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.SavePreset(cmd.CameraCommandParameters.SavePresetParam, cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "Power":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.Power(cmd.CameraCommandParameters.PowerParam, cmd.CameraCommandParameters.CameraNameParam);
                    break;
                case "Home":
                    DevicesControlled.Cameras[cmd.DeviceName].CameraImpl.Home(cmd.CameraCommandParameters.CameraNameParam);
                    break;
            }
        }

        public void ProcessSignagePlayerCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "UpdateClassification":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateClassification(RoomClassification);
                    break;
                case "UpdateMicMuteStatus":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateMicMuteStatus(cmd.SignagePlayerCommandParameters.MicMuteParam);
                    break;
                case "UpdateSpeakerMuteStatus":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateSpeakerMuteStatus(cmd.SignagePlayerCommandParameters.SpeakerMuteParam);
                    break;
                case "UpdateVTCActiveStatus":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateVTCActiveStatus(cmd.SignagePlayerCommandParameters.VtcActiveParam);
                    break;
                case "UpdateZoneName":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateZoneName(cmd.SignagePlayerCommandParameters.ZoneNameParam, CurrentTimezone[args.TouchPanel]);
                    break;
                case "UpdateZoneHourOffset":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateZoneHourOffset(cmd.SignagePlayerCommandParameters.ZoneHourOffsetParam, CurrentTimezone[args.TouchPanel]);
                    break;
                case "UpdateZoneMinuteOffset":
                    DevicesControlled.SignagePlayers[cmd.DeviceName].SignagePlayerImpl.UpdateZoneMinuteOffset(cmd.SignagePlayerCommandParameters.ZoneMinuteOffsetParam, CurrentTimezone[args.TouchPanel]);
                    break;
            }
        }

        public void ProcessPowerControllerCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "SetOutletPowerState":
                    DevicesControlled.PowerControllers[cmd.DeviceName].PowerControllerImpl.SetOutletPowerState(cmd.PowerControllerCommandParameters.OutletParam, cmd.PowerControllerCommandParameters.PowerParam);
                    break;
            }
        }

        public void ProcessBluRayCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "Play":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Play();
                    break;
                case "Stop":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Stop();
                    break;
                case "Pause":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Pause();
                    break;
                case "NextChapter":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.NextChapter();
                    break;
                case "LastChapter":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.LastChapter();
                    break;
                case "FastForward":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.FastForward();
                    break;
                case "Rewind":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Rewind();
                    break;
                case "MenuUp":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.MenuUp();
                    break;
                case "MenuDown":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.MenuDown();
                    break;
                case "MenuLeft":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.MenuLeft();
                    break;
                case "MenuRight":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.MenuRight();
                    break;
                case "MenuEnter":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.MenuEnter();
                    break;
                case "Home":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Home();
                    break;
                case "Setup":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Setup();
                    break;
                case "Options":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Options();
                    break;
                case "Info":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Info();
                    break;
                case "Return":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Return();
                    break;
                case "PopupMenu":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.PopupMenu();
                    break;
                case "TopMenu":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.TopMenu();
                    break;
                case "SpecialKey":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.SpecialKey(cmd.BluRayCommandParameters.SpecialKeyParam);
                    break;
                case "Number":
                    DevicesControlled.BluRays[cmd.DeviceName].BluRayImpl.Number(cmd.BluRayCommandParameters.NumberParam);
                    break;
            }
        }

        public void ProcessCatvCommands(Command cmd, TouchPanelEventArgs args)
        {
            switch (cmd.CommandName)
            {
                case "ChannelUp":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.ChannelUp();
                    break;
                case "ChannelDown":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.ChannelDown();
                    break;
                case "SetChannel":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.SetChannel(cmd.CatvCommandParameters.ChannelParam);
                    break;
                case "PreviousChannel":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.PreviousChannel();
                    break;
                case "GetChannel":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.GetChannel();
                    break;
                case "MenuUp":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.MenuUp();
                    break;
                case "MenuDown":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.MenuDown();
                    break;
                case "MenuLeft":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.MenuLeft();
                    break;
                case "MenuRight":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.MenuRight();
                    break;
                case "MenuEnter":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.MenuEnter();
                    break;
                case "Menu":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.Menu();
                    break;
                case "Exit":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.Exit();
                    break;
                case "List":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.List();
                    break;
                case "Info":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.Info();
                    break;
                case "SavePreset":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.SavePreset(cmd.CatvCommandParameters.PresetSlotParam);
                    break;
                case "RecallPreset":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.RecallPreset(cmd.CatvCommandParameters.PresetSlotParam);
                    break;
                case "GetPresets":
                    DevicesControlled.Catvs[CurrentTunerDevice[args.TouchPanel]].CatvImpl.GetPresets();
                    break;
            }
        }


        public void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs args)
        {
            SetDeviceSelections(args);
            SetCommandParameterSelections(args);
            ProcessUserInteractionEvents(args);
        }

        public void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs args)
        {
            SetDeviceSelections(args);
            SetCommandParameterSelections(args);
            ProcessUserInteractionEvents(args);
        }

        public void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args)
        {
            if (args.Name == "RouteValidated")
            {
                DevicesControlled.MatrixSwitches["MatrixSwitch"].MatrixSwitchImpl.MakeRoute(args.Source.SourceNodeIndex, args.Destination.DestinationNodeIndex);
            }
        }




        public void OnPhysicalChangeEvent(object sender, PhysicalEventArgs args)
        {
            switch (args.Category)
            {
                case "Physical":
                    break;

                case "SystemStatusElements":
                    break;
                case "RoomStatusElements":
                    switch (args.Name)
                    {
                        case "RoomOnMode":
                            CurrentVtcDevice["Admin"] = "UnclassVtc";
                            break;
                        case "RoomClassification":
                            RoomClassification = args.CurrentStatus;
                            switch (args.CurrentStatus)
                            {
                                case 1:
                                    CurrentVtcDevice["Admin"] = "UnclassVtc";
                                    break;
                                case 2:
                                    CurrentVtcDevice["Admin"] = "SecretVtc";
                                    break;
                            }
                            break;
                        case "RoomOffMode":
                            CurrentVtcDevice["Admin"] = "UnclassVtc";
                            break;
                    }
                    break;
            }
        }
    }
}
