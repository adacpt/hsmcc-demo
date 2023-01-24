using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MediaTransport
{
    public class Source
    {
        public Guid GUID { get; set; }
        public String FullName { get; set; }
        public String ShortName { get; set; }
        public int SourceNodeIndex { get; set; }
        public ESourceDeviceCategory SourceCategory { get; set; }
        public ESignalType SignalType { get; set; }
        public String State { get; set; }
        public int Classification { get; set; }
        public int Deleted { get; set; }

    }
}
