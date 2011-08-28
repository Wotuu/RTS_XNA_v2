using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Pathfinding;
using Microsoft.Xna.Framework;
using PathfindingTest.Interfaces;
using AStarCollisionMap.Collision;
using AStarCollisionMap.Pathfinding;
using CustomLists.Lists;

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
        public CustomArrayList<Point> removedNodes { get; set; }

        /// <summary>
        /// The rectangle that was used to update the collisionmap
        /// </summary>
        public Rectangle rect { get; set; }

        public void Reverse()
        {
            CollisionChangedEvent e = new CollisionChangedEvent();
            e.collisionMap = collision;
            e.collisionAdded = false;
            e.changedRect = this.rect;

            foreach (CollisionChangedEvent.QuadPart part in e.changedQuads)
            {
                part.quad.collisionTexture.UpdateCollision(part.rectangle, false);
            }

            CustomArrayList<Node> processedNodes = new CustomArrayList<Node>();
            foreach (Node node in createdNodes)
            {
                CustomArrayList<PathfindingNode> connectedNodes = node.GetConnectedNodes();
                for( int i = 0; i < connectedNodes.Count(); i++ ){
                    Node connectedNode = (Node)connectedNodes.ElementAt(i);
                    if (!processedNodes.Contains(connectedNode))
                    {
                        SmartPathfindingNodeProcessor.GetInstance().Push(connectedNode);
                        processedNodes.AddLast(connectedNode);
                    }
                }
                node.Destroy();
            }
            for( int i = 0; i < this.removedNodes.Count(); i++ ){
                Point p = this.removedNodes.ElementAt(i);
                // Console.Out.WriteLine("Restoring node " + p);
                new Node(collision, p.X, p.Y);
            }

            collision.FireCollisionChangedEvent(e);
        }


        public BuildingMesh(RTSCollisionMap collision)
        {
            this.collision = collision;
            createdNodes = new Node[4];
            removedNodes = new CustomArrayList<Point>();
        }
    }
}
