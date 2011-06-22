using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PathfindingTest;
using PathfindingTest.Input;

namespace RTS_Builder
{
    public class Engineer : Unit, MouseClickListener
    {
        
        /// <summary>
        /// Engineer Constructor.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="cm"></param>
        /// <param name="startLocation"></param>
        /// <param name="c"></param>
        public Engineer(Player p, Vector2 startLocation, float movementSpeed, float direction, Color c)
        {
            this.p = p;
            this.movementSpeed = movementSpeed;
            this.direction = direction;
            this.c = c;
            this.type = UnitType.Engineer;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/Engineer");

            Vector2 newLocation = new Vector2((startLocation.X - (this.texture.Width / 2)), (startLocation.Y - (this.texture.Height / 2)));
            this.location = newLocation;

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public override void Update(KeyboardState ks, MouseState ms)
        {
            if (hasToMove == true)
            {
                this.move();
            }
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal override void Draw(SpriteBatch sb)
        {
            sb.Draw(this.texture, this.location, this.c);
        }

        void MouseClickListener.OnMouseClick(MouseEvent me)
        {
            if (me.button == MouseEvent.MOUSE_BUTTON_3 && selected)
            {
                HUD h = HUD.GetInstance();
                MouseState ms = Mouse.GetState();
                
                if (h != null)
                {
                    if (h.draw)
                    {
                        Rectangle mr = new Rectangle(ms.X, ms.Y, 1, 1);

                        if (!h.DefineRectangle().Contains(mr))
                        {
                            this.setWaypoint(ms.X, ms.Y);
                        }
                    }
                }
                else
                {
                    this.setWaypoint(ms.X, ms.Y);
                }
            }
        }

        void MouseClickListener.OnMouseRelease(MouseEvent me)
        {
        }
    }
}
