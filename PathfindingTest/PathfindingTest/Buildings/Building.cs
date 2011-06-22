using System;
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

namespace PathfindingTest.Buildings
{
    public abstract class Building
    {

        public Player p { get; set; }
        public float x { get; set; }
        public float y { get; set; }
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
        public State state { get; set; }

        public LinkedList<ProductionUnit> productionQueue { get; set; }

        public Type type { get; set; }
        public Texture2D texture { get; set; }

        public BuildingMesh mesh { get; set; }
        public HealthBar healthBar { get; set; }
        public ProgressBar progressBar { get; set; }

        public LinkedList<Unit> constructionQueue { get; set; }
        public Point waypoint { get; set; }

        public BuildingMultiplayerData multiplayerData { get; set; }


        public enum Type
        {
            Resources,
            Barracks,
            Factory,
            Fortress
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

        public abstract void Update(KeyboardState ks, MouseState ms);

        internal abstract void Draw(SpriteBatch sb);

        public void DefaultUpdate(KeyboardState ks, MouseState ms)
        {
            switch (state)
            {
                case State.Preview:
                    canPlace = Game1.GetInstance().collision.CanPlace(this.DefineRectangle());
                    this.x = (ms.X - (texture.Width / 2));
                    this.y = (ms.Y - (texture.Height / 2));
                    break;

                case State.Constructing:
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
                    this.constructionStarted = false;
                    break;

                case State.Finished:
                    if (productionQueue != null)
                    {
                        if (productionQueue.Count > 0)
                        {
                            this.state = State.Producing;
                        }
                    }
                    break;

                case State.Repairing:
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
                    if (productionQueue == null)
                    {
                        this.state = State.Finished;
                        break;
                    }

                    if (productionQueue != null)
                    {
                        if (productionQueue.Count == 0)
                        {
                            this.state = State.Finished;
                            break;
                        }
                        else if (productionQueue.Count > 0)
                        {
                            Produce();
                        }
                    }
                    break;

                default:
                    break;
            }

            if (this.DefineRectangle().Contains(ms.X, ms.Y))
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
                        sb.Draw(texture, new Vector2(x, y), previewC);
                    }
                    if (!canPlace)
                    {
                        sb.Draw(texture, new Vector2(x, y), previewCantPlace);
                    }
                    break;

                case State.Constructing:
                    sb.Draw(texture, new Vector2(x, y), constructC);
                    break;

                case State.Interrupted:
                    sb.Draw(texture, new Vector2(x, y), constructC);
                    break;

                case State.Finished:
                    sb.Draw(texture, new Vector2(x, y), c);
                    break;

                case State.Repairing:
                    sb.Draw(texture, new Vector2(x, y), c);
                    break;

                case State.Producing:
                    sb.Draw(texture, new Vector2(x, y), c);
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
                ProductionUnit pu = productionQueue.ElementAt(0);
                progressBar.progress = pu.productionProgress;
                progressBar.Draw(sb);
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
                        newUnit = new ProductionUnit(100f, 5.0, type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Melee:
                    if (this.type == Type.Barracks)
                    {
                        newUnit = new ProductionUnit(100f, 5.0, type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.HeavyMelee:
                    if (this.type == Type.Factory)
                    {
                        newUnit = new ProductionUnit(100f, 5.0, type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Fast:
                    if (this.type == Type.Barracks)
                    {
                        newUnit = new ProductionUnit(100f, 5.0, type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.Ranged:
                    if (this.type == Type.Barracks)
                    {
                        newUnit = new ProductionUnit(100f, 5.0, type);
                        productionQueue.AddLast(newUnit);
                    }
                    break;

                case Unit.Type.HeavyRanged:
                    if (this.type == Type.Factory)
                    {
                        newUnit = new ProductionUnit(100f, 5.0, type);
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
                            newUnit = p.meleeStore.getUnit(pu.type, waypoint.X, waypoint.Y);
                            productionQueue.RemoveFirst();
                        }
                        break;

                    case Unit.Type.Melee:
                        if (this.type == Type.Barracks)
                        {
                            newUnit = p.meleeStore.getUnit(pu.type, waypoint.X, waypoint.Y);
                            productionQueue.RemoveFirst();
                        }
                        break;

                    case Unit.Type.HeavyMelee:
                        if (this.type == Type.Factory)
                        {
                            newUnit = p.meleeStore.getUnit(pu.type, waypoint.X, waypoint.Y);
                            productionQueue.RemoveFirst();
                        }
                        break;

                    case Unit.Type.Fast:
                        if (this.type == Type.Barracks)
                        {
                            newUnit = p.fastStore.getUnit(pu.type, waypoint.X, waypoint.Y);
                            productionQueue.RemoveFirst();
                        }
                        break;

                    case Unit.Type.Ranged:
                        if (this.type == Type.Barracks)
                        {
                            newUnit = p.rangedStore.getUnit(pu.type, waypoint.X, waypoint.Y);
                            productionQueue.RemoveFirst();
                        }
                        break;

                    case Unit.Type.HeavyRanged:
                        if (this.type == Type.Factory)
                        {
                            newUnit = p.rangedStore.getUnit(pu.type, waypoint.X, waypoint.Y);
                            productionQueue.RemoveFirst();
                        }
                        break;

                    default:
                        break;
                }

                //// Synchronize this unit, since the unit has moved (in other words, teleported)
                //if (Game1.GetInstance().IsMultiplayerGame()) Synchronizer.GetInstance().QueueUnit(pu);
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

        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)this.x, (int)this.y, texture.Width, texture.Height);
        }

        public Rectangle DefineSelectedRectangle()
        {
            return new Rectangle((int)this.x - 3, (int)this.y - 3, texture.Width + 6, texture.Height + 6);
        }

        /// <summary>
        /// Disposes of this building
        /// </summary>
        public void Dispose()
        {
            productionQueue = null;
            p.buildings.Remove(this);
            if (this.mesh != null) mesh.Reverse();
        }

        /// <summary>
        /// Gets the point location of this building.
        /// </summary>
        /// <returns>The new point.</returns>
        public Point GetLocation()
        {
            return new Point((int)x, (int)y);
        }

        public Building(Player player)
        {
            this.p = player;
            this.p.buildings.AddLast(this);
            this.constructProgress = 0;

            this.progressBar = new ProgressBar(this);
            this.healthBar = new HealthBar(this);

            this.productionQueue = new LinkedList<ProductionUnit>();

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
    }
}