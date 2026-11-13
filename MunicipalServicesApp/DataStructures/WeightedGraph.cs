using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Weighted graph. Directed edges record which service request must be finished
    /// before another can start. Undirected edges record the kilometre distance between
    /// depots. Breadth-first and depth-first walks, plus Dijkstra's shortest path,
    /// answer the questions the status screen asks of those relationships.
    /// </summary>
    public sealed class WeightedGraph<T>
    {
        private readonly Dictionary<T, List<GraphEdge<T>>> outgoing = new Dictionary<T, List<GraphEdge<T>>>();
        private readonly Dictionary<T, List<GraphEdge<T>>> incoming = new Dictionary<T, List<GraphEdge<T>>>();
        private readonly List<GraphEdge<T>> undirected = new List<GraphEdge<T>>();
        private int sequence;

        public IList<T> Vertices
        {
            get
            {
                var vertices = new List<T>(outgoing.Count);
                foreach (T vertex in outgoing.Keys)
                {
                    vertices.Add(vertex);
                }

                return vertices;
            }
        }

        public IList<GraphEdge<T>> UndirectedEdges
        {
            get { return undirected; }
        }

        public void AddVertex(T vertex)
        {
            if (!outgoing.ContainsKey(vertex))
            {
                outgoing[vertex] = new List<GraphEdge<T>>();
                incoming[vertex] = new List<GraphEdge<T>>();
            }
        }

        public void AddDirectedEdge(T from, T to, int weight, string label)
        {
            AddVertex(from);
            AddVertex(to);
            var edge = new GraphEdge<T>(from, to, weight, label, sequence++);
            outgoing[from].Add(edge);
            incoming[to].Add(edge);
        }

        public void AddUndirectedEdge(T first, T second, int weight, string label)
        {
            AddVertex(first);
            AddVertex(second);
            int edgeSequence = sequence++;
            outgoing[first].Add(new GraphEdge<T>(first, second, weight, label, edgeSequence));
            outgoing[second].Add(new GraphEdge<T>(second, first, weight, label, edgeSequence));
            undirected.Add(new GraphEdge<T>(first, second, weight, label, edgeSequence));
        }

        public IList<GraphEdge<T>> Outgoing(T vertex)
        {
            List<GraphEdge<T>> edges;
            if (!outgoing.TryGetValue(vertex, out edges))
            {
                return new List<GraphEdge<T>>();
            }

            return edges;
        }

        public IList<GraphEdge<T>> Incoming(T vertex)
        {
            List<GraphEdge<T>> edges;
            if (!incoming.TryGetValue(vertex, out edges))
            {
                return new List<GraphEdge<T>>();
            }

            return edges;
        }

        public IList<T> BreadthFirst(T start)
        {
            return Search(start, useQueue: true, bothWays: false);
        }

        public IList<T> DepthFirst(T start)
        {
            return Search(start, useQueue: false, bothWays: false);
        }

        public IList<T> BreadthFirstBothWays(T start)
        {
            return Search(start, useQueue: true, bothWays: true);
        }

        public bool TryShortestPath(T start, T goal, out IList<T> path, out int distance)
        {
            path = new List<T>();
            distance = -1;
            if (!outgoing.ContainsKey(start) || !outgoing.ContainsKey(goal))
            {
                return false;
            }

            var dist = new Dictionary<T, int>();
            var previous = new Dictionary<T, T>();
            var unsettled = new HashSet<T>();
            foreach (T vertex in outgoing.Keys)
            {
                dist[vertex] = int.MaxValue;
                unsettled.Add(vertex);
            }

            dist[start] = 0;
            while (unsettled.Count > 0)
            {
                T current = default(T);
                int best = int.MaxValue;
                string bestName = null;
                bool found = false;
                foreach (T candidate in unsettled)
                {
                    int candidateDistance = dist[candidate];
                    string candidateName = Convert.ToString(candidate);
                    bool closer = candidateDistance < best;
                    bool sameDistanceEarlierName = candidateDistance == best &&
                        (bestName == null || string.Compare(candidateName, bestName, StringComparison.Ordinal) < 0);
                    if (closer || sameDistanceEarlierName)
                    {
                        best = candidateDistance;
                        bestName = candidateName;
                        current = candidate;
                        found = true;
                    }
                }

                if (!found || best == int.MaxValue)
                {
                    break;
                }

                unsettled.Remove(current);
                if (current.Equals(goal))
                {
                    break;
                }

                IList<GraphEdge<T>> edges = outgoing[current];
                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].Weight < 0)
                    {
                        throw new InvalidOperationException("Shortest path does not accept a negative distance.");
                    }

                    int next = dist[current] + edges[i].Weight;
                    if (next < dist[edges[i].To])
                    {
                        dist[edges[i].To] = next;
                        previous[edges[i].To] = current;
                    }
                }
            }

            if (dist[goal] == int.MaxValue)
            {
                return false;
            }

            var reversed = new List<T>();
            T step = goal;
            reversed.Add(step);
            while (!step.Equals(start))
            {
                step = previous[step];
                reversed.Add(step);
            }

            reversed.Reverse();
            path = reversed;
            distance = dist[goal];
            return true;
        }

        public bool TryFindDirectedCycle(out IList<T> cycle)
        {
            cycle = new List<T>();
            var color = new Dictionary<T, int>();
            foreach (T vertex in outgoing.Keys)
            {
                color[vertex] = 0;
            }

            foreach (T vertex in outgoing.Keys)
            {
                if (color[vertex] == 0 && Visit(vertex, color, new List<T>(), out cycle))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Visit(T vertex, Dictionary<T, int> color, List<T> path, out IList<T> cycle)
        {
            color[vertex] = 1;
            path.Add(vertex);
            IList<GraphEdge<T>> edges = outgoing[vertex];
            for (int i = 0; i < edges.Count; i++)
            {
                T next = edges[i].To;
                if (color[next] == 1)
                {
                    int start = path.IndexOf(next);
                    cycle = path.GetRange(start, path.Count - start);
                    cycle.Add(next);
                    return true;
                }

                if (color[next] == 0 && Visit(next, color, path, out cycle))
                {
                    return true;
                }
            }

            path.RemoveAt(path.Count - 1);
            color[vertex] = 2;
            cycle = new List<T>();
            return false;
        }

        private IList<T> Search(T start, bool useQueue, bool bothWays)
        {
            var order = new List<T>();
            if (!outgoing.ContainsKey(start))
            {
                return order;
            }

            var seen = new HashSet<T>();
            var pending = new List<T>();
            pending.Add(start);
            seen.Add(start);

            while (pending.Count > 0)
            {
                T current = useQueue ? pending[0] : pending[pending.Count - 1];
                pending.RemoveAt(useQueue ? 0 : pending.Count - 1);
                order.Add(current);

                var neighbours = new List<T>();
                IList<GraphEdge<T>> edges = outgoing[current];
                for (int i = 0; i < edges.Count; i++)
                {
                    neighbours.Add(edges[i].To);
                }

                if (bothWays)
                {
                    IList<GraphEdge<T>> inbound = incoming[current];
                    for (int i = 0; i < inbound.Count; i++)
                    {
                        neighbours.Add(inbound[i].From);
                    }
                }

                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (seen.Add(neighbours[i]))
                    {
                        pending.Add(neighbours[i]);
                    }
                }
            }

            return order;
        }
    }
}
