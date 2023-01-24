using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerControllerDevice;

namespace DevicesControlled
{
    public class PowerController
    {
        public String Manufacturer { get; set; }
        public IPowerController PowerControllerImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }

    }
}
