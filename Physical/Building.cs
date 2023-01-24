using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class Building
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        public List<Floor> Floors;
        public int Deleted { get; set; }

        
        public Building()
        {
            Id = Guid.NewGuid();
            Floors = new List<Floor>();
        }
        

    }
}
