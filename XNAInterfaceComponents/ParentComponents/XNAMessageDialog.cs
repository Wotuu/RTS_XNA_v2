using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.Misc;
using Microsoft.Xna.Framework.Graphics;

namespace XNAInterfaceComponents.ParentComponents
{
    public class XNAMessageDialog : XNADialog
    {
        private XNALabel label { get; set; }
        private String message { get; set; }


        public XNAButton button1 { get; set; }
        public XNAButton button2 { get; set; }
        public XNAButton button3 { get; set; }
        private int buttonWidth { get; set; }
        private int buttonSpacing { get; set; }


        public DialogType type { get; set; }

        public enum DialogType
        {
            OK,
            OK_CANCEL,
            YES_CANCEL,
            YES_NO,
            YES_NO_CANCEL
        }

        /// <summary>
        /// Re-does the layout for this Message Dialog. Should be called when the user specifies a custom font
        /// rather than the the default font, or when the client window size changes.
        /// </summary>
        public override void DoLayout()
        {
            Vector2 messageDimensions = this.font.MeasureString(message);
            int windowHeight = (int)messageDimensions.Y + this.padding.top + this.padding.bottom + 50;

            this.bounds = new Rectangle((CLIENT_WINDOW_WIDTH / 2) - 200,
                (CLIENT_WINDOW_HEIGHT / 2) - (windowHeight / 2),
                400, windowHeight);

            this.label = new XNALabel(this, new Rectangle(
                0,
                this.padding.top,
                this.bounds.Width,
                (int)messageDimensions.Y), message);
            this.label.textAlign = XNALabel.TextAlign.CENTER;
            this.label.border = null;


            if (type == DialogType.OK)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2), this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "OK");
                this.button1.onClickListeners += this.Dispose;
            }
            else if (type == DialogType.YES_CANCEL || type == DialogType.YES_NO
                || type == DialogType.OK_CANCEL)
            {
                String[] buttonText = new String[2];
                if (type == DialogType.YES_CANCEL)
                {
                    buttonText[0] = "Yes";
                    buttonText[1] = "Cancel";
                }
                else if (type == DialogType.OK_CANCEL)
                {
                    buttonText[0] = "OK";
                    buttonText[1] = "Cancel";
                }
                else if (type == DialogType.YES_NO)
                {
                    buttonText[0] = "Yes";
                    buttonText[1] = "No";
                }
                button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth) - (this.buttonSpacing / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), buttonText[0]);
                this.button1.onClickListeners += this.Dispose;

                this.button2 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) + (this.buttonSpacing / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), buttonText[1]);
                this.button2.onClickListeners += this.Dispose;
            }
            else if (type == DialogType.YES_NO_CANCEL)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2) - this.buttonWidth - this.buttonSpacing,
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Yes");
                this.button1.onClickListeners += this.Dispose;

                this.button2 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "No");
                this.button2.onClickListeners += this.Dispose;

                this.button3 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) + (this.buttonWidth / 2) + this.buttonSpacing,
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Cancel");
                this.button3.onClickListeners += this.Dispose;
            }
        }

        private XNAMessageDialog(String message, DialogType type)
        {
            this.message = message;
            this.type = type;
            this.buttonWidth = 100;
            this.buttonSpacing = 30;

            this.DoLayout();
        }

        /// <summary>
        /// Creates a new message dialog. Note that you have to add your button listeners to this pane for it to do something.
        /// </summary>
        /// <param name="message">The message of the pane.</param>
        /// <param name="type">The type of the pane</param>
        /// <returns></returns>
        public static XNAMessageDialog CreateDialog(String message, DialogType type)
        {
            return new XNAMessageDialog(message, type);
        }
    }
}
