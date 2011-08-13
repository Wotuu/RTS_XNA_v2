using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using XNAInterfaceComponents.Components;

namespace XNAInterfaceComponents.ChildComponents
{
    public class Circle
    {

        public int radius { get; set; }
        public int circleWidth { get; set; }
        public int outerRadius { get; set; }
        public Texture2D outline { get; set; }

        public Point center { get; set; }
        public Color color { get; set; }

        public Component component { get; set; }

        public Circle(Component compontent, int radius, int circleWidth, Point center, Color color)
        {
            this.component = compontent;
            this.radius = radius;
            this.circleWidth = circleWidth;

            this.center = center;
            this.outerRadius = radius * 2 + 2;
            this.color = color;
        }

        public void InitCircle(SpriteBatch sb)
        {
            this.outline = new Texture2D(sb.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Color the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.Transparent;
            }

            // Work out the minimum step necessary using trigonometry + sine approximation.


            for (int i = radius; i > radius - this.circleWidth && i > 0; i--)
            {
                double angleStep = 1f / i;

                for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
                {
                    // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                    int x = (int)Math.Round(i + i * Math.Cos(angle));
                    int y = (int)Math.Round(i + i * Math.Sin(angle));

                    data[y * ( i * 2 + 2) + x + 1] = Color.White;
                }
            }

            this.outline.SetData(data);
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(outline, new Rectangle((int)center.X - radius, (int)center.Y - radius, outerRadius, outerRadius), null, color, 
               0f, Vector2.Zero, SpriteEffects.None, this.component.z - 0.0001f);
        }

        /// <summary>
        /// Get the total surface area of the Circle
        /// </summary>
        /// <returns>The total surface area</returns>
        public double GetArea()
        {
            return Math.Pow(radius, 2) * Math.PI;
        }
    }
}
