using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using PathfindingTest;
using System.Diagnostics;
using RTS_Builder.Buildings;
using RTS_Builder.HUD;

namespace RTS_Builder
{
    public class HUD : MouseClickListener
    {
        Player p;
        Color c;

        Texture2D hudTex;
        Texture2D hudResourcesTex;
        Texture2D hudBarracksTex;
        Texture2D hudFactoryTex;
        Texture2D hudFortressTex;

        Texture2D hudItemDetails;

        SpriteFont sf;

        Boolean loadForEngineer = false;
        public Boolean draw { get; set; }

        Boolean drawResourcesText = false;
        Boolean drawBarracksText = false;
        Boolean drawFactoryText = false;
        Boolean drawFortressText = false;

        public static HUD instance;

        /// <summary>
        /// HUD Constructor.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="cm"></param>
        /// <param name="c"></param>
        public HUD(Player p, Color c)
        {
            this.p = p;
            this.c = c;

            hudTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUD");
            hudResourcesTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDResources");
            hudBarracksTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDBarracks");
            hudFactoryTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFactory");
            hudFortressTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDFortress");

            hudItemDetails = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDItemDetails");

            sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/SpriteFont1");

            draw = false;

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;

            instance = this;
        }

        public static HUD GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public void Update(KeyboardState ks, MouseState ms)
        {
            CheckDraw();
            CountUnits();

            Rectangle mr = new Rectangle(ms.X, ms.Y, 1, 1);
            drawResourcesText = this.DefineResourcesRectangle().Contains(mr);
            drawBarracksText = this.DefineBarracksRectangle().Contains(mr);
            drawFactoryText = this.DefineFactoryRectangle().Contains(mr);
            drawFortressText = this.DefineFortressRectangle().Contains(mr);
        }

        /// <summary>
        /// Checks if the HUD should be drawn.
        /// If an Engineer is selected, load contents for Engineer.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {
            if (draw)
            {
                sb.Draw(hudTex, new Rectangle(0, 652, 1024, 116), c);
            }

            if (loadForEngineer)
            {
                sb.Draw(hudResourcesTex, new Rectangle(278, 688, 28, 28), c);
                sb.Draw(hudBarracksTex, new Rectangle(316, 688, 28, 28), c);
                sb.Draw(hudFactoryTex, new Rectangle(354, 688, 28, 28), c);
                sb.Draw(hudFortressTex, new Rectangle(392, 688, 28, 28), c);
            }

            if (drawResourcesText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, c);
                sb.DrawString(sf, "Resources", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }

            if (drawBarracksText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, c);
                sb.DrawString(sf, "Barracks", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }

            if (drawFactoryText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, c);
                sb.DrawString(sf, "Factory", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }

            if (drawFortressText)
            {
                Rectangle rect = DefineDetailsRectangle();
                sb.Draw(hudItemDetails, rect, c);
                sb.DrawString(sf, "Fortress", new Vector2(rect.X + 5, rect.Y + 5), Color.White, 0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0f);
            }
        }

        void MouseClickListener.OnMouseClick(MouseEvent me)
        {
            if (me.button == MouseEvent.MOUSE_BUTTON_1)
            {
                if (drawResourcesText || drawBarracksText || drawFactoryText || drawFortressText)
                {
                    Building b;

                    if (drawResourcesText)
                    {
                        b = new ResourceGather(this.p, this.c);
                    }
                    else if (drawBarracksText)
                    {
                        b = new Barracks(this.p, this.c);
                    }
                    else //if (drawFactoryText)
                    {
                        b = new Factory(this.p, this.c);
                    }
                    //else if (drawFortressText)
                    //{
                    //    b = new Fortress(this.p, this.c);
                    //}

                    foreach (Building build in p.buildings)
                    {
                        if (build.state == Building.BuildState.Preview)
                        {
                            build.Dispose();
                            break;
                        }
                    }

                    p.buildings.AddLast(b);
                }

                foreach (Building b in p.buildings)
                {
                    if (b.state == Building.BuildState.Preview && !this.DefineRectangle().Contains(new Rectangle(me.location.X, me.location.Y, 1, 1)))
                    {
                        foreach (Engineer e in p.getSelectedUnits())
                        {
                            e.setWaypoint(me.location.X, me.location.Y);
                        }

                        b.state = Building.BuildState.Constructing;
                    }
                }
            }
        }

        void MouseClickListener.OnMouseRelease(MouseEvent me)
        {
        }

        /// <summary>
        /// Count the different unit types selected.
        /// Determines how the HUD should be loaded.
        /// </summary>
        public void CountUnits()
        {
            int engineerCounter = 0;

            foreach (Unit u in p.getSelectedUnits())
            {
                if (u.type == Unit.UnitType.Engineer)
                {
                    engineerCounter++;
                }
            }

            if (engineerCounter > 0)
            {
                loadForEngineer = true;
            }
            else
            {
                loadForEngineer = false;
            }
        }

        public void CheckDraw()
        {
            if (p.getSelectedUnits() != null && p.getSelectedUnits().Count > 0)
            {
                draw = true;
            }
            else
            {
                draw = false;
            }
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle(195, 652, 634, 116);
        }

        public Rectangle DefineDetailsRectangle()
        {
            return new Rectangle(0, 652, 195, 116);
        }

        public Rectangle DefineResourcesRectangle()
        {
            return new Rectangle(278, 688, 28, 28);
        }

        public Rectangle DefineBarracksRectangle()
        {
            return new Rectangle(316, 688, 28, 28);
        }

        public Rectangle DefineFactoryRectangle()
        {
            return new Rectangle(354, 688, 28, 28);
        }

        public Rectangle DefineFortressRectangle()
        {
            return new Rectangle(392, 688, 28, 28);
        }
    }
}