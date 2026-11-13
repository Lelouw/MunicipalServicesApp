using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// AVL tree. After every insert or removal the node is rotated until the heights of
    /// its two children differ by at most one. Reports arrive in date order, which would
    /// stretch a plain search tree into a line. This tree stays short, so date order and
    /// date-range counts stay fast.
    /// </summary>
    public sealed class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public int Height;
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

        public IList<TValue> Range(TKey from, TKey to)
        {
            var values = new List<TValue>();
            CollectRange(root, from, to, values);
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
                throw new InvalidOperationException("AVL tree count does not match the nodes.");
            }
        }

        private Node Insert(Node node, TKey key, TValue value)
        {
            if (node == null)
            {
                Count++;
                return new Node
                {
                    Key = key,
                    Value = value,
                    Height = 1
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
                return node;
            }

            return Rebalance(node);
        }

        private Node Remove(Node node, TKey key, ref bool removed)
        {
            if (node == null)
            {
                return null;
            }

            int compared = key.CompareTo(node.Key);
            if (compared < 0)
            {
                node.Left = Remove(node.Left, key, ref removed);
            }
            else if (compared > 0)
            {
                node.Right = Remove(node.Right, key, ref removed);
            }
            else
            {
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
            }

            return Rebalance(node);
        }

        private static Node Rebalance(Node node)
        {
            UpdateHeight(node);
            int balance = BalanceOf(node);
            if (balance > 1)
            {
                if (BalanceOf(node.Left) < 0)
                {
                    node.Left = RotateLeft(node.Left);
                }

                return RotateRight(node);
            }

            if (balance < -1)
            {
                if (BalanceOf(node.Right) > 0)
                {
                    node.Right = RotateRight(node.Right);
                }

                return RotateLeft(node);
            }

            return node;
        }

        private static Node RotateRight(Node node)
        {
            Node left = node.Left;
            Node moved = left.Right;
            left.Right = node;
            node.Left = moved;
            UpdateHeight(node);
            UpdateHeight(left);
            return left;
        }

        private static Node RotateLeft(Node node)
        {
            Node right = node.Right;
            Node moved = right.Left;
            right.Left = node;
            node.Right = moved;
            UpdateHeight(node);
            UpdateHeight(right);
            return right;
        }

        private static void UpdateHeight(Node node)
        {
            node.Height = 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));
        }

        private static int BalanceOf(Node node)
        {
            if (node == null)
            {
                return 0;
            }

            return HeightOf(node.Left) - HeightOf(node.Right);
        }

        private static int HeightOf(Node node)
        {
            return node == null ? 0 : node.Height;
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

        private static void CollectRange(Node node, TKey from, TKey to, IList<TValue> values)
        {
            if (node == null)
            {
                return;
            }

            if (from.CompareTo(node.Key) < 0)
            {
                CollectRange(node.Left, from, to, values);
            }

            if (from.CompareTo(node.Key) <= 0 && to.CompareTo(node.Key) >= 0)
            {
                values.Add(node.Value);
            }

            if (to.CompareTo(node.Key) > 0)
            {
                CollectRange(node.Right, from, to, values);
            }
        }

        private static void ValidateNode(Node node, ref int counted, ref TKey previous, ref bool hasPrevious)
        {
            if (node == null)
            {
                return;
            }

            int expected = 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));
            if (node.Height != expected)
            {
                throw new InvalidOperationException("AVL node height is out of date.");
            }

            int balance = BalanceOf(node);
            if (balance < -1 || balance > 1)
            {
                throw new InvalidOperationException("AVL tree is unbalanced.");
            }

            ValidateNode(node.Left, ref counted, ref previous, ref hasPrevious);
            if (hasPrevious && previous.CompareTo(node.Key) >= 0)
            {
                throw new InvalidOperationException("AVL tree in-order walk is not sorted.");
            }

            previous = node.Key;
            hasPrevious = true;
            counted++;
            ValidateNode(node.Right, ref counted, ref previous, ref hasPrevious);
        }
    }
}
