using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Pathfinding;
using PathfindingTest.Interfaces;
using PathfindingTest.Players;
using PathfindingTest.QuadTree;
using PathfindingTest.Combat;
using AStarCollisionMap.Pathfinding;
using AStarCollisionMap.Collision;
using XNAInputHandler.MouseInput;
using PathfindingTest.State;
using PathfindingTest.Multiplayer.Data;
using System.Diagnostics;
using PathfindingTest.Buildings;
using PathfindingTest.Units.Melee;

namespace PathfindingTest.Units
{
    public abstract class Unit : OnCollisionChangedListener, Aggroable, Damageable, Offsetable
    {
        public Player player { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public int halfTextureWidth { get; set; }
        public int halfTextureHeight { get; set; }

        public Type type { get; set; }
        public Color color { get; set; }
        public Texture2D texture { get; set; }
        public Texture2D hitTexture { get; set; }
        public Boolean hitting { get; set; }
        public int hitFrame { get; set; }
        public Boolean selected { get; set; }

        public float collisionRadius { get; set; }
        private LinkedList<Unit> collisionWith { get; set; }
        public Boolean repelsOthers { get; set; }
        public Quad quad { get; set; }

        public float currentHealth { get; set; }
        public float maxHealth { get; set; }
        private HealthBar healthBar { get; set; }
        public LinkedList<Unit> enemiesInRange { get; set; }
        public LinkedList<Unit> friendliesProtectingMe { get; set; }
        public LinkedList<Building> buildingsInRange { get; set; }
        public int baseDamage { get; set; }

        public Boolean isDead = false;
        public Job job { get; set; }

        public Unit unitToStalk { get; set; }
        public Unit unitToDefend { get; set; }
        public Building buildingToDestroy { get; set; }
        public float attackRange { get; set; }
        public float aggroRange { get; set; }
        public float fireCooldown { get; set; }
        public float rateOfFire { get; set; }
        public float visionRange { get; set; }

        public Point assaultPoint { get; set; }

        public UnitMultiplayerData multiplayerData { get; set; }

        #region Movement variables
        public LinkedList<Point> waypoints { get; set; }
        public float movementSpeed { get; set; }
        public float direction { get; set; }
        public float rotation { get; set; }
        public float previousRotation { get; set; }
        public Boolean hasToMove { get; set; }
        #endregion

        public enum Type
        {
            Engineer,
            Melee,
            HeavyMelee,
            Fast,
            Ranged,
            HeavyRanged
        }

        public enum Job
        {
            Moving,
            Attacking,
            Defending,
            Patrolling,
            Idle
        }

        public enum Damage
        {
            Engineer = 4,
            Bowman = 15,
            Swordman = 35,
            Horseman = 15
        }

        public enum VisionRange
        {
            Engineer = 100,
            Bowman = 140,
            Swordman = 120,
            Horseman = 135
        }

        public abstract void Update(KeyboardState ks, MouseState ms);

        internal abstract void Draw(SpriteBatch sb);

        /// <summary>
        /// Repels other units that dare come close to our unit!
        /// </summary>
        public void CheckCollision()
        {
            collisionWith.Clear();
            foreach (Player player in Game1.GetInstance().players)
            {
                foreach (Unit unit in player.units)
                {
                    if (unit == this) continue;
                    //if (this.waypoints.Count != 0)
                    //{
                    // Check if the units are close enough
                    if (Util.GetHypoteneuseLength(this.GetLocation(), unit.GetLocation()) < this.collisionRadius + unit.collisionRadius)
                    {
                        // Sorry, we're invading someone else's space, we'll move over in a bit okay?
                        collisionWith.AddLast(unit);
                    }
                    //}
                }
            }
        }

        /// <summary>
        /// Gets a point location of this unit
        /// </summary>
        public Point GetLocation()
        {
            return new Point((int)x, (int)y);
        }

        /// <summary>
        /// Gets the cost of a unit
        /// </summary>
        /// <param name="t">The unit type of which the cost is wanted</param>
        /// <returns>The cost of the unit</returns>
        public static int GetCost(Type t)
        {
            switch (t)
            {
                case Type.Engineer:
                    return 100;

                case Type.Melee:
                    return 50;

                case Type.HeavyMelee:
                    return 75;

                case Type.Fast:
                    return 75;

                case Type.Ranged:
                    return 50;

                case Type.HeavyRanged:
                    return 100;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Updates the movement of this unit.
        /// </summary>
        protected void UpdateMovement()
        {
            if (this.waypoints.Count == 0)
            {
                return;
            }
            // Point target = this.waypoints.ElementAt(0);
            Move();
        }

        /// <summary>
        /// Set's the new Job for the unit
        /// </summary>
        /// <param name="newJob"></param>
        public void SetJob(Unit.Job newJob)
        {
            if (this.job != newJob)
            {
                // Console.WriteLine("Unit's Job updated to + " + newJob);
                this.job = newJob;
            }
        }

        /// <summary>
        /// Set the point this Unit has to move to.
        /// direction != direction is used for checking NaNExceptions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetMoveToTarget(int x, int y)
        {
            double a = Math.Abs(this.x - x);
            double b = Math.Abs(this.y - y);
            direction = (float)Math.Atan(a / b);
            if (direction != direction)
            {
                hasToMove = false;
                return;
            }
            hasToMove = true;
        }

        /// <summary>
        /// Updates the drawing position of this Unit.
        /// </summary>
        private void Move()
        {
            if (!hasToMove)
            {
                return;
            }

            Point waypoint = this.waypoints.FirstOrDefault();
            if (waypoint == null) return;

            SetMoveToTarget(waypoint.X, waypoint.Y);

            if (this.collisionWith.Count > 0)
            {
                // Uh-oh, we got a collision :(
                // Console.Out.WriteLine("Collision found");
                //float angle = Util.GetHypoteneuseAngleRad(this.GetLocation(), this.collisionWith.ElementAt(0).GetLocation());
                //Console.Out.WriteLine(direction);
                //this.direction = angle + (float)(90 * ( Math.PI / 180 ));
            }
            float timeSteppedSpeed = (float)(movementSpeed * GameTimeManager.GetInstance().time_step);

            float xSpeedDirection = timeSteppedSpeed * (float)Math.Sin(direction);
            float ySpeedDirection = timeSteppedSpeed * (float)Math.Cos(direction);

            if (x < waypoint.X && y < waypoint.Y)
            {
                x += xSpeedDirection;
                y += ySpeedDirection;
            }
            else if (x < waypoint.X && y > waypoint.Y)
            {
                x += xSpeedDirection;
                y -= ySpeedDirection;
            }
            else if (x < waypoint.X && y == waypoint.Y)
            {
                x += xSpeedDirection;
            }
            else if (x > waypoint.X && y < waypoint.Y)
            {
                x -= xSpeedDirection;
                y += ySpeedDirection;
            }
            else if (x > waypoint.X && y > waypoint.Y)
            {
                x -= xSpeedDirection;
                y -= ySpeedDirection;
            }
            else if (x > waypoint.X && y == waypoint.Y)
            {
                x -= xSpeedDirection;
            }
            else if (x == waypoint.X && y < waypoint.Y)
            {
                y += ySpeedDirection;
            }
            else if (x == waypoint.X && y > waypoint.Y)
            {
                y -= ySpeedDirection;
            }


            if (Math.Abs(x - waypoint.X) < (timeSteppedSpeed * 1.1) &&
                Math.Abs(y - waypoint.Y) < (timeSteppedSpeed * 1.1))
            {
                this.x = waypoint.X;
                this.y = waypoint.Y;
                if (waypoints.Count > 0)
                {
                    waypoints.RemoveFirst();
                    if (waypoints.Count > 0)
                    {
                        Point newTarget = waypoints.ElementAt(0);
                        SetMoveToTarget(newTarget.X, newTarget.Y);
                    }
                    else
                    {
                        SetJob(Job.Idle);
                    }
                }
                else
                {
                    hasToMove = false;
                }
            }

            // ONLY the current player may sync his units
            if (Game1.CURRENT_PLAYER == this.player && Game1.GetInstance().IsMultiplayerGame())
            {
                double now = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
                if (now - this.multiplayerData.lastPulse >
                    this.multiplayerData.updateRate)
                {
                    // Console.Out.WriteLine("Performing perodic unit update");
                    // We may get stacking queues if we dont do this
                    this.multiplayerData.lastPulse = now;
                    Synchronizer.GetInstance().QueueUnit(this);
                }
            }
        }

        /// <summary>
        /// Updates the enemiesInRange variable, to contain all the enemies within the attack range of this unit.
        /// </summary>
        public void CheckForEnemiesInRange(float rangeToCheck)
        {
            if (enemiesInRange != null)
            {
                enemiesInRange.Clear();
            }
            else
            {
                enemiesInRange = new LinkedList<Unit>();
                CheckForEnemiesInRange(rangeToCheck);
            }
            foreach (Player player in Game1.GetInstance().players)
            {
                // Don't check for units on our alliance
                if (player.alliance.members.Contains(this.player)) continue;
                else
                {
                    foreach (Unit unit in player.units)
                    {
                        if (Util.GetHypoteneuseLength(unit.GetLocation(), this.GetLocation()) < rangeToCheck)
                        {
                            enemiesInRange.AddLast(unit);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the buildingsInRange variable, to contain all the buildings within the attack range of this unit.
        /// </summary>
        public void CheckForBuildingsInRange(float rangeToCheck)
        {
            if (buildingsInRange != null)
            {
                buildingsInRange.Clear();
            }
            else
            {
                buildingsInRange = new LinkedList<Building>();
                CheckForBuildingsInRange(rangeToCheck);
            }
            foreach (Player player in Game1.GetInstance().players)
            {
                // Don't check for units on our alliance
                if (player.alliance.members.Contains(this.player)) continue;
                else
                {
                    foreach (Building building in player.buildings)
                    {
                        if (Util.GetHypoteneuseLength(building.GetLocation(), this.GetLocation()) < rangeToCheck)
                        {
                            buildingsInRange.AddLast(building);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves to a point in a while, by placing this unit and the point in a queue. When the time is there, process the pathfind
        /// This method is preferred to MoveToNow(Point p), as it doesn't cause a performance peek, but it may take a few frames before
        /// your path is ready.
        /// </summary>
        /// <param name="p">The point to move to, in a few frames depending on the queue</param>
        public void MoveToQueue(Point p)
        {
            if (p == this.GetLocation())
            {
                return;
            }

            if (p == Point.Zero) return;

            if (!Game1.GetInstance().IsMultiplayerGame())
            {
                if (
                !PathfindingProcessor.GetInstance().AlreadyInQueue(this))
                    PathfindingProcessor.GetInstance().Push(this, p);
            }
            else
            {
                if (this.multiplayerData.moveTarget == null ||
                    this.multiplayerData.moveTarget == Point.Zero)
                {
                    this.multiplayerData.moveTarget = this.GetLocation();
                }
                if (this.multiplayerData.receivedPathRequest
                    || this.player != Game1.CURRENT_PLAYER) PathfindingProcessor.GetInstance().Push(this, p);
                else
                {
                    this.multiplayerData.moveTarget = p;
                    Synchronizer.GetInstance().QueueUnit(this);
                }
                /*
                if (this.multiplayerData.moveTarget != Point.Zero)
                {
                    PathfindingProcessor.GetInstance().Push(this, p);
                    this.multiplayerData.moveTarget = Point.Zero;
                }
                else
                {
                    this.multiplayerData.moveTarget = p;
                    Console.Out.WriteLine("Queueing unit: " + p);
                    Synchronizer.GetInstance().QueueUnit(this);
                }
                */
            }
        }

        /// <summary>
        /// Calculates a path between the current unit and the point.
        /// </summary>
        /// <param name="p">The point to calculate to.</param>
        /// <returns>The list containing all the points that you should visit.</returns>
        public LinkedList<Point> CalculatePath(Point p)
        {
            LinkedList<Point> result = new LinkedList<Point>();
            long ticks = DateTime.UtcNow.Ticks;
            if (Game1.GetInstance().map.collisionMap.IsCollisionBetween(new Point((int)this.x, (int)this.y), p))
            {
                Game1 game = Game1.GetInstance();
                // Create temp nodes
                Node start = new Node(game.map.collisionMap, (int)this.x, (int)this.y, true);
                Node end = new Node(game.map.collisionMap, p.X, p.Y, true);
                LinkedList<PathfindingNode> nodes = new AStar(start, end).FindPath();
                if (nodes != null)
                {
                    // Remove the first node, because that's the node we're currently on ..
                    nodes.RemoveFirst();
                    // Clear our current waypoints
                    this.waypoints.Clear();
                    foreach (Node n in nodes)
                    {
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

        public void SetAssaultLocation(Point assaultPoint)
        {
            this.assaultPoint = assaultPoint;
        }

        /// <summary>
        /// Feel free to use "\ <(*^_^*)> /"
        /// </summary>
        /// <param name="p">The point to move to</param>
        public void MoveToNow(Point p)
        {
            if (p == this.GetLocation())
            {
                return;
            }
            if (Game1.GetInstance().IsMultiplayerGame())
            {

                PathfindingProcessor.GetInstance().Remove(this);

                this.multiplayerData.moveTarget = p;
                this.multiplayerData.receivedPathRequest = false;
                if ( Game1.CURRENT_PLAYER == this.player)
                {
                    // Console.Out.WriteLine("Queueing unit now: " + p);
                    Synchronizer.GetInstance().QueueUnit(this);
                }
            }
            this.waypoints = CalculatePath(p);
            if (this.waypoints.Count > 0)
            {
                Point newTarget = this.waypoints.First.Value;
                SetMoveToTarget(newTarget.X, newTarget.Y);
            }
            // Console.Out.WriteLine("Found path in " + ((DateTime.UtcNow.Ticks - ticks) / 10000) + "ms");
        }

        public Unit(Player p, int x, int y, float movementSpeed, float attackRange, float aggroRange, float rateOfFire)
        {
            this.player = p;
            this.x = x;
            this.y = y;
            this.z = 1f - ( this.player.units.Count + 1) * 0.0001f;
            this.movementSpeed = movementSpeed;
            this.attackRange = attackRange;
            this.aggroRange = aggroRange;
            this.rateOfFire = rateOfFire;

            this.rotation = 0f;
            this.previousRotation = 0f;

            this.hitting = false;
            this.hitFrame = 0;

            this.color = player.color;
            this.waypoints = new LinkedList<Point>();

            this.repelsOthers = true;
            this.collisionWith = new LinkedList<Unit>();
            this.enemiesInRange = new LinkedList<Unit>();
            this.friendliesProtectingMe = new LinkedList<Unit>();

            this.job = Job.Idle;

            healthBar = new HealthBar(this);

            this.currentHealth = 100;
            this.maxHealth = 100;

            this.player.units.AddLast(this);

            if (Game1.GetInstance().IsMultiplayerGame())
            {
                Boolean isLocal = this.player == Game1.CURRENT_PLAYER;
                this.multiplayerData = new UnitMultiplayerData(this, isLocal);
                if (isLocal)
                {
                    this.multiplayerData.RequestServerID();
                }

                if (this.multiplayerData != null)
                {
                    // Make sure everyone knows of this unit
                    Synchronizer.GetInstance().QueueUnit(this);
                }
            }
        }

        internal void DrawHealthBar(SpriteBatch sb)
        {
            healthBar.percentage = (int)((this.currentHealth / this.maxHealth) * 100.0);
            healthBar.Draw(sb);
        }

        void OnCollisionChangedListener.OnCollisionChanged(CollisionChangedEvent collisionEvent)
        {
            if (waypoints.Count > 0)
            {
                this.MoveToQueue(this.waypoints.ElementAt(this.waypoints.Count - 1));
            }
        }

        /// <summary>
        /// Defines the rectangle/hitbox for the Unit.
        /// With draw offset
        /// </summary>
        /// <returns></returns>
        public Rectangle DefineDrawRectangle()
        {
            return new Rectangle((int)x - (texture.Width / 2) - (int)Game1.GetInstance().drawOffset.X, (int)y - (texture.Height / 2) - (int)Game1.GetInstance().drawOffset.Y, texture.Width, texture.Height);
        }

        /// <summary>
        /// Defines the rectangle/hitbox for the Unit.
        /// without draw offset
        /// </summary>
        /// <returns></returns>
        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)x - (texture.Width / 2), (int)y - (texture.Height / 2), texture.Width, texture.Height);
        }
        

        public abstract void OnAggroRecieved(AggroEvent e);

        public abstract void OnAggro(AggroEvent e);

        public void OnDamage(DamageEvent e)
        {
            if (unitToStalk == null)
            {
                unitToStalk = e.source;
            }
            this.currentHealth -= e.damageDone;
            if (this.currentHealth <= 0 && !this.isDead)
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// Sets the target to attack
        /// </summary>
        /// <param name="unitToAttack"></param>
        public void AttackUnit(Unit unitToAttack)
        {
            this.buildingToDestroy = null;
            this.unitToDefend = null;
            this.unitToStalk = unitToAttack;
        }

        /// <summary>
        /// Sets the target to defend
        /// </summary>
        /// <param name="unitToDefend"></param>
        public void Defend(Unit unitToDefend)
        {
            this.buildingToDestroy = null;
            this.unitToStalk = null;
            this.unitToDefend = unitToDefend;
        }

        public void AttackBuilding(Building enemyBuilding)
        {
            this.unitToDefend = null;
            this.unitToStalk = null;
            this.buildingToDestroy = enemyBuilding;
        }

        public void TryToSwing()
        {
            if (unitToStalk != null)
            {
                if (Util.GetHypoteneuseLength(unitToStalk.GetLocation(), this.GetLocation()) < this.attackRange)
                {
                    this.waypoints.Clear();
                    if (!Game1.GetInstance().IsMultiplayerGame() ||
                        this.multiplayerData.isLocal)
                    {
                        Swing(unitToStalk);
                    }
                }
                else
                {
                    if (waypoints.Count < 1)
                    {
                        Point p = new Point((int)unitToStalk.x, (int)unitToStalk.y);
                        this.MoveToQueue(p);
                    }
                }
            }
            else if (buildingToDestroy != null)
            {
                if (Util.GetHypoteneuseLength(buildingToDestroy.GetLocation(), this.GetLocation()) < this.attackRange)
                {
                    this.waypoints.Clear();
                    if (!Game1.GetInstance().IsMultiplayerGame() ||
                        this.multiplayerData.isLocal)
                    {
                        Swing(buildingToDestroy);
                    }
                }
                else
                {
                    if (waypoints.Count < 1)
                    {
                        Point p = new Point((int)buildingToDestroy.x, (int)buildingToDestroy.y);
                        this.MoveToQueue(p);
                    }
                }
            }
        }
        /// <summary>
        /// This unit will attempt to fire/swing/kill/cast!
        /// </summary>
        public abstract void Swing(Damageable target);

        public void UpdateAttack()
        {
            if (((unitToStalk == null) && (buildingToDestroy == null) || isTargetDead()))
            {
                //get new target
                CheckForEnemiesInRange(this.aggroRange);
                if (this.enemiesInRange.Count > 0)
                {
                    unitToStalk = enemiesInRange.ElementAt(0);
                }
            }
            else
            {
                CheckForBuildingsInRange(this.aggroRange);
                if (this.buildingsInRange.Count > 0)
                {
                    buildingToDestroy = buildingsInRange.ElementAt(0);
                }
            }
        }

        public void UpdateDefense()
        {
            if (!isUnitDead(unitToDefend))
            {
                if (Util.GetHypoteneuseLength(unitToDefend.GetLocation(), this.GetLocation()) < this.attackRange)
                {
                    this.waypoints.Clear();
                }
                else
                {
                    if (waypoints.Count < 1)
                    {
                        Point p = new Point((int)unitToDefend.x, (int)unitToDefend.y);
                        this.MoveToQueue(p);
                    }
                }
            }
        }

        private Boolean isUnitDead(Unit unit)
        {
            if (unit != null && unit.isDead)
            {
                unit = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see wether the set target has died yet.
        /// </summary>
        private Boolean isTargetDead()
        {
            if (unitToStalk != null && unitToStalk.isDead)
            {
                unitToStalk = null;
                return true;
            }
            else if (buildingToDestroy != null && buildingToDestroy.isDestroyed)
            {
                buildingToDestroy = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Dispose of this unit.
        /// </summary>
        public void Dispose()
        {
            this.isDead = true;
            this.player.units.Remove(this);
            if (this.player.currentSelection != null)
            {
                this.player.currentSelection.units.Remove(this);
            }
        }

        /// <summary>
        /// Gets the draw rectangle of this unit.
        /// </summary>
        /// <returns>The draw rectangle</returns>
        public Rectangle GetDrawRectangle()
        {
            Game1 game = Game1.GetInstance(); 
            return new Rectangle(
                (int)(this.x - this.halfTextureWidth - game.drawOffset.X), 
                (int)(this.y - this.halfTextureHeight - game.drawOffset.Y),
                this.texture.Width, this.texture.Height);
        }
    }
}
