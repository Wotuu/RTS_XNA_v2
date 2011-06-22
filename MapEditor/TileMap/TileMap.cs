using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MapEditor.TileMap
{
    public class TileMap
    {
        public List<TileMapLayer> layers = new List<TileMapLayer>();
        public List<string> layerNames = new List<string>();
        static int mapWidth;
        static int mapHeight;
        int layercount = 3;

        /// <summary>
        /// Initializes a new map with 3 layers !
        /// </summary>
        /// <param name="mapWidth">Width of map</param>
        /// <param name="mapHeight">Height of Map</param>
        public TileMap(int mapWidth, int mapHeight)
        {
            TileMap.mapWidth = mapWidth;
            TileMap.mapHeight = mapHeight;

            for (int l = 0; l < layercount; l++)
            {
                TileMapLayer layer = new TileMapLayer(mapWidth, mapHeight);
                for (int y = 0; y < mapHeight; y++)
                    for (int x = 0; x < mapWidth; x++)
                        layer.SetTile(x, y, -1);

                layers.Add(layer);
            }
            //Initialize a new tilemap with 3 layers !
        }

        public TileMap(string mapName)
        {
            LoadMap(mapName);
        }

        public int MapWidth
        {
            get { return mapWidth; }
        }

        public int MapHeight
        {
            get { return mapHeight; }
        }

        public static int WidthInPixels
        {
            get { return mapWidth * Engine.TileWidth; }
        }

        public static int HeightInPixels
        {
            get { return mapHeight * Engine.TileHeight; }
        }

        private void LoadMap(string mapName)
        {
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.Load(mapName);
            }
            catch
            {
                throw new Exception("Unable to find the map.");
            }

            XmlNode rootNode = xmlDoc.FirstChild;

            if (rootNode.Name != "TileMap")
            {
                throw new Exception("Invalid tile map format!");
            }

            mapWidth = Int32.Parse(rootNode.Attributes["Width"].Value);
            mapHeight = Int32.Parse(rootNode.Attributes["Height"].Value);

            XmlNode layersNode = rootNode.FirstChild;

            if (layersNode.Name != "Layers")
            {
                throw new Exception("Invalid tile map format!");
            }

            TileMapLayer layer;
            int layerCounter = 0;
            foreach (XmlNode node in layersNode.ChildNodes)
            {
                if (node.Name == "Layer")
                {
                    try
                    {
                        layer = new TileMapLayer(mapWidth, mapHeight);
                        LoadLayer(node, layer);
                        layers.Add(layer);
                        layerCounter++;
                        layerNames.Add("Layer " + layerCounter.ToString());
                    }
                    catch
                    {
                        throw new Exception("Error reading in map layer!");
                    }
                }
            }
        }

        private void LoadLayer(XmlNode layerNode, TileMapLayer layer)
        {
            int rowCount = 0;

            foreach (XmlNode node in layerNode)
            {
                if (node.Name == "Row")
                {
                    string row = node.InnerText;
                    row.Trim();
                    string[] cells = row.Split(' ');
                    for (int i = 0; i < mapWidth; i++)
                        layer.SetTile(i, rowCount, Int32.Parse(cells[i]));
                    rowCount++;
                }
            }
        }
    }
}
