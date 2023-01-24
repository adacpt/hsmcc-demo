using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtcDevice
{
    public class VtcEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public Dictionary<String, int> CallStatus { get; set; }
        public int PresentationStatus { get; set; }
        public int DTMFStatus { get; set; }
        public int PresetSlot { get; set; }
        public String PresetNumber { get; set; }

        public VtcEventArgs()
        {
            CallStatus = new Dictionary<string, int>();
        }

    }

}
