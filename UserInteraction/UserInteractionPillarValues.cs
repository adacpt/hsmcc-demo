using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;

namespace UserInteraction
{
    public static class UserInteraction
    {
        public static event EventHandler<UserInteractionEventArgs> UserInteractionChangeEvent;
        public static List<TouchPanel> TouchPanels = new List<TouchPanel>();
        public static Keyboard Keyboard = new Keyboard();
        public static List<KeyboardJoinMap> KeyboardJoinMaps = new List<KeyboardJoinMap>();
        public static Dictionary<string, PageNavigationElement> PageNavigationElements = new Dictionary<string, PageNavigationElement>();
        public static Dictionary<string, ButtonJoinCollection> ButtonJoinCollections = new Dictionary<string, ButtonJoinCollection>();
        public static Dictionary<string, List<DeviceFeedbackElement>> DeviceFeedbackElements = new Dictionary<string, List<DeviceFeedbackElement>>();

        public static void OnUserInteractionChangeEvent(UserInteractionEventArgs e)
        {
            CrestronConsole.PrintLine("Invoking UserInteractionChangeEvent");
            UserInteractionChangeEvent?.Invoke(null, e);
        }
    }
}
