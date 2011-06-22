using System;
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

namespace PathfindingTest.State
{
    public class StateManager
    {
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
                    game.players = new LinkedList<Player>();

                    (game.quadTree = new QuadRoot(new Rectangle(0, 0,
                        game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight)
                        )).CreateTree(5);

                    if (game.IsMultiplayerGame())
                    {
                        GameServerConnectionManager.GetInstance().user =
                            ChatServerConnectionManager.GetInstance().user;
                        GameServerConnectionManager.GetInstance().ConnectToServer();


                        GameLobby lobby = (GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
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
                            Player player = new Player(alli, panel.GetSelectedColor());
                            player.multiplayerID = i;
                            if (panel.user.id == ChatServerConnectionManager.GetInstance().user.id)
                            {
                                Game1.CURRENT_PLAYER = player;
                            }
                            alli.members.AddLast(player);
                        }
                    }
                    else
                    {
                        Alliance redAlliance = new Alliance();
                        Player humanPlayer = new Player(redAlliance, Color.Red);
                        Game1.CURRENT_PLAYER = humanPlayer;
                        humanPlayer.SpawnStartUnits(new Point((int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2,
                            (int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2));


                        Alliance greenAlliance = new Alliance();
                        Player aiPlayer = new Player(greenAlliance, Color.Green);
                        aiPlayer.SpawnStartUnits(new Point((int)Game1.GetInstance().graphics.PreferredBackBufferWidth / 2, 200));

                        //SaveManager.GetInstance().SaveNodes("C:\\Users\\Wouter\\Desktop\\test.xml");
                    }
                   
                    MouseManager.GetInstance().mouseClickedListeners += ((MouseClickListener)game).OnMouseClick;
                    MouseManager.GetInstance().mouseReleasedListeners += ((MouseClickListener)game).OnMouseRelease;
                    MouseManager.GetInstance().mouseMotionListeners += ((MouseMotionListener)game).OnMouseMotion;
                    MouseManager.GetInstance().mouseDragListeners += ((MouseMotionListener)game).OnMouseDrag;
                }
                else if (value == State.GameRunning)
                {

                }
                else if (value == State.GamePaused)
                {

                }
                else if (value == State.GameShutdown)
                {
                    // Game1.GetInstance().multiplayerGame = null;
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
            GameRunning,
            GamePaused,
            GameShutdown
        }



        public static StateManager GetInstance()
        {
            if (instance == null) instance = new StateManager();
            return instance;
        }


    }
}
