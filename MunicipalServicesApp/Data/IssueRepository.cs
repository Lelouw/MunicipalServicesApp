using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public sealed class IssueRepository
    {
        private static readonly IssueRepository InstanceValue = new IssueRepository();
        private readonly List<IssueReport> issues = new List<IssueReport>();

        private IssueRepository()
        {
            SeedSampleReports();
        }

        public static IssueRepository Instance
        {
            get { return InstanceValue; }
        }

        public IReadOnlyList<IssueReport> Issues
        {
            get { return issues; }
        }

        public int Count
        {
            get { return issues.Count; }
        }

        public void Add(IssueReport report)
        {
            issues.Add(report);
        }

        private void SeedSampleReports()
        {
            issues.Add(new IssueReport
            {
                RequestId = "MSR-2026-0001",
                Location = "Sandton City, Johannesburg",
                Category = "Street Lighting",
                Description = "Street light out on Rivonia Road near the mall entrance.",
                AttachmentPath = string.Empty,
                ReportedAt = new DateTime(2026, 8, 20, 9, 30, 0),
                Status = "Resolved"
            });

            issues.Add(new IssueReport
            {
                RequestId = "MSR-2026-0002",
                Location = "Soweto, Johannesburg",
                Category = "Waste Collection",
                Description = "Missed refuse collection for two weeks on a residential block.",
                AttachmentPath = string.Empty,
                ReportedAt = new DateTime(2026, 8, 22, 14, 5, 0),
                Status = "Closed"
            });

            issues.Add(new IssueReport
            {
                RequestId = "MSR-2026-0003",
                Location = "Tembisa, Ekurhuleni",
                Category = "Water",
                Description = "Burst pipe causing flooding near the taxi rank.",
                AttachmentPath = string.Empty,
                ReportedAt = new DateTime(2026, 8, 24, 11, 40, 0),
                Status = "Pending"
            });

            issues.Add(new IssueReport
            {
                RequestId = "MSR-2026-0004",
                Location = "Kempton Park",
                Category = "Roads",
                Description = "Large pothole on the main road causing traffic to swerve.",
                AttachmentPath = string.Empty,
                ReportedAt = new DateTime(2026, 8, 26, 8, 15, 0),
                Status = "Pending"
            });
        }
    }
}
