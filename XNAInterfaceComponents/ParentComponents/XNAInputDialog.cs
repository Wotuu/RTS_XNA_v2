using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;

namespace XNAInterfaceComponents.ParentComponents
{
    public class XNAInputDialog : XNADialog
    {
        private XNALabel label { get; set; }
        private String message { get; set; }
        public XNATextField textfield { get; set; }

        public XNAButton button1 { get; set; }
        public XNAButton button2 { get; set; }
        private int buttonWidth { get; set; }
        private int buttonSpacing { get; set; }


        public DialogType type { get; set; }

        public enum DialogType
        {
            OK,
            OK_CANCEL
        }

        private XNAInputDialog(String message, DialogType type)
            : base()
        {
            this.message = message;
            this.type = type;
            this.buttonWidth = 100;
            this.buttonSpacing = 30;

            this.DoLayout();
        }

        public override void DoLayout()
        {
            Vector2 messageDimensions = this.font.MeasureString(message);
            int windowHeight = (int)messageDimensions.Y + this.padding.top + this.padding.bottom + 50 + 40;

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

            this.textfield = new XNATextField(this, new Rectangle(
                (this.bounds.Width / 2) - 150,
                this.label.bounds.Height + this.padding.top + 5,
                300,
                (int)messageDimensions.Y + 10
                ), 1);

            if (type == DialogType.OK)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2), this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "OK");
                this.button1.onClickListeners += this.Dispose;
            }
            else if (type == DialogType.OK_CANCEL)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth) - (this.buttonSpacing / 2),
                    this.bounds.Height - 50,
                    this.buttonWidth, 40), "OK");
                this.button1.onClickListeners += this.Dispose;

                this.button2 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) + (this.buttonSpacing / 2),
                    this.bounds.Height - 50,
                    this.buttonWidth, 40), "Cancel");
                this.button2.onClickListeners += this.Dispose;
            }
        }

        /// <summary>
        /// Creates a new message dialog. Note that you have to add your button listeners to this pane for it to do something.
        /// </summary>
        /// <param name="message">The message of the pane.</param>
        /// <param name="type">The type of the pane</param>
        /// <returns></returns>
        public static XNAInputDialog CreateDialog(String message, DialogType type)
        {
            return new XNAInputDialog(message, type);
        }
    }
}
