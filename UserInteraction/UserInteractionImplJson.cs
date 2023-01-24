using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharpPro.UI;
using Newtonsoft.Json;
using MediaTransport;
using Physical;
using DspDevice;
using VtcDevice;
using CatvDevice;
using DevicesControlled;
using MAVETesting;


namespace UserInteraction
{
    public class UserInteractionImplJson : IUserInteraction
    {
        
        readonly List<XpanelForSmartGraphics> MyXPanels;
        Source CurrentSource = new Source();
        String CurrentDestGroup = "";
        String CurrentKeyboardPurpose = "";
        String UserInteractionPillarConfig = "";
        String CurrentTouchPanel = "";
        int CurrentClassification;


        public UserInteractionImplJson(List<XpanelForSmartGraphics> xPanels) 
        {
            MyXPanels = new List<XpanelForSmartGraphics>();
            MyXPanels = xPanels;
        }

        public void GetPillarConfigValues(String json)
        {
            UserInteractionPillarConfig = json;
            InitializeUserInteractionValues();
        }

        public void InitializeUserInteractionValues()
        {

            RootObject rootObject = new RootObject();
            rootObject = JsonConvert.DeserializeObject<RootObject>(UserInteractionPillarConfig);
            UserInteraction.TouchPanels = rootObject.UserInteractionPillar.TouchPanels;
            UserInteraction.Keyboard = rootObject.UserInteractionPillar.Keyboard;
            UserInteraction.KeyboardJoinMaps = rootObject.UserInteractionPillar.KeyboardJoinMaps;
            UserInteraction.PageNavigationElements = rootObject.UserInteractionPillar.PageNavigationElements;
            UserInteraction.ButtonJoinCollections = rootObject.UserInteractionPillar.ButtonJoinCollections;
            UserInteraction.DeviceFeedbackElements = rootObject.UserInteractionPillar.DeviceFeedbackElements;

            ProcessPageNavigationExclusiveFeedback("Idle", "PageNavigation", "All", ESmartGraphicID.NA);
        }

        public KeyboardJoinMap GetKeyBoardJoinMap(String keyboardName)
        {
            KeyboardJoinMap selectedKb = new KeyboardJoinMap();
            foreach (KeyboardJoinMap keyboardMap in UserInteraction.KeyboardJoinMaps)
            {
                if (keyboardMap.Name == keyboardName)
                {
                    selectedKb = keyboardMap;
                }
            }
            return selectedKb;
        }

        public void MapUIPillarButtonEvents(TouchPanelEventArgs arg)
        {
            CurrentTouchPanel = arg.TouchPanel;

            if (arg.ButtonArgs.ButtonState == EButtonState.On)
            {
                {
                    // get the index of PageNavigationSetting Action Join
                    int joinIndex = -1;

                    if (arg.ButtonArgs.Button == UserInteraction.ButtonJoinCollections["BlankSourceBtn"].PressDigitalJoin)
                    {
                        ButtonJoinCollectionFeedback("BlankSourceBtn", true, arg.TouchPanel);
                    }

                    foreach (KeyboardJoinMap keyboardMap in UserInteraction.KeyboardJoinMaps)
                    {
                        if (keyboardMap.SmartGraphic == (int)arg.ButtonArgs.SmartGraphic && keyboardMap.ActivationPressJoins.Contains((ushort)arg.ButtonArgs.Button))
                        {
                            CurrentKeyboardPurpose = keyboardMap.Name;
                            keyboardMap.TypedString = "";
                            ButtonJoinCollectionSerial(CurrentKeyboardPurpose, ESmartGraphicID.NA, 0, keyboardMap.TypedString, arg.TouchPanel);
                        }
                    }

                    if (arg.ButtonArgs.Button == UserInteraction.Keyboard.BackSpaceJoin)
                    {
                        foreach(KeyboardJoinMap keyboardMap in UserInteraction.KeyboardJoinMaps)
                        {
                            if (CurrentKeyboardPurpose == keyboardMap.Name)
                            {
                                keyboardMap.TypedString = keyboardMap.TypedString.Substring(0, keyboardMap.TypedString.Length - 1);
                                ButtonJoinCollectionSerial(CurrentKeyboardPurpose, ESmartGraphicID.NA, 0, keyboardMap.TypedString, arg.TouchPanel);
                            }
                        }
                    }

                    joinIndex = UserInteraction.Keyboard.KeyBoardJoins.IndexOf((ushort)arg.ButtonArgs.Button);
                    if (joinIndex >= 0)
                    {
                        foreach (KeyboardJoinMap keyboardMap in UserInteraction.KeyboardJoinMaps)
                        {
                            if (CurrentKeyboardPurpose == keyboardMap.Name)
                            {
                                keyboardMap.TypedString += UserInteraction.Keyboard.KeyBoardCharacters[joinIndex];
                                ButtonJoinCollectionSerial(CurrentKeyboardPurpose, ESmartGraphicID.NA, 0, keyboardMap.TypedString, arg.TouchPanel);
                            }
                        }  
                    }

                    if (arg.ButtonArgs.Button == UserInteraction.ButtonJoinCollections["ATCSpeedDialCancel"].PressDigitalJoin)
                    {
                        ButtonJoinCollectionFeedback("ATCSpeedDialKeyboard", false, arg.TouchPanel);
                        ButtonJoinCollectionFeedback("ATCKeyboard", true, arg.TouchPanel);
                    }

                    if (arg.ButtonArgs.Button == UserInteraction.ButtonJoinCollections["VTCSpeedDialCancel"].PressDigitalJoin)
                    {
                        ButtonJoinCollectionFeedback("VTCSpeedDialCancel", false, arg.TouchPanel);
                        ButtonJoinCollectionFeedback("VTCKeyboard", true, arg.TouchPanel);
                    }

                    if (arg.ButtonArgs.Button == UserInteraction.ButtonJoinCollections["PCSourceGroupSelectionBtn"].PressDigitalJoin ||
                        arg.ButtonArgs.Button == UserInteraction.ButtonJoinCollections["VTCSourceGroupSelectionBtn"].PressDigitalJoin ||
                        arg.ButtonArgs.Button == UserInteraction.ButtonJoinCollections["MediaSourceGroupSelectionBtn"].PressDigitalJoin )
                    {
                        PageNavigationElementGroupFeedback("PCSourceSelection", false, arg.TouchPanel);
                        PageNavigationElementGroupFeedback("CollaborationSourceSelection", false, arg.TouchPanel);
                        PageNavigationElementGroupFeedback("MediaSourceSelection", false, arg.TouchPanel);
                    }

                    foreach (KeyValuePair<String, PageNavigationElement> pair in UserInteraction.PageNavigationElements)
                    {
                        PageNavigationElement pageNav = UserInteraction.PageNavigationElements[pair.Key];
                        if ((pageNav.PressJoins.Count > 0) && (pageNav.FeedbackType == "exclusive"))
                        {
                            joinIndex = pageNav.PressJoins.IndexOf((ushort)arg.ButtonArgs.Button);
                            if (joinIndex >= 0)
                            {
                                ProcessPageNavigationExclusiveFeedback(pageNav.ElementNames[joinIndex], pair.Key, arg.TouchPanel, ESmartGraphicID.NA);
                            }
                        }
                    }
                }

                if (arg.ButtonArgs.SmartGraphic == MAVETesting.ESmartGraphicID.MediaSources)
                {
                    switch (arg.ButtonArgs.Button)
                    {
                        case 4012:
                            ProcessPageNavigationExclusiveFeedback("BluRay", "PageNavigation", arg.TouchPanel, ESmartGraphicID.NA);
                            break;
                        case 4016:
                            ProcessPageNavigationExclusiveFeedback("CATV", "PageNavigation", arg.TouchPanel, ESmartGraphicID.NA);
                            break;
                        case 4020:
                            ProcessPageNavigationExclusiveFeedback("CATV", "PageNavigation", arg.TouchPanel, ESmartGraphicID.NA);
                            break;
                    }
                }

            }
            if (arg.ButtonArgs.ButtonState == EButtonState.Hold)
            {
                if (arg.ButtonArgs.Button == UserInteraction.Keyboard.BackSpaceJoin)
                {
                    foreach (KeyboardJoinMap keyboardMap in UserInteraction.KeyboardJoinMaps)
                    {
                        if (CurrentKeyboardPurpose == keyboardMap.Name)
                        {
                            keyboardMap.TypedString = "";
                            ButtonJoinCollectionSerial(CurrentKeyboardPurpose, ESmartGraphicID.NA, 0, keyboardMap.TypedString, arg.TouchPanel);
                        }
                    }
                }
            }
        }
        public int GetTouchPanelIndex(String name)
        {
            int index = 0;
            for ( int i = 0; i < UserInteraction.TouchPanels.Count(); i++)
            {
                if ( UserInteraction.TouchPanels[i].Name == name)
                {
                    index = i;
                }
            }

            return index;
        }

        public KeyValuePair<uint, SmartObject> GetSmartGraphicSmartObject(ESmartGraphicID smartGraphic, XpanelForSmartGraphics tp)
        {
            KeyValuePair<uint, SmartObject> smartGraphicObject = new KeyValuePair<uint, SmartObject>();
            if (smartGraphic != (int)ESmartGraphicID.NA)
            {
                foreach (KeyValuePair<uint, SmartObject> pair in tp.SmartObjects)
                {
                    if (pair.Value.ID == (uint)smartGraphic)
                    {
                        smartGraphicObject = pair;
                    }
                }
            } else
            {
                CrestronConsole.PrintLine("ERROR: No SmartGraphic object found in GetSmartGraphicSmartObject");
            }

            return smartGraphicObject;
        }

        public void ToggleButtonJoinCollectionStatus(string statusName, string touchPanel)
        {
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    if (UserInteraction.ButtonJoinCollections[statusName].CurrentStatus == 0)
                    {
                        UserInteraction.ButtonJoinCollections[statusName].CurrentStatus = 1;
                        ButtonJoinCollectionFeedback(statusName, true, touchPanel);
                    }
                    else
                    {
                        UserInteraction.ButtonJoinCollections[statusName].CurrentStatus = 0;
                        ButtonJoinCollectionFeedback(statusName, true, touchPanel);
                    }
                }
            }
        }

        // Need to add smartgraphic capabilities
        public void ButtonJoinCollectionFeedback(string buttonCollection, Boolean feedback, string touchPanel)
        {
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    MyXPanels[i].BooleanInput[UserInteraction.ButtonJoinCollections[buttonCollection].FeedbackDigitalJoin].BoolValue = feedback;
                }
            }
            else
            {
                MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.ButtonJoinCollections[buttonCollection].FeedbackDigitalJoin].BoolValue = feedback;
            }
        }

        public void ButtonJoinCollectionVisibility(string buttonCollection, Boolean visibility, string touchPanel)
        {
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    MyXPanels[i].BooleanInput[UserInteraction.ButtonJoinCollections[buttonCollection].VisibilityDigitalJoin].BoolValue = visibility;
                }
            }
            else
            {
                MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.ButtonJoinCollections[buttonCollection].VisibilityDigitalJoin].BoolValue = visibility;
            }
        }

        public void ButtonJoinCollectionEnable(string buttonCollection, Boolean enable, string touchPanel)
        {
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    MyXPanels[i].BooleanInput[UserInteraction.ButtonJoinCollections[buttonCollection].EnableDigitalJoin].BoolValue = enable;
                }
            }
            else
            {
                MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.ButtonJoinCollections[buttonCollection].EnableDigitalJoin].BoolValue = enable;
            }
        }

        public void ButtonJoinCollectionSerial(string buttonCollection, ESmartGraphicID smartGraphic, int joinIndex, string text, string touchPanel)
        {
            if (touchPanel == "All")
            {
                foreach (XpanelForSmartGraphics tp in MyXPanels)
                {
                    if (smartGraphic != (int)ESmartGraphicID.NA)
                    {
                        GetSmartGraphicSmartObject(smartGraphic, tp).Value.StringInput[UserInteraction.ButtonJoinCollections[buttonCollection].SerialJoins[joinIndex]].StringValue = text;
                    }
                    else
                    {
                        MyXPanels[GetTouchPanelIndex(tp.Name)].StringInput[UserInteraction.ButtonJoinCollections[buttonCollection].SerialJoins[joinIndex]].StringValue = text;
                    }
                }
            }
            else
            {
                if (smartGraphic != (int)ESmartGraphicID.NA)
                {
                    GetSmartGraphicSmartObject(smartGraphic, MyXPanels[GetTouchPanelIndex(touchPanel)]).Value.StringInput[UserInteraction.ButtonJoinCollections[buttonCollection].SerialJoins[joinIndex]].StringValue = text;
                }
                else
                {
                    MyXPanels[GetTouchPanelIndex(touchPanel)].StringInput[UserInteraction.ButtonJoinCollections[buttonCollection].SerialJoins[joinIndex]].StringValue = text;
                }
            }
        }

        public void ButtonJoinCollectionAnalog( string buttonCollection, short analogValue, string touchPanel)
        {
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    MyXPanels[i].UShortInput[UserInteraction.ButtonJoinCollections[buttonCollection].AnalogJoin].ShortValue = analogValue;
                }
            } else
            {
                MyXPanels[GetTouchPanelIndex(touchPanel)].UShortInput[UserInteraction.ButtonJoinCollections[buttonCollection].AnalogJoin].ShortValue = analogValue;
            }     
        }

        public void ProcessPageNavigationExclusiveFeedback(String elementName, String groupName, String touchPanel, ESmartGraphicID smartGraphicId)
        {
            int joinIndex = -1;

            if (UserInteraction.PageNavigationElements.ContainsKey(groupName))
            {
                PageNavigationElement pageNav = UserInteraction.PageNavigationElements[groupName];

                if (pageNav.ElementNames.Contains(elementName))
                {
                    joinIndex = pageNav.ElementNames.FindIndex(a => a.Contains(elementName));

                    if (touchPanel == "All")
                    {
                        foreach (XpanelForSmartGraphics tp in MyXPanels)
                        {
                            if ((joinIndex >= 0) && (smartGraphicId != ESmartGraphicID.NA)) // SmartGraphic
                            {
                                ExecuteSmartGraphicPageNavigationElementExclusiveFeedback(pageNav, joinIndex, touchPanel, smartGraphicId);
                            }
                            else
                            {
                                if (joinIndex >= 0)
                                {
                                    ExecutePageNavigationElementExclusiveFeedback(pageNav, joinIndex, touchPanel);
                                }
                            }
                        }
                        
                    }
                    else  // Individual touchpanel
                    {
                        if ((joinIndex >= 0) && (smartGraphicId != ESmartGraphicID.NA)) // SmartGraphic
                        {
                            ExecuteSmartGraphicPageNavigationElementExclusiveFeedback(pageNav, joinIndex, touchPanel, smartGraphicId);
                        }
                        else if (joinIndex >= 0)
                        {
                            ExecutePageNavigationElementExclusiveFeedback(pageNav, joinIndex, touchPanel);
                        }
                    }
                }
            }
            
            UserInteractionStatusEventArgs touchPanelStatusArgs = new UserInteractionStatusEventArgs()
            {
                Category = groupName,
                Name = elementName,
                CurrentStatus = joinIndex
            };
            UserInteraction.OnUserInteractionChangeEvent(new UserInteractionEventArgs()
            {
                TouchPanel = touchPanel,
                EventType = "Touch Panel Status",
                StatusArgs = touchPanelStatusArgs
            });
        }

        public void ExecuteSmartGraphicPageNavigationElementExclusiveFeedback( PageNavigationElement pageNav, int joinIndex, String touchPanel, ESmartGraphicID smartGraphicId )
        {
            pageNav.CurrentSelection = joinIndex;

            uint counter = 0;
            foreach (List<String> joins in pageNav.SmartGraphicFeedbackJoins)
            {
                if (counter == joinIndex)
                {
                    foreach (String join in joins)
                    {
                        GetSmartGraphicSmartObject(smartGraphicId, MyXPanels[GetTouchPanelIndex(touchPanel)]).Value.BooleanInput[join].BoolValue = true;
                    }
                }
                else
                {
                    foreach (String join in joins)
                    {
                        GetSmartGraphicSmartObject(smartGraphicId, MyXPanels[GetTouchPanelIndex(touchPanel)]).Value.BooleanInput[join].BoolValue = false;
                    }
                }
                counter++;
            }
        }

        public void ExecutePageNavigationElementExclusiveFeedback(PageNavigationElement pageNav, int joinIndex, String touchPanel)
        {
            pageNav.CurrentSelection = joinIndex;

            uint counter = 0;
            foreach (List<ushort> joins in pageNav.FeedbackJoins)
            {
                if (counter == joinIndex)
                {
                    foreach (ushort join in joins)
                    {
                        PageNavigationElementIndexDiscreteFeedback(join, true, touchPanel);
                    }
                }
                else
                {
                    foreach (ushort join in joins)
                    {
                        PageNavigationElementIndexDiscreteFeedback(join, false, touchPanel);
                    }
                }
                counter++;
            }
        }

        public void PageNavigationElementIndexDiscreteFeedback(ushort join, Boolean feedback, string touchPanel)
        {
            if (touchPanel == "All")
            {
                foreach (XpanelForSmartGraphics tp in MyXPanels)
                {
                    tp.BooleanInput[join].BoolValue = feedback;
                }
            }
            else
            {
                MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[join].BoolValue = feedback;
            }
        }

        public void SmartGraphicPageNavigationElementIndexDiscreteFeedback(string elementName, String name, Boolean feedback, string touchPanel)
        {
            if (touchPanel == "All")
            {
                foreach (XpanelForSmartGraphics tp in MyXPanels)
                {
                    GetSmartGraphicSmartObject((ESmartGraphicID)UserInteraction.PageNavigationElements[elementName].SmartGraphicId, tp).Value.BooleanInput[name].BoolValue = feedback;
                }
            }
            else
            {
                GetSmartGraphicSmartObject((ESmartGraphicID)UserInteraction.PageNavigationElements[elementName].SmartGraphicId, MyXPanels[GetTouchPanelIndex(touchPanel)]).Value.BooleanInput[name].BoolValue = feedback;
            }
        }

        public void PageNavigationElementGroupFeedback(string elementName, Boolean feedback, string touchPanel)
        {

            if (UserInteraction.PageNavigationElements[elementName].SmartGraphicId == 0)
            {
                foreach (List<ushort> feedbackJoins in UserInteraction.PageNavigationElements[elementName].FeedbackJoins)
                {
                    foreach (ushort join in feedbackJoins)
                    {
                        PageNavigationElementIndexDiscreteFeedback(join, feedback, touchPanel);
                    }
                }
            }
            else
            {
                foreach (List<String> joins in UserInteraction.PageNavigationElements[elementName].SmartGraphicFeedbackJoins)
                {
                    foreach (String j in joins)
                    {
                        SmartGraphicPageNavigationElementIndexDiscreteFeedback(elementName, j, feedback, touchPanel);
                    }
                }
            }
        }



        // The next few sections manage routing feedback on the touch panel

        public void SortSources(String groupName, String pageNavElement, String sourceCategory, ESmartGraphicID smartGraphic, String touchPanel)
        {
            switch (groupName)
            {
                case "PC":
                    if (UserInteraction.PageNavigationElements[pageNavElement].PopulationType == "variable")
                    {
                        PopulateVariableSources(pageNavElement, sourceCategory, smartGraphic, touchPanel);
                        PageNavigationElementGroupFeedback(pageNavElement, false, touchPanel);
                    }
                    break;
                case "VTCPresentation":
                    if (UserInteraction.PageNavigationElements[pageNavElement].PopulationType == "variable")
                    {
                        PopulateVariableSources(pageNavElement, sourceCategory, smartGraphic, touchPanel);
                        PageNavigationElementGroupFeedback(pageNavElement, false, touchPanel);
                    }
                    break;
                case "Collaboration":
                    if (UserInteraction.PageNavigationElements[pageNavElement].PopulationType == "static")
                    {
                        PopulateStaticSources(pageNavElement, sourceCategory, touchPanel);
                        PageNavigationElementGroupFeedback(pageNavElement, false, touchPanel);
                    }
                    break;
                case "Media":
                    if (UserInteraction.PageNavigationElements[pageNavElement].PopulationType == "variable")
                    {
                        PopulateVariableSources(pageNavElement, sourceCategory, smartGraphic, touchPanel);
                        PageNavigationElementGroupFeedback(pageNavElement, false, touchPanel);
                    }
                    break;
                case "Shared":
                    //
                    break;
            }
            
        }

        public void PopulateVariableSources ( String pageNavElement, String sourceCategory, ESmartGraphicID smartGraphic, String touchPanel)
        {
            int count;
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Clear();
                    count = 0;
                    foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[i].SortedSourceIODevices[sourceCategory])
                    {
                        foreach (MediaTransport.Source source in iod.Sources)
                        {
                            if (source.Classification <= CurrentClassification)
                            {
                                PopulateVariableSourceTouchPanelElements(GetSmartGraphicSmartObject(smartGraphic, MyXPanels[i]), count, pageNavElement, source, true);
                            }
                            else
                            {
                                PopulateVariableSourceTouchPanelElements(GetSmartGraphicSmartObject(smartGraphic, MyXPanels[i]), count, pageNavElement, source, false);
                            }
                            count++;
                        }
                    }
                }
            }
            else
            {
                UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Clear();
                count = 0;
                foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[GetTouchPanelIndex(touchPanel)].SortedSourceIODevices[sourceCategory])
                {
                    foreach (MediaTransport.Source source in iod.Sources)
                    {
                        if (source.Classification <= CurrentClassification)
                        {
                            PopulateVariableSourceTouchPanelElements(GetSmartGraphicSmartObject(smartGraphic, MyXPanels[GetTouchPanelIndex(touchPanel)]), count, pageNavElement, source, true);
                        }
                        else
                        {
                            PopulateVariableSourceTouchPanelElements(GetSmartGraphicSmartObject(smartGraphic, MyXPanels[GetTouchPanelIndex(touchPanel)]), count, pageNavElement, source, false);
                        }
                        count++;
                    }
                }
            }
        }

        public void PopulateVariableSourceTouchPanelElements(KeyValuePair<uint, SmartObject> pair, int count, String pageNavElement, Source source, Boolean boolValue)
        {
            if (count < UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicVisibilityJoins.Count)
            {
                pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicVisibilityJoins[count]].BoolValue = boolValue;
            }

            if (count < UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicEnableJoins.Count)
            {
                pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicEnableJoins[count]].BoolValue = boolValue;
            }

            if (count < UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicSerialJoins.Count)
            {
                pair.Value.StringInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicSerialJoins[count]].StringValue = source.FullName;
            }

            if (count < UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicAnalogJoins.Count)
            {
                pair.Value.UShortInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicAnalogJoins[count]].ShortValue = (short)source.Classification;
            }

            UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Add(source.FullName);
        }

        public void PopulateStaticSources(String pageNavElement, String sourceCategory, String touchPanel)
        {
            int count;
            int sourceCount;
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    if (MyXPanels[i].Name == touchPanel)
                    {
                        count = 0;
                        UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Clear();
                        sourceCount = 0;
                        foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[GetTouchPanelIndex(touchPanel)].SortedSourceIODevices[sourceCategory])
                        {
                            foreach (MediaTransport.Source source in iod.Sources)
                            {
                                MyXPanels[i].StringInput[UserInteraction.PageNavigationElements[pageNavElement].SerialJoins[count]].StringValue = source.FullName;
                                if (source.Classification <= CurrentClassification)
                                {
                                    MyXPanels[i].BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].EnableJoins[count]].BoolValue = true;
                                }
                                else
                                {
                                    MyXPanels[i].BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].EnableJoins[count]].BoolValue = false;
                                }

                                UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Add(source.FullName);
                                sourceCount++;
                                count++;
                            }
                        }
                        if (sourceCount != UserInteraction.PageNavigationElements[pageNavElement].SerialJoins.Count)
                        {
                            for (int s = (sourceCount - 1); s <= UserInteraction.PageNavigationElements[pageNavElement].SerialJoins.Count; s++)
                            {
                                MyXPanels[GetTouchPanelIndex(touchPanel)].StringInput[UserInteraction.PageNavigationElements[pageNavElement].SerialJoins[count]].StringValue = "";
                                MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].EnableJoins[count]].BoolValue = false;
                            }
                        }
                        else
                        {
                            CrestronConsole.PrintLine("SourceCount is equal");
                        }
                    }
                }
            }
            else
            {
                count = 0;
                UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Clear();
                sourceCount = 0;
                foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[GetTouchPanelIndex(touchPanel)].SortedSourceIODevices[sourceCategory])
                {
                    foreach (MediaTransport.Source source in iod.Sources)
                    {
                        MyXPanels[GetTouchPanelIndex(touchPanel)].StringInput[UserInteraction.PageNavigationElements[pageNavElement].SerialJoins[count]].StringValue = source.FullName;
                        if (source.Classification <= CurrentClassification)
                        {
                            MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].EnableJoins[count]].BoolValue = true;
                        }
                        else
                        {
                            MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.PageNavigationElements["CollaborationSourceSelection"].EnableJoins[count]].BoolValue = false;
                        }

                        UserInteraction.PageNavigationElements["CollaborationSourceSelection"].ElementNames.Add(source.FullName);
                        sourceCount++;
                        count++;
                    }
                }
                if (sourceCount != UserInteraction.PageNavigationElements["CollaborationSourceSelection"].SerialJoins.Count)
                {
                    for (int i = (sourceCount - 1); i <= UserInteraction.PageNavigationElements["CollaborationSourceSelection"].SerialJoins.Count; i++)
                    {
                        MyXPanels[GetTouchPanelIndex(touchPanel)].StringInput[UserInteraction.PageNavigationElements["CollaborationSourceSelection"].SerialJoins[count]].StringValue = "";
                        MyXPanels[GetTouchPanelIndex(touchPanel)].BooleanInput[UserInteraction.PageNavigationElements["CollaborationSourceSelection"].EnableJoins[count]].BoolValue = false;
                    }
                }
                else
                {
                    CrestronConsole.PrintLine("SourceCount is equal");
                }
            }
        }

        public void SortManualDestinations()
        {

            String pageNavElement = "";
            String groupName = "";
            ESmartGraphicID smartGraphic = ESmartGraphicID.NA;
            foreach (MediaTransport.TouchPanel tp in MediaTransport.MediaTransport.TouchPanels)
            {
                foreach (KeyValuePair<String, MediaTransport.DestinationButtonOutputMap> destMap in tp.DestinationButtonOutputMap)
                {
                    if (destMap.Key == "DisplayDestinationButtonMap")
                    {
                        groupName = "Display";
                        pageNavElement = "DisplayDestinationSelection";
                        smartGraphic = ESmartGraphicID.DisplayDestinations;
                        SortDestinations(groupName, pageNavElement, destMap.Key, smartGraphic, tp.Name);
                    }
                    if (destMap.Key == "VTCDestinationSelection")
                    {
                        groupName = "Collaboration";
                        pageNavElement = "VTCDestinationSelection";
                        smartGraphic = ESmartGraphicID.DisplayDestinations;
                        SortDestinations(groupName, pageNavElement, destMap.Key, smartGraphic, tp.Name);
                    }
                }
            }
        }

        public void SortDestinations ( String groupName, String pageNavElement, String destCategory, ESmartGraphicID smartGraphic, String touchPanel )
        {
            switch (groupName)
            {
                case "Display":
                    CurrentDestGroup = groupName;
                    if (UserInteraction.PageNavigationElements[pageNavElement].PopulationType == "variable")
                    {
                        PopulateVariableDestinations(pageNavElement, destCategory, ESmartGraphicID.DisplayDestinations, touchPanel );
                    }
                    break;

                case "Collaboration":
                    CurrentDestGroup = groupName;
                    if (UserInteraction.PageNavigationElements[pageNavElement].PopulationType == "static")
                    {
                        PopulateStaticDestinations(pageNavElement, destCategory, touchPanel);
                    }
                    break;

                case "Shared":
                    //
                    break;
            }
        }

        public Boolean ValidateDestinationViability(MediaTransport.Source source, MediaTransport.Destination dest)
        {
            Boolean result = false;
            if ((dest.SignalType == ESignalType.AudioVideo) && (source.SignalType == ESignalType.AudioVideo || source.SignalType == ESignalType.VideoOnly))
            {
                result = true;
            }
            else if ((dest.SignalType == ESignalType.VideoOnly) && (source.SignalType == ESignalType.AudioVideo || source.SignalType == ESignalType.VideoOnly))
            {
                result = true;
            }
            else if ((dest.SignalType == ESignalType.AudioOnly) && (source.SignalType == ESignalType.AudioVideo || source.SignalType == ESignalType.AudioOnly))
            {
                result = true;
            }
            else if ((dest.SignalType == ESignalType.USB) && (source.SignalType == ESignalType.USB))
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public void PopulateVariableDestinations (String pageNavElement, String destCategory, ESmartGraphicID smartGraphic, String touchPanel)
        {
            int count = 0;
            UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Clear();
            if (touchPanel == "All")
            {
                for (int i = 0; i < MyXPanels.Count(); i++)
                {
                    if (MyXPanels[i].Name == touchPanel)
                    {
                        foreach (KeyValuePair<uint, SmartObject> pair in MyXPanels[i].SmartObjects)
                        {
                            if (pair.Value.ID == (int)smartGraphic)
                            {
                                foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[GetTouchPanelIndex(touchPanel)].SortedDestinationIODevices[destCategory])
                                {
                                    foreach (MediaTransport.Destination destination in iod.Destinations)
                                    {
                                        UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Add(destination.FullName);
                                        pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicVisibilityJoins[count]].BoolValue = true;
                                        if (ValidateDestinationViability(CurrentSource, destination))
                                        {
                                            pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicEnableJoins[count]].BoolValue = true;
                                        }
                                        else
                                        {
                                            pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicEnableJoins[count]].BoolValue = false;
                                        }

                                        foreach (string join in UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicFeedbackJoins[count])
                                        {
                                            pair.Value.BooleanInput[join].BoolValue = true;
                                        }
                                        pair.Value.StringInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicSerialJoins[count]].StringValue = destination.FullName;
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
            } else
            {
                foreach (KeyValuePair<uint, SmartObject> pair in MyXPanels[GetTouchPanelIndex(touchPanel)].SmartObjects)
                {
                    if (pair.Value.ID == (uint)ESmartGraphicID.DisplayDestinations)
                    {
                        foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[GetTouchPanelIndex(touchPanel)].SortedDestinationIODevices[destCategory])
                        {
                            foreach (MediaTransport.Destination destination in iod.Destinations)
                            {
                                UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Add(destination.FullName);
                                pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicVisibilityJoins[count]].BoolValue = true;
                                if (ValidateDestinationViability(CurrentSource, destination))
                                {
                                    pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicEnableJoins[count]].BoolValue = true;
                                }
                                else
                                {
                                    pair.Value.BooleanInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicEnableJoins[count]].BoolValue = false;
                                }

                                foreach (String join in UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicFeedbackJoins[count])
                                {
                                    pair.Value.BooleanInput[join].BoolValue = true;
                                }
                                pair.Value.StringInput[UserInteraction.PageNavigationElements[pageNavElement].SmartGraphicSerialJoins[count]].StringValue = destination.FullName;
                                count++;
                            }
                        }
                    }
                }
            }
        }

        public void PopulateStaticDestinations(String pageNavElement, String destCategory, String touchPanel)
        {
            UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Clear();
            foreach (MediaTransport.IODevice iod in MediaTransport.MediaTransport.TouchPanels[GetTouchPanelIndex(touchPanel)].SortedDestinationIODevices[destCategory])
            {
                foreach (MediaTransport.Destination destination in iod.Destinations)
                {
                    UserInteraction.PageNavigationElements[pageNavElement].ElementNames.Add(destination.FullName);
                }
            }
        }

        public void PopulateRouteDestinationFeedback(String destinationName, String sourceName)
        {
            foreach (KeyValuePair<String, PageNavigationElement> pair in UserInteraction.PageNavigationElements)
            {
                PageNavigationElement pageNav = UserInteraction.PageNavigationElements[pair.Key];
                if (pageNav.ElementNames.Count > 0)
                {
                    int joinIndex = pageNav.ElementNames.IndexOf(destinationName);

                    if (pageNav.ElementNames.Contains(destinationName) && pageNav.SmartGraphicId != (uint)ESmartGraphicID.NA)
                    {
                        foreach (XpanelForSmartGraphics tp in MyXPanels)
                        {
                            GetSmartGraphicSmartObject((ESmartGraphicID)pageNav.SmartGraphicId, tp).Value.StringInput[pageNav.SerialJoins[joinIndex]].StringValue = sourceName;
                        }
                    }
                    else if (pageNav.ElementNames.Contains(destinationName) && pageNav.SmartGraphicId == (uint)ESmartGraphicID.NA)
                    {
                        foreach (XpanelForSmartGraphics tp in MyXPanels)
                        {
                            tp.StringInput[pageNav.SerialJoins[joinIndex]].StringValue = sourceName;
                        }
                    }
                }
            }
        }


        

        public void OnUserInteractionChangeMomentaryEvent(object sender, TouchPanelEventArgs e)
        {
            MapUIPillarButtonEvents(e);
        }
        public void OnUserInteractionChangeHoldEvent(object sender, TouchPanelEventArgs e)
        {
            MapUIPillarButtonEvents(e);
        }
        
        

        public void OnMediaTransportChangeEvent(object sender, MediaTransportEventArgs args )
        {
            
            String pageNavElement = "";
            ESmartGraphicID smartGraphic = ESmartGraphicID.NA;
            if ( args.Category == "IOSourceDevicesSorted") {
                switch (args.Name)
                {
                    case "PC":
                        pageNavElement = "PCSourceSelection";
                        smartGraphic = ESmartGraphicID.PCSources;
                        break;
                    case "VTCPresentation":
                        pageNavElement = "PCSourceSelection";
                        smartGraphic = ESmartGraphicID.VTCPresentationSources;
                        break;
                    case "Collaboration":
                        pageNavElement = "CollaborationSourceSelection";
                        smartGraphic = ESmartGraphicID.NA;
                        break;
                    case "Media":
                        pageNavElement = "MediaSourceSelection";
                        smartGraphic = ESmartGraphicID.MediaSources;
                        break;
                }
                SortSources(args.Name, pageNavElement, args.MapName, smartGraphic, args.TouchPanel);
            }

            if (args.Category == "IODestinationDevicesSorted")
            {
                switch (args.Name)
                {
                    case "Display":
                        pageNavElement = "DisplayDestinationSelection";
                        smartGraphic = ESmartGraphicID.DisplayDestinations;
                        break;
                    case "Collaboration":
                        pageNavElement = "VTCDestinationSelection";
                        smartGraphic = ESmartGraphicID.NA;
                        break;
                }
                SortDestinations(args.Name, pageNavElement, args.MapName, smartGraphic, args.TouchPanel);
            }

            if (args.Name == "Source Selected")
            {
                int joinIndex = -1;
                
                ButtonJoinCollectionSerial("CurrentSourceSelectedBtn", (int)ESmartGraphicID.NA, 0, args.Source.FullName, args.TouchPanel);
                
                CurrentSource = args.Source;
                
                joinIndex = UserInteraction.PageNavigationElements["PCSourceSelection"].ElementNames.FindIndex(a => a.Contains(args.Source.FullName));
                if (joinIndex >= 0)
                {
                    ProcessPageNavigationExclusiveFeedback(args.Source.FullName, "PCSourceSelection", args.TouchPanel, ESmartGraphicID.PCSources);
                    ProcessPageNavigationExclusiveFeedback(args.Source.FullName, "PCSourceSelection", args.TouchPanel, ESmartGraphicID.VTCPresentationSources);
                    ButtonJoinCollectionFeedback("BlankSourceBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCPresentationDestBtn", true, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCCameraDestBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("AVBridgeDestBtn", false, args.TouchPanel);
                }

                joinIndex = UserInteraction.PageNavigationElements["MediaSourceSelection"].ElementNames.FindIndex(a => a.Contains(args.Source.FullName));
                if (joinIndex >= 0)
                {
                    ProcessPageNavigationExclusiveFeedback(args.Source.FullName, "MediaSourceSelection", args.TouchPanel, ESmartGraphicID.MediaSources);
                    ButtonJoinCollectionFeedback("BlankSourceBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCPresentationDestBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCCameraDestBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("AVBridgeDestBtn", false, args.TouchPanel);
                }

                joinIndex = UserInteraction.PageNavigationElements["CollaborationSourceSelection"].ElementNames.FindIndex(a => a.Contains(args.Source.FullName));
                if (joinIndex >= 0)
                {
                    ProcessPageNavigationExclusiveFeedback(args.Source.FullName, "CollaborationSourceSelection", args.TouchPanel, (int)ESmartGraphicID.NA);
                    ButtonJoinCollectionFeedback("BlankSourceBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCPresentationDestBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCCameraDestBtn", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("AVBridgeDestBtn", false, args.TouchPanel);
                    if ( ( args.Source.FullName == "Front Camera" ) || (args.Source.FullName == "Rear Camera" ) )
                    {
                        ButtonJoinCollectionEnable("VTCCameraDestBtn", true, args.TouchPanel);
                        ButtonJoinCollectionEnable("AVBridgeDestBtn", true, args.TouchPanel);
                    }
                }

                if ( args.Source.FullName == "")
                {
                    ButtonJoinCollectionFeedback("BlankSourceBtn", true, args.TouchPanel);
                    PageNavigationElementGroupFeedback("PCSourceSelection", false, args.TouchPanel);
                    PageNavigationElementGroupFeedback("MediaSourceSelection", false, args.TouchPanel);
                    PageNavigationElementGroupFeedback("CollaborationSourceSelection", false, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCPresentationDestBtn", true, args.TouchPanel);
                    ButtonJoinCollectionEnable("VTCCameraDestBtn", true, args.TouchPanel);
                    ButtonJoinCollectionEnable("AVBridgeDestBtn", true, args.TouchPanel);
                }

                SortManualDestinations();
            }

            if ( args.Name == "RouteValidated")
            {
                // The mixture of SerialJoins is crtical and that the PageNavigationElement for Destination is identified.  In this code, the key for the
                // Destination-based PageNavigationElement key value is DisplayDestinationSelection and VTCDestinationSelection
                // If those change, you must include SerialJoins that match the subreference page for the Display SmartGraphic
                // There are two serial join groups that must be used in conjunction with each other.
                // The first one is under SmartGraphicSerialJoins and is what populates the Display name
                // The second one is SerialJoins and that join is used to populate the source name once a route is made
                PopulateRouteDestinationFeedback(args.Destination.FullName, args.Source.FullName);


            }
        }


        

        public void OnPhysicalChangeEvent(object sender, PhysicalEventArgs args)
        {
            int currentClass = 0;
            switch (args.Category)
            {
                case "Physical":
                    
                    foreach (XpanelForSmartGraphics tp in MyXPanels)
                    {
                        tp.StringInput[0].StringValue = Physical.Physical.Building.Floors[0].Systems[0].Rooms[0].Name;
                    }
                    break;

                case "SystemStatusElements":
                    switch (args.Name)
                    {
                        case "DivibleStatus":
                            
                            break;
                    }
                    break;
                case "RoomStatusElements":
                    switch (args.Name)
                    {
                        case "RoomOnMode":
                            ProcessPageNavigationExclusiveFeedback("Classification", "PageNavigation", "All", ESmartGraphicID.NA);
                            ButtonJoinCollectionEnable("UnclassClassificationBtn", true, "All");
                            ButtonJoinCollectionEnable("SecretClassificationBtn", true, "All");
                            break;
                        case "RoomClassification":
                            ProcessPageNavigationExclusiveFeedback("RoomFunction", "PageNavigation", "All", ESmartGraphicID.NA);
                            CurrentClassification = args.CurrentStatus;
                            ButtonJoinCollectionEnable("RoomClassification", true, "All");
                            ButtonJoinCollectionAnalog("RoomClassification", (short)CurrentClassification, "All");
                            if (CurrentClassification == 1)
                            {
                                ButtonJoinCollectionEnable("AudioConferenceSelectionBtn", true, "All");
                                ButtonJoinCollectionVisibility("AudioConferenceBtn", true, "All");
                                ButtonJoinCollectionVisibility("CollaborationDestinationGroupBtn", false, "All");
                                ButtonJoinCollectionEnable("CollaborationDestinationGroupBtn", false, "All");
                            }
                            else
                            {
                                ButtonJoinCollectionEnable("AudioConferenceSelectionBtn", false, "All");
                                ButtonJoinCollectionVisibility("AudioConferenceBtn", false, "All");
                                ButtonJoinCollectionVisibility("CollaborationDestinationGroupBtn", true, "All");
                                ButtonJoinCollectionEnable("CollaborationDestinationGroupBtn", true, "All");
                            }
                            break;

                        
                        case "RoomMode":
                            ProcessPageNavigationExclusiveFeedback("Main", "PageNavigation", "All", ESmartGraphicID.NA);
                            
                            switch (args.CurrentStatus)
                            {
                                case 0: // Local Presentation
                                    ProcessPageNavigationExclusiveFeedback("Workstations", "SourceNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    ProcessPageNavigationExclusiveFeedback("Displays", "DestinationNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    break;
                                case 1: // VTC
                                    ProcessPageNavigationExclusiveFeedback("VTC", "SourceNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    ProcessPageNavigationExclusiveFeedback("VTC", "DestinationNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    break;
                                case 2: // WebConf
                                    ProcessPageNavigationExclusiveFeedback("Workstations", "SourceNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    ProcessPageNavigationExclusiveFeedback("Displays", "DestinationNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    break;
                                case 3: // AudioConf
                                    ProcessPageNavigationExclusiveFeedback("Workstations", "SourceNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    ProcessPageNavigationExclusiveFeedback("Displays", "DestinationNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    break;
                                case 4: // WatchTV
                                    ProcessPageNavigationExclusiveFeedback("CATV", "SourceNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    ProcessPageNavigationExclusiveFeedback("Displays", "DestinationNavigationFeedback", "All", (int)ESmartGraphicID.NA);
                                    break;
                            }

                            ButtonJoinCollectionEnable("SanitizeBtn", true, "All");
                            if ( currentClass == 1)
                            {
                                //ButtonJoinCollectionEnable("AudioConferenceBtn", true, "All");
                            } else
                            {
                                //ButtonJoinCollectionEnable("AudioConferenceBtn", false, "All");
                            }
                            break;
                        
                        case "RoomOffMode":
                            ProcessPageNavigationExclusiveFeedback("Idle", "PageNavigation", "All", ESmartGraphicID.NA);
                            break;
                        case "RoomSanitizeMode":
                            if (args.CurrentStatus == 1)
                            {
                                ButtonJoinCollectionFeedback("SanitizeBtn", true, "All");
                            }
                            if (args.CurrentStatus == 0)
                            {
                                ButtonJoinCollectionFeedback("SanitizeBtn", false, "All");
                            }
                            break;
                    }
                    break;
            }
        }

        public void OnDevicesControlledChangeEvent(object sender, DevicesControlledEventArgs args)
        {
            if (UserInteraction.DeviceFeedbackElements.ContainsKey(args.DeviceCategory))
            {
                switch (args.DeviceCategory)
                {
                    case "DSP":
                        foreach (DeviceFeedbackElement fbElement in UserInteraction.DeviceFeedbackElements[args.DeviceCategory])
                        {
                            if (fbElement.EventName == args.DspEventArgs.EventName)
                            {
                                switch (args.DspEventArgs.EventName)
                                {
                                    case "VolumeMute":
                                        if (args.DspEventArgs.VolumeMute == 1)
                                        {
                                            ButtonJoinCollectionFeedback(fbElement.Button, true, "All");
                                        }

                                        if (args.DspEventArgs.VolumeMute == 0)
                                        {
                                            ButtonJoinCollectionFeedback(fbElement.Button, false, "All");
                                            
                                        }
                                        break;
                                    case "Mic Mute":
                                        if (args.DspEventArgs.MicMute == 1)
                                        {
                                            ButtonJoinCollectionFeedback(fbElement.Button, true, "All");
                                        }

                                        if (args.DspEventArgs.MicMute == 0)
                                        {
                                            ButtonJoinCollectionFeedback(fbElement.Button, false, "All");
                                        }

                                        break;
                                    case "Volume":
                                        ButtonJoinCollectionAnalog(fbElement.Button, (short) ((args.DspEventArgs.Volume*20)/75), "All");
                                        break;
                                    case "StoredNumber":
                                        ButtonJoinCollectionFeedback("ATCKeyboard", false, CurrentTouchPanel);
                                        ButtonJoinCollectionFeedback(fbElement.Button, true, CurrentTouchPanel);
                                        switch (args.DspEventArgs.PresetSlot)
                                        {
                                            case 1:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset1", ESmartGraphicID.ATC, 0, GetKeyBoardJoinMap("ATCKeyboard").TypedString, "All");
                                                break;
                                            case 2:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset2", ESmartGraphicID.ATC, 0, GetKeyBoardJoinMap("ATCKeyboard").TypedString, "All");
                                                break;
                                            case 3:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset3", ESmartGraphicID.ATC, 0, GetKeyBoardJoinMap("ATCKeyboard").TypedString, "All");
                                                break;
                                            case 4:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset4", ESmartGraphicID.ATC, 0, GetKeyBoardJoinMap("ATCKeyboard").TypedString, "All");
                                                break;
                                            case 5:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset5", ESmartGraphicID.ATC, 0, GetKeyBoardJoinMap("ATCKeyboard").TypedString, "All");
                                                break;
                                            case 6:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset6", ESmartGraphicID.ATC, 0, GetKeyBoardJoinMap("ATCKeyboard").TypedString, "All");
                                                break;
                                        }
                                        
                                        break;
                                    case "StoredName":
                                        ButtonJoinCollectionFeedback("ATCSpeedDialKeyboard", false, CurrentTouchPanel);
                                        ButtonJoinCollectionFeedback(fbElement.Button, true, CurrentTouchPanel);
                                        switch (args.DspEventArgs.PresetSlot)
                                        {
                                            case 1:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset1", ESmartGraphicID.ATC, 1, GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 2:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset2", ESmartGraphicID.ATC, 1, GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 3:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset3", ESmartGraphicID.ATC, 1, GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 4:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset4", ESmartGraphicID.ATC, 1, GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 5:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset5", ESmartGraphicID.ATC, 1, GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 6:
                                                ButtonJoinCollectionSerial("ATCSpeedDialPreset6", ESmartGraphicID.ATC, 1, GetKeyBoardJoinMap("ATCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case "VTC":
                        foreach (DeviceFeedbackElement fbElement in UserInteraction.DeviceFeedbackElements[args.DeviceCategory])
                        {
                            if (fbElement.EventName == args.VtcEventArgs.EventName)
                            {
                                switch (args.VtcEventArgs.EventName)
                                {
                                    case "StoredNumber":
                                        ButtonJoinCollectionFeedback("VTCKeyboard", false, CurrentTouchPanel);
                                        ButtonJoinCollectionFeedback(fbElement.Button, true, CurrentTouchPanel);
                                        switch (args.VtcEventArgs.PresetSlot)
                                        {
                                            case 1:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset1", ESmartGraphicID.VTCSpeedDial, 0, GetKeyBoardJoinMap("VTCKeyboard").TypedString, "All");
                                                break;
                                            case 2:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset2", ESmartGraphicID.VTCSpeedDial, 0, GetKeyBoardJoinMap("VTCKeyboard").TypedString, "All");
                                                break;
                                            case 3:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset3", ESmartGraphicID.VTCSpeedDial, 0, GetKeyBoardJoinMap("VTCKeyboard").TypedString, "All");
                                                break;
                                            case 4:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset4", ESmartGraphicID.VTCSpeedDial, 0, GetKeyBoardJoinMap("VTCKeyboard").TypedString, "All");
                                                break;
                                            case 5:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset5", ESmartGraphicID.VTCSpeedDial, 0, GetKeyBoardJoinMap("VTCKeyboard").TypedString, "All");
                                                break;
                                            case 6:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset6", ESmartGraphicID.VTCSpeedDial, 0, GetKeyBoardJoinMap("VTCKeyboard").TypedString, "All");
                                                break;
                                        }

                                        break;
                                    case "StoredName":
                                        ButtonJoinCollectionFeedback("VTCSpeedDialKeyboard", false, CurrentTouchPanel);
                                        ButtonJoinCollectionFeedback(fbElement.Button, true, CurrentTouchPanel);
                                        switch (args.VtcEventArgs.PresetSlot)
                                        {
                                            case 1:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset1", ESmartGraphicID.VTCSpeedDial, 1, GetKeyBoardJoinMap("VTCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 2:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset2", ESmartGraphicID.VTCSpeedDial, 1, GetKeyBoardJoinMap("VTCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 3:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset3", ESmartGraphicID.VTCSpeedDial, 1, GetKeyBoardJoinMap("VTCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 4:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset4", ESmartGraphicID.VTCSpeedDial, 1, GetKeyBoardJoinMap("VTCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 5:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset5", ESmartGraphicID.VTCSpeedDial, 1, GetKeyBoardJoinMap("VTCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                            case 6:
                                                ButtonJoinCollectionSerial("VTCSpeedDialPreset6", ESmartGraphicID.VTCSpeedDial, 1, GetKeyBoardJoinMap("VTCSpeedDialKeyboard").TypedString, "All");
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case "Catv":
                        foreach (DeviceFeedbackElement fbElement in UserInteraction.DeviceFeedbackElements[args.DeviceCategory])
                        {
                            if (fbElement.EventName == args.CatvEventArgs.EventName)
                            {
                                switch (args.CatvEventArgs.EventName)
                                {
                                    case "ChannelPresets":
                                        for(int i=0; i<=4; i++)
                                        {
                                            String joinCollectionName = "CatvChannelPresetSlot" + (i+1);

                                            ButtonJoinCollectionSerial(joinCollectionName, ESmartGraphicID.NA, 0, "", "Admin");
                                        }
                                        foreach (KeyValuePair<int, String> pair in args.CatvEventArgs.ChannelPresets)
                                        {
                                            String joinCollectionName = "CatvChannelPresetSlot" + pair.Key;
                                            
                                            ButtonJoinCollectionSerial(joinCollectionName, ESmartGraphicID.NA, 0, pair.Value, "Admin");

                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
