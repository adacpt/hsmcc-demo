using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class MediaTransportPillar
    {
        public List<IODevice> IODevices;
        public List<TouchPanel> TouchPanels;
        public Dictionary<String, List<AutomaticRoute>> AutomaticRoutes;
        //public Dictionary<String, List<Route>> PresetRoutes;
    }
}
