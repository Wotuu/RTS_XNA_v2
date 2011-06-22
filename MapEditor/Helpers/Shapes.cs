using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.Helpers
{
    class Shapes
    {
        /// <summary>
        /// Draws a line!
        /// </summary>
        /// <param name="batch">The batch to draw on.</param>
        /// <param name="start">Startpoint</param>
        /// <param name="end">Endpoint</param>
        /// <param name="c">The color of the line</param>
        public void DrawLine(SpriteBatch batch, Point start, Point end, Color c, int width,Texture2D Linetexture)
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
            batch.Draw(Linetexture, new Rectangle(start.X, start.Y, (int)Math.Round(hypoteneuse), width), null, c, angle,
                new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public void DrawLine(SpriteBatch batch, Vector2 PointA, Vector2 PointB, Color XnaColor, Texture2D Linetexture)
        {
            int distance = (int)Vector2.Distance(PointA, PointB);

            Vector2 connection = PointB - PointA;
            Vector2 base_vector = new Vector2(1, 0);

            float alpha = (float)Math.Acos(Vector2.Dot(connection, base_vector) / (connection.Length() * base_vector.Length()));

            batch.Draw(Linetexture, new Rectangle((int)PointA.X, (int)PointA.Y, distance, 1),
                              null, XnaColor, alpha, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public void DrawRectangle(Rectangle Rect, Color XnaColor,SpriteBatch batch,Texture2D linetexture)
        {
            // | left
            DrawLine(batch,new Vector2(Rect.X, Rect.Y), new Vector2(Rect.X, Rect.Y + Rect.Height), XnaColor,linetexture);
            // - top
            DrawLine(batch,new Vector2(Rect.X, Rect.Y), new Vector2(Rect.X + Rect.Width, Rect.Y), XnaColor,linetexture);
            // - bottom
            DrawLine(batch, new Vector2(Rect.X, Rect.Y + Rect.Height), new Vector2(Rect.X + Rect.Width, Rect.Y + Rect.Height), XnaColor, linetexture);
            // | right
            DrawLine(batch, new Vector2(Rect.X + Rect.Width, Rect.Y), new Vector2(Rect.X + Rect.Width, Rect.Y + Rect.Height), XnaColor, linetexture);
        }
    }
}
