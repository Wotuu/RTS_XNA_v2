using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using System.Threading;

namespace GameServer.GameServer
{
    public class GameServerManager
    {
        private static GameServerManager instance { get; set; }
        public SocketServer serverSocket { get; set; }
        public int port = 14050;
        public LinkedList<GameClientListener> clients { get; set; }

        private GameServerManager()
        {
            clients = new LinkedList<GameClientListener>();
        }

        public static GameServerManager GetInstance()
        {
            if (instance == null) instance = new GameServerManager();
            return instance;
        }

        public void Start()
        {
            Console.Out.WriteLine("Game server started on port " + this.port);
            SocketServer socket = new SocketServer(port, false, "rts_xna_gameserver");
            this.serverSocket = socket;
            this.serverSocket.onClientConnectedListeners += this.OnClientConnected;
            new Thread(socket.Enable).Start();

            ServerUI.GetInstance().GameServerStatusLbl.Text = "Server running @ " + this.port;
            ServerUI.GetInstance().StartGameServer.Enabled = false;
            ServerUI.GetInstance().StopGameServer.Enabled = true;
            ServerUI.GetInstance().ViewGameServerClientsBtn.Enabled = true;
        }

        public void Stop()
        {
            this.serverSocket.Disable();
            ServerUI.GetInstance().GameServerStatusLbl.Text = "Server offline";
            ServerUI.GetInstance().StartGameServer.Enabled = true;
            ServerUI.GetInstance().StopGameServer.Enabled = false;
            ServerUI.GetInstance().ViewGameServerClientsBtn.Enabled = false;
        }

        public void OnClientConnected(SocketClient client)
        {
            clients.AddLast(new GameClientListener(client));
            client.SocketName = "Client nr " + clients.Count;
        }
    }
}
