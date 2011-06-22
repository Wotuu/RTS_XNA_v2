using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.Managers;
using XNAInterfaceComponents.Interfaces;

namespace XNAInterfaceComponents.AbstractComponents
{
    public abstract class ParentComponent : Component
    {
        protected LinkedList<Component> children { get; set; }

        public ParentComponent(ParentComponent parent, Rectangle bounds)
            : base(bounds)
        {
            this.parent = parent;
            this.children = new LinkedList<Component>();
            if (parent == null) ComponentManager.GetInstance().QueueLoad(this);
            else this.parent.children.AddLast(this);
        }

        #region Child functions
        /// <summary>
        /// Adds a child to this component.
        /// </summary>
        /// <param name="component">The component to add to this parent component</param>
        public void AddChild(Component component)
        {
            this.children.AddLast(component);
            component.parent = this;
        }

        /// <summary>
        /// Removes a child from this component.
        /// </summary>
        /// <param name="component">The component to remove.</param>
        /// <returns>Whether the child was removed or not.</returns>
        public Boolean RemoveChild(Component component)
        {
            if (this.children.Contains(component))
            {
                component.parent = null;
                this.children.Remove(component);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Gets the amount of children in this component.
        /// </summary>
        /// <returns>The amount</returns>
        public int ChildCount() { return this.children.Count; }

        /// <summary>
        /// Gets a component at an index.
        /// </summary>
        /// <param name="index">The index to get a component at.</param>
        /// <returns>The component</returns>
        public Component ChildAt(int index) { return this.children.ElementAt(index); }

        #endregion

        /// <summary>
        /// Requests the screen location of this component. Childs can use this function to get 
        /// their exact location on the screen, instead of relative to their parent.
        /// </summary>
        /// <param name="childLocation">The location of the last child.</param>
        /// <returns>The screen location.</returns>
        public Point RequestScreenLocation(Point childLocation)
        {
            Point newPoint = new Point(this.bounds.X + childLocation.X, this.bounds.Y + childLocation.Y);
            if (this.parent != null)
            {
                return parent.RequestScreenLocation(newPoint);
            }
            else return newPoint;
        }

        /// <summary>
        /// The most top displayed component will grab focus at this location.
        /// </summary>
        /// <param name="p">The point to check for focus</param>
        /// <returns>The flag.</returns>
        public Boolean RequestFocusAt(Point p)
        {
            Component focussedComponent = null;
            foreach (Component child in this.children)
            {
                if (!child.visible) continue;
                if (child is Focusable)
                {
                    if (child.GetScreenBounds().Contains(p))
                    {
                        Focusable f = ((Focusable)child);
                        if (!child.isFocussed) f.OnFocusReceived();
                        focussedComponent = child;
                    }
                    if (focussedComponent != child && child.isFocussed)
                    {
                        ((Focusable)child).OnFocusLost();
                    }
                }
                else if (child is ParentComponent)
                {
                    Boolean result = ((ParentComponent)child).RequestFocusAt(p);
                    if (result) return result;
                }
            }
            return focussedComponent != null;
        }

        /// <summary>
        /// Gets the component at a location (may be this component if no childs contain the location)
        /// </summary>
        /// <returns>The component</returns>
        public Component GetComponentAt(Point p)
        {
            foreach (Component child in this.children)
            {
                if (!child.visible) continue;
                if (child is ParentComponent)
                {
                    Component c = ((ParentComponent)child).GetComponentAt(p);
                    if (c != null && !(c is ParentComponent)) return c;
                }
                if (child.GetScreenBounds().Contains(p))
                {
                    return child;
                }
            }
            if (this.GetScreenBounds().Contains(p)) return this;
            return null;
        }
    }
}
