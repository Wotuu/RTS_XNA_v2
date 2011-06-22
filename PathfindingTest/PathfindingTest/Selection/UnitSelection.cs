using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using PathfindingTest.Selection.Patterns;
using Microsoft.Xna.Framework;
using PathfindingTest.Pathfinding;
using PathfindingTest.UI;
using PathfindingTest.Buildings;
using PathfindingTest.Multiplayer.Data;

namespace PathfindingTest.Selection
{
    public class UnitSelection
    {
        public LinkedList<Unit> units { get; set; }

        /// <summary>
        /// Gets the point that is furthest away of a collcetion from a unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="points">The list of points to chose from</param>
        /// <returns>The point.</returns>
        private Point GetFarthestPoint(Unit unit, LinkedList<Point> points)
        {
            Point farthestPoint = new Point(0, 0);
            double farthestDistance = 0;
            Point unitLocation = new Point((int)unit.x, (int)unit.y);
            foreach (Point p in points)
            {
                double currentDistance = Util.GetHypoteneuseLength(p, unitLocation);
                if (currentDistance > farthestDistance)
                {
                    farthestPoint = p;
                    farthestDistance = currentDistance;
                }
            }
            return farthestPoint;
        }

        /// <summary>
        /// Gets the point that is closest of a collection from a unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="points">The list of points to chose from</param>
        /// <returns>The point.</returns>
        private Point GetClosestPoint(Unit unit, LinkedList<Point> points)
        {
            Point closestPoint = new Point(0, 0);
            double closestDistance = Double.MaxValue;
            Point unitLocation = new Point((int)unit.x, (int)unit.y);
            foreach (Point p in points)
            {
                double currentDistance = Util.GetHypoteneuseLength(p, unitLocation);
                if (currentDistance < closestDistance)
                {
                    closestPoint = p;
                    closestDistance = currentDistance;
                }
            }
            return closestPoint;
        }

        /// <summary>
        /// Moves this unit group according to the given pattern.
        /// </summary>
        /// <param name="pattern">The pattern to use when calculating where the units should go.</param>
        public void MoveTo(UnitGroupPattern pattern)
        {
            LinkedList<Point> points = pattern.ApplyPattern();
            int count = 0;
            foreach (Unit unit in units)
            {
                // This part is used for checking whether an Engineer should stop constructing or not.
                if (unit.type == Unit.Type.Engineer)
                {
                    Engineer temp = (Engineer)unit;

                    if (temp.constructing != null)
                    {
                        if (temp.constructing.state == Building.State.Constructing)
                        {
                            temp.constructing.state = Building.State.Interrupted;
                        }
                        else if (temp.constructing.state == Building.State.Repairing)
                        {
                            temp.constructing.state = Building.State.Finished;
                        }

                        temp.constructing.constructedBy = null;
                        temp.constructing = null;
                    }
                }
                // </check>

                Point farthestPoint = GetClosestPoint(unit, points);
                unit.MoveToQueue(farthestPoint);
                points.Remove(farthestPoint);
                count++;
            }
        }

        /// <summary>
        /// Deselects all units in this UnitSelection
        /// </summary>
        public void DeselectAll()
        {
            foreach (Unit unit in units)
            {
                unit.selected = false;
            }
        }

        /// <summary>
        /// Selects all units in this UnitSelection
        /// </summary>
        public void SelectAll()
        {
            foreach (Unit unit in units)
            {
                unit.selected = true;
            }
        }

        public UnitSelection()
        {
            this.units = new LinkedList<Unit>();
        }

        public UnitSelection(LinkedList<Unit> units)
        {
            this.units = units;
        }
    }
}
