using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoWallDevice;

namespace DevicesControlled
{
    public class VideoWall
    {
        public String Manufacturer { get; set; }
        public IVideoWall VideoWallImpl { get; set; }
        public EConnectionType ConnectionType { get; set; }
        public RawSocketConnectionInfo RawSocketConnectionInfo { get; set; }
        public SerialConnectionInfo SerialConnectionInfo { get; set; }

    }
}
