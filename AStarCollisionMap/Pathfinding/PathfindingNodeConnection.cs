using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStarCollisionMap.Pathfinding
{
    public class PathfindingNodeConnection
    {
        public PathfindingNode node1 { get; set; }
        public PathfindingNode node2 { get; set; }

        public PathfindingNodeConnection(PathfindingNode node1, PathfindingNode node2)
        {
            this.node1 = node1;
            this.node2 = node2;
            /*PathfindingNode closestNode = null;
            if (Util.GetHypoteneuseLength(node1.GetLocation(), new Point(0, 0)) < Util.GetHypoteneuseLength(node2.GetLocation(), new Point(0, 0)))
            {
                closestNode = node1;
            } else closestNode = node2;
            lengthDrawOffset = new Point(closestNode.x + (int)Math.Abs(node1.x - node2.x), closestNode.y + (int)Math.Abs(node1.y - node2.y));*/
        }
    }
}
