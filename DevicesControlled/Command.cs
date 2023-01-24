using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class Command
    {
        public String DeviceCategory { get; set; }
        public String DeviceName { get; set; }
        public String CommandName { get; set; }
        public DisplayCommandParameter DisplayCommandParameters { get; set; }
        public DspCommandParameter DspCommandParameters { get; set; }
        public VtcCommandParameter VtcCommandParameters { get; set; }
        public VideoWallCommandParameter VideoWallCommandParameters { get; set; }
        public CameraCommandParameter CameraCommandParameters { get; set; }
        public SignagePlayerCommandParameter SignagePlayerCommandParameters { get; set; }
        public PowerControllerCommandParameter PowerControllerCommandParameters { get; set; }
        public BluRayCommandParameter BluRayCommandParameters { get; set; }
        public CatvCommandParameter CatvCommandParameters { get; set; }

    }
}
