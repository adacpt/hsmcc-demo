using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class UserInteractionEventArgs : EventArgs
    {
        public String TouchPanel { get; set; }
        public String EventType { get; set; }
        public UserInteractionStatusEventArgs StatusArgs { get; set; }

        public UserInteractionEventArgs()
        {
            StatusArgs = new UserInteractionStatusEventArgs();
        }

    }
}
