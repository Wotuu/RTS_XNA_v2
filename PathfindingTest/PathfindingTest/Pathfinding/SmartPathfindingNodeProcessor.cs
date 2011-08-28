using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStarCollisionMap.Pathfinding;
using PathfindingTest.State;
using System.Threading;

namespace PathfindingTest.Pathfinding
{
    public class SmartPathfindingNodeProcessor : PathfindingNodeProcessor
    {
        public Boolean running { get; set; }

        private SmartPathfindingNodeProcessor() {
        }

        /// <summary>
        /// Spawns a new thread to start processing.
        /// </summary>
        public void StartThread()
        {
            new Thread(this.Process).Start();
        }

        public static new SmartPathfindingNodeProcessor GetInstance()
        {
            if (instance == null) instance = new SmartPathfindingNodeProcessor();
            return (SmartPathfindingNodeProcessor)instance;
        }

        public new void Process()
        {/*
            double timeTaken = 0;
            int count = 0;
            do
            {
                timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
*/          running = true;
            while( running ) {
                try
                {
                    while (toProcess.Count() > 0)
                    {
                        PathfindingNode pop = toProcess.ElementAt(0);
                        pop.CreateConnections();
                        toProcess.RemoveFirst();
                    }
                }
                catch (Exception e) { } 
                Thread.Sleep(5);
            }/*
                else break;
                count++;
                timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds - timeTaken;
                // Console.Out.WriteLine(GameTimeManager.GetInstance().UpdateMSLeftThisFrame());
            }
            while (GameTimeManager.GetInstance().UpdateMSLeftThisFrame() > timeTaken);*/
        }

        public int GetCount()
        {
            return this.toProcess.Count();
        }
    }
}
