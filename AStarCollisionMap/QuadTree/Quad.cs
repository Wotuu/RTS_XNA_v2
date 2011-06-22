using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStarCollisionMap.QuadTree
{
    public class Quad
    {
        public Quad[] children { get; set; }
        public Quad parent { get; set; }
        public Rectangle rectangle { get; set; }
        public QuadRoot tree { get; set; }
        public Boolean highlighted { get; set; }
        public Boolean isLeaf { get; set; }
        public CollisionTexture collisionTexture { get; set; }

        /// <summary>
        /// Gets the depth of this node in the tree.
        /// </summary>
        /// <returns>The depth!</returns>
        public int GetDepth()
        {
            Quad quad = this;
            int depth = 0;
            while (quad.parent != null)
            {
                quad = quad.parent;
                depth++;
            }
            return depth;
        }

        public Quad[] Split()
        {
            Quad[] quads = new Quad[4];

            int newWidth = this.rectangle.Width / 2;
            int newHeight = this.rectangle.Height / 2;
            // [*][ ]
            // [ ][ ]
            quads[0] = new Quad(this.tree, this,
                new Rectangle(this.rectangle.Left, this.rectangle.Top, newWidth, newHeight));
            // [ ][*]
            // [ ][ ]
            quads[1] = new Quad(this.tree, this,
                new Rectangle(this.rectangle.Left + newWidth, this.rectangle.Top, newWidth, newHeight));
            // [ ][ ]
            // [*][ ]
            quads[2] = new Quad(this.tree, this,
                new Rectangle(this.rectangle.Left, this.rectangle.Top + newHeight, newWidth, newHeight));
            // [ ][ ]
            // [ ][*]
            quads[3] = new Quad(this.tree, this,
                new Rectangle(this.rectangle.Left + newWidth, this.rectangle.Top + newHeight, newWidth, newHeight));

            this.children = quads;
            return quads;
        }

        internal void Draw(SpriteBatch sb)
        {
            // If it ain't on the screen
            Rectangle viewPort = new Rectangle( 0, 0, this.tree.collisionMap.game.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
                this.tree.collisionMap.game.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
            if( !(viewPort.Contains( new Point( this.rectangle.Left, this.rectangle.Top ) )
                || viewPort.Contains( new Point( this.rectangle.Right, this.rectangle.Top ) )
                || viewPort.Contains( new Point( this.rectangle.Left, this.rectangle.Bottom ) )
                || viewPort.Contains( new Point( this.rectangle.Right, this.rectangle.Bottom ) )) ) return;


            if (this.children == null)
            {
                // Draw the texture
                sb.Draw(this.collisionTexture.texture, this.rectangle, this.tree.drawColor);
                // Draw the rectangle bounds
                DrawUtil.DrawClearRectangle(sb, this.rectangle, this.tree.borderWidth, this.tree.borderColor);

                // Draw the ????
                if (this.highlighted)
                {
                    DrawUtil.DrawLine(sb,
                        new Point(this.rectangle.Left, this.rectangle.Top),
                        new Point(this.rectangle.Right, this.rectangle.Bottom),
                        this.tree.highlightedColor,
                        this.tree.borderWidth);
                    DrawUtil.DrawLine(sb,
                        new Point(this.rectangle.Right, this.rectangle.Top),
                        new Point(this.rectangle.Left, this.rectangle.Bottom),
                        this.tree.highlightedColor,
                        this.tree.borderWidth);
                }
                return;
            }
            foreach (Quad quad in children)
            {
                quad.Draw(sb);
            }
        }

        /// <summary>
        /// Attempts to search a quad that contains the given point
        /// </summary>
        /// <param name="p">The point to search for</param>
        public Quad Search(Point p)
        {
            if (this.children == null)
            {
                if (this.rectangle.Contains(p)) return this;
                else return null;
            }
            foreach (Quad child in children)
            {
                if (child.rectangle.Contains(p))
                {
                    return child.Search(p);
                }
            }
            // Should never be called, but just in case ..
            return null;
        }



        public Quad(QuadRoot tree, Quad parent, Rectangle rectangle)
        {
            this.tree = tree;
            this.parent = parent;
            this.rectangle = rectangle;

            // Console.Out.WriteLine("Creating quad with " + this.rectangle + " this.GetDepth() = " + this.GetDepth() + ", maxDepth = " + tree.depth);
            if (this.GetDepth() == tree.depth)
            {
                this.isLeaf = true;

                int imageX = this.rectangle.X / this.rectangle.Width;
                int imageY = this.rectangle.Y / this.rectangle.Height;

                if (!tree.collisionMap.drawMode)
                {
                    this.collisionTexture = new CollisionTexture(this, tree.collisionMap.game.Content.Load<Texture2D>
                    (tree.collisionMap.collisionMapPath + "/" + tree.collisionMap.collisionMapName + "_" + imageX + "_" + imageY));
                }
                else
                {
                    this.collisionTexture = new CollisionTexture(this, new Texture2D(tree.collisionMap.game.GraphicsDevice, 
                        this.rectangle.Width, this.rectangle.Height));
                }
            }
        }
    }
}
