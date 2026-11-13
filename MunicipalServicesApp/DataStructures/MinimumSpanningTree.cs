using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Kruskal's minimum spanning tree. Depot links are considered cheapest first.
    /// A link is kept only when it joins two depots that are not yet connected.
    /// The result is the shortest set of roads that still reaches every depot.
    /// </summary>
    public static class MinimumSpanningTree
    {
        public static IList<GraphEdge<T>> Kruskal<T>(WeightedGraph<T> graph)
        {
            var chosen = new List<GraphEdge<T>>();
            if (graph == null)
            {
                return chosen;
            }

            var edges = new List<GraphEdge<T>>(graph.UndirectedEdges);
            edges.Sort(delegate (GraphEdge<T> left, GraphEdge<T> right)
            {
                int byWeight = left.Weight.CompareTo(right.Weight);
                if (byWeight != 0)
                {
                    return byWeight;
                }

                return left.Sequence.CompareTo(right.Sequence);
            });

            var sets = new UnionFind<T>();
            IList<T> vertices = graph.Vertices;
            for (int i = 0; i < vertices.Count; i++)
            {
                sets.Add(vertices[i]);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                if (sets.Union(edges[i].From, edges[i].To))
                {
                    chosen.Add(edges[i]);
                }
            }

            return chosen;
        }

        public static int TotalWeight<T>(IList<GraphEdge<T>> edges)
        {
            int total = 0;
            if (edges == null)
            {
                return total;
            }

            for (int i = 0; i < edges.Count; i++)
            {
                total += edges[i].Weight;
            }

            return total;
        }
    }
}
