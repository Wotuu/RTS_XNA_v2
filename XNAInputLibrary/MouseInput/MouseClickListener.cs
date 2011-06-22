using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAInputHandler.MouseInput
{
    public interface MouseClickListener
    {
        void OnMouseClick(MouseEvent m_event);
        void OnMouseRelease(MouseEvent m_event);
    }
}
