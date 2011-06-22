using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Units;
using PathfindingTest.Multiplayer.Data;
using Microsoft.Xna.Framework;
using PathfindingTest.Combat;
using PathfindingTest.Units.Projectiles;
using PathfindingTest.Units.Damage;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class UnitPacketProcessor
    {
        public int locationMargin = 20;

        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case UnitHeaders.GAME_UNIT_LOCATION:
                    {
                        int serverID = PacketUtil.DecodePacketInt(p, 0);
                        int targetX = PacketUtil.DecodePacketInt(p, 4);
                        int targetY = PacketUtil.DecodePacketInt(p, 8);
                        int currentX = PacketUtil.DecodePacketInt(p, 12);
                        int currentY = PacketUtil.DecodePacketInt(p, 16);

                        MultiplayerData data;
                        int count = 0;
                        do {
                            data = MultiplayerDataManager.GetInstance().GetDataByServerID(serverID);
                            count++;
                            if (count > 15)
                            {
                                Console.Out.WriteLine("Unable to fetch data, requesting..");
                                Packet packet = new Packet(UnitHeaders.GAME_REQUEST_UNIT_DATA);
                                packet.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                                packet.AddInt(serverID);
                                GameServerConnectionManager.GetInstance().SendPacket(packet);

                                return;
                            }
                        }
                        while (data == null);

                        Unit unit = ((UnitMultiplayerData)data).unit;

                        if (unit.waypoints.Count == 0 || unit.waypoints.Last.Value.X != targetX ||
                            unit.waypoints.Last.Value.Y != targetY)
                        {
                            Point target = new Point(targetX, targetY);
                            unit.multiplayerData.moveTarget = target;
                            unit.multiplayerData.receivedPathRequest = true;
                            unit.MoveToQueue(target);
                        }

                        if (Math.Abs(unit.x - currentX) > 20 || Math.Abs(unit.y - currentY) > 20)
                        {
                            // Uhoh .. we're too far apart :(
                            unit.x = currentX;
                            unit.y = currentY;
                        }


                        break;
                    }
                case UnitHeaders.GAME_NEW_UNIT:
                    {

                        int playerID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = PacketUtil.DecodePacketInt(p, 4);
                        int type = PacketUtil.DecodePacketInt(p, 8);

                        CreateUnit(playerID, serverID, type);

                        break;
                    }
                case UnitHeaders.GAME_REQUEST_UNIT_DATA:
                    {
                        int targetUserID = PacketUtil.DecodePacketInt(p, 0);
                        int serverID = PacketUtil.DecodePacketInt(p, 4);
                        Console.Out.WriteLine("Received unit data request by " + targetUserID + ": " + serverID);
                        // Someone wants to know data
                        MultiplayerData data = MultiplayerDataManager.GetInstance().GetDataByServerID(serverID);

                        if (data != null && data.isLocal)
                        {
                            Packet response = new Packet(UnitHeaders.GAME_SEND_UNIT_DATA);
                            response.AddInt(targetUserID);
                            response.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                            response.AddInt(serverID);
                            response.AddInt(data.GetObjectType());
                            GameServerConnectionManager.GetInstance().SendPacket(response);

                            // Queue it for a location update, since someone missed the previous one.
                            if (data is UnitMultiplayerData)
                                Synchronizer.GetInstance().QueueUnit(((UnitMultiplayerData)data).unit);
                            //else if( data is BuildingMultiplayerData )
                                //Synchronizer.GetInstance().QueueBuilding((BuildingMultiplayerData)data));
                        }
                        break;
                    }
                case UnitHeaders.GAME_SEND_UNIT_DATA:
                    {
                        // We received missing data!
                        int playerID = PacketUtil.DecodePacketInt(p, 4);
                        int serverID = PacketUtil.DecodePacketInt(p, 8);
                        int type = PacketUtil.DecodePacketInt(p, 12);

                        CreateUnit(playerID, serverID, type);
                        break;
                    }
                case UnitHeaders.GAME_UNIT_MELEE_DAMAGE:
                    {
                        int damageSource = PacketUtil.DecodePacketInt(p, 0);
                        int fromServerID = PacketUtil.DecodePacketInt(p, 4);
                        int toServerID = PacketUtil.DecodePacketInt(p, 8);
                        Unit fromUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(fromServerID)).unit;
                        Unit targetUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(toServerID)).unit;


                        DamageEvent e = new DamageEvent(DecodeSource(damageSource, fromServerID), targetUnit, fromUnit);

                        targetUnit.OnDamage(e);
                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_SHOT:
                    {
                        int arrowServerID = PacketUtil.DecodePacketInt(p, 4);
                        int sourceServerID = PacketUtil.DecodePacketInt(p, 4);
                        int targetServerID = PacketUtil.DecodePacketInt(p, 8);
                        Unit sourceUnit = 
                            ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(sourceServerID)).unit;
                        Arrow arrow = new Arrow(sourceUnit,
                            ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(targetServerID)).unit);
                        arrow.multiplayerData.serverID = arrowServerID;
                        ((Bowman)sourceUnit).projectiles.AddLast(arrow);

                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_DAMAGE:
                    {
                        int projectileID = PacketUtil.DecodePacketInt(p, 0);
                        int sourceID = PacketUtil.DecodePacketInt(p, 4);
                        int targetID = PacketUtil.DecodePacketInt(p, 8);
                        Unit sourceUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(sourceID)).unit;
                        Unit targetUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(targetID)).unit;
                        Projectile projectile = 
                            ((ProjectileMultiplayerData) MultiplayerDataManager.GetInstance().GetDataByServerID(projectileID)).projectile;

                        DamageEvent e = new DamageEvent(projectile, targetUnit, sourceUnit);
                        targetUnit.OnDamage(e);

                        projectile.Dispose();
                        break;
                    }
            }
        }

        /// <summary>
        /// Gets the damage source from the identifier.
        /// </summary>
        /// <param name="damageSource">The source.</param>
        /// <param name="fromServerID">The server ID that created this event.</param>
        /// <returns>The damageSource, or null</returns>
        private DamageSource DecodeSource(int damageSource, int fromServerID)
        {
            Unit fromUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(fromServerID)).unit;

            switch (damageSource)
            {
                case UnitHeaders.DAMAGE_TYPE_FAST:
                    {
                        return new MeleeSwing(DamageEvent.DamageType.Fast, fromUnit.baseDamage);
                    }
                case UnitHeaders.DAMAGE_TYPE_HEAVY_MELEE:
                    {
                        return new MeleeSwing(DamageEvent.DamageType.HeavyMelee, fromUnit.baseDamage);
                    }
                case UnitHeaders.DAMAGE_TYPE_MELEE:
                    {
                        return new MeleeSwing(DamageEvent.DamageType.Melee, fromUnit.baseDamage);
                    }
                case UnitHeaders.DAMAGE_TYPE_RANGED:
                    {
                        return new MeleeSwing(DamageEvent.DamageType.Ranged, fromUnit.baseDamage);
                    }
            }
            return null;
        }

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="playerID">The player ID.</param>
        /// <param name="serverID">Server ID.</param>
        /// <param name="type">The type of the unit.</param>
        public void CreateUnit(int playerID, int serverID, int type)
        {
            Unit unit = null;
            switch (type)
            {
                case UnitHeaders.TYPE_BOWMAN:
                    {
                        unit =
                            Game1.GetInstance().GetPlayerByMultiplayerID(playerID).rangedStore.getUnit(Unit.Type.Ranged, 0, 0);
                        break;
                    }
                case UnitHeaders.TYPE_ENGINEER:
                    {
                        unit =
                            Game1.GetInstance().GetPlayerByMultiplayerID(playerID).meleeStore.getUnit(Unit.Type.Engineer, 0, 0);
                        break;
                    }
                case UnitHeaders.TYPE_SWORDMAN:
                    {
                        unit =
                            Game1.GetInstance().GetPlayerByMultiplayerID(playerID).meleeStore.getUnit(Unit.Type.Melee, 0, 0);
                        break;
                    }
            }
            unit.multiplayerData.serverID = serverID;
        }
    }
}
