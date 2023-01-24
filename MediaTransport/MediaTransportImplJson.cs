using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Physical;
using UserInteraction;
using DevicesControlled;
using MAVETesting;
using Crestron.SimplSharp;

namespace MediaTransport
{
    
    public class MediaTransportImplJson : IMediaTransport
    {
        public Source CurrentSource;
        public Destination CurrentDestination;

        String MediaTransportPillarConfig = "";
        public int CurrentClassification = 0;

        public void GetPillarConfigValues(String json)
        {
            MediaTransportPillarConfig = json;
            InitializeMediaTransportValues();
        }
        public void InitializeMediaTransportValues()
        {
            RootObject rootObject = new RootObject();
            rootObject = JsonConvert.DeserializeObject<RootObject>(MediaTransportPillarConfig);
            MediaTransport.IODevices = rootObject.MediaTransportPillar.IODevices;
            MediaTransport.AutomaticRoutes = rootObject.MediaTransportPillar.AutomaticRoutes;
            MediaTransport.TouchPanels = rootObject.MediaTransportPillar.TouchPanels;
            CurrentSource = new Source();
            CurrentDestination = new Destination();
        }

        public int GetTouchPanelIndex ( string tpName )
        {
            int tpIndex = 0;
            int index = 0;
            foreach (TouchPanel touchPanel in MediaTransport.TouchPanels)
            {
                if ( touchPanel.Name == tpName)
                {
                    tpIndex = index; 
                }
                index++;
            }
            return tpIndex;
        }

        public void SetIODeviceClassification(int classification)
        {
            foreach (IODevice iod in MediaTransport.IODevices)
            {
                if (iod.State == "Stateless")
                {
                    iod.Classification = classification;
                    foreach ( Source source in iod.Sources)
                    {
                        if (source.State == "Stateless")
                        {
                            source.Classification = classification;
                        }
                    }
                    foreach (Destination destination in iod.Destinations)
                    {
                        if (destination.State == "Stateless")
                        {
                            destination.Classification = classification;
                        }
                    }
                }
            }
        }

        public void ClearSourceButtonInputMapNames( String mapName, String uiName )
        {
            switch (MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].SourceButtonInputMap[mapName].MapType)
            {
                case "Variable":
                    MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].SourceButtonInputMap[mapName].VariableSourceButtonMap.Sources.Clear();
                    break;
                case "StaticTouchPanel":
                    MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].SourceButtonInputMap[mapName].StaticTouchPanelSourceButtonMap.Sources.Clear();
                    break;
                case "StaticSmartGraphic":
                    MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].SourceButtonInputMap[mapName].StaticSmartGraphicSourceButtonMap.Sources.Clear();
                    break;
            }
        }

        public void ClearDestinationButtonOutputMapNames(String mapName, String uiName)
        {
            switch (MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].DestinationButtonOutputMap[mapName].MapType)
            {
                case "Variable":
                    MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].DestinationButtonOutputMap[mapName].VariableDestinationButtonMap.Destinations.Clear();
                    break;
                case "StaticTouchPanel":
                    MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].DestinationButtonOutputMap[mapName].StaticTouchPanelDestinationButtonMap.Destinations.Clear();
                    break;
                case "StaticSmartGraphic":
                    MediaTransport.TouchPanels[GetTouchPanelIndex(uiName)].DestinationButtonOutputMap[mapName].StaticSmartGraphicDestinationButtonMap.Destinations.Clear();
                    break;
            }
        }

        public void ExecuteAddSourceIODevice(String mapName, IODevice iod, String uiName)
        {
            if(iod.Sources.Count > 0)
            {
                foreach (TouchPanel tp in MediaTransport.TouchPanels)
                {
                    if (tp.Name == uiName)
                    {
                        tp.SortedSourceIODevices[mapName].Add(iod);
                    }
                }
            }
        }

        public void AddIOSource(String mapName, IODevice iod, String uiName)
        {
            // NEED TO DOCUMENT WHY WE CREATE A SORTED IO DEVICE LIST AND THE SOURCE BUTTON INPUT MAP LIST
            // AND WHAT THE CORRELATION IS BETWEEN USERINTERFACE AND MEDIA TRANSPORT
            int tpIndex = GetTouchPanelIndex(uiName);

            switch (MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap[mapName].MapType)
            {
                case "Variable":
                    ExecuteAddSourceIODevice(mapName, iod, uiName);
                    foreach (Source source in iod.Sources)
                    {
                        MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap[mapName].VariableSourceButtonMap.Sources.Add(source.FullName);
                    }
                    break;
                case "StaticTouchPanel":
                    // DOCUMENT THAT THE STATIC NATURE FOR THIS PROJECT IS BASED ON CLASSIFICATION.  MIGHT BE DIFFERENT FOR ANOTHER PROJECT
                    if (iod.Classification == CurrentClassification)
                    {
                        ExecuteAddSourceIODevice(mapName, iod, uiName);
                        foreach (Source source in iod.Sources)
                        {
                            MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap[mapName].StaticTouchPanelSourceButtonMap.Sources.Add(source.FullName);
                        }
                    }
                    break;
                case "StaticSmartGraphic":
                    if (iod.Classification <= CurrentClassification)
                    {
                        ExecuteAddSourceIODevice(mapName, iod, uiName);
                        foreach (Source source in iod.Sources)
                        {
                            MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap[mapName].StaticSmartGraphicSourceButtonMap.Sources.Add(source.FullName);
                        }
                    }
                    break;
            }
        }

        public void ExecuteAddDestinationIODevice(String mapName, IODevice iod, String uiName)
        {
            if (iod.Destinations.Count > 0)
            {
                foreach (TouchPanel tp in MediaTransport.TouchPanels)
                {
                    if (tp.Name == uiName)
                    {
                        tp.SortedDestinationIODevices[mapName].Add(iod);
                    }
                }
            }
        }

        public void AddIODestination(String mapName, IODevice iod, String uiName)
        {
            int tpIndex = GetTouchPanelIndex(uiName);

            switch (MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap[mapName].MapType)
            {
                case "Variable":
                    ExecuteAddDestinationIODevice(mapName, iod, uiName);
                    foreach (Destination dest in iod.Destinations)
                    {
                        MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap[mapName].VariableDestinationButtonMap.Destinations.Add(dest.FullName);
                    }
                    break;
                case "StaticTouchPanel":
                    if (iod.Classification == CurrentClassification)
                    {
                        if( iod.Destinations.Count > 0)
                        {
                            ExecuteAddDestinationIODevice(mapName, iod, uiName);
                            foreach (Destination dest in iod.Destinations)
                            {
                                MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap[mapName].StaticTouchPanelDestinationButtonMap.Destinations.Add(dest.FullName);
                            }
                        }
                    }
                    break;
                case "StaticSmartGraphic":
                    if (iod.Classification <= CurrentClassification)
                    {
                        ExecuteAddDestinationIODevice(mapName, iod, uiName);
                        foreach (Destination dest in iod.Destinations)
                        {
                            MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap[mapName].StaticSmartGraphicDestinationButtonMap.Destinations.Add(dest.FullName);
                        }
                    }
                    break;
            }
        }

        public void SortIOSourceDevicesByCategory()
        {
            foreach(TouchPanel tp in MediaTransport.TouchPanels)
            {
                Dictionary<String, SourceButtonInputMap> srcBtnInputMaps = tp.SourceButtonInputMap;

                String catName = "";

                foreach (KeyValuePair<String, SourceButtonInputMap> srcBtnInputMap in srcBtnInputMaps)
                {
                    if (!MediaTransport.TouchPanels[GetTouchPanelIndex(tp.Name)].SortedSourceIODevices.ContainsKey(srcBtnInputMap.Key))
                    {
                        List<IODevice> sortedIOSources = new List<IODevice>();
                        MediaTransport.TouchPanels[GetTouchPanelIndex(tp.Name)].SortedSourceIODevices.Add(srcBtnInputMap.Key, sortedIOSources);
                    }

                    if (srcBtnInputMap.Key != "BlankButtonMap")
                    {
                        MediaTransport.TouchPanels[GetTouchPanelIndex(tp.Name)].SortedSourceIODevices[srcBtnInputMap.Key].Clear();
                        ClearSourceButtonInputMapNames(srcBtnInputMap.Key, tp.Name);

                        foreach (IODevice iod in MediaTransport.IODevices)
                        {
                            // THIS CORRELATION NEEDS TO BE CLEARLY DOCUMENTED
                            // Need to clearly document the available device categories as part of the EDeviceCategory Enum
                            if (iod.DeviceCategories.Contains(srcBtnInputMap.Value.DeviceCategory))
                            {
                                switch (srcBtnInputMap.Value.DeviceCategory)
                                {
                                    case EDeviceCategory.PC:
                                        catName = "PC";
                                        AddIOSource(srcBtnInputMap.Key, iod, tp.Name);
                                        break;
                                    case EDeviceCategory.VTCPresentation:
                                        catName = "VTCPresentation";
                                        AddIOSource(srcBtnInputMap.Key, iod, tp.Name);
                                        break;
                                    case EDeviceCategory.VTC:
                                        catName = "Collaboration";
                                        AddIOSource(srcBtnInputMap.Key, iod, tp.Name);
                                        break;
                                    case EDeviceCategory.Media:
                                        catName = "Media";
                                        AddIOSource(srcBtnInputMap.Key, iod, tp.Name);
                                        break;
                                    case EDeviceCategory.Shared:
                                        catName = "Shared";
                                        break;
                                }
                            }
                        }
                        MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                        {
                            Category = "IOSourceDevicesSorted",
                            Name = catName,
                            MapName = srcBtnInputMap.Key,
                            TouchPanel = tp.Name
                        });
                    }
                }
            }
        }

        public void SortIODestinationDevicesByCategory()
        {
            foreach (TouchPanel tp in MediaTransport.TouchPanels)
            {
                Dictionary<String, DestinationButtonOutputMap> destBtnOutputMaps = tp.DestinationButtonOutputMap;

                String catName = "";

                foreach (KeyValuePair<String, DestinationButtonOutputMap> destBtnOutputMap in destBtnOutputMaps)
                {
                    if (!MediaTransport.TouchPanels[GetTouchPanelIndex(tp.Name)].SortedDestinationIODevices.ContainsKey(destBtnOutputMap.Key))
                    {
                        List<IODevice> sortedIODestinations = new List<IODevice>();
                        MediaTransport.TouchPanels[GetTouchPanelIndex(tp.Name)].SortedDestinationIODevices.Add(destBtnOutputMap.Key, sortedIODestinations);
                    }
                    MediaTransport.TouchPanels[GetTouchPanelIndex(tp.Name)].SortedDestinationIODevices[destBtnOutputMap.Key].Clear();

                    ClearDestinationButtonOutputMapNames(destBtnOutputMap.Key, tp.Name);

                    foreach (IODevice iod in MediaTransport.IODevices)
                    {
                        if (iod.DeviceCategories.Contains(destBtnOutputMap.Value.DeviceCategory))
                        {
                            switch (destBtnOutputMap.Value.DeviceCategory)
                            {
                                case EDeviceCategory.Display:
                                    catName = "Display";
                                    AddIODestination(destBtnOutputMap.Key, iod, tp.Name);
                                    break;
                                case EDeviceCategory.VTC:
                                    catName = "Collaboration";
                                    AddIODestination(destBtnOutputMap.Key, iod, tp.Name);
                                    break;
                                case EDeviceCategory.Shared:
                                    catName = "Shared";
                                    AddIODestination(destBtnOutputMap.Key, iod, tp.Name);
                                    break;
                            }
                        }
                    }
                    MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                    {
                        Category = "IODestinationDevicesSorted",
                        Name = catName,
                        MapName = destBtnOutputMap.Key,
                        TouchPanel = tp.Name
                    });
                }
            }
        }

        public void ValidateRoute(Source currentSrc, Destination currentDest)
        {
            bool routeValid;

            if (currentSrc.Classification <= currentDest.Classification)
            {
                if ((currentDest.SignalType == ESignalType.AudioVideo) && (currentSrc.SignalType == ESignalType.AudioVideo || currentSrc.SignalType == ESignalType.VideoOnly))
                {
                    routeValid = true;
                } else if ((currentDest.SignalType == ESignalType.VideoOnly) && (currentSrc.SignalType == ESignalType.AudioVideo || currentSrc.SignalType == ESignalType.VideoOnly))
                {
                    routeValid = true;
                } else if ((currentDest.SignalType == ESignalType.AudioOnly) && (currentSrc.SignalType == ESignalType.AudioVideo || currentSrc.SignalType == ESignalType.AudioOnly))
                {
                    routeValid = true;
                } else if ((currentDest.SignalType == ESignalType.USB) && (currentSrc.SignalType == ESignalType.USB ))
                {
                    routeValid = true;
                } else
                {
                    routeValid = false;
                }
            } else
            {
                routeValid = false;
            }
            if ( routeValid )
            {
                Route newRoute = new Route
                {
                    Source = currentSrc.FullName,
                    Destination = currentDest.FullName
                };

                if ( MediaTransport.RecallRoutes.ContainsKey(currentDest.FullName))
                {
                    Route recallRoute = new Route
                    {
                        Source = MediaTransport.CurrentRoutes[currentDest.FullName].Source,
                        Destination = MediaTransport.CurrentRoutes[currentDest.FullName].Destination
                    };
                    MediaTransport.RecallRoutes[currentDest.FullName] = recallRoute;
                } else
                {
                    MediaTransport.RecallRoutes.Add(currentDest.FullName, newRoute);
                }
                
                if (MediaTransport.CurrentRoutes.ContainsKey(currentDest.FullName))
                {
                    MediaTransport.CurrentRoutes[currentDest.FullName] = newRoute;
                }
                else
                {
                    MediaTransport.CurrentRoutes.Add(currentDest.FullName, newRoute);
                }

                MakeRoute(currentSrc.FullName, currentDest.FullName);
            }
        }


        public void DisconnectSource(String source)
        {
            foreach (KeyValuePair<String, Route> pair in MediaTransport.CurrentRoutes)
            {
                if (pair.Value.Source == source)
                {
                    MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                    {
                        Category = "Routing",
                        Name = "DisconnectSourc",
                        Source = GetSource(source)
                    });
                }
            }
        }

        public void DisconnectDestination ( String destination)
        {
            ValidateRoute(GetSource("Blank"), GetDestination(destination));
        }

        public void ClearAllDestinations ()
        {
            foreach (IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Destination dest in ioDev.Destinations)
                {
                    ValidateRoute( GetSource(""), GetDestination(dest.FullName) );
                }
            }
        }

        public void RecallRoute ( String destination)
        {
            ValidateRoute(GetSource(MediaTransport.RecallRoutes[destination].Source), GetDestination(destination));
        }

        public void RecallAllRoutes()
        {
            foreach (IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Destination dest in ioDev.Destinations)
                {
                    ValidateRoute(GetSource(MediaTransport.RecallRoutes[dest.FullName].Source), GetDestination(dest.FullName));
                }
            }
        }

        public void RecallAllRoutes (EDestinationDeviceCategory category)
        {

            foreach (IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Destination dest in ioDev.Destinations)
                {
                    if (dest.DestinationCategory == category)
                    {
                        ValidateRoute(GetSource(dest.RecallSource), GetDestination(dest.FullName));
                    }
                }
            }
        }

        public void MakeAutomaticRoutes(String autoRouteCategory, int index)
        {
            foreach (KeyValuePair<String, List<AutomaticRoute>> pair in MediaTransport.AutomaticRoutes)
            {
                if (pair.Key == autoRouteCategory)
                {
                    foreach ( AutomaticRoute route in pair.Value)
                    {
                        ValidateRoute( GetSource( route.Sources[index] ), GetDestination(route.Destinations[index] ) );
                    }
                }
            }
        }

        public void MakePresetRoutes(String presetName)
        {
            foreach (KeyValuePair<String, List<Route>> pair in MediaTransport.PresetRoutes)
            {
                if (pair.Key == presetName)
                {
                    foreach (Route route in pair.Value)
                    {
                        ValidateRoute(GetSource ( route.Source ), GetDestination ( route.Destination) );
                    }
                }
            }
        }

        public void MakeRoute ( String source, String destination)
        {

            MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
            {
                Category = "Routing",
                Name = "RouteValidated",
                Source = GetSource(source),
                Destination = GetDestination(destination)
            });
        }

        public Source GetSource(String sourceName)
        {
            Source source = new Source();
            foreach ( IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Source src in ioDev.Sources)
                {
                    if (src.FullName == sourceName)
                    {
                        source = src;
                    }
                }
            }

            return source;
        }

        public Destination GetDestination(String destinationName)
        {
            Destination output = new Destination();
            foreach (IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Destination outp in ioDev.Destinations)
                {
                    if (outp.FullName == destinationName)
                    {
                        output = outp;
                    }
                }
            }
            return output;
        }

        public IODevice GetIODeviceFromSource ( String sourceName)
        {
            IODevice iod = new IODevice();

            foreach (IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Source src in ioDev.Sources)
                {
                    if (src.FullName == sourceName)
                    {
                        iod = ioDev;
                    }
                }
            }

            return iod;
        }

        public IODevice GetIODeviceFromDestination(String destName)
        {
            IODevice iod = new IODevice();

            foreach (IODevice ioDev in MediaTransport.IODevices)
            {
                foreach (Destination dest in ioDev.Destinations)
                {
                    if (dest.FullName == destName)
                    {
                        iod = ioDev;
                    }
                }
            }

            return iod;
        }

        public void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs arg)
        {
            if (arg.ButtonArgs.ButtonState == EButtonState.On)
            {
                int tpIndex = GetTouchPanelIndex(arg.TouchPanel);

                int tpBtnIndex = -1;

                if (arg.ButtonArgs.Button == 290 && arg.ButtonArgs.SmartGraphic == 0)
                {
                    CurrentSource = GetSource("");
                    MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                    {
                        Category = "Routing",
                        Name = "Source Selected",
                        TouchPanel = arg.TouchPanel,
                        Source = GetSource(CurrentSource.FullName),
                    });
                }

                if ((arg.ButtonArgs.SmartGraphic == ESmartGraphicID.PCSources) || (arg.ButtonArgs.SmartGraphic == ESmartGraphicID.VTCPresentationSources))
                {
                    
                    int sgBtnIndex = MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["PCSourceButtonMap"].VariableSourceButtonMap.SmartGraphicJoins.FindIndex(x => x == arg.ButtonArgs.Name);
                    if (sgBtnIndex >= 0)
                    {
                        CurrentSource = GetSource(MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["PCSourceButtonMap"].VariableSourceButtonMap.Sources[sgBtnIndex]);
                        MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                        {
                            Category = "Routing",
                            Name = "Source Selected",
                            TouchPanel = arg.TouchPanel,
                            Source = GetSource(CurrentSource.FullName),
                        });
                    }
                }

                if (MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["VTCSourceButtonMap"].StaticTouchPanelSourceButtonMap.Joins.Contains((int)arg.ButtonArgs.Button))
                {
                    int sgBtnIndex = MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["VTCSourceButtonMap"].StaticTouchPanelSourceButtonMap.Joins.FindIndex(x => x == arg.ButtonArgs.Button);
                    if (sgBtnIndex >= 0)
                    {
                        CurrentSource = GetSource(MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["VTCSourceButtonMap"].StaticTouchPanelSourceButtonMap.Sources[sgBtnIndex]);
                        MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                        {
                            Category = "Routing",
                            Name = "Source Selected",
                            TouchPanel = arg.TouchPanel,
                            Source = GetSource(CurrentSource.FullName),
                        });
                    }
                }

                if (arg.ButtonArgs.SmartGraphic == ESmartGraphicID.MediaSources)
                {
                    int sgBtnIndex = MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["MediaSourceButtonMap"].VariableSourceButtonMap.SmartGraphicJoins.FindIndex(x => x == arg.ButtonArgs.Name);
                    if (sgBtnIndex >= 0)
                    {
                        CurrentSource = GetSource(MediaTransport.TouchPanels[tpIndex].SourceButtonInputMap["MediaSourceButtonMap"].VariableSourceButtonMap.Sources[sgBtnIndex]);
                        MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                        {
                            Category = "Routing",
                            Name = "Source Selected",
                            TouchPanel = arg.TouchPanel,
                            Source = GetSource(CurrentSource.FullName),
                        });
                    }
                }

                if (arg.ButtonArgs.SmartGraphic == ESmartGraphicID.VTCPresentationSources)
                {
                    if (CurrentClassification == 1)
                    {
                        CurrentDestination = GetDestination("NIPR VTC Monitor 2");
                    }
                    if (CurrentClassification == 2)
                    {
                        CurrentDestination = GetDestination("SIPR VTC Monitor 2");
                    }

                    ValidateRoute(CurrentSource, CurrentDestination);
                }

                if (arg.ButtonArgs.SmartGraphic == ESmartGraphicID.DisplayDestinations)
                {
                    tpBtnIndex = MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap["DisplayDestinationButtonMap"].VariableDestinationButtonMap.SmartGraphicJoins.FindIndex(x => x == arg.ButtonArgs.Name);
                    if (tpBtnIndex >= 0)
                    {
                        CurrentDestination = GetDestination(MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap["DisplayDestinationButtonMap"].VariableDestinationButtonMap.Destinations[tpBtnIndex]);
                        ValidateRoute(CurrentSource, CurrentDestination);
                    }
                }

                tpBtnIndex = MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap["CollaborationDestinationButtonMap"].StaticTouchPanelDestinationButtonMap.Joins.FindIndex(x => x == arg.ButtonArgs.Button);
                if (tpBtnIndex >= 0)
                {
                    CurrentDestination = GetDestination(MediaTransport.TouchPanels[tpIndex].DestinationButtonOutputMap["CollaborationDestinationButtonMap"].StaticTouchPanelDestinationButtonMap.Destinations[tpBtnIndex]);
                    ValidateRoute(CurrentSource, CurrentDestination);
                }
            }
        }
       
        public void OnPhysicalChangeEvent(object sender, PhysicalEventArgs e)
        {
            switch (e.Category)
            {
                case "RoomStatusElements":
                    switch (e.Name)
                    {
                        case "RoomClassification":
                            CurrentClassification = e.CurrentStatus;
                            SetIODeviceClassification(CurrentClassification);
                            SortIOSourceDevicesByCategory();
                            SortIODestinationDevicesByCategory();

                            CurrentSource = GetSource("");
                            MediaTransport.OnMediaTransportChangeEvent(new MediaTransportEventArgs()
                            {
                                Category = "Routing",
                                Name = "Source Selected",
                                TouchPanel = "All",
                                Source = GetSource(CurrentSource.FullName),
                            });

                            break;
                        case "RoomMode":
                            switch (e.CurrentStatus)
                            {
                                case 0: // Local Presentation
                                    //MakeAutomaticRoutes("Room 1 Presentation", Physical.Physical.RoomStatusElements["RoomClassification"].CurrentStatus);
                                    break;
                                case 1: // VTC
                                    //MakeAutomaticRoutes("Room 1 VTC", Physical.Physical.RoomStatusElements["RoomClassification"].CurrentStatus);
                                    break;
                                case 2: // WebConf
                                    //MakeAutomaticRoutes("Room 1 WebConference", Physical.Physical.RoomStatusElements["RoomClassification"].CurrentStatus);
                                    break;
                                case 3: // AudioConf
                                    break;
                                case 4: // WatchTV
                                    //MakeAutomaticRoutes("Room 1 CATV", Physical.Physical.RoomStatusElements["RoomClassification"].CurrentStatus);
                                    break;
                            }
                            break;
                        case "RoomOffMode":

                            break;

                        case "RoomSanitizeMode":
                            if (e.CurrentStatus == 1)
                            {
                                ClearAllDestinations();
                            }
                            if (e.CurrentStatus == 0)
                            {
                                RecallAllRoutes();
                            }
                            break;
                    }
                    break;

            }
        }

        public void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args)
        {
            // No code needed yet
        }

        public void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs args)
        {
            // No code needed yet
        }
    }

}
