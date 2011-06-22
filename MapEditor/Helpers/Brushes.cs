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
namespace MapEditor
{
    partial class Form1
    {
        
        #region Brushes
        private void BtnPaint_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Paint;
            BtnPaint.Checked = true;
            BtnErase.Checked = false;
            BtnMarqueeerase.Checked = false;
            BtnMarqueePaint.Checked = false;
            BtnFill.Checked = false;
            BtnDrawCollision.Checked = false;
        }

        private void BtnMarqueePaint_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Marquee;
            BtnPaint.Checked = false;
            BtnErase.Checked = false;
            BtnMarqueeerase.Checked = false;
            BtnMarqueePaint.Checked = true;
            BtnFill.Checked = false;
            BtnDrawCollision.Checked = false;
        }

        private void BtnErase_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.painterase;
            BtnPaint.Checked = false;
            BtnErase.Checked = true;
            BtnMarqueeerase.Checked = false;
            BtnMarqueePaint.Checked = false;
            BtnFill.Checked = false;
            BtnDrawCollision.Checked = false;
        }

        private void BtnMarqueeerase_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Marqueeerase;
            BtnPaint.Checked = false;
            BtnErase.Checked = false;
            BtnMarqueeerase.Checked = true;
            BtnMarqueePaint.Checked = false;
            BtnFill.Checked = false;
            BtnDrawCollision.Checked = false;
        }

        private void BtnFill_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.Fill;
            BtnPaint.Checked = false;
            BtnErase.Checked = false;
            BtnMarqueeerase.Checked = false;
            BtnMarqueePaint.Checked = false;
            BtnFill.Checked = true;
            BtnDrawCollision.Checked = false;
        }

        private void BtnDrawCollision_Click(object sender, EventArgs e)
        {
            currentbrush = Brush.CollisionPaint;
            BtnPaint.Checked = false;
            BtnErase.Checked = false;
            BtnMarqueeerase.Checked = false;
            BtnMarqueePaint.Checked = false;
            BtnFill.Checked = false;
            BtnDrawCollision.Checked = false;
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
                   

            }
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
                    currentLayer.SetTile(
                    tileX + x,
                    tileY + y,
                    tileset.GetTileID(new Rectangle(selectedrectangle.X + (x * Engine.TileWidth), selectedrectangle.Y + (y * Engine.TileHeight), Engine.TileWidth, Engine.TileHeight), tilemaptexture));

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
                            Engine.TileHeight), Color.Black);

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
            Rectangle colrect = new Rectangle(e.X + (int)camera.Position.X, e.Y + (int)camera.Position.Y, 10, 10);
            CollisionMap.GetData<int>(CollisionData);

            for (int x = 0; x < colrect.Width; x++)
            {
                for (int y = 0; y < colrect.Height; y++)
                {
                    CollisionData[PointToIndex(colrect.X + x, colrect.Y + y)] = (int)Color.Black.PackedValue;
                }
            }
            Texture2D copyfromtexture = new  Texture2D(GraphicsDevice, CollisionMap.Width, CollisionMap.Height);
            copyfromtexture.SetData(CollisionData);




            CollisionMap = copyfromtexture;
           
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

    }
}
