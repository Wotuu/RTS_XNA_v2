﻿using System;
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
using CustomLists.Lists;

namespace PathfindingTest.Pathfinding
{
    public class Node : PathfindingNode, Offsetable
    {
        public Color c { get; set; }

        private Texture2D texture;

        public Boolean generatedOnLoadTime { get; set; }

        public double lastConnectionCreateTime { get; set; }


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
            this.onConnectionsCreatedListeners += this.OnConnectionsCreated;
            PathfindingNodeManager.GetInstance().AddNode(this);
        }

        /// <summary>
        /// Creates a node that doesn't make it's connections in the constructor.
        /// </summary>
        /// <param name="x">The X</param>
        /// <param name="y">The Y</param>
        /// <param name="forceNodeCreation">Instantly creates connections.</param>
        public Node(RTSCollisionMap map, int x, int y, Boolean forceNodeCreation)
            : base(map)
        {
            Init(x, y);
            if (forceNodeCreation) this.CreateConnections(PathfindingNode.MAX_CONNECT_RANGE);
            else SmartPathfindingNodeProcessor.GetInstance().Push(this);
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
            sb.Draw(texture, this.GetDrawRectangle(), null, c, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            // Draw some connections twice, but o well!

            // if( Monitor.TryEnter(this.connections, 0) ){
            lock (this.connectionSyncLock)
            {
                try
                {
                    for( int i = 0; i < this.connections.Count(); i++){
                        new DrawableNodeConnection(this.connections.ElementAt(i)).Draw(sb);
                    }
                }
                catch (Exception e) { }
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
            Game1 game = Game1.GetInstance();
            float drawX = this.x - (texture.Width / 2), drawY = this.y - (texture.Height / 2);
            return new Rectangle((int)(drawX - game.drawOffset.X), (int)(drawY - game.drawOffset.Y),
                this.texture.Width, this.texture.Height);
        }

        public void OnConnectionsCreated(PathfindingNode source)
        {
            if (!this.initialised && !generatedOnLoadTime)
            {
                double currTime = 
                    new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds ;
                // For each node we can make a connection to
                CustomArrayList<PathfindingNode> connectedNodes = this.GetConnectedNodes();

                for( int i = 0; i < connectedNodes.Count(); i++){
                    if (connectedNodes.ElementAt(i) is Node)
                    {
                        Node connectedNode = (Node)connectedNodes.ElementAt(i);
                        // Scedule it for reprocessing
                        if (currTime - connectedNode.lastConnectionCreateTime > 5000)
                        {
                            // Remove its current nodes
                            connectedNode.RemoveAllConnections();
                            connectedNode.lastConnectionCreateTime = currTime;
                            SmartPathfindingNodeProcessor.GetInstance().Push(connectedNode);
                        }
                    }
                    // Console.Out.WriteLine("Re-sceduled a node!");
                }
                this.initialised = true;
            }
            lastConnectionCreateTime = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
        }
    }
}
