using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapEditor;
using MapEditor.TileMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using AStarCollisionMap.Pathfinding;
using MapEditor.Pathfinding;
namespace MapEditor
{
    partial class Form1
    {

        #region Brushes
        private void BtnPaint_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Paint;
            CheckButton(BtnPaint);
        }

        private void BtnMarqueePaint_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Marquee;
            CheckButton(BtnMarqueePaint);
        }

        private void BtnErase_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.painterase;
            CheckButton(BtnErase);
        }

        private void BtnMarqueeerase_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Marqueeerase;
            CheckButton(BtnMarqueeerase);
        }

        private void BtnFill_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Fill;
            CheckButton(BtnFill);
        }

        private void BtnDrawCollision_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.CollisionPaint;
            CheckButton(BtnDrawCollision);
        }


        private void BtnEraseCollision_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.EraseCollision;
            CheckButton(BtnEraseCollision);
        }

        private void BtnAddNode_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.AddNode;
            CheckButton(BtnAddNode);
        }

        private void BtnRemoveNode_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.RemoveNode;
            CheckButton(BtnRemoveNode);
        }

        private void Addplayer_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.AddPlayer;
            CheckButton(Addplayer);
        }

        private void Removeplayer_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.RemovePlayer;
            CheckButton(Removeplayer);
        }




        private void CheckButton(ToolStripButton btn)
        {
            BtnPaint.Checked = false;
            BtnErase.Checked = false;
            BtnMarqueeerase.Checked = false;
            BtnMarqueePaint.Checked = false;
            BtnFill.Checked = false;
            BtnDrawCollision.Checked = false;
            BtnEraseCollision.Checked = false;
            BtnAddNode.Checked = false;
            BtnRemoveNode.Checked = false;
            Addplayer.Checked = false;
            Removeplayer.Checked = false;
            btn.Checked = true;
        }
        private void TBCollisionSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+") && e.KeyChar != (char)System.Windows.Forms.Keys.Back)
                e.Handled = true;
        }

        private void TBCollisionSize_TextChanged(object sender, EventArgs e)
        {
            if (TBCollisionSize.Text.Equals(""))
            {
                TBCollisionSize.Text = 1 + "";
            }
            if (int.Parse(TBCollisionSize.Text) < 5)
            {
                TBCollisionSize.Text = 1 + "";
            }

            collisionsize = int.Parse(TBCollisionSize.Text);
        }
        private void DoBrushLogic(object sender, MouseEventArgs e)
        {


            switch (currentbrush)
            {
                case Brush.Paint:
                    Paint(sender, e);
                    break;
                case Brush.Marquee:
                    MarqueePaint();
                    break;
                case Brush.painterase:
                    ErasePaint(sender, e);
                    break;
                case Brush.Marqueeerase:
                    MarqueeErase();
                    break;
                case Brush.Fill:
                    Fill();
                    break;
                case Brush.CollisionPaint:
                    CollisionPaint(sender, e);
                    break;
                case Brush.EraseCollision:
                    EraseCollision(sender, e);
                    break;
                case Brush.AddNode:
                    AddNode(sender, e);
                    break;
                case Brush.RemoveNode:
                    RemoveNode(sender, e);
                    break;
                case Brush.AddPlayer:
                    AddPlayer(sender, e);
                    break;
                case Brush.RemovePlayer:
                    RemovePlayer(sender, e);
                    break;

            }
            //Invalidate minimap ? 



        }

        private void Paint(object sender, MouseEventArgs e)
        {

            if (trackMouse)
            {
                position.X = e.X + camera.Position.X;
                position.Y = e.Y + camera.Position.Y;
            }
            int tileX = (int)position.X / Engine.TileWidth;
            int tileY = (int)position.Y / Engine.TileHeight;
            //Texture ID opvragen
            for (int y = 0; y < selectedrectangle.Height / Engine.TileHeight; y++)
            {
                for (int x = 0; x < selectedrectangle.Width / Engine.TileWidth; x++)
                {
                    int TileID = tileset.GetTileID(new Rectangle(selectedrectangle.X + (x * Engine.TileWidth), selectedrectangle.Y + (y * Engine.TileHeight), Engine.TileWidth, Engine.TileHeight), tilemaptexture);
                    if (TileID == (int)TileLegend.Water)
                    {
                        //Automatisch Edges toevoegen
                        //AutoEdges(tileX + x, tileY + y, TileID, 0);
                    }

                    currentLayer.SetTile(
                    tileX + x,
                    tileY + y,
                    TileID);
                }
            }
        }
        private void MarqueePaint()
        {
            System.Drawing.Rectangle aux_inital_rectangle = SnapToGrid(MarqueeSelection.InitialLocation);
            System.Drawing.Rectangle aux_final_rectangle = SnapToGrid(MarqueeSelection.FinalLocation);
            Rectangle rect = CreateMarqueeArea(aux_inital_rectangle, aux_final_rectangle);
            for (int y = 0; y < MarqueeSelection.Height; y++)
            {
                for (int x = 0; x < MarqueeSelection.Width; x++)
                {
                    int tileid;
                    int testx = x % (selectedrectangle.Width / Engine.TileWidth);
                    int testy = y % (selectedrectangle.Height / Engine.TileHeight);

                    int seltilex = (selectedrectangle.X + (testx * Engine.TileWidth));
                    int seltiley = (selectedrectangle.Y + (testy * Engine.TileHeight));

                    tileid = tileset.GetTileID(new Rectangle(seltilex, seltiley, Engine.TileWidth, Engine.TileHeight), tilemaptexture);
                    currentLayer.SetTile(x + ((int)camera.Position.X + rect.X) / Engine.TileWidth,
                         y + (rect.Y + (int)camera.Position.Y) / Engine.TileHeight,
                         tileid);
                }
            }
        }

        private void MarqueeErase()
        {
            System.Drawing.Rectangle aux_inital_rectangle = SnapToGrid(MarqueeSelection.InitialLocation);
            System.Drawing.Rectangle aux_final_rectangle = SnapToGrid(MarqueeSelection.FinalLocation);
            Rectangle rect = CreateMarqueeArea(aux_inital_rectangle, aux_final_rectangle);
            for (int y = 0; y < MarqueeSelection.Height; y++)
            {
                for (int x = 0; x < MarqueeSelection.Width; x++)
                {
                    currentLayer.SetTile(x + ((int)camera.Position.X + rect.X) / Engine.TileWidth,
                        y + (rect.Y + (int)camera.Position.Y) / Engine.TileHeight,
                        -1);
                }
            }
        }
        private void ErasePaint(object sender, MouseEventArgs e)
        {
            if (trackMouse)
            {
                position.X = e.X + camera.Position.X;
                position.Y = e.Y + camera.Position.Y;
            }
            int tileX = (int)position.X / Engine.TileWidth;
            int tileY = (int)position.Y / Engine.TileHeight;
            //Texture ID opvragen

            currentLayer.SetTile(
            tileX,
            tileY,
            -1);


        }
        private void MarqueePreview(Rectangle rect, bool erase)
        {
            for (int y = 0; y < MarqueeSelection.Height; y++)
            {
                for (int x = 0; x < MarqueeSelection.Width; x++)
                {
                    int testx = x % (selectedrectangle.Width / Engine.TileWidth);
                    int testy = y % (selectedrectangle.Height / Engine.TileHeight);
                    if (!erase)
                    {
                        spriteBatch.Draw(tilemaptexture, new Rectangle(
                            rect.X + x * Engine.TileWidth,
                            rect.Y + y * Engine.TileHeight,
                            Engine.TileWidth,
                            Engine.TileHeight),
                            new Rectangle(selectedrectangle.X + testx * Engine.TileWidth,
                                selectedrectangle.Y + testy * Engine.TileHeight,
                                Engine.TileWidth,
                                Engine.TileHeight),
                                Color.White);
                    }
                    else
                    {
                        //black texture drawen ?
                        spriteBatch.Draw(EraseTexture, new Rectangle(
                            rect.X + x * Engine.TileWidth,
                            rect.Y + y * Engine.TileHeight,
                            Engine.TileWidth,
                            Engine.TileHeight), Color.FromNonPremultiplied(255, 255, 255, 150));

                    }

                }
            }
        }
        private void Fill()
        {
            for (int x = 0; x < tileMap.MapWidth; x++)
            {
                for (int y = 0; y < tileMap.MapHeight; y++)
                {
                    int tileid;

                    int testx = x % (selectedrectangle.Width / Engine.TileWidth);
                    int testy = y % (selectedrectangle.Height / Engine.TileHeight);

                    int seltilex = (selectedrectangle.X + (testx * Engine.TileWidth));
                    int seltiley = (selectedrectangle.Y + (testy * Engine.TileHeight));

                    tileid = tileset.GetTileID(new Rectangle(seltilex, seltiley, Engine.TileWidth, Engine.TileHeight), tilemaptexture);
                    currentLayer.SetTile(x, y, tileid);
                }
            }
        }

        private void CollisionPaint(object sender, MouseEventArgs e)
        {

            //Rectangle colrect = new Rectangle(e.X + (int)camera.Position.X, e.Y + (int)camera.Position.Y, 10, 10);
            //CollisionMap.GetData<int>(CollisionData);
            //GraphicsDevice.Textures[0] = null; 
            //for (int x = 0; x < colrect.Width; x++)
            //{
            //    for (int y = 0; y < colrect.Height; y++)
            //    {
            //        CollisionData[PointToIndex(colrect.X + x, colrect.Y + y)] = (int)Color.Black.PackedValue;
            //    }
            //}
            ////Texture2D copyfromtexture = new  Texture2D(GraphicsDevice, CollisionMap.Width, CollisionMap.Height);
            //CollisionMap.SetData(CollisionData);
            Rectangle colrect = new Rectangle(e.X + (int)camera.Position.X, e.Y + (int)camera.Position.Y, collisionsize, collisionsize);
            CollisionMap.UpdateCollisionMap(colrect, true);

        }

        private void EraseCollision(object sender, MouseEventArgs e)
        {
            Rectangle colrect = new Rectangle(e.X + (int)camera.Position.X, e.Y + (int)camera.Position.Y, collisionsize, collisionsize);
            CollisionMap.UpdateCollisionMap(colrect, false);
        }


        private void AddNode(object sender, MouseEventArgs e)
        {
            new Pathfinding.Node(CollisionMap, e.X + (int)camera.Position.X, e.Y + (int)camera.Position.Y, GraphicsDevice);
        }

        private void RemoveNode(object sender, MouseEventArgs e)
        {
            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
            foreach (Node node in manager.nodeList)
            {
                if (node.GetDrawRectangle().Contains(e.X, e.Y))
                {
                    node.Destroy();
                    manager.nodeList.Remove(node);
                    break;
                }
            }
        }

        private void AddPlayer(object sender, MouseEventArgs e)
        {
            Players.Add(new Helpers.Player(e.X + (int)camera.Position.X, e.Y + (int)camera.Position.Y));
        }

        private void RemovePlayer(object sender, MouseEventArgs e)
        {
            Rectangle mouserect = new Rectangle(e.X - nodetext.Width, e.Y - nodetext.Height, nodetext.Width * 2, nodetext.Height * 2);
            List<Helpers.Player> removelist = new List<Helpers.Player>();
            foreach (Helpers.Player p in Players)
            {
                if (p.x > (mouserect.X + (int)camera.Position.X) && p.x < ((mouserect.X + (int)camera.Position.X) + mouserect.Width) && p.y > (mouserect.Y + (int)camera.Position.Y) && p.y < ((mouserect.Y + (int)camera.Position.Y) + mouserect.Height))
                {
                    removelist.Add(p);
                }
            }
            foreach (Helpers.Player p in removelist)
            {
                Players.Remove(p);
            }

        }

        /// <summary>
        /// Returns the index of a certain point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The index.</returns>
        public int PointToIndex(int x, int y)
        {
            return x + y * (tileMap.MapWidth * Engine.TileWidth);
        }
        #endregion

        #region MouseCursor change
        private void ChangeMouseCursor(Brush brush)
        {
            Stream MouseCursor;
            switch (brush)
            {
                case Brush.Paint:
                    MouseCursor = (assembly.GetManifestResourceStream("MapEditor.Content.BrushCursor.cur"));
                    tileMapDisplay1.Cursor = new Cursor(MouseCursor);
                    break;
                case Brush.Marquee:
                    MouseCursor = (assembly.GetManifestResourceStream("MapEditor.Content.MarqueeCursor.cur"));
                    tileMapDisplay1.Cursor = new Cursor(MouseCursor);
                    break;
                case Brush.Marqueeerase:
                    MouseCursor = (assembly.GetManifestResourceStream("MapEditor.Content.MarqueeEraser.cur"));
                    tileMapDisplay1.Cursor = new Cursor(MouseCursor);
                    break;
                case Brush.painterase:
                    MouseCursor = (assembly.GetManifestResourceStream("MapEditor.Content.EraserCursor.cur"));
                    tileMapDisplay1.Cursor = new Cursor(MouseCursor);
                    break;
                case Brush.Fill:
                    MouseCursor = (assembly.GetManifestResourceStream("MapEditor.Content.BucketCursor.cur"));
                    tileMapDisplay1.Cursor = new Cursor(MouseCursor);
                    break;
            }
        }
        #endregion


        enum TileLegend
        {
            Grass = 117,
            Dirt = 87,
            Ice = 145,
            Water = 58,
            GrassWaterTop = 262,
            GrassWaterBottom = 320,
            GrassWaterLeft = 290,
            GrassWaterRight = 292,
            GrassWaterTopLeft = 261,
            GrassWaterTopRight = 263,
            GrassWaterBottomLeft = 319,
            GrassWaterBottomRight = 321,
            GrassWaterSmallTopLeft = 203,
            GrassWaterSmallBottomRight = 233,
            GrassWaterSmallTopRight = 204,
            GrassWaterSmallBottomLeft = 232,
            IceWaterTop = 265,
            IceWaterBottom = 323,
            IceWaterLeft = 293,
            IceWaterRight = 295,
            IceWaterTopLeft = 264,
            IceWaterTopRight = 266,
            IceWaterBottomLeft = 322,
            IceWaterBottomRight = 324,
            IceWaterSmallTopLeft = 206,
            IceWaterSmallBottomRight = 236,
            IceWaterSmallTopRight = 207,
            IceWaterSmallBottomLeft = 235,
            DirtWaterTop = 306,
            DirtWaterBottom = 364,
            DirtWaterLeft = 334,
            DirtWaterRight = 336,
            DirtWaterTopLeft = 305,
            DirtWaterTopRight = 307,
            DirtWaterBottomLeft = 363,
            DirtWaterBottomRight = 365,
            DirtWaterSmallTopLeft = 248,
            DirtWaterSmallBottomRight = 278,
            DirtWaterSmallTopRight = 249,
            DirtWaterSmallBottomLeft = 277,
        }

        enum tiletype
        {
            grass =1,
            dirt =2,
            ice =3
        }

        private void AutoEdges_Click(object sender, EventArgs e)
        {
            AutoEdgesFunc();
        }


        private void AutoEdgesFunc()
        {

            for (int y = 0; y < tileMap.MapHeight; y++)
            {
                for (int x = 0; x < tileMap.MapWidth; x++)
                {

                    if (currentLayer.GetTile(x, y) == (int)TileLegend.Water)
                    {
                        bool topwater = false;
                        bool rightwater = false;
                        bool bottomwater = false;
                        bool leftwater = false;
                        bool toprightland = false;
                        bool topleftland = false;
                        bool bottomrightland = false;
                        bool bottomleftland = false;

                        tiletype toplefttype = tiletype.grass;
                        tiletype toprighttype = tiletype.grass;
                        tiletype bottomlefttype = tiletype.grass;
                        tiletype bottomrighttype = tiletype.grass;
                        #region Grass    
                        #region Top of Tile

                        if (y != 0)
                        {
                            int abovetile = currentLayer.GetTile(x, y - 1);
                            bool abovegrastype = false;

                            if (abovetile == (int)TileLegend.GrassWaterTopLeft || abovetile == (int)TileLegend.GrassWaterTopRight || abovetile == (int)TileLegend.GrassWaterBottomLeft || abovetile == (int)TileLegend.GrassWaterBottomRight || abovetile == (int)TileLegend.GrassWaterTop || abovetile == (int)TileLegend.GrassWaterBottom || abovetile == (int)TileLegend.GrassWaterLeft || abovetile == (int)TileLegend.GrassWaterRight)
                            {
                                abovegrastype = true;
                            }
                            //TOP
                            if (abovetile == (int)TileLegend.Grass || abovegrastype)
                            {
                                currentLayer.SetTile(x, y - 1, (int)TileLegend.GrassWaterTop);
                            }
                            else if (abovetile == (int)TileLegend.Water)
                            {
                                topwater = true;
                            }

                            //TOPLEFT
                            if (x != 0)
                            {
                                
                                int aboveleft = currentLayer.GetTile(x - 1, y - 1);
                                bool grastype = false;
                                if (aboveleft == (int)TileLegend.GrassWaterTopLeft || aboveleft == (int)TileLegend.GrassWaterTopRight || aboveleft == (int)TileLegend.GrassWaterBottomLeft || aboveleft == (int)TileLegend.GrassWaterBottomRight || aboveleft == (int)TileLegend.GrassWaterTop || aboveleft == (int)TileLegend.GrassWaterBottom || aboveleft == (int)TileLegend.GrassWaterLeft || aboveleft == (int)TileLegend.GrassWaterRight)
                                {
                                    grastype = true;
                                }
                                if (aboveleft == (int)TileLegend.Grass)
                                {
                                    currentLayer.SetTile(x - 1, y - 1, (int)TileLegend.GrassWaterTopLeft);
                                }
                                else if (aboveleft == (int)TileLegend.Grass || grastype)
                                {
                                    topleftland = true;
                                    toplefttype = tiletype.grass;
                                }
                            }

                            //TOPRIGHT
                            if (x != tileMap.MapWidth -1)
                            {
                                int aboveright = currentLayer.GetTile(x + 1, y - 1);
                                bool grastype = false;
                                if (aboveright == (int)TileLegend.GrassWaterTopLeft || aboveright == (int)TileLegend.GrassWaterTopRight || aboveright == (int)TileLegend.GrassWaterBottomLeft || aboveright == (int)TileLegend.GrassWaterBottomRight || aboveright == (int)TileLegend.GrassWaterTop || aboveright == (int)TileLegend.GrassWaterBottom || aboveright == (int)TileLegend.GrassWaterLeft || aboveright == (int)TileLegend.GrassWaterRight)
                                {
                                    grastype = true;
                                }
                                if (aboveright == (int)TileLegend.Grass)
                                {
                                    currentLayer.SetTile(x + 1, y - 1, (int)TileLegend.GrassWaterTopRight);
                                }
                                else if (aboveright == (int)TileLegend.Grass || grastype)
                                {
                                    toprightland = true;
                                    toprighttype = tiletype.grass;
                                }
                            }
                        }
                        #endregion
                        #region Left of Tile
                        if (x != 0)
                        {
                            int lefttile = currentLayer.GetTile(x - 1, y);
                            bool leftgrastype = false;

                            if (lefttile == (int)TileLegend.GrassWaterTopLeft || lefttile == (int)TileLegend.GrassWaterTopRight || lefttile == (int)TileLegend.GrassWaterBottomLeft || lefttile == (int)TileLegend.GrassWaterBottomRight || lefttile == (int)TileLegend.GrassWaterTop || lefttile == (int)TileLegend.GrassWaterBottom || lefttile == (int)TileLegend.GrassWaterLeft || lefttile == (int)TileLegend.GrassWaterRight)
                            {
                                leftgrastype = true;
                            }

                            if (lefttile == (int)TileLegend.Grass || leftgrastype)
                            {
                                currentLayer.SetTile(x - 1, y, (int)TileLegend.GrassWaterLeft);
                            }
                            else if (lefttile == (int)TileLegend.Water)
                            {
                                leftwater = true;
                            }
                        }


                        #endregion
                        #region Right of tile
                        if (x != tileMap.MapWidth -1)
                        {
                            int righttile = currentLayer.GetTile(x + 1, y);
                            bool rightgrastype = false;

                            if (righttile == (int)TileLegend.GrassWaterTopLeft || righttile == (int)TileLegend.GrassWaterTopRight || righttile == (int)TileLegend.GrassWaterBottomLeft || righttile == (int)TileLegend.GrassWaterBottomRight || righttile == (int)TileLegend.GrassWaterTop || righttile == (int)TileLegend.GrassWaterBottom || righttile == (int)TileLegend.GrassWaterLeft || righttile == (int)TileLegend.GrassWaterRight)
                            {
                                rightgrastype = true;
                            }

                            if (righttile == (int)TileLegend.Grass || rightgrastype)
                            {
                                currentLayer.SetTile(x + 1, y, (int)TileLegend.GrassWaterRight);
                            }
                            else if (righttile == (int)TileLegend.Water)
                            {
                                rightwater = true;
                            }
                        }

                        #endregion
                        #region Below Tile
                        if (y != tileMap.MapHeight -1 )
                        {
                            int belowtile = currentLayer.GetTile(x, y + 1);
                            bool belowgrasttype = false;

                            if (belowtile == (int)TileLegend.GrassWaterTopLeft || belowtile == (int)TileLegend.GrassWaterTopRight || belowtile == (int)TileLegend.GrassWaterBottomLeft || belowtile == (int)TileLegend.GrassWaterBottomRight || belowtile == (int)TileLegend.GrassWaterTop || belowtile == (int)TileLegend.GrassWaterBottom || belowtile == (int)TileLegend.GrassWaterLeft || belowtile == (int)TileLegend.GrassWaterRight)
                            {
                                belowgrasttype = true;
                            }
                            //Bottom
                            if (belowtile == (int)TileLegend.Grass || belowgrasttype)
                            {
                                currentLayer.SetTile(x, y + 1, (int)TileLegend.GrassWaterBottom);
                            }
                            else if (belowtile == (int)TileLegend.Water)
                            {
                                bottomwater = true;
                            }
                            //Bottomleft
                            if (x != 0)
                            {

                                int belowleft = currentLayer.GetTile(x - 1, y + 1);
                                bool grastype = false;
                                if (belowleft == (int)TileLegend.GrassWaterTopLeft || belowleft == (int)TileLegend.GrassWaterTopRight || belowleft == (int)TileLegend.GrassWaterBottomLeft || belowleft == (int)TileLegend.GrassWaterBottomRight || belowleft == (int)TileLegend.GrassWaterTop || belowleft == (int)TileLegend.GrassWaterBottom || belowleft == (int)TileLegend.GrassWaterLeft || belowleft == (int)TileLegend.GrassWaterRight)
                                {
                                    grastype = true;
                                }
                                if (belowleft == (int)TileLegend.Grass)
                                {
                                    currentLayer.SetTile(x - 1, y + 1, (int)TileLegend.GrassWaterBottomLeft);
                                }
                                else if (belowleft == (int)TileLegend.Grass || grastype)
                                {
                                    bottomleftland = true;
                                    bottomlefttype = tiletype.grass;
                                }

                            }
                            //BottomRight
                            if (x != tileMap.MapWidth -1)
                            {
                                int belowright = currentLayer.GetTile(x + 1, y + 1);
                                bool grastype = false;
                                if (belowright == (int)TileLegend.GrassWaterTopLeft || belowright == (int)TileLegend.GrassWaterTopRight || belowright == (int)TileLegend.GrassWaterBottomLeft || belowright == (int)TileLegend.GrassWaterBottomRight || belowright == (int)TileLegend.GrassWaterTop || belowright == (int)TileLegend.GrassWaterBottom || belowright == (int)TileLegend.GrassWaterLeft || belowright == (int)TileLegend.GrassWaterRight)
                                {
                                    grastype = true;
                                }
                                if (belowright == (int)TileLegend.Grass)
                                {
                                    currentLayer.SetTile(x + 1, y + 1, (int)TileLegend.GrassWaterBottomRight);
                                }
                                if (belowright == (int)TileLegend.Grass || grastype)
                                {
                                    bottomrightland = true;
                                    bottomrighttype = tiletype.grass;
                                }
                            }
                        }

                        #endregion
     

                        #endregion

                        #region ice
                        #region Top of Tile

                        if (y != 0)
                        {
                            int abovetile = currentLayer.GetTile(x, y - 1);
                            bool abovegrastype = false;

                            if (abovetile == (int)TileLegend.IceWaterTopLeft || abovetile == (int)TileLegend.IceWaterTopRight || abovetile == (int)TileLegend.IceWaterBottomLeft || abovetile == (int)TileLegend.IceWaterBottomRight || abovetile == (int)TileLegend.IceWaterTop || abovetile == (int)TileLegend.IceWaterBottom || abovetile == (int)TileLegend.IceWaterLeft || abovetile == (int)TileLegend.IceWaterRight)
                            {
                                abovegrastype = true;
                            }
                            //TOP
                            if (abovetile == (int)TileLegend.Ice || abovegrastype)
                            {
                                currentLayer.SetTile(x, y - 1, (int)TileLegend.IceWaterTop);
                            }
                            else if (abovetile == (int)TileLegend.Water)
                            {
                                topwater = true;
                            }

                            //TOPLEFT
                            if (x != 0)
                            {

                                int aboveleft = currentLayer.GetTile(x - 1, y - 1);
                                bool grastype = false;
                                if (aboveleft == (int)TileLegend.IceWaterTopLeft || aboveleft == (int)TileLegend.IceWaterTopRight || aboveleft == (int)TileLegend.IceWaterBottomLeft || aboveleft == (int)TileLegend.IceWaterBottomRight || aboveleft == (int)TileLegend.IceWaterTop || aboveleft == (int)TileLegend.IceWaterBottom || aboveleft == (int)TileLegend.IceWaterLeft || aboveleft == (int)TileLegend.IceWaterRight)
                                {
                                    grastype = true;
                                }
                                if (aboveleft == (int)TileLegend.Ice)
                                {
                                    currentLayer.SetTile(x - 1, y - 1, (int)TileLegend.IceWaterTopLeft);
                                }
                                else if (aboveleft == (int)TileLegend.Ice || grastype)
                                {
                                    topleftland = true;
                                    toplefttype = tiletype.ice;
                                }
                            }

                            //TOPRIGHT
                            if (x != tileMap.MapWidth - 1)
                            {
                                int aboveright = currentLayer.GetTile(x + 1, y - 1);
                                bool grastype = false;
                                if (aboveright == (int)TileLegend.IceWaterTopLeft || aboveright == (int)TileLegend.IceWaterTopRight || aboveright == (int)TileLegend.IceWaterBottomLeft || aboveright == (int)TileLegend.IceWaterBottomRight || aboveright == (int)TileLegend.IceWaterTop || aboveright == (int)TileLegend.IceWaterBottom || aboveright == (int)TileLegend.IceWaterLeft || aboveright == (int)TileLegend.IceWaterRight)
                                {
                                    grastype = true;
                                }
                                if (aboveright == (int)TileLegend.Ice)
                                {
                                    currentLayer.SetTile(x + 1, y - 1, (int)TileLegend.IceWaterTopRight);
                                }
                                else if (aboveright == (int)TileLegend.Ice || grastype)
                                {
                                    toprightland = true;
                                    toprighttype = tiletype.ice;
                                }
                            }
                        }
                        #endregion
                        #region Left of Tile
                        if (x != 0)
                        {
                            int lefttile = currentLayer.GetTile(x - 1, y);
                            bool leftgrastype = false;

                            if (lefttile == (int)TileLegend.IceWaterTopLeft || lefttile == (int)TileLegend.IceWaterTopRight || lefttile == (int)TileLegend.IceWaterBottomLeft || lefttile == (int)TileLegend.IceWaterBottomRight || lefttile == (int)TileLegend.IceWaterTop || lefttile == (int)TileLegend.IceWaterBottom || lefttile == (int)TileLegend.IceWaterLeft || lefttile == (int)TileLegend.IceWaterRight)
                            {
                                leftgrastype = true;
                            }

                            if (lefttile == (int)TileLegend.Ice || leftgrastype)
                            {
                                currentLayer.SetTile(x - 1, y, (int)TileLegend.IceWaterLeft);
                            }
                            else if (lefttile == (int)TileLegend.Water)
                            {
                                leftwater = true;
                            }
                        }


                        #endregion
                        #region Right of tile
                        if (x != tileMap.MapWidth - 1)
                        {
                            int righttile = currentLayer.GetTile(x + 1, y);
                            bool rightgrastype = false;

                            if (righttile == (int)TileLegend.IceWaterTopLeft || righttile == (int)TileLegend.IceWaterTopRight || righttile == (int)TileLegend.IceWaterBottomLeft || righttile == (int)TileLegend.IceWaterBottomRight || righttile == (int)TileLegend.IceWaterTop || righttile == (int)TileLegend.IceWaterBottom || righttile == (int)TileLegend.IceWaterLeft || righttile == (int)TileLegend.IceWaterRight)
                            {
                                rightgrastype = true;
                            }

                            if (righttile == (int)TileLegend.Ice || rightgrastype)
                            {
                                currentLayer.SetTile(x + 1, y, (int)TileLegend.IceWaterRight);
                            }
                            else if (righttile == (int)TileLegend.Water)
                            {
                                rightwater = true;
                            }
                        }

                        #endregion
                        #region Below Tile
                        if (y != tileMap.MapHeight - 1)
                        {
                            int belowtile = currentLayer.GetTile(x, y + 1);
                            bool belowgrasttype = false;

                            if (belowtile == (int)TileLegend.IceWaterTopLeft || belowtile == (int)TileLegend.IceWaterTopRight || belowtile == (int)TileLegend.IceWaterBottomLeft || belowtile == (int)TileLegend.IceWaterBottomRight || belowtile == (int)TileLegend.IceWaterTop || belowtile == (int)TileLegend.IceWaterBottom || belowtile == (int)TileLegend.IceWaterLeft || belowtile == (int)TileLegend.IceWaterRight)
                            {
                                belowgrasttype = true;
                            }
                            //Bottom
                            if (belowtile == (int)TileLegend.Ice || belowgrasttype)
                            {
                                currentLayer.SetTile(x, y + 1, (int)TileLegend.IceWaterBottom);
                            }
                            else if (belowtile == (int)TileLegend.Water)
                            {
                                bottomwater = true;
                            }
                            //Bottomleft
                            if (x != 0)
                            {

                                int belowleft = currentLayer.GetTile(x - 1, y + 1);
                                bool grastype = false;
                                if (belowleft == (int)TileLegend.IceWaterTopLeft || belowleft == (int)TileLegend.IceWaterTopRight || belowleft == (int)TileLegend.IceWaterBottomLeft || belowleft == (int)TileLegend.IceWaterBottomRight || belowleft == (int)TileLegend.IceWaterTop || belowleft == (int)TileLegend.IceWaterBottom || belowleft == (int)TileLegend.IceWaterLeft || belowleft == (int)TileLegend.IceWaterRight)
                                {
                                    grastype = true;
                                }
                                if (belowleft == (int)TileLegend.Ice)
                                {
                                    currentLayer.SetTile(x - 1, y + 1, (int)TileLegend.IceWaterBottomLeft);
                                }
                                else if (belowleft == (int)TileLegend.Ice || grastype)
                                {
                                    bottomleftland = true;
                                    bottomlefttype = tiletype.ice;
                                }

                            }
                            //BottomRight
                            if (x != tileMap.MapWidth - 1)
                            {
                                int belowright = currentLayer.GetTile(x + 1, y + 1);
                                bool grastype = false;
                                if (belowright == (int)TileLegend.IceWaterTopLeft || belowright == (int)TileLegend.IceWaterTopRight || belowright == (int)TileLegend.IceWaterBottomLeft || belowright == (int)TileLegend.IceWaterBottomRight || belowright == (int)TileLegend.IceWaterTop || belowright == (int)TileLegend.IceWaterBottom || belowright == (int)TileLegend.IceWaterLeft || belowright == (int)TileLegend.IceWaterRight)
                                {
                                    grastype = true;
                                }
                                if (belowright == (int)TileLegend.Ice)
                                {
                                    currentLayer.SetTile(x + 1, y + 1, (int)TileLegend.IceWaterBottomRight);
                                }
                                if (belowright == (int)TileLegend.Ice || grastype)
                                {
                                    bottomrightland = true;
                                    bottomrighttype = tiletype.ice;
                                }
                            }
                        }

                        #endregion
 
                        #endregion

                        #region dirt
                        #region Top of Tile

                        if (y != 0)
                        {
                            int abovetile = currentLayer.GetTile(x, y - 1);
                            bool abovegrastype = false;

                            if (abovetile == (int)TileLegend.DirtWaterTopLeft || abovetile == (int)TileLegend.DirtWaterTopRight || abovetile == (int)TileLegend.DirtWaterBottomLeft || abovetile == (int)TileLegend.DirtWaterBottomRight || abovetile == (int)TileLegend.DirtWaterTop || abovetile == (int)TileLegend.DirtWaterBottom || abovetile == (int)TileLegend.DirtWaterLeft || abovetile == (int)TileLegend.DirtWaterRight)
                            {
                                abovegrastype = true;
                            }
                            //TOP
                            if (abovetile == (int)TileLegend.Dirt || abovegrastype)
                            {
                                currentLayer.SetTile(x, y - 1, (int)TileLegend.DirtWaterTop);
                            }
                            else if (abovetile == (int)TileLegend.Water)
                            {
                                topwater = true;
                            }

                            //TOPLEFT
                            if (x != 0)
                            {

                                int aboveleft = currentLayer.GetTile(x - 1, y - 1);
                                bool grastype = false;
                                if (aboveleft == (int)TileLegend.DirtWaterTopLeft || aboveleft == (int)TileLegend.DirtWaterTopRight || aboveleft == (int)TileLegend.DirtWaterBottomLeft || aboveleft == (int)TileLegend.DirtWaterBottomRight || aboveleft == (int)TileLegend.DirtWaterTop || aboveleft == (int)TileLegend.DirtWaterBottom || aboveleft == (int)TileLegend.DirtWaterLeft || aboveleft == (int)TileLegend.DirtWaterRight)
                                {
                                    grastype = true;
                                }
                                if (aboveleft == (int)TileLegend.Dirt)
                                {
                                    currentLayer.SetTile(x - 1, y - 1, (int)TileLegend.DirtWaterTopLeft);
                                }
                                else if (aboveleft == (int)TileLegend.Dirt || grastype)
                                {
                                    topleftland = true;
                                    toplefttype = tiletype.dirt;
                                }
                            }

                            //TOPRIGHT
                            if (x != tileMap.MapWidth - 1)
                            {
                                int aboveright = currentLayer.GetTile(x + 1, y - 1);
                                bool grastype = false;
                                if (aboveright == (int)TileLegend.DirtWaterTopLeft || aboveright == (int)TileLegend.DirtWaterTopRight || aboveright == (int)TileLegend.DirtWaterBottomLeft || aboveright == (int)TileLegend.DirtWaterBottomRight || aboveright == (int)TileLegend.DirtWaterTop || aboveright == (int)TileLegend.DirtWaterBottom || aboveright == (int)TileLegend.DirtWaterLeft || aboveright == (int)TileLegend.DirtWaterRight)
                                {
                                    grastype = true;
                                }
                                if (aboveright == (int)TileLegend.Dirt)
                                {
                                    currentLayer.SetTile(x + 1, y - 1, (int)TileLegend.DirtWaterTopRight);
                                }
                                else if (aboveright == (int)TileLegend.Dirt || grastype)
                                {
                                    toprightland = true;
                                    toprighttype = tiletype.dirt;
                                }
                            }
                        }
                        #endregion
                        #region Left of Tile
                        if (x != 0)
                        {
                            int lefttile = currentLayer.GetTile(x - 1, y);
                            bool leftgrastype = false;

                            if (lefttile == (int)TileLegend.DirtWaterTopLeft || lefttile == (int)TileLegend.DirtWaterTopRight || lefttile == (int)TileLegend.DirtWaterBottomLeft || lefttile == (int)TileLegend.DirtWaterBottomRight || lefttile == (int)TileLegend.DirtWaterTop || lefttile == (int)TileLegend.DirtWaterBottom || lefttile == (int)TileLegend.DirtWaterLeft || lefttile == (int)TileLegend.DirtWaterRight)
                            {
                                leftgrastype = true;
                            }

                            if (lefttile == (int)TileLegend.Dirt || leftgrastype)
                            {
                                currentLayer.SetTile(x - 1, y, (int)TileLegend.DirtWaterLeft);
                            }
                            else if (lefttile == (int)TileLegend.Water)
                            {
                                leftwater = true;
                            }
                        }


                        #endregion
                        #region Right of tile
                        if (x != tileMap.MapWidth - 1)
                        {
                            int righttile = currentLayer.GetTile(x + 1, y);
                            bool rightgrastype = false;

                            if (righttile == (int)TileLegend.DirtWaterTopLeft || righttile == (int)TileLegend.DirtWaterTopRight || righttile == (int)TileLegend.DirtWaterBottomLeft || righttile == (int)TileLegend.DirtWaterBottomRight || righttile == (int)TileLegend.DirtWaterTop || righttile == (int)TileLegend.DirtWaterBottom || righttile == (int)TileLegend.DirtWaterLeft || righttile == (int)TileLegend.DirtWaterRight)
                            {
                                rightgrastype = true;
                            }

                            if (righttile == (int)TileLegend.Dirt || rightgrastype)
                            {
                                currentLayer.SetTile(x + 1, y, (int)TileLegend.DirtWaterRight);
                            }
                            else if (righttile == (int)TileLegend.Water)
                            {
                                rightwater = true;
                            }
                        }

                        #endregion
                        #region Below Tile
                        if (y != tileMap.MapHeight - 1)
                        {
                            int belowtile = currentLayer.GetTile(x, y + 1);
                            bool belowgrasttype = false;

                            if (belowtile == (int)TileLegend.DirtWaterTopLeft || belowtile == (int)TileLegend.DirtWaterTopRight || belowtile == (int)TileLegend.DirtWaterBottomLeft || belowtile == (int)TileLegend.DirtWaterBottomRight || belowtile == (int)TileLegend.DirtWaterTop || belowtile == (int)TileLegend.DirtWaterBottom || belowtile == (int)TileLegend.DirtWaterLeft || belowtile == (int)TileLegend.DirtWaterRight)
                            {
                                belowgrasttype = true;
                            }
                            //Bottom
                            if (belowtile == (int)TileLegend.Dirt || belowgrasttype)
                            {
                                currentLayer.SetTile(x, y + 1, (int)TileLegend.DirtWaterBottom);
                            }
                            else if (belowtile == (int)TileLegend.Water)
                            {
                                bottomwater = true;
                            }
                            //Bottomleft
                            if (x != 0)
                            {

                                int belowleft = currentLayer.GetTile(x - 1, y + 1);
                                bool grastype = false;
                                if (belowleft == (int)TileLegend.DirtWaterTopLeft || belowleft == (int)TileLegend.DirtWaterTopRight || belowleft == (int)TileLegend.DirtWaterBottomLeft || belowleft == (int)TileLegend.DirtWaterBottomRight || belowleft == (int)TileLegend.DirtWaterTop || belowleft == (int)TileLegend.DirtWaterBottom || belowleft == (int)TileLegend.DirtWaterLeft || belowleft == (int)TileLegend.DirtWaterRight)
                                {
                                    grastype = true;
                                }
                                if (belowleft == (int)TileLegend.Dirt)
                                {
                                    currentLayer.SetTile(x - 1, y + 1, (int)TileLegend.DirtWaterBottomLeft);
                                }
                                else if (belowleft == (int)TileLegend.Dirt || grastype)
                                {
                                    bottomleftland = true;
                                    toplefttype = tiletype.dirt;
                                }

                            }
                            //BottomRight
                            if (x != tileMap.MapWidth - 1)
                            {
                                int belowright = currentLayer.GetTile(x + 1, y + 1);
                                bool grastype = false;
                                if (belowright == (int)TileLegend.DirtWaterTopLeft || belowright == (int)TileLegend.DirtWaterTopRight || belowright == (int)TileLegend.DirtWaterBottomLeft || belowright == (int)TileLegend.DirtWaterBottomRight || belowright == (int)TileLegend.DirtWaterTop || belowright == (int)TileLegend.DirtWaterBottom || belowright == (int)TileLegend.DirtWaterLeft || belowright == (int)TileLegend.DirtWaterRight)
                                {
                                    grastype = true;
                                }
                                if (belowright == (int)TileLegend.Dirt)
                                {
                                    currentLayer.SetTile(x + 1, y + 1, (int)TileLegend.DirtWaterBottomRight);
                                }
                                if (belowright == (int)TileLegend.Dirt || grastype)
                                {
                                    bottomrightland = true;
                                    bottomrighttype = tiletype.dirt;
                                }
                            }
                        }

                        #endregion
                        #region Corners
                        if (topwater && leftwater && topleftland)
                        {
                            switch (toplefttype)
                            {
                                case tiletype.grass:
                                    currentLayer.SetTile(x - 1, y - 1, (int)TileLegend.GrassWaterSmallTopLeft);
                                    break;
                                case tiletype.ice:
                                    currentLayer.SetTile(x - 1, y - 1, (int)TileLegend.IceWaterSmallTopLeft);
                                    break;
                                case tiletype.dirt:
                                    currentLayer.SetTile(x - 1, y - 1, (int)TileLegend.DirtWaterSmallTopLeft);
                                    break;
                            }
                            
                        }
                        if (topwater && rightwater && toprightland)
                        {
                            switch (toprighttype)
                            {
                                case tiletype.grass:
                                    currentLayer.SetTile(x + 1, y - 1, (int)TileLegend.GrassWaterSmallTopRight);
                                    break;
                                case tiletype.dirt:
                                    currentLayer.SetTile(x + 1, y - 1, (int)TileLegend.DirtWaterSmallTopRight);
                                    break;
                                case tiletype.ice:
                                    currentLayer.SetTile(x + 1, y - 1, (int)TileLegend.IceWaterSmallTopRight);
                                    break;
                            }
                            
                        }

                        if (bottomwater && leftwater && bottomleftland)
                        {
                            switch (bottomlefttype)
                            {
                                case tiletype.grass:
                                    currentLayer.SetTile(x - 1, y + 1, (int)TileLegend.GrassWaterSmallBottomLeft);
                                    break;
                                case tiletype.dirt:
                                    currentLayer.SetTile(x - 1, y + 1, (int)TileLegend.DirtWaterSmallBottomLeft);
                                    break;
                                case tiletype.ice:
                                    currentLayer.SetTile(x - 1, y + 1, (int)TileLegend.GrassWaterSmallBottomLeft);
                                    break;
                            }
                            
                        }
                        if (bottomwater && rightwater && bottomrightland)
                        {

                            switch (bottomrighttype)
                            {
                                case tiletype.grass:
                                    currentLayer.SetTile(x + 1, y + 1, (int)TileLegend.GrassWaterSmallBottomRight);
                                    break;
                                case tiletype.dirt:
                                    currentLayer.SetTile(x + 1, y + 1, (int)TileLegend.DirtWaterSmallBottomRight);
                                    break;
                                case tiletype.ice:
                                    currentLayer.SetTile(x + 1, y + 1, (int)TileLegend.IceWaterSmallBottomRight);
                                    break;
                            }
                            
                        }
                        #endregion
                        #endregion

                    }
                }
            }
        }

    }
}
