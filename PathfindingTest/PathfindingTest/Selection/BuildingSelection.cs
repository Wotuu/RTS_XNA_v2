using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Buildings;

namespace PathfindingTest.Selection
{
    public class BuildingSelection
    {

        public LinkedList<Building> buildings { get; set; }

        public BuildingSelection()
        {
            this.buildings = new LinkedList<Building>();
        }

        public BuildingSelection(LinkedList<Building> buildings)
        {
            this.buildings = buildings;
        }

        public void DeselectAll()
        {
            foreach (Building b in buildings)
            {
                b.selected = false;
            }
        }

        public void SelectAll()
        {
            foreach (Building b in buildings)
            {
                b.selected = true;
            }
        }
    }
}
