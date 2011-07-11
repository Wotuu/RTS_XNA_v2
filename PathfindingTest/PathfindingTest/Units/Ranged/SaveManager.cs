using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PathfindingTest.Pathfinding;
using PathfindingTest.Interfaces;
using AStarCollisionMap.Pathfinding;

namespace PathfindingTest.SaveLoad
{
    public class SaveManager
    {
        private static SaveManager instance { get; set; }



        private SaveManager()
        {

        }

        public void SaveNodes(String fileLocation)
        {
            XmlTextWriter textWriter = new XmlTextWriter(fileLocation, null);
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteStartDocument();
            textWriter.WriteStartElement("Nodes");

            foreach( XMLAble node in PathfindingNodeManager.GetInstance().nodeList ){
                node.AddToXML(textWriter);
            }


            textWriter.WriteEndElement();
            textWriter.WriteEndDocument();
            textWriter.Close();
        }

        public static SaveManager GetInstance()
        {
            if (instance == null) instance = new SaveManager();
            return instance;
        }
    }
}
