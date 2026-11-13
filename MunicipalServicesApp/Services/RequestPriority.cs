using System;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public static class RequestPriority
    {
        public const int AgeCapDays = 60;

        public static int Score(string category, string status, DateTime reportedAt, DateTime today)
        {
            if (!RequestStatuses.IsOpen(status))
            {
                return 0;
            }

            return CategoryWeight(category) + StatusWeight(status) + CountedAge(reportedAt, today);
        }

        public static string Explain(string category, string status, DateTime reportedAt, DateTime today)
        {
            if (!RequestStatuses.IsOpen(status))
            {
                return "Completed requests leave the urgency queue.";
            }

            int rawAge = RawAge(reportedAt, today);
            string ageText = rawAge > AgeCapDays
                ? "counted as " + AgeCapDays.ToString() + " days open"
                : rawAge.ToString() + " days open";
            return (category ?? "Request") + " · " + RequestStatuses.Canonical(status) + " · " + ageText;
        }

        public static bool IsUrgentCategory(string category)
        {
            return category == "Water" ||
                   category == "Electricity" ||
                   category == "Sanitation" ||
                   category == "Roads";
        }

        public static int RawAge(DateTime reportedAt, DateTime today)
        {
            int age = (int)(today.Date - reportedAt.Date).TotalDays;
            return age < 0 ? 0 : age;
        }

        private static int CountedAge(DateTime reportedAt, DateTime today)
        {
            int age = RawAge(reportedAt, today);
            return age > AgeCapDays ? AgeCapDays : age;
        }

        private static int CategoryWeight(string category)
        {
            switch (category)
            {
                case "Water":
                    return 50;
                case "Electricity":
                    return 45;
                case "Sanitation":
                    return 40;
                case "Roads":
                    return 30;
                case "Street Lighting":
                    return 22;
                case "Utilities":
                    return 18;
                case "Waste Collection":
                    return 14;
                case "Parks and Recreation":
                    return 8;
                case "Other":
                    return 5;
                default:
                    return 10;
            }
        }

        private static int StatusWeight(string status)
        {
            switch (RequestStatuses.Canonical(status))
            {
                case RequestStatuses.Pending:
                    return 24;
                case RequestStatuses.Acknowledged:
                    return 18;
                case RequestStatuses.Assigned:
                    return 12;
                case RequestStatuses.InProgress:
                    return 6;
                default:
                    return 0;
            }
        }
    }
}
