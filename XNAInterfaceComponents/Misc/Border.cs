using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace XNAInterfaceComponents.AbstractComponents
{
    public class Border : Drawable
    {
        public int width { get; set; }
        public Color color { get; set; }
        public Component parent { get; set; }

        public Border(Component parent, int width, Color color)
        {
            this.parent = parent;
            this.width = width;
            this.color = color;
        }

        public void Draw(SpriteBatch sb)
        {
            ComponentUtil.DrawClearRectangle(sb, this.parent.GetScreenBounds(), this.width, this.color);
        }
    }
}
