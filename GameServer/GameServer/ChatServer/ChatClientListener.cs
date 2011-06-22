using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary;
using SocketLibrary.Protocol;
using SocketLibrary.Packets;
using GameServer.Users;
using GameServer.ChatServer.Channels;
using System.Threading;
using GameServer.ChatServer.Games;
using SocketLibrary.Multiplayer;
using System.Windows.Forms;
using System.Timers;

namespace GameServer.ChatServer
{
    public class ChatClientListener
    {
        public SocketClient client { get; set; }
        public long lastAliveTicks { get; set; }
        public Boolean safeShutDown { get; set; }
        public ServerUser user { get; set; }
        public int gameStartSeconds { get; set; }

        public ChatClientListener(SocketClient client)
        {
            this.client = client;
            // Wait for it ...
            while (client.packetProcessor == null) { Console.Out.WriteLine("Waiting for instance.."); Thread.Sleep(1); }
            client.packetProcessor.onProcessPacket += this.OnPacketReceived;
            this.client.onDisconnectListeners += this.OnDisconnect;
            this.client.onPacketSendListeners += this.OnPacketSend;
        }

        /// <summary>
        /// When the user disconnected
        /// </summary>
        public void OnDisconnect()
        {
            Console.Out.WriteLine("Client destroyed! -> " + ChatServerManager.GetInstance().clients.Count);
            if (!safeShutDown)
            {
                try
                {
                    ChannelManager.GetInstance().GetChannelByID(user.channelID).UserLeft(user);
                    ChatServerManager.GetInstance().clients.Remove(this);
                    MultiplayerGame game = MultiplayerGameManager.GetInstance().GetGameByHost(this.user);
                    if (game != null)
                    {
                        // Tell everyone in the lobby that the game was destroyed.
                        ChannelManager.GetInstance().GetChannelByID(1).DestroyGame(game);
                        // Tell everyone in the game itsself that the game was destroyed.
                        ChannelManager.GetInstance().GetChannelByID(game.id).DestroyGame(game);
                        // Remove it completely
                        MultiplayerGameManager.GetInstance().games.Remove(game);
                    }
                    this.client.Disable();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Something went wrong in the OnDisconnect() function .. oh well!");
                }
            }
        }

        delegate void TestDelegate();
        /// <summary>
        /// When a packet is sent
        /// </summary>
        /// <param name="p"></param>
        public void OnPacketSend(Packet p)
        {
            TestDelegate bla = delegate()
                 {
                     if (this.user == null) return;
                     if (ServerUI.GetInstance().lastSelectedClientName == this.user.username)
                         ServerUI.GetInstance().RefillMessageLogs(this.client.log.messageLog);
                 };
            ((Control)ServerUI.GetInstance()).BeginInvoke(bla);
        }

        /// <summary>
        /// Called when the server recieved data from the client.
        /// </summary>
        /// <param name="data">The data that was sent</param>
        public void OnPacketReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case Headers.KEEP_ALIVE:
                    {
                        lastAliveTicks = System.DateTime.UtcNow.Ticks;
                        break;
                    }
                case Headers.HANDSHAKE_1:
                    {
                        client.SendPacket(new Packet(Headers.HANDSHAKE_2));
                        break;
                    }
                case Headers.HANDSHAKE_3:
                    {
                        // Nothing
                        break;
                    }
                case Headers.CLIENT_DISCONNECT:
                    {
                        this.OnDisconnect();
                        break;
                    }
                case Headers.CLIENT_USERNAME:
                    {
                        this.user = new ServerUser(PacketUtil.DecodePacketString(p, 0), this);

                        Packet newPacket = new Packet(Headers.CLIENT_USER_ID);
                        newPacket.AddInt(this.user.id);
                        this.client.SendPacket(newPacket);

                        // Put the user in channel 1 (the main lobby)
                        ChannelManager.GetInstance().GetChannelByID(1).AddUser(user);

                        break;
                    }
                case Headers.CLIENT_CHANNEL:
                    {
                        Console.Error.WriteLine(this.user + " tried to change his channel! That's not allowed.");
                        //int newChannel = PacketUtil.DecodePacketInt(p, 0);
                        //this.user.ChangeChannel(newChannel);
                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        // Get the channel
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        // Get the message
                        String message = PacketUtil.DecodePacketString(p, 4);
                        // Send it to the users!
                        ChannelManager.GetInstance().GetChannelByID(channel).SendMessageToUsers(message);
                        break;
                    }
                case Headers.CLIENT_CREATE_GAME:
                    {
                        if (this.user.channelID != 1)
                        {
                            Console.Error.WriteLine("Received a request to create a channel that is NOT from a user in the lobby!");
                            return;
                        }
                        // User that requested this
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        // Name of the game
                        String gameName = PacketUtil.DecodePacketString(p, 4);
                        // Create game
                        MultiplayerGame mg = new MultiplayerGame(
                            MultiplayerGameManager.GetInstance().RequestGameID(),
                            gameName, "<No map selected yet>");
                        mg.host = (ServerUser)ServerUserManager.GetInstance().GetUserByID(userID);
                        // Add game to list
                        MultiplayerGameManager.GetInstance().games.AddLast(mg);
                        Console.Out.WriteLine("Created a game with ID = " + mg.id);

                        // Return the host its channel and game ID
                        Packet gameIDPacket = new Packet(Headers.GAME_ID);
                        gameIDPacket.AddInt(mg.id);
                        this.client.SendPacket(gameIDPacket);

                        this.user.ChangeChannel(mg.id);

                        // Notify all others in the channel that the game was created.
                        ChannelManager.GetInstance().GetChannelByID(1).CreatedGame(mg);
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {

                        break;
                    }
                case Headers.CLIENT_DESTROY_GAME:
                    {
                        MultiplayerGame game = MultiplayerGameManager.GetInstance().GetGameByHost(this.user);
                        if (game == null)
                        {
                            Console.Error.WriteLine("Client " + user.id + " tried to destroy a game that is not his!");
                        }
                        else
                        {
                            // Tell everyone in the lobby that the game was destroyed.
                            ChannelManager.GetInstance().GetChannelByID(1).DestroyGame(game);
                            // Tell everyone in the game itsself that the game was destroyed.
                            ChannelManager.GetInstance().GetChannelByID(game.id).DestroyGame(game);
                            // Remove it completely
                            MultiplayerGameManager.GetInstance().games.Remove(game);
                        }
                        break;
                    }
                case Headers.CLIENT_REQUEST_JOIN:
                    {
                        int gameID = PacketUtil.DecodePacketInt(p, 0);
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        MultiplayerGame game = MultiplayerGameManager.GetInstance().GetGameByID(gameID);
                        Packet joinPacket = new Packet(Headers.SERVER_REQUEST_JOIN);
                        joinPacket.AddInt(gameID);
                        joinPacket.AddInt(userID);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(game.host.id)).chatListener.client.SendPacket(
                           joinPacket);
                        break;
                    }
                case Headers.CLIENT_OK_JOIN:
                    {
                        // Notify the client of the response by host.
                        int gameID = PacketUtil.DecodePacketInt(p, 0);
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        MultiplayerGame game = MultiplayerGameManager.GetInstance().GetGameByID(gameID);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).chatListener.client.SendPacket(p);
                        // Change this user's channel to the game channel.
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).chatListener.user.ChangeChannel(gameID);
                        break;
                    }
                case Headers.CLIENT_GAME_FULL:
                    {
                        int gameID = PacketUtil.DecodePacketInt(p, 0);
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        MultiplayerGame game = MultiplayerGameManager.GetInstance().GetGameByID(gameID);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).chatListener.client.SendPacket(p);
                        break;
                    }
                case Headers.CLIENT_LEFT_GAME:
                    {
                        // Put user back in lobby.
                        this.user.ChangeChannel(1);
                        break;
                    }
                case Headers.GAME_COLOR_CHANGED:
                    {
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        int color = PacketUtil.DecodePacketInt(p, 8);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).color = color;
                        Channel c = ChannelManager.GetInstance().GetChannelByID(channel);
                        c.ChangeColor(userID, color);
                        break;
                    }
                case Headers.GAME_TEAM_CHANGED:
                    {
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        int team = PacketUtil.DecodePacketInt(p, 8);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).team = team;
                        Channel c = ChannelManager.GetInstance().GetChannelByID(channel);
                        c.ChangeTeam(userID, team);
                        break;
                    }
                case Headers.GAME_READY_CHANGED:
                    {
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        int readyState = PacketUtil.DecodePacketInt(p, 8);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).readyState = readyState;
                        Channel c = ChannelManager.GetInstance().GetChannelByID(channel);
                        c.ChangeReadyState(userID, readyState);
                        break;
                    }
                case Headers.GAME_KICK_CLIENT:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        Packet kickPacket = new Packet(Headers.GAME_KICK_CLIENT);
                        kickPacket.AddInt(userID);
                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).
                            chatListener.client.SendPacket(kickPacket);

                        ((ServerUser)ServerUserManager.GetInstance().GetUserByID(userID)).ChangeChannel(1);
                        break;
                    }
                case Headers.CLIENT_GAME_START:
                    {
                        System.Timers.Timer gameStartTimer = new System.Timers.Timer();
                        gameStartTimer.Elapsed += new ElapsedEventHandler(GameStart);
                        gameStartTimer.Interval = 1000;
                        gameStartTimer.Start();
                        this.gameStartSeconds = 5;
                        break;
                    }
                default:
                    {
                        String currTime = System.DateTime.Now.ToLongTimeString() + "," + System.DateTime.Now.Millisecond + " ";
                        this.client.log.messageLog.AddLast(new Logger.LogMessage(currTime + "UNKNOWN Header switcher reached default. " + 
                            "(" + p.GetHeader() + ")", true));
                        break;
                    }
            }
            TestDelegate bla = delegate()
            {
                if (this.user == null) return;
                if (ServerUI.GetInstance().lastSelectedClientName == this.user.username)
                    ServerUI.GetInstance().RefillMessageLogs(this.client.log.messageLog);
            };
            ((Control)ServerUI.GetInstance()).BeginInvoke(bla);
        }

        /// <summary>
        /// Event that is called when the game is about to start.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void GameStart(object source, ElapsedEventArgs e)
        {
            ChannelManager.GetInstance().GetChannelByID(this.user.channelID).StartGame(this.gameStartSeconds);
            this.gameStartSeconds--;

            if (gameStartSeconds == -1)
            {
                ((System.Timers.Timer)source).Stop();
                ((System.Timers.Timer)source).Interval = Double.MaxValue;
                this.gameStartSeconds = 5;
            }
        }
    }
}
