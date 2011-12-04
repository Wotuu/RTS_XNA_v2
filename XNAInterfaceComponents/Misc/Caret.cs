using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using System.Diagnostics;

namespace XNAInterfaceComponents.Misc
{
    public class Caret : Drawable
    {
        public int width { get; set; }
        public int index { get; set; }
        private int _row { get; set; }
        public int row
        {
            get
            {
                return _row;
            }
            set
            {
                this._row = value;
                int textLengthAtRow = this.GetTextOnCaretRow().Length;
                this.index = Math.Min(textLengthAtRow, this.index);
            }
        }
        public int blinkMS { get; set; }
        public XNATextField parent { get; set; }
        public Color color { get; set; }

        private Boolean visible { get; set; }

        private double previousBlinkMS { get; set; }

        public int GetCaretArrayIndex()
        {
            // Calculate real caret index
            int caretIndex = 0;
            for (int i = 0; i < this.row; i++)
            {
                caretIndex += this.parent.GetTextOnRow(i).Length;
            }
            // Account for \n's
            caretIndex += this.row;
            // Add caret index
            caretIndex += this.index;
            return caretIndex;
        }

        /// <summary>
        /// Gets the text that is on the row of the caret.
        /// </summary>
        /// <returns>The string</returns>
        public String GetTextOnCaretRow()
        {
            return this.parent.text.Split(new char[] { '\n' })[this.row];
        }

        public Caret(XNATextField parent)
        {
            this.parent = parent;
            previousBlinkMS = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
            blinkMS = 500;
            this.color = Color.Black;
            this.width = 1;
            this.visible = true;
        }

        public void Update()
        {
            if (!this.parent.isFocussed) return;
            // Console.Out.WriteLine(System.DateTime.UtcNow.Millisecond + " > " + (previousBlinkTicks + blinkTicks));
            double now = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
            if (now > (previousBlinkMS + blinkMS))
            {
                this.visible = !this.visible;
                previousBlinkMS = now;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (this.visible && this.parent.isFocussed)
            {
                Rectangle drawLocation = parent.GetScreenBounds();
                String toMeasure = "";
                Char[] array = this.GetTextOnCaretRow().ToCharArray();
                if (index == 1 && array.Length == 0)
                {
                    // nothing :3
                }
                else if (array.Length == 0) toMeasure = "I"; 
                else
                {
                    for (int i = parent.hiddenCharacters.Length; i < this.index && i < array.Length; i++)
                    {
                        toMeasure += "" + array[i];
                    }
                }
                Vector2 dimensions = parent.font.MeasureString(toMeasure);
                if (array.Length == 0) dimensions.X = 0;
                else dimensions.X += 2;
                ComponentUtil.DrawLine(sb,
                    new Point(drawLocation.X + parent.padding.left + (int)(dimensions.X),
                        drawLocation.Y + parent.padding.top + (int)(dimensions.Y * (this.row))),
                    new Point(drawLocation.X + parent.padding.left + (int)(dimensions.X),
                        drawLocation.Y + parent.padding.top + (int)(dimensions.Y * (this.row + 1))),
                    this.color, width, parent.z - 0.01f);
            }
        }
    }
}
