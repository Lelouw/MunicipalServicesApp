using System;

namespace MunicipalServicesApp.Models
{
    public struct RequestDateKey : IComparable<RequestDateKey>
    {
        public RequestDateKey(DateTime reportedAt, int number)
        {
            ReportedAt = reportedAt;
            Number = number;
        }

        public DateTime ReportedAt { get; }

        public int Number { get; }

        public int CompareTo(RequestDateKey other)
        {
            int byDate = ReportedAt.CompareTo(other.ReportedAt);
            if (byDate != 0)
            {
                return byDate;
            }

            return Number.CompareTo(other.Number);
        }
    }

    public struct RequestStatusKey : IComparable<RequestStatusKey>
    {
        public RequestStatusKey(int rank, int number)
        {
            Rank = rank;
            Number = number;
        }

        public int Rank { get; }

        public int Number { get; }

        public int CompareTo(RequestStatusKey other)
        {
            int byRank = Rank.CompareTo(other.Rank);
            if (byRank != 0)
            {
                return byRank;
            }

            return Number.CompareTo(other.Number);
        }
    }
}
