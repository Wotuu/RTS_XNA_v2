using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor.TileMap
{
    public static class Engine
    {
        static int tileWidth = 20;
        static int tileHeight = 20;
        static int viewPortWidth = 800;
        static int viewPortHeight = 640;

        public static int TileWidth
        {
            get { return tileWidth; }
        }

        public static int TileHeight
        {
            get { return tileHeight; }
        }

        public static int ViewPortWidth
        {
            get { return viewPortWidth; }
        }

        public static int ViewPortHeight
        {
            get { return viewPortHeight; }
        }
    }
}
