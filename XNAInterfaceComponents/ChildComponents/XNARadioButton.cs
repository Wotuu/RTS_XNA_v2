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

public delegate void OnRadioButtonClick(XNARadioButton source);

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNARadioButton : ChildComponent, Drawable, MouseClickListener
    {
        public OnRadioButtonClick onClickListeners { get; set; }

        public TextAlign textAlign { get; set; }
        public Boolean selected { get; set; }
        private Circle outerCircle { get; set; }
        private Circle innerCircle { get; set; }

        private XNARadioButtonGroup group { get; set; }

        public int crossPadding { get; set; }

        public enum TextAlign
        {
            LEFT,
            RIGHT
        }

        public XNARadioButton(ParentComponent parent, Rectangle bounds, XNARadioButtonGroup group, String text)
            : base(parent, bounds)
        {
            this.text = text;
            this.textAlign = TextAlign.RIGHT;
            this.group = group;
            this.group.RegisterMember(this);

            MouseManager.GetInstance().mouseClickedListeners += OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += OnMouseRelease;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!this.visible) return;
            if (this.clearTexture == null) this.clearTexture = ComponentUtil.GetClearTexture2D(sb);
            if (outerCircle == null)
            {
                outerCircle = new Circle(this, (int)(this.bounds.Width / 2), 1,
                    new Point(this.bounds.X + (this.bounds.Width / 2), this.bounds.Y + (this.bounds.Height / 2)), Color.Red);
                outerCircle.InitCircle(sb);
            }
            if (innerCircle == null)
            {
                innerCircle = new Circle(this, (int)(this.bounds.Width / 2) - 3, 1,
                    new Point(this.bounds.X + (this.bounds.Width / 2) + 3, this.bounds.Y + (this.bounds.Height / 2) + 3), Color.Red);
                innerCircle.InitCircle(sb);
            }


            Color drawColor = new Color();
            if (this.isMouseOver) drawColor = this.mouseOverColor;
            else drawColor = this.backgroundColor;

            Rectangle drawRect = this.GetScreenBounds();

            // Determine where the checkbox should go
            Rectangle radioButtonBounds = new Rectangle();
            if (this.textAlign == TextAlign.RIGHT)
            {
                radioButtonBounds = new Rectangle((int)drawRect.X, (int)drawRect.Y,
                (int)this.bounds.Width, (int)this.bounds.Height);
            }
            else if (this.textAlign == TextAlign.LEFT)
            {
                radioButtonBounds = new Rectangle((int)(drawRect.X + this.bounds.Width - this.bounds.X),
                    (int)(drawRect.Y + this.bounds.Height - this.bounds.Y),
                (int)this.bounds.Width, (int)this.bounds.Height);
            }
            // Draw the outer line of the checkbox
            this.outerCircle.center = new Point(
                (int)radioButtonBounds.X,
                (int)radioButtonBounds.Y + this.outerCircle.radius);
            this.outerCircle.Draw(sb);

            // Draw the cross, if selected
            if (this.selected)
            {
                this.innerCircle.center = new Point(
                    (int)radioButtonBounds.X,
                    (int)radioButtonBounds.Y + this.outerCircle.radius);
                this.innerCircle.Draw(sb);
            }

            // Draw the text
            if (this.textAlign == TextAlign.LEFT)
            {
                sb.DrawString(this.font, this.text,
                    new Vector2(drawRect.X + this.padding.left,
                        drawRect.Y + (drawRect.Height / 2) - (this.font.MeasureString(this.text).Y / 2)),
                        this.fontColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, this.z - 0.01f);
            }
            else if (this.textAlign == TextAlign.RIGHT)
            {
                sb.DrawString(this.font, this.text,
                    new Vector2(drawRect.X + this.padding.left + this.bounds.X,
                        drawRect.Y + (drawRect.Height / 2) - (this.font.MeasureString(this.text).Y / 2)),
                        this.fontColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, this.z - 0.01f);
            }
        }

        public override void Update()
        {

        }

        public override void OnMouseEnter(MouseEvent e)
        {
            if (this.enabled) this.isMouseOver = true;
            // Console.Out.WriteLine("Mouse enter on checkbox!");
        }

        public override void OnMouseExit(MouseEvent e)
        {
            if (this.enabled) this.isMouseOver = false;
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
                    this.selected = true;
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
