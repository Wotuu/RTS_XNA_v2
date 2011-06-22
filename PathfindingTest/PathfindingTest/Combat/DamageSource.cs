using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;

namespace PathfindingTest.Combat
{
    public abstract class DamageSource
    {
        public DamageEvent.DamageType type { get; set; }
        public float baseDamage { get; set; }
        public Unit target { get; set; }
    }
}
