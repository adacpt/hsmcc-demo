using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class AutomaticRoute
    {
        public List<String> Sources { get; set; }
        public List<String> Destinations { get; set; }

        public AutomaticRoute()
        {
            Sources = new List<String>();
            Destinations = new List<String>();
        }
    }
}
