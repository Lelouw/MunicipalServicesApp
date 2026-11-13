using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Binary search tree. Keys to the left are smaller and keys to the right are larger.
    /// Service request references are stored here so a resident can look up one reference
    /// without scanning the whole list. The search path is kept so the screen can show it.
    /// </summary>
    public sealed class BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
        }

        private Node root;

        public int Count { get; private set; }

        public int Height
        {
            get { return HeightOf(root); }
        }

        public void Insert(TKey key, TValue value)
        {
            root = Insert(root, key, value);
        }

        public void InsertInBalancedOrder(IList<KeyValuePair<TKey, TValue>> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            var sorted = new List<KeyValuePair<TKey, TValue>>(items);
            sorted.Sort(delegate (KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
            {
                return left.Key.CompareTo(right.Key);
            });
            InsertMiddle(sorted, 0, sorted.Count - 1);
        }

        public bool TryGetValue(TKey key, out TValue value, IList<TKey> path)
        {
            Node node = root;
            while (node != null)
            {
                if (path != null)
                {
                    path.Add(node.Key);
                }

                int compared = key.CompareTo(node.Key);
                if (compared == 0)
                {
                    value = node.Value;
                    return true;
                }

                node = compared < 0 ? node.Left : node.Right;
            }

            value = default(TValue);
            return false;
        }

        public bool Remove(TKey key)
        {
            bool removed = false;
            root = Remove(root, key, ref removed);
            if (removed)
            {
                Count--;
            }

            return removed;
        }

        public IList<TValue> InOrder()
        {
            var values = new List<TValue>(Count);
            Walk(root, values);
            return values;
        }

        public void Validate()
        {
            int counted = 0;
            TKey previous = default(TKey);
            bool hasPrevious = false;
            ValidateNode(root, ref counted, ref previous, ref hasPrevious);
            if (counted != Count)
            {
                throw new InvalidOperationException("Binary search tree count does not match the nodes.");
            }
        }

        private void InsertMiddle(List<KeyValuePair<TKey, TValue>> items, int low, int high)
        {
            if (low > high)
            {
                return;
            }

            int middle = low + ((high - low) / 2);
            Insert(items[middle].Key, items[middle].Value);
            InsertMiddle(items, low, middle - 1);
            InsertMiddle(items, middle + 1, high);
        }

        private Node Insert(Node node, TKey key, TValue value)
        {
            if (node == null)
            {
                Count++;
                return new Node
                {
                    Key = key,
                    Value = value
                };
            }

            int compared = key.CompareTo(node.Key);
            if (compared < 0)
            {
                node.Left = Insert(node.Left, key, value);
            }
            else if (compared > 0)
            {
                node.Right = Insert(node.Right, key, value);
            }
            else
            {
                node.Value = value;
            }

            return node;
        }

        private static Node Remove(Node node, TKey key, ref bool removed)
        {
            if (node == null)
            {
                return null;
            }

            int compared = key.CompareTo(node.Key);
            if (compared < 0)
            {
                node.Left = Remove(node.Left, key, ref removed);
                return node;
            }

            if (compared > 0)
            {
                node.Right = Remove(node.Right, key, ref removed);
                return node;
            }

            removed = true;
            if (node.Left == null)
            {
                return node.Right;
            }

            if (node.Right == null)
            {
                return node.Left;
            }

            Node successor = node.Right;
            while (successor.Left != null)
            {
                successor = successor.Left;
            }

            node.Key = successor.Key;
            node.Value = successor.Value;
            bool ignored = false;
            node.Right = Remove(node.Right, successor.Key, ref ignored);
            return node;
        }

        private static void Walk(Node node, IList<TValue> values)
        {
            if (node == null)
            {
                return;
            }

            Walk(node.Left, values);
            values.Add(node.Value);
            Walk(node.Right, values);
        }

        private static int HeightOf(Node node)
        {
            if (node == null)
            {
                return 0;
            }

            return 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));
        }

        private static void ValidateNode(Node node, ref int counted, ref TKey previous, ref bool hasPrevious)
        {
            if (node == null)
            {
                return;
            }

            ValidateNode(node.Left, ref counted, ref previous, ref hasPrevious);
            if (hasPrevious && previous.CompareTo(node.Key) >= 0)
            {
                throw new InvalidOperationException("Binary search tree in-order walk is not sorted.");
            }

            previous = node.Key;
            hasPrevious = true;
            counted++;
            ValidateNode(node.Right, ref counted, ref previous, ref hasPrevious);
        }
    }
}
