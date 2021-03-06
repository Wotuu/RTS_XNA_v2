﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Buildings;
using PathfindingTest.Players;
using PathfindingTest.Pathfinding;
using PathfindingTest.Collision;
using PathfindingTest.Units;
using System.Diagnostics;
using PathfindingTest.Units.Stores;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;
using PathfindingTest.Combat;
using PathfindingTest.Interfaces;
using AStarCollisionMap.Pathfinding;
using CustomLists.Lists;

namespace PathfindingTest.Buildings
{
    public abstract class Building : Damageable, Offsetable
    {

        public Player p { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public Color c { get; set; }
        public Color previewC = new Color(175, 175, 175, 80);
        public Color previewCantPlace = new Color(10, 10, 10, 80);
        public Color constructC { get; set; }
        public Engineer constructedBy { get; set; }
        public double constructDuration { get; set; }
        public double constructProgress { get; set; }

        public float currentHealth { get; set; }
        public float maxHealth { get; set; }

        public Boolean selected { get; set; }
        public Boolean mouseOver { get; set; }
        public Boolean canPlace { get; set; }
        public Boolean constructionStarted { get; set; }
        public Boolean isDestroyed = false;
        public State state { get; set; }

        public CustomArrayList<ProductionUnit> productionQueue { get; set; }

        public Type type { get; set; }
        public Texture2D texture { get; set; }

        public BuildingMesh mesh { get; set; }
        public HealthBar healthBar { get; set; }
        public ProgressBar progressBar { get; set; }

        public float visionRange { get; set; }

        public CustomArrayList<Unit> constructionQueue { get; set; }
        public Point originWaypoint { get; set; }
        public CustomArrayList<Point> waypoints { get; set; }

        public BuildingMultiplayerData multiplayerData { get; set; }

        public int engineerInQueue { get; set; }
        public int meleeInQueue { get; set; }
        public int rangedInQueue { get; set; }
        public int fastInQueue { get; set; }
        public int heavyMeleeInQueue { get; set; }
        public int heavyRangedInQueue { get; set; }

        public enum Type
        {
            Resources,
            Barracks,
            Factory,
            Fortress,
            Sentry
        }

        public enum State
        {
            Preview,
            Constructing,
            Interrupted,
            Finished,
            Repairing,
            Producing,
            MultiplayerWaitingForLocation
        }

        public enum VisionRange
        {
            Barracks = 150,
            Factory = 150,
            Fortress = 175,
            ResourceGatherer = 150,
            Sentry = 350
        }

        public abstract void Update(KeyboardState ks, MouseState ms);

        internal abstract void Draw(SpriteBatch sb);

        public void DefaultUpdate(KeyboardState ks, MouseState ms)
        {
            switch (state)
            {
                case State.Preview:
                    if (p.resources < Building.GetCost(this.type))
                    {
                        canPlace = false;
                    }
                    else
                    {
                        canPlace = Game1.GetInstance().map.collisionMap.CanPlace(this.DefinePlacementRectangle());
                    }
                    this.x = (ms.X - (texture.Width / 2)) + Game1.GetInstance().drawOffset.X;
                    this.y = (ms.Y - (texture.Height / 2)) + Game1.GetInstance().drawOffset.Y;
                    break;

                case State.Constructing:
                    CountUnitsInQueue();
                    if (EngineerInRange())
                    {
                        this.constructionStarted = true;

                        if (this.constructC.A < 255)
                        {
                            Color newColor = this.constructC;
                            constructProgress += (1 / constructDuration);
                            newColor.A = (byte)((constructProgress / 100) * 255);

                            float newHealth = (float)((constructProgress / 100) * maxHealth);
                            if (newHealth > maxHealth) newHealth = maxHealth;
                            currentHealth = newHealth;

                            this.constructC = newColor;
                        }
                        else
                        {
                            state = State.Finished;
                        }
                    }
                    break;

                case State.Interrupted:
                    CountUnitsInQueue();
                    this.constructionStarted = false;
                    break;

                case State.Finished:
                    this.visionRange = Building.GetVisionRange(this.type);

                    CountUnitsInQueue();
                    if (productionQueue != null)
                    {
                        if (productionQueue.Count() > 0)
                        {
                            this.state = State.Producing;
                        }
                    }
                    break;

                case State.Repairing:
                    CountUnitsInQueue();
                    if (EngineerInRange())
                    {
                        if (this.currentHealth < this.maxHealth)
                        {
                            constructProgress += (1 / constructDuration);
                            float newHealth = (float)((constructProgress / 100) * maxHealth);
                            if (newHealth > maxHealth) newHealth = maxHealth;
                            currentHealth = newHealth;
                        }
                        else
                        {
                            state = State.Finished;
                        }
                    }
                    break;

                case State.Producing:
                    CountUnitsInQueue();
                    if (productionQueue == null)
                    {
                        this.state = State.Finished;
                        break;
                    }

                    if (productionQueue != null)
                    {
                        if (productionQueue.Count() == 0)
                        {
                            this.state = State.Finished;
                            break;
                        }
                        else if (productionQueue.Count() > 0)
                        {
                            Produce();
                        }
                    }
                    break;

                default:
                    break;
            }

            if (this.DefineDrawRectangle().Contains(ms.X, ms.Y))
            {
                this.mouseOver = true;
            }
            else
            {
                this.mouseOver = false;
            }
        }

        internal void DefaultDraw(SpriteBatch sb)
        {
            switch (state)
            {
                case State.Preview:
                    if (canPlace)
                    {
                        sb.Draw(texture, this.DefineDrawRectangle(), null, previewC, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    }
                    if (!canPlace)
                    {
                        sb.Draw(texture, this.DefineDrawRectangle(), null, previewCantPlace, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    }
                    break;

                case State.Constructing:
                    sb.Draw(texture, this.GetDrawRectangle(), null, constructC, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    DrawWaypointLine(sb);
                    break;

                case State.Interrupted:
                    sb.Draw(texture, this.GetDrawRectangle(), null, constructC, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    DrawWaypointLine(sb);
                    break;

                case State.Finished:
                    sb.Draw(texture, this.GetDrawRectangle(), null, c, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    DrawWaypointLine(sb);
                    break;

                case State.Repairing:
                    sb.Draw(texture, this.GetDrawRectangle(), null, c, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    DrawWaypointLine(sb);
                    break;

                case State.Producing:
                    sb.Draw(texture, this.GetDrawRectangle(), null, c, 0f, Vector2.Zero, SpriteEffects.None, this.z);
                    DrawWaypointLine(sb);
                    break;

                default:
                    break;
            }

            DrawProgressBar(sb);
            DrawHealthBar(sb);
        }

        internal void DrawProgressBar(SpriteBatch sb)
        {
            if (this.state == State.Constructing || this.state == State.Interrupted)
            {
                progressBar.progress = this.constructProgress;
                progressBar.Draw(sb);
            }
            else if (this.state == State.Producing)
            {
                if (productionQueue.Count() > 0)
                {
                    ProductionUnit pu = productionQueue.ElementAt(0);
                    progressBar.progress = pu.productionProgress;
                    progressBar.Draw(sb);
                }
            }
        }

        internal void DrawHealthBar(SpriteBatch sb)
        {
            if (selected || mouseOver)
            {
                int healthPercent = (int)((this.currentHealth / this.maxHealth) * 100.0);
                healthBar.percentage = healthPercent;
                healthBar.Draw(sb);
            }
        }

        internal void DrawQueuedStats(SpriteBatch sb)
        {
            this.CountUnitsInQueue();
            if (selected)
            {
                if (engineerInQueue > 0)
                {
                    Vector2 stringPos = p.hud.sf.MeasureString(engineerInQueue.ToString());
                    sb.DrawString(p.hud.sf, engineerInQueue.ToString(),
                                  new Vector2(p.hud.engineerObject.x + (p.hud.engineerObject.texture.Width / 2) - (stringPos.X / 2),
                                              p.hud.engineerObject.y + (p.hud.engineerObject.texture.Height / 2) - (stringPos.Y / 2)),
                                  Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0998f);
                }
                if (meleeInQueue > 0)
                {
                    Vector2 stringPos = p.hud.sf.MeasureString(meleeInQueue.ToString());
                    sb.DrawString(p.hud.sf, meleeInQueue.ToString(),
                                  new Vector2(p.hud.meleeObject.x + (p.hud.meleeObject.texture.Width / 2) - (stringPos.X / 2),
                                              p.hud.meleeObject.y + (p.hud.meleeObject.texture.Height / 2) - (stringPos.Y / 2)),
                                  Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0998f);
                }
                if (rangedInQueue > 0)
                {
                    Vector2 stringPos = p.hud.sf.MeasureString(rangedInQueue.ToString());
                    sb.DrawString(p.hud.sf, rangedInQueue.ToString(),
                                  new Vector2(p.hud.rangedObject.x + (p.hud.rangedObject.texture.Width / 2) - (stringPos.X / 2),
                                              p.hud.rangedObject.y + (p.hud.rangedObject.texture.Height / 2) - (stringPos.Y / 2)),
                                  Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0998f);
                }
                if (fastInQueue > 0)
                {
                    Vector2 stringPos = p.hud.sf.MeasureString(fastInQueue.ToString());
                    sb.DrawString(p.hud.sf, fastInQueue.ToString(),
                                  new Vector2(p.hud.fastObject.x + (p.hud.fastObject.texture.Width / 2) - (stringPos.X / 2),
                                              p.hud.fastObject.y + (p.hud.fastObject.texture.Height / 2) - (stringPos.Y / 2)),
                                  Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0998f);
                }
                if (heavyMeleeInQueue > 0)
                {
                    Vector2 stringPos = p.hud.sf.MeasureString(heavyMeleeInQueue.ToString());
                    sb.DrawString(p.hud.sf, heavyMeleeInQueue.ToString(),
                                  new Vector2(p.hud.heavyMeleeObject.x + (p.hud.heavyMeleeObject.texture.Width / 2) - (stringPos.X / 2),
                                              p.hud.heavyMeleeObject.y + (p.hud.heavyMeleeObject.texture.Height / 2) - (stringPos.Y / 2)),
                                  Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0998f);
                }
                if (heavyRangedInQueue > 0)
                {
                    Vector2 stringPos = p.hud.sf.MeasureString(heavyRangedInQueue.ToString());
                    sb.DrawString(p.hud.sf, heavyRangedInQueue.ToString(),
                                  new Vector2(p.hud.heavyRangedObject.x + (p.hud.heavyRangedObject.texture.Width / 2) - (stringPos.X / 2),
                                              p.hud.heavyRangedObject.y + (p.hud.heavyRangedObject.texture.Height / 2) - (stringPos.Y / 2)),
                                  Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0998f);
                }
            }
        }

        internal void DrawWaypointLine(SpriteBatch sb)
        {
            if (selected && this.type != Type.Resources && this.type != Type.Sentry)
            {
                Vector2 offset = Game1.GetInstance().drawOffset;

                DrawUtil.DrawLine(sb, new Point((int)x + (this.texture.Width / 2) - (int)offset.X + 1,
                                                (int)y + (this.texture.Height / 2) - (int)offset.Y),
                                  new Point((int)this.originWaypoint.X - (int)offset.X + 1,
                                            (int)this.originWaypoint.Y - (int)offset.Y),
                                  Color.DarkCyan, 2, this.z + 0.00001f);

                if (this.waypoints != null)
                {
                    if (this.waypoints.Count() > 0)
                    {
                        Point pp = originWaypoint;
                        for (int i = 0; i < this.waypoints.Count(); i++)
                        {
                            Point p = this.waypoints.ElementAt(i);
                            DrawUtil.DrawLine(sb, new Point(pp.X - (int)offset.X, pp.Y - (int)offset.Y), new Point(p.X - (int)offset.X, p.Y - (int)offset.Y), Color.DarkCyan, 2, this.z + 0.00002f);
                            sb.Draw(TextureManager.GetInstance().GetSolidTexture(), new Rectangle(pp.X - (int)offset.X - 2, pp.Y - (int)offset.Y - 2, 4, 4), null, Color.DarkCyan, 0f, Vector2.Zero, SpriteEffects.None, this.z + 0.00001f);
                            pp = p;
                        }
                    }
                }
            }
        }

        public void CountUnitsInQueue()
        {
            engineerInQueue = 0;
            fastInQueue = 0;
            heavyMeleeInQueue = 0;
            heavyRangedInQueue = 0;
            meleeInQueue = 0;
            rangedInQueue = 0;

            for (int i = 0; i < this.productionQueue.Count(); i++)
            {
                switch (this.productionQueue.ElementAt(i).type)
                {
                    case Unit.Type.Engineer:
                        engineerInQueue++;
                        break;

                    case Unit.Type.Fast:
                        fastInQueue++;
                        break;

                    case Unit.Type.HeavyMelee:
                        heavyMeleeInQueue++;
                        break;

                    case Unit.Type.HeavyRanged:
                        heavyRangedInQueue++;
                        break;

                    case Unit.Type.Melee:
                        meleeInQueue++;
                        break;

                    case Unit.Type.Ranged:
                        rangedInQueue++;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Places the building on the map
        /// </summary>
        public abstract void PlaceBuilding(Engineer e);

        /// <summary>
        /// Gets the radius of the circle surrounding this building
        /// </summary>
        /// <returns>The value</returns>
        public int GetCircleRadius()
        {
            return (int)(Util.GetHypoteneuseLength(
                new Point((int)this.x, (int)this.y),
                new Point(DefineRectangle().Left, DefineRectangle().Bottom)) / 2);
        }

        public void CreateUnit(Unit.Type type)
        {
            ProductionUnit newUnit = null;

            switch (type)
            {
                case Unit.Type.Engineer:
                    if (this.type == Type.Fortress)
                    {
                        newUnit = new ProductionUnit(100f, 0.2, type);
                        p.resources -= Unit.GetCost(type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Melee:
                    if (this.type == Type.Barracks)
                    {
                        newUnit = new ProductionUnit(100f, 0.2, type);
                        p.resources -= Unit.GetCost(type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.HeavyMelee:
                    if (this.type == Type.Factory)
                    {
                        newUnit = new ProductionUnit(100f, 0.2, type);
                        p.resources -= Unit.GetCost(type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Fast:
                    if (this.type == Type.Barracks)
                    {
                        newUnit = new ProductionUnit(100f, 0.2, type);
                        p.resources -= Unit.GetCost(type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Ranged:
                    if (this.type == Type.Barracks)
                    {
                        newUnit = new ProductionUnit(100f, 0.2, type);
                        p.resources -= Unit.GetCost(type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.HeavyRanged:
                    if (this.type == Type.Factory)
                    {
                        newUnit = new ProductionUnit(100f, 0.2, type);
                        p.resources -= Unit.GetCost(type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                default:
                    break;
            }
        }

        public void Produce()
        {
            ProductionUnit pu = productionQueue.ElementAt(0);

            pu.productionProgress += (1 / pu.productionDuration);

            if (pu.productionProgress >= 100)
            {
                Unit newUnit = null;

                switch (pu.type)
                {
                    case Unit.Type.Engineer:
                        if (this.type == Type.Fortress)
                        {
                            newUnit = p.meleeStore.getUnit(pu.type, originWaypoint.X, originWaypoint.Y);
                        }
                        break;

                    case Unit.Type.Melee:
                        if (this.type == Type.Barracks)
                        {
                            newUnit = p.meleeStore.getUnit(pu.type, originWaypoint.X, originWaypoint.Y);
                        }
                        break;

                    case Unit.Type.HeavyMelee:
                        if (this.type == Type.Factory)
                        {
                            newUnit = p.meleeStore.getUnit(pu.type, originWaypoint.X, originWaypoint.Y);
                        }
                        break;

                    case Unit.Type.Fast:
                        if (this.type == Type.Barracks)
                        {
                            newUnit = p.fastStore.getUnit(pu.type, originWaypoint.X, originWaypoint.Y);
                        }
                        break;

                    case Unit.Type.Ranged:
                        if (this.type == Type.Barracks)
                        {
                            newUnit = p.rangedStore.getUnit(pu.type, originWaypoint.X, originWaypoint.Y);
                        }
                        break;

                    case Unit.Type.HeavyRanged:
                        if (this.type == Type.Factory)
                        {
                            newUnit = p.rangedStore.getUnit(pu.type, originWaypoint.X, originWaypoint.Y);
                        }
                        break;

                    default:
                        break;
                }

                if (newUnit != null)
                {
                    if (waypoints.Count() > 0)
                    {
                        if (Game1.GetInstance().IsMultiplayerGame()) 
                            newUnit.multiplayerData.moveTarget = waypoints.GetLast();

                        newUnit.MoveToQueue(waypoints.GetLast());
                        Console.Out.WriteLine("Moving unit after building it to " + waypoints.GetLast());
                        newUnit.hasToMove = true;
                    }
                    productionQueue.RemoveFirst(true);

                    //// Synchronize this unit, since the unit has moved (in other words, teleported)
                    if (Game1.GetInstance().IsMultiplayerGame()) Synchronizer.GetInstance().QueueUnit(newUnit);
                }
            }
        }

        public Boolean EngineerInRange()
        {
            if (constructedBy != null)
            {
                Rectangle br = new Rectangle((int)this.x - 15, (int)this.y - 15, texture.Width + 30, texture.Height + 30);
                Rectangle er = new Rectangle((int)constructedBy.x, (int)constructedBy.y, 1, 1);

                if (br.Contains(er))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the rectangle of this building, WITHOUT the draw offset.
        /// </summary>
        /// <returns>The rectangle</returns>
        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)this.x, (int)this.y, texture.Width, texture.Height);
        }

        /// <summary>
        /// Gets the placement rectangle of this building, WITHOUT the draw offset.
        /// </summary>
        /// <returns>The rectangle</returns>
        public Rectangle DefinePlacementRectangle()
        {
            if (this.type != Type.Resources && this.type != Type.Sentry)
            {
                return new Rectangle((int)this.x, (int)this.y, texture.Width, texture.Height + 30);
            }
            else
            {
                return DefineRectangle();
            }
        }

        /// <summary>
        /// Defines the draw rectangle of this building, WITH the draw offset (giving screen coordinates)
        /// </summary>
        /// <returns>The rectangle</returns>
        public Rectangle DefineDrawRectangle()
        {
            return new Rectangle((int)this.x - (int)Game1.GetInstance().drawOffset.X, (int)this.y - (int)Game1.GetInstance().drawOffset.Y, texture.Width, texture.Height);
        }

        public Rectangle DefineAddedDrawRectangle()
        {
            return new Rectangle((int)this.x + (int)Game1.GetInstance().drawOffset.X, (int)this.y + (int)Game1.GetInstance().drawOffset.Y, texture.Width, texture.Height);
        }

        public Rectangle DefineSelectedRectangle()
        {
            return new Rectangle((int)this.x - 3 - (int)Game1.GetInstance().drawOffset.X, (int)this.y - 3 - (int)Game1.GetInstance().drawOffset.Y, texture.Width + 6, texture.Height + 6);
        }

        /// <summary>
        /// Disposes of this building
        /// </summary>
        public void Dispose()
        {
            this.isDestroyed = true;
            productionQueue = null;
            p.buildings.Remove(this);
            if (this.mesh != null) mesh.Reverse();

            if (this is ResourceGather && this.state != State.Preview)
            {
                for (int i = 0; i < p.buildings.Count(); i++)
                {
                    Building b = p.buildings.ElementAt(i);
                    if (b is ResourceGather)
                    {
                        ResourceGather rg = (ResourceGather)b;
                        rg.CalculateRPS();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the point location of this building.
        /// </summary>
        /// <returns>The new point.</returns>
        public Point GetLocation()
        {
            return new Point((int)x, (int)y);
        }

        /// <summary>
        /// Gets the cost of a building
        /// </summary>
        /// <param name="t">The building type of which the cost is wanted</param>
        /// <returns>The cost of the building</returns>
        public static int GetCost(Type t)
        {
            switch (t)
            {
                case Type.Barracks:
                    return 300;

                case Type.Factory:
                    return 600;

                case Type.Fortress:
                    return 1000;

                case Type.Resources:
                    return 100;

                case Type.Sentry:
                    return 750;

                default:
                    return 0;
            }
        }

        public static float GetVisionRange(Type t)
        {
            switch (t)
            {
                case Type.Barracks:
                    return (float)VisionRange.Barracks;

                case Type.Factory:
                    return (float)VisionRange.Factory;

                case Type.Fortress:
                    return (float)VisionRange.Fortress;

                case Type.Resources:
                    return (float)VisionRange.ResourceGatherer;

                case Type.Sentry:
                    return (float)VisionRange.Sentry;

                default:
                    return 0f;
            }
        }

        public Building(Player player)
        {
            this.p = player;
            this.p.buildings.AddLast(this);
            this.constructProgress = 0;

            this.z = 1f - player.buildings.Count() * 0.0001f;

            this.progressBar = new ProgressBar(this);
            this.healthBar = new HealthBar(this);

            this.productionQueue = new CustomArrayList<ProductionUnit>();

            if (Game1.GetInstance().IsMultiplayerGame())
            {
                Boolean isLocal = this.p == Game1.CURRENT_PLAYER;
                this.multiplayerData = new BuildingMultiplayerData(this, isLocal);
                if (isLocal)
                {
                    this.multiplayerData.RequestServerID();
                    this.state = State.Preview;
                }
                else
                {
                    this.state = State.MultiplayerWaitingForLocation;
                }
            }
            else
            {
                this.state = State.Preview;
            }
        }

        /// <summary>
        /// Calculates a path between the current originWaypoint and the point.
        /// </summary>
        /// <param name="p">The point to calculate to.</param>
        /// <returns>The list containing all the points that you should visit.</returns>
        public CustomArrayList<Point> CalculatePath(Point p)
        {
            Vector2 offset = Game1.GetInstance().drawOffset;
            CustomArrayList<Point> result = new CustomArrayList<Point>();
            long ticks = DateTime.UtcNow.Ticks;
            if (Game1.GetInstance().map.collisionMap.IsCollisionBetween(this.originWaypoint, p))
            {
                Game1 game = Game1.GetInstance();
                // Create temp nodes
                Node start = new Node(game.map.collisionMap, (int)this.originWaypoint.X, (int)this.originWaypoint.Y, true);
                Node end = new Node(game.map.collisionMap, p.X, p.Y, true);
                CustomArrayList<PathfindingNode> nodes = new AStar(start, end).FindPath();
                if (nodes != null)
                {
                    // Remove the first node, because that's the node we're currently on ..
                    nodes.RemoveFirst(true);
                    // Clear our current waypoints
                    this.waypoints.Clear();
                    for (int i = 0; i < nodes.Count(); i++)
                    {
                        Node n = (Node)nodes.ElementAt(i);
                        result.AddLast(n.GetLocation());
                    }
                }
                // Nodes can no longer be used
                start.Destroy();
                end.Destroy();
            }
            else
            {
                result.AddLast(p);
            }
            return result;
        }

        public void OnDamage(DamageEvent e)
        {
            this.currentHealth -= e.damageDone;
            if (this.currentHealth <= 0 && !this.isDestroyed)
            {
                this.Dispose();
            }
        }

        public Rectangle GetDrawRectangle()
        {
            Game1 game = Game1.GetInstance();
            return new Rectangle((int)(this.x - game.drawOffset.X), (int)(this.y - game.drawOffset.Y),
                this.texture.Width, this.texture.Height);
        }
    }
}