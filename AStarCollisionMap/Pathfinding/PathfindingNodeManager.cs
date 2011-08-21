using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStarCollisionMap.Pathfinding
{
    public class PathfindingNodeManager
    {
        private static PathfindingNodeManager instance;

        private LinkedList<PathfindingNode> nodeList { get; set; }

        public readonly object nodeLock = new object();

        /// <summary>
        ///  Clears the nodes
        /// </summary>
        public void ClearNodes()
        {
            lock (nodeLock)
            {
                this.nodeList.Clear();
            }
        }

        /// <summary>
        /// Adds a node to the list.
        /// </summary>
        /// <param name="node">The node</param>
        public void AddNode(PathfindingNode node)
        {
            lock (nodeLock)
            {
                this.nodeList.AddLast(node);
            }
        }

        /// <summary>
        /// Gets a node at a position in the array.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public PathfindingNode GetNodeAt(int i)
        {
            lock (nodeLock)
            {
                return nodeList.ElementAt(i);
            }
        }

        /// <summary>
        /// Removes a node
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(PathfindingNode node)
        {
            lock (nodeLock)
            {
                this.nodeList.Remove(node);
            }
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <returns>The count</returns>
        public int GetNodeCount()
        {
            return this.nodeList.Count;
        }

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
