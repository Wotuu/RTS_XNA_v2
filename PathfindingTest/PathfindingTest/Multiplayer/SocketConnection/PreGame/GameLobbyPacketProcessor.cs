using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using SocketLibrary.Users;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.UI.Menus;
using PathfindingTest.UI.Menus.Multiplayer;
using SocketLibrary.Multiplayer;
using XNAInterfaceComponents.ParentComponents;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using PathfindingTest.UI.Menus.Multiplayer.Misc;
using PathfindingTest.Players;
using Microsoft.Xna.Framework;
using PathfindingTest.State;
using XNAInterfaceComponents.Managers;

namespace PathfindingTest.Multiplayer.PreGame.SocketConnection
{
    public class GameLobbyPacketProcessor
    {

        public void DataReceived(Packet p)
        {
            ChatServerConnectionManager manager = ChatServerConnectionManager.GetInstance();
            switch (p.GetHeader())
            {
                case Headers.NEW_USER:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        String username = PacketUtil.DecodePacketString(p, 4);
                        User user = new User(username);
                        user.id = userID;
                        user.channelID = manager.user.channelID;
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();


                        if (menu is GameLobby)
                        {
                            GameLobby lobby = ((GameLobby)menu);
                            lobby.UserJoined(user);
                        }

                        break;
                    }
                case Headers.USER_LEFT:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        User user = UserManager.GetInstance().GetUserByID(userID);
                        if (user != null)
                        {
                            ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                            if (menu is GameLobby)
                            {
                                GameLobby lobby = ((GameLobby)menu);
                                lobby.UserLeft(user);
                            }
                        }

                        break;
                    }
                // Client received an ID for creating a game.
                case Headers.GAME_ID:
                    {
                        int gameID = PacketUtil.DecodePacketInt(p, 0);

                        MultiplayerLobby lobby = ((MultiplayerLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu());
                        String gameName = lobby.gameNameInput.textfield.text;

                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.GameLobby);
                        GameLobby gameLobby = (GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu();

                        gameLobby.multiplayerGame = new MultiplayerGame(gameID,
                            gameName, "");
                        gameLobby.multiplayerGame.host = manager.user;
                        break;
                    }
                case Headers.SERVER_CREATE_GAME:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            // Confirmation that the game was created? idk
                        }
                        else if (menu is MultiplayerLobby)
                        {
                            MultiplayerGame game = new MultiplayerGame(
                                PacketUtil.DecodePacketInt(p, 0),
                                PacketUtil.DecodePacketString(p, 4),
                                "<No map selected yet>");
                            MultiplayerLobby lobby = (MultiplayerLobby)menu;
                            lobby.AddGame(game);
                        }
                        break;
                    }
                case Headers.SERVER_DESTROY_GAME:
                    {
                        int gameID = PacketUtil.DecodePacketInt(p, 0);

                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLobby);
                            XNAMessageDialog dialog =
                                XNAMessageDialog.CreateDialog("The host has disconnected.", XNAMessageDialog.DialogType.OK);
                            // When OK is pressed .. get back to the lobby.
                            dialog.button1.onClickListeners +=
                                delegate(XNAButton source)
                                {
                                    // Change channel
                                    Packet leftGamePacket = new Packet(Headers.CLIENT_LEFT_GAME);
                                    ChatServerConnectionManager.GetInstance().SendPacket(leftGamePacket);
                                };

                        }
                        else if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = (MultiplayerLobby)menu;
                            lobby.RemoveGameByID(gameID);
                        }
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = (MultiplayerLobby)menu;
                            MultiplayerGame game = lobby.GetGameByID(PacketUtil.DecodePacketInt(p, 0));
                            if (game != null)
                            {
                                game.mapname = PacketUtil.DecodePacketString(p, 4);
                            }
                        }
                        break;
                    }
                case Headers.SERVER_REQUEST_JOIN:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            GameLobby lobby = (GameLobby)menu;
                            Packet newPacket = new Packet();
                            if (lobby.IsFull()) newPacket.SetHeader(Headers.CLIENT_GAME_FULL);
                            else newPacket.SetHeader(Headers.CLIENT_OK_JOIN);
                            newPacket.AddInt(PacketUtil.DecodePacketInt(p, 0));
                            newPacket.AddInt(PacketUtil.DecodePacketInt(p, 4));
                            manager.SendPacket(newPacket);
                        }
                        break;
                    }
                case Headers.CLIENT_OK_JOIN:
                    {
                        // Packet newPacket = new Packet(Headers.CLIENT_CHANNEL);
                        // newPacket.AddInt(PacketUtil.DecodePacketInt(p, 0));
                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.GameLobby);
                        // manager.SendPacket(newPacket);

                        break;
                    }
                case Headers.CLIENT_GAME_FULL:
                    {
                        XNAMessageDialog.CreateDialog("The game is full.", XNAMessageDialog.DialogType.OK);
                        break;
                    }

                case Headers.GAME_COLOR_CHANGED:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        int colorID = PacketUtil.DecodePacketInt(p, 8);

                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            GameLobby lobby = (GameLobby)menu;
                            UserDisplayPanel panel = lobby.GetDisplayPanelByUserId(userID);
                            if (panel != null)
                            {
                                panel.SelectColor(colorID);
                            }
                            else
                            {
                                Console.Out.WriteLine("Tried to change the color of a user that doesn't exist!");
                            }
                        }

                        break;
                    }

                case Headers.GAME_TEAM_CHANGED:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        int teamID = PacketUtil.DecodePacketInt(p, 8);

                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            GameLobby lobby = (GameLobby)menu;
                            UserDisplayPanel panel = lobby.GetDisplayPanelByUserId(userID);
                            if (panel != null)
                            {
                                panel.teamDropdown.SelectItem(teamID - 1);
                            }
                            else
                            {
                                Console.Out.WriteLine("Tried to change the team of a user that doesn't exist!");
                            }
                        }
                        break;
                    }

                case Headers.GAME_READY_CHANGED:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 4);
                        int readyState = PacketUtil.DecodePacketInt(p, 8);

                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is GameLobby)
                        {
                            GameLobby lobby = (GameLobby)menu;
                            UserDisplayPanel panel = lobby.GetDisplayPanelByUserId(userID);
                            if (panel != null)
                            {
                                panel.readyCheckBox.selected = readyState != 0;
                            }
                            else
                            {
                                Console.Out.WriteLine("Tried to change the ready state of a user that doesn't exist!");
                            }
                        }
                        break;
                    }
                case Headers.GAME_KICK_CLIENT:
                    {
                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLobby);
                        XNAMessageDialog dialog =
                            XNAMessageDialog.CreateDialog("You have been kicked by the host.", XNAMessageDialog.DialogType.OK);

                        break;
                    }
                case Headers.SERVER_GAME_START:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        int seconds = PacketUtil.DecodePacketInt(p, 0);
                        if (seconds != 0)
                        {
                            if (menu is GameLobby)
                            {
                                GameLobby lobby = (GameLobby)menu;
                                lobby.AddMessageToLog("Game will start in " + seconds);
                                lobby.leaveGameButton.visible = false;
                            }
                        }
                        else
                        {
                            if (menu is GameLobby)
                            {

                                Game1.GetInstance().multiplayerGame = new MultiplayerGame(ChatServerConnectionManager.GetInstance().user.channelID,
                                    "<Gamename>", "<Mapname>");

                                StateManager.GetInstance().gameState = StateManager.State.GameInit;
                                MenuManager.GetInstance().ShowMenu(MenuManager.Menu.NoMenu);
                                StateManager.GetInstance().gameState = StateManager.State.GameRunning;
                                int count = 0;
                                foreach (Player player in Game1.GetInstance().players)
                                {
                                    player.SpawnStartUnits(new Point(200 * (count + 1), 200 * (count + 1)));
                                    count++;
                                }

                                ComponentManager.GetInstance().UnloadAllPanels();

                            }
                        }

                        break;
                    }
            }
        }
    }
}
