using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.AbstractComponents;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.ParentComponents;

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

        public float z { get; set; }

        public Component(ParentComponent parent, Rectangle bounds)
        {
            this.parent = parent;
            this.bounds = bounds;
            this.backgroundColor = Color.White;
            this.mouseOverColor = Color.Orange;
            this.visible = true;
            this.border = new Border(this, 1, Color.Black);
            if (parent != null) this.z = parent.z - 0.01f;
            else this.z = 1f - this.GetDrawDepthOffset();
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

        /// <summary>
        /// Takes the depth of this component, and multiplies it by 0.01f to get a value which you can use
        /// to draw the component at the correct depth on the screen.
        /// </summary>
        /// <returns>The value, effectively this.GetDepth() * 0.01f;</returns>
        protected float GetDrawDepthOffset()
        {
            return (this.GetDepth() * 0.01f);
        }

        /// <summary>
        /// Gets the depth of this component.
        /// </summary>
        /// <returns>The depth.</returns>
        public virtual int GetDepth()
        {
            if (this.parent == null) return 1;
            else return this.parent.GetDepth() + 1;
        }

        public abstract void Draw(SpriteBatch sb);
        public abstract void Update();
        public abstract void OnMouseEnter(MouseEvent e);
        public abstract void OnMouseExit(MouseEvent e);
        public abstract void Unload();
    }
}
