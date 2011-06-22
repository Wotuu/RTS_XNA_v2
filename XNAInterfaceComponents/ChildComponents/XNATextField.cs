using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Interfaces;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.Misc;
using XNAInputLibrary.KeyboardInput;
using Microsoft.Xna.Framework.Input;

public delegate void OnTextFieldKeyPressed(KeyEvent e);

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNATextField : ChildComponent, Focusable, KeyboardListener, MouseClickListener, MouseMotionListener
    {
        public Caret caret { get; set; }
        private Rectangle scrollbarBounds { get; set; }
        private Rectangle scrollbarButtonBounds { get; set; }
        private Boolean drawScrollbar { get; set; }
        public int scrollbarIndex { get; set; }
        public String hiddenCharacters { get; set; }
        public int rows { get; set; }
        public int maxLength { get; set; }
        public Boolean isEditable { get; set; }
        public OnTextFieldKeyPressed onTextFieldKeyPressedListeners { get; set; }

        public XNATextField(ParentComponent parent, Rectangle bounds, int rows)
            : base(parent, bounds)
        {
            this.caret = new Caret(this);
            this.rows = rows;
            this.isEditable = true;
            Rectangle drawRect = this.GetScreenBounds();
            this.scrollbarBounds = new Rectangle(drawRect.Left, drawRect.Bottom, drawRect.Width, 10);
            this.scrollbarButtonBounds = new Rectangle(drawRect.Left, drawRect.Bottom, drawRect.Width, 8);
            KeyboardManager.GetInstance().keyPressedListeners += this.OnKeyPressed;
            KeyboardManager.GetInstance().keyTypedListeners += this.OnKeyTyped;
            KeyboardManager.GetInstance().keyReleasedListeners += this.OnKeyReleased;


            MouseManager.GetInstance().mouseDragListeners += this.OnMouseDrag;
            MouseManager.GetInstance().mouseMotionListeners += this.OnMouseMotion;

            MouseManager.GetInstance().mouseClickedListeners += this.OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += this.OnMouseRelease;
            //this.text = "abcdefghijklmnopqrstuvwxyz \nnext line";
            this.hiddenCharacters = "";
        }

        private String previousDisplayText { get; set; }

        /// <summary>
        /// Gets the text on a row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>The text</returns>
        public String GetTextOnRow(int row)
        {
            return this.text.Split(new char[] { '\n' })[row];
        }

        /// <summary>
        /// Gets the display text
        /// </summary>
        /// <returns>The display text</returns>
        public String GetDisplayText()
        {
            Char[] array = this.text.ToArray();

            float currentStringWidth = 0;
            float textWidth = this.font.MeasureString(this.text).X;
            float viewportX = this.bounds.Width - this.padding.left - this.padding.right;

            if (viewportX > textWidth)
            {
                this.previousDisplayText = this.text;
                return this.text;
            }

            String result = "";
            // this.caret.index -= this.scrollbarIndex;
            // int start = 0;

            this.hiddenCharacters = "";
            for (int i = this.text.Length - 1; i >= 0; i--)
            {
                Char currentChar = array[i];
                currentStringWidth += this.font.MeasureString(currentChar + "").X;
                if (currentStringWidth < viewportX)
                {
                    result += currentChar + "";
                }
                else hiddenCharacters += currentChar + "";
            }
            /*for (int i = 0; i < this.text.Length; i++)
            {
                Char currentChar = array[i];

                if( i >= start )
                {
                    currentStringWidth += this.font.MeasureString(currentChar + "").X;
                    if (currentStringWidth < viewportX)
                    {
                        result += currentChar + "";
                    }
                    else
                    {
                        break;
                    }
                }
                else 
                {
                    this.hiddenCharacters += currentChar + "";
                }
            }*/
            // this.caret.index += this.scrollbarIndex;

            //char[] charArray = hiddenCharacters.ToCharArray();
            //Array.Reverse(charArray);
            //hiddenCharacters = new String(charArray);

            array = result.ToCharArray();
            Array.Reverse(array);
            this.previousDisplayText = new string(array);
            return new string(array);
            // return result;
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
            // Draw the button
            sb.Draw(clearTexture, drawRect, drawColor);
            // Draw the border
            if (this.border != null) border.Draw(sb);

            // Draw the caret
            this.caret.Draw(sb);

            // Draw the text
            String displayText = GetDisplayText();
            Vector2 fontDimensions = this.font.MeasureString(displayText);
            float drawY = drawRect.Y + padding.top;

            sb.DrawString(font, displayText,
                new Vector2(drawRect.X + this.padding.left,
                    drawY), this.fontColor);

            // Draw scrollbar
            if (drawScrollbar)
            {
                ComponentUtil.DrawClearRectangle(sb, this.scrollbarBounds, 1, Color.Pink);
                sb.Draw(this.clearTexture, this.scrollbarButtonBounds, Color.Blue);
            }
        }

        public override void Update()
        {
            this.caret.Update();
            #region Old scrollbar update code
            /*float percentageShown = (float)((GetDisplayText().Length / (float)text.Length) * 100.0);
            // Draw it if we're hiding stuff
            this.drawScrollbar = percentageShown < 99;

            scrollbarButtonBounds = new Rectangle(scrollbarButtonBounds.X,
                scrollbarButtonBounds.Y,
                Math.Max((int)((scrollbarBounds.Width / 100.0) * percentageShown), 20),
                scrollbarBounds.Height);

            // Get the valid area (x) of the scrollbar
            float scrollbarValidArea = this.scrollbarBounds.Width - this.scrollbarButtonBounds.Width;
            // Get the percentage the bar is on the slider
            float percentageOnBar = ((this.scrollbarButtonBounds.X - this.scrollbarBounds.X) / scrollbarValidArea) * 100;
            // Measure the string width of the entire text
            float stringWidth = this.font.MeasureString(text).X;
            // Calculate the width that was hidden
            float hiddenWidth = (float)((stringWidth / 100.0) * percentageOnBar);
            
            Char[] array = this.text.ToCharArray();
            float currentWidth = 0;
            int currentIndex = 0;
            for (int i = 0; i < array.Length; i++)
            {
                currentWidth += this.font.MeasureString(array[i] + "").X;
                if (currentWidth > hiddenWidth)
                {
                    break;
                }
                else currentIndex++;
            }
            this.scrollbarIndex = currentIndex;*/
            // Console.Out.WriteLine(currentIndex);
            //this.scrollbarIndex = 
            // Console.Out.WriteLine((int)((scrollbarBounds.Width / 100.0) * percentageShown));
            #endregion
        }

        public override void OnMouseEnter(MouseEvent e)
        {

        }

        public override void OnMouseExit(MouseEvent e)
        {

        }

        public void OnFocusReceived()
        {
            if (this.isEditable)
            {
                this.isFocussed = true;
                // Console.Out.WriteLine("TextField received focus");
            }
        }

        public void OnFocusLost()
        {
            this.isFocussed = false;
            // Console.Out.WriteLine("TextField lost focus");
        }

        #region Insert & Delete character
        /// <summary>
        /// Inserts a string at the current caret position.
        /// </summary>
        /// <param name="s">The string to add</param>
        private void InsertStringAtCaret(String s)
        {
            String newString = "";
            Char[] array = this.text.ToCharArray();


            // Apply changes
            int caretIndex = this.caret.GetCaretArrayIndex();
            for (int i = 0; i < caretIndex && i < array.Length; i++)
            {
                newString += "" + array[i];
            }
            newString += s;
            for (int i = caretIndex; i < array.Length; i++)
            {
                newString += "" + array[i];
            }
            this.caret.index += s.Length;
            this.text = newString;
        }

        /// <summary>
        /// Deletes a character, and places the caret before or after the deleted character
        /// </summary>
        /// <param name="index">The index to remove</param>
        /// <param name="isBackspace"></param>
        private void DeleteCharacterAt(int index, Boolean isBackspace)
        {
            String newString = "";
            Char[] array = this.text.ToCharArray();

            if (isBackspace)
            {
                for (int i = 0; i < index - 1 && i < array.Length; i++)
                {
                    newString += "" + array[i];
                }
                for (int i = index; i < array.Length; i++)
                {
                    newString += "" + array[i];
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    newString += "" + array[i];
                }
                for (int i = index + 1; i < array.Length; i++)
                {
                    newString += "" + array[i];
                }
            }
            this.text = newString;
            if (isBackspace) this.caret.index--;
        }
        #endregion

        private void ProcessKey(KeyEvent e)
        {
            if (this.maxLength != 0 && this.text.Length >= this.maxLength) return;
            if (this.isFocussed)
            {
                if (onTextFieldKeyPressedListeners != null) onTextFieldKeyPressedListeners(e);
                String keyString = e.key.ToString();
                if (keyString.Equals("Left"))
                {
                    if (this.caret.index > 1)
                    {
                        // Console.Out.WriteLine(this.caret.index);
                        this.caret.index--;
                    }
                    else if (this.caret.row > 0)
                    {
                        this.caret.row--;
                        this.caret.index = this.caret.GetTextOnCaretRow().Length - 1;
                    }
                }
                else if (keyString.Equals("Right"))
                {
                    if (this.caret.index < (this.caret.GetTextOnCaretRow().Length)) this.caret.index++;
                    else
                    {
                        int rowCount = this.text.Split(new char[] { '\n' }).Length;
                        if (this.caret.row < rowCount - 1)
                        {
                            this.caret.index = 1;
                            this.caret.row++;
                        }
                    }
                    // Console.Out.WriteLine(this.caret.index);
                    // Console.Out.WriteLine(this.caret.row);
                }
                else if (keyString.Equals("Up"))
                {
                    if (this.caret.row > 0) this.caret.row--;
                }
                else if (keyString.Equals("Down"))
                {
                    if (this.rows > 1 && this.caret.row < this.text.Split(new char[] { '\n' }).Length) this.caret.row++;
                }
                else if (keyString.Contains("Back"))
                {
                    if (this.caret.index > 0) DeleteCharacterAt(this.caret.GetCaretArrayIndex(), true);
                    else if (this.caret.row > 0)
                    {
                        this.caret.row--;
                        this.caret.index = this.caret.GetTextOnCaretRow().Length;
                    }
                }
                else if (keyString.Contains("Delete"))
                {
                    if (this.caret.index < this.text.Length) DeleteCharacterAt(this.caret.GetCaretArrayIndex(), false);
                }
                else if (keyString.Equals("Enter"))
                {
                    if (this.caret.row < this.rows - 1)
                    {
                        this.InsertStringAtCaret("\n ");
                        this.caret.row++;
                        this.caret.index = 1;
                    }
                    // this.OnFocusLost();
                }
                else if (keyString.Equals("F1") ||
                    keyString.Equals("F2") ||
                    keyString.Equals("F3") ||
                    keyString.Equals("F4") ||
                    keyString.Equals("F5") ||
                    keyString.Equals("F6") ||
                    keyString.Equals("F7") ||
                    keyString.Equals("F8") ||
                    keyString.Equals("F9") ||
                    keyString.Equals("F10") ||
                    keyString.Equals("F11") ||
                    keyString.Equals("F12"))
                {
                    // Do nothing
                }
                else
                {
                    String typedChar = "";
                    #region Convert keyString to a typedChar
                    if (keyString.Equals("Space")) typedChar = " ";
                    else if (keyString.Equals("OemQuestion"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "?";
                        else typedChar = "/";
                    }
                    else if (keyString.Equals("OemPeriod"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = ">";
                        else typedChar = ".";
                    }
                    else if (keyString.Equals("OemSemicolon"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = ":";
                        else typedChar = ";";
                    }
                    else if (keyString.Equals("OemQuotes"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "\"";
                        else typedChar = "'";
                    }
                    else if (keyString.Equals("OemComma"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "<";
                        else typedChar = ",";
                    }
                    else if (keyString.Equals("OemOpenBrackets"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "{";
                        else typedChar = "[";
                    }
                    else if (keyString.Equals("OemCloseBrackets"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "}";
                        else typedChar = "]";
                    }
                    else if (keyString.Equals("OemPipe"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "|";
                        else typedChar = "\\";
                    }
                    else if (keyString.Equals("OemMinus"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "_";
                        else typedChar = "-";
                    }
                    else if (keyString.Equals("OemPlus"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "+";
                        else typedChar = "=";
                    }
                    else if (keyString.Equals("OemTilde"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "~";
                        else typedChar = "`";
                    }
                    else if (keyString.Equals("D0"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = ")";
                        else typedChar = "0";
                    }
                    else if (keyString.Equals("D1"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "!";
                        else typedChar = "1";
                    }
                    else if (keyString.Equals("D2"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "@";
                        else typedChar = "2";
                    }
                    else if (keyString.Equals("D3"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "#";
                        else typedChar = "3";
                    }
                    else if (keyString.Equals("D4"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "$";
                        else typedChar = "4";
                    }
                    else if (keyString.Equals("D5"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "%";
                        else typedChar = "5";
                    }
                    else if (keyString.Equals("D6"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "^";
                        else typedChar = "6";
                    }
                    else if (keyString.Equals("D7"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "&";
                        else typedChar = "7";
                    }
                    else if (keyString.Equals("D8"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "*";
                        else typedChar = "8";
                    }
                    else if (keyString.Equals("D9"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "(";
                        else typedChar = "9";
                    }
                    else
                    {
                        if (keyString.Length == 1)
                        {
                            if (!e.modifiers.Contains(KeyEvent.Modifier.SHIFT))
                                keyString = keyString.ToLower();
                        }
                        typedChar = keyString;
                    }
                    #endregion

                    this.InsertStringAtCaret(typedChar);
                }
            }
        }

        public void OnKeyPressed(KeyEvent e)
        {
            ProcessKey(e);
            // Console.Out.WriteLine("Pressed: " + e.key.ToString());
        }

        public void OnKeyTyped(KeyEvent e)
        {
            ProcessKey(e);
            // Console.Out.WriteLine("Typed: " + e.key.ToString());
        }

        public void OnKeyReleased(KeyEvent e)
        {
            // Console.Out.WriteLine("Released: " + e.key.ToString());
            // throw new NotImplementedException();
        }

        public void OnMouseMotion(MouseEvent e)
        {

        }

        // private MouseEvent previousMouseEvent { get; set; }
        public void OnMouseDrag(MouseEvent e)
        {
            /*if (this.scrollbarBounds.Contains(e.location))
            {
                if (previousMouseEvent != null)
                {
                    this.scrollbarButtonBounds = new Rectangle(
                        (int)MathHelper.Clamp(
                            (this.scrollbarButtonBounds.X + (e.location.X - previousMouseEvent.location.X)),
                            scrollbarBounds.Left,
                            scrollbarBounds.Right - scrollbarButtonBounds.Width),
                        scrollbarButtonBounds.Y,
                        scrollbarButtonBounds.Width,
                        scrollbarButtonBounds.Height);
                }
            }
            previousMouseEvent = e;*/
        }

        public void OnMouseClick(MouseEvent e)
        {
            /*if (this.scrollbarBounds.Contains(e.location))
            {
                if (previousMouseEvent != null)
                {
                    this.scrollbarButtonBounds = new Rectangle(
                        (int)MathHelper.Clamp(
                            (e.location.X  - (scrollbarButtonBounds.Width / 2 )),
                            scrollbarBounds.Left,
                            scrollbarBounds.Right - scrollbarButtonBounds.Width),
                        scrollbarButtonBounds.Y,
                        scrollbarButtonBounds.Width,
                        scrollbarButtonBounds.Height);

                }
            }*/

            Rectangle drawLocation = this.GetScreenBounds();

            if (drawLocation.Contains(e.location))
            {
                // Get the index the cursor is at.
                float currentWidth = 0;
                int currentIndex = this.hiddenCharacters.Length;
                if (this.previousDisplayText != null)
                {
                    Char[] array = this.previousDisplayText.ToArray();
                    for (int i = 0; i < array.Length; i++)
                    {
                        currentWidth += this.font.MeasureString(array[i] + "").X;
                        if (currentWidth < e.location.X - drawLocation.X - this.padding.left) currentIndex++;
                        else break;
                    }
                }
                // Console.Out.WriteLine(currentIndex);
                this.caret.index = currentIndex;
                this.caret.row = (int)Math.Min(
                    ((e.location.Y - drawLocation.Y) / this.font.MeasureString("I").Y),
                    this.text.Split(new char[] { '\n' }).Length - 1);
                // Console.Out.WriteLine(((e.location.Y - drawLocation.Y) + " / " + this.font.MeasureString("I").Y));
                // Console.Out.WriteLine("New row: " + this.caret.row);
            }

            // previousMouseEvent = e;
        }

        public void OnMouseRelease(MouseEvent e)
        {
            // throw new NotImplementedException();
        }

        public override void Unload()
        {
            KeyboardManager.GetInstance().keyPressedListeners -= this.OnKeyPressed;
            KeyboardManager.GetInstance().keyTypedListeners -= this.OnKeyTyped;
            KeyboardManager.GetInstance().keyReleasedListeners -= this.OnKeyReleased;


            MouseManager.GetInstance().mouseDragListeners -= this.OnMouseDrag;
            MouseManager.GetInstance().mouseMotionListeners -= this.OnMouseMotion;

            MouseManager.GetInstance().mouseClickedListeners -= this.OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners -= this.OnMouseRelease;
        }
    }
}
