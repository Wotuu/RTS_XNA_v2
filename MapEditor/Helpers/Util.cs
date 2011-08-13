using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using MapEditor.Pathfinding;
using System.Threading;

namespace MapEditor.Helpers
{
    class Util
    {
        /// <summary>
        /// Gets the length of the hypoteneuse between two points.
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The length</returns>
        public static double GetHypoteneuseLength(Point p1, Point p2)
        {
            int xDiff = Math.Abs(p1.X - p2.X);
            int yDiff = Math.Abs(p1.Y - p2.Y);
            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }

        /// <summary>
        /// Gets the angle of the hypoteneuse with the X-axis between two points.
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <returns>The float containing the angle, in radians</returns>
        public static float GetHypoteneuseAngleRad(Point p1, Point p2)
        {
            return (float)(Math.Atan2(p2.Y - p1.Y, p2.X - p1.X));
        }

        /// <summary>
        /// Gets the angle of the hypoteneuse with the X-axis between two points.
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <returns>The float containing the angle, in degrees</returns>
        public static float GetHypoteneuseAngleDegrees(Point p1, Point p2)
        {
            return (float)(Util.GetHypoteneuseAngleRad(p1, p2) * 180 / Math.PI);
        }


        /// <summary>
        /// Gets a point at the edge of a circle at a location, with given radius, with the given angle.
        /// </summary>
        /// <param name="location">The location of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="angle">The angle</param>
        /// <returns>The point at.</returns>
        public static Point GetPointOnCircle(Point location, double radius, double angle)
        {
            return new Point((int)(location.X + radius * Math.Cos(angle * Math.PI / 180F)),
                (int)(location.Y + radius * Math.Sin(angle * Math.PI / 180F)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public static Texture2D GetCustomTexture2D(SpriteBatch batch,Color color)
        {
            Texture2D lineTexture = new Texture2D(batch.GraphicsDevice, 1, 1);
            int[] intColor = { (int)color.PackedValue  };
            lineTexture.SetData(intColor);
            return lineTexture;
        }

        public static int GetQuadDepth(int width)
        {
            double Depth = Math.Sqrt(width);
            Depth = Depth / 3;

            return (int)(Math.Min(Math.Ceiling(Depth),7));
        }

        /// <summary>
        /// Loads collisionnodes from a file
        /// </summary>
        /// <param name="Path">Full path of the XML</param>
        /// <param name="graphicsdevice">Graphics Device</param>
        public static void LoadNodes(String filename, GraphicsDevice graphicsdevice)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlNode declaration = xmldoc.FirstChild;
            XmlNode rootNode = xmldoc.ChildNodes[1];
            if (!rootNode.Name.Equals("GameMap"))
            {
                throw new Exception("XML document is not formatted correctly");
            }
            XmlNode Nodes = rootNode.ChildNodes[1];
            for (int n = 0; n < Nodes.ChildNodes.Count; n++)
            {
                Thread.Sleep(10);
                new Node(Form1.CollisionMap, int.Parse(Nodes.ChildNodes[n].Attributes["x"].Value), int.Parse(Nodes.ChildNodes[n].Attributes["y"].Value),graphicsdevice );
                //new Node(Form1.CollisionMap, 5  * n, 5 * n, graphicsdevice);
            }
        }
    }
}
