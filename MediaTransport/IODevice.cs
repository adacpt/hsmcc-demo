using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class IODevice
    {
        public Guid GUID { get; set; }
        public List<String> Rooms { get; set; }
        public List<EDeviceCategory> DeviceCategories { get; set; }
        public String Name { get; set; }
        public List<Source> Sources { get; set; }
        public List<Destination> Destinations { get; set; }
        public String State { get; set; }
        public int Classification { get; set; }
        public int Deleted { get; set; }

        public IODevice()
        {
            Rooms = new List<String>();
            Sources = new List<Source>();
            Destinations = new List<Destination>();
        }
    }
}
