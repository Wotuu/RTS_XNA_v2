using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PathfindingTest.Players;
using PathfindingTest.Primitives;
using PathfindingTest.Units;
using PathfindingTest.Multiplayer.Data;

namespace PathfindingTest.Buildings
{
    public class ResourceGather : Building
    {

        public Circle resourceRange { get; set; }
        public LinkedList<Rectangle> surface { get; set; }
        public int resources { get; set; }
        public Boolean resourcesAdded { get; set; }
        public int generationBarrier { get; set; }
        public Boolean secondAdded { get; set; }
        public int previousSecond { get; set; }
        public int secondsPast { get; set; }
        public SpriteFont animationFont { get; set; }
        public Boolean drawAnimation { get; set; }
        public int animationCounter { get; set; }

        public ResourceGather(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Resources;
            this.constructDuration = 3;

            this.maxHealth = 1000f;
            this.currentHealth = 0f;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Resources");
            this.animationFont = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/ResourceFont");

            this.resourceRange = new Circle(100, this, c);
            this.generationBarrier = 25;
            this.resourcesAdded = false;
            this.secondAdded = false;
            this.secondsPast = 0;
            this.previousSecond = -1;

            this.drawAnimation = false;
            this.animationCounter = 0;
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            DefaultUpdate(ks, ms);

            if (state == State.Finished)
            {
                Generate();
            }

            if (animationCounter > 75)
            {
                drawAnimation = false;
            }
            else
            {
                this.animationCounter += 1;
            }
        }

        internal override void Draw(SpriteBatch sb)
        {
            DefaultDraw(sb);

            if (state == State.Preview || selected)
            {
                resourceRange.Draw(sb);
            }

            if (drawAnimation)
            {
                float newAlpha = 256 - ((animationCounter / 76f) * 255);
                Vector2 fontVector = animationFont.MeasureString("+" + resources);
                sb.DrawString(animationFont, "+" + resources, new Vector2(x + (texture.Width / 2) - (fontVector.X / 2), y + (texture.Height / 2) - (fontVector.Y) - (animationCounter / 2)), new Color(255, 255, 255, newAlpha));
            }
        }

        public override void PlaceBuilding(Units.Engineer e)
        {
            this.state = State.Constructing;
            this.constructedBy = e;
            e.constructing = this;
            this.mesh = Game1.GetInstance().collision.PlaceBuilding(this.DefineSelectedRectangle());
            this.waypoint = new Point((int)this.x + (this.texture.Width / 2), (int)this.y + this.texture.Height + 20);
            Game1.GetInstance().IsMouseVisible = true;

            if (Game1.GetInstance().IsMultiplayerGame() &&
                     this.p == Game1.CURRENT_PLAYER)
            {
                Synchronizer.GetInstance().QueueBuilding(this);
            }

            resources = CalculateRPS();
        }

        public int CalculateRPS()
        {
            resourceRange.SetSurface();
            double rs = 50;
            //double total = surface.Count;
            //double part = 0;

            //foreach (Rectangle r in surface)
            //{
            //    foreach (ResourceGather rg in p.buildings)
            //    {
            //        if (rg != this)
            //        {
            //            foreach (Rectangle ro in rg.resourceRange.GetSurface())
            //            {
            //                if (r.Contains(ro))
            //                {
            //                    part++;
            //                }
            //            }
            //        }
            //    }
            //}

            //rs -= rs * (part / total);

            return (int)rs;
        }

        public void Generate()
        {
            if (previousSecond == -1)
            {
                previousSecond = DateTime.Now.Second;
            }
            int currentSecond = DateTime.Now.Second;

            if (currentSecond != previousSecond && !secondAdded)
            {
                if (previousSecond == 59 && currentSecond == 0)
                {
                    secondsPast += 1;
                }
                else
                {
                    secondsPast += currentSecond - previousSecond;
                }

                secondAdded = true;
            }
            else if (currentSecond == previousSecond)
            {
                secondAdded = false;
            }

            if (secondsPast == generationBarrier && !resourcesAdded)
            {
                p.resources += resources;
                resourcesAdded = true;
                drawAnimation = true;
                animationCounter = 0;
                secondsPast = 0;
            }
            else if (secondsPast != generationBarrier)
            {
                resourcesAdded = false;
            }

            previousSecond = DateTime.Now.Second;
        }
    }
}