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
using System.Threading;
using System.Xml;

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
            this.selectMapButton.visible = false;

            this.imageBounds = new Rectangle(
                this.GetScreenBounds().X + 15, this.GetScreenBounds().Y + 15,
                this.GetScreenBounds().Width - 30, this.GetScreenBounds().Height - 90);
        }

        public void OnSelectMapButtonClicked(XNAButton source)
        {
            new MapSelectionPanel(this, this.selectedMapLbl.text);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (mapPreview != null) sb.Draw(mapPreview, this.imageBounds, null, Color.White, 0f,
                Vector2.Zero, SpriteEffects.None, this.z - 0.001f);
        }

        /// <summary>
        /// Changes the map.
        /// </summary>
        /// <param name="mapName">The mapname to change to.</param>
        public void ChangeMap(String mapName)
        {
            this.selectedMapLbl.text = mapName;
            int tries = 0;
            int maxTries = 10;
            while (tries < maxTries)
            {
                try
                {
                    Stream stream = new FileStream(Game1.MAPS_FOLDER_LOCATION + "/" + mapName + "/" +
                                mapName + "_preview.png", FileMode.Open);
                    this.mapPreview = Texture2D.FromStream(Game1.GetInstance().GraphicsDevice,
                           stream);
                    stream.Close();
                    stream.Dispose();
                }
                catch (IOException ioe)
                {
                    Console.Error.WriteLine("Error opening " + Game1.MAPS_FOLDER_LOCATION + "/" + mapName + ".xml (try " + tries + ")");
                }
                Thread.Sleep(100);
                tries++;
            }
        }
    }
}
