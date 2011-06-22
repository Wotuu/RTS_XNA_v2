using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PathfindingTest.Buildings
{
    public class ProgressBar
    {

        public double progress { get; set; }
        public Building building { get; set; }

        private Texture2D texture { get; set; }

        private static Color BORDER_COLOR = new Color(0, 0, 0, 255),
            BACKGROUND_COLOR = new Color(0, 0, 0, 255),
            FOREGROUND_COLOR = new Color(255, 255, 0, 255);

        public ProgressBar(Building building)
        {
            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Misc/solid");
            this.building = building;
        }

        internal void Draw(SpriteBatch sb)
        {
            int w = building.texture.Width;
            int h = 5;
            int x = (int)building.x;
            int y = (int)building.y + (building.texture.Height) + h;
            int innerWidth = (int)((w / 100.0) * progress);

            sb.Draw(texture, new Rectangle(x - 1, y, w + 2, h), BORDER_COLOR);
            sb.Draw(texture, new Rectangle(x, y + 1, w - 2, h - 2), BACKGROUND_COLOR);
            sb.Draw(texture, new Rectangle(x, y + 1, innerWidth, h - 2), FOREGROUND_COLOR);
        }
    }
}
