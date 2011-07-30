using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using SocketLibrary.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class MapPreviewPanel : XNAPanel
    {

        public Texture2D mapPreview;

        public MapPreviewPanel(ParentComponent parent, Rectangle bounds) :
            base(parent, new Rectangle())
        {
            this.bounds = bounds;
            this.border = new Border(this, 2, Color.Pink);


        }
    }
}
