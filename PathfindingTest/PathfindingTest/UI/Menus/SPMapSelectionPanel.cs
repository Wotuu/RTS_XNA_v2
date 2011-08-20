using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.UI.Menus.Multiplayer.Panels;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.State;
using XNAInterfaceComponents.ParentComponents;
using PathfindingTest.UI.Menus.Multiplayer.Panels.PlayerLocation;

namespace PathfindingTest.UI.Menus
{
    public class SPMapSelectionPanel : MapSelectionPanel
    {
        public SPMapSelectionPanel()
            : base(null, "")
        {
            this.cancelBtn.onClickListeners += this.OnCancelClick;
            foreach (MapEntryPanel panel in this.panels)
            {
                int count = 0;
            }
        }

        public override void OnOKClick(XNAButton source)
        {
            if (this.GetSelectedMap() != null)
            {
                StateManager.GetInstance().gameState = StateManager.State.GameInit;
                MenuManager.GetInstance().ShowMenu(MenuManager.Menu.SinglePlayerLoadMenu);
                StateManager.GetInstance().gameState = StateManager.State.GameLoading;

                this.Dispose();
            }
            else XNAMessageDialog.CreateDialog("Please select a map first.", XNAMessageDialog.DialogType.OK);
        }

        public void OnCancelClick(XNAButton source)
        {
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MainMenu);
        }

        public override void Unload()
        {
            base.Unload();
            this.cancelBtn.onClickListeners -= this.OnCancelClick;
        }
    }
}
