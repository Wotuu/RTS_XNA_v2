using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RTS_Builder.Buildings
{
    public class Factory : Building
    {

        public Factory(Player p, Color c)
        {
            this.p = p;
            this.c = c;
            this.type = BuildingType.Resources;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Factory");

            this.state = BuildState.Preview;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
        }
    }
}
