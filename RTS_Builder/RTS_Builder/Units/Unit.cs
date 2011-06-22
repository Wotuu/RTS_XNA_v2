using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RTS_Builder
{
    public abstract class Unit
    {

        public Player p { get; set; }
        public Vector2 location { get; set; }
        public float movementSpeed { get; set; }
        public float direction { get; set; }
        public Color c { get; set; }

        public Boolean selected { get; set; }
        public Boolean hasToMove { get; set; }
        public Vector2 waypoint { get; set; }

        public UnitType type { get; set; }
        public Texture2D texture { get; set; }

        public enum UnitType
        {
            Engineer,
            Melee,
            HeavyMelee,
            Fast,
            Ranged
        }

        public abstract void Update(KeyboardState ks, MouseState ms);

        internal abstract void Draw(SpriteBatch sb);

        /// <summary>
        /// Defines the rectangle/hitbox for the Unit.
        /// </summary>
        /// <returns></returns>
        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height);
        }

        /// <summary>
        /// Defines the rectangle for drawing the 'Selected' texture.
        /// </summary>
        /// <returns></returns>
        public Rectangle DefineSelectedRectangle()
        {
            return new Rectangle((int)location.X - 3, (int)location.Y - 3, texture.Width + 6, texture.Height + 6);
        }

        /// <summary>
        /// Adds this Unit to the LinkedList selected units.
        /// </summary>
        /// <param name="b"></param>
        public void setSelected(Boolean b)
        {
            selected = b;
        }

        /// <summary>
        /// Set the point this Unit has to move to.
        /// direction != direction is used for checking NaNExceptions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setWaypoint(int x, int y)
        {
            double a = Math.Abs((int)(location.X + (texture.Width / 2) - x));
            double b = Math.Abs((int)(location.Y + (texture.Height / 2) - y));
            direction = (float)Math.Atan(a / b);

            if (direction != direction)
            {
                hasToMove = false;
                return;
            }

            waypoint = new Vector2(x, y);
            hasToMove = true;
        }

        /// <summary>
        /// Updates the drawing position of this Unit.
        /// </summary>
        public void move()
        {
            float locX = location.X + (texture.Width / 2);
            float locY = location.Y + (texture.Height / 2);

            if (locX < waypoint.X && locY < waypoint.Y)
            {
                Vector2 newLocation = new Vector2((location.X + (movementSpeed * (float)Math.Sin(direction))), (location.Y + (movementSpeed * (float)Math.Cos(direction))));
                this.location = newLocation;
            }
            else if (locX < waypoint.X && locY > waypoint.Y)
            {
                Vector2 newLocation = new Vector2((location.X + (movementSpeed * (float)Math.Sin(direction))), (location.Y - (movementSpeed * (float)Math.Cos(direction))));
                this.location = newLocation;
            }
            else if (locX > waypoint.X && locY < waypoint.Y)
            {
                Vector2 newLocation = new Vector2((location.X - (movementSpeed * (float)Math.Sin(direction))), (location.Y + (movementSpeed * (float)Math.Cos(direction))));
                this.location = newLocation;
            }
            else if (locX > waypoint.X && locY > waypoint.Y)
            {
                Vector2 newLocation = new Vector2((location.X - (movementSpeed * (float)Math.Sin(direction))), (location.Y - (movementSpeed * (float)Math.Cos(direction))));
                this.location = newLocation;
            }

            if (locX == waypoint.X && locY == waypoint.Y)
            {
                hasToMove = false;
            }
        }
    }
}
