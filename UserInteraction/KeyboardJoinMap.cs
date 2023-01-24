using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class KeyboardJoinMap
    {
        public String Name { get; set; }
        public int SmartGraphic { get; set; }
        public List<ushort> ActivationPressJoins { get; set; }
        public String TypedString { get; set; }

        public KeyboardJoinMap()
        {
            ActivationPressJoins = new List<ushort>();
        }
    }
}
