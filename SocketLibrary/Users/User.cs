using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Users
{
    public class User
    {
        public int id { get; set; }
        public String username { get; set; }
        public int channelID { get; set; }

        public User(String username)
        {
            this.username = username;
        }

        public override string ToString()
        {
            return id + " -> " + username + " ch: " + channelID;
        }

        public static bool operator ==(User a, User b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.id == b.id || a.username == b.username;
        }

        public static bool operator !=(User a, User b)
        {
            return !(a == b);
        }
    }
}
