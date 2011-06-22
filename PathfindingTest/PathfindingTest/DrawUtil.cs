using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Pathfinding;

namespace PathfindingTest
{
    public class DrawUtil
    {

        public static Texture2D lineTexture;

        /// <summary>
        /// Gets a 1x1 texture with a clear transparent pixel in it.
        /// </summary>
        /// <param name="batch">The batch to create the texture from.</param>
        /// <returns>The texture</returns>
        public static Texture2D GetClearTexture2D(SpriteBatch batch)
        {
            Texture2D lineTexture = new Texture2D(batch.GraphicsDevice, 1, 1);
            int[] intColor = { (int)Color.White.PackedValue };
            lineTexture.SetData(intColor);
            return lineTexture;
        }

        /// <summary>
        /// Draws a line!
        /// </summary>
        /// <param name="batch">The batch to draw on.</param>
        /// <param name="start">Startpoint</param>
        /// <param name="end">Endpoint</param>
        /// <param name="c">The color of the line</param>
        public static void DrawLine(SpriteBatch batch, Point start, Point end, Color c, int width)
        {
            if (c.A == 0) return;
            if (end.X < start.X)
            {
                // Swap
                Point temp = start;
                start = end;
                end = temp;
            }
            double hypoteneuse = Util.GetHypoteneuseLength(start, end);
            float angle = Util.GetHypoteneuseAngleRad(start, end);
            batch.Draw(lineTexture, new Rectangle(start.X, start.Y, (int)Math.Round(hypoteneuse), width), null, c, angle,
                new Vector2(0, 0), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a cross.
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw on.</param>
        /// <param name="rect">The rectangle to draw on.</param>
        /// <param name="width">The width of the border.</param>
        /// <param name="c">The Color</param>
        public static void DrawCross(SpriteBatch batch, Rectangle rect, int width, Color c)
        {
            // Top left to bottom left
            DrawLine(batch,
                new Point(rect.Left, rect.Top),
                new Point(rect.Left, rect.Bottom),
                c,
                width);
            // Top left to top right
            DrawLine(batch,
                new Point(rect.Left, rect.Top),
                new Point(rect.Right, rect.Top),
                c,
                width);
            // Top right to bottom right
            DrawLine(batch,
                new Point(rect.Right, rect.Top),
                new Point(rect.Right, rect.Bottom),
                c,
                width);
            // Bottom right to bottom left
            DrawLine(batch,
                new Point(rect.Right, rect.Bottom),
                new Point(rect.Left, rect.Bottom),
                c,
                width);
        }

        public static void DrawClearRectangle(SpriteBatch batch, Rectangle rect, int width, Color c)
        {
            DrawUtil.DrawLine(batch,
                new Point(rect.Left, rect.Top),
                new Point(rect.Left, rect.Bottom),
                c,
                width);
            // Top left to top right
            DrawUtil.DrawLine(batch,
                new Point(rect.Left, rect.Top),
                new Point(rect.Right, rect.Top),
                c,
                width);
            // Top right to bottom right
            DrawUtil.DrawLine(batch,
                new Point(rect.Right, rect.Top),
                new Point(rect.Right, rect.Bottom - 1),
                c,
                width);
            // Bottom right to bottom left
            DrawUtil.DrawLine(batch,
                new Point(rect.Right, rect.Bottom - 1),
                new Point(rect.Left, rect.Bottom - 1),
                c,
                width);
        }
    }
}
