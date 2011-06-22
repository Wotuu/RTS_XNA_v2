using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Combat
{
    interface Aggroable
    {
        void OnAggroRecieved(AggroEvent e);
        void OnAggro(AggroEvent e);
    }
}
