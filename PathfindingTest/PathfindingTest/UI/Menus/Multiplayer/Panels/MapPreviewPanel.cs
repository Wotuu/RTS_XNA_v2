using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using SocketLibrary.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using XNAInterfaceComponents.ChildComponents;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class MapPreviewPanel : XNAPanel
    {
        private Rectangle imageBounds { get; set; }
        public Texture2D mapPreview { get; set; }

        public XNALabel selectedMapLbl { get; set; }
        public XNAButton selectMapButton { get; set; }

        public MapPreviewPanel(ParentComponent parent, Rectangle bounds) :
            base(parent, new Rectangle())
        {
            this.bounds = bounds;
            this.border = new Border(this, 2, Color.Pink);

            this.selectedMapLbl = new XNALabel(this, new Rectangle(5, this.bounds.Height - 70, this.bounds.Width - 10, 30), "No map selected");
            this.selectedMapLbl.textAlign = XNALabel.TextAlign.CENTER;
            this.selectedMapLbl.font = MenuManager.BIG_TEXTFIELD_FONT;
            this.selectedMapLbl.border = null;

            this.selectMapButton = new XNAButton(this, new Rectangle(5, this.bounds.Height - 35, this.bounds.Width - 10, 30), "Select map");
            this.selectMapButton.onClickListeners += this.OnSelectMapButtonClicked;
            this.imageBounds = new Rectangle(
                this.GetScreenBounds().X + 5, this.GetScreenBounds().Y + 5,
                this.GetScreenBounds().Width - 10, this.GetScreenBounds().Height - 10);
        }

        public void OnSelectMapButtonClicked(XNAButton source)
        {
            new MapSelectionPanel(this, this.selectedMapLbl.text);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (mapPreview != null) sb.Draw(mapPreview, this.imageBounds, Color.White);
        }
    }
}
