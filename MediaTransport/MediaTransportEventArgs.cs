using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransport
{
    public class MediaTransportEventArgs : EventArgs
    {
        public String Category { get; set; }
        public String Name { get; set; }
        public String MapName { get; set; }
        public String TouchPanel{ get; set; }
        public Source Source { get; set; }
        public Destination Destination { get; set; }
    }
}
