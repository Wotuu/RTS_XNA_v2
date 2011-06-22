using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using SocketLibrary.Users;
using PathfindingTest.UI.Menus;
using System.Net.Sockets;
using XNAInterfaceComponents.ParentComponents;
using System.Threading;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.State;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class GameServerConnectionManager
    {
        private static GameServerConnectionManager instance { get; set; }
        public SocketClient connection { get; set; }

        public String serverLocation = "localhost";
        private int serverPort = 14050;

        public User user { get; set; }
        public Boolean safeShutdown { get; set; }

        private UnitPacketProcessor unitPacketProcessor = new UnitPacketProcessor();
        private BuildingPacketProcessor buildingPacketProcessor = new BuildingPacketProcessor();
        private GeneralPacketProcessor generalPacketProcessor = new GeneralPacketProcessor();

        private GameServerConnectionManager()
        {
        }

        /// <summary>
        /// The server disconnected :(
        /// </summary>
        public void OnDisconnect()
        {
            StateManager.GetInstance().gameState = StateManager.State.GameShutdown;
            StateManager.GetInstance().gameState = StateManager.State.MainMenu;

            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
            if (!safeShutdown)
            {
                XNAMessageDialog.CreateDialog(
                    "The server disconnected.", XNAMessageDialog.DialogType.OK)
                    .button1.onClickListeners += ServerDisconnectOKButtonPressed;
            }
            safeShutdown = false;
        }

        public static GameServerConnectionManager GetInstance()
        {
            if (instance == null) instance = new GameServerConnectionManager();
            return instance;
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        public void ConnectToServer()
        {
            TcpClient sock = new TcpClient();
            try
            {
                sock.Connect(this.serverLocation, this.serverPort);
            }
            catch (SocketException e)
            {
                XNAMessageDialog.CreateDialog(
                    "The game server is offline.", XNAMessageDialog.DialogType.OK);
                return;
            }

            this.connection = new SocketClient(sock.Client, "rts_client");
            new Thread(this.connection.Enable).Start();

            // Give the above thread some time to start running! 
            // Else we'll get a nullpointer right here.
            Thread.Sleep(50);
            this.connection.packetProcessor.onProcessPacket += unitPacketProcessor.DataReceived;
            this.connection.packetProcessor.onProcessPacket += buildingPacketProcessor.DataReceived;
            this.connection.packetProcessor.onProcessPacket += generalPacketProcessor.DataReceived;
            this.connection.DisableLogging();
            this.connection.onDisconnectListeners += this.OnDisconnect;

            this.SendPacket(new Packet(Headers.HANDSHAKE_1));
        }


        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void DisconnectFromServer()
        {
            if (this.connection != null && !this.connection.Receiving) return;
            this.SendPacket(new Packet(Headers.CLIENT_DISCONNECT));
            this.connection.Disable();
            this.safeShutdown = true;
        }

        /// <summary>
        /// User accepted disconnect by the server.
        /// </summary>
        /// <param name="source"></param>
        public void ServerDisconnectOKButtonPressed(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
        }

        /// <summary>
        /// Attempts to send a packet to the server.
        /// </summary>
        /// <returns>If the packet was sent or not.</returns>
        /// <param name="packet">The packet to send</param>
        public Boolean SendPacket(Packet packet)
        {
            if (packet.GetFullData().Length == 0)
            {
                Console.Error.WriteLine("Cannot send a packet with length = 0");
                return false;
            }
            this.connection.SendPacket(packet);
            return true;
        }
    }
}
