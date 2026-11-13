using System;

namespace MunicipalServicesApp.Models
{
    public sealed class StatusChange
    {
        public DateTime At { get; set; }

        public string Status { get; set; }

        public string Note { get; set; }
    }
}
