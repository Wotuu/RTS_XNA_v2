using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ChildComponents;
using XNAInputLibrary.KeyboardInput;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using XNAInterfaceComponents.ParentComponents;
using SocketLibrary.Users;
using SocketLibrary.Multiplayer;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using PathfindingTest.UI.Menus.Multiplayer.Misc;

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class MultiplayerLobby : XNAPanel
    {
        private XNATextField messagesTextField { get; set; }
        private LinkedList<Message> messageLog = new LinkedList<Message>();

        public XNAButton disconnectButton { get; set; }
        public XNAButton createGameButton { get; set; }

        private XNATextField messageTextField { get; set; }
        private XNATextField usersField { get; set; }
        public XNAInputDialog gameNameInput { get; set; }

        public XNAPanel gamesPanel { get; set; }
        private LinkedList<GameDisplayPanel> gameList = new LinkedList<GameDisplayPanel>();


        public MultiplayerLobby()
            : base(null, new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 400,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 300,
                800, 600))
        {
            gamesPanel = new XNAPanel(this, new Rectangle(5, 5, 590, 330));
            gamesPanel.border = new Border(gamesPanel, 1, Color.Blue);



            XNAPanel usersPanel = new XNAPanel(this, new Rectangle(600, 5, 195, 330));
            usersPanel.border = new Border(usersPanel, 1, Color.Blue);

            usersField = new XNATextField(usersPanel, new Rectangle(5, 5, 185, 320), Int32.MaxValue);
            usersField.font = MenuManager.SMALL_TEXTFIELD_FONT;
            usersField.isEditable = false;




            XNAPanel messagesPanel = new XNAPanel(this, new Rectangle(5, 340, 790, 210));
            messagesPanel.border = new Border(messagesPanel, 1, Color.Blue);

            messagesTextField = new XNATextField(messagesPanel, new Rectangle(5, 5, 780, 170), Int32.MaxValue);
            messagesTextField.border = new Border(messagesTextField, 1, Color.Black);
            messagesTextField.font = MenuManager.SMALL_TEXTFIELD_FONT;
            messagesTextField.isEditable = false;

            messageTextField = new XNATextField(messagesPanel, new Rectangle(5, 180, 780, 25), 1);
            messageTextField.border = new Border(messageTextField, 1, Color.Black);
            messageTextField.font = MenuManager.SMALL_TEXTFIELD_FONT;
            messageTextField.onTextFieldKeyPressedListeners += this.OnKeyPressed;

            disconnectButton = new XNAButton(this,
                new Rectangle(this.bounds.Width - 105, this.bounds.Height - 45, 100, 40), "Disconnect");
            disconnectButton.onClickListeners += DisconnectBtnClicked;

            createGameButton = new XNAButton(this,
                new Rectangle(5, this.bounds.Height - 45, 100, 40), "Create Game");
            createGameButton.onClickListeners += CreateGameBtnClicked;

        }

        #region Game Management
        /// <summary>
        /// Gets a game by ID.
        /// </summary>
        /// <param name="gameID">The game ID.</param>
        /// <returns>The game or null</returns>
        public MultiplayerGame GetGameByID(int gameID)
        {
            foreach (GameDisplayPanel game in this.gameList)
            {
                if (game.multiplayerGame.id == gameID) return game.multiplayerGame;
            }
            return null;
        }

        /// <summary>
        /// User entered a game name and pressed OK
        /// </summary>
        /// <param name="source">The source</param>
        public void CreateGame(XNAButton source)
        {
            if (gameNameInput.textfield.text.Length < 4)
            {
                XNAMessageDialog.CreateDialog("Please enter a game name that is 4 characters or longer.", XNAMessageDialog.DialogType.OK);
                return;
            }
            Packet p = new Packet(Headers.CLIENT_CREATE_GAME);
            p.AddInt(ChatServerConnectionManager.GetInstance().user.id);
            p.AddString(gameNameInput.textfield.text);
            ChatServerConnectionManager.GetInstance().SendPacket(p);
        }

        /// <summary>
        /// Removes a game by game ID.
        /// </summary>
        /// <param name="id">The game ID to remove.</param>
        public void RemoveGameByID(int id)
        {
            for (int i = 0; i < this.gameList.Count; i++)
            {
                GameDisplayPanel panel = this.gameList.ElementAt(i);
                if (panel.multiplayerGame.id == id) this.RemoveGame(panel.multiplayerGame);
            }
        }

        /// <summary>
        /// Removes a game from the game list.
        /// </summary>
        /// <param name="toRemove">What game to remove!</param>
        public void RemoveGame(MultiplayerGame toRemove)
        {
            // Remove the panel.
            for (int i = 0; i < this.gameList.Count; i++)
            {
                GameDisplayPanel panel = this.gameList.ElementAt(i);
                if (panel.multiplayerGame == toRemove)
                {
                    this.gameList.Remove(panel);
                    panel.Unload();
                }
            }

            // Re-arrange the panels.
            for (int i = 0; i < this.gameList.Count; i++)
            {
                this.gameList.ElementAt(i).index = i;
            }
        }

        /// <summary>
        /// Adds a game to the lobby list
        /// </summary>
        /// <param name="toAdd">The game to add</param>
        public void AddGame(MultiplayerGame toAdd)
        {
            this.gameList.AddLast(new GameDisplayPanel(gamesPanel, this.gameList.Count, toAdd));
        }
        #endregion

        /// <summary>
        /// User wants to create a game.
        /// </summary>
        /// <param name="source">Bla</param>
        public void CreateGameBtnClicked(XNAButton source)
        {
            gameNameInput = XNAInputDialog.CreateDialog("Please enter the game name: ", XNAInputDialog.DialogType.OK_CANCEL);
            gameNameInput.button1.onClickListeners += CreateGame;
        }

        /// <summary>
        /// The user wants to disconnect.
        /// </summary>
        /// <param name="source"></param>
        public void DisconnectBtnClicked(XNAButton source)
        {
            ChatServerConnectionManager.GetInstance().DisconnectFromServer();
        }

        #region User Management
        /// <summary>
        /// Adds a user to the user log.
        /// </summary>
        /// <param name="user">The user to add</param>
        public void AddUser(User toAdd)
        {
            String result = "(" + UserManager.GetInstance().users.First.Value.id + ") " + UserManager.GetInstance().users.First.Value.username;
            for (int i = 1; i < UserManager.GetInstance().users.Count; i++)
            {
                User user = UserManager.GetInstance().users.ElementAt(i);
                result += "\n" + "(" + user.id + ") " + user.username;
            }
            usersField.text = result;
        }

        /// <summary>
        /// Removes a user from the list.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        public void RemoveUser(User toRemove)
        {
            UserManager.GetInstance().RemoveUserByID(toRemove.id);
            if (UserManager.GetInstance().users.Count == 0)
            {
                usersField.text = "No users for some odd reason.";
                return;
            }
            String result = "(" + UserManager.GetInstance().users.First.Value.id + ") " + UserManager.GetInstance().users.First.Value.username;
            for (int i = 1; i < UserManager.GetInstance().users.Count; i++)
            {
                User user = UserManager.GetInstance().users.ElementAt(i);
                result += "\n" + "(" + user.id + ") " + user.username;
            }
            usersField.text = result;
        }
        #endregion

        /// <summary>
        /// User pressed a key in the message textfield.
        /// </summary>
        /// <param name="e">The event</param>
        public void OnKeyPressed(KeyEvent e)
        {
            if (e.key.ToString() == "Enter")
            {
                ChatServerConnectionManager chat = ChatServerConnectionManager.GetInstance();
                String message = chat.user.username + ": " + messageTextField.text;
                messageTextField.text = "";
                // AddMessageToLog(message);
                Packet packet = new Packet();

                packet.SetHeader(Headers.CHAT_MESSAGE);
                packet.AddInt(chat.user.channelID);
                packet.AddString(message);
                chat.SendPacket(packet);
            }
        }

        public override void Unload()
        {
            base.Unload();
            if (gameNameInput != null) gameNameInput.Unload();
        }

        /// <summary>
        /// Adds a message to the log.
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessageToLog(String message)
        {
            messageLog.AddLast(new Message(message));
            String result = "";
            // If it isn't the first one..
            for (int i = 0; i < messageLog.Count; i++)
            {
                if (i != 0) result += "\n";
                result += messageLog.ElementAt(i).GetComposedMessage();
            }
            messagesTextField.text = result;
        }
    }
}