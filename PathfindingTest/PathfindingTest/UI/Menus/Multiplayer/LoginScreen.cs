using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;
using XNAInterfaceComponents.ChildComponents;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using XNAInterfaceComponents.ParentComponents;
using SocketLibrary.Packets;
using SocketLibrary.Users;
using PathfindingTest.Multiplayer.SocketConnection.InGame;

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class LoginScreen : XNAPanel
    {
        private XNALabel connectingLbl { get; set; }
        private XNATextField usernameTF { get; set; }
        private XNATextField ipTF { get; set; }

        public LoginScreen()
            : base(null, new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 200,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 200,
                400, 400))
        {
            XNALabel ipLbl = new XNALabel(this, new Rectangle(70, 70, 100, 30), "IP");
            ipLbl.border = null;
            ipLbl.textAlign = XNALabel.TextAlign.RIGHT;
            ipLbl.font = MenuManager.BIG_TEXTFIELD_FONT;

            ipTF = new XNATextField(this, new Rectangle(190, 70, 100, 35), 1);
            ipTF.border = new Border(ipTF, 1, Color.Blue);
            ipTF.font = MenuManager.BIG_TEXTFIELD_FONT;
            ipTF.text = "localhost";

            XNALabel usernameLbl = new XNALabel(this, new Rectangle(70, 110, 100, 30), "Username");
            usernameLbl.border = null;
            usernameLbl.textAlign = XNALabel.TextAlign.RIGHT;
            usernameLbl.font = MenuManager.BIG_TEXTFIELD_FONT;

            usernameTF = new XNATextField(this, new Rectangle(190, 110, 100, 35), 1);
            usernameTF.border = new Border(usernameTF, 1, Color.Blue);
            usernameTF.font = MenuManager.BIG_TEXTFIELD_FONT;

            connectingLbl = new XNALabel(this, new Rectangle(0, 160, 400, 30), "");
            connectingLbl.border = null;
            connectingLbl.textAlign = XNALabel.TextAlign.CENTER;
            connectingLbl.font = MenuManager.BUTTON_FONT;

            XNAButton loginButton = new XNAButton(this, new Rectangle(150, 210, 100, 40), "Log in");
            loginButton.font = MenuManager.BUTTON_FONT;
            loginButton.onClickListeners += this.LoginClicked;

            XNAButton backButton = new XNAButton(this, new Rectangle(150, 300, 100, 40), "Back");
            backButton.font = MenuManager.BUTTON_FONT;

            backButton.onClickListeners += this.BackClicked;
        }

        /// <summary>
        /// Sets the new connection status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void SetConnectionStatus(String status)
        {
            this.connectingLbl.text = status;
        }

        /// <summary>
        /// User wants to log in!
        /// </summary>
        /// <param name="source"></param>
        public void LoginClicked(XNAButton source)
        {
            if (this.usernameTF.text.Length < 2)
            {
                XNAMessageDialog.CreateDialog(
                    "Please enter a nickname of 3 characters or longer.", XNAMessageDialog.DialogType.OK);
            }
            else
            {
                GameServerConnectionManager.GetInstance().serverLocation = ipTF.text;

                ChatServerConnectionManager.GetInstance().serverLocation = ipTF.text;
                ChatServerConnectionManager.GetInstance().user = new User(usernameTF.text);
                ChatServerConnectionManager.GetInstance().ConnectToServer();
            }
        }

        /// <summary>
        /// User wants to go back to the main menu.
        /// </summary>
        /// <param name="source"></param>
        public void BackClicked(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MainMenu);
        }
    }
}
