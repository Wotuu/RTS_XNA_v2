using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using SocketLibrary.Users;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.ParentComponents;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class UserDisplayPanel : XNAPanel
    {
        public int index { get; set; }
        public User user { get; set; }
        public static int componentHeight = 40;
        public static int componentSpacing = 10;

        public XNACheckBox readyCheckBox { get; set; }
        public XNALabel usernameLbl { get; set; }
        public XNALabel teamLbl { get; set; }
        public XNADropdown teamDropdown { get; set; }
        public XNADropdown colorDropdown { get; set; }
        public XNAButton kickBtn { get; set; }

        private Color[] colors = new Color[] { Color.Black, Color.Blue, Color.Green, Color.Purple,  
                Color.Red, Color.Pink, Color.Yellow, Color.Orange };

        public UserDisplayPanel(ParentComponent parent, User user, int index) :
            base(parent, new Rectangle())
        {
            this.user = user;
            this.UpdateBounds(index);
            Boolean enabled = false;
            if (user.id == ChatServerConnectionManager.GetInstance().user.id) enabled = true;

            readyCheckBox = new XNACheckBox(this, new Rectangle(10, 10, 20, 20), "");
            readyCheckBox.onClickListeners += this.OnReadyChanged;
            readyCheckBox.enabled = enabled;
            // readyCheckBox

            usernameLbl = new XNALabel(this, new Rectangle(40, 5, 230, 30), user.username);
            usernameLbl.font = MenuManager.BIG_TEXTFIELD_FONT;

            teamDropdown = new XNADropdown(this, new Rectangle(280, 5, 50, 30));
            teamDropdown.dropdownLineSpace = 15;
            teamDropdown.arrowSize = 8;
            teamDropdown.onOptionSelectedChangedListeners += this.OnTeamChanged;
            for (int i = 0; i < 8; i++) teamDropdown.AddOption((i + 1) + "");
            teamDropdown.enabled = enabled;

            colorDropdown = new XNADropdown(this, new Rectangle(340, 5, 50, 30));
            colorDropdown.dropdownLineSpace = 15;
            colorDropdown.arrowSize = 8;
            for (int i = 0; i < colors.Length; i++)
            {
                colorDropdown.AddOption("");
                colorDropdown.SetBackgroundColor(i + 1, colors[i]);
            }
            colorDropdown.onOptionSelectedChangedListeners += this.OnColorChanged;
            colorDropdown.enabled = enabled;

            kickBtn = new XNAButton(this, new Rectangle(400, 5, 75, 30), "Kick");
            kickBtn.visible = false;

            if (((GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu()).IsCurrentUserHost() &&
                this.user.id != ChatServerConnectionManager.GetInstance().user.id)
            {
                kickBtn.visible = true;
                kickBtn.onClickListeners += this.OnKickButtonPressed;
            }

            if (this.user.id == 1)
            {
                // Wotuu
                this.SelectColor(1);
                this.readyCheckBox.selected = true;
                this.teamDropdown.SelectItem(0);
            }
            else if (this.user.id == 2)
            {
                this.SelectColor(3);
                this.readyCheckBox.selected = true;
                this.teamDropdown.SelectItem(1);
            }
        }

        /// <summary>
        /// When the kick button has been pressed.
        /// </summary>
        public void OnKickButtonPressed(XNAButton source)
        {
            Packet p = new Packet(Headers.GAME_KICK_CLIENT);
            p.AddInt(user.channelID);
            p.AddInt(user.id);
            ChatServerConnectionManager.GetInstance().SendPacket(p);
        }

        /// <summary>
        /// When the ready state changed.
        /// </summary>
        /// <param name="source">The source, bla</param>
        public void OnReadyChanged(XNACheckBox source)
        {
            Packet p = new Packet(Headers.GAME_READY_CHANGED);
            p.AddInt(user.channelID);
            p.AddInt(user.id);
            // 1 for true, 0 for false.
            p.AddInt(this.readyCheckBox.selected ? 1 : 0);
            ChatServerConnectionManager.GetInstance().SendPacket(p);
        }

        /// <summary>
        /// Notify the rest that this user's team has changed.
        /// </summary>
        public void OnTeamChanged()
        {
            Packet p = new Packet(Headers.GAME_TEAM_CHANGED);
            p.AddInt(user.channelID);
            p.AddInt(user.id);
            p.AddInt(Int32.Parse(this.teamDropdown.GetSelectedOption()));
            ChatServerConnectionManager.GetInstance().SendPacket(p);
        }

        /// <summary>
        /// Notify the rest that this user's color has changed.
        /// </summary>
        public void OnColorChanged()
        {
            Packet p = new Packet(Headers.GAME_COLOR_CHANGED);
            p.AddInt(user.channelID);
            p.AddInt(user.id);
            for (int i = 0; i < this.colors.Length; i++)
            {
                if (colors[i] == colorDropdown.GetSelectedButton().backgroundColor)
                {
                    p.AddInt(i);
                    break;
                }
            }
            ChatServerConnectionManager.GetInstance().SendPacket(p);
        }

        /// <summary>
        /// Selects a color.
        /// </summary>
        /// <param name="colorIndex">The index of the color to select.</param>
        public void SelectColor(int colorIndex)
        {
            // Start at 1, because the first one is the display button.
            for (int i = 1; i < colorDropdown.ChildCount(); i++)
            {
                if (colorDropdown.ChildAt(i).backgroundColor == this.colors[colorIndex])
                {
                    colorDropdown.SelectItem(i - 1);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the index of this panel, and updates it's location
        /// </summary>
        /// <param name="index"></param>
        public void UpdateBounds(int index)
        {
            this.index = index;
            this.bounds = new Rectangle(5, 5 + (componentHeight + componentSpacing) * this.index,
                   480,
                   componentHeight);
        }

        /// <summary>
        /// Gets the currently selected color.
        /// </summary>
        /// <returns>The color that was selected by the user.</returns>
        public Color GetSelectedColor()
        {
            return colorDropdown.GetSelectedButton().backgroundColor;
        }
    }
}
