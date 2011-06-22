using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAInputHandler.MouseInput
{
    public interface MouseMotionListener
    {
        void OnMouseMotion(MouseEvent e);
        void OnMouseDrag(MouseEvent e);
    }
}
