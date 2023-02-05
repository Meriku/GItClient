using System;
using System.Collections;
using System.Collections.Generic;

namespace GItClient.Core.Models
{
    internal class CircularList<T> : IEnumerable
    {
        private CircularListItem<T> Head { get; set; }
        private int MaxCount { get; set; }
        public int Count { get; set; }


        public CircularList(int count) 
        {
            MaxCount = count;
        }

        public void Add(T data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            var item = new CircularListItem<T>(data);

            if (Head == null)
            {
                Head = item;
                Head.Next = Head;
                Head.Previous = Head;
                Count++;
                return;
            }

            if (Count == MaxCount)
            {
                item.Next = Head.Next;
                item.Previous = Head.Previous;

                item.Previous.Next = item;
                item.Next.Previous = item;

                Head = Head.Next;
                return;
  
            }

            var currentItem = Head;
            for (var i = 0; i < MaxCount; i++)
            {
                if (currentItem.Next == Head)
                {
                    item.Previous = currentItem;
                    item.Next = Head;

                    item.Previous.Next = item;
                    item.Next.Previous = item;

                    Count++;
                    return;
                }
                else
                {
                    currentItem = currentItem.Next;
                }
            }



        }

        public IEnumerator GetEnumerator()
        {
            var current = Head;

            for (var i = 0; i < Count; i++)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
        public IEnumerable<T> GetReversed()
        {
            var current = Head.Previous;
            for (var i = 0; i < Count; i++)
            {
                yield return current.Data;
                current = current.Previous;
            }
        }
    }

    internal class CircularListItem<T>
    {
        public T Data { get; set; }

        public CircularListItem<T> Previous { get; set; }

        public CircularListItem<T> Next { get; set; }

        public CircularListItem(T data)
        {
            Data = data;
        }

    }
}
