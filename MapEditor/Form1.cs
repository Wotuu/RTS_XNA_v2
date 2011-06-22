using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapEditor.HelperForms;
using MapEditor.TileMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Threading;
using MapEditor.Display;
using Microsoft.Xna.Framework.Input;
using MapEditor.Helpers;
using System.Reflection;
namespace MapEditor
{
    public partial class Form1 : Form
    {
#region class fields
        NewMapForm mapform = new NewMapForm();

        TileMap.TileMap tileMap = null;
        public SpriteBatch spriteBatch;

        public Texture2D cursor;
        public Texture2D EraseTexture;
        public Texture2D tilemaptexture;
        Texture2D linetexture;


        Texture2D CollisionMap;
        int[] CollisionData;

        Camera camera = new Camera();
        Tileset tileset = null;
        Vector2 position = new Vector2();

        public Rectangle selectedrectangle;

        TilePalette tilepalette;
        public string TileSetImagepath;

        bool trackMouse = false;

        bool isLeftMouseDown = false;
        bool isRightMouseDown = false;
        TileMapLayer currentLayer = null;
        Shapes shapehelper = new Shapes();
        Brush currentbrush = Brush.Paint;

        Assembly assembly;

        int mouseX;
        int mouseY;


        enum Brush
        {
            Paint = 1,
            Marquee = 2,
            painterase = 3,
            Marqueeerase = 4,
            Fill = 5,
            CollisionPaint = 6
        }
        public struct MarqueeData
        {
            public int Width;
            public int Height;
            public System.Drawing.Point InitialLocation;
            public System.Drawing.Point FinalLocation;
            public bool Show;
            public System.Drawing.Point InitialTile;
        }

        public static MarqueeData MarqueeSelection;

        public GraphicsDevice GraphicsDevice
        {
            get { return tileMapDisplay1.GraphicsDevice; }
        }
#endregion

        #region Constructor
        public Form1()
        {
            InitializeComponent();
            assembly = Assembly.GetExecutingAssembly();
            BtnShowGrid.CheckOnClick = true;
            tileMapDisplay1.OnInitialize += new EventHandler(tileDisplay1_OnInitialize);
            tileMapDisplay1.OnDraw += new EventHandler(tileDisplay1_OnDraw);

            tileMapDisplay1.MouseEnter +=
                 new EventHandler(tileDisplay1_MouseEnter);


            tileMapDisplay1.MouseLeave +=
                 new EventHandler(tileDisplay1_MouseLeave);


            tileMapDisplay1.MouseMove +=
            new MouseEventHandler(tileDisplay1_MouseMove);


            tileMapDisplay1.MouseDown +=
                 new MouseEventHandler(tileDisplay1_MouseDown);


            tileMapDisplay1.MouseUp +=
                 new MouseEventHandler(tileDisplay1_MouseUp);
            
            
        }
        #endregion

        #region Display 
        void tileDisplay1_OnInitialize(object sender, EventArgs e)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //tileMap = new TileMap.TileMap(32, 32);
            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Default;
            FileStream stream = new FileStream("./Content/cursor.png", FileMode.Open);
            cursor = Texture2D.FromStream(GraphicsDevice, stream);
            stream.Close();

            stream.Dispose();

            FileStream stream2 = new FileStream("./Content/solid.png", FileMode.Open);
            linetexture = Texture2D.FromStream(GraphicsDevice, stream2);
            stream2.Close();
            stream2.Dispose();

            EraseTexture = Util.GetCustomTexture2D(spriteBatch, Color.Black);
        }
        void tileDisplay1_OnDraw(object sender, EventArgs e)
        {

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            long ticks = DateTime.UtcNow.Ticks;

            Render();
            Logic();
            spriteBatch.End();



            long runtime = (DateTime.UtcNow.Ticks - ticks) / 10000;

            if (runtime < (1000 / 60))
            {

                int sleeptime = (1000 / 60) - (int)runtime;
                Thread.Sleep(sleeptime);
            }




        }

        private void Logic()
        {
            if (tileMap != null)
            {
                if (trackMouse)
                {
                    position.X = mouseX + camera.Position.X;
                    position.Y = mouseY + camera.Position.Y;
                }

                int tileX = (int)position.X / Engine.TileWidth;
                int tileY = (int)position.Y / Engine.TileHeight;



                // Movement Keyboard
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                {

                    camera.Position.X = MathHelper.Clamp(camera.Position.X - 5, 0, (tileMap.MapWidth - (tileMapDisplay1.Width / Engine.TileWidth)) * Engine.TileWidth);
                }

                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                {

                    camera.Position.X = MathHelper.Clamp(camera.Position.X + 5, 0, (tileMap.MapWidth - (tileMapDisplay1.Width / Engine.TileWidth)) * Engine.TileWidth);
                }

                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                {

                    camera.Position.Y = MathHelper.Clamp(camera.Position.Y - 5, 0, (tileMap.MapHeight - (tileMapDisplay1.Height / Engine.TileHeight)) * Engine.TileHeight);
                }

                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                {

                    camera.Position.Y = MathHelper.Clamp(camera.Position.Y + 5, 0, (tileMap.MapHeight - (tileMapDisplay1.Height / Engine.TileHeight)) * Engine.TileHeight);
                }

                // DRAW TEXTURE

                //if (isLeftMouseDown)
                //{
                //    //Marquee 


                //    //Texture ID opvragen
                //    for (int y = 0; y < selectedrectangle.Height / Engine.TileHeight; y++)
                //    {
                //        for (int x = 0; x < selectedrectangle.Width / Engine.TileWidth; x++)
                //        {
                //            currentLayer.SetTile(
                //            tileX + x,
                //            tileY + y,
                //            tileset.GetTileID(new Rectangle(selectedrectangle.X + (x * Engine.TileWidth), selectedrectangle.Y + (y * Engine.TileHeight), Engine.TileWidth, Engine.TileHeight), tilemaptexture));

                //        }
                //    }
                //}

            }
        }

        private void Render()
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.PowderBlue);
            if (tileMap != null)
            {


                DrawGrid();

                for (int i = 0; i < tileMap.layers.Count; i++)
                {
                    DrawLayer(i);
                }

                if ((currentbrush == Brush.Marquee || currentbrush == Brush.Marqueeerase) && MarqueeSelection.Show == true)
                {
                    DrawMarqueeSelection();
                }

                //DrawLayer(0);
                DrawDisplay();
                //CollisionmapDrawn
                
                    spriteBatch.Draw(CollisionMap, new Rectangle(0, 0, tileMapDisplay1.Width, tileMapDisplay1.Height), Color.White);
             
                
            }
        }

        private void DrawLayer(int layer)
        {

            if (tilemaptexture != null)
            {
                Vector2 SquareOffSet = new Vector2(camera.Position.X % Engine.TileWidth, camera.Position.Y % Engine.TileHeight);
                int tile;
                Rectangle tileRect = new Rectangle(
                    0,
                    0,
                    Engine.TileWidth,
                    Engine.TileHeight);

                Vector2 pos = new Vector2(camera.Position.X / Engine.TileWidth, camera.Position.Y / Engine.TileHeight);
                //New Only draw screen tiles
                for (int y = 0; y < tileMapDisplay1.Height / Engine.TileHeight + 1; y++)
                {

                    for (int x = 0; x < tileMapDisplay1.Width / Engine.TileWidth + 1; x++)
                    {
                        if (x + (int)(camera.Position.X / Engine.TileWidth) < tileMap.MapWidth && y + (int)(camera.Position.Y / Engine.TileWidth) < tileMap.MapHeight)
                        {

                            tile = tileMap.layers[layer].GetTile(x + (int)(camera.Position.X / Engine.TileWidth), y + (int)(camera.Position.Y / Engine.TileWidth));
                            if (tile != -1)
                            {
                                tileRect.X = (x * Engine.TileWidth)
                                    - (int)SquareOffSet.X;
                                tileRect.Y = (y * Engine.TileHeight)
                                    - (int)SquareOffSet.Y;

                                spriteBatch.Draw(tilemaptexture,
                                       tileRect,
                                           (Rectangle)tileset.tiles[tile],
                                           Color.White);

                            }
                        }
                    }
                }

            }
        }
        private void DrawGrid()
        {

            Vector2 SquareOffSet = new Vector2(camera.Position.X % Engine.TileWidth, camera.Position.Y % Engine.TileHeight);


            if (BtnShowGrid.Checked)
            {

                //Horizontal Lines
                for (int x = 0; x <= tileMap.MapHeight; x++)
                {

                    shapehelper.DrawLine(spriteBatch, new Vector2(0, (x * Engine.TileWidth) - (int)SquareOffSet.Y), new Vector2(tileMap.MapWidth * Engine.TileWidth, (x * Engine.TileWidth) - (int)SquareOffSet.Y), Color.White, linetexture);


                }

                //Vertical Lines
                for (int y = 0; y <= tileMap.MapHeight; y++)
                {

                    shapehelper.DrawLine(spriteBatch, new Vector2((y * Engine.TileWidth) - (int)SquareOffSet.X, 0), new Vector2((y * Engine.TileWidth) - (int)SquareOffSet.X, tileMap.MapHeight * Engine.TileHeight), Color.White, linetexture);
                }
            }
        }
        private void DrawDisplay()
        {
            Vector2 FirstSquare = new Vector2(camera.Position.X / Engine.TileWidth, camera.Position.Y / Engine.TileHeight);
            Vector2 SquareOffSet = new Vector2(camera.Position.X % Engine.TileWidth, camera.Position.Y % Engine.TileHeight);

            //GRID Tekenen met SquareOffset !

            ////Selected TILE Draw
            if (currentbrush == Brush.Paint)
            {
                Rectangle dest = new Rectangle(((int)position.X / Engine.TileWidth) * Engine.TileWidth - (int)camera.Position.X,
                                               ((int)position.Y / Engine.TileHeight) * Engine.TileHeight - (int)camera.Position.Y,
                            selectedrectangle.Width,
                            selectedrectangle.Height);


                if (tilemaptexture != null)
                {
                    spriteBatch.Draw(tilemaptexture, dest, selectedrectangle, new Color(255, 255, 255, 200));
                }
            }

            //SELECTION CURSOR RED
            spriteBatch.Draw(cursor,
                new Rectangle(
                    (int)(position.X / Engine.TileWidth) * Engine.TileWidth
                     - (int)camera.Position.X,
                    (int)(position.Y / Engine.TileHeight) * Engine.TileHeight
                     - (int)camera.Position.Y,
                    Engine.TileWidth,
                    Engine.TileHeight),
                    Color.Red);
            

        }
        #endregion

        #region Menu Strip events
        private void openTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();

            file.InitialDirectory = "../Tilesets";
            file.Filter = "Image files (*.img *.jpg *.png *.bmp)|*.img; *.jpg; *.png; *.bmp|All Files (*.*)| *.*";
            file.Title = "Load tiles";

            if (file.ShowDialog() == DialogResult.OK)
            {
                TileSetImagepath = file.FileName;

                //Texture Texture2D

                FileStream stream = new FileStream(TileSetImagepath, FileMode.Open);
                tilemaptexture = Texture2D.FromStream(GraphicsDevice, stream);

                stream.Close();
                stream.Dispose();

                tileset = new Tileset(tilemaptexture);

                //Image inladen in panel / palette
                tilepalette = new TilePalette(this);
                PnlPaletteContainer.Controls.Add(tilepalette);
                tilepalette.SetImage(TileSetImagepath);




            }




        }

        private void createNewMap()
        {
            //get values from newmapform

            tileMap = new TileMap.TileMap(mapform.MapWidth, mapform.MapHeight);
            CollisionMap = new Texture2D(GraphicsDevice, mapform.MapWidth * Engine.TileWidth, mapform.MapHeight * Engine.TileHeight);
            CollisionData  = new int[(mapform.MapWidth * Engine.TileWidth) * (mapform.MapHeight * Engine.TileHeight)];
            currentLayer = tileMap.layers[0];
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapform.ShowDialog();
            createNewMap();
        }
        #endregion

        #region Mouse Handlers


        void tileDisplay1_MouseEnter(object sender, EventArgs e)
        {
            ChangeMouseCursor(currentbrush);
            trackMouse = true;
        }

        void tileDisplay1_MouseLeave(object sender, EventArgs e)
        {
            trackMouse = false;
        }


        void tileDisplay1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            if (isLeftMouseDown)
            {
                //final location opslaan
                MarqueeSelection.FinalLocation = new System.Drawing.Point((int)e.X, (int)e.Y);
                if (currentbrush == Brush.Paint || currentbrush == Brush.CollisionPaint)
                {
                    tileDisplay1_MouseDown(sender, e);
                }
                
            }
        }

        void tileDisplay1_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)

                if (tilemaptexture == null)
                {
                    return;
                }
            isLeftMouseDown = true;


            MarqueeSelection.InitialLocation = new System.Drawing.Point((int)e.X, (int)e.Y);
            MarqueeSelection.FinalLocation = MarqueeSelection.InitialLocation;
            MarqueeSelection.Width = 1;
            MarqueeSelection.Height = 1;

            MarqueeSelection.Show = true;
            DoBrushLogic(sender, e);
            if (e.Button == MouseButtons.Right)
            {
                isRightMouseDown = true;
            }
        }

        void tileDisplay1_MouseUp(object sender, MouseEventArgs e)
        {
            if (tilemaptexture == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                isLeftMouseDown = false;
                //MarqueeSelection.InitialLocation = new System.Drawing.Point() ;

                DoBrushLogic(sender, e);
                MarqueeSelection.Show = false;
                MarqueeSelection.FinalLocation = MarqueeSelection.InitialLocation;

            }
            if (e.Button == MouseButtons.Right)
                isRightMouseDown = false;
        }
        #endregion

        #region ToolstipEvents


        private void BtnLayerDown_Click(object sender, EventArgs e)
        {
            if (tileMap != null)
            {
                int layernumber = int.Parse(TBCurrentLayer.Text);
                if (layernumber != 0)
                {
                    currentLayer = tileMap.layers[layernumber - 1];
                    TBCurrentLayer.Text = (layernumber - 1).ToString();
                }
            }
        }


        private void BtnLayerUp_Click(object sender, EventArgs e)
        {
            if (tileMap != null)
            {
                int layernumber = int.Parse(TBCurrentLayer.Text);
                if (layernumber + 1 < tileMap.layers.Count)
                {
                    currentLayer = tileMap.layers[layernumber + 1];
                    TBCurrentLayer.Text = (layernumber + 1).ToString();
                }
            }
        }
        #endregion

        #region MarqueeHelpers
        private void DrawMarqueeSelection()
        {
            System.Drawing.Rectangle aux_inital_rectangle = SnapToGrid(MarqueeSelection.InitialLocation);
            System.Drawing.Rectangle aux_final_rectangle = SnapToGrid(MarqueeSelection.FinalLocation);
            Rectangle rect = CreateMarqueeArea(aux_inital_rectangle, aux_final_rectangle);
            shapehelper.DrawRectangle(rect, Color.Red, spriteBatch, linetexture);


            MarqueeSelection.Width = rect.Width / Engine.TileWidth;
            MarqueeSelection.Height = rect.Height / Engine.TileHeight;
            if (rect.Width > 0 && rect.Height > 0 && selectedrectangle.Width > 0 && selectedrectangle.Height > 0)
            {

                if (currentbrush == Brush.Marquee)
                {
                    MarqueePreview(rect, false);
                }
                else
                {
                    MarqueePreview(rect, true);
                }
            }
        }
        private Rectangle CreateMarqueeArea(System.Drawing.Rectangle RectangleA, System.Drawing.Rectangle RectangleB)
        {
            Int32 min_size = Engine.TileWidth;
            System.Drawing.Size rect_size = new System.Drawing.Size();
            System.Drawing.Point top_point = new System.Drawing.Point();
            System.Drawing.Point bottom_point = new System.Drawing.Point();

            if (RectangleA.Left < RectangleB.Left)
            {
                top_point.X = RectangleA.Left;
                bottom_point.X = RectangleB.Right;
            }
            else
            {
                top_point.X = RectangleB.Left;
                bottom_point.X = RectangleA.Right;
            }

            if (RectangleA.Top < RectangleB.Top)
            {
                top_point.Y = RectangleA.Top;
                bottom_point.Y = RectangleB.Bottom;
            }
            else
            {
                top_point.Y = RectangleB.Top;
                bottom_point.Y = RectangleA.Bottom;
            }

            if (top_point.X <= bottom_point.X)
            {
                rect_size.Width = bottom_point.X - top_point.X;
            }
            else
            {
                rect_size.Width = top_point.X - bottom_point.X;
            }

            if (top_point.Y <= bottom_point.Y)
            {
                rect_size.Height = bottom_point.Y - top_point.Y;
            }
            else
            {
                rect_size.Height = top_point.Y - bottom_point.Y;
            }

            if (rect_size.Width == 0)
            {
                rect_size.Width = Engine.TileWidth;
            }

            if (rect_size.Height == 0)
            {
                rect_size.Height = Engine.TileWidth;
            }

            return new Rectangle(top_point.X, top_point.Y, rect_size.Width, rect_size.Height);

        }

        System.Drawing.Rectangle SnapToGrid(System.Drawing.Point Location)
        {
            System.Drawing.Point point = new System.Drawing.Point((((int)Location.X + (int)camera.Position.X) / Engine.TileWidth) * Engine.TileWidth - (int)camera.Position.X, (((int)Location.Y + (int)camera.Position.Y )/ Engine.TileWidth) * Engine.TileWidth - (int)camera.Position.Y);
            return new System.Drawing.Rectangle(point, new System.Drawing.Size(Engine.TileHeight, Engine.TileWidth));
        }
        #endregion

        

       

        

    }
}
