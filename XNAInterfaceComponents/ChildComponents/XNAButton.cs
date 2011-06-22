using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.Interfaces;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.AbstractComponents;

public delegate void OnButtonClick(XNAButton source);

namespace XNAInterfaceComponents.AbstractComponents
{
    public class XNAButton : ChildComponent, Focusable, MouseClickListener
    {
        public OnButtonClick onClickListeners { get; set; }

        public XNAButton(ParentComponent parent, Rectangle bounds, String text)
            : base(parent, bounds)
        {
            this.text = text;
            MouseManager.GetInstance().mouseClickedListeners += OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += OnMouseRelease;
        }

        /// <summary>
        /// Draws this button with a Z.
        /// </summary>
        /// <param name="sb">The spritebatch to draw on.</param>
        /// <param name="z">The Z, where 0 is front, and 1 is back</param>
        public void Draw(SpriteBatch sb, float z)
        {
            // Return if this component has no parent, or if it isn't visible
            if (this.parent == null || this.visible == false) return;
            // Get a clear texture if there aint any yet.
            if (this.clearTexture == null) clearTexture = ComponentUtil.GetClearTexture2D(sb);

            // Determine the drawcolor
            Color drawColor = new Color();
            if (this.isMouseOver) drawColor = this.mouseOverColor;
            else drawColor = this.backgroundColor;


            // Get the location on the screen on which to draw this button.
            Rectangle drawRect = this.GetScreenBounds();
            // Draw the button
            sb.Draw(clearTexture, drawRect, null, drawColor, 0, new Vector2(0,0), SpriteEffects.None, z);
            // Draw the border
            if (this.border != null) border.Draw(sb);
            // Draw the text on the button
            if (this.text != null)
            {
                Vector2 fontDimensions = this.font.MeasureString(this.text);
                sb.DrawString(font, this.text,
                    new Vector2(drawRect.X + (this.bounds.Width / 2) - (fontDimensions.X / 2),
                       drawRect.Y + (this.bounds.Height / 2) - (fontDimensions.Y / 2)), this.fontColor, 0,
                       new Vector2(0, 0), 1, SpriteEffects.None, z);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            this.Draw(sb, 1);
        }

        public override void Update()
        {

        }

        public void OnFocusReceived()
        {
            this.isFocussed = true;

            // Console.Out.WriteLine("XNA Button @ " + this.GetScreenLocation() + " grabbed focus!");
        }

        public void OnFocusLost()
        {
            this.isFocussed = false;

            // Console.Out.WriteLine("XNA Button @ " + this.GetScreenLocation() + " lost focus!");
        }

        public void OnMouseClick(MouseEvent m_event)
        {
            if (this.isFocussed && m_event.button == MouseEvent.MOUSE_BUTTON_1)
            {
                Point screenLocation = parent.RequestScreenLocation(new Point(this.bounds.X, this.bounds.Y));
                Rectangle screenRect = new Rectangle(screenLocation.X, screenLocation.Y, this.bounds.Width, this.bounds.Height);
                if (screenRect.Contains(m_event.location))
                {
                    if (this.onClickListeners != null) onClickListeners(this);
                }
            }
        }

        public void OnMouseRelease(MouseEvent m_event)
        {

        }

        public override void OnMouseEnter(MouseEvent m_event)
        {
            if( this.enabled ) this.isMouseOver = true;
            // Console.Out.WriteLine("XNA Button @ " + this.GetScreenLocation() + " mouse entered!");
        }

        public override void OnMouseExit(MouseEvent m_event)
        {
            if( this.enabled ) this.isMouseOver = false;
            // Console.Out.WriteLine("XNA Button @ " + this.GetScreenLocation() + " mouse exitted!");
        }

        public override void Unload()
        {
            MouseManager.GetInstance().mouseClickedListeners -= OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners -= OnMouseRelease;
        }
    }
}
