using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlled
{
    public class ButtonAction
    {
        public List<String> TouchPanels { get; set; }
        public int SmartGraphicId { get; set; }
        public ushort Join { get; set; }
        public String ButtonState { get; set; }
        public List<Command> Commands { get; set; }

        public ButtonAction()
        {
            TouchPanels = new List<String>();
            Commands = new List<Command>();
        }
    }
}
