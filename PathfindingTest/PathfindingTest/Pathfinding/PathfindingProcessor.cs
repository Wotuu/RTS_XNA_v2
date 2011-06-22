using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using Microsoft.Xna.Framework;
using PathfindingTest.State;

namespace PathfindingTest.Pathfinding
{
    public class PathfindingProcessor
    {
        private static PathfindingProcessor instance { get; set; }
        private LinkedList<UnitProcess> toProcess { get; set; }
        private readonly object queueLock = new object();

        /// <summary>
        /// Pushes a unit onto the list.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <param name="target">The target of the unit</param>
        public void Push(Unit unit, Point target)
        {
            lock (queueLock)
            {
                //Console.Out.WriteLine("Adding a unit to find it's path: \n" +
                //    "StackTrace: '{0}'", Environment.StackTrace);
                toProcess.AddLast(new UnitProcess(unit, target));
            }
        }

        /// <summary>
        /// Removes a unit from the process list.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void Remove(Unit unit)
        {
            for (int i = 0; i < toProcess.Count; i++)
            {
                UnitProcess up = toProcess.ElementAt(i);
                if (up.unit == unit)
                {
                    toProcess.Remove(up);
                    break;
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
                double timeTaken = 0;
                int count = 0;
                do
                {
                    timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
                    if (toProcess.Count != 0)
                    {
                        UnitProcess up = toProcess.ElementAt(0);
                        up.unit.MoveToNow(up.target);
                        toProcess.Remove(up);
                    }
                    else break;
                    count++;
                    timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds - timeTaken;
                    // Console.Out.WriteLine(GameTimeManager.GetInstance().UpdateMSLeftThisFrame());
                }
                while (GameTimeManager.GetInstance().UpdateMSLeftThisFrame() > timeTaken);
                // if (count > 0) Console.Out.WriteLine("Paths processed: " + count);
            }
        }


        public static PathfindingProcessor GetInstance()
        {
            if (instance == null) instance = new PathfindingProcessor();
            return instance;
        }

        /// <summary>
        /// Checks if this unit is already in the queue for processing.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <returns>True or false.</returns>
        public Boolean AlreadyInQueue(Unit unit)
        {
            for (int i = 0; i < this.toProcess.Count; i++)
            {
                if (this.toProcess.ElementAt(i).unit == unit) return true;
            }
            return false;
        }

        private PathfindingProcessor()
        {
            toProcess = new LinkedList<UnitProcess>();
        }

        private struct UnitProcess
        {
            public Unit unit;
            public Point target;

            public UnitProcess(Unit unit, Point target)
            {
                this.unit = unit;
                this.target = target;
            }
        }
    }
}
