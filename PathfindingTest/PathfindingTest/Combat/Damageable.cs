using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Combat
{
    public interface Damageable
    {
        void OnDamage(DamageEvent e);
    }
}
