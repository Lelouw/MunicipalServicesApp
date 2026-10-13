using System;

namespace MunicipalServicesApp.Models
{
    public sealed class LocalEvent
    {
        public const int PriorityLow = 1;
        public const int PriorityNormal = 2;
        public const int PriorityImportant = 3;
        public const int PriorityHigh = 4;
        public const int PriorityUrgent = 5;

        public string Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public bool IsAnnouncement { get; set; }

        public int Priority { get; set; }

        public string KindLabel
        {
            get { return IsAnnouncement ? "Announcement" : "Event"; }
        }

        public string PriorityLabel
        {
            get
            {
                if (Priority >= PriorityUrgent)
                {
                    return "Urgent";
                }

                if (Priority >= PriorityHigh)
                {
                    return "High";
                }

                if (Priority >= PriorityImportant)
                {
                    return "Important";
                }

                if (Priority >= PriorityNormal)
                {
                    return "Notice";
                }

                return "Standard";
            }
        }
    }
}
