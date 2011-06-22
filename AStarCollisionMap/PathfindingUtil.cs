using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AStarCollisionMap
{
    class PathfindingUtil
    {
        /// <summary>
        /// Gets the length of the hypoteneuse between two points.
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The length</returns>
        public static double GetHypoteneuseLength(Point p1, Point p2)
        {
            int xDiff = Math.Abs(p1.X - p2.X);
            int yDiff = Math.Abs(p1.Y - p2.Y);
            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }

        /// <summary>
        /// Gets the angle of the hypoteneuse with the X-axis between two points.
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <returns>The float containing the angle, in radians</returns>
        public static float GetHypoteneuseAngleRad(Point p1, Point p2)
        {
            return (float)(Math.Atan2(p2.Y - p1.Y, p2.X - p1.X));
        }

        /// <summary>
        /// Gets the angle of the hypoteneuse with the X-axis between two points.
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <returns>The float containing the angle, in degrees</returns>
        public static float GetHypoteneuseAngleDegrees(Point p1, Point p2)
        {
            return (float)(PathfindingUtil.GetHypoteneuseAngleRad(p1, p2) * 180 / Math.PI);
        }


        /// <summary>
        /// Gets a point at the edge of a circle at a location, with given radius, with the given angle.
        /// </summary>
        /// <param name="location">The location of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="angle">The angle</param>
        /// <returns>The point at.</returns>
        public static Point GetPointOnCircle(Point location, double radius, double angle)
        {
            return new Point((int)(location.X + radius * Math.Cos(angle * Math.PI / 180F)),
                (int)(location.Y + radius * Math.Sin(angle * Math.PI / 180F)));
        }
    }
}
