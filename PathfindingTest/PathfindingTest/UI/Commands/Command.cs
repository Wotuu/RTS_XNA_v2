using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNAInputHandler.MouseInput;
using PathfindingTest.Players;
using PathfindingTest.Units;
using System.Diagnostics;
using PathfindingTest.Buildings;

namespace PathfindingTest.UI.Commands
{

    public class Command : MouseClickListener
    {

        public Texture2D texture { get; set; }
        public Player player { get; set; }
        public Type type { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public Color color { get; set; }

        public enum Type
        {
            Move,
            Attack,
            Defend,
            Stop,
            Repair
        }

        public Command(Texture2D texture, Player player, Type type, float x, float y, Color color)
        {
            this.texture = texture;
            this.player = player;
            this.type = type;
            this.x = x;
            this.y = y;
            this.color = color;

            // Was false
            Game1.GetInstance().IsMouseVisible = true;

            //MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
            this.x = ms.X - (texture.Width / 2);
            this.y = ms.Y - (texture.Height / 2);
        }

        internal void Draw(SpriteBatch sb)
        {
            sb.Draw(this.texture, new Vector2(x, y), this.color);
        }

        public void Dispose()
        {
            Game1.GetInstance().IsMouseVisible = true;
            //MouseManager.GetInstance().mouseClickedListeners -= ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners -= ((MouseClickListener)this).OnMouseRelease;
            player.command = null;
        }

        void MouseClickListener.OnMouseClick(MouseEvent me)
        {
            // Not Used
        }

        void MouseClickListener.OnMouseRelease(MouseEvent me)
        {
            if (me.button == MouseEvent.MOUSE_BUTTON_3)
            {
                switch (this.type)
                {
                    case Type.Repair:
                        if (CheckEngineers() && player.IsMouseOverFriendlyBuilding() != null)
                        {
                            Engineer tempE = null;

                            foreach (Engineer e in player.currentSelection.units)
                            {
                                tempE = e;
                                break;
                            }

                            if (tempE != null)
                            {
                                tempE.Repair(player.IsMouseOverFriendlyBuilding());
                            }
                        }
                        this.Dispose();
                        break;

                    case Type.Move:
                        if (player.currentSelection != null)
                        {
                            foreach (Unit u in player.currentSelection.units)
                            {
                                u.MoveToQueue(new Point((int)x, (int)y));
                            }
                        }
                        this.Dispose();
                        break;

                    case Type.Defend:
                        if (player.GetMouseOverUnit(player.units) != null)
                        {
                            if (player.currentSelection != null)
                            {
                                if (player.GetMouseOverUnit(player.units).player == this.player)
                                {
                                    foreach (Unit u in player.currentSelection.units)
                                    {
                                        u.Defend(player.GetMouseOverUnit(player.units));
                                        player.GetMouseOverUnit(player.units).friendliesProtectingMe.AddLast(u);
                                        u.MoveToQueue(new Point((int)x, (int)y));
                                    }
                                }
                            }
                        }
                        this.Dispose();
                        break;

                    case Type.Attack:
                        if (player.GetMouseOverUnit(player.units) != null)
                        {
                            Unit unitToAttack = player.GetMouseOverUnit(player.units);

                            if (unitToAttack.player != this.player)
                            {
                                if (player.currentSelection != null)
                                {
                                    foreach (Unit u in player.currentSelection.units)
                                    {
                                        u.AttackUnit(unitToAttack);
                                    }
                                }
                            }
                        }
                        else if (player.GetMouseOverBuilding(player.buildings) != null)
                        {
                            Building buildingToAttack = player.GetMouseOverBuilding(player.buildings);

                            if (buildingToAttack.p != this.player)
                            {
                                if (player.buildingSelection != null)
                                {
                                    foreach (Unit u in player.currentSelection.units)
                                    {
                                        u.AttackBuilding(buildingToAttack);
                                    }
                                }
                            }
                        }
                        this.Dispose();
                        break;

                    default:
                        this.Dispose();
                        break;
                }
            }
        }

        private Boolean CheckEngineers()
        {
            if (player.currentSelection != null)
            {
                int engineerCounter = 0;

                foreach (Engineer e in player.currentSelection.units)
                {
                    engineerCounter++;
                }

                if (engineerCounter < 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
