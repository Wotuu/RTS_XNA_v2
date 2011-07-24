using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PathfindingTest.Buildings;
using PathfindingTest.Units;

namespace PathfindingTest.UI
{
    public class HUDObject
    {

        public Texture2D texture { get; set; }
        public Texture2D details { get; set; }
        public SpriteFont sf { get; set; }
        public Type type { get; set; }
        public Color color { get; set; }
        public String detailString { get; set; }
        public String costString { get; set; }

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
            this.details = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDItemDetails");
            this.sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/HUDDetails");

            this.detailString = this.DefineDetailString();
            this.costString = this.DefineCostString();
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle((int)x, (int)y, texture.Width, texture.Height), null, color, 0f, 
                Vector2.Zero, SpriteEffects.None, 0.0999f);

            if (DrawDetails())
            {
                sb.Draw(details, this.DefineDetailsRectangle(), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.0998f);
                sb.DrawString(sf, detailString, new Vector2(5, 657), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0997f);
                sb.DrawString(sf, "Cost: " + costString, new Vector2(5, 751), Color.White, 0f, Vector2.Zero, 
                    1f, SpriteEffects.None, 0.0996f);
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
                case Type.Resources:
                    return "Resource Structure:\r\nProvides resources...";

                case Type.Barracks:
                    return "Barracks:\r\nConstructs Units;\r\n - Swordsmen\r\n - Bowmen\r\n - Horsemen";

                case Type.Factory:
                    return "Factory:\r\nConstructs Units;\r\n - Sentry";

                case Type.Fortress:
                    return "Fortress:\r\nMain Structure.\r\nConstructs Engineers";

                case Type.Engineer:
                    return "Engineer:\r\nAble to Construct and\r\nRepair Structures";

                case Type.Melee:
                    return "Swordsman:\r\nStrong vs. Sentry";

                case Type.Heavy:
                    return "Sentry:\r\nStrong vs. Horsemen";

                case Type.Fast:
                    return "Horseman:\r\nStrong vs. Bowmen";

                case Type.Ranged:
                    return "Bowman:\r\nStrong vs. Swordsmen";

                default:
                    return "Details not Set";
            }
        }

        private String DefineCostString()
        {
            switch (this.type)
            {
                case Type.Resources:
                    return Building.GetCost(Building.Type.Resources).ToString();

                case Type.Barracks:
                    return Building.GetCost(Building.Type.Barracks).ToString();

                case Type.Factory:
                    return Building.GetCost(Building.Type.Factory).ToString();

                case Type.Fortress:
                    return Building.GetCost(Building.Type.Fortress).ToString();

                case Type.Engineer:
                    return Unit.GetCost(Unit.Type.Engineer).ToString();

                case Type.Melee:
                    return Unit.GetCost(Unit.Type.Melee).ToString();

                case Type.Heavy:
                    return Unit.GetCost(Unit.Type.HeavyMelee).ToString();

                case Type.Fast:
                    return Unit.GetCost(Unit.Type.Fast).ToString();

                case Type.Ranged:
                    return Unit.GetCost(Unit.Type.Ranged).ToString();

                default:
                    return "0";
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
