using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatvDevice;

namespace DevicesControlled
{
    public class Catv
    {
        public String Manufacturer { get; set; }
        public ICatv CatvImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }
    }
}
