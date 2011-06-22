using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.Selection
{
    public class UnitControlManager
    {
        private UnitControlManager instance { get; set; }
        private UnitSelection[] selections { get; set; }

        /// <summary>
        /// Sets a unit selection at the given index. Valid ranges are 0 .. 9. 
        /// Giving a parameter outside this, will result in IndexOutOfRangeException.
        /// </summary>
        /// <param name="index">The index, 0 .. 9</param>
        public void SetUnitSelectionAt(int index, UnitSelection units)
        {
            selections[index] = units;
        }

        public UnitControlManager GetInstance()
        {
            if (instance == null) instance = new UnitControlManager();
            return instance;
        }

        private UnitControlManager()
        {
            selections = new UnitSelection[10];
        }
    }
}
