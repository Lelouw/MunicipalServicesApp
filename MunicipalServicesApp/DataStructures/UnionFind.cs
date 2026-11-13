using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Disjoint-set structure used by Kruskal's algorithm. Two depots are already
    /// connected when they share a root. Union by rank and path compression keep
    /// that test cheap while the spanning tree is built.
    /// </summary>
    public sealed class UnionFind<T>
    {
        private readonly Dictionary<T, T> parent = new Dictionary<T, T>();
        private readonly Dictionary<T, int> rank = new Dictionary<T, int>();

        public void Add(T item)
        {
            if (parent.ContainsKey(item))
            {
                return;
            }

            parent[item] = item;
            rank[item] = 0;
        }

        public T Find(T item)
        {
            if (!parent[item].Equals(item))
            {
                parent[item] = Find(parent[item]);
            }

            return parent[item];
        }

        public bool Union(T first, T second)
        {
            T rootA = Find(first);
            T rootB = Find(second);
            if (rootA.Equals(rootB))
            {
                return false;
            }

            int rankA = rank[rootA];
            int rankB = rank[rootB];
            if (rankA < rankB)
            {
                parent[rootA] = rootB;
            }
            else if (rankA > rankB)
            {
                parent[rootB] = rootA;
            }
            else
            {
                parent[rootB] = rootA;
                rank[rootA] = rankA + 1;
            }

            return true;
        }
    }
}
