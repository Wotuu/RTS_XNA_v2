using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.State;
using PathfindingTest.UI.Menus;
using XNAInterfaceComponents.ParentComponents;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using SocketLibrary.Users;

namespace PathfindingTest.UI
{
    public class MainMenu : XNAPanel
    {
        private int buttonWidth = 250;
        private int buttonHeight = 50;
        private int buttonSpacing = 25;

        public MainMenu()
            : base(null, new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 200,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 200,
                400, 400))
        {
            XNAButton startGameButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing, buttonWidth, buttonHeight), "Start Test Game");
            startGameButton.font = MenuManager.BUTTON_FONT;
            startGameButton.onClickListeners += this.StartGameClicked;

            XNAButton multiplayerButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 2 + buttonHeight, buttonWidth, buttonHeight), "Multiplayer");
            multiplayerButton.font = MenuManager.BUTTON_FONT;
            multiplayerButton.onClickListeners += this.MultiplayerClicked;

            XNAButton optionsButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 3 + buttonHeight * 2, buttonWidth, buttonHeight), "Options");
            optionsButton.font = MenuManager.BUTTON_FONT;
            optionsButton.onClickListeners += this.OptionsClicked;

            XNAButton creditsButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 4 + buttonHeight * 3, buttonWidth, buttonHeight), "Credits");
            creditsButton.font = MenuManager.BUTTON_FONT;
            creditsButton.onClickListeners += this.CreditsClicked;

            XNAButton exitButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 5 + buttonHeight * 4, buttonWidth, buttonHeight), "Exit Game");
            exitButton.font = MenuManager.BUTTON_FONT;
            exitButton.onClickListeners += this.ExitClicked;

            /*XNALabel label = new XNALabel(this, new Rectangle(10, 10, 100, 30), "Label test!");
            label.border = new Border(label, 1, Color.Red);
            label.textAlign = XNALabel.TextAlign.LEFT;
            XNAButton button = new XNAButton(this, new Rectangle(10, 50, 100, 40), "Click me");

            XNACheckBox checkBox = new XNACheckBox(this, new Rectangle(10, 110, 100, 20), "Checkbox test!");

            XNATextField textField = new XNATextField(this, new Rectangle(10, 140, 300, 60), 2);*/
        }

        public void StartGameClicked(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.SinglePlayerMapSelectionMenu);
        }

        public void MultiplayerClicked(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
        }

        public void OptionsClicked(XNAButton source)
        {

        }

        public void CreditsClicked(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.CreditsMenu);
        }

        public void ExitClicked(XNAButton source)
        {
            Game1.GetInstance().Exit();
        }

        public override string ToString()
        {
            return "MainMenu @ " + this.bounds;
        }
    }
}
