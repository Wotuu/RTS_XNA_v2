using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Buildings;
using PathfindingTest.Multiplayer.Data;
using PathfindingTest.Players;
using PathfindingTest.Units;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class BuildingPacketProcessor
    {
        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case BuildingHeaders.GAME_NEW_BUILDING:
                    {
                        /*
                        /// <summary>
                        /// Types to be used to request object IDs
                        /// </summary>
                        public const int TYPE_BARRACKS = 0, TYPE_FACTORY = 1, TYPE_FORTRESS = 2, TYPE_RESOURCES_GATHER = 3;


                        /// <summary>
                        /// Client synchronizes a building
                        /// [Header] [Int32 serverID] [Int32 locationX] [Int32 locationY]
                        /// </summary>
                        public const byte GAME_BUILDING_LOCATION = 0x40;
                        */

                        int playerID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = PacketUtil.DecodePacketInt(p, 4);
                        int buildingType = PacketUtil.DecodePacketInt(p, 8);
                        int by = PacketUtil.DecodePacketInt(p, 12);

                        CreateBuilding(playerID, serverID, buildingType, by);

                        break;
                    }
                case BuildingHeaders.GAME_BUILDING_LOCATION:
                    {
                        int serverID = PacketUtil.DecodePacketInt(p, 0);
                        int locationX = PacketUtil.DecodePacketInt(p, 4);
                        int locationY = PacketUtil.DecodePacketInt(p, 8);


                        MultiplayerData data;
                        int count = 0;
                        do
                        {
                            data = MultiplayerDataManager.GetInstance().GetDataByServerID(serverID);
                            count++;
                            if (count > 15)
                            {
                                Console.Out.WriteLine("Unable to fetch data, requesting.. NYI :c");
                                
                                /*
                                Packet packet = new Packet(UnitHeaders.GAME_REQUEST_UNIT_DATA);
                                packet.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                                packet.AddInt(serverID);
                                GameServerConnectionManager.GetInstance().SendPacket(packet);
                                */

                                return;
                            }
                        }
                        while (data == null);

                        Building building = ((BuildingMultiplayerData)data).building;

                        building.x = locationX;
                        building.y = locationY;

                        try
                        {
                            if (building.state == Building.State.MultiplayerWaitingForLocation) building.PlaceBuilding(building.constructedBy);
                        }
                        catch (Exception e) { }

                        break;
                    }
            }
        }

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="playerID">The player ID.</param>
        /// <param name="serverID">Server ID.</param>
        /// <param name="type">The type of the unit.</param>
        public void CreateBuilding(int playerID, int serverID, int type, int byID)
        {
            Building building = null;
            Player p = Game1.GetInstance().GetPlayerByMultiplayerID(playerID);
            Engineer engineer = (Engineer)((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(byID)).unit;
                switch (type)
                {
                    case BuildingHeaders.TYPE_BARRACKS:
                        {
                            building = new Barracks(p, p.color);
                            break;
                        }
                    case BuildingHeaders.TYPE_FACTORY:
                        {
                            building = new Factory(p, p.color);
                            break;
                        }
                    case BuildingHeaders.TYPE_FORTRESS:
                        {
                            building = new Fortress(p, p.color);
                            break;
                        }
                    case BuildingHeaders.TYPE_RESOURCES_GATHER:
                        {
                            building = new ResourceGather(p, p.color);
                            break;
                        }
                }
            building.constructedBy = engineer;
            building.multiplayerData.serverID = serverID;
        }
    }
}
