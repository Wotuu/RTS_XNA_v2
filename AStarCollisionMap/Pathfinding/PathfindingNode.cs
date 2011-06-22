using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AStarCollisionMap.Collision;

namespace AStarCollisionMap.Pathfinding
{
    public abstract class PathfindingNode : OnCollisionChangedListener
    {
        /// <summary>
        /// The location of this node
        /// </summary>
        public int x { get; set; }
        public int y { get; set; }
        /// Variable to determine scoring for the AStar pathfinding algorithm
        public int score { get; set; }
        /// The states this node can be.
        public static int MAX_CONNECT_RANGE = 600;

        /// The parent of this class (used in the algorithm).
        public PathfindingNode parent { get; set; }

        public CollisionMap collisionMap { get; set; }

        /// <summary>
        ///  Costs
        /// </summary>
        public int costToStart { get; set; }
        public int costToEnd { get; set; }

        public LinkedList<PathfindingNodeConnection> connections { get; set; }

        /// <summary>
        /// Completely remove this node.
        /// </summary>
        public void Destroy()
        {
            RemoveAllConnections();
            PathfindingNodeManager.GetInstance().nodeList.Remove(this);
            PathfindingNodeProcessor.GetInstance().Remove(this);
            collisionMap.collisionChangedListeners -= ((OnCollisionChangedListener)this).OnCollisionChanged;
        }

        /// <summary>
        /// Destroys this node, removing all connections from everywhere!
        /// </summary>
        public void RemoveAllConnections()
        {
            foreach (PathfindingNode node in PathfindingNodeManager.GetInstance().nodeList)
            {
                node.RemoveConnection(this);
            }
        }

        /// <summary>
        /// Whether or not this node is connected to the parameter node.
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <returns>The connection, or null otherwise</returns>
        public PathfindingNodeConnection IsConnected(PathfindingNode node) {
            foreach (PathfindingNodeConnection conn in connections) {
                if (conn.node1 == node || conn.node2 == node) {
                    return conn;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes a connection.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void RemoveConnection(PathfindingNode node) {
            // Can't use foreach here, because you're removing elements
            for (int i = 0; i < connections.Count; i++)
            {
                PathfindingNodeConnection conn = connections.ElementAt(i);
                if (conn.node1 == node || conn.node2 == node)
                {
                    connections.Remove(conn);
                    node.RemoveConnection(this);
                    i--;
                }
            }
        }

        /// <summary>
        /// Gets the connected nodes 
        /// </summary>
        /// <returns></returns>
        public LinkedList<PathfindingNode> GetConnectedNodes()
        {
            LinkedList<PathfindingNode> list = new LinkedList<PathfindingNode>();
            foreach (PathfindingNodeConnection conn in connections) {
                if (conn.node1 != this) list.AddLast(conn.node1);
                else if (conn.node2 != this) list.AddLast(conn.node2);
            }
            return list;
        }

        /// <summary>
        /// Gets the point containing the x and y of this node.
        /// </summary>
        /// <returns>The point.</returns>
        public Point GetLocation() {
            return new Point(x, y);
        }

        /**
        * Get the score for this node.
        * @param end The node to calculate to.
        * @return The score.
        */
        public int CalculateScore(Point end) {
            return (int)PathfindingUtil.GetHypoteneuseLength(this.GetLocation(), end);
        }

        public LinkedList<PathfindingNode> BuildPath(PathfindingNode start)
        {
            LinkedList<PathfindingNode> path = new LinkedList<PathfindingNode>();
            int count = 0;
            PathfindingNode currentParent = this;
            while (currentParent != null)
            {
                path.AddFirst(currentParent);
                if (currentParent != start) {
                    currentParent = currentParent.parent;
                    count++;
                    if (count > 1000) {
                        Console.Out.WriteLine("Breaking @ Building path, something went wrong probably.");
                        break;
                    }
                }
                else {
                    break;
                }
            }
            /*LinkedList<PathfindingNode> reversedPath = new LinkedList<PathfindingNode>();
            foreach (PathfindingNode node in path)
            {
                reversedPath.AddFirst(node);
            }*/
            return path;
        }

        /// <summary>
        /// Creates the connections between this node and other nodes!
        /// </summary>
        public void CreateConnections()
        {
            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
            foreach (PathfindingNode node in manager.nodeList)
            {
                // No connection with itsself
                if (node == this) continue;
                if (PathfindingUtil.GetHypoteneuseLength(node.GetLocation(), this.GetLocation()) > MAX_CONNECT_RANGE) continue;
                if (!collisionMap.IsCollisionBetween(this.GetLocation(), node.GetLocation()))
                {
                    // Console.Out.WriteLine("Adding a new connection between " + this + " and " + node);
                    PathfindingNodeConnection conn = new PathfindingNodeConnection(this, node);
                    this.connections.AddLast(conn);
                    if (node.IsConnected(this) == null) node.connections.AddLast(conn);
                }
            }
        }

        void OnCollisionChangedListener.OnCollisionChanged(CollisionChangedEvent collisionEvent)
        {
            if (collisionEvent.collisionAdded)
            {
                if (collisionEvent.changedRect.Contains(this.GetLocation())) this.Destroy();
            }
        }

        public PathfindingNode(CollisionMap collisionMap)
        {
            this.collisionMap = collisionMap;
            connections = new LinkedList<PathfindingNodeConnection>();
            collisionMap.collisionChangedListeners += ((OnCollisionChangedListener)this).OnCollisionChanged;
        }
    }
}
