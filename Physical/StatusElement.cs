using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical
{
    public class StatusElement
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public int SmartGraphicId { get; set; }
        public List<ushort> ActionJoins { get; set; }
        public List<String> OptionNames { get; set; }
        public List<int> RelativeValues { get; set; }
        public int CurrentStatus { get; set; }

        public StatusElement()
        {
            ActionJoins = new List<ushort>();
            OptionNames = new List<String>();
            RelativeValues = new List<int>();
        }

    }
}
