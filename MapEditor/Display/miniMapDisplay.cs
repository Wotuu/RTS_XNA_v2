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

namespace MapEditor.Display
{
    public partial class miniMapDisplay : GraphicsDeviceControl
    {

        BasicEffect effect;
        Stopwatch timer;

        SpriteBatch sp;
        // Vertex positions and colors used to display a spinning triangle.
        public readonly VertexPositionColor[] Vertices =
        {
            new VertexPositionColor(new Vector3(-1, -1, 0), Color.Black),
            new VertexPositionColor(new Vector3( 1, -1, 0), Color.Black),
            new VertexPositionColor(new Vector3( 0,  1, 0), Color.Black),
        };

        protected override void Initialize()
        {
            GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Default;
            sp = new SpriteBatch(GraphicsDevice);

            
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        protected override void Draw()
        {
            sp.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            long ticks = DateTime.UtcNow.Ticks;
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Red);
            //if (Form1.tileMap != null && Form1.tilemaptexture != null)
            //{
                
                
            //    //foreach (TileMapLayer layer in Form1.tileMap.layers)
            //    //{
            //        for (int x = 0; x < Form1.tileMap.MapWidth; x++)
            //        {
            //            for (int y = 0; y < Form1.tileMap.MapHeight; y++)
            //            {
            //                int tile = Form1.tileMap.layers[0].GetTile(x , y );
            //                if (tile != -1)
            //                {
            //                    sp.Draw(Form1.tilemaptexture, new Rectangle(x * 3, y * 3, 3, 3), (Rectangle)Form1.tileset.tiles[tile], Color.White);
            //                }
            //            }
            //        }
            //    //}
               
            //}
            sp.End();

            long runtime = (DateTime.UtcNow.Ticks - ticks) / 10000;

            if (runtime < (1000 / 60))
            {
                int sleeptime = (1000 / 60) - (int)runtime;
                Thread.Sleep(sleeptime);
            }
            //GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
            //                                Vertices, 0, 1);
        }

    }
}
