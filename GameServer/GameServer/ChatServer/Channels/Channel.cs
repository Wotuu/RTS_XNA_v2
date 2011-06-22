using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Users;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using SocketLibrary.Multiplayer;
using GameServer.ChatServer.Games;

namespace GameServer.ChatServer.Channels
{
    public class Channel
    {
        public int id { get; set; }
        private LinkedList<ServerUser> users = new LinkedList<ServerUser>();
        public int objectCount { get; set; }

        public Channel()
        {
            this.id = ChannelManager.GetInstance().RequestChannelID();
            ChannelManager.GetInstance().channels.AddLast(this);
        }

        public Channel(int id)
        {
            this.id = id;
            Console.Out.WriteLine("Creating channel with id " + id);
            ChannelManager.GetInstance().channels.AddLast(this);
        }

        /// <summary>
        /// Whether a user exists or not.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True or false .. doh</returns>
        public Boolean UserExists(ServerUser toCheck)
        {
            for (int i = 0; i < this.users.Count; i++)
            {
                ServerUser user = this.users.ElementAt(i);
                if (user.id == toCheck.id ||
                    user.username == toCheck.username) return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a user to the list, and notifies other users of this person's arrival.
        /// </summary>
        /// <param name="toAdd">The user to add to this channel</param>
        public void AddUser(ServerUser toAdd)
        {
            Console.Out.Write("User " + toAdd + " joins channel " + this.id);
            if (this.UserExists(toAdd))
            {
                // toAdd.listener.client.messageLog.AddLast(new LogMes"TRIED TO ADD YOU TO A CHANNEL YOU'RE ALREADY IN");
                return;
            }
            toAdd.channelID = this.id;

            // If this is the lobby, send the games that are already created
            if (this.id == 1)
            {
                for (int i = 0; i < MultiplayerGameManager.GetInstance().games.Count; i++)
                {
                    MultiplayerGame game = MultiplayerGameManager.GetInstance().games.ElementAt(i);
                    Packet p = new Packet(Headers.SERVER_CREATE_GAME);
                    p.AddInt(game.id);
                    p.AddString(game.gamename);
                    toAdd.chatListener.client.SendPacket(p);

                    p = new Packet(Headers.GAME_MAP_CHANGED);
                    p.AddInt(game.id);
                    p.AddString(game.mapname);
                }
            }
            // This is not the lobby, send the user all data
            else
            {
                for (int i = 0; i < this.users.Count; i++)
                {
                    ServerUser user = this.users.ElementAt(i);
                    Packet p = new Packet(Headers.GAME_COLOR_CHANGED);
                    p.AddInt(this.id);
                    p.AddInt(user.id);
                    p.AddInt(user.color);
                    toAdd.chatListener.client.SendPacket(p);

                    p = new Packet(Headers.GAME_TEAM_CHANGED);
                    p.AddInt(this.id);
                    p.AddInt(user.id);
                    p.AddInt(user.team);
                    toAdd.chatListener.client.SendPacket(p);
                }
            }

            // User joins the channel
            users.AddLast(toAdd);
            Packet newChannelPacket = new Packet(Headers.CLIENT_CHANNEL);
            newChannelPacket.AddInt(this.id);
            toAdd.chatListener.client.SendPacket(newChannelPacket);


            // The user that joined must know who is already in the channel
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                if (user.id == toAdd.id) continue;
                Packet p = new Packet(Headers.NEW_USER);
                p.AddInt(user.id);
                p.AddString(user.username);
                toAdd.chatListener.client.SendPacket(p);
            }

            // Notify everyone that this user has joined (including our new user)
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.NEW_USER);
                p.AddInt(toAdd.id);
                p.AddString(toAdd.username);
                user.chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// A user has left this channel.
        /// </summary>
        /// <param name="toRemove">The user to remove from this channel</param>
        public void UserLeft(ServerUser toRemove)
        {
            Console.Out.Write("Removed user " + toRemove + " from channel " + this.id);
            users.Remove(toRemove);
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.USER_LEFT);
                p.AddInt(toRemove.id);
                p.AddString(toRemove.username);
                user.chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Sends a message to all users in this channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessageToUsers(String message)
        {
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.CHAT_MESSAGE);
                p.AddInt(id);
                p.AddString(message);
                user.chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Notify all users in this channel of a game that has been created.
        /// </summary>
        /// <param name="game"></param>
        public void CreatedGame(MultiplayerGame game)
        {
            if (this.id != 1)
            {
                Console.Error.WriteLine("Attempted to create a game in channel " + this.id + ". This is not allowed. Fix this message.");
                return;
            }
            // Host should no longer be in the lobby
            this.UserLeft((ServerUser)game.host);
            for (int i = 0; i < users.Count; i++)
            {
                ServerUser user = users.ElementAt(i);
                Packet p = new Packet(Headers.SERVER_CREATE_GAME);
                p.AddInt(game.id);
                p.AddString(game.gamename);
                user.chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Notifies all users in this channel that the game has been destroyed. Sorry :c
        /// All users in the lobby will also be notified.
        /// </summary>
        /// <param name="game">The game to destroy.</param>
        public void DestroyGame(MultiplayerGame game)
        {
            foreach (ServerUser user in this.users)
            {
                Packet p = new Packet(Headers.SERVER_DESTROY_GAME);
                p.AddInt(game.id);
                user.chatListener.client.SendPacket(p);
                // user.listener.user.ChangeChannel(1);
            }
        }

        /// <summary>
        /// Changes the color of a user, notifying everyone else in the room.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="colorIndex">The color index.</param>
        public void ChangeColor(int userID, int colorIndex)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).id == userID) continue;
                Packet p = new Packet(Headers.GAME_COLOR_CHANGED);
                p.AddInt(this.id);
                p.AddInt(userID);
                p.AddInt(colorIndex);
                users.ElementAt(i).chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Changes the color of a user, notifying everyone else in the room.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="team">The team.</param>
        public void ChangeTeam(int userID, int team)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).id == userID) continue;
                Packet p = new Packet(Headers.GAME_TEAM_CHANGED);
                p.AddInt(this.id);
                p.AddInt(userID);
                p.AddInt(team);
                users.ElementAt(i).chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Changes the state of a user, notifying everyone else in the room.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="state">The team.</param>
        public void ChangeReadyState(int userID, int state)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).id == userID) continue;
                Packet p = new Packet(Headers.GAME_READY_CHANGED);
                p.AddInt(this.id);
                p.AddInt(userID);
                p.AddInt(state);
                users.ElementAt(i).chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Notifies all clients that the game will start.
        /// </summary>
        /// <param name="seconds">The amount of seconds left till start.</param>
        public void StartGame(int seconds)
        {
            for (int i = 0; i < users.Count; i++)
            {
                Packet p = new Packet(Headers.SERVER_GAME_START);
                p.AddInt(seconds);
                users.ElementAt(i).chatListener.client.SendPacket(p);
            }
        }
        

        /// <summary>
        /// Sends a packet to all users in this channel.
        /// </summary>
        /// <param name="p">The packet to send.</param>
        public void SendChatPacketToAll(Packet p)
        {
            for (int i = 0; i < users.Count; i++)
            {
                users.ElementAt(i).chatListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Sends a packet to all users in this channel.
        /// </summary>
        /// <param name="p">The packet to send.</param>
        public void SendGamePacketToAll(Packet p)
        {
            for (int i = 0; i < users.Count; i++)
            {
                users.ElementAt(i).gameListener.client.SendPacket(p);
            }
        }

        /// <summary>
        /// Gets the list size.
        /// </summary>
        public int GetUserCount()
        {
            return this.users.Count;
        }

        /// <summary>
        /// Gets a user at a certain index in the list.
        /// </summary>
        /// <param name="index">The index.</param>
        public ServerUser GetUserAt(int index)
        {
            return this.users.ElementAt(index);
        }
    }
}
