using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspDevice
{
    public class DspEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public int Volume { get; set; }
        public int VolumeMute { get; set; }
        public int MicMute { get; set; }
        public int PresetSlot { get; set; }
        public String PresetNumber { get; set; }

    }
}
