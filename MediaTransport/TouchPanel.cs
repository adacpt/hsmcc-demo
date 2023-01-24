using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class TouchPanel
    {
        public String Name { get; set; }
        public Dictionary<String, List<IODevice>> SortedSourceIODevices { get; set; }
        public Dictionary<String, List<IODevice>> SortedDestinationIODevices { get; set; }
        public Dictionary<String, SourceButtonInputMap> SourceButtonInputMap {get; set; }
        public Dictionary<String, DestinationButtonOutputMap> DestinationButtonOutputMap { get; set; }
        public AutomaticRoutingButtonMap AutomaticRoutingButtonMap { get; set; }

        public TouchPanel()
        {
            SortedSourceIODevices = new Dictionary<string, List<IODevice>>();
            SortedDestinationIODevices = new Dictionary<string, List<IODevice>>();
        }

    }
}
