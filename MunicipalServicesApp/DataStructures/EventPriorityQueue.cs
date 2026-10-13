using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Max-heap priority queue for announcements. .NET Framework 4.8 has no built-in
    /// PriorityQueue, so urgent notices are ordered here: higher priority first, then earlier date.
    /// </summary>
    public sealed class EventPriorityQueue
    {
        private readonly List<LocalEvent> heap = new List<LocalEvent>();

        public int Count
        {
            get { return heap.Count; }
        }

        public void Enqueue(LocalEvent item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            heap.Add(item);
            SiftUp(heap.Count - 1);
        }

        public LocalEvent Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            return heap[0];
        }

        public LocalEvent Dequeue()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            LocalEvent top = heap[0];
            int lastIndex = heap.Count - 1;
            heap[0] = heap[lastIndex];
            heap.RemoveAt(lastIndex);

            if (heap.Count > 0)
            {
                SiftDown(0);
            }

            return top;
        }

        public IList<LocalEvent> ToPriorityOrder()
        {
            var copy = new EventPriorityQueue();
            for (int i = 0; i < heap.Count; i++)
            {
                copy.Enqueue(heap[i]);
            }

            var ordered = new List<LocalEvent>(copy.Count);
            while (copy.Count > 0)
            {
                ordered.Add(copy.Dequeue());
            }

            return ordered;
        }

        private static bool Outranks(LocalEvent candidate, LocalEvent current)
        {
            if (candidate.Priority != current.Priority)
            {
                return candidate.Priority > current.Priority;
            }

            return candidate.StartDate < current.StartDate;
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (!Outranks(heap[index], heap[parent]))
                {
                    break;
                }

                Swap(index, parent);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                int left = (index * 2) + 1;
                int right = left + 1;
                int best = index;

                if (left < heap.Count && Outranks(heap[left], heap[best]))
                {
                    best = left;
                }

                if (right < heap.Count && Outranks(heap[right], heap[best]))
                {
                    best = right;
                }

                if (best == index)
                {
                    break;
                }

                Swap(index, best);
                index = best;
            }
        }

        private void Swap(int first, int second)
        {
            LocalEvent temp = heap[first];
            heap[first] = heap[second];
            heap[second] = temp;
        }
    }
}
