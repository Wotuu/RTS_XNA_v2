using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PathfindingTest.Interfaces;
using System.Threading;
using System.Xml;
using AStarCollisionMap.Pathfinding;
using PathfindingTest.Collision;

namespace PathfindingTest.Pathfinding
{
    public class Node : PathfindingNode, XMLAble
    {
        public Color c { get; set; }

        public Rectangle drawRect { get; set; }

        private Texture2D texture;



        private Boolean _selected;
        public Boolean selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (value) c = Color.Blue;
                else c = Color.Red;

                _selected = value;

                if (value) PathfindingNodeManager.GetInstance().selectedNode = this;
                else if (PathfindingNodeManager.GetInstance().selectedNode == this) PathfindingNodeManager.GetInstance().selectedNode = null;
            }
        }

        private void Init(int x, int y)
        {
            Game1 game = Game1.GetInstance();
            this.x = x;
            this.y = y;
            texture = game.Content.Load<Texture2D>("Misc/node");
            this.c = Color.Red;

            PathfindingNodeManager.GetInstance().nodeList.AddLast(this);
        }

        /// <summary>
        /// Creates a node that doesn't make it's connections in the constructor.
        /// </summary>
        /// <param name="x">The X</param>
        /// <param name="y">The Y</param>
        /// <param name="forceNodeCreation">Instantly creates connections. This param does nothing.</param>
        public Node(RTSCollisionMap map, int x, int y, Boolean forceNodeCreation)
            : base(map)
        {
            Init(x, y);
            this.CreateConnections();
        }

        public Node(RTSCollisionMap map, int x, int y)
            : base(map)
        {
            Init(x, y);
            SmartPathfindingNodeProcessor.GetInstance().Push(this);
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {
            int drawX = this.x - (int)(texture.Width / 2), drawY = this.y - (int)(texture.Height / 2);
            this.drawRect = new Rectangle(drawX, drawY, texture.Width, texture.Height);
            sb.Draw(texture, this.drawRect, c);
            // Draw some connections twice, but o well!

            // if( Monitor.TryEnter(this.connections, 0) ){
            /*foreach (PathfindingNodeConnection conn in this.connections)
            {
                new DrawableNodeConnection(conn).Draw(sb);
            }*/
            // }
            /*for (int i = 0; i < this.connections.Count; i++)
            {
                PathfindingNodeConnection conn = this.connections.ElementAt(i);
                conn.Draw(sb, game);
            }
            */
            // sb.DrawString(game.font, "" + score, new Vector2(drawX, drawY - 14), Color.Black);
            // sb.DrawString(game.font, "" + costToStart, new Vector2(drawX, drawY + texture.Height - 2), Color.Black);
            // sb.DrawString(game.font, "" + costToEnd, new Vector2(drawX, drawY + texture.Height + 8), Color.Black);
        }

        void XMLAble.AddToXML(XmlTextWriter textWriter)
        {
            textWriter.WriteStartElement("Node");
            textWriter.WriteAttributeString("x", this.GetLocation().X + "");
            textWriter.WriteAttributeString("y", this.GetLocation().Y + "");
            textWriter.WriteEndElement();
        }

        public override String ToString()
        {

            return "Node(" + this.GetLocation().X + ", " + this.GetLocation().Y + ")";
        }
    }
}
