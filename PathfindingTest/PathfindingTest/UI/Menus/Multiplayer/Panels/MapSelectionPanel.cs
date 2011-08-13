﻿using System;
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
using Microsoft.Xna.Framework.Graphics;

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
        public Texture2D previewTexture { get; set; }

        public Vector2 previewTextureSize = new Vector2(200f, 200f);

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

            int panelHeight = (int)Math.Max(this.panels.Count * MapEntryPanel.ENTRY_HEIGHT + 70, 300);
            this.bounds = new Rectangle(((CLIENT_WINDOW_WIDTH / 2) - 250),
                ((CLIENT_WINDOW_HEIGHT / 2) - (panelHeight / 2)),
                500,
                panelHeight);
            this.DoLayout();
        }

        /// <summary>
        /// Override the XNADialog GetDepth() function to be a fraction lower.
        /// </summary>
        /// <returns>A high depth</returns>
        public override int GetDepth()
        {
            return 15;
        }

        /// <summary>
        /// Loads the mapnames from disk.
        /// </summary>
        /// <returns>The list of names.</returns>
        public LinkedList<String> LoadMapNames()
        {
            DirectoryInfo di = new DirectoryInfo(Game1.MAPS_FOLDER_LOCATION);
            LinkedList<String> names = new LinkedList<String>();
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension == ".xml") names.AddLast(fi.Name.Replace(".xml", ""));
            }
            return names;
        }

        /// <summary>
        /// Gets the currently selected map.
        /// </summary>
        /// <returns>The selected map, or null if none was selected.</returns>
        public String GetSelectedMap()
        {
            foreach (MapEntryPanel panel in this.panels)
            {
                if (panel.previewButton.selected)
                {
                    return panel.previewButton.text;
                }
            }
            return null;
        }

        /// <summary>
        /// Called when the OK button was pressed.
        /// </summary>
        /// <param name="source">The XNA Button</param>
        public virtual void OnOKClick(XNAButton source)
        {
            if (!ChatServerConnectionManager.GetInstance().connection.Receiving) return;
            String map = this.GetSelectedMap();
            if (map != null)
            {
                this.previewPanel.selectedMapLbl.text = map;

                Packet p = new Packet(Headers.GAME_MAP_CHANGED);
                p.AddInt(ChatServerConnectionManager.GetInstance().user.channelID);
                p.AddString(this.previewPanel.selectedMapLbl.text);
                ChatServerConnectionManager.GetInstance().SendPacket(p);
            }
            this.Dispose();
        }

        public override void DoLayout()
        {
            this.okBtn = new XNAButton(this, new Rectangle(
                (this.bounds.Width / 2) - (this.buttonWidth) - (this.buttonSpacing / 2),
                this.bounds.Height - 50,
                this.buttonWidth, 40), "OK");
            // this.okBtn.onClickListeners += this.Dispose;
            this.okBtn.onClickListeners += this.OnOKClick;

            this.cancelBtn = new XNAButton(this, new Rectangle(
                (this.bounds.Width / 2) + (this.buttonSpacing / 2),
                this.bounds.Height - 50,
                this.buttonWidth, 40), "Cancel");
            this.cancelBtn.onClickListeners += this.Dispose;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if( previewTexture == null ) return;
            sb.Draw(this.previewTexture, 
                new Rectangle( this.bounds.X + MapEntryPanel.ENTRY_WIDTH + 20, this.bounds.Y + 10, 
                    (int)previewTextureSize.X, (int)previewTextureSize.Y), 
                null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, this.z - 0.001f);
        }

        /// <summary>
        /// Called when the map selection has changed.
        /// </summary>
        public void OnMapSelectionChanged(MapEntryPanel newSelection)
        {
            this.previewTexture = newSelection.previewTexture;
        }

        public override void Unload()
        {
            base.Unload();
            this.okBtn.onClickListeners -= this.OnOKClick;
        }
    }
}