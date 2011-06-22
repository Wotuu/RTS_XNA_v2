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

namespace GameServer.GameServer
{
    public class GameClientListener
    {
        public SocketClient client { get; set; }
        public long lastAliveTicks { get; set; }
        public Boolean safeShutDown { get; set; }
        public ServerUser user { get; set; }
        public int gameStartSeconds { get; set; }
        private readonly object gameObjectCountLock = new object();

        public GameClientListener(SocketClient client)
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
            Console.Out.WriteLine("Client destroyed! -> " + GameServerManager.GetInstance().clients.Count);
            if (!safeShutDown)
            {
                ChannelManager.GetInstance().GetChannelByID(user.channelID).UserLeft(user);
                GameServerManager.GetInstance().clients.Remove(this);
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
                case Headers.CLIENT_DISCONNECT:
                    {
                        this.OnDisconnect();
                        break;
                    }
                case Headers.CLIENT_USER_ID:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        this.user = (ServerUser)ServerUserManager.GetInstance().GetUserByID(userID);
                        this.user.gameListener = this;

                        break;
                    }
                case Headers.GAME_REQUEST_OBJECT_ID:
                    {
                        int localID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = 0;
                        lock (gameObjectCountLock)
                        {
                            ChannelManager.GetInstance().GetChannelByID(this.user.channelID).objectCount++;
                            serverID = ChannelManager.GetInstance().GetChannelByID(this.user.channelID).objectCount;
                        }
                        Packet packet = new Packet(Headers.GAME_OBJECT_ID);
                        packet.AddInt(localID);
                        packet.AddInt(serverID);
                        this.client.SendPacket(packet);
                        break;
                    }
                case UnitHeaders.GAME_UNIT_LOCATION:
                    {
                        // The server merely makes sure that the packet is forwarded to 
                        // all the players
                        ChannelManager.GetInstance().GetChannelByID(user.channelID).SendGamePacketToAll(p);

                        /*int serverID = PacketUtil.DecodePacketInt(p, 0);
                        int targetX = PacketUtil.DecodePacketInt(p, 4);
                        int targetY = PacketUtil.DecodePacketInt(p, 8);
                        int currentX = PacketUtil.DecodePacketInt(p, 12);
                        int currentY = PacketUtil.DecodePacketInt(p, 16);*/

                        break;
                    }
                case UnitHeaders.GAME_NEW_UNIT:
                    {
                        Channel c = ChannelManager.GetInstance().GetChannelByID(user.channelID);
                        for (int i = 0; i < c.GetUserCount(); i++)
                        {
                            ServerUser serverUser = c.GetUserAt(i);
                            if (serverUser != this.user)
                            {
                                // Notify everyone but the one who created the unit, that the unit has been created.
                                serverUser.gameListener.client.SendPacket(p);
                            }
                        }
                        break;
                    }
                case UnitHeaders.GAME_REQUEST_UNIT_DATA:
                    {
                        // Our user wants to know data about a unit
                        Channel c = ChannelManager.GetInstance().GetChannelByID(user.channelID);
                        for (int i = 0; i < c.GetUserCount(); i++)
                        {
                            ServerUser serverUser = c.GetUserAt(i);
                            if (serverUser != this.user)
                            {
                                // Notify everyone but the one who requested the unit, that someone wants data
                                serverUser.gameListener.client.SendPacket(p);
                            }
                        }
                        break;
                    }
                case UnitHeaders.GAME_SEND_UNIT_DATA:
                    {
                        // I own data that someone else wants
                        // Find the user to send data to
                        int targetUserID = PacketUtil.DecodePacketInt(p, 0);
                        ServerUser targetUser = ChannelManager.GetInstance().GetChannelByID(this.user.channelID).GetUserAt(targetUserID);
                        targetUser.gameListener.client.SendPacket(p);

                        break;
                    }
                case BuildingHeaders.GAME_NEW_BUILDING:
                    {
                        Channel c = ChannelManager.GetInstance().GetChannelByID(user.channelID);
                        for (int i = 0; i < c.GetUserCount(); i++)
                        {
                            ServerUser serverUser = c.GetUserAt(i);
                            if (serverUser != this.user)
                            {
                                // Notify everyone but the one who created the unit, that the unit has been created.
                                serverUser.gameListener.client.SendPacket(p);
                            }
                        }

                        break;
                    }
                case BuildingHeaders.GAME_BUILDING_LOCATION:
                    {
                        // Send the packet back to everyone.
                        ChannelManager.GetInstance().GetChannelByID(user.channelID).SendGamePacketToAll(p);
                        break;
                    }
                case UnitHeaders.GAME_UNIT_MELEE_DAMAGE:
                    {
                        Channel c = ChannelManager.GetInstance().GetChannelByID(user.channelID);
                        for (int i = 0; i < c.GetUserCount(); i++)
                        {
                            ServerUser serverUser = c.GetUserAt(i);
                            if (serverUser != this.user)
                            {
                                // Notify everyone but the one who created the unit, that the unit has been created.
                                serverUser.gameListener.client.SendPacket(p);
                            }
                        }

                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_SHOT:
                    {
                        Channel c = ChannelManager.GetInstance().GetChannelByID(user.channelID);
                        for (int i = 0; i < c.GetUserCount(); i++)
                        {
                            ServerUser serverUser = c.GetUserAt(i);
                            if (serverUser != this.user)
                            {
                                // Notify everyone but the one who created the unit, that the shot has been created.
                                serverUser.gameListener.client.SendPacket(p);
                            }
                        }

                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_DAMAGE:
                    {
                        Channel c = ChannelManager.GetInstance().GetChannelByID(user.channelID);
                        for (int i = 0; i < c.GetUserCount(); i++)
                        {
                            ServerUser serverUser = c.GetUserAt(i);
                            if (serverUser != this.user)
                            {
                                // Notify everyone but the one who created the unit, that the damage has been done
                                serverUser.gameListener.client.SendPacket(p);
                            }
                        }

                        break;
                    }
                default:
                    {
                        String currTime = System.DateTime.Now.ToLongTimeString() + "," + System.DateTime.Now.Millisecond + " ";
                        this.client.log.messageLog.AddLast(new Logger.LogMessage(currTime + "UNKNOWN Header switcher reached default. (" +
                        p.GetHeader() + ")", true));
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
    }
}
