using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.UI.Menus.Multiplayer.Misc
{
    public class Message
    {
        private String timestamp { get; set; }
        private String message { get; set; }

        public Message(String message)
        {
            this.message = message;
            this.timestamp = DateTime.UtcNow.ToLongTimeString();
        }

        /// <summary>
        /// Gets the message with a timestamp.
        /// </summary>
        /// <returns>The string to display</returns>
        public String GetComposedMessage()
        {
            return "[" + timestamp + "] " + this.message;
        }
    }
}
