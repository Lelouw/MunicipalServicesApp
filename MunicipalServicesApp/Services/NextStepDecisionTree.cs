using System.Collections.Generic;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public sealed class DecisionStep
    {
        public string Prompt { get; set; }

        public string Detail { get; set; }

        public System.Func<DecisionFacts, bool> Test { get; set; }

        public bool IsLeaf
        {
            get { return Test == null; }
        }
    }

    public sealed class DecisionFacts
    {
        public string Status { get; set; }

        public string Category { get; set; }

        public int AgeDays { get; set; }

        public bool IsOpen { get; set; }

        public bool Urgent { get; set; }
    }

    public sealed class DecisionOutcome
    {
        public string Title { get; set; }

        public string Detail { get; set; }

        public IList<string> Path { get; set; }
    }

    /// <summary>
    /// Binary tree of yes/no questions. Left is yes, right is no.
    /// The heap decides which open request is most urgent. This tree decides
    /// what the municipality should do next with the one request on screen.
    /// </summary>
    public static class NextStepDecisionTree
    {
        public static BinaryTreeNode<DecisionStep> Create()
        {
            BinaryTreeNode<DecisionStep> root = Question(
                "Is this request still open?",
                facts => facts.IsOpen);

            root.Right = Leaf(
                "No further municipal action",
                "Resolved and closed requests stay on record so the reference number still works.");

            root.Left = Question(
                "Is a crew already on the job?",
                facts => facts.Status == RequestStatuses.InProgress);

            root.Left.Left = Leaf(
                "Wait for the crew to finish, then record the result",
                "The request is in progress. The next useful update is that the visit has ended.");

            root.Left.Right = Question(
                "Is this an urgent category?",
                facts => facts.Urgent);

            root.Left.Right.Left = Question(
                "Has it been open for more than 3 days?",
                facts => facts.AgeDays > 3);

            root.Left.Right.Left.Left = Leaf(
                "Escalate to the depot supervisor today",
                "Urgent work that is still not on site after 3 days is pushed to the front of the depot's day.");

            root.Left.Right.Left.Right = Leaf(
                "Dispatch the on-call crew",
                "Urgent work reported in the last 3 days goes to the on-call crew before it ages.");

            root.Left.Right.Right = Question(
                "Is it still waiting to be acknowledged?",
                facts => facts.Status == RequestStatuses.Pending);

            root.Left.Right.Right.Left = Leaf(
                "Acknowledge the request and assign a depot",
                "The report is in. The next step is to accept it and name the depot that will do the work.");

            root.Left.Right.Right.Right = Leaf(
                "Move the request to the next status",
                "The request has been accepted. Record the next update so the resident can see movement.");

            return root;
        }

        public static DecisionOutcome Evaluate(BinaryTreeNode<DecisionStep> root, DecisionFacts facts)
        {
            var path = new List<string>();
            BinaryTreeNode<DecisionStep> node = root;
            while (node != null && !node.Value.IsLeaf)
            {
                bool yes = node.Value.Test(facts);
                path.Add(node.Value.Prompt + (yes ? " Yes." : " No."));
                node = yes ? node.Left : node.Right;
            }

            if (node == null)
            {
                return new DecisionOutcome
                {
                    Title = "Review the request manually",
                    Detail = "The decision tree has no answer for this combination.",
                    Path = path
                };
            }

            return new DecisionOutcome
            {
                Title = node.Value.Prompt,
                Detail = node.Value.Detail,
                Path = path
            };
        }

        private static BinaryTreeNode<DecisionStep> Question(string prompt, System.Func<DecisionFacts, bool> test)
        {
            return new BinaryTreeNode<DecisionStep>(new DecisionStep
            {
                Prompt = prompt,
                Test = test
            });
        }

        private static BinaryTreeNode<DecisionStep> Leaf(string title, string detail)
        {
            return new BinaryTreeNode<DecisionStep>(new DecisionStep
            {
                Prompt = title,
                Detail = detail
            });
        }
    }
}
