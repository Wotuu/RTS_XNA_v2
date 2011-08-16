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

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class MapEntryPanel : XNAPanel
    {
        public static int ENTRY_HEIGHT = 30;
        public static int ENTRY_WIDTH = 270;
        public int padding = 10;
        public XNARadioButton previewButton { get; set; }
        public Texture2D previewTexture { get; set; }

        public int selectedMapWidth { get; set; }
        public int selectedMapHeight { get; set; }

        public LinkedList<Point> mapPlayerLocations = new LinkedList<Point>();

        public MapEntryPanel(MapSelectionPanel parent, String mapname, int index) :
            base( parent, new Rectangle() )
        {
            this.bounds = new Rectangle(5, index * (ENTRY_HEIGHT + padding) + padding, ENTRY_WIDTH, ENTRY_HEIGHT);

            this.previewButton = new XNARadioButton(this,
                new Rectangle(10, 5, 20, 20), parent.group, mapname);
            this.previewButton.onClickListeners += OnRadioButtonClick;

            Stream stream = new FileStream(Game1.MAPS_FOLDER_LOCATION + "/" + mapname + "/" +
                        mapname + "_preview.png", FileMode.Open);
            this.previewTexture = Texture2D.FromStream(Game1.GetInstance().GraphicsDevice, 
                   stream);
            stream.Close();
            stream.Dispose();


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Game1.MAPS_FOLDER_LOCATION + "/" + mapname + ".xml");

            foreach (XmlNode rootChild in xmldoc.ChildNodes[1].ChildNodes)
            {
                if (rootChild.Name == "Players")
                {
                    this.mapPlayerLocations.Clear();
                    foreach (XmlNode playerNode in rootChild.ChildNodes)
                    {
                        this.mapPlayerLocations.AddLast(
                            new Point(
                            Int32.Parse(playerNode.Attributes["x"].Value), Int32.Parse(playerNode.Attributes["y"].Value)
                            ));
                    }
                }
                else if (rootChild.Name == "Data")
                {
                    this.selectedMapWidth = Int32.Parse(rootChild.Attributes["width"].Value) *
                        Int32.Parse(rootChild.Attributes["tileWidth"].Value);
                    this.selectedMapHeight = Int32.Parse(rootChild.Attributes["height"].Value) *
                        Int32.Parse(rootChild.Attributes["tileHeight"].Value);
                }
            }
        }

        /// <summary>
        /// When the user has clicked on this radio button.
        /// </summary>
        /// <param name="source">The radio button that was clicked on (kinda obsolete, but w/e)</param>
        public void OnRadioButtonClick(XNARadioButton source)
        {
            ((MapSelectionPanel)this.parent).OnMapSelectionChanged(this);
        }
    }
}
