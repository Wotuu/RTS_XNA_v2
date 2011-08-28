using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Managers;
using PathfindingTest.UI.Menus.Multiplayer;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using CustomLists.Lists;

namespace PathfindingTest.UI.Menus
{
    public class MenuManager
    {
        public CustomArrayList<ParentComponent> addedComponents { get; set; }
        public static SpriteFont BUTTON_FONT { get; set; }
        public static SpriteFont BIG_TEXTFIELD_FONT { get; set; }
        public static SpriteFont SMALL_TEXTFIELD_FONT { get; set; }
        public static SpriteFont PROGRESSBAR_FONT { get; set; }

        private static MenuManager instance;

        public enum Menu
        {
            MainMenu,
            SinglePlayerMapSelectionMenu,
            SinglePlayerLoadMenu,
            OptionsMenu,
            CreditsMenu,

            IngameMenu,

            MultiplayerLogin,
            TestConnection,
            MultiplayerLobby,
            GameLobby,
            MultiPlayerLoadMenu,


            NoMenu
        }

        /// <summary>
        /// Gets the menu that is currently displayed.
        /// </summary>
        /// <returns>The Parent component. Cast this one.</returns>
        public ParentComponent GetCurrentlyDisplayedMenu()
        {
            if (this.addedComponents.Count() == 0) return null;
            else return this.addedComponents.ElementAt(0);
        }

        public void ShowMenu(Menu menu)
        {
            for( int i = 0; i < this.addedComponents.Count(); i++ ){
                this.addedComponents.ElementAt(i).Unload();
            }
            addedComponents.Clear();
            if (menu == Menu.MainMenu)
            {
                this.addedComponents.AddLast(new MainMenu());
            }
            else if (menu == Menu.SinglePlayerMapSelectionMenu)
            {
                this.addedComponents.AddLast(new SPMapSelectionPanel());
            }
            else if (menu == Menu.SinglePlayerLoadMenu)
            {
                this.addedComponents.AddLast(new SPLoadScreen(
                    new Rectangle((Game1.GetInstance().graphics.PreferredBackBufferWidth / 2) - 200,
                         (Game1.GetInstance().graphics.PreferredBackBufferHeight / 2) - 200, 400, 250)));
            }
            else if (menu == Menu.OptionsMenu)
            {

            }
            else if (menu == Menu.CreditsMenu)
            {
                this.addedComponents.AddLast(new CreditsMenu());
            }
            else if (menu == Menu.IngameMenu)
            {
                this.addedComponents.AddLast(new IngameMenu());
            }
            else if (menu == Menu.MultiplayerLogin)
            {
                this.addedComponents.AddLast(new LoginScreen());
            }
            else if (menu == Menu.TestConnection)
            {
                this.addedComponents.AddLast(new TestConnectionMenu());
            }
            else if (menu == Menu.MultiplayerLobby)
            {
                this.addedComponents.AddLast(new MultiplayerLobby());
            }
            else if (menu == Menu.GameLobby)
            {
                this.addedComponents.AddLast(new GameLobby());
            }
            else if (menu == Menu.MultiPlayerLoadMenu)
            {
                this.addedComponents.AddLast(new MPLoadScreen());
            }
            else if (menu == Menu.NoMenu)
            {
                // Unload everything, just in case
                ComponentManager.GetInstance().UnloadAllPanels();
            }
        }

        private MenuManager()
        {
            addedComponents = new CustomArrayList<ParentComponent>();

            BUTTON_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/MenuButton");
            BIG_TEXTFIELD_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/BigMenuTextField");
            SMALL_TEXTFIELD_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/SmallMenuTextField");
            PROGRESSBAR_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/ProgressBarFont");
        }

        public static MenuManager GetInstance()
        {
            if (instance == null) instance = new MenuManager();
            return instance;
        }
    }
}
