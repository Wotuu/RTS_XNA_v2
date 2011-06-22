using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PathfindingTest.Pathfinding;
using AStarCollisionMap.Collision;

namespace PathfindingTest.Selection.Patterns
{
    public class RectanglePattern : UnitGroupPattern
    {
        public int width { get; set; }
        public int spacing { get; set; }
        public int orientation { get; set; }


        public RectanglePattern(Point location, UnitSelection selection, int width, int spacing, int orientation) :
            base(location, selection)
        {
            this.width = width;
            this.spacing = spacing;
            this.orientation = orientation;
        }

        public override LinkedList<Point> ApplyPattern()
        {
            LinkedList<Point> list = new LinkedList<Point>();
            if (selection.units.Count == 1)
            {
                list.AddLast(location);
                return list;
            }

            int rows = (int)Math.Ceiling((selection.units.Count / (double)width));

            int offset_x = 0 - (((width - 1) * spacing) / 2);
            int offset_y = 0 - (((rows - 1) * spacing) / 2);

            int count = 0;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; (x < width) && (count < selection.units.Count); x++)
                {
                    // Create the rectangle pattern
                    Point newPoint = new Point(location.X + offset_x + x * spacing, 
                        location.Y + offset_y + y * spacing);
                    // Rotate the pattern around
                    newPoint = Util.GetPointOnCircle(location, Util.GetHypoteneuseLength(newPoint, this.location),
                        orientation - (Util.GetHypoteneuseAngleRad(newPoint, this.location) * (180 / Math.PI)));
                    // If it collided with a wall, adjust the pattern to fit
                    CollisionMap c = Game1.GetInstance().collision;
                    // If this point is in the middle of collision
                    if (c.CollisionAt(newPoint))
                    {
                        // Get all points between the point and the centre
                        Point[] points = c.PixelsBetweenPoints(newPoint, this.location, 1);
                        foreach (Point point in points)
                        {
                            // Get the point that doesn't have collision, and is farthest away from the centre
                            if (!c.CollisionAt(point))
                            {
                                newPoint = point;
                                break;
                            }
                        }
                    }
                    list.AddLast(newPoint);
                    count++;
                }
            }
            return list;
        }
    }
}
