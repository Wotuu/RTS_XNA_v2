using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using PathfindingTest.Units.Projectiles;

namespace PathfindingTest.Combat
{
    public class DamageEvent
    {
        public DamageType type { get; set; }

        public DamageSource by { get; set; }
        public Unit target { get; set; }
        public Unit source { get; set; }
        public float damageDone { get; set; }
        public float usedModifier { get; set; }

        private float[,] modifiers = {
            { 1,    2, 0.5f, 1    },
            { 0.5f, 1,    1, 2    },
            { 2,    1,    1, 0.5f },
            { 1, 0.5f,    2, 1    }
        };

        private float GetModifier()
        {
            int x = 0;
            if (by.type == DamageType.HeavyMelee) x = 1;
            else if (by.type == DamageType.Ranged) x = 2;
            else if (by.type == DamageType.Fast) x = 3;

            if (target.type == Unit.Type.Ranged) return modifiers[x, 2];
            else if (target.type == Unit.Type.Engineer) return modifiers[x, 0];
            else return 1f;
        }

        public enum DamageType
        {
            Melee,
            HeavyMelee,
            Fast,
            Ranged
        }

        public DamageEvent(DamageSource by, Unit target, Unit source)
        {
            this.source = source;
            this.by = by;
            this.target = target;
            this.usedModifier = this.GetModifier();
            this.damageDone = by.baseDamage * this.usedModifier;
        }
    }
}
