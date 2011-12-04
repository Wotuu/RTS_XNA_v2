using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CustomLists.Lists
{
    public class CustomArrayList<T> : AbstractCustomList<T>
    {
        private volatile T[] elements;
        private volatile int currentIndex = 0;


        public CustomArrayList()
        {
            // 8 to get started
            this.elements = new T[8];
        }

        public CustomArrayList(LinkedList<T> list)
        {
            // 8 to get started
            this.elements = new T[8];
            for (int i = 0; i < list.Count; i++)
            {
                this.AddLast(list.ElementAt(i));
            }
        }

        /// <summary>
        /// Trims the list to a certain size. If the list is greater or equal than capacity, elements will be deleted.
        /// If the list is smaller than capacity, nothing is changed.
        /// </summary>
        /// <param name="capacity">The capacity to trim to</param>
        public void Trim(int capacity)
        {
            if (this.elements.Length >= capacity)
            {
                T[] newArray = new T[capacity];
                for (int i = 0; i < capacity; i++)
                {
                    if (elements[i] != null) newArray[i] = this.elements[i];
                }
                this.elements = newArray;
                this.currentIndex = capacity;
            }
        }

        /// <summary>
        /// Ensures the capacity in this array list.
        /// </summary>
        /// <param name="capacity">The capacity to ensure.</param>
        public void EnsureCapacity(int capacity)
        {
            if (this.elements == null) this.elements = new T[capacity];
            else if (this.elements.Length < capacity)
            {
                T[] newArray = new T[capacity];
                for (int i = 0; i < this.elements.Length; i++)
                {
                    if (elements[i] != null) newArray[i] = this.elements[i];
                }
                this.elements = newArray;
            } // else do nothing
        }

        /// <summary>
        /// Gets the count of elements in this list.
        /// </summary>
        /// <returns>The count.</returns>
        public override int Count()
        {
            return this.currentIndex;
        }

        /// <summary>
        /// Gets an element at a position in the list
        /// </summary>
        /// <returns></returns>
        public override T ElementAt(int index)
        {
            if (index < 0 || this.elements.Length <= index) return default(T);
            else return this.elements[index];
        }

        /// <summary>
        /// Doubles the capacity of this array list.
        /// </summary>
        private void DoubleCapacity()
        {
            this.EnsureCapacity(this.Count() * 2);
        }

        /// <summary>
        /// Adds an element to the last position in the array.
        /// </summary>
        public override void AddLast(T item)
        {
            if (this.elements.Length <= currentIndex) this.DoubleCapacity();
            this.elements[currentIndex] = item;
            currentIndex++;
        }

        /// <summary>
        /// Removes an element from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Whether or not an item was removed</returns>
        public override bool Remove(T item)
        {
            return this.Remove(item, false);
        }

        /// <summary>
        /// Removes an element from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="retainOrder">Whether to retain the order of the list or not.</param>
        /// <returns>If an element was removed or not</returns>
        public bool Remove(T item, Boolean retainOrder)
        {
            if (retainOrder)
            {
                int removedCount = 0;
                for (int i = 0; i < this.currentIndex - removedCount; i++)
                {
                    this.elements[i] = this.elements[i + removedCount];
                    if (this.ElementAt(i + removedCount).Equals(item))
                    {
                        removedCount++;
                        if (i + removedCount < currentIndex)
                        {
                            this.elements[i] = this.elements[i + removedCount];
                        }
                    }
                }
                currentIndex -= removedCount;
                if (removedCount > 0) return true;
                else return false;
            }
            else
            {
                for (int i = 0; i < this.currentIndex; i++)
                {
                    if (this.ElementAt(i).Equals(item))
                    {
                        this.elements[i] = this.GetLast();
                        this.elements[currentIndex - 1] = default(T);
                        currentIndex--;
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Removes an item at a certain position
        /// </summary>
        /// <param name="index">The index to remove</param>
        /// <returns>Whether the operation removed an element</returns>
        public override bool Remove(int index)
        {
            if (this.currentIndex <= 0) return false;
            else if (this.currentIndex < index) return false;
            this.elements[index] = this.GetLast();
            this.elements[currentIndex] = default(T);
            currentIndex--;
            return true;
        }

        /// <summary>
        /// Performs a foreach on the passed function. A T parameter will be given for the current element that's being
        /// processed in the loop.
        /// </summary>
        /// <param name="function">The function you want executed on every pass of the loop.</param>
        public override void Each(Delegate function)
        {
            for (int i = 0; i < this.currentIndex; i++)
            {
                function.DynamicInvoke(this.elements[i], i);
            }
        }

        /// <summary>
        /// Gets the first element of this arraylist.
        /// </summary>
        /// <returns>The first element.</returns>
        public override T GetFirst()
        {
            if (this.currentIndex <= 0) return default(T);
            return this.elements[0];
        }

        /// <summary>
        /// Gets the last element of this arraylist.
        /// </summary>
        /// <returns>The last element.</returns>
        public override T GetLast()
        {
            if (this.currentIndex <= 0) return default(T);

            return this.elements[currentIndex - 1];
        }

        /// <summary>
        /// Prints all elements to the console, using Debug.WriteLine();
        /// </summary>
        public override void PrintAll()
        {
            for (int i = 0; i < currentIndex; i++)
            {
                Debug.WriteLine(this.elements[i]);
            }
        }

        /// <summary>
        /// Clears the list completely.
        /// </summary>
        public override void Clear()
        {
            this.elements = new T[1];
            this.currentIndex = 0;
        }

        /// <summary>
        /// Checks if an item exists in this list.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Yes or no.</returns>
        public override bool Contains(T item)
        {
            for (int i = 0; i < this.currentIndex; i++)
            {
                if (this.elements[i].Equals(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an element to the first of the list
        /// </summary>
        /// <param name="item"></param>
        public override void AddFirst(T item)
        {
            this.AddFirst(item, false);
        }

        /// <summary>
        /// Adds an item to the first of the list.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="retainOrder">Whether to retain the order of the list or not. If false, the first element is 
        /// added to the last of the list, and the new item inserted in the first. Otherwise, the entire list will be shifted (slower).</param>
        public void AddFirst(T item, Boolean retainOrder)
        {
            if (retainOrder)
            {
                this.EnsureCapacity(this.currentIndex + 2);
                for (int i = currentIndex; i >= 0; i--)
                {
                    this.elements[i + 1] = this.elements[i];
                }
                this.elements[0] = item;
            }
            else
            {
                this.AddLast(this.GetFirst());
                this.elements[0] = item;
            }
            currentIndex++;
        }

        /// <summary>
        /// Removes the first element of the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public override void RemoveFirst()
        {
            this.RemoveFirst(false);
        }

        /// <summary>
        /// Removes the first element of the list.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <param name="retainOrder">Whether to retain the order of the list or not. If false, the last element replaces the 
        /// current first element. Otherwise, the entire list will be shifted (slower).</param>
        public void RemoveFirst(Boolean retainOrder)
        {
            if (retainOrder)
            {
                for (int i = 1; i < currentIndex; i++)
                {
                    this.elements[i - 1] = this.elements[i];
                }
            }
            else
            {
                this.elements[0] = this.GetLast();
            }
            // Null the last element, just in case
            if (currentIndex > 0)
            {
                this.elements[currentIndex - 1] = default(T);
                this.currentIndex--;
            }
        }

        /// <summary>
        /// Converts this CustomArrayList to an array.
        /// </summary>
        /// <returns>The array containing all elements inside the list.</returns>
        public override T[] ToArray()
        {
            T[] newArray = new T[currentIndex];
            for (int i = 0; i < this.currentIndex; i++)
            {
                newArray[i] = this.elements[i];
            }
            return newArray;
        }

        /// <summary>
        /// Copies this list to a new one.
        /// </summary>
        /// <returns>The Linked List</returns>
        public override CustomArrayList<T> Copy()
        {
            CustomArrayList<T> list = new CustomArrayList<T>();
            for (int i = 0; i < this.currentIndex; i++)
            {
                list.AddLast(this.elements[i]);
            }
            return list;
        }

        /// <summary>
        /// Converts this CustomArrayList to a LinkedList.
        /// </summary>
        /// <returns>The Linked List</returns>
        public override LinkedList<T> ToLinkedList()
        {
            LinkedList<T> list = new LinkedList<T>();
            for (int i = 0; i < this.currentIndex; i++)
            {
                list.AddLast(this.elements[i]);
            }
            return list;
        }

        /// <summary>
        /// Gets the index of given item, or -1 otherwise.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The index, or -1 otherwise.</returns>
        public override int IndexOf(T item)
        {
            for (int i = 0; i < this.currentIndex; i++)
            {
                if (this.elements[i].Equals(item)) return i;
            }
            return -1;
        }
    }
}
