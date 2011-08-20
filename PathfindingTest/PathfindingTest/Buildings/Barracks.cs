using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Players;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;
using PathfindingTest.Pathfinding;

namespace PathfindingTest.Buildings
{
    public class Barracks : Building
    {

        public Barracks(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Barracks;
            this.constructDuration = 5;

            this.visionRange = 50f;

            this.maxHealth = 2000f;
            this.currentHealth = 0f;

            this.texture = TextureManager.GetInstance().GetTexture(this.type);
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            DefaultUpdate(ks, ms);
        }

        public override void PlaceBuilding(Units.Engineer e)
        {
            this.state = State.Constructing;
            this.constructedBy = e;
            e.constructing = this;
            this.mesh = Game1.GetInstance().map.collisionMap.PlaceBuilding(this.DefineRectangle());
            this.originWaypoint = new Point((int)this.x + (this.texture.Width / 2), (int)this.y + this.texture.Height + 20);
            this.waypoints = new LinkedList<Point>();
            Game1.GetInstance().IsMouseVisible = true;

            foreach (Building b in p.buildings)
            {
                if (b != this)
                {
                    if (b.type != Type.Resources && b.type != Type.Sentry)
                    {
                        if (b.waypoints.Count > 0)
                        {
                            Point point = b.waypoints.Last.Value;
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

        internal override void Draw(SpriteBatch sb)
        {
            DefaultDraw(sb);
        }
    }
}
