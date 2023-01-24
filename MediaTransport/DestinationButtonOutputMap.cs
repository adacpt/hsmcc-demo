using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class DestinationButtonOutputMap
    {
        public EDeviceCategory DeviceCategory { get; set; }
        public String MapType { get; set; }
        public ManualRoutingButtonMap VariableDestinationButtonMap { get; set; }
        public ManualRoutingButtonMap StaticTouchPanelDestinationButtonMap { get; set; }
        public ManualRoutingButtonMap StaticSmartGraphicDestinationButtonMap { get; set; }

        public DestinationButtonOutputMap()
        {
        }
    }
}
