using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro.UI;

namespace UserInteraction
{
    public class TouchPanel
    {
        public Guid Id { get; set; }
        public int IPID { get; set; }
        public String Name { get; set; }
        public static int Deleted { get; set; }

        public TouchPanel()
        {
            Id = Guid.NewGuid();
        }
    }
}
