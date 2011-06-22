using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Units;

namespace PathfindingTest.Selection
{
    public class SelectRectangle
    {
        private Rectangle rect { get; set; }
        private Rectangle innerRect { get; set; }
        private Texture2D texture { get; set; }

        public Point clickedPoint { get; set; }
        private int borderWidth { get; set; }

        private static Color BORDER_COLOR = new Color(255, 0, 0, 255), CENTER_COLOR = new Color(0, 0, 0, 0);
        //CENTER_COLOR = new Color(0, 255, 0, 30);

        private void init()
        {
            Game1 game = Game1.GetInstance();
            texture = game.Content.Load<Texture2D>("Misc/solid");
            borderWidth = 2;
        }

        public SelectRectangle(Rectangle rect)
        {
            init();
            this.rect = rect;
        }

        public SelectRectangle()
        {
            init();
        }

        /// <summary>
        /// Normalizes the rectangle, so it has positive widths and heights
        /// </summary>
        /// <param name="rect">The rect to normalize</param>
        /// <returns>The normalized rect</returns>
        public Rectangle Normalize(Rectangle rect)
        {
            int x = rect.X, y = rect.Y;
            int w = rect.Width, h = rect.Height;
            if (w < 0 && h > 0) {
                return new Rectangle(x + w, y, w * -1, h);
            } else if (w > 0 && h < 0) {
                return new Rectangle(x, y + h, w, h * -1);
            } else if (w < 0 && h < 0) {
                return new Rectangle(x + w, y + h, w * -1, h * -1);
            }
            return rect;
        }

        /// <summary>
        /// Sets the rectangle of the outer dimensions of the select rectangle
        /// </summary>
        /// <param name="rect"></param>
        public void SetRectangle(Rectangle rect)
        {
            this.rect = rect;
            this.rect = Normalize(this.rect);
            this.innerRect = new Rectangle(this.rect.X + borderWidth, this.rect.Y + borderWidth,
                this.rect.Width - borderWidth * 2, this.rect.Height - borderWidth * 2);
            this.innerRect = Normalize(this.innerRect);
        }

        public Rectangle GetRectangle()
        {
            return this.rect;
        }

        internal void Draw(SpriteBatch sb)
        {
            DrawUtil.DrawClearRectangle(sb, this.rect, this.borderWidth, SelectRectangle.BORDER_COLOR);
            /*// Top left to bottom left
            Game1.GetInstance().DrawLine(sb,
                new Point(this.rect.Left, this.rect.Top),
                new Point(this.rect.Left, this.rect.Bottom + 2),
                SelectRectangle.BORDER_COLOR,
                this.borderWidth);
            // Top left to top right
            Game1.GetInstance().DrawLine(sb,
                new Point(this.rect.Left, this.rect.Top),
                new Point(this.rect.Right, this.rect.Top),
                SelectRectangle.BORDER_COLOR,
                this.borderWidth);
            // Top right to bottom right
            Game1.GetInstance().DrawLine(sb,
                new Point(this.rect.Right, this.rect.Top),
                new Point(this.rect.Right, this.rect.Bottom),
                SelectRectangle.BORDER_COLOR,
                this.borderWidth);
            // Bottom right to bottom left
            Game1.GetInstance().DrawLine(sb,
                new Point(this.rect.Right, this.rect.Bottom),
                new Point(this.rect.Left, this.rect.Bottom),
                SelectRectangle.BORDER_COLOR,
                this.borderWidth);*/
            /*Game1.GetInstance().DrawLine(sb,
                new Point(this.innerRect.Left, this.innerRect.Top),
                new Point(this.innerRect.Right, this.innerRect.Bottom),
                SelectRectangle.BORDER_COLOR,
                this.borderWidth);*/
            // sb.Draw(texture, this.rect, SelectRectangle.BORDER_COLOR);
            // sb.Draw(texture, this.innerRect, SelectRectangle.CENTER_COLOR);
        }
    }
}
