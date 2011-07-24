using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AStarCollisionMap.Collision;

namespace AStarCollisionMap.QuadTree
{
    public class QuadRoot
    {
        public Quad root { get; set; }
        public int depth { get; set; }
        public Rectangle rectangle { get; set; }

        public Color drawColor { get; set; }
        public Color highlightedColor { get; set; }

        public int borderWidth { get; set; }
        public Color borderColor { get; set; }

        public Vector2 drawOffset { get; set; }

        public CollisionMap collisionMap { get; set; }

        public LinkedList<Quad> leafList = new LinkedList<Quad>();

        public QuadRoot(Rectangle rect, CollisionMap map)
        {
            this.drawColor = Color.Green;
            this.highlightedColor = Color.Blue;
            this.borderWidth = 1;
            this.drawColor = Color.White;
            this.borderColor = Color.Green;
            this.rectangle = rect;

            this.collisionMap = map;
        }

        public void CreateTree(int depth)
        {
            this.depth = depth;
            this.root = new Quad(this, null, this.rectangle);
            SplitQuad(root);
        }

        private void SplitQuad(Quad quad)
        {
            // Console.Out.WriteLine("Splitting a quad!");
            if (quad.GetDepth() < this.depth)
            {
                Quad[] newQuads = quad.Split();
                for (int i = 0; i < newQuads.Length; i++)
                {
                    SplitQuad(newQuads[i]);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if( this.root != null ) root.Draw(sb);
        }

        public Quad GetQuadByPoint(Point p)
        {
            return root.Search(p);
        }
    }
}
