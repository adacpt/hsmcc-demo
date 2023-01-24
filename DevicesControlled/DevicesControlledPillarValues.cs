using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;


namespace DevicesControlled
{
    public static class DevicesControlled
    {
        public static event EventHandler<DevicesControlledEventArgs> DevicesControlledChangeEvent;

        public static Dictionary<String, List<DeviceSelectionMap>> DeviceSelectionMap = new Dictionary<String, List<DeviceSelectionMap>>();
        public static Dictionary<String, List<CommandParameterSelectionMap>> CommandParameterSelectionMap = new Dictionary<String, List<CommandParameterSelectionMap>>();
        public static Dictionary<String, Display> Displays = new Dictionary<String, Display>();
        public static Dictionary<String, Dsp> DSPs = new Dictionary<String, Dsp>();
        public static Dictionary<String, MatrixSwitch> MatrixSwitches = new Dictionary<String, MatrixSwitch>();
        public static Dictionary<String, Vtc> VTCs = new Dictionary<String, Vtc>();
        public static Dictionary<String, VideoWall> VideoWalls = new Dictionary<string, VideoWall>();
        public static Dictionary<String, Camera> Cameras = new Dictionary<string, Camera>();
        public static Dictionary<String, SignagePlayer> SignagePlayers = new Dictionary<string, SignagePlayer>();
        public static Dictionary<String, PowerController> PowerControllers = new Dictionary<string, PowerController>();
        public static Dictionary<String, BluRay> BluRays = new Dictionary<string, BluRay>();
        public static Dictionary<String, Catv> Catvs = new Dictionary<string, Catv>();
        public static List<ButtonAction> ButtonActions = new List<ButtonAction>();
        public static void OnDevicesControlledChangeEvent(DevicesControlledEventArgs e)
        {
            DevicesControlledChangeEvent?.Invoke(null, e);
        }
    }
}
