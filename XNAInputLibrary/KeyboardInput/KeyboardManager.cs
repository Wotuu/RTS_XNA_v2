using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using XNAInputLibrary.KeyboardInput;


public delegate void OnKeyPressed(KeyEvent e);
public delegate void OnKeyTyped(KeyEvent e);
public delegate void OnKeyReleased(KeyEvent e);

namespace XNAInputLibrary.KeyboardInput
{
    public class KeyboardManager
    {
        public static KeyboardManager instance = null;
        public OnKeyPressed keyPressedListeners { get; set; }
        public OnKeyTyped keyTypedListeners { get; set; }
        public OnKeyReleased keyReleasedListeners { get; set; }

        private long firstPressedKeyTicks { get; set; }
        private Keys firstPressedTypeableKey { get; set; }
        public int keyTypedLagMS { get; set; }

        private Keys[] previousFramePressedKeys { get; set; }

        public LinkedList<KeyEvent.Modifier> heldModifiers { get; set; }



        private KeyboardManager()
        {
            keyTypedLagMS = 500;
            this.heldModifiers = new LinkedList<KeyEvent.Modifier>();
        }

        private KeyEvent.Modifier GetModifier(Keys key)
        {
            String keyString = key.ToString();
            if (keyString.Equals("LeftControl") || keyString.Equals("RightControl")) return KeyEvent.Modifier.CONTROL;
            else if (keyString.Equals("LeftAlt") || keyString.Equals("RightAlt")) return KeyEvent.Modifier.ALT;
            else if (keyString.Equals("LeftShift") || keyString.Equals("RightShift")) return KeyEvent.Modifier.SHIFT;
            return KeyEvent.Modifier.NONE;
        }

        private Boolean IsTypeable(Keys key)
        {
            if (key.ToString().Length == 1) return true;
            String keyString = key.ToString();
            
            if (
                #region it's not typeable
                keyString.Contains("Left") ||
                keyString.Contains("Right") ||
                keyString.Contains("Lock") ||
                keyString.Contains("Tab") ||
                keyString.Contains("Back") ||
                keyString.Contains("Enter") ||
                keyString.Contains("Up") ||
                keyString.Contains("Down") ||
                keyString.Contains("Del") ||
                keyString.Contains("Esc") ||
                keyString.Contains("F1") ||
                keyString.Contains("F2") ||
                keyString.Contains("F3") ||
                keyString.Contains("F4") ||
                keyString.Contains("F5") ||
                keyString.Contains("F6") ||
                keyString.Contains("F7") ||
                keyString.Contains("F8") ||
                keyString.Contains("F9") ||
                keyString.Contains("F10") ||
                keyString.Contains("F11") ||
                keyString.Contains("F12")
                #endregion
                )
            
            {
                return false;
            }
            return true;
        }

        public void Update(KeyboardState state)
        {
            LinkedList<Keys> releasedKeys = this.GetReleasedKeys(state.GetPressedKeys());
            if (releasedKeys.Contains(this.firstPressedTypeableKey))
            {
                // Console.Out.WriteLine("Reset first typed key!");
                this.firstPressedTypeableKey = new Keys();
                this.firstPressedKeyTicks = 0;
            }
            foreach (Keys key in releasedKeys)
            {
                if (this.keyReleasedListeners != null && GetModifier(key) == KeyEvent.Modifier.NONE)
                {
                    keyReleasedListeners(new KeyEvent(key, KeyEvent.Type.Released, this.heldModifiers.ToArray()));
                }
            }



            // Set modifiers
            this.heldModifiers.Clear();
            foreach (Keys key in state.GetPressedKeys())
            {
                // Console.Out.WriteLine("Key: " + key.ToString());
                KeyEvent.Modifier mod = GetModifier(key);
                if (mod != KeyEvent.Modifier.NONE)
                {
                    if (!this.heldModifiers.Contains(mod))
                    {
                        // Console.Out.WriteLine("Adding a modifier!");
                        this.heldModifiers.AddLast(mod);
                    }
                    continue;
                }
            }

            // set rest of keys
            foreach (Keys key in state.GetPressedKeys())
            {
                // No modifiers allowed here
                if (GetModifier(key) != KeyEvent.Modifier.NONE) continue;

                // The rest
                if (this.keyPressedListeners != null
                    && !this.previousFramePressedKeys.Contains(key))
                {
                    if( keyPressedListeners != null ) 
                    keyPressedListeners(new KeyEvent(key, KeyEvent.Type.Pressed, this.heldModifiers.ToArray()));
                }

                if ( firstPressedTypeableKey.ToString() == "None")
                {
                    firstPressedTypeableKey = key;
                    firstPressedKeyTicks = System.DateTime.UtcNow.Ticks;
                }

                if (key == firstPressedTypeableKey)
                {
                    if (System.DateTime.UtcNow.Ticks > ((firstPressedKeyTicks + (keyTypedLagMS * 10000))))
                    {
                        if (keyTypedListeners != null) 
                        keyTypedListeners(new KeyEvent(key, KeyEvent.Type.Typed, this.heldModifiers.ToArray()));
                    }
                }
            }
            /*
            foreach (Keys key in state.GetPressedKeys())
            {
                if (lastPressedKey == key)
                {
                    keyLastPressedTicks[index] = System.DateTime.UtcNow.Ticks;
                    keyIsHeld[index] = true;

                    if (this.keyPressedListeners != null)
                    {
                        // Only for characters
                        if (key.ToString().Length == 1) keyPressedListeners(new KeyEvent(key, KeyEvent.Type.Pressed));
                        else  {

                        }
                    }
                    // Console.Out.WriteLine("Key pressed: " + key.ToString());
                }
                else
                {
                    // Console.Out.WriteLine(System.DateTime.UtcNow.Ticks + ">" + ((keyLastPressedTicks[index] + (keyTypedLagMS * 10000))));
                    if (System.DateTime.UtcNow.Ticks > ((keyLastPressedTicks[index] + (keyTypedLagMS * 10000))))
                    {
                        if (this.keyTypedListeners != null)
                        {
                            // Only for characters
                            if( key.ToString().Length == 1 ) keyTypedListeners(new KeyEvent(key, KeyEvent.Type.Typed));
                            else
                            {

                            }
                        }
                        //Console.Out.WriteLine("Key typed: " + key.ToString());
                    }
                }
            }*/
            this.previousFramePressedKeys = state.GetPressedKeys();
        }

        /// <summary>
        /// Gets the released keys since last frame
        /// </summary>
        /// <param name="newKeys"></param>
        /// <returns></returns>
        private LinkedList<Keys> GetReleasedKeys(Keys[] newKeys)
        {
            LinkedList<Keys> result = new LinkedList<Keys>();
            if (this.previousFramePressedKeys == null) return result;
            for (int i = 0; i < this.previousFramePressedKeys.Length; i++)
            {
                Keys currentKey = this.previousFramePressedKeys[i];
                result.AddLast(currentKey);
                for (int j = 0; j < newKeys.Length; j++)
                {
                    if (currentKey == newKeys[j])
                    {
                        result.Remove(currentKey);
                        break;
                    }
                }
            }
            return result;
        }

        public static KeyboardManager GetInstance()
        {
            if (instance == null) instance = new KeyboardManager();
            return instance;
        }
    }
}
