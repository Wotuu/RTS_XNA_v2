﻿using System;
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

namespace PathfindingTest.Units.Fast
{
    class Horseman : Unit
    {
        public Horseman(Player p, int x, int y)
            : base(p, x, y, 2f, 20f, 100f, 50f)
        {
            this.baseDamage = (int)Unit.Damage.Horseman;
            this.type = Type.Fast;
            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/horseman");

            this.collisionRadius = texture.Width / 2;
            this.halfTextureWidth = this.texture.Width / 2;
            this.halfTextureHeight = this.texture.Height / 2;
        }

        public override void Update(Microsoft.Xna.Framework.Input.KeyboardState ks,
            Microsoft.Xna.Framework.Input.MouseState ms)
        {
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

        internal override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            sb.Draw(this.texture, new Rectangle((int)this.x - this.halfTextureWidth, (int)this.y - this.halfTextureHeight,
                texture.Width, texture.Height), null, this.color, 0f, Vector2.Zero, SpriteEffects.None, this.z);
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
            if (target is Unit)
            {
                AggroEvent e = new AggroEvent(this, target, true);
                ((Unit)target).OnAggroRecieved(e);
                this.OnAggro(e);
            }
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
