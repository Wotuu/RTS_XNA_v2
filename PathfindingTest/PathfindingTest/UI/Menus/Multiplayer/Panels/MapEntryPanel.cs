using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.Components;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class MapEntryPanel : XNAPanel
    {
        public static int ENTRY_HEIGHT = 30;
        public static int ENTRY_WIDTH = 200;
        public int padding = 5;
        public XNARadioButton previewButton { get; set; }

        public MapEntryPanel(MapSelectionPanel parent, String mapname, int index) :
            base( parent, new Rectangle() )
        {
            this.bounds = new Rectangle(5, index * (ENTRY_HEIGHT + padding) + padding, ENTRY_WIDTH, ENTRY_HEIGHT);

            this.previewButton = new XNARadioButton(this,
                new Rectangle(10, padding, 20, 20), parent.group, mapname);

        }
    }
}
