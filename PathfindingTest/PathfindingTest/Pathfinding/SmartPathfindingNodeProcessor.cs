using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStarCollisionMap.Pathfinding;
using PathfindingTest.State;

namespace PathfindingTest.Pathfinding
{
    public class SmartPathfindingNodeProcessor : PathfindingNodeProcessor
    {
        private SmartPathfindingNodeProcessor() { }

        public static new SmartPathfindingNodeProcessor GetInstance()
        {
            if (instance == null) instance = new SmartPathfindingNodeProcessor();
            return (SmartPathfindingNodeProcessor)instance;
        }

        public new void Process()
        {
            double timeTaken = 0;
            int count = 0;
            do
            {
                timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;

                if (toProcess.Count > 0)
                {
                    PathfindingNode pop = toProcess.ElementAt(0);
                    pop.CreateConnections();
                    toProcess.RemoveFirst();
                }
                else break;
                count++;
                timeTaken = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds - timeTaken;
                // Console.Out.WriteLine(GameTimeManager.GetInstance().UpdateMSLeftThisFrame());
            }
            while (GameTimeManager.GetInstance().UpdateMSLeftThisFrame() > timeTaken);
        }
    }
}
