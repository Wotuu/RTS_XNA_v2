using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;

namespace PathfindingTest.Units.Stores
{
    class RangedStore : UnitStore
    {
        private readonly object lockObj = new object();

        public RangedStore(Player player)
        {
            this.player = player;
        }

        protected override Unit createUnit(Unit.Type type, int x, int y)
        {
            lock (lockObj)
            {
                switch (type)
                {
                    case Unit.Type.Ranged:
                        return new Bowman(player, x, y);
                    default: return null;
                }
            }
        }
    }
}
