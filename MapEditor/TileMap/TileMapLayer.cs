using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor.TileMap
{
    public class TileMapLayer
    {
        int[,] map;

        public TileMapLayer(int width, int height)
        {
            map = new int[height, width];
        }

        public void SetTile(int x, int y, int tileIndex)
        {
            if (x >= 0 && y >= 0)
            {
                if (x < map.GetLength(1) && y < map.GetLength(0))
                    map[y, x] = tileIndex;
            }
            
        }

        public int GetTile(int x, int y)
        {
            return map[y, x];
        }

    }
}
