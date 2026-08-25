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
    }
}
