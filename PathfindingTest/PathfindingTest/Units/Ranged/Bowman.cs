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
            : base(p, x, y, 1f, 100f, 100f, 120)
        {
            this.baseDamage = baseDamage;

            this.type = Type.Ranged;
            this.visionRange = (float)VisionRange.Bowman;

            this.texture = TextureManager.GetInstance().GetTexture(this.type);
            this.halfTextureWidth = this.texture.Width / 2;
            this.halfTextureHeight = this.texture.Height / 2;
            // Console.Out.WriteLine("Constructed a bowman @ " + this.GetLocation() + " (" + x + ", " + y + ")");
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            UpdateMovement();
            AttemptReload();
            switch (this.job)
            {
                case Job.Moving: break;
                case Job.Attacking: 
                    if (Game1.GetInstance().frames % 15 == 0 && unitToDefend == null)
                    {
                        UpdateAttack();
                    }

                    if (Game1.GetInstance().frames % 15 == 0 && unitToStalk == null)
                    {
                        if (this.waypoints.Count() < 1)
                        {
                            this.waypoints.Clear();
                            MoveToQueue(assaultPoint);
                        }
                    }
                    else
                    {

                        if (Game1.GetInstance().frames % 4 == 0 && (unitToStalk != null || buildingToDestroy != null))
                        {
                            TryToSwing();
                        }
                    }

                    break;
                default:
                    {
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
                    break;
            }
        }

        internal override void Draw(SpriteBatch sb)
        {
            Rectangle rect = this.GetDrawRectangle();
            rect.X += texture.Width / 2;
            rect.Y += texture.Height / 2;
            if (!Game1.GetInstance().IsOnScreen(rect)) return;

            if (this.waypoints.Count() > 0 && unitToStalk == null)
            {
                rotation = (float)(Util.GetHypoteneuseAngleRad(this.GetLocation(), this.waypoints.GetFirst()) + (90 * (Math.PI / 180)));                

                if (rotation != rotation)
                {
                    rotation = previousRotation;
                }
            }
            else if (unitToStalk != null)
            {
                rotation = (float)(Util.GetHypoteneuseAngleRad(this.GetLocation(), this.unitToStalk.GetLocation()) + (90 * (Math.PI / 180)));

                if (rotation != rotation)
                {
                    rotation = previousRotation;
                }
            }
            else
            {
                rotation = previousRotation;
            }

            sb.Draw(this.texture, rect, null, this.color, rotation, new Vector2((this.texture.Width / 2), (this.texture.Height / 2)), SpriteEffects.None, this.z);

            previousRotation = rotation;
        }

        public override void OnAggroRecieved(AggroEvent e)
        {
            if (unitToStalk == null)
            {
                unitToStalk = e.from;
            }
            if (friendliesProtectingMe.Count() > 0)
            {
                for (int i = 0; i < this.friendliesProtectingMe.Count(); i++)
                {
                    Unit unit = this.friendliesProtectingMe.ElementAt(i);
                    if( unit != this) unit.OnAggroRecieved(e);
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
