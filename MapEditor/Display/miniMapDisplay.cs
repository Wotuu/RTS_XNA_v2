using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using MapEditor.TileMap;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using MapEditor.Helpers;

namespace MapEditor.Display
{
    public partial class miniMapDisplay : GraphicsDeviceControl
    {
        public int scalefactor = -1;
        int maxwidth;
        int maxheight;
        Shapes shapehelper = new Shapes();
        SpriteBatch sp;
        public int rectposx = -1;
        public int rectposy = -1;

        int rectheight = 0;
        int rectwidth = 0;
        bool mousedown = false;
        public bool onlyupdaterect = false;
        public Texture2D MiniMapText;
        // Vertex positions and colors used to display a spinning triangle.

        protected override void Initialize()
        {
            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Two;
            sp = new SpriteBatch(GraphicsDevice);
            maxheight = this.Height;
            maxwidth = this.Width;

            this.MouseDown += new MouseEventHandler(OnClick);
            this.MouseMove += new MouseEventHandler(OnMove);
            this.MouseUp += new MouseEventHandler(OnUp);
            // Hook the idle event to constantly redraw our animation.
            //Application.Idle += delegate { Invalidate(); };
        }
        long previousticks = 0;

        protected override void Draw()
        {

            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Two;
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.PowderBlue);
            long ticks = DateTime.UtcNow.Ticks;
            //if (ticks - previousticks > 10000000)
            //{


            sp.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (Form1.tileMap != null && Form1.tilemaptexture != null)
            {



                if (Form1.tileMap.MapWidth > Form1.tileMap.MapHeight)
                {
                    scalefactor = maxwidth / Form1.tileMap.MapWidth;
                }
                else if (Form1.tileMap.MapHeight > Form1.tileMap.MapHeight)
                {
                    scalefactor = maxheight / Form1.tileMap.MapHeight;
                }
                else
                {
                    scalefactor = maxwidth / Form1.tileMap.MapWidth;
                }



                this.Width = Form1.tileMap.MapWidth * scalefactor;
                this.Height = Form1.tileMap.MapHeight * scalefactor;
               

                if (!onlyupdaterect)
                {
                    for (int l = 0; l < Form1.tileMap.layers.Count; l++)
                    {

                        for (int x = 0; x < Form1.tileMap.MapWidth; x++)
                        {
                            for (int y = 0; y < Form1.tileMap.MapHeight; y++)
                            {
                                int tile = Form1.tileMap.layers[l].GetTile(x, y);
                                if (tile != -1)
                                {
                                    sp.Draw(Form1.tilemaptexture, new Rectangle(x * scalefactor, y * scalefactor, scalefactor, scalefactor), (Rectangle)Form1.tileset.tiles[tile], Color.White);

                                }
                            }
                        }
                    }
                    RenderTarget2D minimaptarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
                    MiniMapText = new Texture2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
                    
                    int[] data = new int[GraphicsDevice.PresentationParameters.BackBufferWidth * GraphicsDevice.PresentationParameters.BackBufferHeight];
                    GraphicsDevice.GetBackBufferData(data);
                    //minimaptarget.GetData(data);
                    MiniMapText.SetData(data);
                    sp.Draw(MiniMapText, Vector2.Zero, Color.White);
                }
                else
                {

                   
                    sp.Draw(MiniMapText, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                }
                onlyupdaterect = false;
                //DRAW Rectangle
                // 800 *640
                rectheight = (800 / Engine.TileHeight) * scalefactor;
                rectwidth = (800 / Engine.TileWidth) * scalefactor;

                //Stay within bounds
                shapehelper.DrawRectangle(new Rectangle(rectposx, rectposy, rectwidth, rectheight), Color.Red, sp, Form1.linetexture);
            }
            GraphicsDevice.GetType().GetField("lazyClearFlags", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GraphicsDevice, 0);




            sp.End();
            //long runtime = (DateTime.UtcNow.Ticks - ticks) / 10000;
            previousticks = DateTime.UtcNow.Ticks;
            //if (runtime < 1000)
            //{
            //    int sleeptime = 1000 - (int)runtime;
            //    Thread.Sleep(sleeptime);
            //}
            //GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
            // Vertices, 0, 1);


            //}
        }


        /// <summary>
        /// Creates a minimap - the new way!
        /// </summary>
        /// <param name="dummy">Just a dummy</param>
        public Texture2D CreateMiniMap(Boolean dummy)
        {
            Texture2D minimap;
            int targetWidth = (int)(Form1.tileMap.MapWidth * 3);
            int targetHeight = (int)(Form1.tileMap.MapHeight * 3);

            RenderTargetBinding[] OldRenderTargetBindings = this.GraphicsDevice.GetRenderTargets();
            this.GraphicsDevice.SetRenderTargets(null);
            // Get the Texture with the screen drawn on it
            // Create the Render Target to draw the scaled Texture to 
            RenderTarget2D newRenderTarget =
                new RenderTarget2D(this.GraphicsDevice, targetWidth, targetHeight);

            // Set the given Graphics Device to draw to the new Render Target 
            this.GraphicsDevice.SetRenderTarget(newRenderTarget);

            // Clear the scene 
            this.GraphicsDevice.Clear(Color.Transparent);

            // Create the new SpriteBatch that will be used to scale the Texture 
            SpriteBatch sb = new SpriteBatch(this.GraphicsDevice);

            // Draw the scaled Texture 
            sb.Begin();
            for (int i = 0; i < Form1.tileMap.MapWidth; i++)
            {
                for (int j = 0; j < Form1.tileMap.MapHeight; j++)
                {

                    //sb.Draw(this.map.mapTiles[i, j],
                    //    new Rectangle(i * 3, j * 3,
                    //        3, 3),
                    //    Color.White);
                    int tile = Form1.tileMap.layers[1].GetTile(i, j);
                    if (tile != -1)
                    {
                        sb.Draw(Form1.tilemaptexture, new Rectangle(i * 3, j * 3, 3, 3), (Rectangle)Form1.tileset.tiles[tile], Color.White);
                    }
                }
            }
            // End it
            sb.End();

            // Clear Graphics Device render target
            this.GraphicsDevice.SetRenderTarget(null);
            // Restore the given Graphics Device's Render Target Bindings
            this.GraphicsDevice.SetRenderTargets(OldRenderTargetBindings);

            minimap = new Texture2D(this.GraphicsDevice, targetWidth, targetHeight);
            int[] data = new int[targetWidth * targetHeight];
            newRenderTarget.GetData(data);
            minimap.SetData(data);
            // Set the Texture To Return to the scaled Texture 
            return minimap;
        }

        void OnClick(object sender, MouseEventArgs e)
        {

            mousedown = true;
            rectposx = e.X - (rectwidth / 2);
            rectposy = e.Y - (rectheight / 2);
            setrectpos();
            Form1.camera.Position.X = (rectposx * (Form1.tileMap.MapWidth * Engine.TileWidth)) / this.Width;
            Form1.camera.Position.Y = (rectposy * (Form1.tileMap.MapHeight * Engine.TileHeight)) / this.Height;
            this.Invalidate();
        }


        void OnMove(object sender, MouseEventArgs e)
        {

            if (mousedown)
            {
                rectposx = e.X - (rectwidth / 2);
                rectposy = e.Y - (rectheight / 2);
                setrectpos();
                Form1.camera.Position.X = (rectposx * (Form1.tileMap.MapWidth * Engine.TileWidth)) / this.Width;
                Form1.camera.Position.Y = (rectposy * (Form1.tileMap.MapHeight * Engine.TileHeight)) / this.Height;
                this.Invalidate();
            }

        }

        void OnUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            rectposx = e.X - (rectwidth / 2);
            rectposy = e.Y - (rectheight / 2);
            setrectpos();
            Form1.camera.Position.X = (rectposx * (Form1.tileMap.MapWidth * Engine.TileWidth)) / this.Width;
            Form1.camera.Position.Y = (rectposy * (Form1.tileMap.MapHeight * Engine.TileHeight)) / this.Height;
            this.Invalidate();
        }

        private void setrectpos()
        {
            onlyupdaterect = true;
            if (rectposx < 0)
            {
                rectposx = 0;
            }

            if ((rectposx + rectwidth) > this.Width)
            {
                rectposx = this.Width - rectwidth;
            }

            if (rectposy < 0)
            {
                rectposy = 0;
            }

            if ((rectposy + rectheight) > this.Height)
            {
                rectposy = this.Height - rectheight;
            }

        }

    }
}
