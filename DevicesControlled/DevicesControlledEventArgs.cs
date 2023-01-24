using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DspDevice;
using VideoWallDevice;
using VtcDevice;
using MatrixSwitchDevice;
using CameraDevice;
using SignagePlayerDevice;
using DisplayDevice;
using PowerControllerDevice;
using BluRayDevice;
using CatvDevice;



namespace DevicesControlled
{
    public class DevicesControlledEventArgs : EventArgs
    {
        public String DeviceCategory { get; set; }
        public DisplayEventArgs DisplayEventArgs { get; set; }
        public DspEventArgs DspEventArgs { get; set; }
        public VideoWallEventArgs VideoWallEventArgs { get; set; }
        public VtcEventArgs VtcEventArgs { get; set; }
        public MatrixSwitchEventArgs MatrixSwitchEventArgs { get; set; }
        public CameraEventArgs CameraEventArgs { get; set; }
        public SignagePlayerEventArgs SignagePlayerEventArgs { get; set; }
        public PowerControllerEventArgs PowerControllerEventArgs { get; set; }
        public BluRayEventArgs BluRayEventArgs { get; set; }
        public CatvEventArgs CatvEventArgs { get; set; }

    }
}
