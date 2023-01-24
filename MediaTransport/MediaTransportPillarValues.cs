using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;

namespace MediaTransport
{
    public static class MediaTransport
    {
        public static event EventHandler<MediaTransportEventArgs> MediaTransportChangeEvent;
        public static List<IODevice> IODevices = new List<IODevice>();
        public static List<TouchPanel> TouchPanels = new List<TouchPanel>();
        public static Dictionary<String, List<AutomaticRoute>> AutomaticRoutes = new Dictionary<string, List<AutomaticRoute>>();
        public static Dictionary<String, List<Route>> PresetRoutes = new Dictionary<string, List<Route>>();
        public static Dictionary<String, Route> CurrentRoutes = new Dictionary<String, Route>();
        public static Dictionary<String, Route> RecallRoutes = new Dictionary<String, Route>();

        public static void OnMediaTransportChangeEvent(MediaTransportEventArgs e)
        {
            CrestronConsole.PrintLine("Invoking MediaTransportChangeEvent");
            MediaTransportChangeEvent?.Invoke(null, e);
        }
    }
}
