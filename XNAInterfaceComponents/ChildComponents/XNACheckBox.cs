using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.Interfaces;

public delegate void OnCheckBoxClick(XNACheckBox source);

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNACheckBox : ChildComponent, Drawable, MouseClickListener
    {
        public OnCheckBoxClick onClickListeners { get; set; }

        public TextAlign textAlign { get; set; }
        public Vector2 checkBoxSize { get; set; }
        public Boolean selected { get; set; }

        public int crossPadding { get; set; }

        public enum TextAlign
        {
            LEFT,
            RIGHT
        }

        public XNACheckBox(ParentComponent parent, Rectangle bounds, String text)
            : base(parent, bounds)
        {
            this.text = text;
            this.textAlign = TextAlign.RIGHT;
            this.checkBoxSize = new Vector2(20, 20);
            this.crossPadding = 3;

            MouseManager.GetInstance().mouseClickedListeners += OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += OnMouseRelease;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!this.visible) return;
            if (this.clearTexture == null) clearTexture = ComponentUtil.GetClearTexture2D(sb);


            Color drawColor = new Color();
            if (this.isMouseOver) drawColor = this.mouseOverColor;
            else drawColor = this.backgroundColor;

            Rectangle drawLocation = this.GetScreenBounds();

            // Draw the background
            sb.Draw(this.clearTexture, drawLocation, drawColor);
            
            // Determine where the checkbox should go
            Rectangle checkBoxBounds = new Rectangle();
            if (this.textAlign == TextAlign.RIGHT)
            {
                checkBoxBounds = new Rectangle((int)drawLocation.X, (int)drawLocation.Y,
                (int)this.checkBoxSize.X, (int)this.checkBoxSize.Y);
            }
            else if (this.textAlign == TextAlign.LEFT)
            {
                checkBoxBounds = new Rectangle((int)(drawLocation.X + this.bounds.Width - this.checkBoxSize.X),
                    (int)(drawLocation.Y + this.bounds.Height - this.checkBoxSize.Y),
                (int)this.checkBoxSize.X, (int)this.checkBoxSize.Y);
            }
            // Draw the outer line of the checkbox
            ComponentUtil.DrawClearRectangle(sb, checkBoxBounds, 1, Color.Black);
            // Draw the cross, if selected
            if (this.selected)
            {
                DrawCross(sb, checkBoxBounds);
            }
            // Draw the text
            if (this.textAlign == TextAlign.LEFT)
            {
                sb.DrawString(this.font, this.text,
                    new Vector2(drawLocation.X + this.padding.left,
                        drawLocation.Y + (drawLocation.Height / 2) - (this.font.MeasureString(this.text).Y / 2)), 
                        this.fontColor);
            }
            else if (this.textAlign == TextAlign.RIGHT)
            {
                sb.DrawString(this.font, this.text,
                    new Vector2(drawLocation.X + this.padding.left + this.checkBoxSize.X,
                        drawLocation.Y + (drawLocation.Height / 2) - (this.font.MeasureString(this.text).Y / 2)),
                        this.fontColor);
            }
        }

        private void DrawCross(SpriteBatch sb, Rectangle checkBoxBounds)
        {
            ComponentUtil.DrawLine(sb,
                new Point(checkBoxBounds.X + this.crossPadding, checkBoxBounds.Y + this.crossPadding),
                new Point(checkBoxBounds.X + checkBoxBounds.Width - this.crossPadding,
                    checkBoxBounds.Y + checkBoxBounds.Height - this.crossPadding),
                    Color.Blue, 1);
            ComponentUtil.DrawLine(sb,
                new Point(checkBoxBounds.X + checkBoxBounds.Width - this.crossPadding, checkBoxBounds.Y + this.crossPadding),
                new Point(checkBoxBounds.X + this.crossPadding, checkBoxBounds.Y + checkBoxBounds.Height - this.crossPadding),
                Color.Blue, 1);
        }

        public override void Update()
        {

        }

        public override void OnMouseEnter(MouseEvent e)
        {
            if( this.enabled ) this.isMouseOver = true;
            // Console.Out.WriteLine("Mouse enter on checkbox!");
        }

        public override void OnMouseExit(MouseEvent e)
        {
            if( this.enabled ) this.isMouseOver = false;
            // Console.Out.WriteLine("Mouse out on checkbox!");
        }

        public void OnMouseClick(MouseEvent m_event)
        {
            if (!this.enabled) return;
            if (m_event.button == MouseEvent.MOUSE_BUTTON_1)
            {
                Point screenLocation = parent.RequestScreenLocation(new Point(this.bounds.X, this.bounds.Y));
                Rectangle screenRect = new Rectangle(screenLocation.X, screenLocation.Y, this.bounds.Width, this.bounds.Height);
                if (screenRect.Contains(m_event.location))
                {
                    // Console.Out.WriteLine("Pressed on a checkbox!");
                    this.selected = !this.selected;
                    if (this.onClickListeners != null) onClickListeners(this);
                }
            }
        }

        public void OnMouseRelease(MouseEvent m_event)
        {

        }

        public override void Unload()
        {
            MouseManager.GetInstance().mouseClickedListeners -= OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners -= OnMouseRelease;
        }
    }
}
