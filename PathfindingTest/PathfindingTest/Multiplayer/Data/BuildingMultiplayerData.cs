using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Buildings;
using SocketLibrary.Protocol;

namespace PathfindingTest.Multiplayer.Data
{
    public class BuildingMultiplayerData : MultiplayerData
    {
        public Building building { get; set; }

        public BuildingMultiplayerData(Building building, Boolean isLocal)
            : base(isLocal)
        {
            this.building = building;
        }

        /// <summary>
        /// Gets the type of this unit as defined in UnitHeaders.cs
        /// </summary>
        /// <returns></returns>
        public override int GetObjectType()
        {
            if (this.building is Barracks) return BuildingHeaders.TYPE_BARRACKS;
            else if (this.building is Factory) return BuildingHeaders.TYPE_FACTORY;
            else if (this.building is Fortress) return BuildingHeaders.TYPE_FORTRESS;
            else if (this.building is ResourceGather) return BuildingHeaders.TYPE_RESOURCES_GATHER;
            else return Int32.MaxValue;
        }
    }
}
