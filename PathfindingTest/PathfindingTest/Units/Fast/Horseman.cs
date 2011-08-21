using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;
using Microsoft.Xna.Framework;
using PathfindingTest.Combat;
using PathfindingTest.Units.Damage;
using PathfindingTest.Audio;
using PathfindingTest.Pathfinding;

namespace PathfindingTest.Units.Fast
{
    class Horseman : Unit
    {
        public Horseman(Player p, int x, int y)
            : base(p, x, y, 2f, 20f, 100f, 80)
        {
            this.baseDamage = (int)Damage.Horseman;
            this.type = Type.Fast;
            this.visionRange = (float)VisionRange.Horseman;
            this.texture = TextureManager.GetInstance().GetTexture(this.type);
            this.hitTexture = TextureManager.GetInstance().GetHitTexture(this.type);

            this.collisionRadius = texture.Width / 2;
            this.halfTextureWidth = this.texture.Width / 2;
            this.halfTextureHeight = this.texture.Height / 2;
        }

        public override void Update(Microsoft.Xna.Framework.Input.KeyboardState ks,
            Microsoft.Xna.Framework.Input.MouseState ms)
        {
            fireCooldown--;
            UpdateMovement();
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
            Rectangle rect = this.GetDrawRectangle();
            rect.X += texture.Width / 2;
            rect.Y += texture.Height / 2;
            if (!Game1.GetInstance().IsOnScreen(rect)) return;
            //sb.Draw(this.texture, rect, null, this.color, this.direction * -1, Vector2.Zero, SpriteEffects.None, this.z);

            if (this.waypoints.Count > 0 && unitToStalk == null)
            {
                rotation = (float)(Util.GetHypoteneuseAngleRad(this.GetLocation(), this.waypoints.First.Value) + (90 * (Math.PI / 180)));

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

            if (!hitting)
            {
                sb.Draw(this.texture, rect, null, this.color, rotation, new Vector2((this.texture.Width / 2), (this.texture.Height / 2)), SpriteEffects.None, this.z);
            }
            else
            {
                sb.Draw(this.hitTexture, rect, new Rectangle(0 + 30 * (int)(hitFrame / 2), 0, 30, 45), this.color, rotation, new Vector2((this.texture.Width / 2), (this.texture.Height / 2)), SpriteEffects.None, this.z);

                if (hitFrame == (hitTexture.Width / 30) * 2 - 1)
                {
                    hitFrame = 0;
                    hitting = false;
                }
                else
                {
                    hitFrame++;
                }
            }

            previousRotation = rotation;
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
            //Console.Out.WriteLine("Recieved aggro from something! D=");
        }

        public override void OnAggro(AggroEvent e)
        {
            // Console.Out.WriteLine("Aggroing something, *grins*");
        }

        public override void Swing(Damageable target)
        {
            if (fireCooldown < 0)
            {
                hitting = true;

                if (target is Unit)
                {
                    AggroEvent e = new AggroEvent(this, target, true);
                    ((Unit)target).OnAggroRecieved(e);
                    this.OnAggro(e);
                }

                SoundManager.GetInstance().PlaySFX(SoundManager.GetInstance().lanceSounds);

                DamageEvent dmgEvent = new DamageEvent(new MeleeSwing(PathfindingTest.Combat.DamageEvent.DamageType.Melee, baseDamage), target, this);
                target.OnDamage(dmgEvent);
                this.fireCooldown = this.rateOfFire;

                // We already know that this unit is local
                if (Game1.GetInstance().IsMultiplayerGame())
                {
                    Synchronizer.GetInstance().QueueDamageEvent(dmgEvent);
                }
            }
        }
    }
}
