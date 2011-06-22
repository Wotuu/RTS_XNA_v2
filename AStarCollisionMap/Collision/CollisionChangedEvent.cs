using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AStarCollisionMap.QuadTree;

namespace AStarCollisionMap.Collision
{
    public class CollisionChangedEvent
    {
        public CollisionMap collisionMap { get; set; }
        public Rectangle changedRect { get; set; }
        /// <summary>
        /// Whether collision was added or removed!
        /// </summary>
        public Boolean collisionAdded { get; set; }

        public LinkedList<QuadPart> changedQuads = new LinkedList<QuadPart>();



        public CollisionChangedEvent()
        {

        }

        public CollisionChangedEvent(CollisionMap collisionMap, Rectangle changedRect, Boolean collisionAdded)
        {
            this.collisionMap = collisionMap;
            this.changedRect = changedRect;
            this.collisionAdded = collisionAdded;
        }

        public struct QuadPart
        {
            public Quad quad;
            public Rectangle rectangle;

            public QuadPart(Quad quad, Rectangle rectangle)
            {
                this.quad = quad;
                this.rectangle = rectangle;
            }
        }
    }
}
