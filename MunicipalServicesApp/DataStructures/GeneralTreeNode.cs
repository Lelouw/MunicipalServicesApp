using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A basic tree node. A node may have any number of children, which is what a
    /// service catalogue needs: one city, several departments, and several categories under each.
    /// </summary>
    public sealed class GeneralTreeNode<T>
    {
        public GeneralTreeNode(T value)
        {
            Value = value;
            Children = new List<GeneralTreeNode<T>>();
        }

        public T Value { get; private set; }

        public GeneralTreeNode<T> Parent { get; private set; }

        public List<GeneralTreeNode<T>> Children { get; private set; }

        public GeneralTreeNode<T> AddChild(T value)
        {
            var child = new GeneralTreeNode<T>(value);
            child.Parent = this;
            Children.Add(child);
            return child;
        }

        public void Preorder(IList<GeneralTreeNode<T>> order)
        {
            order.Add(this);
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Preorder(order);
            }
        }
    }
}
