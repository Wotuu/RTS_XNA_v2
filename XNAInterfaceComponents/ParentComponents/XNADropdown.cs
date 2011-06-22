using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInputHandler.MouseInput;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.Components;

public delegate void OnOptionSelectedChanged();

namespace XNAInterfaceComponents.ParentComponents
{
    public class XNADropdown : ParentComponent
    {
        private XNAButton shownButton { get; set; }
        private int selectedIndex { get; set; }
        public OnOptionSelectedChanged onOptionSelectedChangedListeners { get; set; }
        public int dropdownLineSpace { get; set; }
        public int arrowSize { get; set; }

        public int optionHeight { get; set; }

        private Boolean isExpanded { get; set; }
        private Boolean _enabled { get; set; }
        public Boolean enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                foreach( ChildComponent c in this.children ){
                    c.enabled = value;
                }
                this._enabled = value;
            }
        }

        public XNADropdown(ParentComponent parent, Rectangle bounds)
            : base(parent, bounds)
        {
            this.enabled = true;
            optionHeight = this.bounds.Height;
            shownButton = new XNAButton(this, new Rectangle(
                0,
                0,
                this.bounds.Width,
            optionHeight), "");
            shownButton.onClickListeners += ShownButtonClicked;
            dropdownLineSpace = 30;
            arrowSize = 10;
        }

        /// <summary>
        /// Sets the background color of a specific menu item.
        /// </summary>
        /// <param name="index">The index to color.</param>
        /// <param name="c">The new color.</param>
        public void SetBackgroundColor(int index, Color c)
        {
            // Start at 1, because 1 is the root
            if (index < 1 && index >= this.children.Count) return;
            this.children.ElementAt(index).backgroundColor = c;
            this.children.ElementAt(index).mouseOverColor = c;
        }

        /// <summary>
        /// Always use this over the standard property 'backgroundColor'!
        /// </summary>
        /// <param name="c">The new color of this dropdown.</param>
        public void SetBackgroundColor(Color c)
        {
            Console.Out.WriteLine("Setting a new background color.");
            shownButton.backgroundColor = c;
            foreach (XNAButton button in this.children)
            {
                button.backgroundColor = c;
            }
            this.backgroundColor = c;
        }

        /// <summary>
        /// DO NOT USE THIS FUNCTION UNLESS YOU WANT TO HAVE A NICE FAT EXCEPTION.
        /// </summary>
        /// <param name="c">Fuck you</param>
        public new void AddChild(Component c)
        {
            throw new NotSupportedException("Use AddChild(String optionName) instead.");
        }

        /// <summary>
        /// DO NOT USE THIS FUNCTION UNLESS YOU WANT TO HAVE A NICE FAT EXCEPTION.
        /// </summary>
        /// <param name="c">Fuck you</param>
        public new Boolean RemoveChild(Component c)
        {
            throw new NotSupportedException("Use RemoveChild(String optionName) instead.");
        }

        /// <summary>
        /// Adds an option to the list.
        /// </summary>
        /// <param name="optionName">The option to add.</param>
        public void AddOption(String optionName)
        {
            XNAButton option = new XNAButton(this, new Rectangle(), optionName);
            option.onClickListeners += this.OnOptionSelected;
            option.backgroundColor = this.backgroundColor;
        }

        /// <summary>
        /// Removes an option from the list.
        /// </summary>
        /// <param name="optionName">The option to remove</param>
        public void RemoveOption(String optionName)
        {
            foreach (XNAButton button in this.children)
            {
                if (button.text == optionName)
                {
                    button.Unload();
                    this.children.Remove(button);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes an option by button.
        /// </summary>
        /// <param name="button">The button</param>
        public void RemoveButton(XNAButton button)
        {
            this.children.Remove(button);
            button.Unload();
        }

        /// <summary>
        /// Removes an option by index.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        public void RemoveIndex(int index)
        {
            Component c = this.children.ElementAt(index + 1);
            this.children.Remove(c);
            c.Unload();
        }

        /// <summary>
        /// When the user wants to expand the dropdown/
        /// </summary>
        /// <param name="source"></param>
        private void ShownButtonClicked(XNAButton source)
        {
            if (this.enabled) this.isExpanded = !this.isExpanded;
        }

        /// <summary>
        /// Called when an option is selected by the user.
        /// </summary>
        /// <param name="source"></param>
        private void OnOptionSelected(XNAButton source)
        {
            if (!this.isExpanded) return;
            for (int i = 0; i < this.children.Count; i++)
            {
                if (this.children.ElementAt(i) == source)
                {
                    if (i != selectedIndex)
                    {
                        SelectItem(i - 1);
                        if (onOptionSelectedChangedListeners != null)
                            onOptionSelectedChangedListeners();
                    }
                    this.isExpanded = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Selects an item.
        /// </summary>
        /// <param name="index">The index to select.</param>
        public void SelectItem(int index)
        {
            this.selectedIndex = index + 1;
            // +1 because the shownButton is a child too.
            XNAButton button = ((XNAButton)this.children.ElementAt(this.selectedIndex));
            shownButton.text = button.text;
            shownButton.backgroundColor = button.backgroundColor;
        }

        /// <summary>
        /// Gets the currently selected option.
        /// </summary>
        /// <returns>The selected option.</returns>
        public String GetSelectedOption()
        {
            return ((XNAButton)this.children.ElementAt(selectedIndex)).text;
        }

        /// <summary>
        /// Gets the button that was selected. (Yes, the Dropdown consists of buttons.)
        /// </summary>
        /// <returns>The button that is selected.</returns>
        public XNAButton GetSelectedButton()
        {
            return ((XNAButton)this.children.ElementAt(selectedIndex));
        }

        public override void Draw(SpriteBatch sb)
        {
            this.shownButton.Draw(sb);

            Rectangle drawLocation = this.GetScreenBounds();
            if (this.isExpanded)
            {
                for (int i = 1; i < this.children.Count; i++)
                {
                    XNAButton child = (XNAButton)this.children.ElementAt(i);
                    child.bounds = new Rectangle(
                        0,
                        (optionHeight * i) - (i), // minus i to prevent double borders in the center
                        drawLocation.Width,
                        optionHeight);
                    child.visible = true;
                    child.Draw(sb, 0.9f);
                }
            }
            else
            {
                for (int i = 1; i < this.children.Count; i++)
                {
                    this.children.ElementAt(i).visible = false;
                }
            }


            ComponentUtil.DrawLine(sb,
                new Point(drawLocation.Right - dropdownLineSpace, drawLocation.Y),
                new Point(drawLocation.Right - dropdownLineSpace, drawLocation.Y + this.optionHeight),
                Color.Black, 1);

            if (this.isExpanded)
            {
                ComponentUtil.DrawLine(sb,
                    new Point(drawLocation.Right - (dropdownLineSpace / 2),
                        drawLocation.Top + (this.optionHeight / 2) - (arrowSize / 2)),
                    new Point(drawLocation.Right - (dropdownLineSpace / 2) - (arrowSize / 2),
                        drawLocation.Top + (this.optionHeight / 2) + (arrowSize / 2)), this.border.color, 2);

                ComponentUtil.DrawLine(sb,
                    new Point(drawLocation.Right - (dropdownLineSpace / 2),
                        drawLocation.Top + (this.optionHeight / 2) - (arrowSize / 2)),
                    new Point(drawLocation.Right - (dropdownLineSpace / 2) + (arrowSize / 2),
                        drawLocation.Top + (this.optionHeight / 2) + (arrowSize / 2)), this.border.color, 2);
            }
            else
            {
                ComponentUtil.DrawLine(sb,
                    new Point(drawLocation.Right - (dropdownLineSpace / 2),
                        drawLocation.Top + (this.optionHeight / 2) + (arrowSize / 2)),

                    new Point(drawLocation.Right - (dropdownLineSpace / 2) - (arrowSize / 2),
                        drawLocation.Top + (this.optionHeight / 2) - (arrowSize / 2)), this.border.color, 2);

                ComponentUtil.DrawLine(sb,
                    new Point(drawLocation.Right - (dropdownLineSpace / 2),
                        drawLocation.Top + (this.optionHeight / 2) + (arrowSize / 2)),
                    new Point(drawLocation.Right - (dropdownLineSpace / 2) + (arrowSize / 2),
                        drawLocation.Top + (this.optionHeight / 2) - (arrowSize / 2)), this.border.color, 2);
            }
        }

        public override void Update()
        {

        }

        public override void OnMouseEnter(MouseEvent e)
        {
            // throw new NotImplementedException();
        }

        public override void OnMouseExit(MouseEvent e)
        {
            // throw new NotImplementedException();
        }

        public override void Unload()
        {
            this.shownButton.Unload();
            this.shownButton.onClickListeners -= this.ShownButtonClicked;
            foreach (XNAButton button in this.children)
            {
                button.Unload();
                button.onClickListeners -= this.OnOptionSelected;
            }
            this.children.Clear();
        }
    }
}
