using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAInterfaceComponents.Misc
{
    public class Padding
    {
        public int left { get; set; }
        public int right { get; set; }
        public int top { get; set; }
        public int bottom { get; set; }

        public Padding()
        {

        }

        public Padding(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }
}
