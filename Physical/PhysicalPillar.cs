using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class PhysicalPillar
    {
        
        public List<Classification> ClassificationList;
        public Building Building { get; set; }
        public List<StatusElement> SystemStatusElements;
        public List<StatusElement> RoomStatusElements;
    }
}
