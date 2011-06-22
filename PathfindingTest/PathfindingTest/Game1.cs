using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using PathfindingTest.Units;
using System.Timers;
using PathfindingTest.Collision;
using PathfindingTest.SaveLoad;
using PathfindingTest.Selection;
using PathfindingTest.Selection.Patterns;
using PathfindingTest.Players;
using PathfindingTest.QuadTree;
using PathfindingTest.Pathfinding;
using AStarCollisionMap.Pathfinding;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.Managers;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ChildComponents;
using XNAInputLibrary.KeyboardInput;
using PathfindingTest.State;
using System.Collections;
using SocketLibrary.Packets;
using XNAInterfaceComponents.ParentComponents;
using SocketLibrary.Multiplayer;
using PathfindingTest.Multiplayer.Data;
using PathfindingTest.Misc;

namespace PathfindingTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game, MouseClickListener, MouseMotionListener
    {

        public GraphicsDeviceManager graphics { get; set; }
        SpriteBatch spriteBatch;
        public RTSCollisionMap collision { get; set; }
        public SpriteFont font { get; set; }
        public QuadRoot quadTree { get; set; }
        //
        private long previousFrameUpdateTime { get; set; }
        private int previousFrameUpdateFrames { get; set; }
        public int frames { get; set; }

        private long previousDrawUpdateTime { get; set; }
        private int previousDrawUpdateFrames { get; set; }
        private int draws { get; set; }

        public LinkedList<Player> players { get; set; }
        public static Player CURRENT_PLAYER { get; set; }

        public MultiplayerGame multiplayerGame { get; set; }
        public int objectsCreated { get; set; }

        public int exceptionsCount { get; set; }


        private static Game1 instance { get; set; }

        public static Game1 GetInstance()
        {
            return instance;
        }




        /// <summary>
        /// 
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content = new SynchronizedContentManager(this.Services);

            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            instance = this;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.ToggleFullScreen();
            graphics.ApplyChanges();
            this.InactiveSleepTime = new System.TimeSpan(0);
        }



        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            DrawUtil.lineTexture = this.Content.Load<Texture2D>("Misc/solid");
            font = Content.Load<SpriteFont>("Fonts/Arial");
            ChildComponent.DEFAULT_FONT = font;

            XNAMessageDialog.CLIENT_WINDOW_WIDTH = graphics.PreferredBackBufferWidth;
            XNAMessageDialog.CLIENT_WINDOW_HEIGHT = graphics.PreferredBackBufferHeight;

            (collision = new RTSCollisionMap(this, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)).PlaceNodesAroundEdges();
            graphics.PreferMultiSampling = true;

            StateManager.GetInstance().gameState = StateManager.State.MainMenu;

            base.Initialize();
        }

        #region Random Functions
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // try
            // {
                GameTimeManager.GetInstance().OnStartUpdate();
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                StateManager sm = StateManager.GetInstance();

                // Update input
                MouseManager.GetInstance().Update(this);
                KeyboardManager.GetInstance().Update(Keyboard.GetState());

                // Updates all interface componentss
                ComponentManager.GetInstance().Update();
                switch (sm.gameState)
                {
                    case StateManager.State.MainMenu:
                        break;
                    case StateManager.State.GameInit:
                        break;
                    case StateManager.State.GameRunning:
                        // TODO: Add your update logic here

                        // Update units
                        foreach (Player p in players)
                        {
                            p.Update(Keyboard.GetState(), Mouse.GetState());
                        }

                        // Update other random stuff?
                        KeyboardState keyboardState = Keyboard.GetState();
                        MouseState mouseState = Mouse.GetState();
                        if ((keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                            && mouseState.LeftButton == ButtonState.Pressed)
                        {
                            PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
                            if (manager.selectedNode != null)
                            {
                                manager.selectedNode.x = mouseState.X;
                                manager.selectedNode.y = mouseState.Y;
                            }
                        }

                        /*
                         * The NodeProcessor has a stack of Nodes. When popping a node, it calculates the connections.
                         * This is done to save a massive lagspike when updating the collision mesh!
                         */
                        //if (frames % 2 == 0) 

                        DateTime UtcNow = new DateTime(DateTime.UtcNow.Ticks);
                        DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
                        long timeStamp = (UtcNow - baseTime).Ticks / 10000;
                        if (timeStamp - previousFrameUpdateTime > 1000)
                        {
                            // Console.Out.WriteLine("Updates this second: " + (frames - previousFrameUpdateFrames) + ", slowly: " + gameTime.IsRunningSlowly);
                            previousFrameUpdateTime = timeStamp;
                            previousFrameUpdateFrames = frames;
                        }

                        if (IsMultiplayerGame()) Synchronizer.GetInstance().Synchronize();
                        // These two fill the rest of the frame, so they're supposed to go last.
                        SmartPathfindingNodeProcessor.GetInstance().Process();
                        PathfindingProcessor.GetInstance().Process();

                        frames++;
                        break;

                    case StateManager.State.GamePaused:
                        break;
                    case StateManager.State.GameShutdown:
                        break;

                    default: break;
                }
                base.Update(gameTime);
            /*}
            catch (Exception e)
            {
                if (exceptionsCount < 0) exceptionsCount = 1;
                else exceptionsCount++;

                if (exceptionsCount > 3) 
                    throw e; 
            }*/
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            try
            {
                GameTimeManager.GetInstance().OnStartDraw();
                GraphicsDevice.Clear(Color.CornflowerBlue);

                // TODO: Add your drawing code here
                spriteBatch.Begin(SpriteSortMode.Immediate, null);

                StateManager sm = StateManager.GetInstance();

                // Draws all interface components
                ComponentManager.GetInstance().Draw(spriteBatch);

                if (sm.gameState == StateManager.State.MainMenu)
                {

                }
                else if (sm.gameState == StateManager.State.GameInit)
                {

                }
                else if (sm.gameState == StateManager.State.GameRunning)
                {
                    //quadTree.Draw(spriteBatch);
                    collision.DrawMap(spriteBatch);

                    try
                    {
                        LinkedList<PathfindingNode> list = PathfindingNodeManager.GetInstance().nodeList;
                        foreach (Node node in list)
                        {
                            node.Draw(spriteBatch);
                        }

                        foreach (Player p in players)
                        {
                            p.Draw(this.spriteBatch);
                        }
                    }
                    catch (Exception e) { }
                }
                else if (sm.gameState == StateManager.State.GamePaused)
                {

                }
                else if (sm.gameState == StateManager.State.GameShutdown)
                {

                }




                spriteBatch.End();

                DateTime UtcNow = new DateTime(DateTime.UtcNow.Ticks);
                DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0);
                long timeStamp = (UtcNow - baseTime).Ticks / 10000;
                if (timeStamp - previousDrawUpdateTime > 1000)
                {
                    // Console.Out.WriteLine("Draws this second: " + (draws - previousDrawUpdateFrames));
                    previousDrawUpdateTime = timeStamp;
                    previousDrawUpdateFrames = draws;
                }

                draws++;

                base.Draw(gameTime);
            }
            catch (Exception e)
            {
                if (exceptionsCount < 0) exceptionsCount = 1;
                else exceptionsCount++;

                if (exceptionsCount > 3)
                    throw e; 
            }
        }

        void MouseClickListener.OnMouseClick(MouseEvent e)
        {
            if (e.button == MouseEvent.MOUSE_BUTTON_2)
            {
                /*PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
                foreach (Node node in manager.nodeList)
                {
                    node.selected = false;
                    foreach (PathfindingNodeConnection conn in node.connections)
                    {
                        conn.drawColor = PathfindingNodeConnection.TRANSPARENT;
                    }
                }


                foreach (Unit unit in UnitManager.GetInstance().GetSelectedUnits().units)
                {
                    unit.MoveTo(e.location);
                }*/
            }
            else if (e.button == MouseEvent.MOUSE_BUTTON_3)
            {
                PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
                // We need to capture it, because it will be reset after this most probably
                PathfindingNode selectedNode = manager.selectedNode;
                Node removedNode = null;
                foreach (Node node in manager.nodeList)
                {
                    if (node.drawRect.Contains(e.location))
                    {
                        manager.nodeList.Remove(node);
                        removedNode = node;
                        break;
                    }
                }
                foreach (Node node in PathfindingNodeManager.GetInstance().nodeList)
                {
                    if (removedNode == null) node.selected = false;
                    else node.RemoveConnection(removedNode);
                }
            }
        }

        void MouseClickListener.OnMouseRelease(MouseEvent e)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (e.button == MouseEvent.MOUSE_BUTTON_1)
            {
                if (CURRENT_PLAYER.selectBox != null)
                {
                    Boolean selectedANode = false;
                    PathfindingNodeManager manager = PathfindingNodeManager.GetInstance();
                    // We need to capture it, because it will be reset after this most probably
                    PathfindingNode selectedNode = manager.selectedNode;
                    foreach (Node node in manager.nodeList)
                    {
                        if (!selectedANode && node.drawRect.Contains(e.location))
                        {
                            // Update connections
                            Boolean controlDown = (keyboardState.IsKeyDown(Keys.RightControl) || keyboardState.IsKeyDown(Keys.LeftControl));
                            if (controlDown && selectedNode != null && selectedNode != node)
                            {
                                if (selectedNode.IsConnected(node) != null)
                                {
                                    selectedNode.RemoveConnection(node);
                                }
                                else
                                {
                                    PathfindingNodeConnection conn = new PathfindingNodeConnection(node, selectedNode);
                                    selectedNode.connections.AddLast(conn);
                                    node.connections.AddLast(conn);
                                }
                            }
                            //if (!controlDown) {
                            node.selected = true;
                            selectedANode = true;
                            //}
                        }
                        else node.selected = false;
                    }
                    // Console.Out.WriteLine("Left mouse button clicked!");
                    //if (!selectedANode)
                    //{
                    //    Texture2D nodeTexture = this.Content.Load<Texture2D>("Misc/node");
                    //    new Node(e.location.X, e.location.Y);
                    //}
                }
            }
        }

        void MouseMotionListener.OnMouseDrag(MouseEvent e)
        {

        }

        void MouseMotionListener.OnMouseMotion(MouseEvent e)
        {
            // Console.Out.WriteLine("Mouse moved!");

            ///if( e.location.X >= 10 && e.location.X <= graphics.PreferredBackBufferWidth - 10 &&
            ///    e.location.Y >= 10 && e.location.Y <= graphics.PreferredBackBufferHeight - 10)
            ///collision.UpdateCollisionMap(new Rectangle(e.location.X - 10, e.location.Y - 10, 20, 20), true);
        }

        /// <summary>
        /// Checks whether the current game is a multiplayer game or not.
        /// </summary>
        /// <returns>The flag.</returns>
        public Boolean IsMultiplayerGame()
        {
            return this.multiplayerGame != null;
        }

        /// <summary>
        /// Gets a player by multiplayer ID.
        /// </summary>
        /// <param name="id">The ID that you want.</param>
        /// <returns>The player that you need.</returns>
        public Player GetPlayerByMultiplayerID(int id)
        {
            foreach (Player player in players)
            {
                if (player.multiplayerID == id) return player;
            }
            return null;
        }
    }
}
