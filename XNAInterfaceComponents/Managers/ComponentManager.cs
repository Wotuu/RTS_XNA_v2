#region Old file
/*
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
                if (pc is XNADialog)
                {
                    if( pc.RequestFocusAt(m_event.location) ) return;
                }
            }
            // Otherwise, the rest comes
            foreach (ParentComponent pc in this.componentList)
            {
                if (pc.RequestFocusAt(m_event.location)) return;
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
                    break;
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
*/
#endregion
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
                if (pc is XNADialog)
                {
                    if( pc.RequestFocusAt(m_event.location) ) return;
                }
            }
            // Otherwise, the rest comes
            foreach (ParentComponent pc in this.componentList)
            {
                if (pc.RequestFocusAt(m_event.location)) return;
            }
        }

        public void OnMouseRelease(MouseEvent m_event)
        {

        }

        private Component previousMouseOver = null;
        public void OnMouseMotion(MouseEvent e)
        {
            LinkedList<ParentComponent> sortedList = this.SortComponentsByZ(this.componentList, false);

            foreach (ParentComponent pc in sortedList)
            {
                Component mouseOver = pc.GetComponentAt(e.location);

                if (previousMouseOver != null && previousMouseOver != mouseOver) previousMouseOver.OnMouseExit(e);
                
                //FireMouseExitEvents(e);
                if (mouseOver != null)
                {
                    if (!mouseOver.isMouseOver)
                    {
                        ((MouseOverable)mouseOver).OnMouseEnter(e);
                    }
                    previousMouseOver = mouseOver;
                    break;
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


        #region Sort components by Z
        /// <summary>
        /// Sorts a linked list of components by their Z value.
        /// </summary>
        /// <param name="toSort">The list to sort.</param>
        /// <returns>The sorted list.</returns>
        public LinkedList<ParentComponent> SortComponentsByZ(LinkedList<ParentComponent> toSort, Boolean asc)
        {
            // Bubble sort :]
            var sorted = false;
            if (toSort.Count() < 2) return toSort;
            while (!sorted)
            {
                ParentComponent previous = null;
                for (var i = 0; i < toSort.Count(); i++)
                {
                    ParentComponent current = toSort.ElementAt(i);

                    if (previous != null && current.z > previous.z)
                    {
                        toSort.AddAfter(toSort.Find(toSort.ElementAt(i - 1)), current);
                        toSort.AddAfter(toSort.Find(toSort.ElementAt(i)), previous);

                        break;
                    }


                    if (i + 1 == toSort.Count())
                    {
                        sorted = true;
                    }

                    previous = current;
                }
            }
            if (!asc)
            {
                LinkedList<ParentComponent> reversedList = new LinkedList<ParentComponent>();
                for (int i = toSort.Count - 1; i > -1; i--)
                {
                    reversedList.AddLast(toSort.ElementAt(i));
                }
                return reversedList;
            }
            return toSort;
        }

        /// <summary>
        /// Sorts a linked list of components by their Z value.
        /// </summary>
        /// <param name="toSort">The list to sort.</param>
        /// <returns>The sorted list.</returns>
        public LinkedList<Component> SortComponentsByZ(LinkedList<Component> toSort, Boolean asc)
        {
            // Bubble sort :]
            var sorted = false;
            if (toSort.Count() < 2) return toSort;
            while (!sorted)
            {
                Component previous = null;
                for (var i = 0; i < toSort.Count(); i++)
                {
                    Component current = toSort.ElementAt(i);

                    if (previous != null && current.z > previous.z)
                    {
                        toSort.AddAfter(toSort.Find(toSort.ElementAt(i - 1)), current);
                        toSort.AddAfter(toSort.Find(toSort.ElementAt(i)), previous);

                        break;
                    }


                    if (i + 1 == toSort.Count())
                    {
                        sorted = true;
                    }

                    previous = current;
                }
            }
            if (!asc)
            {
                LinkedList<Component> reversedList = new LinkedList<Component>();
                for (int i = toSort.Count - 1; i > -1; i--)
                {
                    reversedList.AddLast(toSort.ElementAt(i));
                }
                return reversedList;
            }
            return toSort;
        }
        #endregion
    }
}
