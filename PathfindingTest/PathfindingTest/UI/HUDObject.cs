using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PathfindingTest.UI
{
    public class HUDObject
    {

        public Texture2D texture { get; set; }
        public Type type { get; set; }
        public Color color { get; set; }

        public float x;
        public float y;

        public enum Type
        {
            Resources,
            Barracks,
            Factory,
            Fortress,
            Engineer,
            Melee,
            Heavy,
            Fast,
            Ranged
        }

        public HUDObject(Texture2D texture, Type type, float x, float y, Color color)
        {
            this.texture = texture;
            this.type = type;
            this.x = x;
            this.y = y;
            this.color = color;
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle((int)x, (int)y, texture.Width, texture.Height), color);
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)this.x, (int)this.y, 28, 28);
        }
    }
}
