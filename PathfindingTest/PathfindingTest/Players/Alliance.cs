using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLists.Lists;

namespace PathfindingTest.Players
{
    public class Alliance
    {
        public CustomArrayList<Player> members { get; set; }

        public Alliance()
        {
            members = new CustomArrayList<Player>();
        }
    }
}
