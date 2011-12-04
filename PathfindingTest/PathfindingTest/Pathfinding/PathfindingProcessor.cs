using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using Microsoft.Xna.Framework;
using PathfindingTest.State;
using PathfindingTest.Buildings;
using System.Threading;

namespace PathfindingTest.Pathfinding
{
    public class PathfindingProcessor
    {
        private static PathfindingProcessor instance { get; set; }
        private LinkedList<ObjectProcess> toProcess { get; set; }
        private readonly object listLock = new object();
        public Boolean running { get; set; }

        /// <summary>
        /// Pushes an object onto the list.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <param name="target">The target of the unit</param>
        public void Push(Object obj, Point target)
        {
            lock (listLock)
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
                lock (listLock)
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
                            lock (listLock) { toProcess.Remove(up); }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process the queue.
        /// </summary>
        public void Process()
        {
            running = true;
            while (running)
            {
                if (SmartPathfindingNodeProcessor.GetInstance().GetCount() == 0)
                {
                    if (toProcess.Count != 0)
                    {
                        lock (listLock)
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
                    }
                    else Thread.Sleep(10);
                }
                else Thread.Sleep(10);
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
            lock (this.listLock)
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
            }
            return false;
        }

        private PathfindingProcessor()
        {
            toProcess = new LinkedList<ObjectProcess>();
            Thread t = new Thread(this.Process);
            t.IsBackground = true;
            t.Start();
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
