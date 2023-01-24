using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class CameraCommandParameter
    {
        public String CameraNameParam { get; set; }
        public int SpeedParam { get; set; }
        public int ActionParam { get; set; }
        public int PanParam { get; set; }
        public int TiltParam { get; set; }
        public int ZoomParam { get; set; }
        public String StopParam { get; set; }
        public int RecallPresetParam { get; set; }
        public int SavePresetParam { get; set; }
        public int PowerParam { get; set; }
        public String HomePositionParam { get; set; }
    }
}
