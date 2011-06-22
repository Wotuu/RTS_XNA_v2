using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PathfindingTest;
using PathfindingTest.Players;
using PathfindingTest.UI;
using PathfindingTest.Pathfinding;
using PathfindingTest.Combat;
using PathfindingTest.Buildings;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;

namespace PathfindingTest.Units
{
    public class Engineer : Unit
    {
        private Texture2D collisionRadiusTexture { get; set; }

        public Building constructing { get; set; }

        /// <summary>
        /// Engineer Constructor.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="cm"></param>
        /// <param name="startLocation"></param>
        /// <param name="c"></param>
        public Engineer(Player p, int x, int y)
            : base(p, x, y, 1f, 1f, 1f, 1f)
        {
            this.baseDamage = (int) Unit.Damage.Engineer;
            this.type = Type.Engineer;
            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/Engineer");
            this.collisionRadiusTexture = Game1.GetInstance().Content.Load<Texture2D>("Misc/patternPreview");

            // Console.Out.WriteLine("Constructed an engineer @ " + this.GetLocation() + " (" + x + ", " + y + ")");

            this.collisionRadius = texture.Width / 2;
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public override void Update(KeyboardState ks, MouseState ms)
        {
                UpdateMovement();
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal override void Draw(SpriteBatch sb)
        {
                //sb.Draw(this.collisionRadiusTexture,
                //    new Rectangle((int)(x - collisionRadius), (int)(y - collisionRadius), 
                //        (int)(collisionRadius * 2), (int)(collisionRadius * 2)), this.color);
                sb.Draw(this.texture, new Vector2(x - (texture.Width / 2), y - (texture.Height / 2)), this.color);

                /*if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    this.DrawHealthBar(sb);
                }*/
        }

        public override void OnAggroRecieved(AggroEvent e)
        {
            //todo flee
            // Console.Out.WriteLine("Recieved aggro from something! D=");
        }

        public override void OnAggro(AggroEvent e)
        {
            // Console.Out.WriteLine("Aggroing something, *grins*");
        }

        public override void Swing()
        {
        }


        public void Repair(Building b)
        {
            Point p = new Point((int)(b.x + (b.texture.Width / 2)), (int)(b.y + (b.texture.Height / 2)));

            // Add a point that is on the circle near the building, not inside the building!
            Point targetPoint = new Point(0, 0);
            if (this.waypoints.Count == 0) targetPoint = new Point((int)this.x, (int)this.y);
            else targetPoint = this.waypoints.ElementAt(this.waypoints.Count - 1);
            // Move to the point around the circle of the building, but increase the radius a bit
            // so we're not standing on the exact top of the building
            this.MoveToQueue(
                Util.GetPointOnCircle(p, b.GetCircleRadius() + this.texture.Width / 2,
                Util.GetHypoteneuseAngleDegrees(p, targetPoint)));
            
            b.constructedBy = this;
            this.constructing = b;

            if (b.state == Building.State.Interrupted)
            {
                b.state = Building.State.Constructing;
            }
            else if (b.state == Building.State.Finished || b.state == Building.State.Producing)
            {
                b.state = Building.State.Repairing;
            }
        }
    }
}
