﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PathfindingTest.Buildings;
using PathfindingTest.Players;
using PathfindingTest.Units;
using PathfindingTest.Pathfinding;
using XNAInputHandler.MouseInput;
using PathfindingTest.UI.Commands;
using AStarCollisionMap.Pathfinding;
using CustomLists.Lists;

namespace PathfindingTest.UI
{
    public class HUD : MouseClickListener
    {

        public Player player;
        public Color color;

        public Texture2D hudTex;
        public Texture2D miniMapTex;
        public SpriteFont sf;

        public Boolean loadForEngineer { get; set; }
        public Boolean loadForUnit { get; set; }
        public Boolean loadForResources { get; set; }
        public Boolean loadForBarracks { get; set; }
        public Boolean loadForFactory { get; set; }
        public Boolean loadForFortress { get; set; }
        public Boolean loadForSentry { get; set; }
        public Boolean draw { get; set; }

        public CustomArrayList<HUDObject> objects { get; set; }
        public CustomArrayList<HUDCommandObject> commandObjects { get; set; }

        private float startObjectX = 278;
        private float startObjectY = 688;
        private float startCommandX = 673;
        private float startCommandY = 688;

        public HUDCommandObject moveCommand { get; set; }
        public HUDCommandObject attackCommand { get; set; }
        public HUDCommandObject defendCommand { get; set; }
        public HUDCommandObject stopCommand { get; set; }
        public HUDCommandObject repairCommand { get; set; }

        public HUDObject resourceObject { get; set; }
        public HUDObject barracksObject { get; set; }
        public HUDObject factoryObject { get; set; }
        public HUDObject fortressObject { get; set; }
        public HUDObject sentryObject { get; set; }

        public HUDObject engineerObject { get; set; }
        public HUDObject meleeObject { get; set; }
        public HUDObject rangedObject { get; set; }
        public HUDObject fastObject { get; set; }
        public HUDObject heavyMeleeObject { get; set; }
        public HUDObject heavyRangedObject { get; set; }

        /// <summary>
        /// Sets the textures to use for the HUD.
        /// Creates new instances of needed components.
        /// Sets variables to their default values.
        /// </summary>
        /// <param name="p">The player this HUD belongs to</param>
        /// <param name="c">The desired color for this HUD</param>
        public HUD(Player p, Color c)
        {
            this.player = p;
            this.color = c;

            hudTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUD");
            miniMapTex = Game1.GetInstance().Content.Load<Texture2D>("HUD/HUDMiniMap");
            sf = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/SpriteFont1");

            loadForEngineer = false;
            loadForUnit = false;
            loadForResources = false;
            loadForSentry = false;
            loadForBarracks = false;
            loadForFactory = false;
            loadForFortress = false;
            draw = false;

            objects = new CustomArrayList<HUDObject>();

            LoadCommands();

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)this).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)this).OnMouseRelease;
        }

        private void LoadCommands()
        {
            commandObjects = new CustomArrayList<HUDCommandObject>();
            startCommandX = 673;
            startCommandY = 688;

            //No more than 8 seperate Commands!!!
            moveCommand = new HUDCommandObject(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Move), HUDCommandObject.Type.Move, startCommandX, startCommandY, new Color(0, 100, 255, 255), this.color);
            commandObjects.AddLast(moveCommand);
            IncrementStartCommandXY(startCommandX);

            attackCommand = new HUDCommandObject(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Attack), HUDCommandObject.Type.Attack, startCommandX, startCommandY, new Color(255, 0, 12, 255), this.color);
            commandObjects.AddLast(attackCommand);
            IncrementStartCommandXY(startCommandX);

            defendCommand = new HUDCommandObject(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Defend), HUDCommandObject.Type.Defend, startCommandX, startCommandY, new Color(255, 125, 0, 255), this.color);
            commandObjects.AddLast(defendCommand);
            IncrementStartCommandXY(startCommandX);

            stopCommand = new HUDCommandObject(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Stop), HUDCommandObject.Type.Stop, startCommandX, startCommandY, new Color(255, 0, 0, 255), this.color);
            commandObjects.AddLast(stopCommand);
            IncrementStartCommandXY(startCommandX);

            repairCommand = new HUDCommandObject(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Repair), HUDCommandObject.Type.Repair, startCommandX, startCommandY, new Color(255, 187, 0, 255), this.color);
            commandObjects.AddLast(repairCommand);
            IncrementStartCommandXY(startCommandX);
        }

        /// <summary>
        /// Checks whether the HUD should be hidden or not. (if the player this HUD belongs to is not the player that is watching)
        /// Loads the objects to display according to selected units/buildings.
        /// </summary>
        /// <param name="ks">Default KeyboardState</param>
        /// <param name="ms">Default MouseState</param>
        public void Update(KeyboardState ks, MouseState ms)
        {
            if (!draw)
            {
                draw = true;
            }
            CountUnits();

            objects = new CustomArrayList<HUDObject>();

            for (int i = 0; i < this.commandObjects.Count(); i++)
            {
                this.commandObjects.ElementAt(i).disabled = true;
            }

            if (loadForEngineer)
            {
                resourceObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Resources), HUDObject.Type.Resources, startObjectX, startObjectY, color);
                objects.AddLast(resourceObject);
                IncrementStartObjectXY(startObjectX);

                barracksObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Barracks), HUDObject.Type.Barracks, startObjectX, startObjectY, color);
                objects.AddLast(barracksObject);
                IncrementStartObjectXY(startObjectX);

                factoryObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Factory), HUDObject.Type.Factory, startObjectX, startObjectY, color);
                objects.AddLast(factoryObject);
                IncrementStartObjectXY(startObjectX);

                sentryObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Sentry), HUDObject.Type.Sentry, startObjectX, startObjectY, color);
                objects.AddLast(sentryObject);
                IncrementStartObjectXY(startObjectX);

                fortressObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Fortress), HUDObject.Type.Fortress, startObjectX, startObjectY, color);
                objects.AddLast(fortressObject);
                IncrementStartObjectXY(startObjectX);

                for (int i = 0; i < this.commandObjects.Count(); i++)
                {
                    HUDCommandObject co = this.commandObjects.ElementAt(i);
                    if (co.type == HUDCommandObject.Type.Repair || co.type == HUDCommandObject.Type.Move || co.type == HUDCommandObject.Type.Stop || co.type == HUDCommandObject.Type.Defend)
                    {
                        co.disabled = false;
                    }
                }
            }
            if (loadForUnit)
            {
                for (int i = 0; i < this.commandObjects.Count(); i++)
                {
                    HUDCommandObject co = this.commandObjects.ElementAt(i);
                    if (co.type == HUDCommandObject.Type.Attack || co.type == HUDCommandObject.Type.Defend || co.type == HUDCommandObject.Type.Move || co.type == HUDCommandObject.Type.Stop)
                    {
                        co.disabled = false;
                    }
                }
            }
            if (loadForBarracks)
            {
                meleeObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Melee), HUDObject.Type.Melee, startObjectX, startObjectY, color);
                objects.AddLast(meleeObject);
                IncrementStartObjectXY(startObjectX);

                rangedObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Ranged), HUDObject.Type.Ranged, startObjectX, startObjectY, color);
                objects.AddLast(rangedObject);
                IncrementStartObjectXY(startObjectX);

                fastObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Fast), HUDObject.Type.Fast, startObjectX, startObjectY, color);
                objects.AddLast(fastObject);
                IncrementStartObjectXY(startObjectX);
            }
            if (loadForFortress)
            {
                engineerObject = new HUDObject(TextureManager.GetInstance().GetTexture(HUDObject.Type.Engineer), HUDObject.Type.Engineer, startObjectX, startObjectY, color);
                objects.AddLast(engineerObject);
                IncrementStartObjectXY(startObjectX);
            }

            startObjectX = 278;
            startObjectY = 688;
        }

        public void IncrementStartObjectXY(float startX)
        {
            if (startX == 658)
            {
                startObjectY += 38;
            }

            if (startX == 658)
            {
                startObjectX = 278;
            }
            else
            {
                startObjectX += 38;
            }
        }

        public void IncrementStartCommandXY(float startX)
        {
            if (startX == 787)
            {
                startCommandY += 38;
            }

            if (startX == 787)
            {
                startCommandX = 673;
            }
            else
            {
                startCommandX += 38;
            }
        }

        /// <summary>
        /// Draws the HUD and the objects required.
        /// </summary>
        /// <param name="sb">Default SpriteBatch</param>
        internal void Draw(SpriteBatch sb)
        {

            // Set this, can't do it in the constructor
            Game1.GetInstance().map.miniMap.rectangleColor = this.color;
            Game1.GetInstance().map.miniMap.z = 0.09998f;

            // Draw mini map

            Game1.GetInstance().map.miniMap.Draw(sb, new Rectangle(834, 574, 185, 189));
            //sb.Draw(miniMapTex, this.DefineMiniMapRectangle(), null, Color.Transparent, 0f, Vector2.Zero, SpriteEffects.None, 0.09999f);
            DrawUtil.DrawClearRectangle(sb, this.DefineMiniMapRectangle(), 1, color, 0.09999f);

            if (draw)
            {
                sb.Draw(hudTex, new Rectangle(0, 652, 1024, 116), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                String resS = player.resources.ToString();
                sb.DrawString(sf, resS, new Vector2(198, 654), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0999f);
            }
            else return;

            if (player.buildingSelection != null)
            {
                for (int i = 0; i < player.buildingSelection.buildings.Count(); i++)
                {
                    Building b = player.buildingSelection.buildings.ElementAt(i);
                    if (b.state != Building.State.Preview)
                    {
                        b.DrawQueuedStats(sb);
                    }
                }
            }

            for (int i = 0; i < this.objects.Count(); i++)
            {
                this.objects.ElementAt(i).Draw(sb);
            }

            for (int i = 0; i < this.commandObjects.Count(); i++)
            {
                this.commandObjects.ElementAt(i).Draw(sb);
            }
        }

        /// <summary>
        /// Used for creating units and buildings respectively.
        /// </summary>
        /// <param name="me">The MouseEvent to use</param>
        void MouseClickListener.OnMouseClick(MouseEvent me)
        {
            MouseState state = Mouse.GetState();
            if (me.button == MouseEvent.MOUSE_BUTTON_1)
            {
                for (int i = 0; i < this.objects.Count(); i++)
                {
                    HUDObject o = this.objects.ElementAt(i);
                    if (o.DefineRectangle().Contains(state.X, state.Y))
                    {
                        player.RemovePreviewBuildings();
                        Building b;

                        switch (o.type)
                        {
                            case HUDObject.Type.Resources:
                                if (player.resources >= Building.GetCost(Building.Type.Resources))
                                {
                                    b = new ResourceGather(this.player, this.color);
                                    // Was false
                                    Game1.GetInstance().IsMouseVisible = true;
                                }
                                break;

                            case HUDObject.Type.Barracks:
                                if (player.resources >= Building.GetCost(Building.Type.Barracks))
                                {
                                    b = new Barracks(this.player, this.color);
                                    // Was false
                                    Game1.GetInstance().IsMouseVisible = true;
                                }
                                break;

                            case HUDObject.Type.Factory:
                                if (player.resources >= Building.GetCost(Building.Type.Factory))
                                {
                                    b = new Factory(this.player, this.color);
                                    // Was false
                                    Game1.GetInstance().IsMouseVisible = true;
                                }
                                break;

                            case HUDObject.Type.Sentry:
                                if (player.resources >= Building.GetCost(Building.Type.Sentry))
                                {
                                    b = new Sentry(this.player, this.color);
                                    // Was false
                                    Game1.GetInstance().IsMouseVisible = true;
                                }
                                break;

                            case HUDObject.Type.Fortress:
                                if (player.resources >= Building.GetCost(Building.Type.Fortress))
                                {
                                    b = new Fortress(this.player, this.color);
                                    // Was false
                                    Game1.GetInstance().IsMouseVisible = true;
                                }
                                break;

                            case HUDObject.Type.Engineer:
                                for (int k = 0; k < player.buildingSelection.buildings.Count(); k++)
                                {
                                    Building bu = player.buildingSelection.buildings.ElementAt(k);
                                    if (bu is Fortress)
                                    {
                                        Fortress building = (Fortress)bu;
                                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                                        {
                                            for (int j = 0; j < 5; j++)
                                            {
                                                if (player.resources >= Unit.GetCost(Unit.Type.Engineer))
                                                {
                                                    building.CreateUnit(Unit.Type.Engineer);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (player.resources >= Unit.GetCost(Unit.Type.Engineer))
                                            {
                                                building.CreateUnit(Unit.Type.Engineer);
                                            }
                                        }
                                    }
                                }
                                break;

                            case HUDObject.Type.Ranged:
                                for (int k = 0; k < player.buildingSelection.buildings.Count(); k++)
                                {
                                    Building bu = player.buildingSelection.buildings.ElementAt(k);
                                    if (bu is Barracks)
                                    {
                                        Barracks building = (Barracks)bu;
                                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                                        {
                                            for (int j = 0; j < 5; j++)
                                            {
                                                if (player.resources >= Unit.GetCost(Unit.Type.Ranged))
                                                {
                                                    building.CreateUnit(Unit.Type.Ranged);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (player.resources >= Unit.GetCost(Unit.Type.Ranged))
                                            {
                                                building.CreateUnit(Unit.Type.Ranged);
                                            }
                                        }
                                    }
                                }
                                break;

                            case HUDObject.Type.Melee:
                                for (int k = 0; k < player.buildingSelection.buildings.Count(); k++)
                                {
                                    Building bu = player.buildingSelection.buildings.ElementAt(k);
                                    if (bu is Barracks)
                                    {
                                        Barracks building = (Barracks)bu;
                                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                                        {
                                            for (int j = 0; j < 5; j++)
                                            {
                                                if (player.resources >= Unit.GetCost(Unit.Type.Melee))
                                                {
                                                    building.CreateUnit(Unit.Type.Melee);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (player.resources >= Unit.GetCost(Unit.Type.Melee))
                                            {
                                                building.CreateUnit(Unit.Type.Melee);
                                            }
                                        }
                                    }
                                }
                                break;

                            case HUDObject.Type.Fast:
                                for (int k = 0; k < player.buildingSelection.buildings.Count(); k++)
                                {
                                    Building bu = player.buildingSelection.buildings.ElementAt(k);
                                    if (bu is Barracks)
                                    {
                                        Barracks building = (Barracks)bu;
                                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                                        {
                                            for (int j = 0; j < 5; j++)
                                            {
                                                if (player.resources >= Unit.GetCost(Unit.Type.Fast))
                                                {
                                                    building.CreateUnit(Unit.Type.Fast);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (player.resources >= Unit.GetCost(Unit.Type.Fast))
                                            {
                                                building.CreateUnit(Unit.Type.Fast);
                                            }
                                        }
                                    }
                                }
                                break;

                            case HUDObject.Type.Heavy:
                                for (int k = 0; k < player.buildingSelection.buildings.Count(); k++)
                                {
                                    Building bu = player.buildingSelection.buildings.ElementAt(k);
                                    if (bu is Factory)
                                    {
                                        Factory building = (Factory)bu;
                                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                                        {
                                            for (int j = 0; j < 5; j++)
                                            {
                                                if (player.resources >= Unit.GetCost(Unit.Type.HeavyMelee))
                                                {
                                                    building.CreateUnit(Unit.Type.HeavyMelee);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (player.resources >= Unit.GetCost(Unit.Type.HeavyMelee))
                                            {
                                                building.CreateUnit(Unit.Type.HeavyMelee);
                                            }
                                        }
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                for (int i = 0; i < this.commandObjects.Count(); i++)
                {
                    HUDCommandObject co = this.commandObjects.ElementAt(i);
                    if (co.DefineRectangle().Contains(state.X, state.Y))
                    {
                        switch (co.type)
                        {
                            case HUDCommandObject.Type.Repair:
                                if (!co.disabled)
                                {
                                    player.command = new Command(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Repair), this.player, Command.Type.Repair, state.X, state.Y, new Color(255, 187, 0, 255));
                                }
                                break;

                            case HUDCommandObject.Type.Attack:
                                if (!co.disabled)
                                {
                                    player.command = new Command(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Attack), this.player, Command.Type.Attack, state.X, state.Y, new Color(255, 0, 12, 255));
                                }
                                break;

                            case HUDCommandObject.Type.Defend:
                                if (!co.disabled)
                                {
                                    player.command = new Command(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Defend), this.player, Command.Type.Defend, state.X, state.Y, new Color(255, 125, 0, 255));
                                }
                                break;

                            case HUDCommandObject.Type.Move:
                                if (!co.disabled)
                                {
                                    player.command = new Command(TextureManager.GetInstance().GetTexture(HUDCommandObject.Type.Move), this.player, Command.Type.Move, state.X, state.Y, new Color(0, 100, 255, 255));
                                }
                                break;

                            case HUDCommandObject.Type.Stop:
                                if (!co.disabled)
                                {
                                    if (player.currentSelection != null)
                                    {
                                        for (int j = 0; j < player.currentSelection.units.Count(); j++)
                                        {
                                            Unit u = player.currentSelection.units.ElementAt(j);
                                            u.unitToDefend = null;
                                            u.unitToStalk = null;
                                            u.waypoints.Clear();
                                            u.SetJob(Unit.Job.Idle);
                                            u.hasToMove = false;
                                            u.buildingToDestroy = null;
                                        }
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                for (int i = 0; i < player.buildings.Count(); i++)
                {
                    Building b = player.buildings.ElementAt(i);
                    if (b.state == Building.State.Preview &&
                        !this.DefineRectangle().Contains(new Rectangle(me.location.X, me.location.Y, 1, 1)) &&
                        Game1.GetInstance().map.collisionMap.CanPlace(b.DefineRectangle()))
                    {
                        Engineer temp = null;
                        for (int j = 0; j < player.currentSelection.units.Count(); j++)
                        {
                            Unit u = player.currentSelection.units.ElementAt(j);

                            if (u.type == Unit.Type.Engineer)
                            {
                                Point p = new Point((int)(b.x + (b.texture.Width / 2)), (int)(b.y + (b.texture.Height / 2)));

                                // Add a point that is on the circle near the building, not inside the building!
                                Point targetPoint = new Point(0, 0);
                                if (u.waypoints.Count() == 0) targetPoint = new Point((int)u.x, (int)u.y);
                                else targetPoint = u.waypoints.ElementAt(u.waypoints.Count() - 1);
                                // Move to the point around the circle of the building, but increase the radius a bit
                                // so we're not standing on the exact top of the building
                                u.MoveToQueue(
                                    Util.GetPointOnCircle(p, b.GetCircleRadius() + u.texture.Width / 2,
                                    Util.GetHypoteneuseAngleDegrees(p, targetPoint)));

                                // Set the Engineer to link with the Building so construction won't start without an Engineer
                                // Since only one Engineer is needed, break aftwards
                                if (temp == null)
                                {
                                    temp = (Engineer)u;
                                    break;
                                }
                            }
                        }

                        b.PlaceBuilding(temp);
                    }
                }
            }
        }

        /// <summary>
        /// MouseRelease Listener.
        /// </summary>
        /// <param name="me">The MouseEvent to use</param>
        void MouseClickListener.OnMouseRelease(MouseEvent me)
        {
            // Not Implemented
        }

        /// <summary>
        /// Count the different unit/building types selected.
        /// Checks what objects to load in the HUD.
        /// </summary>
        public void CountUnits()
        {
            int engineerCounter = 0;
            int unitCounter = 0;

            if (player.currentSelection != null)
            {
                for (int j = 0; j < player.currentSelection.units.Count(); j++)
                {
                    switch (player.currentSelection.units.ElementAt(j).type)
                    {
                        case Unit.Type.Engineer:
                            engineerCounter++;
                            break;

                        case Unit.Type.Fast:
                            unitCounter++;
                            break;

                        case Unit.Type.HeavyMelee:
                            unitCounter++;
                            break;

                        case Unit.Type.HeavyRanged:
                            unitCounter++;
                            break;

                        case Unit.Type.Melee:
                            unitCounter++;
                            break;

                        case Unit.Type.Ranged:
                            unitCounter++;
                            break;

                        default:
                            break;
                    }
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

            if (unitCounter > 0)
            {
                loadForUnit = true;
            }
            else
            {
                loadForUnit = false;
            }

            int resourcesCounter = 0;
            int barracksCounter = 0;
            int factoryCounter = 0;
            int fortressCounter = 0;
            int sentryCounter = 0;

            if (player.buildingSelection != null)
            {
                for (int i = 0; i < player.buildingSelection.buildings.Count(); i++)
                {
                    Building b = player.buildingSelection.buildings.ElementAt(i);
                    switch (b.type)
                    {
                        case Building.Type.Resources:
                            resourcesCounter++;
                            break;

                        case Building.Type.Barracks:
                            barracksCounter++;
                            break;

                        case Building.Type.Factory:
                            factoryCounter++;
                            break;

                        case Building.Type.Fortress:
                            fortressCounter++;
                            break;

                        case Building.Type.Sentry:
                            sentryCounter++;
                            break;

                        default:
                            break;
                    }
                }
            }

            if (resourcesCounter > 0)
            {
                loadForResources = true;
            }
            else
            {
                loadForResources = false;
            }

            if (barracksCounter > 0)
            {
                loadForBarracks = true;
            }
            else
            {
                loadForBarracks = false;
            }

            if (factoryCounter > 0)
            {
                loadForFactory = true;
            }
            else
            {
                loadForFactory = false;
            }

            if (fortressCounter > 0)
            {
                loadForFortress = true;
            }
            else
            {
                loadForFortress = false;
            }

            if (sentryCounter > 0)
            {
                loadForSentry = true;
            }
            else
            {
                loadForSentry = false;
            }
        }

        /// <summary>
        /// Checks whether the mouse is hovering over an object on the HUD.
        /// </summary>
        /// <returns>Returns false if not true, else returns true</returns>
        public Boolean IsMouseOverBuilding()
        {
            Boolean check = false;
            MouseState state = Mouse.GetState();

            for( int i = 0; i < this.objects.Count(); i++ ){
                HUDObject obj = this.objects.ElementAt(i);
                if (obj.DefineRectangle().Contains(state.X, state.Y))
                {
                    check = true;
                }
            }

            return check;
        }

        /// <summary>
        /// Defines the space the HUD is using.
        /// </summary>
        /// <returns>Returns a Rectangle with the right size</returns>
        public Rectangle DefineRectangle()
        {
            return new Rectangle(195, 652, 634, 116);
        }

        /// <summary>
        /// Defines the space the minimap is using
        /// </summary>
        /// <returns>The rectangle the minimap is using</returns>
        public Rectangle DefineMiniMapRectangle()
        {
            return new Rectangle(831, 571, 191, 195);
        }
    }
}