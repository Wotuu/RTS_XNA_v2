using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PathfindingTest.Buildings;

namespace PathfindingTest.Units
{
    public class HealthBar
    {
        public int percentage { get; set; }
        public Unit unit { get; set; }
        public Building building { get; set; }
        private Type type { get; set; }

        private Texture2D texture { get; set; }

        private enum Type
        {
            Building,
            Unit
        }

        private static Color BORDER_COLOR = new Color(0, 0, 0, 255),
            BACKGROUND_COLOR = new Color(255, 0, 0, 255),
            FOREGROUND_COLOR = new Color(0, 255, 0, 255);

        public HealthBar(Unit unit)
        {
            texture = Game1.GetInstance().Content.Load<Texture2D>("Misc/solid");
            this.unit = unit;
            this.type = Type.Unit;
        }

        public HealthBar(Building building)
        {
            texture = Game1.GetInstance().Content.Load<Texture2D>("Misc/solid");
            this.building = building;
            this.type = Type.Building;
        }


        internal void Draw(SpriteBatch sb){

            int w;
            int h;
            int x;
            int y;
            int innerWidth;

            switch (type)
            {
                case Type.Building:
                    w = building.texture.Width;
                    h = 5;
                    x = (int)building.x - 1;
                    y = (int)building.y - h * 2;
                    innerWidth = (int)((w / 100.0) * percentage);
                    break;

                case Type.Unit:
                    w = unit.texture.Width;
                    h = 5;
                    x = (int)unit.x - (w / 2);
                    y = (int)unit.y - (unit.texture.Height / 2) - h;
                    innerWidth = (int)((w / 100.0) * percentage);
                    break;

                default:
                    return;
            }

            sb.Draw(texture, new Rectangle(x, y, w + 2, h), BORDER_COLOR);
            sb.Draw(texture, new Rectangle(x + 1, y + 1, w, h - 2), BACKGROUND_COLOR);
            sb.Draw(texture, new Rectangle(x + 1, y + 1, innerWidth, h - 2), FOREGROUND_COLOR);
        }
    }
}
