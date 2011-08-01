using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Map
{
    public class Layer
    {
        public int[,] data { get; set; }


        public Layer(int[,] data)
        {
            this.data = data;
        }
    }
}
