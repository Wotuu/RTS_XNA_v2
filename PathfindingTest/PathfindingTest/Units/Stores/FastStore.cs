using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using PathfindingTest.Units.Fast;

namespace PathfindingTest.Units.Stores
{
    class FastStore : UnitStore
    {

        public FastStore(Player player)
        {
            this.player = player;
        }

        protected override Unit createUnit(Unit.Type type, int x, int y)
        {
            switch (type)
            {
                case Unit.Type.Fast: 
                    return new Horseman(player, x, y);
                default: break;
            }
            return null;
        }
    }
}
