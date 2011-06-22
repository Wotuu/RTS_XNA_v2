using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;

namespace PathfindingTest.Units.Stores
{
    public abstract class UnitStore
    {

        public Player player;
        
        /// <summary>
        /// Should not be used, use UnitStore(Player p) instead.
        /// @deprecated
        /// @since 29-5-2011
        /// </summary>
        public UnitStore() {}

        public UnitStore(Player p)
        {
            this.player = p;
        }

        public Unit getUnit(Unit.Type type, int x, int y)
        {
            return createUnit(type, x, y);
        }

        protected abstract Unit createUnit(Unit.Type type, int x, int y);
    }
}
