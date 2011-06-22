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

namespace MapEditor.Display
{
    public partial class TileMapDisplay : GraphicsDeviceControl
    {

        public event EventHandler OnInitialize;
        public event EventHandler OnDraw;
        public TileMapDisplay()
        {
            InitializeComponent();
        }

        public TileMapDisplay(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void Initialize()
        {
            if (OnInitialize != null)
                OnInitialize(this, null);
            Application.Idle += delegate { Invalidate(); };
        }

        protected override void Draw()
        {
            if (OnDraw != null)
                OnDraw(this, null);
        }

    }      
}
