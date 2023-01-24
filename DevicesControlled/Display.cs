using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisplayDevice;

namespace DevicesControlled
{
    public class Display
    {
        public String Manufacturer { get; set; }
        public IDisplay DisplayImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }

    }
}
