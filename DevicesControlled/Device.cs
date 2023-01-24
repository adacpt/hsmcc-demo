using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class Device
    {
        public String DeviceCategory { get; set; }
        public String Manufacturer { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }
    }
}
