using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.State;

namespace PathfindingTest.UI.Menus
{
    public class IngameMenu : XNAPanel
    {
        private int buttonWidth = 250;
        private int buttonHeight = 50;
        private int buttonSpacing = 40;

        public XNAButton resumeGameButton { get; set; }
        public XNAButton optionsButton { get; set; }
        public XNAButton exitGameButton { get; set; }
        public XNAButton exitWindowsButton { get; set; }

        public IngameMenu() :
            base(null, new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 200,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 200,
                400, 400))
        {
            // Always on top, somewhat
            this.z = 0.2f;

            this.resumeGameButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing, buttonWidth, buttonHeight), "Resume game");
            this.resumeGameButton.font = MenuManager.BUTTON_FONT;
            this.resumeGameButton.onClickListeners += this.OnResumeGame;

            this.optionsButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 2 + buttonHeight, buttonWidth, buttonHeight), "Options");
            this.optionsButton.font = MenuManager.BUTTON_FONT;
            //this.resumeGameButton.onClickListeners += this.o

            this.exitGameButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 3 + buttonHeight * 2, buttonWidth, buttonHeight), "Exit to main menu");
            this.exitGameButton.font = MenuManager.BUTTON_FONT;
            this.exitGameButton.onClickListeners += this.OnExitGame;

            this.exitWindowsButton = new XNAButton(this,
                new Rectangle((this.bounds.Width / 2) - (buttonWidth / 2),
                    buttonSpacing * 4 + buttonHeight * 3, buttonWidth, buttonHeight), "Exit to windows");
            this.exitWindowsButton.font = MenuManager.BUTTON_FONT;
            this.exitWindowsButton.onClickListeners += this.OnExitToWindows;
        }

        public void OnResumeGame(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.NoMenu);
            if (!Game1.GetInstance().IsMultiplayerGame()) StateManager.GetInstance().gameState = StateManager.State.GameRunning;
        }

        public void OnExitGame(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MainMenu);
            if (!Game1.GetInstance().IsMultiplayerGame()) StateManager.GetInstance().gameState = StateManager.State.MainMenu;
        }

        public void OnExitToWindows(XNAButton source)
        {
            Game1.GetInstance().Exit();
        }
    }
}
