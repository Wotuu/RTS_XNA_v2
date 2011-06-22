using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PathfindingTest.Buildings;
using System.Diagnostics;
using PathfindingTest.Pathfinding;

namespace PathfindingTest.Primitives
{
    public class Circle
    {

        public int radius { get; set; }
        public int outerRadius { get; set; }
        public Texture2D outline { get; set; }
        public Texture2D surface { get; set; }
        public Building drawOn { get; set; }
        public Color color { get; set; }
        public float px { get; set; }
        public float py { get; set; }

        public Circle(int radius, Building drawOn, Color color)
        {
            this.radius = radius;
            this.outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            this.outline = new Texture2D(Game1.GetInstance().GraphicsDevice, outerRadius, outerRadius);
            this.drawOn = drawOn;
            this.color = color;

            Color[] data = new Color[outerRadius * outerRadius];

            // Color the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            this.outline.SetData(data);
        }

        internal void Draw(SpriteBatch sb)
        {
            this.px = drawOn.x - radius + (drawOn.texture.Width / 2);
            this.py = drawOn.y - radius + (drawOn.texture.Height / 2);
            sb.Draw(outline, new Rectangle((int)px, (int)py, outerRadius, outerRadius), color);

            if (surface != null)
            {
                sb.Draw(surface, new Rectangle((int)px, (int)py, outerRadius, outerRadius), Color.Blue);
            }
        }

        /// <summary>
        /// Set the surface Texture for this Circle
        /// </summary>
        public void SetSurface()
        {
            this.surface = new Texture2D(Game1.GetInstance().GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep * (8 * Math.PI))
            {
                int x = (int)Math.Round(radius + radius * Math.Cos(angle)) + (int)px + 1;
                int y = (int)Math.Round(radius + radius * Math.Sin(angle)) + (int)py;
                int cx = (int)px + radius;
                int cy = (int)py + radius;

                Rectangle r = new Rectangle();

                if (x <= cx && y <= cy)
                {
                    r = new Rectangle(x, y, cx - x, cy - y);
                }
                else if (x <= cx && y > cy)
                {
                    r = new Rectangle(x, cy, cx - x, y - cy);
                }
                else if (x > cx && y <= cy)
                {
                    r = new Rectangle(cx, y, x - cx, cy - y);
                }
                else if (x > cx && y > cy)
                {
                    r = new Rectangle(cx, cy, x - cx, y - cy);
                }

                for (int i = 0; i < r.Width; i += 8)
                {
                    for (int j = 0; j < r.Height; j += 8)
                    {
                        data[i * outerRadius + j + 1] = Color.White;
                    }
                }
            }

            this.surface.SetData(data);
        }
    }
}
