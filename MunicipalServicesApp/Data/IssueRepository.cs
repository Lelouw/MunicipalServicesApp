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

        public event Action<IssueReport> ReportAdded;

        public void Add(IssueReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            issues.Add(report);
            Action<IssueReport> handler = ReportAdded;
            if (handler != null)
            {
                handler(report);
            }
        }

        private void SeedSampleReports()
        {
            AddSample("MSR-2026-0001", "Rivonia Road, Sandton", "Street Lighting",
                "Street light out on Rivonia Road near the mall entrance.",
                new DateTime(2026, 8, 20, 9, 30, 0), RequestStatuses.Resolved);
            AddSample("MSR-2026-0002", "Vilakazi Street, Soweto", "Waste Collection",
                "Missed refuse collection for two weeks on a residential block.",
                new DateTime(2026, 8, 22, 14, 5, 0), RequestStatuses.Closed);
            AddSample("MSR-2026-0003", "Taxi rank, Tembisa", "Water",
                "Burst pipe causing flooding near the taxi rank.",
                new DateTime(2026, 8, 24, 11, 40, 0), RequestStatuses.Pending);
            AddSample("MSR-2026-0004", "CR Swart Drive, Kempton Park", "Roads",
                "Large pothole on the main road causing traffic to swerve.",
                new DateTime(2026, 8, 26, 8, 15, 0), RequestStatuses.Pending);
            AddSample("MSR-2026-0005", "Orlando East, Soweto", "Sanitation",
                "Blocked sewer overflowing into the street.",
                new DateTime(2026, 8, 28, 16, 20, 0), RequestStatuses.InProgress);
            AddSample("MSR-2026-0006", "3rd Avenue, Alexandra", "Electricity",
                "Several houses have been without power since yesterday morning.",
                new DateTime(2026, 9, 1, 7, 50, 0), RequestStatuses.Assigned);
            AddSample("MSR-2026-0007", "Taxi rank road, Tembisa", "Roads",
                "The road beside the taxi rank has broken up where the water is flooding it.",
                new DateTime(2026, 9, 2, 10, 5, 0), RequestStatuses.Acknowledged);
            AddSample("MSR-2026-0008", "Taxi rank lights, Tembisa", "Street Lighting",
                "Street lights at the taxi rank are out. The poles can go back once the road is safe.",
                new DateTime(2026, 9, 3, 18, 10, 0), RequestStatuses.Pending);
            AddSample("MSR-2026-0009", "Bath Avenue, Rosebank", "Electricity",
                "A feeder fault has cut power to shops on Bath Avenue.",
                new DateTime(2026, 9, 5, 13, 25, 0), RequestStatuses.InProgress);
            AddSample("MSR-2026-0010", "Bath Avenue lights, Rosebank", "Street Lighting",
                "Street lights on Bath Avenue are off because the feeder is still down.",
                new DateTime(2026, 9, 6, 19, 40, 0), RequestStatuses.Pending);
            AddSample("MSR-2026-0011", "Joubert Park, Johannesburg CBD", "Parks and Recreation",
                "Broken benches and illegal dumping along the park path.",
                new DateTime(2026, 9, 8, 9, 0, 0), RequestStatuses.Acknowledged);
            AddSample("MSR-2026-0012", "Grand Central, Midrand", "Utilities",
                "A damaged stormwater inlet is flooding the pavement after rain.",
                new DateTime(2026, 9, 12, 15, 35, 0), RequestStatuses.Pending);
            AddSample("MSR-2026-0013", "Ontdekkers Road, Roodepoort", "Water",
                "Low water pressure reported for a block of flats.",
                new DateTime(2026, 9, 18, 8, 45, 0), RequestStatuses.Assigned);
            AddSample("MSR-2026-0014", "Grayston Drive, Sandton", "Waste Collection",
                "A public bin on Grayston Drive has not been emptied.",
                new DateTime(2026, 10, 1, 11, 15, 0), RequestStatuses.Pending);
        }

        private void AddSample(string requestId, string location, string category, string description, DateTime reportedAt, string status)
        {
            issues.Add(new IssueReport
            {
                RequestId = requestId,
                Location = location,
                Category = category,
                Description = description,
                AttachmentPath = string.Empty,
                ReportedAt = reportedAt,
                Status = status
            });
        }
    }
}
