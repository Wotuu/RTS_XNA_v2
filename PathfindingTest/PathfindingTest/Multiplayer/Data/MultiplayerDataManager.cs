using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Multiplayer.Data
{
    public class MultiplayerDataManager
    {
        private LinkedList<MultiplayerData> data = new LinkedList<MultiplayerData>();
        private static MultiplayerDataManager instance { get; set; }

        public static MultiplayerDataManager GetInstance()
        {
            if (instance == null) instance = new MultiplayerDataManager();
            return instance;
        }

        private MultiplayerDataManager() { }

        /// <summary>
        /// Gets data by server ID.
        /// </summary>
        /// <param name="serverID">The server ID.</param>
        /// <returns></returns>
        public MultiplayerData GetDataByServerID(int serverID)
        {
            foreach (MultiplayerData data in this.data)
            {
                if (data.serverID == serverID) return data;
            }
            Console.Out.WriteLine("Cannot find multiplayer data by server ID " + serverID);
            return null;
        }

        /// <summary>
        /// Gets data by local ID.
        /// </summary>
        /// <param name="localID">The local ID.</param>
        /// <returns></returns>
        public MultiplayerData GetDataByLocalID(int localID)
        {
            foreach (MultiplayerData data in this.data)
            {
                if (data.localID == localID) return data;
            }
            return null;
        }

        /// <summary>
        /// Adds data to the manager.
        /// </summary>
        /// <param name="data">The data to add.</param>
        public void AddData(MultiplayerData data)
        {
            this.data.AddLast(data);
        }
    }
}
