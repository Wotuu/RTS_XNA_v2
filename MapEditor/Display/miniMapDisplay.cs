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
                else
                {
                    scalefactor = maxheight / Form1.tileMap.MapHeight;
                }


                this.Width = Form1.tileMap.MapWidth * scalefactor;
                this.Height = Form1.tileMap.MapHeight * scalefactor;

                //foreach (TileMapLayer layer in Form1.tileMap.layers)
                //{
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
                //DRAW Rectangle
                // 800 *640
                rectheight = (640 / Engine.TileHeight) * scalefactor;
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

        void OnClick(object sender, MouseEventArgs e)
        {
            mousedown = true;
            rectposx = e.X - (rectwidth / 2);
            rectposy = e.Y - (rectheight / 2);
            
            if (rectposx >= 0 && (rectposx + rectwidth) <= this.Width && rectposy >= 0 && (rectposy + rectheight) <= this.Height)
            {
                Form1.camera.Position.X = e.X * (Engine.TileWidth / scalefactor);
                Form1.camera.Position.Y = e.Y * (Engine.TileHeight / scalefactor);
                this.Invalidate();
            }

        }


        void OnMove(object sender, MouseEventArgs e)
        {

            if (mousedown)
            {
                rectposx = e.X - (rectwidth / 2);
                rectposy = e.Y - (rectheight / 2);
                if (rectposx >= 0 && (rectposx + rectwidth) <= this.Width && rectposy >= 0 && (rectposy + rectheight) <= this.Height)
                {

                    this.Invalidate();
                }
            }

        }

        void OnUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            rectposx = e.X - (rectwidth / 2);
            rectposy = e.Y - (rectheight / 2);
            if (rectposx >= 0 && (rectposx + rectwidth) <= this.Width && rectposy >= 0 && (rectposy + rectheight) <= this.Height)
            {

                this.Invalidate();
            }
        }

    }
}
