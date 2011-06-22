using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapEditor.TileMap;

namespace MapEditor.Display
{
    public partial class TilePalette : Panel
    {
        Form1 baseform;
        Image tilesetimage;

        Pen pen_grid;
        Pen pen_highlight_a;
        Pen pen_highlight_b;

        Boolean marquee_selection;


        static System.Drawing.Rectangle selected_area;
        System.Drawing.Rectangle start_rectangle;
        System.Drawing.Rectangle end_rectangle;

        public TilePalette()
        {
            InitializeComponent();
            
        }
        public TilePalette(Form1 baseform)
        {
            InitializeComponent();
            pen_grid = new Pen(System.Drawing.Color.Black, 1.0f);
            pen_highlight_a = new Pen(System.Drawing.Color.Fuchsia, 2.0f);
            pen_highlight_b = new Pen(System.Drawing.Color.White, 0.5f);

            this.AutoScroll = true;
            this.DoubleBuffered = true;

            this.baseform = baseform;
        }

        //Override panel paint
        protected override void OnPaint(PaintEventArgs e)
        {
            if (tilesetimage == null)
            {
                return;
            }

            if (tilesetimage != null)
            {
                e.Graphics.DrawImage(tilesetimage, 0, 0, tilesetimage.Width, tilesetimage.Height);
            }
            for (int id_y = 0; id_y < tilesetimage.Height; id_y += Engine.TileWidth)
            {
                e.Graphics.DrawLine(pen_grid, new System.Drawing.Point(0, id_y), new System.Drawing.Point(tilesetimage.Width, id_y));
            }
            for (int id_x = 0; id_x < tilesetimage.Width; id_x += Engine.TileWidth)
            {
                e.Graphics.DrawLine(pen_grid, new System.Drawing.Point(id_x, 0), new System.Drawing.Point(id_x, tilesetimage.Height));
            }



            if (selected_area.Width == 0 || selected_area != null)
            {
                // Draw highlighted cell
                e.Graphics.DrawRectangle(pen_highlight_a, selected_area);
                e.Graphics.DrawRectangle(pen_highlight_b, selected_area);
            }

        }

        //Display the loaded image
        public void SetImage(string FileName)
        {
            tilesetimage = null;
            tilesetimage = Image.FromFile(FileName);
            this.MinimumSize = new Size((tilesetimage.Width / Engine.TileWidth) * Engine.TileWidth, (tilesetimage.Height / Engine.TileWidth) * Engine.TileWidth);
            this.Size = this.MinimumSize;
        }//SetImage()



        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left)
            {
                marquee_selection = false;
            }

            start_rectangle = GetSelectedCell(e.Location);
            System.Drawing.Rectangle aux_rect = GetSelectedCell(e.Location);

            selected_area = aux_rect;
            baseform.selectedrectangle = new Microsoft.Xna.Framework.Rectangle(aux_rect.X, aux_rect.Y, 20, 20);
            this.Invalidate(true);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (img_tile_set == null)
            //{
            //    // No need to check anything yet
            //    return;
            //}

            System.Drawing.Rectangle aux_rect = GetSelectedCell(e.Location);
            //try
            //{
            //    Tile aux_tile = GLB_Data.TilePalette[aux_rect.X / GLB_Data.MapSize.TileSize, aux_rect.Y / GLB_Data.MapSize.TileSize];
            //}
            //catch
            //{
            //    return;
            //}

            if (e.Button == MouseButtons.Left)
            {
                marquee_selection = true;
                end_rectangle = GetSelectedCell(e.Location);

                selected_area = CreateMarqueeArea(start_rectangle, end_rectangle);
                baseform.selectedrectangle = new Microsoft.Xna.Framework.Rectangle(selected_area.X, selected_area.Y, selected_area.Width,selected_area.Height);
            }
            else
            {
                return;
            }

            this.Invalidate(true);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (marquee_selection)
            {

                if (selected_area.Width == Engine.TileHeight && selected_area.Height == Engine.TileHeight)
                {
                    marquee_selection = false;
                    return;
                }
       

            }
        }

        public System.Drawing.Rectangle GetSelectedCell(System.Drawing.Point Location)
        {
            //if (GLB_Data.TilesTexture == null)
            //{
            //    return new Rectangle(0, 0, 0, 0);
            //}

            ////for (int id_y = 0; id_y < tilesetimage.Height * Engine.TileWidth; id_y += Engine.TileWidth)
            ////{
            ////    for (int id_x = 0; id_x < tilesetimage.Width * Engine.TileHeight; id_x += Engine.TileHeight)
            ////    {
            ////        if ((Location.X >= id_x && Location.X <= id_x + Engine.TileHeight) &&
            ////            (Location.Y >= id_y && Location.Y <= id_y + Engine.TileHeight))
            ////        {
            ////             FOUND                        
            ////            return new System.Drawing.Rectangle(new System.Drawing.Point(id_x, id_y), new Size(Engine.TileHeight, Engine.TileWidth));
            ////        }
            ////    }
            ////}
            for (int id_y = 0; id_y < tilesetimage.Height; id_y += Engine.TileWidth)
            {
                for (int id_x = 0; id_x < tilesetimage.Width; id_x += Engine.TileHeight)
                {
                    if ((Location.X >= id_x && Location.X <= id_x + Engine.TileHeight) &&
                        (Location.Y >= id_y && Location.Y <= id_y + Engine.TileHeight))
                    {
                        // FOUND                        
                        return new System.Drawing.Rectangle(new System.Drawing.Point(id_x, id_y), new Size(Engine.TileHeight, Engine.TileWidth));
                    }
                }
            }

            // NOT FOUND
            return new System.Drawing.Rectangle(0, 0, 0, 0);
        }
        private System.Drawing.Rectangle CreateMarqueeArea(System.Drawing.Rectangle RectangleA, System.Drawing.Rectangle RectangleB)
        {
            Int32 min_size = Engine.TileWidth;
            Size rect_size = new Size();
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

            return new System.Drawing.Rectangle(top_point, rect_size);

        }
    }
}
