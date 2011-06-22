using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Pathfinding;
using Microsoft.Xna.Framework;
using AStarCollisionMap.Collision;
using PathfindingTest.Players;
using PathfindingTest.Units;
using AStarCollisionMap.Pathfinding;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingTest.Collision
{
    public class RTSCollisionMap : CollisionMap
    {
        /// <summary>
        /// Checks whether you can place the rectangle on the map. You can of course still update the collision map
        /// despite the outcome of this function.
        /// </summary>
        /// <param name="rect">The rect you want to update</param>
        /// <returns>The flag.</returns>
        public Boolean CanPlace(Rectangle rect)
        {
            double x_step = rect.Width / 10;
            double y_step = rect.Height / 10;
            // Collision with collisionmap
            for (double x = rect.Left; x < rect.Right; x += x_step)
            {
                for (double y = rect.Top; y < rect.Bottom; y += y_step)
                {
                    if (CollisionAt(PointToIndex((int)Math.Floor(x), (int)Math.Floor(y)))) return false;
                }
            }
            // Collision with units
            foreach (Player player in Game1.GetInstance().players)
            {
                foreach (Unit unit in player.units)
                {
                    if (rect.Contains(unit.GetLocation())) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the collisionmap, adding the rectangle to the collisionmap, and performing new connections, as for placing nodes 
        /// </summary>
        /// <param name="rect"></param>
        public BuildingMesh PlaceBuilding(Rectangle rect)
        {
            CollisionChangedEvent e = new CollisionChangedEvent();
            e.collision = this;
            e.changedRect = rect;
            e.collisionAdded = true;



            long ticks = DateTime.UtcNow.Ticks;
            e.oldData = (Boolean[])data.Clone();
            Console.Out.WriteLine("Clone time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");


            ticks = DateTime.UtcNow.Ticks;
            Boolean color = true;
            // Update collisionmap data
            for (int i = rect.Left; i < rect.Right; i++)
            {
                for (int j = rect.Top; j < rect.Bottom; j++)
                {
                    data[PointToIndex(i, j)] = color;
                }
            }
            // Update collisionmap texture
            this.texture = BoolToTexture(Game1.GetInstance().graphics.GraphicsDevice, this.data, mapWidth, collisionMapTextureScale);
            e.newData = data;
            Console.Out.WriteLine("Data put time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");


            ticks = DateTime.UtcNow.Ticks;
            Node[] newNodes = new Node[4];
            RTSCollisionMap map = Game1.GetInstance().collision;
            newNodes[0] = new Node(map, rect.Left - 1, rect.Top - 1, true);
            newNodes[1] = new Node(map, rect.Right + 1, rect.Top - 1, true);
            newNodes[2] = new Node(map, rect.Left - 1, rect.Bottom + 1, true);
            newNodes[3] = new Node(map, rect.Right + 1, rect.Bottom + 1, true);
            Console.Out.WriteLine("Node create time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");


            ticks = DateTime.UtcNow.Ticks;
            BuildingMesh mesh = new BuildingMesh(this);
            mesh.rect = rect;
            mesh.createdNodes = newNodes;
            LinkedList<PathfindingNode> nodes = PathfindingNodeManager.GetInstance().nodeList;
            foreach (PathfindingNode node in nodes)
            {
                if (rect.Contains(node.GetLocation())) mesh.removedNodes.AddLast(node.GetLocation());
            }
            Console.Out.WriteLine("Mesh create time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");

            ticks = DateTime.UtcNow.Ticks;
            LinkedList<Node> processedNodes = new LinkedList<Node>();
            int nodesAdded = 0;
            foreach (Node newNode in newNodes)
            {
                foreach (Node connectedNode in newNode.GetConnectedNodes())
                {
                    // Let's not process things twice, as it's quite a heavy computation
                    if (!processedNodes.Contains(connectedNode))
                    {
                        // Remove its current nodes
                        connectedNode.RemoveAllConnections();
                        // Scedule it for reprocessing
                        SmartPathfindingNodeProcessor.GetInstance().Push(connectedNode);
                        processedNodes.AddLast(connectedNode);
                        nodesAdded++;
                    }
                }
            }

            Console.Out.WriteLine("Node connection time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms, adding " + nodesAdded + " nodes");

            ticks = DateTime.UtcNow.Ticks;
            FireCollisionChangedEvent(e);
            Console.Out.WriteLine("Event time: " + (DateTime.UtcNow.Ticks - ticks));
            return mesh;
        }

        /// <summary>
        /// Draws this map on the screen.
        /// </summary>
        /// <param name="sb"></param>
        public void DrawMap(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle(0, 0, mapWidth, mapHeight), new Color(255, 255, 255, 63));
            //sb.Draw(collisionMap, new Rectangle(0, 0, screenWidth, screenHeight), new Color(255, 255, 255, 63));
            // CheckCollisionBetween(new Point(0, 0), new Point(100, 100));
        }


        /// <summary>
        /// Places nodes around collision edges
        /// </summary>
        public void PlaceNodesAroundEdges(){
            LinkedList<Point> pointList = this.GetNodeLocationsAroundEdges();
            foreach (Point p in pointList)
            {
                new Node(Game1.GetInstance().collision, p.X, p.Y);
            }
        }

        public RTSCollisionMap(Game game, int width, int height)
            : base(game, width, height)
        {

        }
    }
}
