using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.Misc;

namespace XNAInterfaceComponents.AbstractComponents
{
    public abstract class ChildComponent : Component
    {
        public static SpriteFont DEFAULT_FONT { get; set; }
        public static Color DEFAULT_FONT_COLOR { get; set; }
        public String text { get; set; }
        public Boolean enabled { get; set; }
        public SpriteFont font { get; set; }
        public Color fontColor { get; set; }
        public Padding padding { get; set; }

        static ChildComponent()
        {
            DEFAULT_FONT_COLOR = Color.Black;
        }

        public ChildComponent(ParentComponent parent, Rectangle bounds)
            : base(bounds)
        {
            this.parent = parent;
            parent.AddChild(this);
            this.enabled = true;
            this.backgroundColor = Color.Red;
            this.font = DEFAULT_FONT;
            this.fontColor = DEFAULT_FONT_COLOR;
            this.padding = new Padding(5, 5, 5, 5);

            this.text = "";
        }
    }
}
