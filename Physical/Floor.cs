using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class Floor
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public List<System> Systems { get; set; }
        public int Deleted { get; set; }

        public Floor()
        {
            Id = Guid.NewGuid();
            Systems = new List<System>();
        }

    }
}
