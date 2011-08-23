using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AStarCollisionMap.Collision;
using AStarCollisionMap.Pathfinding;

public delegate void OnConnectionsCreated(PathfindingNode source);

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
        public readonly object connectionSyncLock = new object();

        public OnConnectionsCreated onConnectionsCreatedListeners { get; set; }

        public Boolean initialised { get; set; }

        /// <summary>
        /// Completely remove this node.
        /// </summary>
        public void Destroy()
        {
            RemoveAllConnections();
            PathfindingNodeManager.GetInstance().RemoveNode(this);
            PathfindingNodeProcessor.GetInstance().Remove(this);
            collisionMap.collisionChangedListeners -= ((OnCollisionChangedListener)this).OnCollisionChanged;
        }

        /// <summary>
        /// Destroys this node, removing all connections from everywhere!
        /// </summary>
        public void RemoveAllConnections()
        {
            for (int i = 0; i < PathfindingNodeManager.GetInstance().GetNodeCount(); i++ )
            {
                PathfindingNodeManager.GetInstance().GetNodeAt(i).RemoveConnection(this);
            }
        }


        /// <summary>
        /// Whether or not this node is connected to the parameter node.
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <returns>The connection, or null otherwise</returns>
        public PathfindingNodeConnection IsConnected(PathfindingNode node) {

            lock (this.connectionSyncLock)
            {
                foreach (PathfindingNodeConnection conn in connections)
                {
                    if (conn.node1 == node || conn.node2 == node)
                    {
                        return conn;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Removes a connection.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void RemoveConnection(PathfindingNode node) {
            lock (this.connectionSyncLock)
            {
                try
                {
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
                catch (Exception e) { }
            }
        }

        /// <summary>
        /// Gets the connected nodes 
        /// </summary>
        /// <returns></returns>
        public LinkedList<PathfindingNode> GetConnectedNodes()
        {
            LinkedList<PathfindingNode> list = new LinkedList<PathfindingNode>();
            lock (this.connectionSyncLock)
            {
                foreach (PathfindingNodeConnection conn in this.connections)
                {
                    if (conn.node1 != this) list.AddLast(conn.node1);
                    else if (conn.node2 != this) list.AddLast(conn.node2);
                }
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
        /// Creates the connections with a max range (default is 600)
        /// </summary>
        /// <param name="maxRange">The new max range</param>
        public void CreateConnections(int maxRange)
        {
            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
            // Power it, so we don't have to root the distances that we have to calculate, saving a massive performance hit
            maxRange = (int)Math.Pow(maxRange, 2);
            for (int i = 0; i < manager.GetNodeCount(); i++)
            {
                PathfindingNode node = manager.GetNodeAt(i);
                // No connection with itsself
                if (node == this) continue;
                if (PathfindingUtil.GetSquaredHypoteneuseLength(node.GetLocation(), this.GetLocation()) > maxRange) continue;
                if (!collisionMap.IsCollisionBetween(this.GetLocation(), node.GetLocation()))
                {
                    lock (connectionSyncLock)
                    {
                        // Console.Out.WriteLine("Adding a new connection between " + this + " and " + node);
                        PathfindingNodeConnection conn = new PathfindingNodeConnection(this, node);
                        this.connections.AddLast(conn);
                        if (node.IsConnected(this) == null) node.connections.AddLast(conn);
                    }
                }
            }

            if (this.onConnectionsCreatedListeners != null) this.onConnectionsCreatedListeners(this);
        }

        /// <summary>
        /// Creates the connections between this node and other nodes!
        /// </summary>
        public void CreateConnections()
        {
            this.CreateConnections(MAX_CONNECT_RANGE);
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
