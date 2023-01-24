using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DspDevice;

namespace DevicesControlled
{
    public class Dsp
    {
        public String Manufacturer { get; set; }
        public IDsp DspImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }

    }
}
