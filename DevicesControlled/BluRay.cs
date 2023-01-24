using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluRayDevice;

namespace DevicesControlled
{
    public class BluRay
    {
        public String Manufacturer { get; set; }
        public IBluRay BluRayImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }

    }
}
