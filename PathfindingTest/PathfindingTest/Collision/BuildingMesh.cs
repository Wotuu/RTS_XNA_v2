using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Pathfinding;
using Microsoft.Xna.Framework;
using PathfindingTest.Interfaces;
using AStarCollisionMap.Collision;
using AStarCollisionMap.Pathfinding;

namespace PathfindingTest.Collision
{
    public class BuildingMesh
    {
        /// <summary>
        /// The collisionmap that this Mesh was applied on.
        /// </summary>
        public RTSCollisionMap collision { get; set; }

        /// <summary>
        /// The nodes that were created 
        /// </summary>
        public Node[] createdNodes { get; set; }

        /// <summary>
        /// Contains points of nodes that were removed by the placement of this 
        /// </summary>
        public LinkedList<Point> removedNodes { get; set; }

        /// <summary>
        /// The rectangle that was used to update the collisionmap
        /// </summary>
        public Rectangle rect { get; set; }

        public void Reverse()
        {
            CollisionChangedEvent e = new CollisionChangedEvent();
            e.collision = collision;
            e.collisionAdded = false;
            e.changedRect = this.rect;
            e.oldData = (Boolean[])collision.data.Clone();

            Boolean color = false;
            for (int i = rect.Left; i < rect.Right; i++)
            {
                for (int j = rect.Top; j < rect.Bottom; j++)
                {
                    collision.data[collision.PointToIndex(i, j)] = color;
                }
            }
            collision.texture = collision.BoolToTexture(Game1.GetInstance().GraphicsDevice, collision.data, collision.mapWidth, collision.collisionMapTextureScale);
            e.newData = collision.data;

            LinkedList<Node> processedNodes = new LinkedList<Node>();
            foreach (Node node in createdNodes)
            {
                foreach (Node connectedNode in node.GetConnectedNodes())
                {
                    if (!processedNodes.Contains(connectedNode))
                    {
                        SmartPathfindingNodeProcessor.GetInstance().Push(connectedNode);
                        processedNodes.AddLast(connectedNode);
                    }
                }
                node.Destroy();
            }

            foreach (Point p in removedNodes)
            {
                // Console.Out.WriteLine("Restoring node " + p);
                new Node(collision, p.X, p.Y);
            }

            collision.FireCollisionChangedEvent(e);
        }


        public BuildingMesh(RTSCollisionMap collision)
        {
            this.collision = collision;
            createdNodes = new Node[4];
            removedNodes = new LinkedList<Point>();
        }
    }
}
