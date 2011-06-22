using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingTest.Selection.Patterns
{
    public class PatternPreviewObject
    {
        public Point location { get; set; }
        public Texture2D texture { get; set; }


        public PatternPreviewObject(Point location)
        {
            texture = Game1.GetInstance().Content.Load<Texture2D>("Misc/patternpreview");
            this.location = location;
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle(location.X - (texture.Width / 2), location.Y - (texture.Height / 2), 
                texture.Width, texture.Height), Color.White);
        }
    }
}
