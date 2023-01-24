using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DevicesControlled
{
    public class DevicesControlledPillar
    {
        public Dictionary<String, List<DeviceSelectionMap>> DeviceSelectionMap;
        public Dictionary<String, List<CommandParameterSelectionMap>> CommandParameterSelectionMap;
        public Dictionary<String, Display> Displays;
        public Dictionary<String, Dsp> DSPs;
        public Dictionary<String, MatrixSwitch> MatrixSwitches;
        public Dictionary<String, Vtc> VTCs;
        public Dictionary<String, VideoWall> VideoWalls;
        public Dictionary<String, Camera> Cameras;
        public Dictionary<String, SignagePlayer> SignagePlayers;
        public Dictionary<String, PowerController> PowerControllers;
        public Dictionary<String, BluRay> BluRays;
        public Dictionary<String, Catv> Catvs;
        public List<ButtonAction> ButtonActions;
        
        
        public DevicesControlledPillar()
        {
            DeviceSelectionMap = new Dictionary<String, List<DeviceSelectionMap>>();
            ButtonActions = new List<ButtonAction>();
            Displays = new Dictionary<String, Display>();
            DSPs = new Dictionary<String, Dsp>();
            MatrixSwitches = new Dictionary<String, MatrixSwitch>();
            VTCs = new Dictionary<String, Vtc>();
            VideoWalls = new Dictionary<String, VideoWall>();
            Cameras = new Dictionary<String, Camera>();
            SignagePlayers = new Dictionary<String, SignagePlayer>();
            PowerControllers = new Dictionary<String, PowerController>();
            BluRays = new Dictionary<String, BluRay>();
            Catvs = new Dictionary<String, Catv>();
        }
        

    }
}
