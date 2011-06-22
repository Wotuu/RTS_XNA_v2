using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.ChatServer;
using GameServer.ChatServer.Channels;
using SocketLibrary.Users;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using GameServer.GameServer;

namespace GameServer.Users
{
    public class ServerUser : User
    {
        public ChatClientListener chatListener { get; set; }
        public GameClientListener gameListener { get; set; }
        public int color { get; set; }
        public int team { get; set; }
        public int readyState { get; set; }

        public ServerUser(String username, ChatClientListener listener ) : base(username)
        {
            // Init on -1, because 0 is a color! -1 will make sure nothing is selected.
            this.color = -1;
            this.id = ServerUserManager.GetInstance().RequestUserID();
            this.chatListener = listener;
            this.chatListener.client.onDisconnectListeners += this.OnDisconnect;

            ServerUserManager.GetInstance().users.AddLast(this);
        }

        
        /// <summary>
        /// When the user disconnects from the server.
        /// </summary>
        public void OnDisconnect()
        {
            ChannelManager.GetInstance().GetChannelByID(this.channelID).UserLeft(this);
            ServerUserManager.GetInstance().users.Remove(this);
        }

        /// <summary>
        /// Changes the channel of this user.
        /// </summary>
        /// <param name="newChannel"></param>
        public void ChangeChannel(int newChannel)
        {
            Console.Out.WriteLine(this + " leaving channel " + this.channelID);
            ChannelManager.GetInstance().GetChannelByID(this.channelID).UserLeft(this);
            Console.Out.WriteLine(this + " joining channel " + newChannel);
            ChannelManager.GetInstance().GetChannelByID(newChannel).AddUser(this);
        }
    }
}
