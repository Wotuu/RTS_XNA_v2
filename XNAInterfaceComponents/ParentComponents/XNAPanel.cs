using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.Managers;
using XNAInterfaceComponents.ParentComponents;

namespace XNAInterfaceComponents.Components
{
    public class XNAPanel : ParentComponent
    {
        public XNAPanel(ParentComponent parent, Rectangle bounds)
            : base(parent, bounds)
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            if (!this.visible) return;
            if (this.clearTexture == null) clearTexture = ComponentUtil.GetClearTexture2D(sb);
            // Console.Out.WriteLine("Drawing panel!");

            Color drawColor = new Color();
            //if (this.isMouseOver) drawColor = this.mouseOverColor;
            //else 
            drawColor = this.backgroundColor;

            sb.Draw(clearTexture, this.GetScreenBounds(), drawColor);
            if (this.border != null) this.border.Draw(sb);

            for (int i = 0; i < this.children.Count; i++)
            {
                Component c = this.children.ElementAt(i);
                if (!(c is XNADropdown))
                {
                    c.Draw(sb);
                }
            }
            for (int i = 0; i < this.children.Count; i++)
            {
                Component c = this.children.ElementAt(i);
                if (c is XNADropdown)
                {
                    c.Draw(sb);
                }
            }
        }

        public override void Update()
        {
            for (int i = 0; i < this.children.Count; i++)
            {
                Component c = this.children.ElementAt(i);
                c.Update();
            }
        }

        public override void OnMouseEnter(MouseEvent m_event)
        {
            this.isMouseOver = true;
            // Console.Out.WriteLine("Panel @ " + this.GetScreenLocation() + " mouse entered!");
        }

        public override void OnMouseExit(MouseEvent m_event)
        {
            this.isMouseOver = false;
            // Console.Out.WriteLine("Panel @ " + this.GetScreenLocation() + " mouse exitted!");
        }

        public override void Unload()
        {
            // Console.Out.WriteLine("Unloading a " + this.GetType().FullName);

            if (this.parent == null) ComponentManager.GetInstance().QueueUnload(this);
            else this.parent.RemoveChild(this);

            for (int i = 0; i < children.Count; i++)
            {
                children.ElementAt(i).Unload();
            }
        }
    }
}
