using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Red-black tree. A status change deletes the old key and inserts the new one.
    /// Recolouring and rotation keep the tree short while requests move between Pending,
    /// In Progress, and Resolved. An in-order walk lists requests grouped by status.
    /// </summary>
    public sealed class RedBlackTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public Node Parent;
            public bool Red;
        }

        private readonly Node nil;
        private Node root;

        public RedBlackTree()
        {
            nil = new Node();
            nil.Red = false;
            nil.Left = nil;
            nil.Right = nil;
            nil.Parent = nil;
            root = nil;
        }

        public int Count { get; private set; }

        public int Height
        {
            get { return HeightOf(root); }
        }

        public void Insert(TKey key, TValue value)
        {
            Node parent = nil;
            Node current = root;
            while (current != nil)
            {
                parent = current;
                int compared = key.CompareTo(current.Key);
                if (compared == 0)
                {
                    current.Value = value;
                    return;
                }

                current = compared < 0 ? current.Left : current.Right;
            }

            var node = new Node
            {
                Key = key,
                Value = value,
                Left = nil,
                Right = nil,
                Parent = parent,
                Red = true
            };

            if (parent == nil)
            {
                root = node;
            }
            else if (key.CompareTo(parent.Key) < 0)
            {
                parent.Left = node;
            }
            else
            {
                parent.Right = node;
            }

            Count++;
            InsertFix(node);
            root.Red = false;
            nil.Parent = nil;
        }

        public bool Remove(TKey key)
        {
            Node node = root;
            while (node != nil)
            {
                int compared = key.CompareTo(node.Key);
                if (compared == 0)
                {
                    break;
                }

                node = compared < 0 ? node.Left : node.Right;
            }

            if (node == nil)
            {
                return false;
            }

            Delete(node);
            Count--;
            nil.Parent = nil;
            return true;
        }

        public IList<TValue> InOrder()
        {
            var values = new List<TValue>(Count);
            Walk(root, values);
            return values;
        }

        public void Validate()
        {
            if (root.Red)
            {
                throw new InvalidOperationException("Red-black root must be black.");
            }

            int counted = 0;
            int blackHeight = -1;
            TKey previous = default(TKey);
            bool hasPrevious = false;
            ValidateNode(root, 0, ref blackHeight, ref counted, ref previous, ref hasPrevious);
            if (counted != Count)
            {
                throw new InvalidOperationException("Red-black tree count does not match the nodes.");
            }
        }

        private void InsertFix(Node node)
        {
            while (node.Parent.Red)
            {
                if (node.Parent == node.Parent.Parent.Left)
                {
                    Node uncle = node.Parent.Parent.Right;
                    if (uncle.Red)
                    {
                        node.Parent.Red = false;
                        uncle.Red = false;
                        node.Parent.Parent.Red = true;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Right)
                        {
                            node = node.Parent;
                            RotateLeft(node);
                        }

                        node.Parent.Red = false;
                        node.Parent.Parent.Red = true;
                        RotateRight(node.Parent.Parent);
                    }
                }
                else
                {
                    Node uncle = node.Parent.Parent.Left;
                    if (uncle.Red)
                    {
                        node.Parent.Red = false;
                        uncle.Red = false;
                        node.Parent.Parent.Red = true;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Left)
                        {
                            node = node.Parent;
                            RotateRight(node);
                        }

                        node.Parent.Red = false;
                        node.Parent.Parent.Red = true;
                        RotateLeft(node.Parent.Parent);
                    }
                }
            }
        }

        private void Delete(Node node)
        {
            Node moved = node;
            bool movedWasRed = moved.Red;
            Node child;

            if (node.Left == nil)
            {
                child = node.Right;
                Transplant(node, node.Right);
            }
            else if (node.Right == nil)
            {
                child = node.Left;
                Transplant(node, node.Left);
            }
            else
            {
                moved = Minimum(node.Right);
                movedWasRed = moved.Red;
                child = moved.Right;
                if (moved.Parent == node)
                {
                    child.Parent = moved;
                }
                else
                {
                    Transplant(moved, moved.Right);
                    moved.Right = node.Right;
                    moved.Right.Parent = moved;
                }

                Transplant(node, moved);
                moved.Left = node.Left;
                moved.Left.Parent = moved;
                moved.Red = node.Red;
            }

            if (!movedWasRed)
            {
                DeleteFix(child);
            }
        }

        private void DeleteFix(Node node)
        {
            while (node != root && !node.Red)
            {
                if (node == node.Parent.Left)
                {
                    Node sibling = node.Parent.Right;
                    if (sibling.Red)
                    {
                        sibling.Red = false;
                        node.Parent.Red = true;
                        RotateLeft(node.Parent);
                        sibling = node.Parent.Right;
                    }

                    if (!sibling.Left.Red && !sibling.Right.Red)
                    {
                        sibling.Red = true;
                        node = node.Parent;
                    }
                    else
                    {
                        if (!sibling.Right.Red)
                        {
                            sibling.Left.Red = false;
                            sibling.Red = true;
                            RotateRight(sibling);
                            sibling = node.Parent.Right;
                        }

                        sibling.Red = node.Parent.Red;
                        node.Parent.Red = false;
                        sibling.Right.Red = false;
                        RotateLeft(node.Parent);
                        node = root;
                    }
                }
                else
                {
                    Node sibling = node.Parent.Left;
                    if (sibling.Red)
                    {
                        sibling.Red = false;
                        node.Parent.Red = true;
                        RotateRight(node.Parent);
                        sibling = node.Parent.Left;
                    }

                    if (!sibling.Right.Red && !sibling.Left.Red)
                    {
                        sibling.Red = true;
                        node = node.Parent;
                    }
                    else
                    {
                        if (!sibling.Left.Red)
                        {
                            sibling.Right.Red = false;
                            sibling.Red = true;
                            RotateLeft(sibling);
                            sibling = node.Parent.Left;
                        }

                        sibling.Red = node.Parent.Red;
                        node.Parent.Red = false;
                        sibling.Left.Red = false;
                        RotateRight(node.Parent);
                        node = root;
                    }
                }
            }

            node.Red = false;
        }

        private void Transplant(Node leaving, Node arriving)
        {
            if (leaving.Parent == nil)
            {
                root = arriving;
            }
            else if (leaving == leaving.Parent.Left)
            {
                leaving.Parent.Left = arriving;
            }
            else
            {
                leaving.Parent.Right = arriving;
            }

            arriving.Parent = leaving.Parent;
        }

        private Node Minimum(Node node)
        {
            while (node.Left != nil)
            {
                node = node.Left;
            }

            return node;
        }

        private void RotateLeft(Node node)
        {
            Node right = node.Right;
            node.Right = right.Left;
            if (right.Left != nil)
            {
                right.Left.Parent = node;
            }

            right.Parent = node.Parent;
            if (node.Parent == nil)
            {
                root = right;
            }
            else if (node == node.Parent.Left)
            {
                node.Parent.Left = right;
            }
            else
            {
                node.Parent.Right = right;
            }

            right.Left = node;
            node.Parent = right;
        }

        private void RotateRight(Node node)
        {
            Node left = node.Left;
            node.Left = left.Right;
            if (left.Right != nil)
            {
                left.Right.Parent = node;
            }

            left.Parent = node.Parent;
            if (node.Parent == nil)
            {
                root = left;
            }
            else if (node == node.Parent.Right)
            {
                node.Parent.Right = left;
            }
            else
            {
                node.Parent.Left = left;
            }

            left.Right = node;
            node.Parent = left;
        }

        private void Walk(Node node, IList<TValue> values)
        {
            if (node == nil)
            {
                return;
            }

            Walk(node.Left, values);
            values.Add(node.Value);
            Walk(node.Right, values);
        }

        private int HeightOf(Node node)
        {
            if (node == nil)
            {
                return 0;
            }

            return 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));
        }

        private void ValidateNode(
            Node node,
            int blackSoFar,
            ref int blackHeight,
            ref int counted,
            ref TKey previous,
            ref bool hasPrevious)
        {
            if (node == nil)
            {
                if (blackHeight == -1)
                {
                    blackHeight = blackSoFar;
                }
                else if (blackHeight != blackSoFar)
                {
                    throw new InvalidOperationException("Red-black black-height is uneven.");
                }

                return;
            }

            if (node.Red && (node.Left.Red || node.Right.Red))
            {
                throw new InvalidOperationException("Red-black tree has a red node with a red child.");
            }

            if (node.Left != nil && node.Left.Parent != node)
            {
                throw new InvalidOperationException("Red-black parent pointer is broken.");
            }

            if (node.Right != nil && node.Right.Parent != node)
            {
                throw new InvalidOperationException("Red-black parent pointer is broken.");
            }

            int nextBlack = blackSoFar + (node.Red ? 0 : 1);
            ValidateNode(node.Left, nextBlack, ref blackHeight, ref counted, ref previous, ref hasPrevious);
            if (hasPrevious && previous.CompareTo(node.Key) >= 0)
            {
                throw new InvalidOperationException("Red-black tree in-order walk is not sorted.");
            }

            previous = node.Key;
            hasPrevious = true;
            counted++;
            ValidateNode(node.Right, nextBlack, ref blackHeight, ref counted, ref previous, ref hasPrevious);
        }
    }
}
