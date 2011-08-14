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

        public float z { get; set; }
        public double progress { get; set; }
        public Building building { get; set; }

        private Texture2D texture { get; set; }

        private static Color BORDER_COLOR = new Color(0, 0, 0, 255),
            BACKGROUND_COLOR = new Color(0, 0, 0, 255),
            FOREGROUND_COLOR = new Color(255, 255, 0, 255);

        public ProgressBar(Building building)
        {
            this.texture = TextureManager.GetInstance().GetSolidTexture();
            this.building = building;

            this.z = this.building.z - 0.1f;
        }

        internal void Draw(SpriteBatch sb)
        {
            int w = building.texture.Width;
            int h = 5;
            int x = (int)building.x - (int)Game1.GetInstance().drawOffset.X;
            int y = (int)building.y + (building.texture.Height) + h - (int)Game1.GetInstance().drawOffset.Y;
            int innerWidth = (int)((w / 100.0) * progress);



            sb.Draw(this.texture, new Rectangle(x - 1, y,
                w + 2, h), null, BORDER_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z);

            sb.Draw(this.texture, new Rectangle(x, y + 1,
                w - 2, h - 2), null, BACKGROUND_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z - 0.0001f);

            sb.Draw(this.texture, new Rectangle(x, y + 1,
                innerWidth, h - 2), null, FOREGROUND_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z - 0.0002f);

            /* sb.Draw(texture, new Rectangle(x - 1, y, w + 2, h), BORDER_COLOR);
            sb.Draw(texture, new Rectangle(x, y + 1, w - 2, h - 2), BACKGROUND_COLOR);
            sb.Draw(texture, new Rectangle(x, y + 1, innerWidth, h - 2), FOREGROUND_COLOR); */
        }
    }
}
