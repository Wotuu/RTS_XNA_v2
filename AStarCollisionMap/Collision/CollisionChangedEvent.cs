using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AStarCollisionMap.Collision
{
    public class CollisionChangedEvent
    {
        public CollisionMap collision { get; set; }
        public Boolean[] oldData { get; set; }
        public Boolean[] newData { get; set; }
        public Rectangle changedRect { get; set; }
        /// <summary>
        /// Whether collision was added or removed!
        /// </summary>
        public Boolean collisionAdded { get; set; }

        public CollisionChangedEvent()
        {

        }

        public CollisionChangedEvent(CollisionMap c, Boolean[] oldData, Boolean[] newData, Rectangle changedRect, Boolean collisionAdded)
        {
            this.collision = c;
            this.oldData = oldData;
            this.newData = newData;
            this.changedRect = changedRect;
            this.collisionAdded = collisionAdded;
        }
    }
}
