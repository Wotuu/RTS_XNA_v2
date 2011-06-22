using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Users
{
    public class UserManager
    {
        public LinkedList<User> users = new LinkedList<User>();

        protected static UserManager instance { get; set; }

        protected UserManager() { }

        public static UserManager GetInstance()
        {
            if (instance == null) instance = new UserManager();
            return instance;
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to search</param>
        /// <returns>Returns the user, or null if no user was found.</returns>
        public User GetUserByID(int id)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).id == id) return users.ElementAt(i);
            }
            return null;
        }

        /// <summary>
        /// Removes a user by ID.
        /// </summary>
        /// <param name="userID"></param>
        public void RemoveUserByID(int userID)
        {
            for (int i = 0; i < users.Count; i++)
            {
                User user = users.ElementAt(i);
                if (user.id == userID) users.Remove(user);
            }
        }
    }
}
