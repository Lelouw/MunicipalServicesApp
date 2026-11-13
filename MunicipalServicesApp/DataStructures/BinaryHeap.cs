using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Binary max-heap. The most urgent open service request sits at the root.
    /// .NET Framework 4.8 has no built-in priority queue. A status change updates the
    /// stored priority and sifts that request up or down, so the work queue does not
    /// have to be rebuilt from scratch.
    /// </summary>
    public sealed class BinaryHeap<T>
    {
        private sealed class Entry
        {
            public string Id;
            public int Priority;
            public int TieBreak;
            public T Value;
        }

        private readonly List<Entry> heap = new List<Entry>();
        private readonly Dictionary<string, int> indexOf = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public int Count
        {
            get { return heap.Count; }
        }

        public bool Contains(string id)
        {
            return id != null && indexOf.ContainsKey(id);
        }

        public void Upsert(string id, int priority, int tieBreak, T value)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A heap entry needs an id.", nameof(id));
            }

            int existing;
            if (indexOf.TryGetValue(id, out existing))
            {
                heap[existing].Priority = priority;
                heap[existing].TieBreak = tieBreak;
                heap[existing].Value = value;
                SiftUp(existing);
                SiftDown(indexOf[id]);
                return;
            }

            heap.Add(new Entry
            {
                Id = id,
                Priority = priority,
                TieBreak = tieBreak,
                Value = value
            });
            indexOf[id] = heap.Count - 1;
            SiftUp(heap.Count - 1);
        }

        public bool Remove(string id)
        {
            int index;
            if (id == null || !indexOf.TryGetValue(id, out index))
            {
                return false;
            }

            int last = heap.Count - 1;
            Swap(index, last);
            string movedId = index == last ? null : heap[index].Id;
            heap.RemoveAt(last);
            indexOf.Remove(id);
            if (movedId != null)
            {
                SiftUp(indexOf[movedId]);
                SiftDown(indexOf[movedId]);
            }

            return true;
        }

        public T Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            return heap[0].Value;
        }

        public IList<T> ToPriorityOrder()
        {
            var copy = new BinaryHeap<T>();
            for (int i = 0; i < heap.Count; i++)
            {
                copy.Upsert(heap[i].Id, heap[i].Priority, heap[i].TieBreak, heap[i].Value);
            }

            var ordered = new List<T>(copy.Count);
            while (copy.Count > 0)
            {
                ordered.Add(copy.heap[0].Value);
                copy.Remove(copy.heap[0].Id);
            }

            return ordered;
        }

        public void Validate()
        {
            for (int i = 0; i < heap.Count; i++)
            {
                int stored;
                if (!indexOf.TryGetValue(heap[i].Id, out stored) || stored != i)
                {
                    throw new InvalidOperationException("Heap index map is out of date.");
                }

                int left = (i * 2) + 1;
                int right = left + 1;
                if (left < heap.Count && Outranks(heap[left], heap[i]))
                {
                    throw new InvalidOperationException("Heap order is broken on the left child.");
                }

                if (right < heap.Count && Outranks(heap[right], heap[i]))
                {
                    throw new InvalidOperationException("Heap order is broken on the right child.");
                }
            }
        }

        private static bool Outranks(Entry candidate, Entry current)
        {
            if (candidate.Priority != current.Priority)
            {
                return candidate.Priority > current.Priority;
            }

            return candidate.TieBreak < current.TieBreak;
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
            Entry temp = heap[first];
            heap[first] = heap[second];
            heap[second] = temp;
            indexOf[heap[first].Id] = first;
            indexOf[heap[second].Id] = second;
        }
    }
}
