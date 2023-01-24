using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class SignagePlayerCommandParameter
    {
        public String CurrentZoneParam{ get; set; }
        public int ClassificationParam { get; set; }
        public int MicMuteParam { get; set; }
        public int SpeakerMuteParam { get; set; }
        public int VtcActiveParam { get; set; }
        public String ZoneNameParam { get; set; }
        public int ZoneHourOffsetParam { get; set; }
        public int ZoneMinuteOffsetParam { get; set; }
    }
}
