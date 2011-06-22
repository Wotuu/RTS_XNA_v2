using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Combat
{
    interface Damageable
    {
        void OnDamage(DamageEvent e);
    }
}
