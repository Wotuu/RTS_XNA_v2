using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNAInputHandler.MouseInput;

public delegate void MouseClicked(MouseEvent m);
public delegate void MouseReleased(MouseEvent m);
public delegate void MouseMotion(MouseEvent m);
public delegate void MouseDrag(MouseEvent m);

namespace XNAInputHandler.MouseInput
{
    public class MouseManager
    {
        public MouseClicked mouseClickedListeners { get; set; }
        public MouseReleased mouseReleasedListeners { get; set; }
        public MouseClicked mouseMotionListeners { get; set; }
        public MouseReleased mouseDragListeners { get; set; }

        private ButtonState previousLmbState { get; set; }

        private ButtonState previousMmbState { get; set; }

        private ButtonState previousRmbState { get; set; }

        private MouseState previousMouseState { get; set; }



        private static MouseManager instance;

        public void Update(Game game)
        {
            MouseState state = Mouse.GetState();
            // Point location = new Point(state.X, state.Y);
            if (!game.IsActive) return;

            // 0: Left mouse click, 1: middle mouse click, 2: right mouse click, 3: left mouse release, etc
            Boolean[] firedEvents = new Boolean[6];

            if (previousLmbState != state.LeftButton)
            {
                // Was released, is now pressed, thus we clicked
                if (previousLmbState == ButtonState.Released)
                {
                    if (mouseClickedListeners != null)
                    {
                        mouseClickedListeners(new MouseEvent(MouseEvent.MOUSE_BUTTON_1, MouseEvent.STATE_CLICKED, new Point(state.X, state.Y)));
                        firedEvents[0] = true;
                    }
                }
                // Else, was clicked, is now released
                else if (previousLmbState == ButtonState.Pressed)
                {
                    if (mouseReleasedListeners != null)
                    {
                        mouseReleasedListeners(new MouseEvent(MouseEvent.MOUSE_BUTTON_1, MouseEvent.STATE_RELEASED, new Point(state.X, state.Y)));
                        firedEvents[3] = true;
                    }
                }
            }
            previousLmbState = state.LeftButton;



            if (previousMmbState != state.MiddleButton)
            {

                if (previousMmbState == ButtonState.Released)
                {
                    if (mouseClickedListeners != null)
                    {
                        mouseClickedListeners(new MouseEvent(MouseEvent.MOUSE_BUTTON_2, MouseEvent.STATE_CLICKED, new Point(state.X, state.Y)));
                        firedEvents[1] = true;
                    }
                }
                // Else, was clicked, is now released
                else if (previousMmbState == ButtonState.Pressed)
                {
                    if (mouseReleasedListeners != null)
                    {
                        mouseReleasedListeners(new MouseEvent(MouseEvent.MOUSE_BUTTON_2, MouseEvent.STATE_RELEASED, new Point(state.X, state.Y)));
                        firedEvents[4] = true;
                    }
                }
            }
            previousMmbState = state.MiddleButton;


            if (previousRmbState != state.RightButton)
            {

                if (previousRmbState == ButtonState.Released)
                {
                    if (mouseClickedListeners != null)
                    {
                        mouseClickedListeners(new MouseEvent(MouseEvent.MOUSE_BUTTON_3, MouseEvent.STATE_CLICKED, new Point(state.X, state.Y)));
                        firedEvents[2] = true;
                    }
                }
                // Else, was clicked, is now released
                else if (previousRmbState == ButtonState.Pressed)
                {
                    if (mouseReleasedListeners != null)
                    {
                        mouseReleasedListeners(new MouseEvent(MouseEvent.MOUSE_BUTTON_3, MouseEvent.STATE_RELEASED, new Point(state.X, state.Y)));
                        firedEvents[5] = true;
                    }
                }
            }
            previousRmbState = state.RightButton;


            /*if (firedEvents[0] || firedEvents[1] || firedEvents[2] )
            {
                int buttonNumber = -1;
                if( firedEvents[0] ) buttonNumber = MouseEvent.MOUSE_BUTTON_1;
                else if( firedEvents[1] ) buttonNumber = MouseEvent.MOUSE_BUTTON_2;
                else if (firedEvents[2]) buttonNumber = MouseEvent.MOUSE_BUTTON_3;

            }*/

            if (previousMouseState != null &&
                (state.X != previousMouseState.X || state.Y != previousMouseState.Y))
            {
                if (IsButtonDown(state) != -1)
                {
                    if (mouseDragListeners != null)
                    {
                        mouseDragListeners(new MouseEvent(IsButtonDown(state), MouseEvent.STATE_DRAGGED, new Point(state.X, state.Y)));
                    }
                }
                else
                {
                    if (mouseMotionListeners != null)
                    {
                        mouseMotionListeners(new MouseEvent(IsButtonDown(state), MouseEvent.STATE_MOVED, new Point(state.X, state.Y)));
                    }
                }
            }
            previousMouseState = state;

        }

        /// <summary>
        /// Checks whether a button is down.
        /// </summary>
        /// <param name="state">The MouseState</param>
        /// <returns>MouseEvent.MOUSE_BUTTON_1 etc etc</returns>
        public int IsButtonDown(MouseState state)
        {
            if (state.LeftButton == ButtonState.Pressed) return MouseEvent.MOUSE_BUTTON_1;
            else if (state.MiddleButton == ButtonState.Pressed) return MouseEvent.MOUSE_BUTTON_2;
            else if (state.RightButton == ButtonState.Pressed) return MouseEvent.MOUSE_BUTTON_3;
            return -1;

        }

        public static MouseManager GetInstance()
        {
            if (instance == null) instance = new MouseManager();
            return instance;
        }

        private MouseManager()
        {
        }
    }
}
