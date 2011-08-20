using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using Microsoft.Xna.Framework;
using PathfindingTest.State;
using PathfindingTest.Buildings;

namespace PathfindingTest.Pathfinding
{
    public class PathfindingProcessor
    {
        private static PathfindingProcessor instance { get; set; }
        private LinkedList<ObjectProcess> toProcess { get; set; }
        private readonly object queueLock = new object();

        /// <summary>
        /// Pushes an object onto the list.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <param name="target">The target of the unit</param>
        public void Push(Object obj, Point target)
        {
            lock (queueLock)
            {
                //Console.Out.WriteLine("Adding a unit to find it's path: \n" +
                //    "StackTrace: '{0}'", Environment.StackTrace);
                toProcess.AddLast(new ObjectProcess(obj, target));
            }
        }

        /// <summary>
        /// Removes an object from the process list.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void Remove(Object obj)
        {
            for (int i = 0; i < toProcess.Count; i++)
            {
                ObjectProcess up = toProcess.ElementAt(i);
                if (obj is Unit)
                {
                    if (up.unit == (Unit)obj)
                    {
                        toProcess.Remove(up);
                        break;
                    }
                }
                else if (obj is Building)
                {
                    if (up.building == (Building)obj)
                    {
                        toProcess.Remove(up);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Process the queue.
        /// </summary>
        public void Process()
        {
            lock (queueLock)
            {
                if (SmartPathfindingNodeProcessor.GetInstance().GetCount() == 0)
                {
                    double timeTaken = 0;
                    int count = 0;
                    do
                    {
                        timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
                        if (toProcess.Count != 0)
                        {
                            ObjectProcess up = toProcess.ElementAt(0);

                            if (up.unit != null)
                            {
                                up.unit.MoveToNow(up.target);
                                toProcess.Remove(up);
                            }
                            else if (up.building != null)
                            {
                                up.building.waypoints = up.building.CalculatePath(up.target);
                                toProcess.Remove(up);
                            }
                        }
                        else break;
                        count++;
                        timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds - timeTaken;
                        // Console.Out.WriteLine(GameTimeManager.GetInstance().UpdateMSLeftThisFrame());
                    }
                    while (GameTimeManager.GetInstance().UpdateMSLeftThisFrame() > timeTaken);
                }

                // if (count > 0) Console.Out.WriteLine("Paths processed: " + count);
            }
        }


        public static PathfindingProcessor GetInstance()
        {
            if (instance == null) instance = new PathfindingProcessor();
            return instance;
        }

        /// <summary>
        /// Checks if this object is already in the queue for processing.
        /// </summary>
        /// <param name="unit">The object to check.</param>
        /// <returns>True or false.</returns>
        public Boolean AlreadyInQueue(Object obj)
        {
            for (int i = 0; i < this.toProcess.Count; i++)
            {
                if (obj is Unit)
                {
                    if (this.toProcess.ElementAt(i).unit == obj) return true;
                }
                else if (obj is Building)
                {
                    if (this.toProcess.ElementAt(i).building == obj) return true;
                }
            }
            return false;
        }

        private PathfindingProcessor()
        {
            toProcess = new LinkedList<ObjectProcess>();
        }

        private struct ObjectProcess
        {
            public Unit unit;
            public Building building;
            public Point target;

            public ObjectProcess(Object obj, Point target)
            {
                if (obj is Unit)
                {
                    unit = (Unit)obj;
                    building = null;
                }
                else
                {
                    building = (Building)obj;
                    unit = null;
                }
                this.target = target;
            }
        }
    }
}
