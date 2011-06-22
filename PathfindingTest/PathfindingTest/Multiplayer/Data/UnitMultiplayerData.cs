using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units;
using Microsoft.Xna.Framework;
using SocketLibrary.Protocol;
using PathfindingTest.Units.Melee;

namespace PathfindingTest.Multiplayer.Data
{
    public class UnitMultiplayerData : MultiplayerData
    {
        public Unit unit { get; set; }
        // Send an update every 3 seconds.
        public int updateRate = 3000;

        public Boolean receivedPathRequest { get; set; }

        /// <summary>
        /// Unit must know where it is going to.
        /// Used for multiplayer.
        /// </summary>
        public Point moveTarget { get; set; }

        public UnitMultiplayerData(Unit unit, Boolean isLocal)
            : base(isLocal)
        {
            this.unit = unit;
        }

        /// <summary>
        /// Gets the type of this unit as defined in UnitHeaders.cs
        /// </summary>
        /// <returns></returns>
        public override int GetObjectType()
        {
            if (this.unit is Bowman) return UnitHeaders.TYPE_BOWMAN;
            else if (this.unit is Swordman) return UnitHeaders.TYPE_SWORDMAN;
            else if (this.unit is Engineer) return UnitHeaders.TYPE_ENGINEER;
            else return Int32.MaxValue;
        }
    }
}
