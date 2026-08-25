using System;

namespace MunicipalServicesApp.Models
{
    public class IssueReport
    {
        public string RequestId { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string AttachmentPath { get; set; }

        public DateTime ReportedAt { get; set; }

        public string Status { get; set; }
    }
}
