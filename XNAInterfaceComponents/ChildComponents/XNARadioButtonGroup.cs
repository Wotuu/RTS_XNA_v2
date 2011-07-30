using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNARadioButtonGroup
    {
        public LinkedList<XNARadioButton> members = new LinkedList<XNARadioButton>();


        public XNARadioButtonGroup(){
        }

        public void OnSelect(XNARadioButton selectedButton)
        {
            foreach (XNARadioButton button in members)
            {
                if( button != selectedButton ) button.selected = false;
            }
        }

        /// <summary>
        /// Registers a member to this group.
        /// </summary>
        /// <param name="radioButton">The radio button to add.</param>
        public void RegisterMember(XNARadioButton radioButton)
        {
            radioButton.onClickListeners += OnSelect;
            this.members.AddLast(radioButton);
        }

        /// <summary>
        /// Unregisters a member to this group.
        /// </summary>
        /// <param name="radioButton">The radio button to remove.</param>
        public void UnregisterMember(XNARadioButton radioButton)
        {
            radioButton.onClickListeners -= OnSelect;
            this.members.Remove(radioButton);
        }
    }
}
