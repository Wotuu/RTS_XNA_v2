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
        private float z { get; set; }
        public int percentage { get; set; }
        public Unit unit { get; set; }
        public Building building { get; set; }
        private Type type { get; set; }
        /// <summary>
        /// Set to true to dynamically color this healthbar according to its emptyness.
        /// @see fullColor, emptyColor
        /// </summary>
        public Boolean useDynamicColoring { get; set; }
        public Color fullColor { get; set; }
        public Color emptyColor { get; set; }

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

            this.useDynamicColoring = true;
            fullColor = new Color(0, 255, 0, 255);
            emptyColor = new Color(255, 0, 0, 255);

            this.z = this.unit.z - 0.01f;
        }

        public HealthBar(Building building)
        {
            texture = Game1.GetInstance().Content.Load<Texture2D>("Misc/solid");
            this.building = building;
            this.type = Type.Building;

            this.z = this.building.z - 0.01f;
        }


        internal void Draw(SpriteBatch sb){

            int w;
            int h;
            int x;
            int y;
            int innerWidth;

            Vector2 offset = Game1.GetInstance().drawOffset;

            switch (type)
            {
                case Type.Building:
                    w = building.texture.Width;
                    h = 5;
                    x = (int)(building.x - 1 - offset.X);
                    y = (int)(building.y - h * 2 - offset.Y);
                    innerWidth = (int)((w / 100.0) * percentage);
                    break;

                case Type.Unit:
                    w = unit.texture.Width;
                    h = 5;
                    x = (int)(unit.x - (w / 2) - offset.X);
                    y = (int)(unit.y - (unit.texture.Height / 2) - h - offset.Y);
                    innerWidth = (int)((w / 100.0) * percentage);
                    break;

                default:
                    return;
            }

            sb.Draw(this.texture, new Rectangle(x, y,
                w + 2, h), null, BORDER_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z);

            sb.Draw(this.texture, new Rectangle(x + 1, y + 1,
                w, h - 2), null, BACKGROUND_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z - 0.0001f);

            Color foreground = Color.Gold;
            if (this.useDynamicColoring)
            {
                foreground = this.GetDynamicColor();
            }
            else foreground = FOREGROUND_COLOR;

            sb.Draw(this.texture, new Rectangle(x + 1, y + 1,
                innerWidth, h - 2), null, foreground, 0f, Vector2.Zero, SpriteEffects.None, z - 0.0002f);

            /*
            sb.Draw(texture, new Rectangle(x, y, w + 2, h), BORDER_COLOR);
            sb.Draw(texture, new Rectangle(x + 1, y + 1, w, h - 2), BACKGROUND_COLOR);
            sb.Draw(texture, new Rectangle(x + 1, y + 1, innerWidth, h - 2), FOREGROUND_COLOR);*/
        }

        /// <summary>
        /// Gets a dynamic color.
        /// </summary>
        /// <returns>The color.</returns>
        public Color GetDynamicColor()
        {
            int[] emptyData = new int[] { emptyColor.R, emptyColor.G, emptyColor.B, emptyColor.A };
            int[] fullData = new int[] { fullColor.R, fullColor.G, fullColor.B, fullColor.A };

            int[] differenceData = new int[] { emptyColor.R - fullColor.R, emptyColor.G - fullColor.G, 
                emptyColor.B - fullColor.B, emptyColor.A - fullColor.A };

            return Color.FromNonPremultiplied(
                emptyColor.R - (int)((differenceData[0] / 100.0) * this.percentage),
                emptyColor.G - (int)((differenceData[1] / 100.0) * this.percentage),
                emptyColor.B - (int)((differenceData[2] / 100.0) * this.percentage),
                emptyColor.A - (int)((differenceData[3] / 100.0) * this.percentage));

        }
    }
}
