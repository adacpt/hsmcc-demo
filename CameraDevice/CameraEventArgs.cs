using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraDevice
{
    public class CameraEventArgs : EventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public int PresetRecalled { get; set; }
        public int PresetStored { get; set; }
    }
}
