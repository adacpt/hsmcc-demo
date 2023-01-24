using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtcDevice;

namespace DevicesControlled
{
    public class Vtc
    {
        public String Manufacturer { get; set; }
        public IVtc VtcImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }
    }
}
