using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace RTS_Builder.Buildings
{
    public class ResourceGather : Building
    {

        public ResourceGather(Player p, Color c)
        {
            this.p = p;
            this.c = c;
            this.type = BuildingType.Resources;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Resources");

            this.state = BuildState.Preview;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
        }
    }
}