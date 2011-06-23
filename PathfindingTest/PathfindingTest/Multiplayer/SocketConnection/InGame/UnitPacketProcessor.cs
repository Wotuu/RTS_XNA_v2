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
                            if (count > 5)
                            {
                                Console.Out.WriteLine("Unable to fetch data (unit), requesting..");
                                Packet packet = new Packet(Headers.GAME_REQUEST_OBJECT_DATA);
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

                        ObjectCreator.GetInstance().CreateUnit(playerID, serverID, type);

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

                        ObjectCreator.GetInstance().CreateProjectile(sourceServerID, targetServerID, arrowServerID);
                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_DAMAGE:
                    {
                        int projectileID = PacketUtil.DecodePacketInt(p, 0);
                        int sourceID = PacketUtil.DecodePacketInt(p, 4);
                        int targetID = PacketUtil.DecodePacketInt(p, 8);
                        Unit sourceUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(sourceID)).unit;
                        Unit targetUnit = ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(targetID)).unit;

                        MultiplayerData data;
                        int count = 0;
                        do
                        {
                            data = MultiplayerDataManager.GetInstance().GetDataByServerID(projectileID);
                            count++;
                            if (count > 5)
                            {
                                Console.Out.WriteLine("Unable to fetch data (projectile), requesting..");
                                Packet packet = new Packet(Headers.GAME_REQUEST_OBJECT_DATA);
                                packet.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                                packet.AddInt(projectileID);
                                GameServerConnectionManager.GetInstance().SendPacket(packet);

                                return;
                            }
                        }
                        while (data == null);
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
    }
}
