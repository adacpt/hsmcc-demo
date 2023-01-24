using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayDevice
{
    public class DisplayEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public int Power { get; set; }
        public int Input { get; set; }
        public int Volume { get; set; }
        public int VolumeMute { get; set; }
        public int MultiView { get; set; }

    }
}