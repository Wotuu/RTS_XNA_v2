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

namespace PathfindingTest.Units
{
    public abstract class Unit : OnCollisionChangedListener, Aggroable, Damageable
    {
        public Player player { get; set; }
        public float x { get; set; }
        public float y { get; set; }

        public Type type { get; set; }
        public Color color { get; set; }
        public Texture2D texture { get; set; }
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
        public int baseDamage { get; set; }

        public Boolean isDead = false;
        public Job job { get; set; }

        public Unit unitToStalk { get; set; }
        public Unit unitToDefend { get; set; }
        public float attackRange { get; set; }
        public float aggroRange { get; set; }
        public float fireCooldown { get; set; }
        public float rateOfFire { get; set; }

        public UnitMultiplayerData multiplayerData { get; set; }

        #region Movement variables
        public LinkedList<Point> waypoints { get; set; }
        public float movementSpeed { get; set; }
        private float direction { get; set; }
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
        /// Updates the movement of this unit.
        /// </summary>
        protected void UpdateMovement()
        {
            if (this.waypoints.Count == 0)
            {
                this.job = Unit.Job.Idle;
                return;
            }
            // Point target = this.waypoints.ElementAt(0);
            Move();
        }

        /// <summary>
        /// Set the point this Engineer has to move to.
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
            Point waypoint = this.waypoints.ElementAt(0);
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

            QuadRoot root = Game1.GetInstance().quadTree;
            root.GetQuadByPoint(this.GetLocation()).highlighted = true;
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
        /// Moves to a point in a while, by placing this unit and the point in a queue. When the time is there, process the pathfind
        /// This method is preferred to MoveToNow(Point p), as it doesn't cause a performance peek, but it may take a few frames before
        /// your path is ready.
        /// </summary>
        /// <param name="p">The point to move to, in a few frames depending on the queue</param>
        public void MoveToQueue(Point p)
        {
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
            if (Game1.GetInstance().collision.IsCollisionBetween(new Point((int)this.x, (int)this.y), p))
            {
                Game1 game = Game1.GetInstance();
                // Create temp nodes
                Node start = new Node(game.collision, (int)this.x, (int)this.y, true);
                Node end = new Node(game.collision, p.X, p.Y, true);
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

        /// <summary>
        /// Feel free to use "\ <(*^_^*)> /"
        /// </summary>
        /// <param name="p">The point to move to</param>
        public void MoveToNow(Point p)
        {
            if (Game1.GetInstance().IsMultiplayerGame())
            {
                PathfindingProcessor.GetInstance().Remove(this);
                this.multiplayerData.moveTarget = p;
                this.multiplayerData.receivedPathRequest = false;
                if (Game1.CURRENT_PLAYER == this.player)
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
            this.movementSpeed = movementSpeed;
            this.attackRange = attackRange;
            this.aggroRange = aggroRange;
            this.rateOfFire = rateOfFire;
            (this.quad = Game1.GetInstance().quadTree.GetQuadByPoint(this.GetLocation())).highlighted = true;

            this.color = player.color;
            this.waypoints = new LinkedList<Point>();

            this.repelsOthers = true;
            this.collisionWith = new LinkedList<Unit>();
            this.enemiesInRange = new LinkedList<Unit>();
            this.friendliesProtectingMe = new LinkedList<Unit>();

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
            if (this.currentHealth <= 0)
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// Sets the target to attack
        /// </summary>
        /// <param name="unitToAttack"></param>
        public void Attack(Unit unitToAttack)
        {
            this.unitToDefend = null;
            this.unitToStalk = unitToAttack;
        }

        /// <summary>
        /// Sets the target to defenc
        /// </summary>
        /// <param name="unitToDefend"></param>
        public void Defend(Unit unitToDefend)
        {
            this.unitToStalk = null;
            this.unitToDefend = unitToDefend;
        }

        public void TryToSwing()
        {
            if (Util.GetHypoteneuseLength(unitToStalk.GetLocation(), this.GetLocation()) < this.attackRange)
            {
                this.waypoints.Clear();
                if (!Game1.GetInstance().IsMultiplayerGame() ||
                    this.multiplayerData.isLocal)
                {
                    Swing();
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
        /// <summary>
        /// This unit will attempt to fire/swing/kill/cast!
        /// </summary>
        public abstract void Swing();

        public void UpdateAttack()
        {
            if (unitToStalk == null || isTargetDead())
            {
                //get new target
                CheckForEnemiesInRange(this.aggroRange);
                if (this.enemiesInRange.Count > 0)
                {
                    unitToStalk = enemiesInRange.ElementAt(0);
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
            return false;
        }

        /// <summary>
        /// Dispose of this unit.
        /// </summary>
        public void Dispose()
        {
            this.isDead = true;
            this.player.units.Remove(this);
            if (this.player.currentSelection != null) this.player.currentSelection.units.Remove(this);
        }
    }
}
