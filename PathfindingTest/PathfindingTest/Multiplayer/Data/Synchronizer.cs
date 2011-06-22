using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using PathfindingTest.Buildings;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer.SocketConnection.InGame;
using PathfindingTest.Combat;
using PathfindingTest.Units.Projectiles;
using PathfindingTest.Units.Damage;

namespace PathfindingTest.Multiplayer.Data
{
    public class Synchronizer
    {
        private LinkedList<Unit> unitList = new LinkedList<Unit>();
        private LinkedList<Building> buildingList = new LinkedList<Building>();
        private LinkedList<DamageEvent> eventList = new LinkedList<DamageEvent>();
        private LinkedList<Projectile> projectileList = new LinkedList<Projectile>();

        private int maxObjectsPerFrame = 5;

        private static Synchronizer instance;

        public static Synchronizer GetInstance()
        {
            if (instance == null) instance = new Synchronizer();
            return instance;
        }

        private Synchronizer() { }

        /// <summary>
        /// Manage all units and buildings etc to be in sync with the rest of the players in the
        /// multiplayer game.
        /// </summary>
        public void Synchronize()
        {
            int objectsSynced = 0;

            while (objectsSynced < maxObjectsPerFrame && unitList.Count > 0)
            {
                // Sync this unit.
                Unit unit = unitList.First.Value;
                if (!unit.multiplayerData.isCreated)
                {
                    // Notify the rest of the world of the creation of this unit.
                    Packet newUnitPacket = new Packet(UnitHeaders.GAME_NEW_UNIT);

                    // Get this packet going before the other one
                    newUnitPacket.AddInt(unit.player.multiplayerID);
                    newUnitPacket.AddInt(unit.multiplayerData.serverID);
                    newUnitPacket.AddInt(unit.multiplayerData.GetObjectType());
                    // Notify everyone else that we have created a unit
                    GameServerConnectionManager.GetInstance().SendPacket(newUnitPacket);
                    unit.multiplayerData.isCreated = true;
                }


                Packet p = new Packet(UnitHeaders.GAME_UNIT_LOCATION);
                p.AddInt(unit.multiplayerData.serverID);
                p.AddInt(unit.multiplayerData.moveTarget.X);
                p.AddInt(unit.multiplayerData.moveTarget.Y);
                p.AddInt((int)unit.x);
                p.AddInt((int)unit.y);

                unit.multiplayerData.lastPulse = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
                GameServerConnectionManager.GetInstance().SendPacket(p);

                Console.Out.WriteLine("Synchronised " + unit);

                unitList.RemoveFirst();
                objectsSynced++;
            }


            while (objectsSynced < maxObjectsPerFrame && buildingList.Count > 0)
            {
                Building building = buildingList.First.Value;

                if (!building.multiplayerData.isCreated)
                {
                    // Notify the rest of the world of the creation of this unit.
                    Packet newBuildingPacket = new Packet(BuildingHeaders.GAME_NEW_BUILDING);

                    // Get this packet going before the other one
                    newBuildingPacket.AddInt(building.p.multiplayerID);
                    newBuildingPacket.AddInt(building.multiplayerData.serverID);
                    newBuildingPacket.AddInt(building.multiplayerData.GetObjectType());
                    newBuildingPacket.AddInt(building.constructedBy.multiplayerData.serverID);

                    // Notify everyone else that we have created a unit
                    GameServerConnectionManager.GetInstance().SendPacket(newBuildingPacket);
                    building.multiplayerData.isCreated = true;
                }

                Packet movePacket = new Packet(BuildingHeaders.GAME_BUILDING_LOCATION);
                movePacket.AddInt(building.multiplayerData.serverID);
                movePacket.AddInt((int)building.x);
                movePacket.AddInt((int)building.y);
                GameServerConnectionManager.GetInstance().SendPacket(movePacket);



                buildingList.RemoveFirst();
                objectsSynced++;
            }

            while (objectsSynced < maxObjectsPerFrame && eventList.Count > 0)
            {
                DamageEvent e = eventList.First.Value;

                Packet damagePacket = new Packet();
                if (e.by is Arrow)
                {
                    Arrow arrow = (Arrow)e.by;
                    damagePacket.SetHeader(UnitHeaders.GAME_UNIT_RANGED_DAMAGE);
                    damagePacket.AddInt(arrow.multiplayerData.serverID);
                    damagePacket.AddInt(e.source.multiplayerData.serverID);
                    damagePacket.AddInt(e.target.multiplayerData.serverID);
                }
                else if (e.by is MeleeSwing)
                {
                    MeleeSwing swing = (MeleeSwing)e.by;
                    damagePacket.SetHeader(UnitHeaders.GAME_UNIT_MELEE_DAMAGE);
                    damagePacket.AddInt(EncodeMeleeSwing(swing.type));
                    damagePacket.AddInt(e.source.multiplayerData.serverID);
                    damagePacket.AddInt(e.target.multiplayerData.serverID);
                }

                GameServerConnectionManager.GetInstance().SendPacket(damagePacket);

                eventList.RemoveFirst();
                objectsSynced++;
            }

            while (objectsSynced < maxObjectsPerFrame && projectileList.Count > 0)
            {
                Projectile toSync = projectileList.First.Value;

                if (toSync is Arrow && !toSync.multiplayerData.isCreated)
                {
                    Packet newArrowPacket = new Packet(UnitHeaders.GAME_UNIT_RANGED_SHOT);
                    newArrowPacket.AddInt(toSync.multiplayerData.serverID);
                    newArrowPacket.AddInt(toSync.parent.multiplayerData.serverID);
                    newArrowPacket.AddInt(toSync.target.multiplayerData.serverID);

                    GameServerConnectionManager.GetInstance().SendPacket(newArrowPacket);
                    toSync.multiplayerData.isCreated = true;
                }

                projectileList.RemoveFirst();
                objectsSynced++;
            }
        }

        /// <summary>
        /// Encodes a ranged damage source to an int.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The int.</returns>
        private int EncodeRanged(DamageSource source)
        {
            if (source is Arrow) return UnitHeaders.PROJECTILE_ARROW;
            return -1;
        }

       
        /// <summary>
        /// Identify the source of the DamageSource.
        /// </summary>
        /// <param name="type">The source to identify.</param>
        /// <returns>The identified source.</returns>
        private int EncodeMeleeSwing(DamageEvent.DamageType type){
            if( type == DamageEvent.DamageType.Fast ){
                return UnitHeaders.DAMAGE_TYPE_FAST;
            }
            else if (type == DamageEvent.DamageType.HeavyMelee)
            {
                return UnitHeaders.DAMAGE_TYPE_HEAVY_MELEE;
            }
            else if (type == DamageEvent.DamageType.Melee)
            {
                return UnitHeaders.DAMAGE_TYPE_MELEE;
            }
            else if (type == DamageEvent.DamageType.Ranged)
            {
                return UnitHeaders.DAMAGE_TYPE_RANGED;
            } 
            return -1;
        }

        public void QueueUnit(Unit unit)
        {
            this.unitList.AddLast(unit);
        }

        public void QueueBuilding(Building building)
        {
            this.buildingList.AddLast(building);
        }

        public void QueueDamageEvent(DamageEvent e)
        {
            this.eventList.AddLast(e);
        }

        public void QueueProjectile(Projectile p)
        {
            this.projectileList.AddLast(p);
        }
    }
}
