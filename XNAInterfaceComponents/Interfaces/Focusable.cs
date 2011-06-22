using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAInterfaceComponents.Interfaces
{
    interface Focusable
    {
        void OnFocusReceived();

        void OnFocusLost();
    }
}
