using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Managers;
using PathfindingTest.UI.Menus.Multiplayer;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingTest.UI.Menus
{
    public class MenuManager
    {
        public LinkedList<ParentComponent> addedComponents { get; set; }
        public static SpriteFont BUTTON_FONT { get; set; }
        public static SpriteFont BIG_TEXTFIELD_FONT { get; set; }
        public static SpriteFont SMALL_TEXTFIELD_FONT { get; set; }

        private static MenuManager instance;

        public enum Menu
        {
            MainMenu,
            OptionsMenu,


            MultiplayerLogin,
            MultiplayerLobby,
            GameLobby,


            NoMenu
        }

        /// <summary>
        /// Gets the menu that is currently displayed.
        /// </summary>
        /// <returns>The Parent component. Cast this one.</returns>
        public ParentComponent GetCurrentlyDisplayedMenu()
        {
            if (this.addedComponents.Count == 0) return null;
            else return this.addedComponents.ElementAt(0);
        }

        public void ShowMenu(Menu menu)
        {
            foreach( ParentComponent pc in addedComponents ){
                pc.Unload();
            }
            addedComponents.Clear();
            if (menu == Menu.MainMenu)
            {
                this.addedComponents.AddLast(new MainMenu());
            }
            else if (menu == Menu.OptionsMenu)
            {

            }
            else if (menu == Menu.MultiplayerLogin)
            {
                this.addedComponents.AddLast(new LoginScreen());
            }
            else if (menu == Menu.MultiplayerLobby)
            {
                this.addedComponents.AddLast(new MultiplayerLobby());
            }
            else if (menu == Menu.GameLobby)
            {
                this.addedComponents.AddLast(new GameLobby());
            }
            else if (menu == Menu.NoMenu)
            {
                // Unload everything, just in case
                ComponentManager.GetInstance().UnloadAllPanels();
            }
        }

        private MenuManager()
        {
            addedComponents = new LinkedList<ParentComponent>();

            BUTTON_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/MenuButton");
            BIG_TEXTFIELD_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/BigMenuTextField");
            SMALL_TEXTFIELD_FONT = Game1.GetInstance().Content.Load<SpriteFont>("Fonts/SmallMenuTextField");
        }

        public static MenuManager GetInstance()
        {
            if (instance == null) instance = new MenuManager();
            return instance;
        }
    }
}
