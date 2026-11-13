using System;

namespace MunicipalServicesApp.Models
{
    public static class RequestStatuses
    {
        public const string Pending = "Pending";
        public const string Acknowledged = "Acknowledged";
        public const string Assigned = "Assigned";
        public const string InProgress = "In Progress";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";

        public static readonly string[] Workflow =
        {
            Pending,
            Acknowledged,
            Assigned,
            InProgress,
            Resolved,
            Closed
        };

        public static int Rank(string status)
        {
            if (string.IsNullOrWhiteSpace(status) ||
                string.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            for (int i = 0; i < Workflow.Length; i++)
            {
                if (string.Equals(Workflow[i], status, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return 0;
        }

        public static string Canonical(string status)
        {
            return Workflow[Rank(status)];
        }

        public static bool IsOpen(string status)
        {
            return Rank(status) < Rank(Resolved);
        }

        public static bool IsFinished(string status)
        {
            return !IsOpen(status);
        }

        public static string Next(string status)
        {
            int rank = Rank(status);
            if (rank >= Workflow.Length - 1)
            {
                return null;
            }

            return Workflow[rank + 1];
        }
    }
}
