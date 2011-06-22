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

namespace PathfindingTest.Units
{
    public class Bowman : Unit
    {
        public LinkedList<Projectile> projectiles { get; set; }

        public Bowman(Player p, int x, int y)
            : base(p, x, y, 1f, 100f, 100f, 60)
        {
            this.baseDamage = baseDamage;

            this.type = Type.Ranged;
            this.projectiles = new LinkedList<Projectile>();

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/bowman");
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

                if (Game1.GetInstance().frames % 4 == 0 && unitToStalk != null)
                {
                    TryToSwing();
                }

                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles.ElementAt(i).Update(ks, ms);
                }
        }

        internal override void Draw(SpriteBatch sb)
        {
                sb.Draw(this.texture, new Vector2(x - (texture.Width / 2), y - (texture.Height / 2)), this.color);

                /*if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    this.DrawHealthBar(sb);
                }*/

                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles.ElementAt(i).Draw(sb);
                }
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
        public override void Swing()
        {
            if (this.fireCooldown < 0)
            {
                AggroEvent e = new AggroEvent(this, unitToStalk, true);
                unitToStalk.OnAggroRecieved(e);
                this.OnAggro(e);
                this.projectiles.AddLast(new Arrow(this, unitToStalk));
                this.fireCooldown = this.rateOfFire;
            }
        }
    }
}
