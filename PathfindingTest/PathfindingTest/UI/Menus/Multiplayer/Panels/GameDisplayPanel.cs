using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using SocketLibrary.Multiplayer;
using XNAInterfaceComponents.ChildComponents;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class GameDisplayPanel : XNAPanel
    {
        public static int componentHeight = 40;
        public static int componentSpacing = 10;

        public int index { get; set; }
        public MultiplayerGame multiplayerGame { get; set; }
        private XNALabel mapNameLbl { get; set; }
        private XNAButton joinGameBtn { get; set; }

        public GameDisplayPanel(ParentComponent parent, int index, MultiplayerGame multiplayerGame) :
            base(parent, new Rectangle())
        {
            this.index = index;
            this.bounds = new Rectangle(5, 5 + (componentHeight + componentSpacing) * this.index,
                580,
                componentHeight);
            this.border = new Border(this, 2, Color.Pink);

            this.multiplayerGame = multiplayerGame;


            XNALabel gameNameLbl = new XNALabel(this, new Rectangle(5, 5, 180, 30), this.multiplayerGame.gamename);
            mapNameLbl = new XNALabel(this, new Rectangle(195, 5, 180, 30), this.multiplayerGame.mapname);

            joinGameBtn = new XNAButton(this, new Rectangle(475, 5, 100, 30), "Join game");
            joinGameBtn.onClickListeners += this.JoinGame;
        }

        /// <summary>
        /// Change the map name.
        /// </summary>
        /// <param name="newMapname">The new mapname.</param>
        public void ChangeMapname(String newMapname)
        {
            this.mapNameLbl.text = newMapname;
        }

        /// <summary>
        /// User wants to join this game.
        /// </summary>
        /// <param name="button">The button that was pressed.</param>
        public void JoinGame(XNAButton button)
        {
            Packet joinPacket = new Packet(Headers.CLIENT_REQUEST_JOIN);
            joinPacket.AddInt(this.multiplayerGame.id);
            joinPacket.AddInt(ChatServerConnectionManager.GetInstance().user.id);
            ChatServerConnectionManager.GetInstance().SendPacket(joinPacket);
        }
    }
}
