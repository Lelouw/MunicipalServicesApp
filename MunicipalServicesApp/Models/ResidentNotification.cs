using System;

namespace MunicipalServicesApp.Models
{
    public sealed class ResidentNotification
    {
        public string Id { get; set; }

        public string RequestId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }
    }
}
