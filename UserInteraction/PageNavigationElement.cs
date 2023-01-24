using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class PageNavigationElement
    {
        public String PopulationType { get; set; }
        public String FeedbackType { get; set; }
        public int SmartGraphicId { get; set; }
        public List<ushort> PressJoins { get; set; }
        public List<String> SmartGraphicPressJoins { get; set; }
        public List<List<ushort>> FeedbackJoins { get; set; }
        public List<List<String>> SmartGraphicFeedbackJoins { get; set; }
        public List<ushort> VisibilityJoins { get; set; }
        public List<String> SmartGraphicVisibilityJoins { get; set; }
        public List<ushort> EnableJoins { get; set; }
        public List<String> SmartGraphicEnableJoins { get; set; }
        public List<ushort> SerialJoins { get; set; }
        public List<String> SmartGraphicSerialJoins { get; set; }
        public List<ushort> AnalogJoins { get; set; }
        public List<String> SmartGraphicAnalogJoins { get; set; }
        public List<String> ElementNames { get; set; }
        public int CurrentSelection { get; set; }

        public PageNavigationElement()
        {
            PressJoins = new List<ushort>();
            FeedbackJoins = new List<List<ushort>>();
            VisibilityJoins = new List<ushort>();
            EnableJoins = new List<ushort>();
            SerialJoins = new List<ushort>();
            AnalogJoins = new List<ushort>();
            SmartGraphicPressJoins = new List<String>();
            SmartGraphicFeedbackJoins = new List<List<String>>();
            SmartGraphicVisibilityJoins = new List<String>();
            SmartGraphicEnableJoins = new List<String>();
            SmartGraphicSerialJoins = new List<String>();
            SmartGraphicAnalogJoins = new List<String>();
            ElementNames = new List<String>();
        }
    }
}
