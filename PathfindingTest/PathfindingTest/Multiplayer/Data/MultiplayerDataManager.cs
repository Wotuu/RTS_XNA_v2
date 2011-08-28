using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLists.Lists;

namespace PathfindingTest.Multiplayer.Data
{
    public class MultiplayerDataManager
    {
        private CustomArrayList<MultiplayerData> data = new CustomArrayList<MultiplayerData>();
        private static MultiplayerDataManager instance { get; set; }

        public readonly object dataSyncLock = new object();

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
        public MultiplayerData GetDataByServerID(int serverID, Boolean showError)
        {
            lock (this.dataSyncLock)
            {
                for (int i = 0; i < this.data.Count(); i++)
                {
                    MultiplayerData mpData = this.data.ElementAt(i);
                    if (mpData.serverID == serverID) return mpData;
                }
                if( showError ) Console.Out.WriteLine("Cannot find multiplayer data by server ID " + serverID);
            }
            return null;
        }

        /// <summary>
        /// Gets data by local ID.
        /// </summary>
        /// <param name="localID">The local ID.</param>
        /// <returns></returns>
        public MultiplayerData GetDataByLocalID(int localID)
        {
            lock (this.dataSyncLock)
            {
                for( int i = 0; i < this.data.Count(); i++){
                    MultiplayerData mpData = this.data.ElementAt(i);
                    if (mpData.localID == localID) return mpData;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds data to the manager.
        /// </summary>
        /// <param name="data">The data to add.</param>
        public void AddData(MultiplayerData data)
        {
            lock (this.dataSyncLock)
            {
                this.data.AddLast(data);
            }
        }
    }
}
