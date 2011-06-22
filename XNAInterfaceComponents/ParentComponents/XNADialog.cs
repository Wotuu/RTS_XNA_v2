using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.Misc;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.AbstractComponents;

namespace XNAInterfaceComponents.ParentComponents
{
    public abstract class XNADialog : XNAPanel
    {

        public static int CLIENT_WINDOW_WIDTH { get; set; }
        public static int CLIENT_WINDOW_HEIGHT { get; set; }


        public Padding padding { get; set; }
        public SpriteFont font { get; set; }

        public XNADialog()
            : base(null,
                new Rectangle())
        {
            this.font = ChildComponent.DEFAULT_FONT;
            this.padding = new Padding(5, 5, 5, 5);
            this.border = new Border(this, 3, Color.Black);
        }

        public abstract void DoLayout();


        /// <summary>
        /// Every button calls this, as in .. on click, remove!
        /// </summary>
        /// <param name="source">The button that clicked it (irrelevant schmuck)</param>
        public void Dispose(XNAButton source)
        {
            this.Dispose();
        }

        /// <summary>
        /// Disposes the message dialog. Does the same as calling Unload();.
        /// </summary>
        public void Dispose()
        {
            this.Unload();
        }
    }
}
