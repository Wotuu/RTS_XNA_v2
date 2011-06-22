using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PathfindingTest.Interfaces
{
    interface XMLAble
    {
        void AddToXML(XmlTextWriter textWriter);
    }
}
