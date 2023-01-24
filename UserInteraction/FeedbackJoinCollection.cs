using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class FeedbackJoinCollection
    {
        public List<ushort> FeedbackJoins { get; set; }

        public FeedbackJoinCollection()
        {
            FeedbackJoins = new List<ushort>();
        }
    }
}
