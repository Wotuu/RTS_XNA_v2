using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CustomLists.Lists;

namespace PathfindingTest.Selection.Patterns
{
    public abstract class UnitGroupPattern
    {
        public UnitSelection selection { get; set; }
        public Point location { get; set; }

        public abstract CustomArrayList<Point> ApplyPattern();

        public UnitGroupPattern(Point location, UnitSelection selection){
            this.location = location;
            this.selection = selection;
        }
    }
}
