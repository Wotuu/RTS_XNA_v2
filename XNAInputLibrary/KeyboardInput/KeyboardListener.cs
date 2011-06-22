using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAInputLibrary.KeyboardInput
{
    public interface KeyboardListener
    {
        void OnKeyPressed(KeyEvent e);
        void OnKeyTyped(KeyEvent e);
        void OnKeyReleased(KeyEvent e);
    }
}
