using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.AbstractComponents;
using XNAInputHandler.MouseInput;
using XNAInterfaceComponents.ParentComponents;

namespace XNAInterfaceComponents.Managers
{
    public class ComponentManager : Drawable, MouseClickListener, MouseMotionListener
    {
        private LinkedList<ParentComponent> componentList = new LinkedList<ParentComponent>();
        private LinkedList<ParentComponent> unloadList = new LinkedList<ParentComponent>();
        private LinkedList<ParentComponent> loadList = new LinkedList<ParentComponent>();
        private static ComponentManager instance = null;

        public int draws { get; set; }

        /// <summary>
        /// Draws all panels
        /// </summary>
        /// <param name="sb">The spritebatch to draw on.</param>
        public void Draw(SpriteBatch sb)
        {
            if (draws == 0) ComponentUtil.lineTexture = ComponentUtil.GetClearTexture2D(sb);
            foreach (ParentComponent c in componentList)
            {
                c.Draw(sb);
            }
            draws++;
        }

        /// <summary>
        /// Updates all the panels.
        /// </summary>
        public void Update()
        {
            foreach (ParentComponent c in componentList)
            {
                c.Update();
            }
            if (unloadList.Count > 0)
            {
                for (int i = 0; i < unloadList.Count; i++)
                {
                    ParentComponent unload = unloadList.ElementAt(i);
                    componentList.Remove(unload);
                }
                unloadList.Clear();
            }

            if (loadList.Count > 0)
            {
                foreach (ParentComponent pc in loadList)
                {
                    componentList.AddLast(pc);
                }
                loadList.Clear();
            }
        }

        /// <summary>
        /// Queues the element for loading by the mananger.
        /// </summary>
        /// <param name="component">The component to load.</param>
        public void QueueLoad(ParentComponent component)
        {
            loadList.AddLast(component);
        }

        /// <summary>
        /// Queues the element for unloading by the manager.
        /// </summary>
        /// <param name="component">The component to unload</param>
        public void QueueUnload(ParentComponent component)
        {
            unloadList.AddLast(component);
        }

        /// <summary>
        /// Unloads all panels.
        /// </summary>
        public void UnloadAllPanels()
        {
            foreach (ParentComponent c in this.componentList)
            {
                QueueUnload(c);
            }
        }


        private ComponentManager()
        {
            MouseManager.GetInstance().mouseClickedListeners += this.OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += this.OnMouseRelease;
            MouseManager.GetInstance().mouseMotionListeners += this.OnMouseMotion;
            MouseManager.GetInstance().mouseDragListeners += this.OnMouseDrag;
        }

        public static ComponentManager GetInstance()
        {
            if (instance == null) instance = new ComponentManager();
            return instance;
        }

        public void OnMouseClick(MouseEvent m_event)
        {
            // Message dialogs get top priority.
            for (int i = this.componentList.Count - 1; i >= 0; i--)
            {
                ParentComponent pc = this.componentList.ElementAt(i);
                if (pc is XNAMessageDialog)
                {
                    pc.RequestFocusAt(m_event.location);
                    return;
                }
            }
            // Otherwise, the rest comes
            foreach (ParentComponent pc in this.componentList)
            {
                pc.RequestFocusAt(m_event.location);
            }
        }

        public void OnMouseRelease(MouseEvent m_event)
        {

        }

        private Component previousMouseOver = null;
        public void OnMouseMotion(MouseEvent e)
        {
            foreach (ParentComponent pc in this.componentList)
            {
                Component mouseOver = pc.GetComponentAt(e.location);

                if (previousMouseOver != null && previousMouseOver != mouseOver) previousMouseOver.OnMouseExit(e);//FireMouseExitEvents(e);
                if (mouseOver != null)
                {
                    if (!mouseOver.isMouseOver)
                    {
                        ((MouseOverable)mouseOver).OnMouseEnter(e);
                    }
                    previousMouseOver = mouseOver;
                }
            }
        }

        /// <summary>
        /// Fire mouse exit events on those that currently have mouse over.
        /// </summary>
        /// <param name="e">The event to pass</param>
        private void FireMouseExitEvents(MouseEvent e)
        {
            for (int i = 0; i < this.componentList.Count; i++)
            {
                ParentComponent pc = this.componentList.ElementAt(i);
                if (pc.isMouseOver) ((MouseOverable)pc).OnMouseExit(e);
                for (int j = 0; j < pc.ChildCount(); j++)
                {
                    Component c = pc.ChildAt(j);
                    if (c.isMouseOver) ((MouseOverable)c).OnMouseExit(e);
                }
            }
        }

        public void OnMouseDrag(MouseEvent e)
        {
            // throw new NotImplementedException();
        }
    }
}
