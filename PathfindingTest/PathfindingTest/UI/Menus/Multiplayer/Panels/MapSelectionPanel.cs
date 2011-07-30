using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.ParentComponents;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using System.IO;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class MapSelectionPanel : XNADialog
    {
        public MapPreviewPanel previewPanel { get; set; }
        public XNAButton okBtn { get; set; }
        public XNAButton cancelBtn { get; set; }
        private int buttonWidth { get; set; }
        private int buttonSpacing { get; set; }

        public LinkedList<MapEntryPanel> panels = new LinkedList<MapEntryPanel>();
        public XNARadioButtonGroup group { get; set; }

        public MapSelectionPanel(MapPreviewPanel previewPanel, String selectedMapName)
            : base()
        {
            this.previewPanel = previewPanel;
            this.buttonWidth = 100;
            this.buttonSpacing = 30;

            this.group = new XNARadioButtonGroup();
            int index = 0;
            LinkedList<String> mapNames = LoadMapNames();
            foreach (String map in mapNames)
            {
                this.panels.AddLast(new MapEntryPanel(this, map, index));
                if (selectedMapName == map) this.panels.Last.Value.previewButton.selected = true; 
                index++;
            }

            this.bounds = new Rectangle( ((CLIENT_WINDOW_WIDTH / 2) - 250), 
                ((CLIENT_WINDOW_HEIGHT / 2) - ( this.panels.Count * MapEntryPanel.ENTRY_HEIGHT + 70 / 2)), 
                500,
                (int)Math.Max( this.panels.Count * MapEntryPanel.ENTRY_HEIGHT + 70, 200));
            this.DoLayout();
        }

        /// <summary>
        /// Loads the mapnames from disk.
        /// </summary>
        /// <returns>The list of names.</returns>
        public LinkedList<String> LoadMapNames()
        {
            DirectoryInfo di = new DirectoryInfo("./Maps/");
            LinkedList<String> names = new LinkedList<String>();
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension == ".xml") names.AddLast(fi.Name.Replace(".xml", ""));
            }
            return names;
        }

        /// <summary>
        /// Called when the OK button was pressed.
        /// </summary>
        /// <param name="source">The XNA Button</param>
        public void OnOKClick(XNAButton source)
        {
            foreach( MapEntryPanel panel in this.panels ){
                if( panel.previewButton.selected ){
                    this.previewPanel.selectedMapLbl.text = panel.previewButton.text;
                    if (Game1.GetInstance().IsMultiplayerGame())
                    {
                        Packet p = new Packet(Headers.GAME_MAP_CHANGED);
                        p.AddInt(ChatServerConnectionManager.GetInstance().user.channelID);
                        p.AddString(this.previewPanel.selectedMapLbl.text);
                        ChatServerConnectionManager.GetInstance().SendPacket(p);
                    }

                    break;
                }
            }
        }

        public override void DoLayout()
        {
            this.okBtn = new XNAButton(this, new Rectangle(
                (this.bounds.Width / 2) - (this.buttonWidth) - (this.buttonSpacing / 2),
                this.panels.Count * MapEntryPanel.ENTRY_HEIGHT + 20,
                this.buttonWidth, 40), "OK");
            this.okBtn.onClickListeners += this.Dispose;
            this.okBtn.onClickListeners += this.OnOKClick;

            this.cancelBtn = new XNAButton(this, new Rectangle(
                (this.bounds.Width / 2) + (this.buttonSpacing / 2),
                this.panels.Count * MapEntryPanel.ENTRY_HEIGHT + 20,
                this.buttonWidth, 40), "Cancel");
            this.cancelBtn.onClickListeners += this.Dispose;
        }
    }
}
