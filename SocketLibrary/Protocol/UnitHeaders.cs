using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Protocol
{
    public class UnitHeaders
    {
        /// <summary>
        /// Types to be used to request object IDs
        /// </summary>
        public const int TYPE_BOWMAN = 10, TYPE_SWORDMAN = 11, TYPE_ENGINEER = 12;

        public const int DAMAGE_TYPE_FAST = 20, DAMAGE_TYPE_HEAVY_MELEE = 21, DAMAGE_TYPE_MELEE = 22, DAMAGE_TYPE_RANGED = 23;

        public const int PROJECTILE_ARROW = 30;


        /// <summary>
        /// Client synchronises a unit
        /// [Header] [Int32 serverID] [Int32 targetX] [Int32 targetY] [Int32 currentX] [Int32 currentY]
        /// </summary>
        public const byte GAME_UNIT_LOCATION = 0x30;

        /// <summary>
        /// Someone made a new unit. A unit location package will follow soon enough.
        /// [Header] [Int32 playerID] [Int32 serverID] [Int32 type]
        /// </summary>
        public const byte GAME_NEW_UNIT = 0x31;

        /// <summary>
        /// Client didn't receive data about a unit, when it did need it for processing.
        /// [Header] [Int32 requestingPlayerID] [Int32 serverID]
        /// </summary>
        public const byte GAME_REQUEST_UNIT_DATA = 0x32;

        /// <summary>
        /// Client that manages the data about this unit, will reply with the data
        /// and a movement update right after
        /// [Header] [Int32 requestingPlayerID] [Int32 owningPlayerID] [Int32 serverID] [Int32 type]
        /// </summary>
        public const byte GAME_SEND_UNIT_DATA = 0x33;

        /// <summary>
        /// Client has damaged a remote unit.
        /// fromSource as defined in UnitHeaders.
        /// [Header] [Int32 fromSource] [Int32 damagingServerID] [Int32 targetServerID]
        /// </summary>
        public const byte GAME_UNIT_MELEE_DAMAGE = 0x34;

        /// <summary>
        /// Client has shot off a projectile.
        /// [Header] [Int32 arrowServerID] [Int32 sourceServerID] [Int32 targetServerID]
        /// </summary>
        public const byte GAME_UNIT_RANGED_SHOT = 0x35;

        /// <summary>
        /// Client has done damage with a projectile.
        /// [Header] [Int32 projectileServerID] [Int32 sourceServerID] [Int32 targetServerID]
        /// </summary>
        public const byte GAME_UNIT_RANGED_DAMAGE = 0x36;

    }
}
