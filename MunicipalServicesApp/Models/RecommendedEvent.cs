namespace MunicipalServicesApp.Models
{
    public sealed class RecommendedEvent
    {
        public LocalEvent Event { get; set; }

        public string Reason { get; set; }

        public int Score { get; set; }
    }
}
