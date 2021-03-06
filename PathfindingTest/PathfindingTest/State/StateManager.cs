﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PathfindingTest.QuadTree;
using PathfindingTest.Players;
using XNAInputHandler.MouseInput;
using PathfindingTest.UI.Menus;
using PathfindingTest.UI.Menus.Multiplayer;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Multiplayer;
using SocketLibrary.Users;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using PathfindingTest.Multiplayer.SocketConnection.InGame;
using PathfindingTest.Buildings;
using PathfindingTest.Collision;
using PathfindingTest.Map;
using System.Threading;
using CustomLists.Lists;

namespace PathfindingTest.State
{
    public class StateManager
    {
        private String mapToLoad { get; set; }

        private Point[] tempPlayerLocations { get; set; }

        private State _gameState { get; set; }
        public State gameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                if (value == State.MainMenu)
                {
                    MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MainMenu);
                }
                else if (value == State.GameInit)
                {
                    Game1 game = Game1.GetInstance();
                    game.players = new CustomArrayList<Player>();

                    String mapname = "";
                    if (game.IsMultiplayerGame())
                    {
                        mapname = ((GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu()).mapPreviewPanel.selectedMapLbl.text;
                        this.PrepareMultiplayerGame();
                    }
                    else
                    {
                        SPMapSelectionPanel panel = ((SPMapSelectionPanel)MenuManager.GetInstance().GetCurrentlyDisplayedMenu());
                        mapname = panel.GetSelectedMap();
                        tempPlayerLocations = new Point[] { panel.GetSelectedMapEntry().mapPlayerLocationGroup.GetMapLocationByButtonTextNumber(1) };
                    }
                    this.mapToLoad = mapname;
                    if (!mapToLoad.EndsWith(".xml")) this.mapToLoad += ".xml";
                    new Thread(this.LoadMap).Start();
                }
                else if (value == State.GameLoading)
                {

                }
                else if (value == State.GameRunning)
                {
                    MenuManager.GetInstance().ShowMenu(MenuManager.Menu.NoMenu);
                }
                else if (value == State.GamePaused)
                {

                }
                else if (value == State.GameShutdown)
                {

                }

                Console.Out.WriteLine("State is now " + value.ToString());
                _gameState = value;
            }
        }
        private static StateManager instance;
        private StateManager() { }

        public enum State
        {
            MainMenu,
            GameInit,
            GameLoading,
            GameRunning,
            GamePaused,
            GameShutdown
        }



        public static StateManager GetInstance()
        {
            if (instance == null) instance = new StateManager();
            return instance;
        }


        /// <summary>
        /// Used for a-synchronous loading of the map.
        /// </summary>
        /// <param name="name">The name of the map to load.</param>
        private void LoadMap()
        {
            Game1.GetInstance().TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000 / 10f) );
            Game1 game = Game1.GetInstance();
            game.maxLoadProgress += 5000;
            game.map = new GameMap(this.mapToLoad);
            // game.map.collisionMap.PlaceNodesAroundEdges();

            (game.quadTree = new QuadRoot(new Rectangle(0, 0,
                game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight)
                )).CreateTree(5);
            game.currentLoadProgress += 5000;
            if (game.IsMultiplayerGame())
            {
                GameServerConnectionManager.GetInstance().user =
                    ChatServerConnectionManager.GetInstance().user;

                GameServerConnectionManager.GetInstance().serverLocation =
                    ChatServerConnectionManager.GetInstance().serverLocation;

                GameServerConnectionManager.GetInstance().ConnectToServer();
            }
            Game1.GetInstance().TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000 / 60f));
        }

        /// <summary>
        /// Should be called when the map is finished with loading.
        /// </summary>
        public void FinishedLoadingMap()
        {
            Game1 game = Game1.GetInstance();
            if (game.IsMultiplayerGame())
            {
                // Do nothing, used to be something here
            }
            else
            {
                Alliance redAlliance = new Alliance();
                Player humanPlayer = new Player(redAlliance, Color.Blue, this.tempPlayerLocations[0]);
                Game1.CURRENT_PLAYER = humanPlayer;
                humanPlayer.SpawnStartUnits();


                Alliance greenAlliance = new Alliance();
                Player aiPlayer = new Player(greenAlliance, Color.Purple,
                    new Point((int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2, 200));
                aiPlayer.SpawnStartUnits();

                StateManager.GetInstance().gameState = StateManager.State.GameRunning;
                //SaveManager.GetInstance().SaveNodes("C:\\Users\\Wouter\\Desktop\\test.xml");
            }

            game.map.miniMap = new MiniMap(game.map);
            game.map.miniMap.CreateMiniMap(true);

            game.drawOffset = new Vector2(Game1.CURRENT_PLAYER.startLocation.X - ( game.graphics.PreferredBackBufferWidth / 2 ),
                Game1.CURRENT_PLAYER.startLocation.Y - ( game.graphics.PreferredBackBufferHeight / 2 ));

            MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)game).OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)game).OnMouseRelease;
            MouseManager.GetInstance().mouseMotionListeners += ((MouseMotionListener)game).OnMouseMotion;
            MouseManager.GetInstance().mouseDragListeners += ((MouseMotionListener)game).OnMouseDrag;
        }

        /// <summary>
        /// Prepares the multiplayer game
        /// </summary>
        public void PrepareMultiplayerGame()
        {
            GameLobby lobby = (GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
            if (ChatServerConnectionManager.GetInstance().user.username == "Testy")
            {
                // Wait 5 seconds, this is purely for debugging to prevent I/O exceptions when
                // 2 local clients are accessing the same files.
                Thread.Sleep(2000);
            }
            foreach (User user in UserManager.GetInstance().users)
            {
                Game1.GetInstance().multiplayerGame.AddUser(user);
            }

            Alliance[] alliances = new Alliance[8];
            for (int i = 0; i < alliances.Length; i++)
            {
                alliances[i] = new Alliance();
            }

            for (int i = 0; i < lobby.GetDisplayPanelCount(); i++)
            {
                UserDisplayPanel panel = lobby.GetDisplayPanel(i);
                Alliance alli = alliances[Int32.Parse(panel.teamDropdown.GetSelectedOption()) - 1];
                Player player = new Player(alli, panel.GetSelectedColor(),
                    lobby.mapPreviewPanel.playerLocationGroup.GetMapLocationByButtonTextNumber(i + 1));
                player.multiplayerID = panel.user.id;
                if (panel.user.id == ChatServerConnectionManager.GetInstance().user.id)
                {
                    Game1.CURRENT_PLAYER = player;
                }
                alli.members.AddLast(player);
            }
        }
    }
}
