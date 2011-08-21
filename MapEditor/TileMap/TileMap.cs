using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using MapEditor.Pathfinding;
using AStarCollisionMap.Pathfinding;
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

        //public TileMap(string mapName)
        //{
        //    LoadMap(mapName);
        //}

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

        //private void LoadMap(string mapName)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();

        //    try
        //    {
        //        xmlDoc.Load(mapName);
        //    }
        //    catch
        //    {
        //        throw new Exception("Unable to find the map.");
        //    }

        //    XmlNode rootNode = xmlDoc.FirstChild;

        //    if (rootNode.Name != "TileMap")
        //    {
        //        throw new Exception("Invalid tile map format!");
        //    }

        //    mapWidth = Int32.Parse(rootNode.Attributes["Width"].Value);
        //    mapHeight = Int32.Parse(rootNode.Attributes["Height"].Value);

        //    XmlNode layersNode = rootNode.FirstChild;

        //    if (layersNode.Name != "Layers")
        //    {
        //        throw new Exception("Invalid tile map format!");
        //    }

        //    TileMapLayer layer;
        //    int layerCounter = 0;
        //    foreach (XmlNode node in layersNode.ChildNodes)
        //    {
        //        if (node.Name == "Layer")
        //        {
        //            try
        //            {
        //                layer = new TileMapLayer(mapWidth, mapHeight);
        //                LoadLayer(node, layer);
        //                layers.Add(layer);
        //                layerCounter++;
        //                layerNames.Add("Layer " + layerCounter.ToString());
        //            }
        //            catch
        //            {
        //                throw new Exception("Error reading in map layer!");
        //            }
        //        }
        //    }
        //}

        //private void LoadLayer(XmlNode layerNode, TileMapLayer layer)
        //{
        //    int rowCount = 0;

        //    foreach (XmlNode node in layerNode)
        //    {
        //        if (node.Name == "Row")
        //        {
        //            string row = node.InnerText;
        //            row.Trim();
        //            string[] cells = row.Split(' ');
        //            for (int i = 0; i < mapWidth; i++)
        //                layer.SetTile(i, rowCount, Int32.Parse(cells[i]));
        //            rowCount++;
        //        }
        //    }
        //}

        public void savemap(String filename)
        {
            XmlDocument mapxml = new XmlDocument();
            XmlDeclaration xmlDeclaration = mapxml.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement rootNode = mapxml.CreateElement("GameMap");
            mapxml.InsertBefore(xmlDeclaration, mapxml.DocumentElement);
            mapxml.AppendChild(rootNode);
            XmlElement parentNode = mapxml.CreateElement("Layers");
            rootNode.AppendChild(parentNode);
            for(int i  = 0 ; i < layers.Count(); i++ ){
                XmlElement Layernode = mapxml.CreateElement("Layer");
                parentNode.AppendChild(Layernode);
                XmlElement Rowsnode = mapxml.CreateElement("Rows");
                Layernode.AppendChild(Rowsnode);
                for (int h = 0; h < this.MapHeight;  h++)
                {
                    XmlElement rownode = mapxml.CreateElement("row");
                    for (int r = 0; r < this.MapWidth; r++)
                    {
                        rownode.InnerText += layers[i].GetTile(r, h) + ",";
                    }
                    rownode.InnerText = rownode.InnerText.TrimEnd(',');
                    Rowsnode.AppendChild(rownode);
                }
            }

            //Nodes ook opslaan
            XmlElement Nodes = mapxml.CreateElement("Nodes");
            rootNode.AppendChild(Nodes);
            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
            for (int i = 0; i < manager.GetNodeCount(); i++)
            {
                Node node = (Node)manager.GetNodeAt(i);
                XmlElement NodeElement = mapxml.CreateElement("Node");
                NodeElement.SetAttribute("x", node.x.ToString());
                NodeElement.SetAttribute("y", node.y.ToString());
                Nodes.AppendChild(NodeElement);
            }


            // Map data
            XmlElement DataElement = mapxml.CreateElement("Data");
            DataElement.SetAttribute("width", this.MapWidth + "");
            DataElement.SetAttribute("height", this.MapHeight + "");
            DataElement.SetAttribute("tileWidth", Engine.TileWidth + "");
            DataElement.SetAttribute("tileHeight", Engine.TileHeight + "");
            rootNode.AppendChild(DataElement);

            //Players
            XmlElement PlayersElement = mapxml.CreateElement("Players");
            rootNode.AppendChild(PlayersElement);
            foreach (Helpers.Player p in Form1.Players)
            {
                XmlElement NodeElement = mapxml.CreateElement("Player");
                NodeElement.SetAttribute("x", p.x.ToString());
                NodeElement.SetAttribute("y", p.y.ToString());
                PlayersElement.AppendChild(NodeElement);
            }

            mapxml.Save(filename);
        }

        
        public TileMap opentilemap(string filename)
        {
            
            int mapwidth = 0;
            int mapheight = 0;
            TileMap map = null;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlNode declaration = xmldoc.FirstChild;

            XmlNode rootNode = xmldoc.ChildNodes[1];
            if (!rootNode.Name.Equals("GameMap"))
            {
                throw new Exception("XML document is not formatted correctly");
            }
            XmlNode layers = rootNode.FirstChild;
            if (!layers.Name.Equals("Layers"))
            {
                throw new Exception("XML document is not formatted correctly");
            }
            
            for (int i = 0; i < layers.ChildNodes.Count; i++)
            {
                XmlNode layer = layers.ChildNodes[i];
                if (!layer.Name.Equals("Layer"))
                {
                    throw new Exception("XML document is not formatted correctly");
                }

                //Rows uitlezen
                XmlNode rows = layer.FirstChild;
                if (!rows.Name.Equals("Rows"))
                {
                    throw new Exception("XML document is not formatted correctly");
                }

                XmlNode row = rows.FirstChild;
                if (!row.Name.Equals("row"))
                {
                    throw new Exception("XML document is not formatted correctly");
                }
                
                mapheight = rows.ChildNodes.Count;
                mapwidth = row.InnerText.Split(',').Count();
                
                if(i == 0)map = new TileMap(mapwidth, mapheight);
                for (int r = 0; r < rows.ChildNodes.Count; r++)
                {
                    string[] tiles = rows.ChildNodes[r].InnerText.Split(',');
                    for (int c = 0; c < mapwidth; c++)
                    {
                        map.layers[i].SetTile(c, r, int.Parse(tiles[c]));
                    }
                }

               

               
            }

            return map;
           

        }

        
    }
}
