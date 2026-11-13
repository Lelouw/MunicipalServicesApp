using System;
using System.Collections.Generic;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Keeps every service-request index used by the status screen.
    /// The list in IssueRepository remains the record of the report. These structures
    /// organise that record so a reference, a date, a status, a category, a dependency,
    /// or a depot can be answered without scanning the list each time.
    /// </summary>
    public sealed class ServiceRequestCatalogue
    {
        private static ServiceRequestCatalogue instanceValue;

        private static readonly Dictionary<string, string[]> SampleDependencies =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "MSR-2026-0007", new[] { "MSR-2026-0003" } },
                { "MSR-2026-0008", new[] { "MSR-2026-0007" } },
                { "MSR-2026-0010", new[] { "MSR-2026-0009" } }
            };

        private static readonly Dictionary<string, string> SampleNotes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "MSR-2026-0007", "The road at the Tembisa taxi rank cannot be repaired while the burst pipe is still flooding it." },
                { "MSR-2026-0008", "Street lights at the taxi rank go in after the road surface is safe to work on." },
                { "MSR-2026-0010", "The lights on Bath Avenue stay off until the electricity fault is repaired." }
            };

        private static readonly string[] KnownAreas =
        {
            "Kempton Park",
            "Johannesburg CBD",
            "Roodepoort",
            "Sandton",
            "Soweto",
            "Tembisa",
            "Alexandra",
            "Rosebank",
            "Midrand"
        };

        private readonly Dictionary<string, IssueReport> byId = new Dictionary<string, IssueReport>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ServiceRequestProfile> profiles = new Dictionary<string, ServiceRequestProfile>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, int> numbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, string> idByNumber = new Dictionary<int, string>();
        private readonly AvlTree<RequestDateKey, string> dateIndex = new AvlTree<RequestDateKey, string>();
        private readonly RedBlackTree<RequestStatusKey, string> statusIndex = new RedBlackTree<RequestStatusKey, string>();
        private readonly BinarySearchTree<int, string> referenceIndex = new BinarySearchTree<int, string>();
        private readonly BinaryHeap<string> urgency = new BinaryHeap<string>();
        private readonly WeightedGraph<string> dependencies = new WeightedGraph<string>();
        private readonly WeightedGraph<string> depots;
        private readonly IList<GraphEdge<string>> spanningTree;
        private readonly BinaryTreeNode<DecisionStep> decisions;
        private readonly ServiceCatalogue catalogue;
        private readonly HashSet<string> dependencyEdges = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private ServiceRequestCatalogue()
        {
            catalogue = ServiceCatalogue.Create();
            decisions = NextStepDecisionTree.Create();
            depots = DepotNetwork.Create();
            spanningTree = MinimumSpanningTree.Kruskal(depots);

            IReadOnlyList<IssueReport> existing = IssueRepository.Instance.Issues;
            var referencePairs = new List<KeyValuePair<int, string>>();
            for (int i = 0; i < existing.Count; i++)
            {
                Remember(existing[i], insertReference: false);
                referencePairs.Add(new KeyValuePair<int, string>(NumberOf(existing[i].RequestId), existing[i].RequestId));
            }

            referenceIndex.InsertInBalancedOrder(referencePairs);
            IssueRepository.Instance.ReportAdded += OnReportAdded;
            ValidateIndexes();
        }

        public static bool IsReady
        {
            get { return instanceValue != null; }
        }

        public static ServiceRequestCatalogue Instance
        {
            get
            {
                if (instanceValue == null)
                {
                    instanceValue = new ServiceRequestCatalogue();
                }

                return instanceValue;
            }
        }

        public ServiceCatalogue Catalogue
        {
            get { return catalogue; }
        }

        public IList<CrewLink> CrewLinks()
        {
            var links = new List<CrewLink>();
            for (int i = 0; i < spanningTree.Count; i++)
            {
                links.Add(new CrewLink
                {
                    From = spanningTree[i].From,
                    To = spanningTree[i].To,
                    Kilometres = spanningTree[i].Weight
                });
            }

            return links;
        }

        public IndexOverview Overview()
        {
            var plain = new BinarySearchTree<RequestDateKey, string>();
            IList<string> byDate = dateIndex.InOrder();
            for (int i = 0; i < byDate.Count; i++)
            {
                IssueReport report = byId[byDate[i]];
                plain.Insert(new RequestDateKey(report.ReportedAt, NumberOf(report.RequestId)), report.RequestId);
            }

            var fromSeptember = dateIndex.Range(
                new RequestDateKey(new DateTime(2026, 9, 1), 0),
                new RequestDateKey(DateTime.MaxValue, int.MaxValue));

            return new IndexOverview
            {
                Count = byId.Count,
                AvlHeight = dateIndex.Height,
                BinarySearchHeight = referenceIndex.Height,
                RedBlackHeight = statusIndex.Height,
                PlainDateTreeHeight = plain.Height,
                ReportedFromSeptember = fromSeptember.Count,
                MstTotalKm = MinimumSpanningTree.TotalWeight(spanningTree),
                MstLinkCount = spanningTree.Count,
                DepotCount = depots.Vertices.Count,
                DepotBreadthFirst = depots.BreadthFirst(DepotNetwork.Central),
                DepotDepthFirst = depots.DepthFirst(DepotNetwork.Central)
            };
        }

        public IList<ServiceRequestView> List(RequestListOrder order, GeneralTreeNode<CatalogueItem> categoryNode, string text)
        {
            IList<string> ordered = IdsInOrder(order);
            HashSet<string> allowed = null;
            if (categoryNode != null && categoryNode != catalogue.Root)
            {
                allowed = new HashSet<string>(catalogue.RequestIds(categoryNode), StringComparer.OrdinalIgnoreCase);
            }

            string needle = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            var views = new List<ServiceRequestView>();
            for (int i = 0; i < ordered.Count; i++)
            {
                if (allowed != null && !allowed.Contains(ordered[i]))
                {
                    continue;
                }

                ServiceRequestView view = ToView(byId[ordered[i]]);
                if (needle != null && !Matches(view, needle))
                {
                    continue;
                }

                views.Add(view);
            }

            return views;
        }

        public IList<ServiceRequestView> UrgentQueue()
        {
            IList<string> ids = urgency.ToPriorityOrder();
            var views = new List<ServiceRequestView>(ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                views.Add(ToView(byId[ids[i]]));
            }

            return views;
        }

        public TrackResult Track(string input)
        {
            var path = new List<int>();
            int number;
            if (!RequestReference.TryParseNumber(input, out number))
            {
                return new TrackResult
                {
                    Found = false,
                    Path = path,
                    Message = "Enter a reference such as 3 or MSR-2026-0003."
                };
            }

            string requestId;
            bool found = referenceIndex.TryGetValue(number, out requestId, path);
            string pathText = JoinPath(path);
            if (!found)
            {
                return new TrackResult
                {
                    Found = false,
                    Path = path,
                    Message = "No request uses reference #" + number.ToString() + ". Binary search tree path: " + pathText + "."
                };
            }

            return new TrackResult
            {
                Found = true,
                Request = ToView(byId[requestId]),
                Path = path,
                Message = "Found " + RequestReference.Display(requestId) + " after " + path.Count.ToString() +
                          " comparison" + (path.Count == 1 ? string.Empty : "s") +
                          ". Binary search tree path: " + pathText + "."
            };
        }

        public RequestDetail Detail(string requestId)
        {
            IssueReport report;
            if (string.IsNullOrWhiteSpace(requestId) || !byId.TryGetValue(requestId, out report))
            {
                return null;
            }

            ServiceRequestView view = ToView(report);
            DecisionOutcome outcome = NextStepDecisionTree.Evaluate(decisions, new DecisionFacts
            {
                Status = RequestStatuses.Canonical(report.Status),
                Category = report.Category,
                AgeDays = RequestPriority.RawAge(report.ReportedAt, DateTime.Today),
                IsOpen = view.IsOpen,
                Urgent = RequestPriority.IsUrgentCategory(report.Category)
            });

            var waiting = new List<ServiceRequestView>();
            IList<GraphEdge<string>> inbound = dependencies.Incoming(requestId);
            var blockers = new List<ServiceRequestView>();
            for (int i = 0; i < inbound.Count; i++)
            {
                IssueReport predecessor;
                if (!byId.TryGetValue(inbound[i].From, out predecessor))
                {
                    continue;
                }

                ServiceRequestView predecessorView = ToView(predecessor);
                waiting.Add(predecessorView);
                if (!RequestStatuses.IsFinished(predecessor.Status))
                {
                    blockers.Add(predecessorView);
                }
            }

            var holding = new List<ServiceRequestView>();
            IList<GraphEdge<string>> outbound = dependencies.Outgoing(requestId);
            for (int i = 0; i < outbound.Count; i++)
            {
                IssueReport successor;
                if (byId.TryGetValue(outbound[i].To, out successor))
                {
                    holding.Add(ToView(successor));
                }
            }

            IList<string> relatedIds = dependencies.BreadthFirstBothWays(requestId);
            IList<string> downstreamIds = Downstream(requestId);
            IList<string> cycle;
            bool hasCycle = dependencies.TryFindDirectedCycle(out cycle);

            string area = profiles[requestId].Area;
            IList<string> travel;
            int kilometres;
            if (!depots.TryShortestPath(DepotNetwork.Central, area, out travel, out kilometres))
            {
                travel = new List<string> { area };
                kilometres = 0;
            }

            return new RequestDetail
            {
                Request = view,
                DecisionTitle = outcome.Title,
                DecisionDetail = outcome.Detail,
                DecisionPath = outcome.Path,
                WaitingOn = waiting,
                HoldingUp = holding,
                Related = ViewsFor(relatedIds),
                Downstream = ViewsFor(downstreamIds),
                DependencyNote = profiles[requestId].DependencyNote,
                HasCycle = hasCycle,
                CycleMessage = hasCycle
                    ? "Circular dependency: " + JoinRequests(cycle) + "."
                    : "Depth-first check: no circular dependencies.",
                BlockMessage = DescribeBlock(view, blockers),
                DepotName = area,
                TravelKm = kilometres,
                TravelPath = travel
            };
        }

        public AdvanceResult TryAdvance(string requestId)
        {
            IssueReport report;
            if (string.IsNullOrWhiteSpace(requestId) || !byId.TryGetValue(requestId, out report))
            {
                return new AdvanceResult
                {
                    Updated = false,
                    Message = "Select a request first."
                };
            }

            string current = RequestStatuses.Canonical(report.Status);
            string next = RequestStatuses.Next(current);
            if (next == null)
            {
                return new AdvanceResult
                {
                    Updated = false,
                    Message = RequestReference.Display(requestId) + " is already " + current + "."
                };
            }

            if (RequestStatuses.Rank(next) >= RequestStatuses.Rank(RequestStatuses.InProgress))
            {
                IList<ServiceRequestView> blockers = OpenPredecessors(requestId);
                if (blockers.Count > 0)
                {
                    return new AdvanceResult
                    {
                        Updated = false,
                        Message = RequestReference.Display(requestId) + " cannot move to " + next +
                                  " yet. It is waiting on " + DescribeViews(blockers) + "."
                    };
                }
            }

            CommitStatus(report, current, next);
            return new AdvanceResult
            {
                Updated = true,
                NewStatus = next,
                Message = RequestReference.Display(requestId) + " is now " + next + "."
            };
        }

        public bool ApplyStatus(string requestId, string newStatus, out string error)
        {
            IssueReport report;
            if (string.IsNullOrWhiteSpace(requestId) || !byId.TryGetValue(requestId, out report))
            {
                error = "That report is not on the status index yet.";
                return false;
            }

            string current = RequestStatuses.Canonical(report.Status);
            string target = RequestStatuses.Canonical(newStatus);
            if (string.Equals(current, target, StringComparison.OrdinalIgnoreCase))
            {
                error = RequestReference.Display(requestId) + " is already " + target + ".";
                return false;
            }

            CommitStatus(report, current, target);
            error = null;
            return true;
        }

        private void CommitStatus(IssueReport report, string current, string next)
        {
            int number = NumberOf(report.RequestId);
            statusIndex.Remove(new RequestStatusKey(RequestStatuses.Rank(current), number));
            report.Status = next;
            profiles[report.RequestId].History.Add(new StatusChange
            {
                At = DateTime.Now,
                Status = next,
                Note = HistoryNote(next)
            });
            statusIndex.Insert(new RequestStatusKey(RequestStatuses.Rank(next), number), report.RequestId);
            RefreshUrgency(report);
            statusIndex.Validate();
            urgency.Validate();
            NotificationCentre.Instance.PublishStatusChange(report, current, next);
        }

        public void ValidateIndexes()
        {
            dateIndex.Validate();
            statusIndex.Validate();
            referenceIndex.Validate();
            urgency.Validate();

            if (spanningTree.Count != depots.Vertices.Count - 1)
            {
                throw new InvalidOperationException("The depot spanning tree does not connect every depot exactly once.");
            }

            IList<string> cycle;
            if (dependencies.TryFindDirectedCycle(out cycle))
            {
                throw new InvalidOperationException("The sample dependency graph contains a cycle.");
            }
        }

        private void OnReportAdded(IssueReport report)
        {
            if (report == null || byId.ContainsKey(report.RequestId))
            {
                return;
            }

            Remember(report, insertReference: true);
        }

        private void Remember(IssueReport report, bool insertReference)
        {
            byId[report.RequestId] = report;
            if (!profiles.ContainsKey(report.RequestId))
            {
                profiles[report.RequestId] = CreateProfile(report);
            }

            int number = NumberOf(report.RequestId);
            ServiceRequestProfile profile = profiles[report.RequestId];
            catalogue.AttachRequest(report.Category, report.RequestId);
            dateIndex.Insert(new RequestDateKey(report.ReportedAt, number), report.RequestId);
            statusIndex.Insert(new RequestStatusKey(RequestStatuses.Rank(report.Status), number), report.RequestId);
            if (insertReference)
            {
                referenceIndex.Insert(number, report.RequestId);
            }

            RefreshUrgency(report);
            dependencies.AddVertex(report.RequestId);
            if (profile.DependsOn == null)
            {
                return;
            }

            for (int i = 0; i < profile.DependsOn.Count; i++)
            {
                string edgeKey = profile.DependsOn[i] + "->" + report.RequestId;
                if (!dependencyEdges.Add(edgeKey))
                {
                    continue;
                }

                dependencies.AddDirectedEdge(profile.DependsOn[i], report.RequestId, 1, profile.DependencyNote);
            }
        }

        private void RefreshUrgency(IssueReport report)
        {
            if (!RequestStatuses.IsOpen(report.Status))
            {
                urgency.Remove(report.RequestId);
                return;
            }

            urgency.Upsert(
                report.RequestId,
                RequestPriority.Score(report.Category, report.Status, report.ReportedAt, DateTime.Today),
                NumberOf(report.RequestId),
                report.RequestId);
        }

        private IList<string> IdsInOrder(RequestListOrder order)
        {
            if (order == RequestListOrder.Reference)
            {
                return referenceIndex.InOrder();
            }

            if (order == RequestListOrder.Status)
            {
                return statusIndex.InOrder();
            }

            if (order == RequestListOrder.Urgency)
            {
                var ids = new List<string>(urgency.ToPriorityOrder());
                var seen = new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase);
                IList<string> byDate = dateIndex.InOrder();
                for (int i = 0; i < byDate.Count; i++)
                {
                    if (seen.Add(byDate[i]))
                    {
                        ids.Add(byDate[i]);
                    }
                }

                return ids;
            }

            return dateIndex.InOrder();
        }

        private ServiceRequestProfile CreateProfile(IssueReport report)
        {
            string[] dependsOn;
            if (!SampleDependencies.TryGetValue(report.RequestId, out dependsOn))
            {
                dependsOn = new string[0];
            }

            string note;
            if (!SampleNotes.TryGetValue(report.RequestId, out note))
            {
                note = string.Empty;
            }

            return new ServiceRequestProfile
            {
                RequestId = report.RequestId,
                Area = AreaFromLocation(report.Location),
                DependsOn = new List<string>(dependsOn),
                DependencyNote = note,
                History = BuildHistory(report)
            };
        }

        private static List<StatusChange> BuildHistory(IssueReport report)
        {
            var history = new List<StatusChange>();
            DateTime at = report.ReportedAt;
            string current = RequestStatuses.Canonical(report.Status);
            for (int i = 0; i < RequestStatuses.Workflow.Length; i++)
            {
                string step = RequestStatuses.Workflow[i];
                history.Add(new StatusChange
                {
                    At = at,
                    Status = step,
                    Note = HistoryNote(step)
                });

                if (string.Equals(step, current, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                at = at.AddDays(1).AddHours(2);
            }

            return history;
        }

        private static string HistoryNote(string status)
        {
            switch (status)
            {
                case RequestStatuses.Pending:
                    return "Report received from the resident.";
                case RequestStatuses.Acknowledged:
                    return "The service desk accepted the report.";
                case RequestStatuses.Assigned:
                    return "A depot was assigned.";
                case RequestStatuses.InProgress:
                    return "A crew started the work.";
                case RequestStatuses.Resolved:
                    return "The work was completed.";
                case RequestStatuses.Closed:
                    return "The request was closed after the resident was told.";
                default:
                    return "Status updated.";
            }
        }

        private static string AreaFromLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return DepotNetwork.Central;
            }

            if (location.IndexOf("Joubert", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DepotNetwork.Central;
            }

            for (int i = 0; i < KnownAreas.Length; i++)
            {
                if (location.IndexOf(KnownAreas[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return KnownAreas[i];
                }
            }

            return DepotNetwork.Central;
        }

        private int NumberOf(string requestId)
        {
            int existing;
            if (numbers.TryGetValue(requestId, out existing))
            {
                return existing;
            }

            int parsed;
            if (!RequestReference.TryParseNumber(requestId, out parsed))
            {
                parsed = 0;
            }

            if (parsed <= 0 || idByNumber.ContainsKey(parsed))
            {
                parsed = 1;
                while (idByNumber.ContainsKey(parsed))
                {
                    parsed++;
                }
            }

            numbers[requestId] = parsed;
            idByNumber[parsed] = requestId;
            return parsed;
        }

        private ServiceRequestView ToView(IssueReport report)
        {
            ServiceRequestProfile profile = profiles[report.RequestId];
            return new ServiceRequestView
            {
                Report = report,
                Number = NumberOf(report.RequestId),
                Area = profile.Area,
                Priority = RequestPriority.Score(report.Category, report.Status, report.ReportedAt, DateTime.Today),
                PriorityReason = RequestPriority.Explain(report.Category, report.Status, report.ReportedAt, DateTime.Today),
                History = profile.History,
                IsOpen = RequestStatuses.IsOpen(report.Status)
            };
        }

        private IList<ServiceRequestView> ViewsFor(IList<string> ids)
        {
            var views = new List<ServiceRequestView>();
            if (ids == null)
            {
                return views;
            }

            for (int i = 0; i < ids.Count; i++)
            {
                IssueReport report;
                if (byId.TryGetValue(ids[i], out report))
                {
                    views.Add(ToView(report));
                }
            }

            return views;
        }

        private IList<string> Downstream(string requestId)
        {
            var order = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var stack = new Stack<string>();
            stack.Push(requestId);
            seen.Add(requestId);
            while (stack.Count > 0)
            {
                string current = stack.Pop();
                if (!string.Equals(current, requestId, StringComparison.OrdinalIgnoreCase))
                {
                    order.Add(current);
                }

                IList<GraphEdge<string>> edges = dependencies.Outgoing(current);
                for (int i = edges.Count - 1; i >= 0; i--)
                {
                    if (seen.Add(edges[i].To))
                    {
                        stack.Push(edges[i].To);
                    }
                }
            }

            return order;
        }

        private IList<ServiceRequestView> OpenPredecessors(string requestId)
        {
            var blockers = new List<ServiceRequestView>();
            IList<GraphEdge<string>> inbound = dependencies.Incoming(requestId);
            for (int i = 0; i < inbound.Count; i++)
            {
                IssueReport predecessor;
                if (byId.TryGetValue(inbound[i].From, out predecessor) &&
                    !RequestStatuses.IsFinished(predecessor.Status))
                {
                    blockers.Add(ToView(predecessor));
                }
            }

            return blockers;
        }

        private static string DescribeBlock(ServiceRequestView view, IList<ServiceRequestView> blockers)
        {
            if (blockers == null || blockers.Count == 0)
            {
                return "This request is not waiting on another job.";
            }

            string names = DescribeViews(blockers);
            if (RequestStatuses.Rank(view.Report.Status) < RequestStatuses.Rank(RequestStatuses.InProgress))
            {
                return "It can be acknowledged and assigned, but work cannot start until " + names + " is finished.";
            }

            return "Work cannot move on until " + names + " is finished.";
        }

        private static string DescribeViews(IList<ServiceRequestView> views)
        {
            var parts = new List<string>();
            for (int i = 0; i < views.Count; i++)
            {
                parts.Add(RequestReference.Display(views[i].Report.RequestId) + " " +
                          views[i].Report.Category + " (" + RequestStatuses.Canonical(views[i].Report.Status) + ")");
            }

            return string.Join(", ", parts);
        }

        private static string JoinRequests(IList<string> ids)
        {
            var parts = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                parts.Add(RequestReference.Display(ids[i]));
            }

            return string.Join(" → ", parts);
        }

        private static string JoinPath(IList<int> path)
        {
            if (path == null || path.Count == 0)
            {
                return "the tree was empty";
            }

            var parts = new List<string>(path.Count);
            for (int i = 0; i < path.Count; i++)
            {
                parts.Add("#" + path[i].ToString());
            }

            return string.Join(" → ", parts);
        }

        private static bool Matches(ServiceRequestView view, string needle)
        {
            return Contains(view.Report.RequestId, needle) ||
                   Contains(RequestReference.Display(view.Report.RequestId), needle) ||
                   Contains(view.Report.Category, needle) ||
                   Contains(view.Report.Location, needle) ||
                   Contains(view.Report.Description, needle) ||
                   Contains(view.Area, needle) ||
                   Contains(view.Report.Status, needle);
        }

        private static bool Contains(string source, string needle)
        {
            return !string.IsNullOrEmpty(source) &&
                   source.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
