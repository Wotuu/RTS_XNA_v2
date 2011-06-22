using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace MapEditor.TileMap

{
    public class Tileset
    {
        public string textureName;
        public int tileWidth, tileHeight;
        public ArrayList tiles;
        public string TilesetTextureName;


        public Tileset(Texture2D TilesetTexture)
        {
            tiles = new ArrayList();
            textureName = "";
            tileWidth = 0;
            tileHeight = 0;
            //tiles = new List<Rectangle>();
            ProcessTileSet(TilesetTexture);
        }

        void ProcessTileSet(Texture2D TilesetTexture)
        {
            
            for (int y = 0; y <= TilesetTexture.Height / Engine.TileHeight; y++)
            {
                for (int x = 0; x <= TilesetTexture.Width / Engine.TileWidth; x++)
                {
                    Rectangle tile = new Rectangle(x * Engine.TileWidth, y * Engine.TileHeight, Engine.TileWidth, Engine.TileHeight);
                    tiles.Add(tile);
                }
            }
        }

        public int GetTileID(Rectangle TileRectangle, Texture2D TilesetTexture)
        {
            int TileID = 0;
            for (int y = 0; y <= TilesetTexture.Height / Engine.TileHeight; y++)
            {
                for (int x = 0; x <= TilesetTexture.Width / Engine.TileWidth; x++)
                {
                    if ((TileRectangle.X / Engine.TileWidth).Equals(x) && (TileRectangle.Y / Engine.TileHeight).Equals(y))
                    {
                        return TileID;
                    }
                    TileID++;
                }
            }

            return TileID;
        }

    }
}
