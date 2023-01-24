using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class System
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public int Identifier { get; set; }
        public List<Room> Rooms { get; set; }
        public int Deleted { get; set; }

        public System()
        {
            Id = Guid.NewGuid();
            Rooms = new List<Room>();
        }
    }
}
