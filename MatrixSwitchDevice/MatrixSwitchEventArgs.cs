using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixSwitchDevice
{
    public class MatrixSwitchEventArgs
    {
        public String EventName { get; set; }
        public String DeviceName { get; set; }
        public Dictionary<int, int> Route { get; set; }
    }
}
