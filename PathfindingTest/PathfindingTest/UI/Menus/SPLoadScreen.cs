using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;

namespace PathfindingTest.UI.Menus
{
    public class SPLoadScreen : XNAPanel
    {
        public XNAProgressBar progressBar;
        public XNALabel loadingWhatLabel;

        public XNALabel loadingLabel;


        public SPLoadScreen(Rectangle bounds)
            : base(null, bounds)
        {
            progressBar = new XNAProgressBar(this, 
                new Rectangle(20, 30, this.bounds.Width - 40, 20),
                100);
            progressBar.progressDisplayLabel.font = MenuManager.BUTTON_FONT;
            progressBar.progressDisplayLabel.fontColor = Color.White;

            loadingWhatLabel = new XNALabel(this, new Rectangle(20, 80, this.bounds.Width - 40, 30), "Startup..");
            loadingWhatLabel.textAlign = XNALabel.TextAlign.CENTER;
            loadingWhatLabel.border = null;

            loadingLabel = new XNALabel(this, new Rectangle(20, this.bounds.Height - 30, this.bounds.Width - 40, 20), "Loading..");
            loadingLabel.textAlign = XNALabel.TextAlign.CENTER;
            loadingLabel.border = null;
            
        }
    }
}
