using System;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A binary tree node: at most a left child and a right child.
    /// Used for the yes/no decision procedure that chooses the next step on a request.
    /// Left means yes and right means no.
    /// </summary>
    public sealed class BinaryTreeNode<T>
    {
        public BinaryTreeNode(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        public BinaryTreeNode<T> Left { get; set; }

        public BinaryTreeNode<T> Right { get; set; }

        public int Height
        {
            get
            {
                int left = Left == null ? 0 : Left.Height;
                int right = Right == null ? 0 : Right.Height;
                return 1 + Math.Max(left, right);
            }
        }

        public void Inorder(System.Collections.Generic.IList<T> order)
        {
            if (Left != null)
            {
                Left.Inorder(order);
            }

            order.Add(Value);
            if (Right != null)
            {
                Right.Inorder(order);
            }
        }
    }
}
