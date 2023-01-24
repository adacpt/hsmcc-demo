using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatvDevice
{
    public class CatvEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public String Channel { get; set; }
        public Dictionary<int, String> ChannelPresets { get; set; }

        public CatvEventArgs()
        {
            ChannelPresets = new Dictionary<int, string>();
        }
    }
}
