namespace MunicipalServicesApp.Models
{
    public sealed class LocationSuggestion
    {
        public string DisplayName { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public override string ToString()
        {
            return DisplayName ?? string.Empty;
        }
    }
}
