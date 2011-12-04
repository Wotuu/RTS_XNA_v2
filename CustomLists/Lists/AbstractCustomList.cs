using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomLists.Lists
{
    public delegate void Delegate(String T, int index);

    public abstract class AbstractCustomList<T>
    {
        protected readonly object syncLock = new object();
        public abstract int Count();
        public abstract T ElementAt(int index);
        public abstract Boolean Contains(T item);
        public abstract int IndexOf(T item);

        public abstract T GetFirst();
        public abstract T GetLast();

        public abstract void AddFirst(T item);
        public abstract void AddLast(T item);

        public abstract Boolean Remove(T item);
        public abstract Boolean Remove(int index);

        public abstract void RemoveFirst();

        public abstract void Each(Delegate function);
        public abstract void Clear();

        public abstract void PrintAll();
        public abstract T[] ToArray();

        public abstract CustomArrayList<T> Copy();
        public abstract LinkedList<T> ToLinkedList();
    }
}
