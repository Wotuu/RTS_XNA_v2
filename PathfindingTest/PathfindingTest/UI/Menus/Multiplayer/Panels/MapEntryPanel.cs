using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml;
using PathfindingTest.UI.Menus.Multiplayer.Panels.PlayerLocation;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class MapEntryPanel : XNAPanel
    {
        public static int ENTRY_HEIGHT = 30;
        public static int ENTRY_WIDTH = 270;
        public int padding = 10;
        public XNARadioButton previewButton { get; set; }
        public Texture2D previewTexture { get; set; }

        public MapPlayerLocationGroup mapPlayerLocationGroup { get; set; }

        public MapEntryPanel(MapSelectionPanel parent, String mapname, int index) :
            base(parent, new Rectangle())
        {
            this.bounds = new Rectangle(5, index * (ENTRY_HEIGHT + padding) + padding, ENTRY_WIDTH, ENTRY_HEIGHT);

            this.previewButton = new XNARadioButton(this,
                new Rectangle(10, 5, 20, 20), parent.group, mapname);
            this.previewButton.onClickListeners += OnRadioButtonClick;

            this.mapPlayerLocationGroup = new MapPlayerLocationGroup(parent,
                new Point(MapEntryPanel.ENTRY_WIDTH + 20, 10), mapname);

            Stream stream = new FileStream(Game1.MAPS_FOLDER_LOCATION + "/" + mapname + "/" +
                        mapname + "_preview.png", FileMode.Open);
            this.previewTexture = Texture2D.FromStream(Game1.GetInstance().GraphicsDevice,
                   stream);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// When the user has clicked on this radio button.
        /// </summary>
        /// <param name="source">The radio button that was clicked on (kinda obsolete, but w/e)</param>
        public void OnRadioButtonClick(XNARadioButton source)
        {
            ((MapSelectionPanel)this.parent).OnMapSelectionChanged(this);
            this.mapPlayerLocationGroup.OnMapSelectionChanged(true);
        }
    }
}
