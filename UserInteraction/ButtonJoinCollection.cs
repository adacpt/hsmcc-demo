using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class ButtonJoinCollection
    {
        public int SmartGraphic { get; set; }
        public ushort PressDigitalJoin { get; set; }
        public ushort FeedbackDigitalJoin { get; set; }
        public ushort EnableDigitalJoin { get; set; }
        public ushort VisibilityDigitalJoin { get; set; }
        public ushort AnalogJoin { get; set; }
        public List<ushort> SerialJoins { get; set; }
        public int CurrentStatus { get; set; }

        public ButtonJoinCollection()
        {
            SerialJoins = new List<ushort>();
        }
    }
}
