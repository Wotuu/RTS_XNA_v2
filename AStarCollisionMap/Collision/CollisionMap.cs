using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AStarCollisionMap.Collision;
using AStarCollisionMap.Pathfinding;

public delegate void OnCollisionChanged(CollisionChangedEvent c_event);

namespace AStarCollisionMap.Collision
{
    public class CollisionMap
    {
        public Texture2D texture { get; set; }
        public int mapWidth { get; set; }
        public int mapHeight { get; set; }
        public int collisionMapTextureScale = 1;

        /// <summary>
        /// The data of this collisionmap. DO NOT MODIFY. USE UpdateCollisionMap(); instead!
        /// </summary>
        public Boolean[] data { get; set; }

        private const int VERTICAL_COLLISION_CHECK_SPACING = 50, HORIZONTAL_COLLISION_CHECK_SPACING = 50,
            NODE_REMOVE_DISTANCE = 25;

        public OnCollisionChanged collisionChangedListeners { get; set; }


        /// <summary>
        /// Converts a texture to a boolean array
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        private Boolean[] TextureToBoolean(Texture2D texture)
        {
            Boolean[] data = new Boolean[texture.Width * texture.Height];
            int[] intData = new int[texture.Width * texture.Height];
            texture.GetData(intData);
            for( int i = 0; i < intData.Length; i++ )
            {
                //if (i == 0) Console.Out.WriteLine(intData[i] + "");
                // 0 = no collision, 1 = collision
                data[i] = (intData[i] != 0);
                //if (i == 0) Console.Out.WriteLine(data[i] + "");
            }
            return data;
        }

        /// <summary>
        /// Converts a boolean to a texture.
        /// </summary>
        /// <param name="device">The device to create the texture on</param>
        /// <param name="data">The data</param>
        /// <param name="width">the width</param>
        /// <param name="inversedScale">An invesed scale. 2 means, 2 times as small. 4 times as small, etc. USE IN MULTIPLES OF 2, OR FAIL.</param>
        /// <returns></returns>
        public Texture2D BoolToTexture(GraphicsDevice device, Boolean[] data, int width, int inversedScale)
        {
            if (inversedScale == 1) return BoolToTexture(device, data, width);
            int[] intData = new int[data.Length / ( inversedScale * inversedScale )];
            Texture2D texture = new Texture2D(device, width / inversedScale,
                ( data.Length / width) / inversedScale);


            int newPixelCount = 0;
            int skips = 0;
            for (int i = 0; i < data.Length - 1; i += inversedScale)
            {
                bool skipNext = false;
                if (i % (width * inversedScale) == 0) skipNext = true; 
                if (data[i]) intData[newPixelCount] = (int)Color.Black.PackedValue;
                else intData[newPixelCount] = 0; 

                newPixelCount++;
                if (skipNext)
                {
                    // Console.Out.Write("Skipping @ " + i + ", " + newPixelCount + ", " + skips);
                    i += width * (inversedScale - 1);
                    // Console.Out.WriteLine(" new: " + i);
                    skips++;
                    continue;
                }
            }
            // Console.Out.WriteLine("Finished, copied " + newPixelCount + " pixels, while the array size is " + intData.Length);
            texture.SetData(intData);
            return texture;
        }

        private Texture2D BoolToTexture(GraphicsDevice device, Boolean[] data, int width)
        {
            int[] intData = new int[data.Length];
            Texture2D texture = new Texture2D(device, width, data.Length / width);
            
            for( int i = 0; i < intData.Length; i++ )
            {
                if (data[i]) intData[i] = (int)Color.Black.PackedValue;
                else intData[i] = 0;
            }
            texture.SetData(intData);
            return texture;
        }



        /// <summary>
        /// Updates the collisionmap, without updating or adding pathfinding points. 
        /// DO NOT USE THIS METHOD IN-GAME, ONLY IN THE EDITOR.
        /// </summary>
        /// <param name="device">The graphics device to create the new texture on</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="add">Whether to add or to remove the rect.</param>
        public void UpdateCollisionMap(GraphicsDevice device, Rectangle rect, Boolean add)
        {
            CollisionChangedEvent e = new CollisionChangedEvent();
            e.collision = this;
            e.changedRect = rect;
            e.collisionAdded = add;


            e.oldData = (Boolean[])data.Clone();


            int pixelsChanged = 0;
            // Update collisionmap data
            for (int i = rect.Left; i < rect.Right; i++)
            {
                for (int j = rect.Top; j < rect.Bottom; j++)
                {
                    if (data[PointToIndex(i, j)] != add)
                    {
                        data[PointToIndex(i, j)] = add;
                        pixelsChanged++;
                    }
                }
            }
            if (pixelsChanged > 0)
            {
                // Update collisionmap texture
                this.texture = BoolToTexture(device, this.data, mapWidth, collisionMapTextureScale);
                e.newData = data;

                FireCollisionChangedEvent(e);
            }
        }


        
        /// <summary>
        /// Places nodes around all the edges
        /// </summary>
        public LinkedList<Point> GetNodeLocationsAroundEdges()
        {
            Boolean previous = data[0];
            LinkedList<Point> pointList = new LinkedList<Point>();
            for(int i = 0; i < data.Length - 1; i++)
            {
                if ((int)(i / mapWidth) % VERTICAL_COLLISION_CHECK_SPACING != 0) { continue; }
                if (i % mapWidth > mapWidth - 15 || i % mapWidth < 15) continue;
                if (data[i] != previous)
                {
                        if (!data[i] && (i + 10 < data.Length) && !data[i + 10]) pointList.AddLast(IndexToPoint(i + 10));
                        else if ((i - 10 > -1) && !data[i - 10]) pointList.AddLast(IndexToPoint(i - 10));
                }
                previous = data[i];
            }
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    int index = i + j * mapWidth;
                    if (index % mapWidth % HORIZONTAL_COLLISION_CHECK_SPACING != 0) { continue; }
                    // if (index % screenWidth > screenWidth - 15 || index % screenWidth < 15) continue;
                    if (data[index] != previous)
                    {
                        if (!data[index] && (index + (10 * mapWidth) < data.Length) 
                            && !data[index + (10 * mapWidth)])
                            pointList.AddLast(IndexToPoint(index + (10 * mapWidth)));
                        else if ((index - (10 * mapWidth) > -1) && !data[index - (10 * mapWidth)])
                            pointList.AddLast(IndexToPoint(index - (10 * mapWidth)));
                    }
                    previous = data[index];
                }
            }

            // Remove nodes that are closer than NODE_REMOVE_DISTANCE of each other
            for (int i = 0; i < pointList.Count; i++)
            {
                Point p1 = pointList.ElementAt(i);
                for (int j = 0; j < pointList.Count; j++)
                {
                    Point p2 = pointList.ElementAt(j);
                    if (p1 != p2 && PathfindingUtil.GetHypoteneuseLength(p1, p2) < NODE_REMOVE_DISTANCE)
                    {
                        pointList.Remove(p1);
                        i--; 
                        j--;
                        break;
                    }
                }
            }
            return pointList;
        }

        #region Convenient methods
        public Boolean IndexExists(Point p)
        {
            if (p.X < 0 || p.X >= mapWidth) return false;
            if (p.Y < 0 || p.Y >= mapHeight) return false;
            return true;
        }

        public Boolean IndexExists(int index)
        {
            if (index < 0 || index >= data.Length) return false;
            return true;
        }


        /// <summary>
        /// Returns the index of a certain point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The index.</returns>
        public int PointToIndex(int x, int y)
        {
            return x + y * mapWidth;
        }

        /// <summansry>
        /// Returns the index of a certain point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The index.</returns>
        public int PointToIndex(Point p)
        {
            return p.X + p.Y * mapWidth;
        }

        /// <summary>
        /// Converts an index to a point.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The point</returns>
        public Point IndexToPoint(int index)
        {
            return new Point(index % mapWidth, (int)(index / mapWidth));
        }

        /// <summary>
        /// Checks whether there is collision at an index.
        /// </summary>
        /// <param name="i">The index</param>
        /// <returns>The flag!</returns>
        public Boolean CollisionAt(int i)
        {
            if (IndexExists(i)) return data[i];
            else return true;
        }

        /// <summary>
        /// Checks whether there is collision at a point.
        /// </summary>
        /// <param name="p">The point to check.</param>
        /// <returns>The flag, yes or no.</returns>
        public Boolean CollisionAt(Point p) {
            if (IndexExists(p)) return data[PointToIndex(p)];
            else return true;
        }

        /// <summary>
        /// Gets the closest node in line of sight from the given point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The node, or null if no node is in range.</returns>
        public PathfindingNode ClosestNodeInLOS(Point p)
        {
            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
            LinkedList<PathfindingNode> inRangeNodes = new LinkedList<PathfindingNode>();
            foreach (PathfindingNode node in manager.nodeList)
            {
                if (this.IsCollisionBetween(p, node.GetLocation()))
                {
                    inRangeNodes.AddLast(node);
                }
            }
            PathfindingNode closestNode = inRangeNodes.ElementAt(0);
            double closestDistance = Double.MaxValue;
            foreach (PathfindingNode node in inRangeNodes)
            {
                double newDistance = PathfindingUtil.GetHypoteneuseLength(closestNode.GetLocation(), node.GetLocation());
                if (closestDistance > newDistance)
                {
                    closestDistance = newDistance;
                    closestNode = node;
                }
            }
            return closestNode;
        }

        /// <summary>
        /// Gets the pixels between points.
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <param name="spacing">The pixel spacing between the points (default should be 1). Lower than 1 will use 1.</param>
        /// <returns></returns>
        public Point[] PixelsBetweenPoints(Point p1, Point p2, int spacing) {
            if (spacing < 1) spacing = 1;
            int xDiff = p2.X - p1.X;
            int yDiff = p2.Y - p1.Y;

            double maxWidth = Math.Max(Math.Abs(xDiff) , Math.Abs(yDiff));

            double xDelta = (xDiff / maxWidth) * spacing;
            // Console.Out.WriteLine("xDelta: " + xDelta);
            double yDelta = (yDiff / maxWidth) * spacing;
            // Console.Out.WriteLine("yDelta: " + yDelta);

            LinkedList<Point> pointList = new LinkedList<Point>();
            Point currentPoint = p1;
            Point previousPoint = new Point(-100, -100);
            double currentX = p1.X, currentY = p1.Y;
            for( int i = 0; i < maxWidth / spacing; i++ ) {
                currentX += xDelta;
                currentY += yDelta;

                currentPoint.X = (int)currentX;
                currentPoint.Y = (int)currentY;

                Point newPoint = new Point( currentPoint.X, currentPoint.Y );
                if ( previousPoint != null && !previousPoint.Equals(newPoint) ) {
                    // Console.Out.WriteLine(i + ". Adding point to list: " + newPoint);
                    pointList.AddLast(newPoint);
                }
                previousPoint = newPoint;
            }
            return pointList.ToArray<Point>();
        }

        /// <summary>
        /// Checks whether there is collision between two points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>Yes if there was collision, false if there wasn't</returns>
        public Boolean IsCollisionBetween(Point p1, Point p2) {
            Point[] pixelsBetween = PixelsBetweenPoints(p1, p2, 4);
            foreach (Point p in pixelsBetween) {
                if (CollisionAt(p)) return true; 
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Fires a collision changed event to all listeners.
        /// </summary>
        /// <param name="e">The event</param>
        public void FireCollisionChangedEvent(CollisionChangedEvent e)
        {
            collisionChangedListeners(e);
        }

        public CollisionMap(Game game, int width, int height)
        {
            Texture2D map = game.Content.Load<Texture2D>("CollisionMap/collisionMap5");
            mapWidth = width;
            mapHeight = height;
            this.data = TextureToBoolean(map);
            this.texture = BoolToTexture(game.GraphicsDevice, this.data, 1024, collisionMapTextureScale);
            // collisionMap.GetData(data);
        }
    }
}
