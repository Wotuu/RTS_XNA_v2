using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PathfindingTest.Units;
using PathfindingTest.Pathfinding;
using PathfindingTest.Collision;

namespace PathfindingTest.Selection.Patterns
{
    public class CirclePattern : UnitGroupPattern
    {
        public int radius { get; set; }
        public int rotation { get; set; }


        public CirclePattern(Point location, UnitSelection selection, int radius, int rotation) :
            base(location, selection)
        {
            this.radius = radius;
            this.rotation = rotation;
        }


        public override LinkedList<Point> ApplyPattern()
        {
            LinkedList<Point> list = new LinkedList<Point>();
            if (selection.units.Count == 1)
            {
                list.AddLast(location);
                return list;
            }

            double angleStep = 360.0 / (double)selection.units.Count;

            double currentAngle = rotation;
            for( int i = 0; i < selection.units.Count; i++ )
            {
                Point p = Util.GetPointOnCircle(location, this.radius, currentAngle);
                RTSCollisionMap c = Game1.GetInstance().collision;
                // If this point is in the middle of collision
                if (c.CollisionAt(p))
                {
                    // Get all points between the point and the centre
                    Point[] points = c.PixelsBetweenPoints(p, this.location, 1);
                    foreach (Point point in points)
                    {
                        // Get the point that doesn't have collision, and is farthest away from the centre
                        if (!c.CollisionAt(point))
                        {
                            p = point;
                            break;
                        }
                    }
                }
                list.AddLast(p);
                currentAngle += angleStep;
            }

            return list;
        }
    }
}
