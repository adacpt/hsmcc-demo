using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class Room
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public int Deleted { get; set; }

        public Room()
        {
            Id = Guid.NewGuid();
        }
    }
}
