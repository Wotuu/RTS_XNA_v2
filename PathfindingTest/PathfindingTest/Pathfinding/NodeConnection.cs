using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStarCollisionMap.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingTest.Pathfinding
{
    public class DrawableNodeConnection
    {
        public static Color TRANSPARENT { get; set; }
        public static Color HIGHLIGHTED { get; set; }
        private Point lengthDrawOffset { get; set; }
        public Color drawColor { get; set; }
        public PathfindingNodeConnection conn { get; set; }

        static DrawableNodeConnection()
        {
            // TRANSPARENT = new Color(0, 0, 0, 0);
            TRANSPARENT = Color.Red;
            HIGHLIGHTED = Color.Blue;
        }


        internal void Draw(SpriteBatch sb)
        {
            DrawUtil.DrawLine(sb, this.conn.node1.GetLocation(), this.conn.node2.GetLocation(), this.drawColor, 1);
            //sb.DrawString( game.font, 
            //    (int) Util.GetHypoteneuseLength( node1.GetLocation(), node2.GetLocation() ) + "", 
            //    new Vector2( lengthDrawOffset.X, lengthDrawOffset.Y ), this.drawColor );
        }

        public DrawableNodeConnection(PathfindingNodeConnection conn) 
        {
            this.conn = conn;
            this.drawColor = TRANSPARENT;
            /*PathfindingNode closestNode = null;
            if (Util.GetHypoteneuseLength(node1.GetLocation(), new Point(0, 0)) < Util.GetHypoteneuseLength(node2.GetLocation(), new Point(0, 0)))
            {
                closestNode = node1;
            } else closestNode = node2;
            lengthDrawOffset = new Point(closestNode.x + (int)Math.Abs(node1.x - node2.x), closestNode.y + (int)Math.Abs(node1.y - node2.y));*/
        }

        public DrawableNodeConnection(PathfindingNodeConnection conn, Color c)
        {
            this.conn = conn;
            this.drawColor = c;
        }
    }
}
