using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer.Data;

namespace PathfindingTest.Buildings
{
    public class Fortress : Building
    {

        public Fortress(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Fortress;
            this.constructDuration = 15;

            this.visionRange = (float)VisionRange.Fortress;

            this.maxHealth = 5000f;
            this.currentHealth = 0f;

            this.texture = TextureManager.GetInstance().GetTexture(this.type);
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            DefaultUpdate(ks, ms);
        }

        internal override void Draw(SpriteBatch sb)
        {
            DefaultDraw(sb);
        }

        public override void PlaceBuilding(Units.Engineer e)
        {
            this.state = State.Constructing;
            this.constructedBy = e;
            e.constructing = this;
            this.mesh = Game1.GetInstance().map.collisionMap.PlaceBuilding(this.DefineRectangle());
            this.originWaypoint = new Point((int)this.x + (this.texture.Width / 2), (int)this.y + this.texture.Height + 20);
            this.waypoint = new Point((int)this.x + (this.texture.Width / 2), (int)this.y + this.texture.Height + 20);
            Game1.GetInstance().IsMouseVisible = true; 
            
            if (Game1.GetInstance().IsMultiplayerGame() &&
                     this.p == Game1.CURRENT_PLAYER)
            {
                Synchronizer.GetInstance().QueueBuilding(this);
            }

            p.resources -= Building.GetCost(this.type);
        }
    }
}
