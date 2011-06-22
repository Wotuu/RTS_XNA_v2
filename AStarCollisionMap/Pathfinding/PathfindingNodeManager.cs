using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStarCollisionMap.Pathfinding
{
    public class PathfindingNodeManager
    {
        private static PathfindingNodeManager instance;

        public LinkedList<PathfindingNode> nodeList { get; set; }

        private PathfindingNode _selectedNode;
        public PathfindingNode selectedNode
        { 
            get {
                return _selectedNode;
            }
            set
            {
                this._selectedNode = value;
            }
        }

        /// <summary>
        /// Gets the instance of this PathfindingNodeManager.
        /// </summary>
        /// <returns>The PathfindingNodeManager.</returns>
        public static PathfindingNodeManager GetInstance()
        {
            if (instance == null) instance = new PathfindingNodeManager();
            return instance;
        }

        private PathfindingNodeManager()
        {
            // Console.Out.WriteLine("Making a new PathfindingNodeManager");
            this.nodeList = new LinkedList<PathfindingNode>();
        }
    }
}
