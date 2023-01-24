using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class DeviceFeedbackElement
    {
        public String DeviceName { get; set; }
        public String EventName { get; set; }
        public EButtonType FeedbackType { get; set; }
        public String Button { get; set; }
    }
}
