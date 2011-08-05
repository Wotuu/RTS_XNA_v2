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
using PathfindingTest.Audio;
using PathfindingTest.Map;
using PathfindingTest.UI.Menus;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;

namespace PathfindingTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game, MouseClickListener, MouseMotionListener, KeyboardListener
    {

        public GraphicsDeviceManager graphics { get; set; }
        SpriteBatch spriteBatch;
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

        public int mapMoveSensitivity { get; set; }
        public GameMap map { get; set; }

        private Vector2 _drawOffset { get; set; }
        public Vector2 drawOffset
        {
            get
            {
                return _drawOffset;
            }
            set
            {
                if (map != null && map.collisionMap != null) map.collisionMap.drawOffset = value;
                this._drawOffset = value;
            }
        }

        public int maxLoadProgress { get; set; }
        public int currentLoadProgress { get; set; }
        public String loadingWhat { get; set; }


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


            drawOffset = Vector2.Zero;
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

            /*Texture2D[] texs = new Texture2D[] { 
                Content.Load<Texture2D>("Test/background"), 
            Content.Load<Texture2D>("Test/layer1"),
            Content.Load<Texture2D>("Test/layer2")};
            this.testTex = map.BlendTextures(texs, GameMap.BlendMode.PriorityBlend);

            Texture2D red = Content.Load<Texture2D>("Test/red");
            Texture2D green = Content.Load<Texture2D>("Test/green");
            Texture2D blue = Content.Load<Texture2D>("Test/blue");
            Texture2D black = Content.Load<Texture2D>("Test/black");

            texs = new Texture2D[] { red, green, blue, black };
            Texture2D[] subResults = new Texture2D[4];
            for (int i = 0; i < subResults.Length; i++)
            {
                subResults[i] = map.MergeTextures(texs);
            }
            this.testTex2 = map.MergeTextures(subResults);*/

            DrawUtil.lineTexture = this.Content.Load<Texture2D>("Misc/solid");
            font = Content.Load<SpriteFont>("Fonts/Arial");
            ChildComponent.DEFAULT_FONT = font;

            XNAMessageDialog.CLIENT_WINDOW_WIDTH = graphics.PreferredBackBufferWidth;
            XNAMessageDialog.CLIENT_WINDOW_HEIGHT = graphics.PreferredBackBufferHeight;
            graphics.PreferMultiSampling = true;

            StateManager.GetInstance().gameState = StateManager.State.MainMenu;

            SoundManager.GetInstance();

            KeyboardManager.GetInstance().keyPressedListeners += this.OnKeyPressed;
            KeyboardManager.GetInstance().keyReleasedListeners += this.OnKeyReleased;
            KeyboardManager.GetInstance().keyTypedListeners += this.OnKeyTyped;

            this.mapMoveSensitivity = 8;

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
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();
            StateManager sm = StateManager.GetInstance();

            // Update input
            MouseManager.GetInstance().Update(this);
            KeyboardManager.GetInstance().Update(Keyboard.GetState());

            // Update the SoundManager
            SoundManager.GetInstance().Update();

            // Updates all interface componentss
            ComponentManager.GetInstance().Update();
            switch (sm.gameState)
            {
                case StateManager.State.MainMenu:
                    // Play Music
                    SoundManager.GetInstance().PlayBGM(SoundManager.BGMType.Menu);
                    break;
                case StateManager.State.GameInit:
                    break;
                case StateManager.State.GameLoading:
                    SPLoadScreen loadingscreen = ((SPLoadScreen)MenuManager.GetInstance().GetCurrentlyDisplayedMenu());
                    loadingscreen.progressBar.maxValue = maxLoadProgress;
                    loadingscreen.progressBar.currentValue = currentLoadProgress;
                    loadingscreen.loadingWhatLabel.text = loadingWhat;


                    // Finished loading
                    if (maxLoadProgress != 0 && currentLoadProgress == maxLoadProgress)
                    {
                        // Notify the rest that we're done with loading
                        if (this.IsMultiplayerGame())
                        {
                            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.NoMenu);

                            ComponentManager.GetInstance().UnloadAllPanels();

                            Packet doneLoadingPacket = new Packet(Headers.DONE_LOADING);
                            doneLoadingPacket.AddInt(Game1.CURRENT_PLAYER.multiplayerID);
                            ChatServerConnectionManager.GetInstance().SendPacket(doneLoadingPacket);
                        }
                        else
                        {
                            StateManager.GetInstance().FinishedLoadingMap();
                        }
                    }
                    break;
                case StateManager.State.GameRunning:
                    // TODO: Add your update logic here

                    // Play Music
                    SoundManager.GetInstance().PlayBGM(SoundManager.BGMType.InGame);

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


                    // Use arrow keys to move the map
                    if (keyboardState.IsKeyDown(Keys.Left) && drawOffset.X > 0)
                    {
                        if (drawOffset.X - mapMoveSensitivity < 0)
                        {
                            this.drawOffset = new Vector2(0, this.drawOffset.Y);
                        }
                        else
                        {
                            this.drawOffset = new Vector2(this.drawOffset.X - mapMoveSensitivity, this.drawOffset.Y);
                        }
                    }
                    if (keyboardState.IsKeyDown(Keys.Right) && drawOffset.X + 1024 < map.collisionMap.mapWidth)
                    {
                        if (drawOffset.X + mapMoveSensitivity > map.collisionMap.mapWidth)
                        {
                            this.drawOffset = new Vector2(map.collisionMap.mapWidth, this.drawOffset.Y);
                        }
                        else
                        {
                            this.drawOffset = new Vector2(this.drawOffset.X + mapMoveSensitivity, this.drawOffset.Y);
                        }
                    }
                    if (keyboardState.IsKeyDown(Keys.Up) && drawOffset.Y > 0)
                    {
                        if (drawOffset.Y - mapMoveSensitivity < 0)
                        {
                            this.drawOffset = new Vector2(this.drawOffset.X, 0);
                        }
                        else
                        {
                            this.drawOffset = new Vector2(this.drawOffset.X, this.drawOffset.Y - mapMoveSensitivity);
                        }
                    }
                    if (keyboardState.IsKeyDown(Keys.Down) && drawOffset.Y + 768 < map.collisionMap.mapHeight)
                    {
                        if (drawOffset.Y + mapMoveSensitivity > map.collisionMap.mapHeight)
                        {
                            this.drawOffset = new Vector2(this.drawOffset.X, map.collisionMap.mapHeight);
                        }
                        else
                        {
                            this.drawOffset = new Vector2(this.drawOffset.X, this.drawOffset.Y + mapMoveSensitivity);
                        }
                    }


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
            /*try
            {*/
            GameTimeManager.GetInstance().OnStartDraw();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            // spriteBatch.Begin(SpriteSortMode.Immediate, null);

            StateManager sm = StateManager.GetInstance();

            // Draws all interface components
            ComponentManager.GetInstance().Draw(spriteBatch);

            if (sm.gameState == StateManager.State.MainMenu)
            {

            }
            else if (sm.gameState == StateManager.State.GameInit)
            {

            }
            else if (sm.gameState == StateManager.State.GameRunning ||
               sm.gameState == StateManager.State.GamePaused)
            {
                //quadTree.Draw(spriteBatch);
                // map.collisionMap.DrawMap(spriteBatch);
                map.Draw(spriteBatch);
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
            else if (sm.gameState == StateManager.State.GameShutdown)
            {

            }

            /*
            spriteBatch.Draw(testTex, new Rectangle(20, 20, 100, 100), Color.White);
            spriteBatch.Draw(testTex2, new Rectangle(120, 20, 160, 160), Color.White);
            */

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
            /*}
            catch (Exception e)
            {
                if (exceptionsCount < 0) exceptionsCount = 1;
                else exceptionsCount++;

                if (exceptionsCount > 3)
                    throw e; 
            }*/
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
                foreach (Node node in manager.nodeList)
                {
                    if (node.GetDrawRectangle().Contains(e.location))
                    {
                        node.Destroy();
                        manager.nodeList.Remove(node);
                        break;
                    }
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
                        if (!selectedANode && node.GetDrawRectangle().Contains(e.location))
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

        // private MouseEvent previousEvent { get; set; }
        void MouseMotionListener.OnMouseDrag(MouseEvent e)
        {

        }

        void MouseMotionListener.OnMouseMotion(MouseEvent e)
        {
            // previousEvent = null;
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
            Console.Error.WriteLine("Cannot find player with id = " + id);
            return null;
        }

        /// <summary>
        /// Checks whether the rectangle is on the screen or not.
        /// </summary>
        /// <param name="rect">The rectangle to check.</param>
        /// <returns>Yes or no.</returns>
        public Boolean IsOnScreen(Rectangle rect)
        {
            return rect.Intersects(new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
        }

        public void OnKeyPressed(KeyEvent e)
        {
            if (e.key == Keys.Escape)
            {
                MenuManager.GetInstance().ShowMenu(MenuManager.Menu.IngameMenu);
                if (!this.IsMultiplayerGame()) StateManager.GetInstance().gameState = StateManager.State.GamePaused;
            }
        }

        public void OnKeyTyped(KeyEvent e)
        {

        }

        public void OnKeyReleased(KeyEvent e)
        {

        }
    }
}
