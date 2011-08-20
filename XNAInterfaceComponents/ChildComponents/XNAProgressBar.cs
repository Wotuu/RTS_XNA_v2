using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;


public delegate void OnProgressChange(XNAProgressBar source);

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNAProgressBar : ChildComponent
    {
        public Color innerColor { get; set; }
        public OnProgressChange onProgressChangeListeners { get; set; }
        private int _currentValue { get; set; }
        public int currentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                this._currentValue = value;
                if (this.onProgressChangeListeners != null) this.onProgressChangeListeners(this);
            }
        }
        public XNALabel progressDisplayLabel { get; set; }
        public int maxValue { get; set; }

        private static Color BORDER_COLOR = new Color(0, 0, 0, 255),
        BACKGROUND_COLOR = new Color(0, 0, 0, 255);


        public XNAProgressBar(ParentComponent parent, Rectangle bounds, int maxValue)
            : base(parent, bounds)
        {
            this.maxValue = maxValue;

            this.innerColor = Color.Green;

            this.fontColor = Color.Red;

            this.progressDisplayLabel = new XNALabel(parent, this.bounds, "0%");
            this.progressDisplayLabel.z = this.z - 0.00001f;
            this.progressDisplayLabel.textAlign = XNALabel.TextAlign.CENTER;
            this.progressDisplayLabel.border = null;
        }


        /// <summary>
        /// Gets the percentage that the progress bar is completed.
        /// </summary>
        /// <returns>The percentage</returns>
        public double GetPercentageCompleted()
        {
            double val = (((double)this.currentValue / (double)this.maxValue) * 100.0);
            if (val == Double.NaN) val = 0;
            // 2 decimals
            val = ((int)(val * 100)) / 100.0;
            return val;
        }

        public override void Draw(SpriteBatch sb)
        {
            int innerWidth = (int)(((double)this.bounds.Width / 100.0) * this.GetPercentageCompleted());

            Rectangle drawRect = this.GetScreenBounds();

            sb.Draw(ComponentUtil.lineTexture, new Rectangle(drawRect.X - 1, drawRect.Y,
                drawRect.Width + 2, drawRect.Height), null, BORDER_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z);

            sb.Draw(ComponentUtil.lineTexture, new Rectangle(drawRect.X, drawRect.Y + 1,
                drawRect.Width - 2, drawRect.Height - 2), null, BACKGROUND_COLOR, 0f, Vector2.Zero, SpriteEffects.None, z - 0.0001f);

            sb.Draw(ComponentUtil.lineTexture, new Rectangle(drawRect.X, drawRect.Y + 1,
                innerWidth, drawRect.Height - 2), null, this.innerColor, 0f, Vector2.Zero, SpriteEffects.None, z - 0.0002f);

            String percentage = this.GetPercentageCompleted() + "";
            String[] split = percentage.Split(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToArray());
            if (split.Length == 2)
            {
                for (int i = 0; i < 2 - split[1].Length; i++) percentage += "0";
            } else percentage += NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00";
            this.progressDisplayLabel.text = percentage + "%";
            /*Vector2 measure = this.font.MeasureString(target);
            
            sb.DrawString(this.font, target, new Vector2(drawRect.X + (drawRect.Width / 2) - (measure.X / 2),
                 drawRect.Y + (drawRect.Height / 2) - (measure.Y / 2)), this.fontColor);*/
        }


        public override void Update()
        {

        }

        public override void OnMouseEnter(XNAInputHandler.MouseInput.MouseEvent e)
        {

        }

        public override void OnMouseExit(XNAInputHandler.MouseInput.MouseEvent e)
        {

        }

        public override void Unload()
        {

        }
    }
}
