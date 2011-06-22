using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units.Projectiles;
using SocketLibrary.Protocol;

namespace PathfindingTest.Multiplayer.Data
{
    public class ProjectileMultiplayerData: MultiplayerData
    {
        public Projectile projectile { get; set; }

        public ProjectileMultiplayerData(Projectile projectile, Boolean isLocal)
            : base(isLocal)
        {
            this.projectile = projectile;
        }



        public override int GetObjectType()
        {
            if (this.projectile is Arrow) return UnitHeaders.PROJECTILE_ARROW;
            return -1;
        }
    }
}
