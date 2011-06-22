using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.UI.Menus;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.UI.Menus.Multiplayer;
using SocketLibrary.Users;

namespace PathfindingTest.Multiplayer.PreGame.SocketConnection
{
    public class ChatPacketProcessor
    {
        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case Headers.KEEP_ALIVE:
                    {
                        ChatServerConnectionManager.GetInstance().SendPacket(new Packet(Headers.KEEP_ALIVE));
                        break;
                    }
                case Headers.HANDSHAKE_2:
                    {
                        ChatServerConnectionManager.GetInstance().SetLoginStatus("Connected");
                        // Finish handshake
                        ChatServerConnectionManager.GetInstance().SendPacket(new Packet(Headers.HANDSHAKE_3));

                        // Request for a user ID
                        Packet newPacket = new Packet(Headers.CLIENT_USERNAME);
                        newPacket.AddString(ChatServerConnectionManager.GetInstance().user.username);
                        ChatServerConnectionManager.GetInstance().SendPacket(newPacket);

                        MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLobby);
                        break;
                    }
                case Headers.SERVER_DISCONNECT:
                    {
                        ChatServerConnectionManager.GetInstance().DisconnectFromServer();
                        // Create a dialog, and add a listener to the OK button.
                        ChatServerConnectionManager.GetInstance().OnDisconnect();
                        break;
                    }
                case Headers.CLIENT_USER_ID:
                    {
                        // Set the received user ID.
                        ChatServerConnectionManager.GetInstance().user.id = PacketUtil.DecodePacketInt(p, 0);
                        // Console.Out.WriteLine("Received user ID from the server: " + ChatServerConnectionManager.GetInstance().user.id);
                        break;
                    }
                case Headers.CLIENT_CHANNEL:
                    {
                        UserManager.GetInstance().users.Clear();
                        // UserManager.GetInstance().users.Clear();
                        ChatServerConnectionManager.GetInstance().user.channelID = PacketUtil.DecodePacketInt(p, 0);
                        // Console.Out.WriteLine("Switched channel to: " + ChatServerConnectionManager.GetInstance().user.channelID);
                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        // Get the channel
                        int channel = PacketUtil.DecodePacketInt(p, 0);
                        // Get the message
                        String message = PacketUtil.DecodePacketString(p, 4);
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = ((MultiplayerLobby)menu);
                            lobby.AddMessageToLog(message);
                        }
                        else if (menu is GameLobby)
                        {
                            GameLobby lobby = ((GameLobby)menu);
                            lobby.AddMessageToLog(message);
                        }
                        break;
                    }
                case Headers.NEW_USER:
                    {
                        int userID = PacketUtil.DecodePacketInt(p, 0);
                        String username = PacketUtil.DecodePacketString(p, 4);
                        User user = new User(username);
                        user.id = userID;
                        user.channelID = ChatServerConnectionManager.GetInstance().user.channelID;
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (UserManager.GetInstance().GetUserByID(user.id) == null) UserManager.GetInstance().users.AddLast(user);

                        if (menu is MultiplayerLobby)
                        {
                            MultiplayerLobby lobby = ((MultiplayerLobby)menu);
                            lobby.AddUser(user);
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
                            if (menu is MultiplayerLobby)
                            {
                                MultiplayerLobby lobby = ((MultiplayerLobby)menu);
                                lobby.RemoveUser(user);
                            }
                        }

                        break;
                    }
            }
        }
    }
}
