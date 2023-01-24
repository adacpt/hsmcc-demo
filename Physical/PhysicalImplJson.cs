using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using MAVETesting;
using DevicesControlled;
using MediaTransport;



namespace Physical
{
    public class PhysicalImplJson : IPhysical
    {
        
        String PhysicalPillarConfig = "";
        public event EventHandler<PhysicalEventArgs> PhysicalChangeEvent;

        public void GetPillarConfigValues(String json)
        {
            PhysicalPillarConfig = json;
            InitializePhysicalPillarValues();
        }
        public void InitializePhysicalPillarValues()
        {
            //CrestronConsole.PrintLine(PhysicalPillarConfig);
            RootObject rootObject = new RootObject();
            rootObject = JsonConvert.DeserializeObject<RootObject>(PhysicalPillarConfig);
            Physical.Building = rootObject.PhysicalPillar.Building;
            Physical.ClassificationList = rootObject.PhysicalPillar.ClassificationList;
            Physical.SystemStatusElements = rootObject.PhysicalPillar.SystemStatusElements;
            Physical.RoomStatusElements = rootObject.PhysicalPillar.RoomStatusElements;

            OnPhysicalChangeEvent(new PhysicalEventArgs()
            {
                Category = "Physical",
                Name = "Building"
            });
        }

        public void UpdateFloorValues(String oldName, String newName)
        {
            foreach (Floor fl in Physical.Building.Floors)
            {
                if (fl.Name.ToUpper() == oldName.ToUpper())
                {
                    fl.Name = newName;
                }
            }
            OnPhysicalChangeEvent(new PhysicalEventArgs()
            {
                Category = "Physical",
                Name = "Building"
            });
        }

        public void updateSystemValues(String oldName, String newName)
        {
            foreach (Floor fl in Physical.Building.Floors)
            {
                foreach (System sys in fl.Systems)
                {
                    if (sys.Name.ToUpper() == oldName.ToUpper())
                    {
                        sys.Name = newName;
                    }
                }
            }
            OnPhysicalChangeEvent(new PhysicalEventArgs()
            {
                Category = "Physical",
                Name = "Building"
            });
        }

        public void updateRoomValues(String oldName, String newName)
        {
            foreach (Floor fl in Physical.Building.Floors)
            {
                CrestronConsole.PrintLine("At Floor");
                foreach (System sys in fl.Systems)
                {
                    CrestronConsole.PrintLine("At System");
                    foreach (Room room in sys.Rooms)
                    {
                        if (room.Name.ToUpper() == oldName.ToUpper())
                        {
                            room.Name = newName;
                        }
                    }
                }
            }
            OnPhysicalChangeEvent(new PhysicalEventArgs()
            {
                Category = "Physical",
                Name = "Building"
            });
        }

        public void ProcessUserInteractionButtonEvent(ushort button, int smartGraphicId)
        {

            foreach (StatusElement element in Physical.SystemStatusElements)
            {
                int count = 0;
                foreach (ushort btn in element.ActionJoins)
                {
                    if (btn == button)
                    {
                        switch (element.Type)
                        {
                            case "exclusive":
                                element.CurrentStatus = count;
                                break;
                            case "toggle":
                                if (element.CurrentStatus == 0)
                                    element.CurrentStatus = 1;
                                if (element.CurrentStatus == 1)
                                    element.CurrentStatus = 0;

                                break;

                            case "discrete":

                                break;
                        }
                        OnPhysicalChangeEvent(new PhysicalEventArgs()
                        {
                            Category = "SystemStatusElements",
                            Name = element.Name,
                            CurrentStatus = element.CurrentStatus
                        });
                    }
                    count++;
                }
            }
            CrestronConsole.PrintLine("Got through System");

            
            foreach (StatusElement element in Physical.RoomStatusElements)
            {
                int joinIndex;
                joinIndex = element.ActionJoins.IndexOf((ushort)button);
                if (joinIndex >= 0)
                {
                    switch (element.Type)
                    {
                        case "exclusive":
                            element.CurrentStatus = joinIndex;
                            OnPhysicalChangeEvent(new PhysicalEventArgs()
                            {
                                Category = "RoomStatusElements",
                                Name = element.Name,
                                CurrentStatus = element.CurrentStatus
                            });
                            break;
                        case "toggle":
                            if (element.CurrentStatus == 0)
                                element.CurrentStatus = 1;
                            else if (element.CurrentStatus == 1)
                                element.CurrentStatus = 0;

                            OnPhysicalChangeEvent(new PhysicalEventArgs()
                            {
                                Category = "RoomStatusElements",
                                Name = element.Name,
                                CurrentStatus = element.CurrentStatus
                            });
                            break;
                        case "discrete":
                            if (element.SmartGraphicId == (int)ESmartGraphicID.NA)
                            {
                                OnPhysicalChangeEvent(new PhysicalEventArgs()
                                {
                                    Category = "RoomStatusElements",
                                    Name = element.Name,
                                    CurrentStatus = element.CurrentStatus
                                });
                            }
                            break;
                    }
                }

            }
        }

        public void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs args)
        {
            switch (args.EventType)
            {
                case "Button":
                    switch (args.ButtonArgs.Button)
                    {
                        default:
                            switch (args.ButtonArgs.ButtonState)
                            {

                                case EButtonState.On:
                                    ProcessUserInteractionButtonEvent((ushort)args.ButtonArgs.Button, (int)args.ButtonArgs.SmartGraphic);
                                    break;
                                case EButtonState.Off:
                                    break;

                            }
                            break;
                    }
                    break;
            }

        }
        
        public void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs args)
        {
            switch (args.EventType)
            {
                case "Button":
                    switch (args.ButtonArgs.Button)
                    {
                        default:
                            switch (args.ButtonArgs.ButtonState)
                            {
                                case EButtonState.On:
                                    ProcessUserInteractionButtonEvent((ushort)args.ButtonArgs.Button, (int)args.ButtonArgs.SmartGraphic);
                                    break;
                                case EButtonState.Off:
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        public void OnPhysicalChangeEvent(PhysicalEventArgs e)
        {
            PhysicalChangeEvent?.Invoke(null, e);
        }


        public void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args)
        {
            //
        }
        
        public void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args)
        {
            //
        }
        
        
    }
}
