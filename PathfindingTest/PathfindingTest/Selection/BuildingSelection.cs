using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Buildings;
using CustomLists.Lists;

namespace PathfindingTest.Selection
{
    public class BuildingSelection
    {

        public CustomArrayList<Building> buildings { get; set; }

        public BuildingSelection()
        {
            this.buildings = new CustomArrayList<Building>();
        }

        public BuildingSelection(CustomArrayList<Building> buildings)
        {
            this.buildings = buildings;
        }

        public void DeselectAll()
        {
            for( int i = 0; i < this.buildings.Count(); i++){
                this.buildings.ElementAt(i).selected = false;
            }
        }

        public void SelectAll()
        {
            for (int i = 0; i < this.buildings.Count(); i++)
            {
                this.buildings.ElementAt(i).selected = true;
            }
        }
    }
}
