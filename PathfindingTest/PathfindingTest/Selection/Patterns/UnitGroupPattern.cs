using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PathfindingTest.Selection.Patterns
{
    public abstract class UnitGroupPattern
    {
        public UnitSelection selection { get; set; }
        public Point location { get; set; }

        public abstract LinkedList<Point> ApplyPattern();

        public UnitGroupPattern(Point location, UnitSelection selection){
            this.location = location;
            this.selection = selection;
        }
    }
}
