using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class VtcCommandParameter
    {
        public String MakeCallParam { get; set; }
        public String HangupCallParam { get; set; }
        public int PresentationParam { get; set; }
        public int SelfviewParam { get; set; }
        public int DtmfParam { get; set; }
        public int PresetSlotParam { get; set; }
        public String NumberParam { get; set; }
        public String NameParam { get; set; }
    }
}
