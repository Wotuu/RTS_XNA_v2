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
            }
        }
    }
}
