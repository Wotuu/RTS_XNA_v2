using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Units
{
    public class ProductionUnit
    {        

        public float currentHealth { get; set; }
        public float maxHealth { get; set; }
        public double productionDuration { get; set; }
        public double productionProgress { get; set; }
        public Unit.Type type { get; set; }

        public ProductionUnit(float maxHealth, double productionDuration, Unit.Type type)
        {
            this.currentHealth = 0;
            this.maxHealth = maxHealth;
            this.productionDuration = productionDuration;
            this.productionProgress = 0;
            this.type = type;
        }
    }
}
