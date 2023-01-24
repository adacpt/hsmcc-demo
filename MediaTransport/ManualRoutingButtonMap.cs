using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class ManualRoutingButtonMap
    {
        public List<int> Joins { get; set; }
        public List<String> SmartGraphicJoins { get; set; }
        public List<String> Sources { get; set; }
        public List<String> Destinations { get; set; }

        public ManualRoutingButtonMap()
        {
            Joins = new List<int>();
            SmartGraphicJoins = new List<String>();
            Sources = new List<String>();
            Destinations = new List<String>();
        }
    }
}
