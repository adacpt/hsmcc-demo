using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerControllerDevice
{
    public class PowerControllerEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public int Outlet { get; set; }
        public int PowerState { get; set; }
    }
}
