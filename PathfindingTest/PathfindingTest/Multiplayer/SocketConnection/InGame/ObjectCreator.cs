using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using SocketLibrary.Protocol;
using PathfindingTest.Buildings;
using PathfindingTest.Players;
using PathfindingTest.Multiplayer.Data;
using PathfindingTest.Units.Projectiles;

namespace PathfindingTest.Multiplayer.SocketConnection.InGame
{
    public class ObjectCreator
    {
        private static ObjectCreator instance;

        private ObjectCreator() { }


        public static ObjectCreator GetInstance()
        {
            if (instance == null) instance = new ObjectCreator();
            return instance;
        }

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="playerID">The player ID.</param>
        /// <param name="serverID">Server ID.</param>
        /// <param name="type">The type of the unit.</param>
        public void CreateUnit(int playerID, int serverID, int type)
        {
            if (MultiplayerDataManager.GetInstance().GetDataByServerID(serverID, false) != null) return;
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
                case UnitHeaders.TYPE_HORSEMAN:
                    {
                        unit =
                            Game1.GetInstance().GetPlayerByMultiplayerID(playerID).meleeStore.getUnit(Unit.Type.Fast, 0, 0);
                        break;
                    }
            }
            if( unit != null ) unit.multiplayerData.serverID = serverID;
        }

        /// <summary>
        /// Creates a building.
        /// </summary>
        /// <param name="playerID">The player ID.</param>
        /// <param name="serverID">Server ID.</param>
        /// <param name="type">The type of the unit.</param>
        public void CreateBuilding(int playerID, int serverID, int type, int byID)
        {
            if (MultiplayerDataManager.GetInstance().GetDataByServerID(serverID, false) != null) return;
            Building building = null;
            Player p = Game1.GetInstance().GetPlayerByMultiplayerID(playerID);
            Engineer engineer = (Engineer)((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(byID, true)).unit;
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
                case BuildingHeaders.TYPE_SENTRY:
                    {
                        building = new Sentry(p, p.color);
                        break;
                    }
            }
            building.constructedBy = engineer;
            building.multiplayerData.serverID = serverID;
        }

        /// <summary>
        /// Creates a projectile.
        /// </summary>
        /// <param name="shooterServerID">The shooter's ID.</param>
        /// <param name="targetServerID">The target's ID.</param>
        /// <param name="arrowServerID">The arrow's ID</param>
        public void CreateProjectile(int shooterServerID, int targetServerID, int arrowServerID)
        {
            Unit sourceUnit =
                ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(shooterServerID, true)).unit;
            Arrow arrow = new Arrow(sourceUnit,
                ((UnitMultiplayerData)MultiplayerDataManager.GetInstance().GetDataByServerID(targetServerID, true)).unit);
            arrow.multiplayerData.serverID = arrowServerID;
            //((Bowman)sourceUnit).projectiles.AddLast(arrow); @@WMP moet naar de arrowmanager 
            Console.Out.WriteLine("Created arrow arrow by request; " + arrowServerID);
        }
    }
}
