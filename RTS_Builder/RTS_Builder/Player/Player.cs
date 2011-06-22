using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RTS_Builder
{
    public class Player
    {

        Game1 g;
        Color c; 

        Texture2D selectionTex;
        Texture2D selectedTex;

        LinkedList<Unit> units = new LinkedList<Unit>();
        LinkedList<Unit> selected = new LinkedList<Unit>();

        public LinkedList<Building> buildings { get; set; }

        HUD hud;

        Vector2 selectionStart;
        Vector2 selectionEnd;

        Boolean drawSelectionRectangle = false;
        Boolean leftMouseButtonPressed = false;

        /// <summary>
        /// Player constructor.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cm"></param>
        /// <param name="c"></param>
        public Player(Game1 g, Color c)
        {
            this.g = g;
            this.c = c;

            selectionTex = Game1.GetInstance().Content.Load<Texture2D>("Selection");
            selectedTex = Game1.GetInstance().Content.Load<Texture2D>("Selected");

            Unit u = new Engineer(this, new Vector2(g.screenWidth / 2, g.screenHeight / 2), 1f, 0f, c);
            units.AddLast(u);

            buildings = new LinkedList<Building>();
            hud = new HUD(this, c);
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public void Update(KeyboardState ks, MouseState ms)
        {
            CheckDrawSelection();

            foreach (Unit u in units)
            {
                u.Update(ks, ms);
            }

            foreach (Building b in buildings)
            {
                b.Update(ks, ms);
            }

            hud.Update(Keyboard.GetState(), Mouse.GetState());
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {

            if (drawSelectionRectangle)
            {
                sb.Draw(selectionTex, DefineSelectionRectangle(), new Color(150, 0, 0, 25));
            }

            foreach (Unit u in units)
            {
                u.Draw(sb);
            }
            foreach (Engineer e in selected)
            {
                sb.Draw(selectedTex, e.DefineSelectedRectangle(), c);
            }

            foreach (Building b in buildings)
            {
                b.Draw(sb);
            }

            hud.Draw(sb);
        }

        /// <summary>
        /// Checks the area in which the 'Selection' texture will be drawn.
        /// </summary>
        private void CheckDrawSelection()
        {
            MouseState ms = Mouse.GetState();
            Rectangle mr = new Rectangle(ms.X, ms.Y, 1, 1);

            if (!hud.DefineRectangle().Contains(mr))
            {
                if (ms.LeftButton == ButtonState.Pressed && leftMouseButtonPressed == false)
                {
                    leftMouseButtonPressed = true;
                    drawSelectionRectangle = true;
                    selectionStart = new Vector2(ms.X, ms.Y);
                }
                if (leftMouseButtonPressed == true)
                {
                    selectionEnd = new Vector2(ms.X, ms.Y);
                }
                if (ms.LeftButton == ButtonState.Released && leftMouseButtonPressed == true)
                {
                    selected.Clear();
                    foreach (Unit u in units)
                    {
                        u.setSelected(false);
                        if (DefineSelectionRectangle().Contains(u.DefineRectangle()))
                        {
                            selected.AddLast(u);
                            u.setSelected(true);
                        }
                    }

                    drawSelectionRectangle = false;
                    leftMouseButtonPressed = false;
                }
            }
        }

        /// <summary>
        /// Defines the area in which the 'Selection' texture will be drawn.
        /// </summary>
        /// <returns></returns>
        private Rectangle DefineSelectionRectangle()
        {
            int selectStartX = (int)selectionStart.X;
            int selectStartY = (int)selectionStart.Y;
            int selectEndX = (int)selectionEnd.X;
            int selectEndY = (int)selectionEnd.Y;

            Rectangle r = new Rectangle();

            if (selectEndX < selectStartX && selectEndY > selectStartY)
            {
                r = new Rectangle(selectEndX, selectStartY, selectStartX - selectEndX, selectEndY - selectStartY);
            }
            else if (selectEndX < selectStartX && selectEndY < selectStartY)
            {
                r = new Rectangle(selectEndX, selectEndY, selectStartX - selectEndX, selectStartY - selectEndY);
            }
            else if (selectEndX > selectStartX && selectEndY > selectStartY)
            {
                r = new Rectangle(selectStartX, selectStartY, selectEndX - selectStartX, selectEndY - selectStartY);
            }
            else if (selectEndX > selectStartX && selectEndY < selectStartY)
            {
                r = new Rectangle(selectStartX, selectEndY, selectEndX - selectStartX, selectStartY - selectEndY);
            }

            return r;
        }

        /// <summary>
        /// Used to retrieve the units that are selected.
        /// </summary>
        /// <returns></returns>
        public LinkedList<Unit> getSelectedUnits()
        {
            return selected;
        }
    }
}
