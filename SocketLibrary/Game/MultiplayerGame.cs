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

        /// <summary>
        /// Gets a user at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        public User GetUser(int index)
        {
            return this.users.ElementAt(index);
        }

        /// <summary>
        /// Gets the amount of users in this multiplayer game.
        /// </summary>
        /// <returns>The count.</returns>
        public int GetUserCount()
        {
            return this.users.Count;
        }

        /// <summary>
        /// Gets a user by its ID.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        /// <returns>The User, or null</returns>
        public User GetUserByID(int userID)
        {
            foreach (User user in this.users)
            {
                if (user.id == userID)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
