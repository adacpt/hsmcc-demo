using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAVETesting
{
    
    public class Building
    {
        public int Guid { get; set; }
        public string Name { get; set; }
        public int Deleted { get; set; }

    }
    public class Phys
    {
        public Building Building { get; set; }
    }
    public class RootObject
    {
        public Phys Phys { get; set; }
    }
}
