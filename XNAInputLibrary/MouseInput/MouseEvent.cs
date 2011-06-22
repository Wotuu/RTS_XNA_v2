using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAInputHandler.MouseInput
{
    public class MouseEvent
    {
        public const int MOUSE_BUTTON_1 = 0, MOUSE_BUTTON_2 = 1, MOUSE_BUTTON_3 = 2;
        public const int STATE_RELEASED = 10, STATE_CLICKED = 11, STATE_MOVED = 12, STATE_DRAGGED = 13;

        public int button { get; set; }
        public int state { get; set; }
        public Point location { get; set; }


        public MouseEvent(int button, int state, Point location) {
            this.button = button;
            this.state = state;
            this.location = location;
        }
    }
}
