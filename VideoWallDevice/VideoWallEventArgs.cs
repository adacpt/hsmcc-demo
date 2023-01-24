using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoWallDevice
{
    public class VideoWallEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public int Layout { get; set; }
    }
}
