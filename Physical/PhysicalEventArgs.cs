using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class PhysicalEventArgs : EventArgs
    {
        public String Category { get; set; }
        public String Name { get; set; }
        public int CurrentStatus { get; set; }

    }
}
