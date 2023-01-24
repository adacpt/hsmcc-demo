using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignagePlayerDevice;

namespace DevicesControlled
{
    public class SignagePlayer
    {
        public String Manufacturer { get; set; }
        public ISignagePlayer SignagePlayerImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }
        public int TimeZones { get; set; }

    }
}
