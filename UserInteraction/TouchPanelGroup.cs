using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class TouchPanelGroup
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public Dictionary<String, List<TouchPanel>> TouchPanels;
        public static int Deleted { get; set; }

        public TouchPanelGroup()
        {
            TouchPanels = new Dictionary<string, List<TouchPanel>>();
        }

    }
}
