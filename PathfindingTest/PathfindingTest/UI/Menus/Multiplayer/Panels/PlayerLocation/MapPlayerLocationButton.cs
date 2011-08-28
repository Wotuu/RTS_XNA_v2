using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;
using XNAInputLibrary.KeyboardInput;
using Microsoft.Xna.Framework.Input;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels.PlayerLocation
{
    public class MapPlayerLocationButton : XNAButton
    {
        public static Texture2D TEXTURE { get; set; }
        public static Texture2D MOUSE_OVER_TEXTURE { get; set; }
        public static Point SIZE { get; set; }

        public Point mapLocation { get; set; }
        public Point miniMapLocation { get; set; }

        public MapPlayerLocationGroup group { get; set; }

        static MapPlayerLocationButton()
        {
            SIZE = new Point(15, 15);
            TEXTURE = Game1.GetInstance().Content.Load<Texture2D>("Misc/playerPreview");
            MOUSE_OVER_TEXTURE = Game1.GetInstance().Content.Load<Texture2D>("Misc/playerPreviewMouseOver");
        }

        public MapPlayerLocationButton(MapPlayerLocationGroup group, XNAPanel parent, Point mapLocation, Point miniMapLocation) :
            base(parent, 
            new Rectangle(miniMapLocation.X, miniMapLocation.Y,
                SIZE.X, SIZE.Y), "" )
        {
            this.group = group;
            this.mapLocation = mapLocation;
            this.miniMapLocation = miniMapLocation;
            this.backgroundColor = Color.Transparent;
            this.mouseOverColor = Color.Transparent;

            this.font = MenuManager.BUTTON_FONT;
            this.fontColor = Color.Red;

            this.border = null;

            this.onClickListeners += this.OnClick;
        }

        /// <summary>
        /// Draws the map player location panel on the screen.
        /// </summary>
        /// <param name="sb">Spritebatch to draw on.</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (!this.visible) return;
            Texture2D drawTexture = TEXTURE;

            this.bounds = new Rectangle(miniMapLocation.X + group.offset.X - SIZE.X / 2,
                miniMapLocation.Y + group.offset.Y - SIZE.Y / 2, SIZE.X, SIZE.Y);

            Rectangle drawBounds = this.GetScreenBounds();
            MouseState state = Mouse.GetState();

            if ( ( this.parent is SPMapSelectionPanel || 
                this.parent is MapPreviewPanel ) &&
                drawBounds.Contains( state.X, state.Y ) ) drawTexture = MOUSE_OVER_TEXTURE; 

            sb.Draw(drawTexture, drawBounds,
                null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, this.parent.z - 0.002f);
        }

        /// <summary>
        /// When this button was clicked.
        /// </summary>
        /// <param name="button">The button, bla</param>
        public void OnClick(XNAButton button)
        {
            if (this.parent is SPMapSelectionPanel)
            {
                this.group.OnPlayerIndexChanged(this, 1);
            }
            else if (this.parent is MapPreviewPanel)
            {
                // We're currently in the lobby
                GameLobby lobby = (GameLobby)MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                int userID = ChatServerConnectionManager.GetInstance().user.id;

                this.group.OnPlayerIndexChangedMP(userID, this.group.GetIndexOfButton(this));
            }
        }

        public override void Unload()
        {
            base.Unload();
            this.onClickListeners -= this.OnClick;
        }
    }
}
