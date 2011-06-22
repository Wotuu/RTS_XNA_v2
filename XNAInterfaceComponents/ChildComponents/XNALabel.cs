using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInputHandler.MouseInput;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.Interfaces;

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNALabel : ChildComponent, Drawable
    {
        public TextAlign textAlign = TextAlign.LEFT;
        public int paddingLeft = 5;
        public int paddingRight = 5;

        public enum TextAlign
        {
            LEFT,
            CENTER,
            RIGHT
        }


        public XNALabel(ParentComponent parent, Rectangle bounds, String text)
            : base(parent, bounds)
        {
            this.text = text;
            this.backgroundColor = Color.Transparent;
        }

        public override void Draw(SpriteBatch sb)
        {
            // Get a clear texture if there aint any yet.
            if (this.clearTexture == null) clearTexture = ComponentUtil.GetClearTexture2D(sb);
            // Return if this component has no parent, or if it isn't visible
            if (this.parent == null || this.visible == false) return;

            // Determine the drawcolor
            Color drawColor = new Color();
            if (this.isMouseOver) drawColor = this.mouseOverColor;
            else drawColor = this.backgroundColor;

            // Get the location on the screen on which to draw this button.
            Rectangle drawRect = this.GetScreenBounds();
            // Draw the label
            sb.Draw(clearTexture, drawRect, drawColor);
            // Draw the border
            if (this.border != null) border.Draw(sb);
            // Draw the text on the button
            if (this.text != null)
            {

                Vector2 fontDimensions = this.font.MeasureString(this.text);
                float drawY = drawRect.Y + (this.bounds.Height / 2) - (fontDimensions.Y / 2);
                if (this.textAlign == TextAlign.LEFT)
                {
                    sb.DrawString(font, this.text,
                        new Vector2( drawRect.X + this.padding.left, 
                            drawY), this.fontColor);
                }
                else if (this.textAlign == TextAlign.CENTER)
                {
                    sb.DrawString(font, this.text,
                        new Vector2(drawRect.X + (this.bounds.Width / 2) - (fontDimensions.X / 2),
                           drawY), this.fontColor);
                }
                else if (this.textAlign == TextAlign.RIGHT)
                {
                    sb.DrawString(font, this.text,
                        new Vector2(drawRect.X + this.bounds.Width - fontDimensions.X - this.padding.right,
                            drawY), this.fontColor);
                }
            }
        }

        public override void Update()
        {
            // throw new NotImplementedException();
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

        }
    }
}
