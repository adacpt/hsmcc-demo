using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class SourceButtonInputMap
    {
        public EDeviceCategory DeviceCategory { get; set; }
        public String MapType { get; set; }
        public ManualRoutingButtonMap VariableSourceButtonMap { get; set; }
        public ManualRoutingButtonMap StaticTouchPanelSourceButtonMap { get; set; }
        public ManualRoutingButtonMap StaticSmartGraphicSourceButtonMap { get; set; }


        public SourceButtonInputMap()
        {

        }
    }
}
