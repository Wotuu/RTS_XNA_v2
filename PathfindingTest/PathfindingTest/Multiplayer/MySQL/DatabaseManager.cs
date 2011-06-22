using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Net.Sockets;

namespace PathfindingTest.Multiplayer.MySQL
{
    public class DatabaseManager
    {
        private static DatabaseManager instance { get; set; }
        private String serverUrl = "linux75.webawere.nl";
        private String databaseName = "koppen21_xnarts";
        private String username = "koppen21_johndoe";
        private String password = "6500089a";

        private MySqlConnection connection { get; set; }
        private MySqlDataAdapter data { get; set; }

        private DatabaseManager()
        {

        }

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        public void Connect()
        {
            connection = new MySqlConnection();
            data = new MySqlDataAdapter();
            connection.ConnectionString =
            "server=" + serverUrl + ";"
            + "database=" + databaseName + ";"
            + "uid=" + username + ";"
            + "password=" + password + ";";
            connection.Open();
        }

        /// <summary>
        /// Disconnects the connection to the database
        /// </summary>
        public void Disconnect()
        {
            if (connection != null) connection.Close();
        }

        public Boolean CreateGame(MultiplayerGame game)
        {
            this.Connect();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `games` (username, gamename, mapname, ip) " +
            "VALUES('" + game.username + "', '" + game.gamename + "', '" + game.mapname + "', '" + game.hostIP + "');";
            Socket.
            command.ExecuteNonQuery();
            this.Disconnect();
        }

        public LinkedList<MultiplayerGame> GetMultiplayerGames()
        {
            LinkedList<MultiplayerGame> result = new LinkedList<MultiplayerGame>();
            MySqlDataReader Reader;
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM `games`";
            connection.Open();
            Reader = command.ExecuteReader();


            bool tagExists = false;
            while (Reader.Read())
            {
                for (int i = 0; i < Reader.FieldCount; i++)
                {
                    if (Reader.GetName(i).Equals("TagID"))
                    {
                        String currentTagID = Reader.GetValue(i) + "";
                        Console.Out.WriteLine("Comparing " + currentTagID + " to " + tagID);
                        if (currentTagID.Equals(tagID))
                        {
                            tagExists = true;
                            Console.Out.WriteLine("Tag already exists!");
                            break;
                        }
                    }
                }
                if (tagExists)
                {
                    break;
                }
            }
            connection.Close();
            Reader.Close();

            return result;
        }

        public static DatabaseManager GetInstance()
        {
            if (instance == null) instance = new DatabaseManager();
            return instance;
        }



    }
}
