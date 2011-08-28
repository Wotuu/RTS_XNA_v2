using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Multiplayer.Data;
using PathfindingTest.Pathfinding;

namespace PathfindingTest.Buildings
{
    public class Sentry : Building
    {

        public Sentry(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Sentry;
            this.constructDuration = 10;

            this.visionRange = 50f;

            this.maxHealth = 500f;
            this.currentHealth = 0f;

            this.texture = TextureManager.GetInstance().GetTexture(this.type);
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            this.DefaultUpdate(ks, ms);
        }

        internal override void Draw(SpriteBatch sb)
        {
            this.DefaultDraw(sb);
        }

        public override void PlaceBuilding(Units.Engineer e)
        {
            this.state = State.Constructing;
            this.constructedBy = e;
            e.constructing = this;
            this.mesh = Game1.GetInstance().map.collisionMap.PlaceBuilding(this.DefineRectangle());
            Game1.GetInstance().IsMouseVisible = true;

            for( int i = 0; i < p.buildings.Count(); i++){
                Building b = p.buildings.ElementAt(i);
                if (b != this)
                {
                    if (b.type != Type.Resources && b.type != Type.Sentry)
                    {
                        if (b.waypoints.Count() > 0)
                        {
                            Point point = b.waypoints.GetLast();
                            PathfindingProcessor.GetInstance().Push(b, point);
                        }
                    }
                }
            }

            if (Game1.GetInstance().IsMultiplayerGame() &&
                     this.p == Game1.CURRENT_PLAYER)
            {
                Synchronizer.GetInstance().QueueBuilding(this);
            }

            p.resources -= Building.GetCost(this.type);
        }
    }
}
