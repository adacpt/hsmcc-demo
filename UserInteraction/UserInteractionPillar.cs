using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class UserInteractionPillar
    {
        public List<TouchPanel> TouchPanels;
        public Keyboard Keyboard;
        public List<KeyboardJoinMap> KeyboardJoinMaps;
        public Dictionary<string, PageNavigationElement> PageNavigationElements;
        public Dictionary<string, ButtonJoinCollection> ButtonJoinCollections;
        public Dictionary<string, List<DeviceFeedbackElement>> DeviceFeedbackElements;

    }
}
