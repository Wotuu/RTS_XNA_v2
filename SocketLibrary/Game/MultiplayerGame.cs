using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Users;

namespace SocketLibrary.Multiplayer
{
    public class MultiplayerGame
    {
        public int id { get; set; }
        public String gamename { get; set; }
        public String mapname { get; set; }
        public User host { get; set; }
        private LinkedList<User> users = new LinkedList<User>();

        public MultiplayerGame(int gameID, String gamename, String mapname)
        {
            this.id = gameID;
            this.gamename = gamename;
            this.mapname = mapname;
        }

        /// <summary>
        /// Adds a user to this game.
        /// </summary>
        /// <param name="user">The user to add</param>
        public void AddUser(User user)
        {
            this.users.AddLast(user);
        }

        /// <summary>
        /// Removes a user from the list.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        public void RemoveUser(User user)
        {
            this.users.Remove(user);
        }
    }
}
