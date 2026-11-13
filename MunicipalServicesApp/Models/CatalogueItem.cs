namespace MunicipalServicesApp.Models
{
    public sealed class CatalogueItem
    {
        public string Name { get; set; }

        public string RequestId { get; set; }

        public bool IsRequest
        {
            get { return !string.IsNullOrEmpty(RequestId); }
        }
    }
}
