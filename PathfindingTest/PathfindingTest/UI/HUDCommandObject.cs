using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PathfindingTest.UI
{
    public class HUDCommandObject
    {

        public Texture2D texture { get; set; }
        public Texture2D details { get; set; }
        public SpriteFont sf { get; set; }
        public Type type { get; set; }
        public Color disabledColor = new Color(125, 125, 125, 255);
        public Color color { get; set; }
        public Color hudColor { get; set; }
        public String detailString { get; set; }

        public Boolean disabled { get; set; }

        public float x;
        public float y;

        public enum Type
        {
            Move,
            Attack,
            Defend,
            Stop,
            Repair
        }

        public HUDCommandObject(Texture2D texture, Type type, float x, float y, Color color, Color hudColor)
        {
            this.texture = texture;
            this.type = type;
            this.x = x;
            this.y = y;
            this.color = color;
            this.hudColor = hudColor;
            this.details = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDItemDetails");
            this.sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/HUDDetails");

            this.detailString = this.DefineDetailString();
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
        }

        internal void Draw(SpriteBatch sb)
        {
            if (disabled)
            {
                sb.Draw(texture, new Rectangle((int)x, (int)y, texture.Width, texture.Height), disabledColor);
            }
            else
            {
                sb.Draw(texture, new Rectangle((int)x, (int)y, texture.Width, texture.Height), color);
            }

            if (DrawDetails() && !disabled)
            {
                sb.Draw(details, this.DefineDetailsRectangle(), null, hudColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0998f);
                sb.DrawString(sf, detailString, new Vector2(5, 657), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0997f);
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

        private String DefineDetailString()
        {
            switch (this.type)
            {
                case Type.Move:
                    return "Move Unit";

                case Type.Attack:
                    return "Attack Unit";

                case Type.Defend:
                    return "Defend Unit";

                case Type.Stop:
                    return "Stop whatever this Unit is\r\ndoing";

                case Type.Repair:
                    return "Repair Structure";

                default:
                    return "Details not Set";
            }
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
