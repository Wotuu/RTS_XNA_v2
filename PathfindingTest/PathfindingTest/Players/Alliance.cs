using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Players
{
    public class Alliance
    {
        public LinkedList<Player> members { get; set; }

        public Alliance()
        {
            members = new LinkedList<Player>();
        }
    }
}
