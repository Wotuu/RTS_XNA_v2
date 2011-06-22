using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStarCollisionMap.Collision
{
    public interface OnCollisionChangedListener
    {
        void OnCollisionChanged(CollisionChangedEvent collisionEvent);
    }
}
