using System;
using System.Collections.Generic;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// In-memory notices for references the resident is watching.
    /// A report submitted on this computer is watched automatically.
    /// A reference can also be watched later, without creating an account.
    /// </summary>
    public sealed class NotificationCentre
    {
        private static readonly NotificationCentre InstanceValue = new NotificationCentre();
        private readonly HashSet<string> watching = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<ResidentNotification> notifications = new List<ResidentNotification>();
        private int nextId = 1;

        private NotificationCentre()
        {
        }

        public static NotificationCentre Instance
        {
            get { return InstanceValue; }
        }

        public event Action Changed;

        public int UnreadCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < notifications.Count; i++)
                {
                    if (!notifications[i].IsRead)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public IList<ResidentNotification> Notifications
        {
            get { return notifications.AsReadOnly(); }
        }

        public bool IsWatching(string requestId)
        {
            return !string.IsNullOrWhiteSpace(requestId) && watching.Contains(requestId);
        }

        public void FollowSubmittedReport(IssueReport report)
        {
            if (report == null || string.IsNullOrWhiteSpace(report.RequestId))
            {
                return;
            }

            watching.Add(report.RequestId);
            Add(
                report.RequestId,
                "Report received " + RequestReference.Display(report.RequestId),
                "City of Jozi has your " + (report.Category ?? "report") +
                ". This bell will update when it moves to In Progress or Resolved.");
        }

        public bool TryWatch(string input, out string message)
        {
            int number;
            if (!RequestReference.TryParseNumber(input, out number))
            {
                message = "Enter the reference you were given, for example 4 or MSR-2026-0004.";
                return false;
            }

            IssueReport report = Find(number);
            if (report == null)
            {
                message = "No report uses reference #" + number.ToString() + ".";
                return false;
            }

            if (watching.Contains(report.RequestId))
            {
                message = "You are already watching " + RequestReference.Display(report.RequestId) + ".";
                return false;
            }

            watching.Add(report.RequestId);
            Add(
                report.RequestId,
                "Watching " + RequestReference.Display(report.RequestId),
                (report.Category ?? "Request") + " at " + (report.Location ?? "the reported place") +
                " is " + RequestStatuses.Canonical(report.Status) +
                ". Further movement will show on this bell.");
            message = "Watching " + RequestReference.Display(report.RequestId) + ".";
            return true;
        }

        public void PublishStatusChange(IssueReport report, string fromStatus, string toStatus)
        {
            if (report == null || !watching.Contains(report.RequestId))
            {
                return;
            }

            Add(
                report.RequestId,
                RequestReference.Display(report.RequestId) + " is now " + toStatus,
                (report.Category ?? "Your report") + " moved from " + fromStatus + " to " + toStatus + ".");
        }

        public void MarkRead(string notificationId)
        {
            for (int i = 0; i < notifications.Count; i++)
            {
                if (notifications[i].Id == notificationId && !notifications[i].IsRead)
                {
                    notifications[i].IsRead = true;
                    RaiseChanged();
                    return;
                }
            }
        }

        public void MarkAllRead()
        {
            bool changed = false;
            for (int i = 0; i < notifications.Count; i++)
            {
                if (!notifications[i].IsRead)
                {
                    notifications[i].IsRead = true;
                    changed = true;
                }
            }

            if (changed)
            {
                RaiseChanged();
            }
        }

        private void Add(string requestId, string title, string message)
        {
            notifications.Insert(0, new ResidentNotification
            {
                Id = nextId.ToString(),
                RequestId = requestId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            });
            nextId++;
            RaiseChanged();
        }

        private static IssueReport Find(int number)
        {
            IReadOnlyList<IssueReport> issues = IssueRepository.Instance.Issues;
            for (int i = 0; i < issues.Count; i++)
            {
                int parsed;
                if (RequestReference.TryParseNumber(issues[i].RequestId, out parsed) && parsed == number)
                {
                    return issues[i];
                }
            }

            return null;
        }

        private void RaiseChanged()
        {
            Action handler = Changed;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
