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
        public Texture2D details { get; set; }
        public SpriteFont sf { get; set; }
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
            this.sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/HUDDetails");
            this.details = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDItemDetails");
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle((int)x, (int)y, texture.Width, texture.Height), color);

            if (DrawDetails())
            {
                sb.Draw(details, this.DefineDetailsRectangle(), color);

                switch (this.type)
                {
                    case Type.Resources:
                        sb.DrawString(sf, "Resource Structure:\r\nProvides resources...", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Barracks:
                        sb.DrawString(sf, "Barracks:\r\nConstructs Units;\r\n - Swordsmen\r\n - Bowmen\r\n - Horsemen", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Factory:
                        sb.DrawString(sf, "Factory:\r\nConstructs Units;\r\n - Sentry", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Fortress:
                        sb.DrawString(sf, "Fortress:\r\nMain Structure.\r\nConstructs Engineers", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Engineer:
                        sb.DrawString(sf, "Engineer:\r\nAble to Construct and\r\nRepair Structures", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Melee:
                        sb.DrawString(sf, "Swordsman:\r\nStrong vs. Sentry", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Heavy:
                        sb.DrawString(sf, "Sentry:\r\nStrong vs. Horsemen", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Fast:
                        sb.DrawString(sf, "Horseman:\r\nStrong vs. Bowmen", new Vector2(5, 657), Color.White);
                        break;

                    case Type.Ranged:
                        sb.DrawString(sf, "Bowman:\r\nStrong vs. Swordsmen", new Vector2(5, 657), Color.White);
                        break;

                    default:
                        break;
                }
            }
        }

        public Boolean DrawDetails()
        {
            MouseState ms = Mouse.GetState();

            if (this.DefineRectangle().Contains(new Rectangle(ms.X, ms.Y, 1, 1)))
            {
                return true;
            }

            return false;
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)this.x, (int)this.y, 28, 28);
        }

        public Rectangle DefineDetailsRectangle()
        {
            return new Rectangle(0, 652, 195, 116);
        }
    }
}
