using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class SerialConnectionInfo
    {
        public string HostName { get; set; }
        public string BaudRate { get; set; }
        public int DeviceId { get; set; }
        public int ComPort { get; set; }
    }
}
