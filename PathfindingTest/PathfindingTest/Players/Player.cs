﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Units;
using PathfindingTest.Buildings;
using PathfindingTest.UI;
using PathfindingTest.Selection;
using PathfindingTest.Selection.Patterns;
using PathfindingTest.Pathfinding;
using XNAInputHandler.MouseInput;
using PathfindingTest.Units.Stores;
using System.Diagnostics;
using PathfindingTest.UI.Commands;
using PathfindingTest.Units.Damage;

namespace PathfindingTest.Players
{
    public class Player : MouseClickListener, MouseMotionListener
    {

        public Color color;
        public Alliance alliance;

        Texture2D selectionTex;
        Texture2D selectedTex;

        public int resources { get; set; }
        public LinkedList<Unit> units { get; set; }
        public UnitSelection currentSelection { get; set; }
        public LinkedList<Building> buildings { get; set; }
        public BuildingSelection buildingSelection { get; set; }
        public ArrowManager arrowManager;

        public Command command { get; set; }

        public HUD hud { get; set; }

        public Point previewPatternClick { get; set; }
        public UnitGroupPattern previewPattern { get; set; }

        public SelectRectangle selectBox { get; set; }

        private int lastBtn1ClickFrames { get; set; }

        public UnitStore meleeStore;
        public UnitStore rangedStore;
        public UnitStore fastStore;

        public Texture2D lightTexture;

        public GraphicsDevice device;

        /// <summary>
        /// And ID used to identify this player in multiplayer games.
        /// </summary>
        public int multiplayerID { get; set; }
        /// <summary>
        /// Used for keeping track if this user is done loading yet.
        /// </summary>
        public Boolean doneLoading { get; set; }

        /// <summary>
        /// Player constructor.
        /// </summary>
        /// <param name="color"></param>
        public Player(Alliance alliance, Color color)
        {
            Game1.GetInstance().players.AddLast(this);
            this.device = Game1.GetInstance().GraphicsDevice;
            this.alliance = alliance;
            if (!this.alliance.members.Contains(this)) this.alliance.members.AddLast(this);
            this.color = color;

            selectionTex = Game1.GetInstance().Content.Load<Texture2D>("Selection");
            selectedTex = Game1.GetInstance().Content.Load<Texture2D>("Selected");

            units = new LinkedList<Unit>();
            buildings = new LinkedList<Building>();
            hud = new HUD(this, color);
            resources = 10000;

            meleeStore = new MeleeStore(this);
            rangedStore = new RangedStore(this);
            fastStore = new FastStore(this);

            arrowManager = new ArrowManager();

            lightTexture = Game1.GetInstance().Content.Load<Texture2D>("Fog/Light");

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;
            MouseManager.GetInstance().mouseMotionListeners += ((MouseMotionListener)this).OnMouseMotion;
            MouseManager.GetInstance().mouseDragListeners += ((MouseMotionListener)this).OnMouseDrag;
        }

        /// <summary>
        /// Spawns the starting units of this player.
        /// </summary>
        /// <param name="location">The location to spawn them at.</param>
        public void SpawnStartUnits(Point location)
        {
            if (Game1.GetInstance().IsMultiplayerGame() &&
                Game1.CURRENT_PLAYER != this) return;

            if (!Game1.GetInstance().IsMultiplayerGame())
            {
                int unitCount = 25;


                LinkedList<Unit> temp_units = new LinkedList<Unit>();
                // +1 to compensate for the engineer
                for (int i = 0; i < unitCount + 1; i++)
                {
                    // Fill the list with dummy units
                    Unit u = null;
                    temp_units.AddLast(u);
                }

                UnitSelection selection = new UnitSelection(temp_units);
                UnitGroupPattern pattern = new CirclePattern(location, selection, 90, 0);
                LinkedList<Point> points = pattern.ApplyPattern();

                for (int i = 0; i < unitCount; i++)
                {

                    Point p = points.ElementAt(i);
                    if (i % 2 == 0)
                    {
                        //temp_units.AddLast(fastStore.getUnit(Unit.Type.Fast, p.X, p.Y));
                    }
                    else
                    {
                        temp_units.AddLast(rangedStore.getUnit(Unit.Type.Ranged, p.X, p.Y));
                    }

                    // Point p = points.ElementAt(i);
                    //temp_units.AddLast(meleeStore.getUnit(Unit.Type.Melee, p.X, p.Y));
                }
            }

            meleeStore.getUnit(Unit.Type.Engineer, location.X, location.Y);

        }

        public UnitSelection GetSelectedUnits()
        {
            UnitSelection selection = new UnitSelection();

            foreach (Unit unit in this.units)
            {
                Console.WriteLine("Unit added");
                if (unit.selected) selection.units.AddLast(unit);
            }

            return selection;
        }

        public BuildingSelection GetSelectedBuildings()
        {
            BuildingSelection selection = new BuildingSelection();

            foreach (Building b in this.buildings)
            {
                if (b.selected)
                {
                    selection.buildings.AddLast(b);
                }
            }

            return selection;
        }

        public void DeselectAllUnits()
        {
            foreach (Unit unit in this.units)
            {
                unit.selected = false;
            }
        }

        public void DeselectAllBuildings()
        {
            foreach (Building b in this.buildings)
            {
                b.selected = false;
            }
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public void Update(KeyboardState ks, MouseState ms)
        {
            try
            {
                for (int i = 0; i < units.Count; i++)
                {
                    units.ElementAt(i).Update(ks, ms);
                }
            }
            catch (InvalidOperationException e) { }

            try
            {
                for (int i = 0; i < buildings.Count; i++)
                {
                    buildings.ElementAt(i).Update(ks, ms);
                }
            }
            catch (Exception e) { }

            try
            {
                arrowManager.UpdateProjectiles(ks, ms);
            }
            catch (InvalidOperationException e) { }


            if (command != null)
            {
                command.Update(ks, ms);
            }

            // Show healthbar over units that mouse is hovering over
            /*Boolean selectedAUnit = false;
            if (this != Game1.CURRENT_PLAYER) return;
            try
            {
                foreach (Player p in Game1.GetInstance().players)
                {
                    for (int i = 0; i < p.units.Count; i++)
                    {
                        Unit u = p.units.ElementAt(i);
                        // Sometimes the unit would still be constructed, and it's updated already .. :c
                        // Debug only
                        if (u == null || u.texture == null) continue;
                        if (!selectedAUnit && u.GetDrawRectangle().Contains(ms.X, ms.Y))
                        {
                            u.selected = true;
                            selectedAUnit = true;
                        }
                        else if (this.currentSelection == null || !this.currentSelection.units.Contains(u))
                        {
                            u.selected = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }*/

            hud.Update(ks, ms);
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal void Draw(SpriteBatch sb)
        {
            if (selectBox != null) selectBox.Draw(sb);

            if (previewPattern != null)
            {
                LinkedList<Point> points = previewPattern.ApplyPattern();
                foreach (Point p in points)
                {
                    new PatternPreviewObject(p).Draw(sb);
                }
            }


            try
            {
                for (int i = 0; i < this.units.Count; i++)
                {
                    Unit unit = units.ElementAt(i);

                    unit.Draw(sb);
                }

            }
            catch (Exception e) { }

            arrowManager.DrawProjectiles(sb);

            // Healthbars
            if (currentSelection != null)
            {
                try
                {
                    for (int i = 0; i < currentSelection.units.Count; i++)
                    {
                        currentSelection.units.ElementAt(i).DrawHealthBar(sb);
                    }
                }
                catch (Exception e) { }
            }

            Unit mouseOver = GetMouseOverUnit();
            if (mouseOver != null) mouseOver.DrawHealthBar(sb);

            foreach (Building b in buildings)
            {
                b.Draw(sb);
            }
        }

        public void DrawHud(SpriteBatch sb)
        {
            if (command != null)
            {
                command.Draw(sb);
            }

            if (this == Game1.CURRENT_PLAYER)
            {
                hud.Draw(sb);
            }
        }

        public void DrawLights(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Unit unit in this.units)
            {
                spriteBatch.Draw(
                    lightTexture,
                    new Vector2(unit.x + (unit.texture.Width / 2) - Game1.GetInstance().drawOffset.X,
                                unit.y + (unit.texture.Height / 2) - Game1.GetInstance().drawOffset.Y),
                    null,
                    Color.White,
                    0f,
                    new Vector2(16, 16),
                    (unit.visionRange / lightTexture.Width * 4),
                    SpriteEffects.None,
                    1.0f);
            }

            foreach (Building building in this.buildings)
            {
                if (building.state != Building.State.Preview)
                {
                    spriteBatch.Draw(
                        lightTexture,
                        new Vector2(building.x + (building.texture.Width / 2) - Game1.GetInstance().drawOffset.X,
                                    building.y + (building.texture.Height / 2) - Game1.GetInstance().drawOffset.Y),
                        null,
                        Color.White,
                        0f,
                        new Vector2(16, 16),
                        (building.visionRange / lightTexture.Width * 4),
                        SpriteEffects.None,
                        1.0f);
                }
            }
        }


        /// <summary>
        /// Whether the player is currently previewing a building
        /// </summary>
        /// <returns>Yes or no.</returns>
        public Boolean IsPreviewingBuilding()
        {
            foreach (Building b in buildings)
            {
                if (b.state == Building.State.Preview) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the unit that the mouse is over, of all players.
        /// </summary>
        /// <returns>The unit.</returns>
        public Unit GetMouseOverUnit()
        {
            foreach (Player p in Game1.GetInstance().players)
            {
                for (int i = 0; i < p.units.Count; i++)
                {
                    Unit u = p.units.ElementAt(i);

                    if (u.GetDrawRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        return u;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// Whether the mouse is over a unit or not
        /// if it is, it'll return it. =D
        /// </summary>
        /// <returns>The unit, or null if there was no unit!</returns>
        public Unit GetMouseOverUnit(LinkedList<Unit> units)
        {
            foreach (Unit u in units)
            {
                if (u.GetDrawRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y)) return u;
            }
            return null;
        }

        /// <summary>
        /// Whether the mouse is over a unit or not
        /// if it is, it'll return it. =D
        /// </summary>
        /// <returns>The unit, or null if there was no unit!</returns>
        public Building GetMouseOverBuilding(LinkedList<Building> buildings)
        {
            foreach (Building b in buildings)
            {
                if (b.DefineDrawRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y)) return b;
            }
            return null;
        }

        public Building IsMouseOverFriendlyBuilding()
        {
            foreach (Building b in buildings)
            {
                if (b.DefineDrawRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    return b;
                }
            }

            return null;
        }


        public UnitSelection GetUnits()
        {
            UnitSelection selection = new UnitSelection(new LinkedList<Unit>());
            foreach (Unit unit in units)
            {
                if (this.selectBox.GetRectangle().Intersects(unit.GetDrawRectangle()))
                {
                    selection.units.AddLast(unit);
                }
            }
            return selection;
        }

        public BuildingSelection GetBuildings()
        {
            BuildingSelection selection = new BuildingSelection(new LinkedList<Building>());

            foreach (Building b in this.buildings)
            {
                if (this.selectBox.GetRectangle().Contains((int)b.x, (int)b.y))
                {
                    selection.buildings.AddLast(b);
                }
            }

            return selection;
        }

        /// <summary>
        /// Removes all the preview buildings
        /// </summary>
        public void RemovePreviewBuildings()
        {
            for (int i = 0; i < this.buildings.Count; i++)
            {
                Building build = this.buildings.ElementAt(i);
                if (build.state == Building.State.Preview)
                {
                    build.Dispose();
                    i--;
                }
            }

            Game1.GetInstance().IsMouseVisible = true;
        }

        public void OnMouseClick(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this)
            {
                return;
            }

            if ((m.button == MouseEvent.MOUSE_BUTTON_3))
            {
                if (IsPreviewingBuilding())
                {
                    this.RemovePreviewBuildings();
                }
                else
                {
                    previewPatternClick = GetAddedOffsettedMouseLocation(m);
                }
            }

            if (!hud.DefineRectangle().Contains(m.location) &&
                !hud.DefineMiniMapRectangle().Contains(m.location))
            {
                if (m.button == MouseEvent.MOUSE_BUTTON_1)
                {
                    Unit mouseOverUnit = this.GetMouseOverUnit(this.units);
                    if (mouseOverUnit == null)
                    {
                        if (this.currentSelection != null && this.currentSelection.units.Count != 0 &&
                            !this.hud.IsMouseOverBuilding() && !this.IsPreviewingBuilding())
                        {
                            this.DeselectAllUnits();
                            this.currentSelection = null;
                        }
                    }
                    else
                    {
                        this.DeselectAllUnits();
                        this.DeselectAllBuildings();
                        // Performed a double click!
                        if (Game1.GetInstance().frames - lastBtn1ClickFrames < 20)
                        {
                            LinkedList<Unit> selectionUnits = new LinkedList<Unit>();
                            foreach (Unit unit in units)
                            {
                                if (mouseOverUnit.type == unit.type) selectionUnits.AddLast(unit);
                            }
                            this.currentSelection = new UnitSelection(selectionUnits);
                        }
                        else if (!mouseOverUnit.selected)
                        {
                            LinkedList<Unit> selectionUnits = new LinkedList<Unit>();
                            selectionUnits.AddLast(mouseOverUnit);
                            this.currentSelection = new UnitSelection(selectionUnits);
                        }
                    }

                    Building mouseOverBuilding = this.IsMouseOverFriendlyBuilding();
                    if (mouseOverBuilding == null)
                    {
                        if (this.buildingSelection != null && this.buildingSelection.buildings.Count != 0 &&
                            !this.IsPreviewingBuilding())
                        {
                            this.DeselectAllBuildings();
                            this.buildingSelection = null;
                        }
                    }
                    else
                    {
                        this.DeselectAllUnits();
                        this.DeselectAllBuildings();
                        // Performed a double click!
                        if (Game1.GetInstance().frames - lastBtn1ClickFrames < 20)
                        {
                            LinkedList<Building> selectionBuildings = new LinkedList<Building>();
                            foreach (Building b in buildings)
                            {
                                if (mouseOverBuilding.type == b.type)
                                {
                                    selectionBuildings.AddLast(b);
                                }
                            }
                            this.buildingSelection = new BuildingSelection(selectionBuildings);
                        }
                        else if (!mouseOverBuilding.selected)
                        {
                            LinkedList<Building> selectionBuildings = new LinkedList<Building>();
                            selectionBuildings.AddLast(mouseOverBuilding);
                            this.buildingSelection = new BuildingSelection(selectionBuildings);
                            this.buildingSelection.SelectAll();
                        }
                    }
                }
            }

            if (m.button == MouseEvent.MOUSE_BUTTON_1)
            {
                lastBtn1ClickFrames = Game1.GetInstance().frames;
            }

        }

        public void OnMouseRelease(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            if (selectBox != null && !IsPreviewingBuilding())
            {
                this.currentSelection = this.GetUnits();
                this.currentSelection.SelectAll();

                if (currentSelection.units.Count == 0)
                {
                    this.buildingSelection = this.GetBuildings();
                    this.buildingSelection.SelectAll();
                }
            }
            selectBox = null;
            ///unitz
            if (this.currentSelection != null && this.currentSelection.units.Count != 0
                && (m.button == MouseEvent.MOUSE_BUTTON_3))
            {
                foreach (Player player in Game1.GetInstance().players)
                {
                    if (player.alliance.members.Contains(this))
                    {
                        Unit selectedFriendly = GetMouseOverUnit(player.units);
                        if (selectedFriendly != null)
                        {
                            foreach (Unit unit in currentSelection.units)
                            {
                                unit.Defend(selectedFriendly);
                                selectedFriendly.friendliesProtectingMe.AddLast(unit);
                            }
                        }
                    }
                    else
                    {
                        Unit selectedEnemy = GetMouseOverUnit(player.units);
                        if (selectedEnemy != null && (this.command == null || (this.command != null && this.command.type != Command.Type.Defend)))
                        {
                            foreach (Unit unit in currentSelection.units)
                            {
                                unit.AttackUnit(selectedEnemy);
                            }
                            return;
                        }
                        else
                        {
                            Building enemyBuilding = GetMouseOverBuilding(player.buildings);
                            if (enemyBuilding != null)
                            {
                                foreach (Unit unit in currentSelection.units)
                                {
                                    unit.AttackBuilding(enemyBuilding);
                                }
                                return;
                            }
                            if (previewPattern != null)
                            {
                                stopUnitSelection();
                                this.currentSelection.MoveTo(previewPattern);
                            }
                            // If we're suppose to move in the first place
                            else if (this.command == null || (this.command != null && this.command.type != Command.Type.Repair && this.command.type != Command.Type.Defend && this.command.type != Command.Type.Attack))
                            {
                                stopUnitSelection();
                                Point offsettedMouseLocation = GetAddedOffsettedMouseLocation(m);
                                this.currentSelection.MoveTo(GetNewPreviewPattern(previewPatternClick, offsettedMouseLocation, 0));
                            }
                        }
                        previewPattern = null;

                    }
                }
            }

            if (this.buildingSelection != null && this.buildingSelection.buildings.Count != 0
                && m.button == MouseEvent.MOUSE_BUTTON_3)
            {
                foreach (Building b in buildingSelection.buildings)
                {
                    if (b.type != Building.Type.Resources && b.type != Building.Type.Sentry)
                    {
                        b.waypoint = m.location;
                    }
                }
            }
        }

        public void stopUnitSelection()
        {
            foreach (Unit unit in currentSelection.units)
            {
                unit.unitToDefend = null;
                unit.unitToStalk = null;
                unit.waypoints.Clear();
            }
        }

        public void OnMouseMotion(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            //throw new NotImplementedException();
        }

        public void OnMouseDrag(MouseEvent m)
        {
            // Bots dont use the mouse, or shouldn't
            if (Game1.CURRENT_PLAYER != this) return;
            this.previewPattern = null;
            if (m.button == MouseEvent.MOUSE_BUTTON_1 &&
                !hud.DefineRectangle().Contains(m.location) &&
                !hud.DefineMiniMapRectangle().Contains(m.location))
            {
                if (selectBox == null)
                {
                    this.selectBox = new SelectRectangle();
                    this.selectBox.clickedPoint = m.location;
                }
                else
                {
                    int x = selectBox.clickedPoint.X;
                    int y = selectBox.clickedPoint.Y;
                    this.selectBox.SetRectangle(new Rectangle(x, y, m.location.X - x, m.location.Y - y));
                }
            }
            else if (m.button == MouseEvent.MOUSE_BUTTON_3)
            {
                if (this.currentSelection != null)
                {
                    /*previewPattern = new CirclePattern(previewPatternClick,
                        currentSelection,
                        Math.Max(currentSelection.units.Count * 5, (int)Util.GetHypoteneuseLength(e.location, previewPatternClick)),
                        angle);*/
                    Point offsettedMouseLocation = GetAddedOffsettedMouseLocation(m);
                    // Point location = new Point((int)(m.location.X), (int)(m.location.Y));
                    this.previewPattern = GetNewPreviewPattern(
                        previewPatternClick,
                        offsettedMouseLocation,
                        (int)Util.GetHypoteneuseAngleDegrees(offsettedMouseLocation, previewPatternClick));
                    /*previewPattern = new RectanglePattern(previewPatternClick,
                        currentSelection, 5,
                        Math.Max((int)(Util.GetHypoteneuseLength(m.location, previewPatternClick) / 2.0), 30),
                        angle);*/
                }
            }
        }

        /// <summary>
        /// Gets the mouse location with the offset added.
        /// </summary>
        /// <param name="m">The mouse event.</param>
        /// <returns>The point in map coordinates</returns>
        public Point GetAddedOffsettedMouseLocation(MouseEvent m)
        {
            Vector2 offset = Game1.GetInstance().drawOffset;
            return new Point((int)(m.location.X + offset.X), (int)(m.location.Y + offset.Y));
        }

        /// <summary>
        /// Gets the mouse location with the offset substracted.
        /// </summary>
        /// <param name="m">The mouse event.</param>
        /// <returns>The mouse location with the offset substracted</returns>
        public Point GetSubstractedOffsettedMouseLocation(MouseEvent m)
        {
            Vector2 offset = Game1.GetInstance().drawOffset;
            return new Point((int)(m.location.X - offset.X), (int)(m.location.Y - offset.Y));
        }

        /// <summary>
        /// Gets the new current preview pattern (TEMPORARY FUNCTION)
        /// </summary>
        /// <param name="offsettedMouseLocation">The location at which the mouse is at, with offset substracted</param>
        /// <param name="angle">The angle</param>
        /// <returns>A pattern</returns>
        public UnitGroupPattern GetNewPreviewPattern(Point patternCenterLocation, Point offsettedMouseLocation, int angle)
        {
            Vector2 offset = Game1.GetInstance().drawOffset;

            return new CirclePattern(patternCenterLocation,
                        currentSelection,
                        Math.Max((int)(Util.GetHypoteneuseLength(offsettedMouseLocation, patternCenterLocation) / 2.0), 30),
                        angle);
            /*
            return new RectanglePattern(offsetClickLocation,
                        currentSelection, (int)Math.Ceiling(Math.Sqrt(currentSelection.units.Count)),
                        Math.Max((int)(Util.GetHypoteneuseLength(patternCenterLocation, offsetClickLocation) / 2.0), 30),
                        angle);*/
        }

        public void DrawLights(GameTime gameTime, SpriteBatch spriteBatch, Texture2D lightTexture)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            foreach (Unit unit in this.units)
            {

                spriteBatch.Draw(
                    lightTexture,
                    new Vector2(unit.x, unit.y),
                    null,
                    Color.White,
                    0f,
                    new Vector2(16, 16),
                    Vector2.One * 15,
                    SpriteEffects.None,
                    1.0f);
            }

            spriteBatch.End();
        }
    }
}
