using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// The status path residents were shown: Pending, then In Progress, then Resolved.
    /// Staff set it from the municipal desk. The bell updates any reference being watched.
    /// </summary>
    public static class IssueProgress
    {
        public static readonly string[] Choices =
        {
            RequestStatuses.Pending,
            RequestStatuses.InProgress,
            RequestStatuses.Resolved
        };

        public static bool TryChange(IssueReport report, string newStatus, out string message)
        {
            if (report == null)
            {
                message = "Select a report first.";
                return false;
            }

            if (!IsChoice(newStatus))
            {
                message = "Choose Pending, In Progress, or Resolved.";
                return false;
            }

            string current = RequestStatuses.Canonical(report.Status);
            if (string.Equals(current, newStatus, System.StringComparison.OrdinalIgnoreCase))
            {
                message = RequestReference.Display(report.RequestId) + " is already " + newStatus + ".";
                return false;
            }

            if (ServiceRequestCatalogue.IsReady)
            {
                string error;
                if (!ServiceRequestCatalogue.Instance.ApplyStatus(report.RequestId, newStatus, out error))
                {
                    message = error;
                    return false;
                }
            }
            else
            {
                report.Status = newStatus;
                NotificationCentre.Instance.PublishStatusChange(report, current, newStatus);
            }

            message = RequestReference.Display(report.RequestId) + " is now " + newStatus +
                      ". The bell updates anyone watching this reference.";
            return true;
        }

        private static bool IsChoice(string status)
        {
            for (int i = 0; i < Choices.Length; i++)
            {
                if (string.Equals(Choices[i], status, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
