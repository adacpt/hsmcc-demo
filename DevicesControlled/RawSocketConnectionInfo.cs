using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class RawSocketConnectionInfo
    {
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public int DeviceId { get; set; }
        public int Port { get; set; }
    }
}
