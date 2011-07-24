using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Xml;
using AStarCollisionMap.Pathfinding;
using System.IO;
using AStarCollisionMap.Collision;

namespace MapEditor.Pathfinding
{
    public class Node : PathfindingNode
    {
        public Color c { get; set; }

        public Rectangle drawRect { get; set; }

        private Texture2D texture;

        GraphicsDevice graphicsDevice = null;

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
            //Game1 game = Game1.GetInstance();
            this.x = x;
            this.y = y;
            FileStream stream = new FileStream("./Content/node.png", FileMode.Open);
            texture = Texture2D.FromStream(graphicsDevice, stream);
            stream.Close();
            stream.Dispose();
            this.c = Color.Red;

            PathfindingNodeManager.GetInstance().nodeList.AddLast(this);
        }

        /// <summary>
        /// Creates a node that doesn't make it's connections in the constructor.
        /// </summary>
        /// <param name="x">The X</param>
        /// <param name="y">The Y</param>
        /// <param name="forceNodeCreation">Instantly creates connections. This param does nothing.</param>
        public Node(CollisionMap map, int x, int y, Boolean forceNodeCreation,GraphicsDevice adapter)
            : base(map)
        {
            this.graphicsDevice = adapter;
            Init(x, y);
            this.CreateConnections();
        }

        public Node(CollisionMap map, int x, int y,GraphicsDevice adapter)
            : base(map)
        {
            this.graphicsDevice = adapter;
            Init(x, y);
            PathfindingNodeProcessor.GetInstance().Push(this);
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, this.GetDrawRectangle(), null, c, 0f, Vector2.Zero, SpriteEffects.None, 0.01f);
            // Draw some connections twice, but o well!

            // if( Monitor.TryEnter(this.connections, 0) ){
            foreach (PathfindingNodeConnection conn in this.connections)
            {
                new DrawableNodeConnection(conn).Draw(sb);
            }
            
            // sb.DrawString(game.font, "" + score, new Vector2(drawX, drawY - 14), Color.Black);
            // sb.DrawString(game.font, "" + costToStart, new Vector2(drawX, drawY + texture.Height - 2), Color.Black);
            // sb.DrawString(game.font, "" + costToEnd, new Vector2(drawX, drawY + texture.Height + 8), Color.Black);
        }

       

        public override String ToString()
        {

            return "Node(" + this.GetLocation().X + ", " + this.GetLocation().Y + ")";
        }

        public Rectangle GetDrawRectangle()
        {
            //Game1 game = Game1.GetInstance();
            float drawX = this.x - (texture.Width / 2), drawY = this.y - (texture.Height / 2);
            return new Rectangle((int)(drawX - Form1.camera.Position.X), (int)(drawY - Form1.camera.Position.Y),
                this.texture.Width, this.texture.Height);
        }
    }
}
