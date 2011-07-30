﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;

namespace PathfindingTest.Buildings
{
    public class Factory : Building
    {

        public Factory(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Factory;
            this.constructDuration = 8;

            this.maxHealth = 3000f;
            this.currentHealth = 0f;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Factory");
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
            this.mesh = Game1.GetInstance().collision.PlaceBuilding(this.DefineSelectedRectangle());
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
