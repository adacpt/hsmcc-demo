using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInteraction
{
    public class Keyboard
    {
        public List<ushort> KeyBoardJoins { get; set; }
        public List<String> KeyBoardCharacters { get; set; }
        public ushort BackSpaceJoin { get; set; }
        public Keyboard()
        {
            KeyBoardJoins = new List<ushort>();
            KeyBoardCharacters = new List<String>();
        }
    }
}
