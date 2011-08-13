using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.ChildComponents;
using Microsoft.Xna.Framework;
using SocketLibrary.Users;

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class MPLoadScreen : XNAPanel
    {
        public XNAProgressBar[] progressBars;
        public XNALabel[] loadingWhatLabels;

        public XNALabel loadingLabel;

        private int itemHeight = 20;
        private int itemPadding = 10;
        private int userPadding = 20;

        public MPLoadScreen()
            : base(null, Rectangle.Empty)
        {
            int userCount = Game1.GetInstance().multiplayerGame.GetUserCount();
            int panelHeight = (((itemHeight * 2) + (itemPadding * 2) + userPadding) * userCount) + 40;
            progressBars = new XNAProgressBar[userCount];
            loadingWhatLabels = new XNALabel[userCount];

            this.bounds = new Rectangle((Game1.GetInstance().graphics.PreferredBackBufferWidth / 2) - 200,
            (Game1.GetInstance().graphics.PreferredBackBufferHeight / 2) - (panelHeight / 2),
            400, panelHeight);

            for (int i = 0; i < userCount; i++)
            {
                progressBars[i] = new XNAProgressBar(this,
                    new Rectangle(20, (i * itemHeight) + (i * itemPadding) + ((i + 1) * userPadding), this.bounds.Width - 40, itemHeight),
                    100);
                progressBars[i].progressDisplayLabel.font = MenuManager.BUTTON_FONT;
                progressBars[i].progressDisplayLabel.fontColor = Color.White;
                progressBars[i].currentValue = 0;
                progressBars[i].maxValue = 100;

                loadingWhatLabels[i] = new XNALabel(this,
                    new Rectangle(20, ((i + 1) * itemHeight) + ((i + 1) * itemPadding) + ((i + 1) * userPadding), 
                        this.bounds.Width - 40, itemHeight), "Startup..");
                loadingWhatLabels[i].textAlign = XNALabel.TextAlign.CENTER;
                loadingWhatLabels[i].border = null;
            }

            loadingLabel = new XNALabel(this, new Rectangle(20, this.bounds.Height - 30, this.bounds.Width - 40, itemHeight), "Loading..");
            loadingLabel.textAlign = XNALabel.TextAlign.CENTER;
            loadingLabel.border = null;
        }

        /// <summary>
        /// Sets the amount done for a certain user.
        /// </summary>
        /// <param name="user">The user to set.</param>
        /// <param name="percentage">The percentage to set to</param>
        public void SetPercentageDone(User user, int percentage)
        {
            int userCount = Game1.GetInstance().multiplayerGame.GetUserCount();
            for (int i = 0; i < userCount; i++)
            {
                if (Game1.GetInstance().multiplayerGame.GetUser(i) == user)
                {
                    this.progressBars[i].currentValue = percentage;
                    break;
                }
            }
        }

        /// <summary>
        /// Sets what the user is currently loading.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="what">What said user is loading.</param>
        public void SetLoadingWhat(User user, String what)
        {
            int userCount = Game1.GetInstance().multiplayerGame.GetUserCount();
            for (int i = 0; i < userCount; i++)
            {
                if (Game1.GetInstance().multiplayerGame.GetUser(i) == user)
                {
                    this.loadingWhatLabels[i].text = what;
                    break;
                }
            }
        }
    }
}
