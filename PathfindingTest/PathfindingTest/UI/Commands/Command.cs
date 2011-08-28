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
using PathfindingTest.Selection.Patterns;
using PathfindingTest.Pathfinding;

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

                            for (int i = 0; i < this.player.currentSelection.units.Count(); i++)
                            {
                                if (this.player.currentSelection.units.ElementAt(i) is Engineer)
                                {
                                    Engineer e = (Engineer)this.player.currentSelection.units.ElementAt(i);
                                    tempE = e;
                                    break;
                                }
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
                            for (int i = 0; i < this.player.currentSelection.units.Count(); i++)
                            {
                                this.player.currentSelection.units.ElementAt(i).MoveToQueue(new Point((int)x, (int)y));
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
                                    for (int i = 0; i < this.player.currentSelection.units.Count(); i++)
                                    {
                                        Unit u = this.player.currentSelection.units.ElementAt(i);
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
                                    for (int i = 0; i < this.player.currentSelection.units.Count(); i++)
                                    {
                                        this.player.currentSelection.units.ElementAt(i).AttackUnit(unitToAttack);
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
                                    for (int i = 0; i < this.player.currentSelection.units.Count(); i++)
                                    {
                                        this.player.currentSelection.units.ElementAt(i).AttackBuilding(buildingToAttack);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Player currentPlayer = Game1.CURRENT_PLAYER;

                            Point offsettedMouseLocation = currentPlayer.GetAddedOffsettedMouseLocation(me);
                            // Point location = new Point((int)(m.location.X), (int)(m.location.Y));
                            UnitGroupPattern newPattern = currentPlayer.GetNewPreviewPattern(
                                offsettedMouseLocation,
                                offsettedMouseLocation,
                                (int)Util.GetHypoteneuseAngleDegrees(offsettedMouseLocation, offsettedMouseLocation));
                            player.currentSelection.Assault(newPattern);
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

                for( int i = 0; i < player.currentSelection.units.Count(); i++ ){
                    if( player.currentSelection.units.ElementAt(i) is Engineer ) engineerCounter++;
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
