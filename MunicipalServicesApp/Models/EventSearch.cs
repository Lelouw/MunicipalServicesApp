using System;

namespace MunicipalServicesApp.Models
{
    public sealed class EventSearch
    {
        public string Keyword { get; set; }

        public string Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime SearchedAt { get; set; }
    }
}
