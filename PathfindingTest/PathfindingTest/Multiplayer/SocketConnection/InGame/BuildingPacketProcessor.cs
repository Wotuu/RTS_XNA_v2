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

                        if (MultiplayerDataManager.GetInstance().GetDataByServerID(serverID, false) == null)
                        {
                            ObjectCreator.GetInstance().CreateBuilding(playerID, serverID, buildingType, by);
                        }

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
                            data = MultiplayerDataManager.GetInstance().GetDataByServerID(serverID, true);
                            count++;
                            if (count > 5)
                            {
                                Console.Out.WriteLine("Unable to fetch data, requesting..");
                                Packet packet = new Packet(Headers.GAME_REQUEST_OBJECT_DATA);
                                packet.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                                packet.AddInt(serverID);
                                GameServerConnectionManager.GetInstance().SendPacket(packet);

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
    }
}
