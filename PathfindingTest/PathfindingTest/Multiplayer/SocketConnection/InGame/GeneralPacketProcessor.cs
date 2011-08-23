using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer.Data;
using PathfindingTest.Units;
using Microsoft.Xna.Framework;
using System.Threading;
using PathfindingTest.Buildings;
using PathfindingTest.Units.Projectiles;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class GeneralPacketProcessor
    {
        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case Headers.KEEP_ALIVE:
                    {
                        GameServerConnectionManager.GetInstance().SendPacket(new Packet(Headers.KEEP_ALIVE));
                        break;
                    }
                case Headers.HANDSHAKE_2:
                    {
                        // Finish handshake
                        GameServerConnectionManager.GetInstance().SendPacket(new Packet(Headers.HANDSHAKE_3));

                        // Tell the server which user we are.
                        Packet userID = new Packet(Headers.CLIENT_USER_ID);
                        userID.AddInt(GameServerConnectionManager.GetInstance().user.id);
                        GameServerConnectionManager.GetInstance().SendPacket(userID);
                        break;
                    }
                case Headers.SERVER_DISCONNECT:
                    {
                        GameServerConnectionManager.GetInstance().DisconnectFromServer();
                        // Create a dialog, and add a listener to the OK button.
                        GameServerConnectionManager.GetInstance().OnDisconnect();
                        break;
                    }
                case Headers.GAME_OBJECT_ID:
                    {
                        // Received an id
                        int localID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = PacketUtil.DecodePacketInt(p, 4);
                        MultiplayerData data = MultiplayerDataManager.GetInstance().GetDataByLocalID(localID);

                        if (data is UnitMultiplayerData)
                        {
                            Unit unit = ((UnitMultiplayerData)data).unit;
                            unit.multiplayerData.serverID = serverID;

                            // Now we have the data, queue the unit for synching .. as in, move it to it's own location :]
                            unit.multiplayerData.moveTarget = unit.GetLocation();
                            unit.MoveToQueue(unit.GetLocation());
                        }
                        else if (data is BuildingMultiplayerData)
                        {
                            Building building = ((BuildingMultiplayerData)data).building;
                            building.multiplayerData.serverID = serverID;
                        }
                        else if (data is ProjectileMultiplayerData)
                        {
                            Projectile projectile = ((ProjectileMultiplayerData)data).projectile;
                            projectile.multiplayerData.serverID = serverID;
                            // Sync the projectile with the rest
                            Synchronizer.GetInstance().QueueProjectile(projectile);
                        }
                        break;
                    }
                case Headers.GAME_REQUEST_OBJECT_DATA:
                    {
                        int targetUserID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = PacketUtil.DecodePacketInt(p, 4);
                        Console.Out.WriteLine("Received object data request by " + targetUserID + ": " + serverID);
                        // Someone wants to know data
                        MultiplayerData data = MultiplayerDataManager.GetInstance().GetDataByServerID(serverID, true);

                        if (data != null && data.isLocal)
                        {
                            Packet response = new Packet(Headers.GAME_SEND_OBJECT_DATA);
                            response.AddInt(targetUserID);
                            response.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                            response.AddInt(serverID);
                            response.AddInt(data.GetObjectType());
                            if (data is BuildingMultiplayerData)
                            {
                                response.AddInt(((BuildingMultiplayerData)data).building.constructedBy.multiplayerData.serverID);
                            }
                            else if (data is ProjectileMultiplayerData)
                            {
                                Projectile projectile = ((ProjectileMultiplayerData)data).projectile;
                                response.AddInt(projectile.parent.multiplayerData.serverID);
                                //response.AddInt(projectile.target.multiplayerData.serverID);
                            }
                            GameServerConnectionManager.GetInstance().SendPacket(response);

                            // Queue it for a location update, since someone missed the previous one.
                            if (data is UnitMultiplayerData)
                                Synchronizer.GetInstance().QueueUnit(((UnitMultiplayerData)data).unit);
                            //else if( data is BuildingMultiplayerData )
                            //Synchronizer.GetInstance().QueueBuilding((BuildingMultiplayerData)data));
                        }
                        break;
                    }
                case Headers.GAME_SEND_OBJECT_DATA:
                    {
                        // We received missing data!
                        int playerID = PacketUtil.DecodePacketInt(p, 4);
                        int serverID = PacketUtil.DecodePacketInt(p, 8);
                        int type = PacketUtil.DecodePacketInt(p, 12);

                        switch (type)
                        {

                            case UnitHeaders.TYPE_BOWMAN:
                            case UnitHeaders.TYPE_ENGINEER:
                            case UnitHeaders.TYPE_SWORDMAN:
                            case UnitHeaders.TYPE_HORSEMAN:
                                {
                                    ObjectCreator.GetInstance().CreateUnit(playerID, serverID, type);
                                    break;
                                }
                            case BuildingHeaders.TYPE_BARRACKS:
                            case BuildingHeaders.TYPE_FACTORY:
                            case BuildingHeaders.TYPE_FORTRESS:
                            case BuildingHeaders.TYPE_RESOURCES_GATHER:
                            case BuildingHeaders.TYPE_SENTRY:
                                {
                                    ObjectCreator.GetInstance().CreateBuilding(playerID, serverID, type,
                                        PacketUtil.DecodePacketInt(p, 16));
                                    break;
                                }
                            case UnitHeaders.PROJECTILE_ARROW:
                                {
                                    ObjectCreator.GetInstance().CreateProjectile(
                                        PacketUtil.DecodePacketInt(p, 16), 
                                        PacketUtil.DecodePacketInt(p, 20), serverID);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
