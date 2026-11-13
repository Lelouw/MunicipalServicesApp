namespace MunicipalServicesApp.DataStructures
{
    public sealed class GraphEdge<T>
    {
        public GraphEdge(T from, T to, int weight, string label, int sequence)
        {
            From = from;
            To = to;
            Weight = weight;
            Label = label ?? string.Empty;
            Sequence = sequence;
        }

        public T From { get; private set; }

        public T To { get; private set; }

        public int Weight { get; private set; }

        public string Label { get; private set; }

        public int Sequence { get; private set; }
    }
}
