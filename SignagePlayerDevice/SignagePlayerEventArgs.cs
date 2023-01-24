using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignagePlayerDevice
{
    public class SignagePlayerEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public String ElementChanged { get; set; }
        public int ZoneIndex { get; set; }
        public int Classification { get; set; }
        public int MicMute { get; set; }
        public int SpeakerMute { get; set; }
        public int VtcActive { get; set; }
        public String ZoneName { get; set; }
        public int ZoneHourOffset { get; set; }
        public int ZoneMinuteOffset { get; set; }
    }
}
