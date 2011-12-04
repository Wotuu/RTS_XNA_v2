using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.ParentComponents;
using System.Diagnostics;
using XNAInterfaceComponents.ChildComponents;
using XNAInputLibrary.KeyboardInput;

public delegate void OnSliderValueChanged(XNASlider source);

namespace XNAInterfaceComponents.ParentComponents
{
    public class XNASlider : ParentComponent, MouseMotionListener
    {
        private float _minValue { get; set; }

        public float minValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                this.sliderTextField.text = value + "";
                this._minValue = value;
            }
        }
        public float maxValue { get; set; }

        private float _currentValue { get; set; }
        public float currentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                this._currentValue = MathHelper.Clamp(value, this.minValue, this.maxValue);

                if (!this.sliderTextField.isFocussed)
                {
                    if (useIntegerValues)
                        this.sliderTextField.text = (int)value + "";
                    else
                        this.sliderTextField.text = value + "";
                }


                if (onSliderValueChangedListeners != null)
                    onSliderValueChangedListeners(this);
                this.UpdateSlider(value);
            }
        }
        public XNATextField sliderTextField { get; set; }

        private XNAButton sliderButton { get; set; }
        public Color lineColor { get; set; }

        public Boolean useIntegerValues { get; set; }

        private MouseEvent previousMouseEvent { get; set; }
        public OnSliderValueChanged onSliderValueChangedListeners { get; set; }

        public XNASlider(ParentComponent parent, Rectangle bounds) :
            base(parent, bounds)
        {
            this.maxValue = 100f;

            this.sliderButton = new XNAButton(this, new Rectangle(0, 0, 10, this.bounds.Height), "");
            this.sliderTextField = new XNATextField(this, new Rectangle(this.bounds.Width + 5, 0, 60, this.bounds.Height), 1);

            // this.sliderButton.z = this.z - 0.0001f;
            this.lineColor = Color.Black;
            // this.border = new Border(this, 1, Color.Black);

            MouseManager.GetInstance().mouseDragListeners += this.OnMouseDrag;
            MouseManager.GetInstance().mouseMotionListeners += this.OnMouseMotion;
            this.sliderTextField.onTextFieldValueChangedListeners += this.OnTextFieldValueChanged;

            this.sliderTextField.text = "0";
        }


        public override void Draw(SpriteBatch sb)
        {
            Rectangle drawRect = this.GetScreenBounds();

            ComponentUtil.DrawLine(sb, new Point(drawRect.X, drawRect.Y + (this.sliderButton.bounds.Height / 2)),
                new Point(drawRect.X + drawRect.Width, drawRect.Y + (this.sliderButton.bounds.Height / 2)), this.lineColor,
                2, this.z);

            this.border.Draw(sb);

            foreach (Component comp in this.children)
            {
                comp.Draw(sb);
            }
        }

        public void SetSliderButtonWidth(int width)
        {
            this.sliderButton.bounds = new Rectangle(this.sliderButton.bounds.X,
                this.bounds.Y, width, this.sliderButton.bounds.Height);
        }

        /// <summary>
        /// Sets the slider button height.
        /// </summary>
        /// <param name="height">The new height</param>
        public void SetSliderButtonHeight(int height)
        {
            this.sliderButton.bounds = new Rectangle(this.sliderButton.bounds.X,
                this.bounds.Y - (height / 2), this.sliderButton.bounds.Width, height);
        }

        public override void Update()
        {
            foreach (Component comp in this.children)
            {
                comp.Update();
            }
        }

        public override void OnMouseEnter(MouseEvent e)
        {

        }

        public override void OnMouseExit(MouseEvent e)
        {

        }

        public override void Unload()
        {

        }

        public void OnMouseMotion(MouseEvent e)
        {
            previousMouseEvent = null;
        }

        public void OnMouseDrag(MouseEvent e)
        {
            if (this.sliderButton.GetScreenBounds().Contains(e.location) || this.sliderButton.isMouseOver)
            {
                if (previousMouseEvent != null)
                {
                    this.sliderButton.bounds = new Rectangle(
                        (int)MathHelper.Clamp(this.sliderButton.bounds.X - (previousMouseEvent.location.X - e.location.X),
                            0, this.bounds.Width - this.sliderButton.bounds.Width),
                        this.sliderButton.bounds.Y,
                        this.sliderButton.bounds.Width, this.sliderButton.bounds.Height);

                    this.UpdateValue();
                }
                previousMouseEvent = e;
            }
            else previousMouseEvent = null;
        }

        public void OnTextFieldValueChanged()
        {
            if (this.sliderTextField.isFocussed)
            {
                float result = 0;
                if (float.TryParse(this.sliderTextField.text, out result))
                {
                    this.UpdateSlider(result);
                    this.currentValue = result;
                    if (this.onSliderValueChangedListeners != null)
                        this.onSliderValueChangedListeners(this);
                }
            }
        }

        /// <summary>
        /// Updates the value of this slider, according to the slider position
        /// </summary>
        /// <returns>The pixels.</returns>
        public void UpdateValue()
        {
            int slideableWidth = this.bounds.Width - (this.sliderButton.bounds.Width);

            float percentageOnBar = (this.sliderButton.bounds.X / (float)slideableWidth) * 100f;

            this.currentValue = this.minValue +
                (((this.maxValue - this.minValue) / 100f) * percentageOnBar);
        }

        /// <summary>
        /// Updates the slider position, according to given value.
        /// </summary>
        /// <param name="value">The value</param>
        private void UpdateSlider(float value)
        {
            float range = (this.maxValue - this.minValue);
            value -= this.minValue;

            float percentage = (value / range) * 100f;

            int slideableWidth = this.bounds.Width - (this.sliderButton.bounds.Width);

            this.sliderButton.bounds = new Rectangle(
                (int)MathHelper.Clamp(((slideableWidth / 100f) * percentage),
                0, this.bounds.Width - this.sliderButton.bounds.Width),
                        this.sliderButton.bounds.Y,
                        this.sliderButton.bounds.Width, this.sliderButton.bounds.Height);
        }
    }
}
