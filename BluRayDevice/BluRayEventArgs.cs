using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluRayDevice
{
    public class BluRayEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public int Play { get; set; }
        public int Stop { get; set; }
        public int Pause { get; set; }
    }
}
