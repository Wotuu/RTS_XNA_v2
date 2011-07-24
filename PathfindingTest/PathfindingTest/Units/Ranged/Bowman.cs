using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Units.Projectiles;
using PathfindingTest.Combat;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;
using PathfindingTest.Pathfinding;
using PathfindingTest.Units.Damage;

namespace PathfindingTest.Units
{
    public class Bowman : Unit
    {
        private ArrowManager arrowManager;

        public Bowman(Player p, int x, int y)
            : base(p, x, y, 1f, 100f, 100f, 60)
        {
            this.baseDamage = baseDamage;

            this.type = Type.Ranged;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/bowman");
            this.halfTextureWidth = this.texture.Width / 2;
            this.halfTextureHeight = this.texture.Height / 2;
            // Console.Out.WriteLine("Constructed a bowman @ " + this.GetLocation() + " (" + x + ", " + y + ")");
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
                UpdateMovement();
                AttemptReload();
                if (Game1.GetInstance().frames % 15 == 0 && unitToDefend == null)
                {
                    UpdateAttack();
                }
                else if (Game1.GetInstance().frames % 15 == 0 && unitToDefend != null)
                {
                    UpdateDefense();
                }

                if (Game1.GetInstance().frames % 4 == 0 && (unitToStalk != null || buildingToDestroy != null))
                {
                    TryToSwing();
                }
        }

        internal override void Draw(SpriteBatch sb)
        {
                sb.Draw(this.texture, new Rectangle((int)this.x - this.halfTextureWidth, (int)this.y - this.halfTextureHeight, 
                    texture.Width, texture.Height), null, this.color, 0f, 
                    Vector2.Zero, SpriteEffects.None, this.z);

                /*if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    this.DrawHealthBar(sb);
                }*/
        }

        public override void OnAggroRecieved(AggroEvent e)
        {
            if (unitToStalk == null)
            {
                unitToStalk = e.from;
            }
            if (friendliesProtectingMe.Count > 0)
            {
                foreach (Unit unit in friendliesProtectingMe)
                {
                    unit.OnAggroRecieved(e);
                }
            }
            // Console.Out.WriteLine("Recieved aggro from something! D=");
        }

        public override void OnAggro(AggroEvent e)
        {
            // Console.Out.WriteLine("Aggroing something, *grins*");
        }

        /// <summary>
        /// Reduces the fire cooldown of this unit.
        /// </summary>
        public void AttemptReload()
        {
            this.fireCooldown--;
        }


        /// <summary>
        /// Attempt to fire the weapon!
        /// </summary>
        public override void Swing(Damageable target)
        {
            if (this.fireCooldown < 0)
            {
                if (target is Unit)
                {
                    AggroEvent e = new AggroEvent(this, target, true);
                    ((Unit)target).OnAggroRecieved(e);
                    this.OnAggro(e);
                }
                Arrow newArrow = new Arrow(this, target);
                this.player.arrowManager.AddProjectile(newArrow);
                this.fireCooldown = this.rateOfFire;
            }
        }
    }
}
