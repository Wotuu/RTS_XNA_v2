using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Protocol
{
    public class BuildingHeaders
    {
        // 0 - 31   Chat reserved ranges (0x00, 0x1F )
        // 32 - 47  General game reserved ranges (0x20, 0x2F)
        // 48 - 63  Unit reserved ranges (0x30, 0x3F)
        // 64 - 79  Building reserved ranges (0x40, 0x4F)
        // 80 - 96  Test reserved ranges (0x50, 0x5F);

        /// <summary>
        /// Types to be used to request object IDs
        /// </summary>
        public const int TYPE_BARRACKS = 0, TYPE_FACTORY = 1, TYPE_FORTRESS = 2, TYPE_RESOURCES_GATHER = 3, TYPE_SENTRY = 4;


        /// <summary>
        /// Client synchronizes a building
        /// [Header] [Int32 serverID] [Int32 locationX] [Int32 locationY]
        /// </summary>
        public const byte GAME_BUILDING_LOCATION = 0x40;

        /// <summary>
        /// Client notifies the rest of a new building
        /// [Header] [Int32 playerID] [Int32 serverID] [Int32 buildingType] [Int32 constructedByServerID]
        /// </summary>
        public const byte GAME_NEW_BUILDING = 0x41;
    }
}
