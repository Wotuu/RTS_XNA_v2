using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels.PlayerLocation
{
    public class MapPlayerLocationGroup
    {
        public LinkedList<MapPlayerLocationButton> buttons = new LinkedList<MapPlayerLocationButton>();
        public XNAPanel parent { get; set; }

        public Point offset { get; set; }
        public int mapWidth { get; set; }
        public int selectedMapHeight { get; set; }

        public MapPlayerLocationGroup(XNAPanel parent, Point offset, String mapname)
        {
            this.parent = parent;
            this.offset = offset;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Game1.MAPS_FOLDER_LOCATION + "/" + mapname + ".xml");

            foreach (XmlNode rootChild in xmldoc.ChildNodes[1].ChildNodes)
            {
                if (rootChild.Name == "Players")
                {
                    this.buttons.Clear();
                    foreach (XmlNode playerNode in rootChild.ChildNodes)
                    {
                        Point mapLocation = new Point(
                            Int32.Parse(playerNode.Attributes["x"].Value), Int32.Parse(playerNode.Attributes["y"].Value));
                        this.buttons.AddLast(new MapPlayerLocationButton(
                            this,
                            parent,
                            mapLocation,
                            this.MapToMiniMap(mapLocation)));
                    }
                    if (parent is MapSelectionPanel) this.OnMapSelectionChanged(false);
                    else if (parent is MapPreviewPanel) this.OnMapSelectionChanged(true);
                }
                else if (rootChild.Name == "Data")
                {
                    this.mapWidth = Int32.Parse(rootChild.Attributes["width"].Value) *
                        Int32.Parse(rootChild.Attributes["tileWidth"].Value);
                    this.selectedMapHeight = Int32.Parse(rootChild.Attributes["height"].Value) *
                        Int32.Parse(rootChild.Attributes["tileHeight"].Value);
                }
            }
        }

        /// <summary>
        /// Should be called when the map selection has changed
        /// </summary>
        /// <param name="flag">Whether the flag was visible or not.</param>
        public void OnMapSelectionChanged(Boolean flag)
        {
            if (flag == true)
            {
                if (parent is MapSelectionPanel)
                {
                    MapSelectionPanel panel = ((MapSelectionPanel)parent);
                    foreach (MapEntryPanel entry in panel.panels)
                    {
                        entry.mapPlayerLocationGroup.OnMapSelectionChanged(false);
                    }
                }
            }
            foreach (MapPlayerLocationButton button in buttons)
            {
                button.visible = flag;
            }
        }

        /// <summary>
        /// Converts map coordinates to mini map coordinates
        /// </summary>
        /// <param name="mapCoordinates">The map coordinates you want to convert</param>
        /// <returns>The mini map coordinates</returns>
        public Point MapToMiniMap(Point mapCoordinates)
        {
            float unscaledFactorX = (this.offset.X / (float)this.mapWidth);
            float unscaledFactorY = (this.offset.Y / (float)this.selectedMapHeight);

            float scaledFactorX = 1.0f;
            float scaledFactorY = 1.0f;

            if (this.parent is MapSelectionPanel)
            {
                MapSelectionPanel parent = (MapSelectionPanel)this.parent;

                scaledFactorX = (parent.previewTextureSize.X / (float)this.offset.X);
                scaledFactorY = (parent.previewTextureSize.Y / (float)this.offset.Y);
            }
            else if (this.parent is MapPreviewPanel)
            {
                MapPreviewPanel parent = (MapPreviewPanel)this.parent;

                scaledFactorX = (parent.imageBounds.Width / (float)this.offset.X);
                scaledFactorY = (parent.imageBounds.Height / (float)this.offset.Y);
            }

            return new Point((int)(mapCoordinates.X * unscaledFactorX * scaledFactorX),
                (int)(mapCoordinates.Y * unscaledFactorY * scaledFactorY));
        }

        /// <summary>
        /// Changes the index of a user on the map.
        /// </summary>
        /// <param name="userID">User ID to change</param>
        /// <param name="buttonIndex">The button index.</param>
        public void OnPlayerIndexChangedMP(int userID, int buttonIndex)
        {

            // If we're in the multiplayer lobby, and thus having a multiplayer game
            if (ChatServerConnectionManager.GetInstance().user.id == userID)
            {
                Packet p = new Packet(Headers.MAP_POSITION_CHANGED);
                p.AddInt(ChatServerConnectionManager.GetInstance().user.id);
                p.AddInt(buttonIndex);
                ChatServerConnectionManager.GetInstance().SendPacket(p);
            }

            Console.Out.WriteLine("Changing mp player index " + userID + ", " + buttonIndex);
            GameLobby lobby = (GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
            this.OnPlayerIndexChanged(
                lobby.mapPreviewPanel.playerLocationGroup.buttons.ElementAt(buttonIndex),
                lobby.GetDisplayPanelIndexByUserID(userID) + 1);
        }

        /// <summary>
        /// When a user clicks on an empty MapPlayerLocationButton, a number needs to appear.
        /// </summary>
        /// <param name="index">Index to display on the map</param>
        public void OnPlayerIndexChanged(MapPlayerLocationButton clickedButton, int index)
        {
            int result = 0;
            if (Int32.TryParse(clickedButton.text, out result) && result == index)
            {
                clickedButton.text = "";
            }
            else
            {
                if (clickedButton.text == "")
                {
                    clickedButton.text = index + "";
                }
                else
                {
                    foreach (MapPlayerLocationButton button in this.buttons)
                    {
                        if (Int32.TryParse(button.text, out result) && result == index) button.text = "";
                    }
                }
            }
        }
    }
}
