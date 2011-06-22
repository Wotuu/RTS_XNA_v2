using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Combat;

namespace PathfindingTest.Units.Damage
{
    class MeleeSwing : DamageSource
    {
        public MeleeSwing(DamageEvent.DamageType type, int baseDamage)
        {
            this.type = type;
            this.baseDamage = baseDamage;
        }
    }
}
