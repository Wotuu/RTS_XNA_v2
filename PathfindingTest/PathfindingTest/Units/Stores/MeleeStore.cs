using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units.Stores;
using PathfindingTest.Players;
using PathfindingTest.Units.Melee;

namespace PathfindingTest.Units
{
    public class MeleeStore : UnitStore
    {

        public MeleeStore(Player player)
        {
            this.player = player;
        }

        protected override Unit createUnit(Unit.Type type, int x, int y)
        {
            switch (type)
            {
                case Unit.Type.Engineer:
                    return new Engineer(player, x, y);
                case Unit.Type.Melee:
                    return new Swordman(player, x, y);
                default: Console.WriteLine("Null Returned"); return null;
            }
        }
    }
}
