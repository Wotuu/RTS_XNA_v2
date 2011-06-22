using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingTest.QuadTree
{
    public class Quad
    {
        public Quad[] children { get; set; }
        public Quad parent { get; set; }
        public Rectangle rectangle { get; set; }
        public QuadRoot tree { get; set; }
        public Boolean highlighted { get; set; }

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
            if (this.children == null)
            {
                DrawUtil.DrawCross(sb, this.rectangle, this.tree.drawWidth, this.tree.drawColor);
                // Top left to bottom left
                /*Game1.GetInstance().DrawLine(sb,
                    new Point(this.rectangle.Left, this.rectangle.Top),
                    new Point(this.rectangle.Left, this.rectangle.Bottom),
                    this.tree.drawColor,
                    this.tree.drawWidth);
                // Top left to top right
                Game1.GetInstance().DrawLine(sb,
                    new Point(this.rectangle.Left, this.rectangle.Top),
                    new Point(this.rectangle.Right, this.rectangle.Top),
                    this.tree.drawColor,
                    this.tree.drawWidth);
                // Top right to bottom right
                Game1.GetInstance().DrawLine(sb,
                    new Point(this.rectangle.Right, this.rectangle.Top),
                    new Point(this.rectangle.Right, this.rectangle.Bottom),
                    this.tree.drawColor,
                    this.tree.drawWidth);
                // Bottom right to bottom left
                Game1.GetInstance().DrawLine(sb,
                    new Point(this.rectangle.Right, this.rectangle.Bottom),
                    new Point(this.rectangle.Left, this.rectangle.Bottom),
                    this.tree.drawColor,
                    this.tree.drawWidth);*/

                if (this.highlighted)
                {
                    DrawUtil.DrawLine(sb,
                        new Point(this.rectangle.Left, this.rectangle.Top),
                        new Point(this.rectangle.Right, this.rectangle.Bottom),
                        this.tree.highlightedColor,
                        this.tree.drawWidth);
                    DrawUtil.DrawLine(sb,
                        new Point(this.rectangle.Right, this.rectangle.Top),
                        new Point(this.rectangle.Left, this.rectangle.Bottom),
                        this.tree.highlightedColor,
                        this.tree.drawWidth);
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
        }
    }
}
