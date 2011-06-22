using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Users;

namespace GameServer.Users
{
    public class ServerUserManager : UserManager
    {
        private ServerUserManager() {  }
        
        public static new ServerUserManager GetInstance()
        {
            if (instance == null) instance = new ServerUserManager();
            return (ServerUserManager) instance;
        }

        /// <summary>
        /// Requests a new user ID.
        /// </summary>
        /// <returns>The ID.</returns>
        public int RequestUserID()
        {
            if (users.Count == 0) return 1;
            else return users.Last.Value.id + 1;
        }
    }
}
