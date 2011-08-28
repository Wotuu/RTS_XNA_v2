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
using System.Threading;
using CustomLists.Lists;

namespace PathfindingTest.Multiplayer.Data
{
    public class Synchronizer
    {
        private CustomArrayList<Unit> unitList = new CustomArrayList<Unit>();
        private CustomArrayList<Building> buildingList = new CustomArrayList<Building>();
        private CustomArrayList<DamageEvent> eventList = new CustomArrayList<DamageEvent>();
        private CustomArrayList<Projectile> projectileList = new CustomArrayList<Projectile>();

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

            while (objectsSynced < maxObjectsPerFrame && unitList.Count() > 0)
            {
                // Sync this unit.
                Unit unit = unitList.GetFirst();
                if (unit.multiplayerData.serverID != -1)
                {
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

                    // Console.Out.WriteLine("Synchronised " + unit);

                    unitList.RemoveFirst();
                    objectsSynced++;
                }
            }


            while (objectsSynced < maxObjectsPerFrame && buildingList.Count() > 0)
            {
                Building building = buildingList.GetFirst();

                if (building.multiplayerData.serverID != -1)
                {
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
            }

            while (objectsSynced < maxObjectsPerFrame && eventList.Count() > 0)
            {
                DamageEvent e = eventList.GetFirst();

                Packet damagePacket = new Packet();
                if (e.by is MeleeSwing)
                {
                    MeleeSwing swing = (MeleeSwing)e.by;
                    damagePacket.SetHeader(UnitHeaders.GAME_UNIT_MELEE_DAMAGE);
                    damagePacket.AddInt(EncodeMeleeSwing(swing.type));
                    damagePacket.AddInt(e.source.multiplayerData.serverID);
                    if (e.target is Unit) damagePacket.AddInt(((Unit)e.target).multiplayerData.serverID);
                    else if (e.target is Building) damagePacket.AddInt(((Building)e.target).multiplayerData.serverID);
                    //damagePacket.AddInt(e.target.multiplayerData.serverID);
                    GameServerConnectionManager.GetInstance().SendPacket(damagePacket);
                }


                eventList.RemoveFirst();
                objectsSynced++;
            }

            while (objectsSynced < maxObjectsPerFrame && projectileList.Count() > 0)
            {
                Projectile toSync = projectileList.GetFirst();

                if (toSync.multiplayerData.serverID != -1)
                {
                    if (toSync is Arrow && !toSync.multiplayerData.isCreated)
                    {
                        Packet newArrowPacket = new Packet(UnitHeaders.GAME_UNIT_RANGED_SHOT);
                        newArrowPacket.AddInt(toSync.multiplayerData.serverID);
                        newArrowPacket.AddInt(toSync.parent.multiplayerData.serverID);
                        if (toSync.target is Unit) newArrowPacket.AddInt(((Unit)toSync.target).multiplayerData.serverID);
                        else if (toSync.target is Building) newArrowPacket.AddInt(((Building)toSync.target).multiplayerData.serverID); 

                        Console.Out.WriteLine("Sending existance of arrow " + toSync.multiplayerData.serverID);
                        GameServerConnectionManager.GetInstance().SendPacket(newArrowPacket);
                        toSync.multiplayerData.isCreated = true;
                    }

                    projectileList.RemoveFirst();
                    objectsSynced++;
                }
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
        private int EncodeMeleeSwing(DamageEvent.DamageType type)
        {
            if (type == DamageEvent.DamageType.Fast)
            {
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

        #region Already in Queue
        public Boolean AlreadyInQueue(Unit unit)
        {
            try
            {
                for (int i = 0; i < this.unitList.Count(); i++)
                {
                    Unit currentUnit = this.unitList.ElementAt(i);
                    if (currentUnit == unit) return true;
                }
                return false;
            }
            catch (Exception e) { return false; }
        }

        public Boolean AlreadyInQueue(Building building)
        {
            try
            {
                for (int i = 0; i < this.buildingList.Count(); i++)
                {
                    Building currentBuilding = this.buildingList.ElementAt(i);
                    if (currentBuilding == building) return true;
                }
                return false;
            }
            catch (Exception e) { return false; }
        }

        public Boolean AlreadyInQueue(DamageEvent e)
        {
            try
            {
                for (int i = 0; i < this.eventList.Count(); i++)
                {
                    DamageEvent currentEvent = this.eventList.ElementAt(i);
                    if (currentEvent == e) return true;
                }
                return false;
            }
            catch (Exception ex) { return false; }
        }

        public Boolean AlreadyInQueue(Projectile projectile)
        {
            try
            {
                for (int i = 0; i < this.projectileList.Count(); i++)
                {
                    Projectile currentProjectile = this.projectileList.ElementAt(i);
                    if (currentProjectile == projectile) return true;
                }
                return false;
            }
            catch (Exception e) { return false; }
        }
        #endregion

        public void QueueUnit(Unit unit)
        {
            if (!this.AlreadyInQueue(unit))
            {
                this.unitList.AddLast(unit);
                // Console.WriteLine("StackTrace: '{0}'\n target={1}", Environment.StackTrace, unit.multiplayerData.moveTarget);
            }
        }

        public void QueueBuilding(Building building)
        {
            if (!this.AlreadyInQueue(building)) this.buildingList.AddLast(building);
        }

        public void QueueDamageEvent(DamageEvent e)
        {
            if (!this.AlreadyInQueue(e)) this.eventList.AddLast(e);
        }

        public void QueueProjectile(Projectile p)
        {
            if (!this.AlreadyInQueue(p)) this.projectileList.AddLast(p);
        }
    }
}
