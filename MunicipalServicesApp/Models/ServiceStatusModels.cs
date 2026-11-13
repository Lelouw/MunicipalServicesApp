using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    public enum RequestListOrder
    {
        Date,
        Reference,
        Status,
        Urgency
    }

    public sealed class ServiceRequestView
    {
        public IssueReport Report { get; set; }

        public int Number { get; set; }

        public string Area { get; set; }

        public int Priority { get; set; }

        public string PriorityReason { get; set; }

        public IList<StatusChange> History { get; set; }

        public bool IsOpen { get; set; }
    }

    public sealed class TrackResult
    {
        public bool Found { get; set; }

        public ServiceRequestView Request { get; set; }

        public IList<int> Path { get; set; }

        public string Message { get; set; }
    }

    public sealed class RequestDetail
    {
        public ServiceRequestView Request { get; set; }

        public string DecisionTitle { get; set; }

        public string DecisionDetail { get; set; }

        public IList<string> DecisionPath { get; set; }

        public IList<ServiceRequestView> WaitingOn { get; set; }

        public IList<ServiceRequestView> HoldingUp { get; set; }

        public IList<ServiceRequestView> Related { get; set; }

        public IList<ServiceRequestView> Downstream { get; set; }

        public string DependencyNote { get; set; }

        public bool HasCycle { get; set; }

        public string CycleMessage { get; set; }

        public string BlockMessage { get; set; }

        public string DepotName { get; set; }

        public int TravelKm { get; set; }

        public IList<string> TravelPath { get; set; }
    }

    public sealed class AdvanceResult
    {
        public bool Updated { get; set; }

        public string Message { get; set; }

        public string NewStatus { get; set; }
    }

    public sealed class IndexOverview
    {
        public int Count { get; set; }

        public int AvlHeight { get; set; }

        public int BinarySearchHeight { get; set; }

        public int RedBlackHeight { get; set; }

        public int PlainDateTreeHeight { get; set; }

        public int ReportedFromSeptember { get; set; }

        public int MstTotalKm { get; set; }

        public int MstLinkCount { get; set; }

        public int DepotCount { get; set; }

        public IList<string> DepotBreadthFirst { get; set; }

        public IList<string> DepotDepthFirst { get; set; }
    }

    public sealed class CrewLink
    {
        public string From { get; set; }

        public string To { get; set; }

        public int Kilometres { get; set; }
    }
}
