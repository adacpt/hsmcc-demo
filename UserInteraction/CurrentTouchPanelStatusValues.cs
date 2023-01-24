using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;

namespace UserInteraction
{
    public class CurrentTouchPanelStatusValues
    {
        private int currentTuner;
        public CurrentTouchPanelStatusValues()
        {
            currentTuner = 1;
        }

        public int CurrentTuner
        {
            get { return currentTuner; }
            set
            {
                // need a try catch in case value isn't valid
                currentTuner = value;
                
                CrestronConsole.PrintLine("Setting CurrentTuner");
                UserInteractionStatusEventArgs statusArgs = new UserInteractionStatusEventArgs()
                {
                    Category = "Status",
                    Name = "Tuner"
                };

                UserInteraction.OnUserInteractionChangeEvent(new UserInteractionEventArgs()
                {
                    TouchPanel = "Admin",
                    EventType = "Status",
                    StatusArgs = statusArgs
                });
            }
        }

    }
}
