﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Protocol
{
    public class UnitHeaders
    {
        // 0 - 31   Chat reserved ranges (0x00, 0x1F )
        // 32 - 47  General game reserved ranges (0x20, 0x2F)
        // 48 - 63  Unit reserved ranges (0x30, 0x3F)
        // 64 - 79  Building reserved ranges (0x40, 0x4F)
        // 80 - 96  Test reserved ranges (0x50, 0x5F);

        /// <summary>
        /// Types to be used to request object IDs
        /// </summary>
        public const int TYPE_BOWMAN = 10, TYPE_SWORDMAN = 11, TYPE_ENGINEER = 12, TYPE_HORSEMAN = 13;

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
        /// Client has damaged a remote unit.
        /// fromSource as defined in UnitHeaders.
        /// [Header] [Int32 fromSource] [Int32 damagingServerID] [Int32 targetServerID]
        /// </summary>
        public const byte GAME_UNIT_MELEE_DAMAGE = 0x32;

        /// <summary>
        /// Client has shot off a projectile.
        /// [Header] [Int32 arrowServerID] [Int32 sourceServerID] [Int32 targetServerID]
        /// </summary>
        public const byte GAME_UNIT_RANGED_SHOT = 0x33;
    }
}
