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
        public LinkedList<Circle> surface { get; set; }
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
            this.surface = new LinkedList<Circle>();

            Color[] data = new Color[outerRadius * outerRadius];

            // Color the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.Transparent;
            }

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
        }

        /// <summary>
        /// Set the surface Texture for this Circle
        /// </summary>
        public void SetSurface()
        {
            for (int i = radius; i >= drawOn.texture.Width / 2; i -= 16)
            {
                surface.AddLast(new Circle(i, this.drawOn, Color.Blue));
            }
        }

        /// <summary>
        /// Get the Point in the center of the Circle
        /// </summary>
        /// <returns>The central Point</returns>
        public Point GetCenter()
        {
            return new Point((int)px + (outline.Width / 2), (int)py + (outline.Height / 2));
        }

        public void UpdatePosition()
        {
            this.px = drawOn.x - radius + (drawOn.texture.Width / 2);
            this.py = drawOn.y - radius + (drawOn.texture.Height / 2);

            foreach (Circle circle in surface)
            {
                circle.UpdatePosition();
            }
        }

        /// <summary>
        /// Get the total surface area of the Circle
        /// </summary>
        /// <returns>The total surface area</returns>
        public double GetArea()
        {
            return Math.Pow(radius, 2) * Math.PI;
        }

        /// <summary>
        /// Calculate the distance between the central point of two Circles
        /// </summary>
        /// <param name="oc">The other Circle</param>
        /// <returns>The distance between the two Circles</returns>
        public double CalculateDistance(Circle oc)
        {
            Point p1 = this.GetCenter();
            Point p2 = oc.GetCenter();
            double deltaX = Math.Abs(p1.X - p2.X);
            double deltaY = Math.Abs(p1.Y - p2.Y);

            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            return distance;
        }

        /// <summary>
        /// Checks if two Circles overlap or not
        /// </summary>
        /// <param name="oc">The other Circle</param>
        /// <returns>True if the two Circles overlap, false if not</returns>
        public Boolean Intersects(Circle oc)
        {
            double distanceSB = this.radius + oc.radius;
            double distance = this.CalculateDistance(oc);

            if (distance < distanceSB)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates how much of two Circles overlap
        /// Isn't Math great ಥ-ಥ
        /// </summary>
        /// <param name="oc">The other Circle</param>
        /// <returns>The amount of overlap as a number between 0 and 1</returns>
        public double CalculateOverlap(Circle oc)
        {
            if (this.Intersects(oc))
            {
                double c = this.CalculateDistance(oc);
                double CBD = 2 * Math.Acos((Math.Pow(oc.radius, 2) + Math.Pow(c, 2) - Math.Pow(this.radius, 2)) / (2 * oc.radius * c));
                double CAD = 2 * Math.Acos((Math.Pow(this.radius, 2) + Math.Pow(c, 2) - Math.Pow(oc.radius, 2)) / (2 * this.radius * c));
                
                double circleArea = this.GetArea();
                double overlapArea = ((0.5 * CBD * Math.Pow(oc.radius, 2)) - (0.5 * Math.Pow(oc.radius, 2) * Math.Sin(CBD)))
                                     + ((0.5 * CAD * Math.Pow(this.radius, 2)) - (0.5 * Math.Pow(this.radius, 2) * Math.Sin(CAD)));

                double overlap = overlapArea / circleArea;

                return overlap;
            }
            else
            {
                return 0;
            }
        }
    }
}
