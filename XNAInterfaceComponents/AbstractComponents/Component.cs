using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.AbstractComponents;
using XNAInputHandler.MouseInput;

namespace XNAInterfaceComponents.Components
{
    public abstract class Component : Drawable, MouseOverable
    {
        public Rectangle bounds { get; set; }
        public Boolean visible { get; set; }
        public ParentComponent parent { get; set; }
        public Color backgroundColor { get; set; }

        public Border border { get; set; }
        protected Texture2D clearTexture { get; set; }

        public Color mouseOverColor { get; set; }
        public Boolean isMouseOver { get; set; }
        public Boolean isFocussed { get; set; }

        public Component(Rectangle bounds)
        {
            this.bounds = bounds;
            this.backgroundColor = Color.White;
            this.mouseOverColor = Color.Orange;
            this.visible = true;
            this.border = new Border(this, 1, Color.Black);
        }

        /// <summary>
        /// Gets the location on the screen of this component.
        /// </summary>
        /// <returns>The location relative to the window.</returns>
        public Rectangle GetScreenBounds()
        {
            if (this.parent != null)
            {
                Point screenLocation = parent.RequestScreenLocation(this.GetLocation());
                return new Rectangle(screenLocation.X, screenLocation.Y, this.bounds.Width, this.bounds.Height);
            }
            else return this.bounds;
        }

        /// <summary>
        /// Gets the location relative to the parent.
        /// </summary>
        /// <returns>The location</returns>
        public Point GetLocation()
        {
            return new Point(this.bounds.X, this.bounds.Y);
        }

        public abstract void Draw(SpriteBatch sb);
        public abstract void Update();
        public abstract void OnMouseEnter(MouseEvent e);
        public abstract void OnMouseExit(MouseEvent e);
        public abstract void Unload();
    }
}
