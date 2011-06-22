using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.ChatServer.Channels
{
    public class ChannelManager
    {
        private static ChannelManager instance { get; set; }
        public LinkedList<Channel> channels = new LinkedList<Channel>();

        private ChannelManager() { }

        public static ChannelManager GetInstance()
        {
            if (instance == null) instance = new ChannelManager();
            return instance;
        }

        /// <summary>
        /// Gets the channel with the ID, or creates a channel with that ID!
        /// </summary>
        /// <returns>The channel</returns>
        public Channel GetChannelByID(int id)
        {
            foreach (Channel ch in channels)
            {
                if (ch.id == id) return ch;
            }
            return new Channel(id);
        }

        /// <summary>
        /// Requests a new channel ID.
        /// </summary>
        /// <returns>The new channel ID.</returns>
        public int RequestChannelID()
        {
            if (channels.Count == 0) return 1;
            else return channels.Last.Value.id + 1;
        }
    }
}
