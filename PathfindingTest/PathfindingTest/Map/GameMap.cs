using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Collision;
using Microsoft.Xna.Framework;
using System.Xml;
using PathfindingTest.Map;
using System.IO;
using AStarCollisionMap.Collision;
using PathfindingTest.Pathfinding;
using AStarCollisionMap.Pathfinding;
using System.Threading;


public delegate void OnLoadProgressChanged(GameMap source, int percentDone);

namespace PathfindingTest.Map
{
    public class GameMap
    {

        public MiniMap miniMap { get; set; }
        public String mapName { get; set; }
        public Texture2D tileMap { get; set; }
        public Texture2D[,] mapTiles { get; set; }
        public RTSCollisionMap collisionMap { get; set; }

        public LinkedList<Rectangle> rectangleTiles = new LinkedList<Rectangle>();
        public Texture2D[] individualTiles { get; set; }

        public static int TILE_WIDTH = 20, TILE_HEIGHT = 20;

        public LinkedList<Layer> layers = new LinkedList<Layer>();

        public OnLoadProgressChanged onLoadProgressChangedListeners { get; set; }

        public enum BlendMode
        {
            PriorityBlend,
            AlphaBlend
        }

        public GameMap(String mapName)
        {
            this.mapName = mapName;
            Game1.GetInstance().loadingWhat = "Initializing graphics";
            this.tileMap = Game1.GetInstance().Content.Load<Texture2D>("Tilemap/StandardTileset");


            for (int y = 0; y <= tileMap.Height / TILE_HEIGHT; y++)
            {
                for (int x = 0; x <= tileMap.Width / TILE_WIDTH; x++)
                {
                    Rectangle tile = new Rectangle(x * TILE_WIDTH, y * TILE_HEIGHT, TILE_WIDTH, TILE_HEIGHT);
                    rectangleTiles.AddLast(tile);
                }
            }

            int xCount = 0;
            int yCount = 0;
            this.individualTiles = Split(this.tileMap, TILE_WIDTH, TILE_HEIGHT, out xCount, out yCount);

            // For every texture in the collisionmap, minus the map preview texture
            Game1.GetInstance().maxLoadProgress +=
                (Directory.GetFileSystemEntries(Game1.MAPS_FOLDER_LOCATION + mapName.Replace(".xml", "") + "/").Length - 1) * 400;
            // For initing the collisionmap
            Game1.GetInstance().maxLoadProgress += 4000;

            Console.Out.WriteLine("Max load progress: " + Game1.GetInstance().maxLoadProgress);
            Game1.GetInstance().loadingWhat = "Layers";
            // Max progress will be determined in here as well!
            this.LoadLayers(Game1.MAPS_FOLDER_LOCATION + mapName);


            Game1.GetInstance().loadingWhat = "Optimizing layers";
            this.MergeLayers();


            Game1.GetInstance().loadingWhat = "Initialising collisionmap";
            this.collisionMap = new RTSCollisionMap(Game1.GetInstance().GraphicsDevice,
                this.mapTiles.GetLength(0) * TILE_WIDTH, this.mapTiles.GetLength(1) * TILE_HEIGHT,
                CalculateQuadDepth(this.mapTiles.GetLength(0)));
            Game1.GetInstance().currentLoadProgress += 4000;

            this.collisionMap.onMapTileLoadListeners += this.OnMapTileLoaded;

            Game1.GetInstance().loadingWhat = "Collisionmap";
            this.collisionMap.LoadMap(Game1.MAPS_FOLDER_LOCATION, mapName.Replace(".xml", ""));

            Game1.GetInstance().loadingWhat = "Pathfinding";
            this.LoadPathfindingNodes(Game1.MAPS_FOLDER_LOCATION + mapName);

            Console.Out.WriteLine("Finished loading! " + Game1.GetInstance().currentLoadProgress + " / " + Game1.GetInstance().maxLoadProgress);
            Game1.GetInstance().loadingWhat = "Finished";
            // Just in case
            Game1.GetInstance().currentLoadProgress = Game1.GetInstance().maxLoadProgress;
        }

        /// <summary>
        /// When the minimap reports that it has loaded a small bit.
        /// </summary>
        /// <param name="map">The map that's rendering</param>
        private void OnMiniMapRenderTick(MiniMap map)
        {
            Game1.GetInstance().currentLoadProgress += 3;
        }

        /// <summary>
        /// Called when a map tile of the collisionmap is loaded!
        /// </summary>
        /// <param name="source">The collisionmap that triggered this event.</param>
        private void OnMapTileLoaded(CollisionMap source)
        {
            Game1.GetInstance().currentLoadProgress += 400;
        }

        public static int CalculateQuadCount(int mapWidth)
        {
            int depth = CalculateQuadDepth(mapWidth);
            return (int)(Math.Pow(depth, 2) * Math.Pow(depth, 2));
        }

        public static int CalculateQuadDepth(int mapHeight)
        {
            double Depth = Math.Sqrt(mapHeight);
            Depth = Depth / 3;

            return (int)(Math.Min(Math.Ceiling(Depth), 7));
        }

        /// <summary>
        /// Blends the textures into one.
        /// </summary>
        /// <param name="textures">The textures to merge.</param>
        /// <returns>The blended texture</returns>
        public Texture2D BlendTextures(Texture2D[] textures, BlendMode mode)
        {
            Texture2D first = textures[0];
            foreach (Texture2D tex in textures)
            {
                if (first.Width != tex.Width || first.Height != tex.Height)
                {
                    throw new Exception("Texture sizes aren't equal; cannot merge!");
                }
            }

            Texture2D result = first;
            int[] resultData = new int[result.Width * result.Height];
            int[] currentData = new int[result.Width * result.Height];
            // Set the result data to the first texture;
            result.GetData(resultData);
            if (mode == BlendMode.PriorityBlend)
            {
                for (int i = 1; i < textures.Length; i++)
                {
                    textures[i].GetData(currentData);

                    for (int j = 0; j < resultData.Length; j++)
                    {
                        /*int rCurrent = (currentData[j] >> 24) & 0xFF;
                        int gCurrent = (currentData[j] >> 16) & 0xFF;
                        int bCurrent = (currentData[j] >> 8) & 0xFF;*/
                        int aCurrent = (currentData[j] >> 0) & 0xFF; // optimize away ...

                        resultData[j] = (aCurrent != 0) ? currentData[j] : resultData[j];
                    }
                }
            }
            else if (mode == BlendMode.AlphaBlend)
            {
                for (int i = 1; i < textures.Length; i++)
                {
                    textures[i].GetData(currentData);

                    for (int j = 0; j < resultData.Length; j++)
                    {
                        int rCurrent = (currentData[j] >> 24) & 0xFF;
                        int gCurrent = (currentData[j] >> 16) & 0xFF;
                        int bCurrent = (currentData[j] >> 8) & 0xFF;
                        int aCurrent = (currentData[j] >> 0) & 0xFF;

                        int rResult = (resultData[j] >> 24) & 0xFF;
                        int gResult = (resultData[j] >> 16) & 0xFF;
                        int bResult = (resultData[j] >> 8) & 0xFF;
                        int aResult = (resultData[j] >> 0) & 0xFF;

                        int red = ((rCurrent + rResult) / 2);
                        int green = ((gCurrent + gResult) / 2);
                        int blue = ((bCurrent + bResult) / 2);
                        int alpha = ((aCurrent + aResult) / 2);

                        resultData[j] = (int)Color.FromNonPremultiplied(red, green, blue, alpha).PackedValue;

                        // resultData[j] = (aCurrent != 0) ? currentData[j] : resultData[j];
                    }
                }
            }
            result = new Texture2D(Game1.GetInstance().graphics.GraphicsDevice, textures[0].Width, textures[0].Height);
            result.SetData(resultData);
            return result;
        }

        /// <summary>
        /// Merges a linked list of textures.
        /// </summary>
        /// <param name="textures"></param>
        /// <returns></returns>
        public Texture2D MergeTextures(LinkedList<Texture2D> textures)
        {
            return this.MergeTextures(textures.ToArray());
        }

        /// <summary>
        /// Merges 4 (FOUR) textures together to a big texture.
        /// </summary>
        /// <param name="toMerge">The textures to merge</param>
        /// <returns>The new texture</returns>
        public Texture2D MergeTextures(Texture2D[] toMerge)
        {
            Texture2D first = toMerge[0];
            int newWidth = (toMerge[0].Width * 2);
            int newHeight = (toMerge[0].Height * 2);

            Rectangle[] rects = new Rectangle[] {
                new Rectangle(0, 0, toMerge[0].Width, toMerge[0].Height),
                new Rectangle(toMerge[0].Width, 0, toMerge[0].Width, toMerge[0].Height),
                new Rectangle(0, toMerge[0].Height, toMerge[0].Width, toMerge[0].Height),
                new Rectangle(toMerge[0].Width, toMerge[0].Height, toMerge[0].Width, toMerge[0].Height)
            };

            foreach (Texture2D tex in toMerge)
            {
                if (first.Width != tex.Width || first.Height != tex.Height)
                {
                    throw new Exception("Texture sizes aren't equal; cannot merge!");
                }
            }

            if (toMerge.Length != 4) throw new Exception("Texture sizes aren't equal; cannot merge!");

            int[] result = new int[newWidth * newHeight];
            int[] texData0 = new int[toMerge[0].Width * toMerge[0].Height];
            int[] texData1 = new int[toMerge[0].Width * toMerge[0].Height];
            int[] texData2 = new int[toMerge[0].Width * toMerge[0].Height];
            int[] texData3 = new int[toMerge[0].Width * toMerge[0].Height];

            toMerge[0].GetData(texData0);
            toMerge[1].GetData(texData1);
            toMerge[2].GetData(texData2);
            toMerge[3].GetData(texData3);


            for (int i = 0; i < result.Length; i++)
            {
                Point p = new Point(i % newWidth, (int)(i / newWidth));
                for (int j = 0; j < rects.Length; j++)
                {
                    if (rects[j].Contains(p))
                    {
                        if (j == 0) result[i] = texData0[i - (toMerge[0].Width * p.Y)];
                        else if (j == 1) result[i] = texData1[i - (toMerge[0].Width * (p.Y + 1))];
                        else if (j == 2) result[i] = texData2[i - (newWidth * toMerge[0].Height + toMerge[0].Width * (p.Y - toMerge[0].Height))];
                        else if (j == 3) result[i] = texData3[i - (newWidth * toMerge[0].Height + (toMerge[0].Width * ((p.Y - toMerge[0].Height) + 1)))];
                    }
                }
            }
            Texture2D resultTex = new Texture2D(Game1.GetInstance().graphics.GraphicsDevice, newWidth, newHeight);
            resultTex.SetData(result);
            return resultTex;
        }

        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="sb">SpriteBatch to draw on.</param>
        public void Draw(SpriteBatch sb)
        {
            //this.collisionMap.DrawMap(sb);
            Vector2 offset = Game1.GetInstance().drawOffset;
            int startY = Math.Abs((int)(Game1.GetInstance().drawOffset.Y / GameMap.TILE_HEIGHT));
            int startX = Math.Abs((int)(Game1.GetInstance().drawOffset.X / GameMap.TILE_WIDTH));
            for (int y = startY;
                y < startY + Game1.GetInstance().graphics.PreferredBackBufferHeight / GameMap.TILE_HEIGHT + 2; y++)
            {
                for (int x = startX;
                    x < startX + Game1.GetInstance().graphics.PreferredBackBufferWidth / GameMap.TILE_WIDTH + 2; x++)
                {
                    if (x < this.mapTiles.GetLength(0) &&
                        y < this.mapTiles.GetLength(1))
                    {
                        Texture2D tex = this.mapTiles[x, y];
                        int texX = x * GameMap.TILE_WIDTH;
                        int texY = y * GameMap.TILE_HEIGHT;

                        sb.Draw(tex, new Rectangle((int)(texX - offset.X), (int)(texY - offset.Y), tex.Width, tex.Height), null, Color.White,
                            0f, Vector2.Zero, SpriteEffects.None, 1f);
                    }
                }
            }
        }


        /// <summary>
        /// Loads the layers from a certain map.
        /// </summary>
        /// <param name="filename">The filename</param>
        public void LoadLayers(string filename)
        {
            this.layers.Clear();
            int mapwidth = 0;
            int mapheight = 0;

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

            XmlNode nodes = rootNode.ChildNodes.Item(1);
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

                Layer currentLayer = new Layer(new int[mapwidth, mapheight]);
                // Adjust the max progress
                if (this.layers.Count == 0)
                {
                    // For every layer that we load
                    Game1.GetInstance().maxLoadProgress += mapwidth * mapheight * 3;

                    // For every texture that we blend, but that's 3 times heavier than this one, I guess!
                    Game1.GetInstance().maxLoadProgress += mapwidth * mapheight * 9;

                    // 100 for every collision node
                    Game1.GetInstance().maxLoadProgress += nodes.ChildNodes.Count * 100;
                }
                this.layers.AddLast(currentLayer);

                for (int y = 0; y < mapheight; y++)
                {
                    string[] tiles = rows.ChildNodes[y].InnerText.Split(',');
                    for (int x = 0; x < mapwidth; x++)
                    {
                        currentLayer.data[x, y] = int.Parse(tiles[x]);
                        // One step closer to completion
                        Game1.GetInstance().currentLoadProgress++;
                    }
                }
            }
        }


        /// <summary>
        /// Loads all the pathfinding nodes in the game. Also creates the connections.
        /// </summary>
        /// <param name="filename">The filename of the map to load from</param>
        public void LoadPathfindingNodes(String filename)
        {
            PathfindingNodeManager.GetInstance().ClearNodes();


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);


            XmlNode rootNode = xmldoc.ChildNodes[1];

            XmlNode nodes = rootNode.ChildNodes.Item(1);
            if (!nodes.Name.Equals("Nodes"))
            {
                throw new Exception("XML document is not formatted correctly");
            }

            for (int i = 0; i < nodes.ChildNodes.Count; i++)
            {
                XmlNode node = nodes.ChildNodes[i];

                if (!node.Name.Equals("Node"))
                {
                    throw new Exception("XML document is not formatted correctly");
                }

                Node pfNode = new Node(this.collisionMap,
                    Int32.Parse(node.Attributes.GetNamedItem("x").Value),
                    Int32.Parse(node.Attributes.GetNamedItem("y").Value), true);
                pfNode.generatedOnLoadTime = true;
                // One more node ready
                Game1.GetInstance().currentLoadProgress += 100;
            }

            SmartPathfindingNodeProcessor.GetInstance().StartThread();
        }

        /// <summary>
        /// Merges the loaded layers!
        /// </summary>
        public void MergeLayers()
        {
            if (this.layers.Count == 0)
            {
                throw new Exception("Perform LoadLayers() first.");
            }

            mapTiles = new Texture2D[this.layers.First.Value.data.GetLength(0), this.layers.First.Value.data.GetLength(1)];
            for (int i = 0; i < this.layers.First.Value.data.GetLength(0); i++)
            {
                for (int j = 0; j < this.layers.First.Value.data.GetLength(1); j++)
                {
                    LinkedList<Texture2D> preBlend = new LinkedList<Texture2D>();
                    for (int k = 0; k < this.layers.Count; k++)
                    {
                        int data = this.layers.ElementAt(k).data[i, j];
                        if (data != -1) preBlend.AddLast(this.individualTiles[data]);
                    }
                    mapTiles[i, j] = this.BlendTextures(preBlend.ToArray(), BlendMode.PriorityBlend);

                    // One step closer to completion
                    Game1.GetInstance().currentLoadProgress += 9;
                }
            }

        }



        /// <summary>
        /// Splits a texture into an array of smaller textures of the specified size.
        /// </summary>
        /// <param name="original">The texture to be split into smaller textures</param>
        /// <param name="partWidth">The width of each of the smaller textures that will be contained in the returned array.</param>
        /// <param name="partHeight">The height of each of the smaller textures that will be contained in the returned array.</param>
        public Texture2D[] Split(Texture2D original, int partWidth, int partHeight, out int xCount, out int yCount)
        {
            yCount = original.Height / partHeight + (partHeight % original.Height == 0 ? 0 : 1);//The number of textures in each horizontal row
            xCount = original.Width / partWidth + (partWidth % original.Width == 0 ? 0 : 1);//The number of textures in each vertical column
            Texture2D[] r = new Texture2D[xCount * yCount];//Number of parts = (area of original) / (area of each part).
            int dataPerPart = partWidth * partHeight;//Number of pixels in each of the split parts

            //Get the pixel data from the original texture:
            Color[] originalData = new Color[original.Width * original.Height];
            original.GetData<Color>(originalData);

            int index = 0;
            for (int y = 0; y < yCount * partHeight; y += partHeight)
                for (int x = 0; x < xCount * partWidth; x += partWidth)
                {
                    //The texture at coordinate {x, y} from the top-left of the original texture
                    Texture2D part = new Texture2D(original.GraphicsDevice, partWidth, partHeight);
                    //The data for part
                    Color[] partData = new Color[dataPerPart];

                    //Fill the part data with colors from the original texture
                    for (int py = 0; py < partHeight; py++)
                        for (int px = 0; px < partWidth; px++)
                        {
                            int partIndex = px + py * partWidth;
                            //If a part goes outside of the source texture, then fill the overlapping part with Color.Transparent
                            if (y + py >= original.Height || x + px >= original.Width)
                                partData[partIndex] = Color.Transparent;
                            else
                                partData[partIndex] = originalData[(x + px) + (y + py) * original.Width];
                        }

                    //Fill the part with the extracted data
                    part.SetData<Color>(partData);
                    //Stick the part in the return array:                    
                    r[index++] = part;
                }
            //Return the array of parts.
            return r;
        }

        /// <summary>
        /// Disposes of the game map, unloading most resources used by it.
        /// </summary>
        public void Dispose()
        {
            // tileMap.Dispose();
            foreach (Texture2D tex in this.individualTiles)
            {
                tex.Dispose();
            }
            foreach (Texture2D tex in this.mapTiles)
            {
                tex.Dispose();
            }
            this.collisionMap.Dispose();
        }
    }
}
