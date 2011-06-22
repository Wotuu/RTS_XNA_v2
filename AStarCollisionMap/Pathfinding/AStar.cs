using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AStarCollisionMap.Pathfinding
{
    public class AStar
    {
        /// <summary>
        /// Ignores all blocked squares in the algorithm.
        /// </summary>
        public const int MODE_IGNORE_BLOCKED = 1;
  
        /// <summary>
        /// Ignores only buildings in the algorithm.
        /// </summary>
        public const int MODE_IGNORE_BUILDINGS = 2; 
        
        /// <summary>
        /// Allows everything in the algorithm (all squares are available) 
        /// </summary>
        public const int MODE_ALLOW_ALL = 3; 

        /// <summary>
        ///  Ignores both buildings and blocked squares. 
        /// </summary>
        public const int MODE_NORMAL = 4;

  
        private PathfindingNode start {get; set; }
        private PathfindingNode end {get; set; }

        private LinkedList<PathfindingNode> closedList { get; set; }
        private LinkedList<PathfindingNode> openList { get; set; }

        public AStar(PathfindingNode start, PathfindingNode end)
        {
            this.start = start;
            this.end = end;
            this.closedList = new LinkedList<PathfindingNode>();
            this.openList = new LinkedList<PathfindingNode>();
        }

        /**
         * Find the quickest path given the start and end node are set properly.<br>
         * Returns false if the start or end is null, or are not actual start or end points.<br>
         * @return The list containing all points that are required to be visited in order to reach the end.
         */
        public LinkedList<PathfindingNode> FindPath()
        {
            if (start == null || end == null)
            {
                return null;
            }
            openList.AddLast(start);

            while (openList.Count != 0 )
            {
                PathfindingNode node = null;
                foreach( PathfindingNode openNode in openList )
                {
                    if (node == null || node.score > openNode.score) node = openNode;
                }
                if (node == end) return node.BuildPath(start); 

                openList.Remove(node);
                closedList.AddLast(node);


                foreach (PathfindingNode currentSuccessor in node.GetConnectedNodes())
                {
                    if( closedList.Contains( currentSuccessor ) ) continue;
                    int currentClosestDistance = node.costToStart + (int)PathfindingUtil.GetHypoteneuseLength(currentSuccessor.GetLocation(), node.GetLocation());
                    Boolean better = false;

                    if( !openList.Contains( currentSuccessor ) ){
                        openList.AddLast(currentSuccessor);
                        better = true;
                    } else if( currentClosestDistance < currentSuccessor.costToStart ){
                        better = true;
                    }
 
                    if( better ) {
                        currentSuccessor.parent = node;
                        currentSuccessor.costToStart = currentClosestDistance;
                        currentSuccessor.costToEnd = currentSuccessor.CalculateScore(end.GetLocation());
                        currentSuccessor.score = currentSuccessor.costToStart + currentSuccessor.costToEnd;
                    }
                }
            }
            return null;
        }
    }
}
