using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Pathfinding;
using PathfindingTest.Combat;

namespace PathfindingTest.Units.Projectiles
{
    public class Arrow : Projectile
    {

        private Texture2D collisionPointTex { get; set; }

        public Arrow(Unit parent, Damageable target)
            : base(parent, target, DamageEvent.DamageType.Ranged, 4.0f, 300, 15)
        {
            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/Projectiles/wooden_arrow_scale");
            this.collisionPointTex = Game1.GetInstance().Content.Load<Texture2D>("Misc/solid");
        }

        internal override void Draw(SpriteBatch sb)
        {
            Rectangle rect = this.GetDrawRectangle();
            Game1 game = Game1.GetInstance();
            if (!game.IsOnScreen(rect)) return;

            sb.Draw(this.texture, rect, null, Color.White,
                (float)(Util.GetHypoteneuseAngleRad(this.GetLocation(), this.waypoint) + (90 * (Math.PI / 180))),
                new Vector2((this.texture.Width / 2), (this.texture.Height / 2)), SpriteEffects.None, 0);

            Point p = Util.GetPointOnCircle(this.GetLocation(), this.texture.Height / 2,
                        (float)(Util.GetHypoteneuseAngleDegrees(this.GetLocation(), this.waypoint)));
            sb.Draw(this.collisionPointTex, new Rectangle(
                (int)(p.X - game.drawOffset.X), 
                (int)(p.Y - game.drawOffset.Y), 2, 2), Color.Red);


            sb.Draw(this.collisionPointTex, new Rectangle(
                (int)(p.X - game.drawOffset.X),
                (int)(p.Y - game.drawOffset.Y), 2, 2), Color.Green);
        }
    }
}
