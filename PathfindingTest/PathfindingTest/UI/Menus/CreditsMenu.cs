using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.AbstractComponents;

namespace PathfindingTest.UI.Menus
{
    public class CreditsMenu : XNAPanel
    {
        public XNALabel programmingLbl { get; set; }
        public XNALabel programmingContributorsLbl { get; set; }

        public XNALabel thankyouLbl { get; set; }
        public XNALabel thankyouContributorsLbl { get; set; }

        public XNAButton backBtn { get; set; }


        public CreditsMenu()
            : base(null, new Rectangle(
                Game1.GetInstance().graphics.PreferredBackBufferWidth / 2 - 200,
                Game1.GetInstance().graphics.PreferredBackBufferHeight / 2 - 200,
                400, 400))
        {
            this.programmingLbl = new XNALabel(this, new Rectangle(10, 10, this.bounds.Width - 20, 30), "Programming:");
            this.programmingLbl.border = null;

            this.programmingContributorsLbl = new XNALabel(this, new Rectangle(40, 45, this.bounds.Width - 50, 30),
                "Joost den Boon\nEldin Hulsman\nWouter Koppenol\nMenno van Scheers");
            this.programmingContributorsLbl.border = null;


            this.thankyouLbl = new XNALabel(this, new Rectangle(10, this.bounds.Height - 145, this.bounds.Width - 20, 30), "Thanks to:");
            this.thankyouLbl.border = null;
            this.thankyouContributorsLbl = new XNALabel(this,
                new Rectangle(40, this.bounds.Height - 115, this.bounds.Width - 50, 30), "Ourselves");
            this.thankyouContributorsLbl.border = null;

            this.backBtn = new XNAButton(this, 
                new Rectangle(this.bounds.Width / 2 - 125, this.bounds.Height - 75, 250, 50), "Back");
            this.backBtn.onClickListeners += this.OnBackBtnClicked;
        }

        /// <summary>
        /// When the back button was clicked.
        /// </summary>
        /// <param name="source">XNAButton that was clicked</param>
        public void OnBackBtnClicked(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MainMenu);
        }
    }
}
