using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Multiplayer;
using GameServer.ChatServer.Channels;
using SocketLibrary.Users;

namespace GameServer.ChatServer.Games
{
    public class MultiplayerGameManager
    {
        private static MultiplayerGameManager instance;
        public LinkedList<MultiplayerGame> games = new LinkedList<MultiplayerGame>();

        private MultiplayerGameManager() { }

        public static MultiplayerGameManager GetInstance()
        {
            if (instance == null) instance = new MultiplayerGameManager();
            return instance;
        }

        /// <summary>
        /// Requests a new game ID.
        /// </summary>
        /// <returns>The new game ID that you should use.</returns>
        public int RequestGameID()
        {
            return ChannelManager.GetInstance().RequestChannelID();
        }

        /// <summary>
        /// Gets a game by game id.
        /// </summary>
        /// <param name="gameID">The game ID</param>
        /// <returns>The game.</returns>
        public MultiplayerGame GetGameByID(int gameID)
        {
            foreach (MultiplayerGame game in this.games)
            {
                if (game.id == gameID) return game;
            }
            return null;
        }

        /// <summary>
        /// Gets the game by a user 
        /// </summary>
        /// <param name="host">The host that has a game</param>
        /// <returns>Null if that user does not have a game, the MultiplayerGame otherwise.</returns>
        public MultiplayerGame GetGameByHost(User host)
        {
            foreach (MultiplayerGame game in this.games)
            {
                if (game.host == host) return game;
            }
            return null;
        }
    }
}
