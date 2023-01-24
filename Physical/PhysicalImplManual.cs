using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
/*
using UserInteraction;
using MediaTransport;
using DevicesControlled;
*/

namespace Physical
{
    public class PhysicalImplManual 
    {
        /*
        String PhysicalPillarConfig = "";

        public void GetPillarConfigValues(String config)
        {
            PhysicalPillarConfig = config;
        }
        public void InitializePhysicalPillarValues()
        {
            Floor floor = new Floor();
            System system = new System();
            Room room = new Room
            {
                Name = "Living Room",
                Deleted = 0
            };

            system.Name = "Test System";
            system.Identifier = 1;
            system.Deleted = 0;
            system.Rooms.Add(room);

            floor.Name = "1st Floor";
            floor.Systems.Add( system);
            floor.Deleted = 0;

            Physical.Building.Name = "Household";
            CrestronConsole.PrintLine("Added 1st Floor");


            // initialize classifications
            Dictionary<string, Classification> ClassificationList = new Dictionary<string, Classification>();

            Classification noClass = new Classification
            {
                Name = "NO STATE",
                RelativeValue = 0,
                Deleted = 0
            };
            ClassificationList["NO STATE"] = noClass;

            Classification unclass = new Classification
            {
                Name = "UNCLASSIFIED",
                RelativeValue = 1,
                Deleted = 0
            };
            ClassificationList["UNCLASS"] = unclass;

            Classification secret = new Classification
            {
                Name = "SECRET",
                RelativeValue = 2,
                Deleted = 0
            };
            ClassificationList["SECRET"] = secret;

            Physical.ClassificationList = ClassificationList;

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
            Physical.OnPhysicalChangeEvent(new PhysicalEventArgs()
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
            Physical.OnPhysicalChangeEvent(new PhysicalEventArgs()
            {
                Category = "Physical",
                Name = "Building"
            });
        }

        public void updateRoomValues(String oldName, String newName)
        {
            foreach (Floor fl in Physical.Building.Floors)
            {
                foreach (System sys in fl.Systems)
                {
                    foreach (Room room in sys.Rooms)
                    {
                        if (room.Name.ToUpper() == oldName.ToUpper())
                        {
                            room.Name = newName;
                        }
                    }
                }
            }
        }
        

        

        public void OnUserInteractionChangeMomentaryEvent(object sender, UserInteractionEventArgs args)
        {
            CrestronConsole.PrintLine("Saw Button Event in Physical");
            switch (args.EventType)
            {
                case "Button":
                    switch (args.ButtonArgs.Button)
                    {
                        case 13:
                            switch (args.ButtonArgs.DigitalValue)
                            {
                                case DigitalState.On:
                                    //updateRoomValues("Living Room", "Kitchen");
                                    break;
                                case DigitalState.Off:

                                    break;
                            }
                            break;
                        case 14:
                            switch (args.ButtonArgs.DigitalValue)
                            {
                                case DigitalState.On:
                                    //updateRoomValues("Living Room", "Basement");
                                    break;
                                case DigitalState.Off:

                                    break;
                            }
                            break;
                    }
                    break;
            }

        }

        public void HoldButton_Momentary(object sender, UserInteractionEventArgs args)
        {
            switch (args.EventType)
            {
                case "Button":
                    switch (args.ButtonArgs.Button)
                    {
                        default:
                            switch (args.ButtonArgs.DigitalValue)
                            {
                                case DigitalState.On:
                                    
                                    break;
                                case DigitalState.Off:
                                    break;
                            }
                            break;
                    }
                    break;
            }

        }

        public void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args)
        {
        }

        public void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args)
        {
            
        }
        */
    }
}
