using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;

namespace PathfindingTest.Combat
{
    public class AggroEvent
    {
        public Unit from { get; set; }
        public Damageable to { get; set; }
        public Boolean received { get; set; }


        public AggroEvent(Unit from, Damageable to, Boolean received)
        {
            this.from = from;
            this.to = to;
            this.received = received;
        }
    }
}
