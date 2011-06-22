using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RTS_Builder.Buildings;

namespace RTS_Builder
{
    public abstract class Building
    {

        public Player p { get; set; }
        public Vector2 location { get; set; }
        public Color c { get; set; }
        public Color previewC = new Color(175, 175, 175, 80);
        public Color constructC { get; set; }
        
        public Boolean selected { get; set; }
        public BuildState state { get; set; }

        public BuildingType type { get; set; }
        public Texture2D texture { get; set; }

        public enum BuildingType
        {
            Resources,
            Barracks,
            Factory,
            Fortress
        }

        public enum BuildState
        {
            Preview,
            Constructing,
            Finished
        }

        public void Update(KeyboardState ks, MouseState ms)
        {
            if (state == BuildState.Preview)
            {
                Vector2 newPos = new Vector2(ms.X - (texture.Width / 2), ms.Y - (texture.Height / 2));
                this.location = newPos;
            }
            else if (state == BuildState.Constructing)
            {
                if (this.constructC.A < 255)
                {
                    Color newColor = this.constructC;
                    newColor.A += 1;
                    this.constructC = newColor;
                }
                else
                {
                    state = BuildState.Finished;
                }
            }
        }

        internal void Draw(SpriteBatch sb)
        {
            if (state == BuildState.Preview)
            {
                sb.Draw(texture, location, previewC);
            }
            else if (state == BuildState.Constructing)
            {
                sb.Draw(texture, location, constructC);
            }
            else if (state == BuildState.Finished)
            {
                sb.Draw(texture, location, c);
            }
        }

        public Rectangle DefineRectangle()
        {
            return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height);
        }

        public Rectangle DefineSelectedRectangle()
        {
            return new Rectangle((int)location.X - 3, (int)location.Y - 3, texture.Width + 6, texture.Height + 6);
        }

        public void Dispose()
        {
            p.buildings.Remove(this);
        }
    }
}
