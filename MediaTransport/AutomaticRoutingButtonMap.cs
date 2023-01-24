using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class AutomaticRoutingButtonMap
    {
        public int SGID { get; set; }
        public int TPID { get; set; }
        public List<int> Joins { get; set; }
        public List<String> AutomaticRoutes { get; set; }

        public AutomaticRoutingButtonMap()
        {
            Joins = new List<int>();
            AutomaticRoutes = new List<String>();
        }
    }
}
