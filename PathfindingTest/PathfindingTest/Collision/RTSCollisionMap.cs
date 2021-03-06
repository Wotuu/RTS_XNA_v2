﻿using System;
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
using CustomLists.Lists;

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
            for( int i = 0; i < Game1.GetInstance().players.Count(); i++ ){
                Player player = Game1.GetInstance().players.ElementAt(i);
                for( int j = 0; j < player.units.Count(); j++ ){
                    Unit unit = player.units.ElementAt(j);
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
            e.collisionMap = this;
            e.changedRect = rect;
            e.collisionAdded = true;

            long ticks = DateTime.UtcNow.Ticks;
            this.UpdateCollisionMap(rect, true);
            Console.Out.WriteLine("Data put time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");


            ticks = DateTime.UtcNow.Ticks;
            Node[] newNodes = new Node[4];
            RTSCollisionMap map = Game1.GetInstance().map.collisionMap;
            newNodes[0] = new Node(map, rect.Left - 1, rect.Top - 1, false);
            newNodes[1] = new Node(map, rect.Right + 1, rect.Top - 1, false);
            newNodes[2] = new Node(map, rect.Left - 1, rect.Bottom + 1, false);
            newNodes[3] = new Node(map, rect.Right + 1, rect.Bottom + 1, false);
            Console.Out.WriteLine("Node create time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");


            ticks = DateTime.UtcNow.Ticks;
            BuildingMesh mesh = new BuildingMesh(this);
            mesh.rect = rect;
            mesh.createdNodes = newNodes;
            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
            for (int i = 0; i < manager.GetNodeCount(); i++)
            {
                Node node = (Node)manager.GetNodeAt(i);
                if (rect.Contains(node.GetLocation())) mesh.removedNodes.AddLast(node.GetLocation());
            }
            Console.Out.WriteLine("Mesh create time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms");
            /*
            ticks = DateTime.UtcNow.Ticks;
            CustomArrayList<Node> processedNodes = new CustomArrayList<Node>();
            int nodesAdded = 0;
            foreach (Node newNode in newNodes)
            {
            }

            Console.Out.WriteLine("Node connection time: " + (DateTime.UtcNow.Ticks - ticks) / 10000 + "ms, adding " + nodesAdded + " nodes");
            */
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
            this.tree.Draw(sb);
            //sb.Draw(collisionMap, new Rectangle(0, 0, screenWidth, screenHeight), new Color(255, 255, 255, 63));
            // CheckCollisionBetween(new Point(0, 0), new Point(100, 100));
        }


        /// <summary>
        /// Places nodes around collision edges
        /// </summary>
        public void PlaceNodesAroundEdges(){
            CustomArrayList<Point> pointList = this.GetNodeLocationsAroundEdges();
            for( int i = 0; i < pointList.Count(); i++ ){
                Point p = pointList.ElementAt(i);
                new Node(Game1.GetInstance().map.collisionMap, p.X, p.Y);
            }
        }

        public RTSCollisionMap(GraphicsDevice device, int width, int height, int quadDepth)
            : base(device, width, height, true, quadDepth)
        {

        }

        public RTSCollisionMap(Game game, int width, int height, String collisionMapPath, String collisionMapName)
            : base(game, width, height, collisionMapPath, collisionMapName, false, 1)
        {

        }
    }
}
