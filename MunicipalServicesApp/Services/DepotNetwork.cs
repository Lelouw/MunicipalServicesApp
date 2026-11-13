using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Undirected weighted graph of City of Jozi depots. The weight is kilometres.
    /// Kruskal keeps the cheapest set of links that still connects every depot.
    /// Dijkstra, on the full road graph, finds the shortest trip for one request.
    /// Those two answers are not the same road.
    /// </summary>
    public static class DepotNetwork
    {
        public const string Central = "Johannesburg CBD";

        public static WeightedGraph<string> Create()
        {
            var graph = new WeightedGraph<string>();
            Link(graph, Central, "Rosebank", 6);
            Link(graph, "Rosebank", "Sandton", 8);
            Link(graph, "Kempton Park", "Tembisa", 8);
            Link(graph, "Rosebank", "Alexandra", 10);
            Link(graph, "Midrand", "Kempton Park", 12);
            Link(graph, "Sandton", "Alexandra", 12);
            Link(graph, "Soweto", "Roodepoort", 14);
            Link(graph, "Alexandra", "Midrand", 14);
            Link(graph, "Midrand", "Tembisa", 15);
            Link(graph, "Sandton", "Midrand", 16);
            Link(graph, Central, "Alexandra", 16);
            Link(graph, Central, "Soweto", 18);
            Link(graph, Central, "Sandton", 18);
            Link(graph, Central, "Roodepoort", 22);
            Link(graph, Central, "Midrand", 24);
            Link(graph, "Sandton", "Roodepoort", 28);
            Link(graph, Central, "Kempton Park", 30);
            Link(graph, Central, "Tembisa", 36);
            return graph;
        }

        private static void Link(WeightedGraph<string> graph, string from, string to, int kilometres)
        {
            graph.AddUndirectedEdge(from, to, kilometres, kilometres.ToString() + " km");
        }
    }
}
