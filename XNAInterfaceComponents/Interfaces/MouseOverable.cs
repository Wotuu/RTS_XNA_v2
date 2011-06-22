using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInputHandler.MouseInput;

namespace XNAInterfaceComponents.Interfaces
{
    interface MouseOverable
    {
        void OnMouseEnter(MouseEvent e);
        void OnMouseExit(MouseEvent e);
    }
}
