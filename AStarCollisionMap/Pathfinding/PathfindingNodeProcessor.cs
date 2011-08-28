using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLists.Lists;

namespace AStarCollisionMap.Pathfinding
{
    public class PathfindingNodeProcessor
    {
        protected CustomArrayList<PathfindingNode> toProcess { get; set; }
        protected static PathfindingNodeProcessor instance { get; set; }

        /// <summary>
        /// Removes a node from the queue.
        /// </summary>
        /// <param name="node"></param>
        public void Remove(PathfindingNode node)
        {
            toProcess.Remove(node);
        }

        /// <summary>
        /// Pushes a node onto the list.
        /// </summary>
        /// <param name="node">The node to push.</param>
        public void Push(PathfindingNode node)
        {
            if( !toProcess.Contains(node) ) toProcess.AddLast(node);
        }

        /// <summary>
        /// Processes an item in the list
        /// </summary>
        public void Process()
        {
            if (toProcess.Count() > 0)
            {
                PathfindingNode pop = toProcess.ElementAt(0);
                //lock (pop.connections)
                //{
                pop.CreateConnections();
                toProcess.RemoveFirst();
                //}
            }
        }

        private void FinishedProcessing()
        {

        }

        public static PathfindingNodeProcessor GetInstance()
        {
            if (instance == null) instance = new PathfindingNodeProcessor();
            return instance;
        }

        protected PathfindingNodeProcessor()
        {
            toProcess = new CustomArrayList<PathfindingNode>();
        }
    }
}
