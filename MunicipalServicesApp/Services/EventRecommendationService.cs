using System;
using System.Collections.Generic;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public sealed class EventRecommendationService
    {
        private const int SameCategoryWeight = 12;
        private const int RelatedCategoryWeight = 6;
        private const int AnchorCategoryWeight = 5;
        private const int TitleKeywordWeight = 8;
        private const int DetailKeywordWeight = 3;
        private const int NearbyDateWeight = 3;
        private const int NearbyDayWindow = 21;
        private const int MaxPatternLabels = 2;

        private static readonly Dictionary<string, string[]> RelatedCategories =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "Arts and Culture", new[] { "Community", "Parks and Recreation", "Youth" } },
                { "Community", new[] { "Arts and Culture", "Safety", "Youth" } },
                { "Parks and Recreation", new[] { "Arts and Culture", "Community", "Youth", "Waste and Environment" } },
                { "Roads and Transport", new[] { "Safety", "Utilities" } },
                { "Safety", new[] { "Community", "Roads and Transport", "Utilities" } },
                { "Utilities", new[] { "Roads and Transport", "Safety", "Water" } },
                { "Waste and Environment", new[] { "Parks and Recreation", "Community", "Water" } },
                { "Water", new[] { "Utilities", "Waste and Environment", "Safety" } },
                { "Youth", new[] { "Community", "Arts and Culture", "Parks and Recreation" } }
            };

        private readonly EventRepository repository;

        public EventRecommendationService(EventRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        public string DescribeSearchPatterns()
        {
            if (repository.RecentSearchCount == 0)
            {
                return "Search by a category, a date, or a keyword. City of Jozi will suggest events that match what you look for.";
            }

            string categories = JoinTop(repository.CategorySearchCounts);
            string keywords = JoinTop(repository.KeywordSearchCounts);

            if (categories.Length > 0 && keywords.Length > 0)
            {
                return "Your searches favour " + categories + ". Keywords: " + keywords + ".";
            }

            if (categories.Length > 0)
            {
                return "Your searches favour " + categories + ".";
            }

            if (keywords.Length > 0)
            {
                return "Your searches mention " + keywords + ".";
            }

            return "Your searches are based on dates. Recommendations favour events close to those dates.";
        }

        public IList<RecommendedEvent> Recommend(IList<LocalEvent> currentResults, int maxResults)
        {
            var recommendations = new List<RecommendedEvent>();
            if (repository.RecentSearchCount == 0 || maxResults <= 0)
            {
                return recommendations;
            }

            IList<EventSearch> searches = repository.GetRecentSearches();
            HashSet<string> shownIds = BuildIdSet(currentResults);
            HashSet<string> anchorCategories = BuildAnchorCategories(currentResults);
            IList<LocalEvent> pool = repository.Search(string.Empty, string.Empty, null, null);

            var scored = ScoreEvents(pool, searches, anchorCategories, includePast: UserSearchedThePast(searches));
            scored.Sort(CompareScored);

            AppendRecommendations(recommendations, scored, shownIds, maxResults);
            if (recommendations.Count == 0)
            {
                AppendRecommendations(recommendations, scored, null, maxResults);
            }

            return recommendations;
        }

        private List<ScoredEvent> ScoreEvents(
            IList<LocalEvent> pool,
            IList<EventSearch> searches,
            HashSet<string> anchorCategories,
            bool includePast)
        {
            var scored = new List<ScoredEvent>();
            DateTime today = DateTime.Today;

            for (int i = 0; i < pool.Count; i++)
            {
                LocalEvent item = pool[i];
                if (!includePast && item.StartDate.Date < today)
                {
                    continue;
                }

                string reason;
                int score = ScoreEvent(item, searches, anchorCategories, out reason);
                if (score <= 0)
                {
                    continue;
                }

                scored.Add(new ScoredEvent
                {
                    Event = item,
                    Score = score,
                    Reason = reason
                });
            }

            return scored;
        }

        private int ScoreEvent(
            LocalEvent item,
            IList<EventSearch> searches,
            HashSet<string> anchorCategories,
            out string reason)
        {
            int score = 0;
            string bestReason = string.Empty;
            int bestWeight = 0;

            int categoryCount;
            if (!string.IsNullOrEmpty(item.Category) &&
                repository.CategorySearchCounts.TryGetValue(item.Category, out categoryCount) &&
                categoryCount > 0)
            {
                int weight = categoryCount * SameCategoryWeight;
                score += weight;
                ConsiderReason(ref bestReason, ref bestWeight, weight, "You searched " + item.Category);
            }

            score += ScoreRelatedCategories(item, ref bestReason, ref bestWeight);
            score += ScoreAnchorCategory(item, anchorCategories, ref bestReason, ref bestWeight);
            score += ScoreDiscoveredCategories(item, anchorCategories, ref bestReason, ref bestWeight);
            score += ScoreKeywords(item, ref bestReason, ref bestWeight);

            if (IsNearSearchedDates(item, searches))
            {
                score += NearbyDateWeight;
                ConsiderReason(ref bestReason, ref bestWeight, NearbyDateWeight, "Close to the dates you searched");
            }

            reason = bestReason;
            return score;
        }

        private int ScoreRelatedCategories(LocalEvent item, ref string bestReason, ref int bestWeight)
        {
            int score = 0;
            foreach (KeyValuePair<string, int> searched in repository.CategorySearchCounts)
            {
                if (searched.Value <= 0 || string.Equals(searched.Key, item.Category, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!IsRelated(searched.Key, item.Category))
                {
                    continue;
                }

                int weight = searched.Value * RelatedCategoryWeight;
                score += weight;
                ConsiderReason(ref bestReason, ref bestWeight, weight, "Related to " + searched.Key);
            }

            return score;
        }

        private static int ScoreAnchorCategory(
            LocalEvent item,
            HashSet<string> anchorCategories,
            ref string bestReason,
            ref int bestWeight)
        {
            if (anchorCategories == null || string.IsNullOrEmpty(item.Category) || !anchorCategories.Contains(item.Category))
            {
                return 0;
            }

            ConsiderReason(ref bestReason, ref bestWeight, AnchorCategoryWeight, "More like the " + item.Category + " events you found");
            return AnchorCategoryWeight;
        }

        private int ScoreDiscoveredCategories(
            LocalEvent item,
            HashSet<string> anchorCategories,
            ref string bestReason,
            ref int bestWeight)
        {
            if (anchorCategories == null || string.IsNullOrEmpty(item.Category))
            {
                return 0;
            }

            int score = 0;
            foreach (string anchor in anchorCategories)
            {
                int searchedCount;
                if (repository.CategorySearchCounts.TryGetValue(anchor, out searchedCount) && searchedCount > 0)
                {
                    continue;
                }

                if (!IsRelated(anchor, item.Category))
                {
                    continue;
                }

                score += RelatedCategoryWeight;
                ConsiderReason(ref bestReason, ref bestWeight, RelatedCategoryWeight, "Related to " + anchor);
            }

            return score;
        }

        private int ScoreKeywords(LocalEvent item, ref string bestReason, ref int bestWeight)
        {
            int score = 0;
            foreach (KeyValuePair<string, int> keyword in repository.KeywordSearchCounts)
            {
                if (keyword.Value <= 0 || string.IsNullOrWhiteSpace(keyword.Key))
                {
                    continue;
                }

                if (ContainsText(item.Title, keyword.Key))
                {
                    int weight = keyword.Value * TitleKeywordWeight;
                    score += weight;
                    ConsiderReason(ref bestReason, ref bestWeight, weight, "Matches \"" + keyword.Key + "\"");
                    continue;
                }

                if (ContainsText(item.Location, keyword.Key) ||
                    ContainsText(item.Description, keyword.Key) ||
                    ContainsText(item.Category, keyword.Key))
                {
                    int weight = keyword.Value * DetailKeywordWeight;
                    score += weight;
                    ConsiderReason(ref bestReason, ref bestWeight, weight, "Matches \"" + keyword.Key + "\"");
                }
            }

            return score;
        }

        private static void AppendRecommendations(
            List<RecommendedEvent> target,
            List<ScoredEvent> scored,
            HashSet<string> excludeIds,
            int maxResults)
        {
            for (int i = 0; i < scored.Count && target.Count < maxResults; i++)
            {
                ScoredEvent candidate = scored[i];
                if (excludeIds != null && excludeIds.Contains(candidate.Event.Id))
                {
                    continue;
                }

                target.Add(new RecommendedEvent
                {
                    Event = candidate.Event,
                    Reason = candidate.Reason,
                    Score = candidate.Score
                });
            }
        }

        private static HashSet<string> BuildIdSet(IList<LocalEvent> events)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (events == null)
            {
                return ids;
            }

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i] != null && !string.IsNullOrEmpty(events[i].Id))
                {
                    ids.Add(events[i].Id);
                }
            }

            return ids;
        }

        private static HashSet<string> BuildAnchorCategories(IList<LocalEvent> events)
        {
            var anchors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (events == null)
            {
                return anchors;
            }

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i] != null && !string.IsNullOrWhiteSpace(events[i].Category))
                {
                    anchors.Add(events[i].Category);
                }
            }

            return anchors;
        }

        private static bool UserSearchedThePast(IList<EventSearch> searches)
        {
            DateTime today = DateTime.Today;
            for (int i = 0; i < searches.Count; i++)
            {
                if (searches[i].FromDate.HasValue && searches[i].FromDate.Value.Date < today)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNearSearchedDates(LocalEvent item, IList<EventSearch> searches)
        {
            DateTime day = item.StartDate.Date;
            for (int i = 0; i < searches.Count; i++)
            {
                EventSearch search = searches[i];
                if (!search.FromDate.HasValue && !search.ToDate.HasValue)
                {
                    continue;
                }

                DateTime start = search.FromDate.HasValue ? search.FromDate.Value.Date : search.ToDate.Value.Date;
                DateTime end = search.ToDate.HasValue ? search.ToDate.Value.Date : search.FromDate.Value.Date;
                if (end < start)
                {
                    DateTime swap = start;
                    start = end;
                    end = swap;
                }

                bool inside = day >= start && day <= end;
                bool nearby = day >= start.AddDays(-NearbyDayWindow) && day <= end.AddDays(NearbyDayWindow);
                if (nearby && !inside)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsRelated(string searchedCategory, string eventCategory)
        {
            string[] related;
            if (!RelatedCategories.TryGetValue(searchedCategory, out related))
            {
                return false;
            }

            for (int i = 0; i < related.Length; i++)
            {
                if (string.Equals(related[i], eventCategory, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsText(string value, string term)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void ConsiderReason(ref string bestReason, ref int bestWeight, int weight, string reason)
        {
            if (weight > bestWeight)
            {
                bestWeight = weight;
                bestReason = reason;
            }
        }

        private static int CompareScored(ScoredEvent left, ScoredEvent right)
        {
            int byScore = right.Score.CompareTo(left.Score);
            if (byScore != 0)
            {
                return byScore;
            }

            return left.Event.StartDate.CompareTo(right.Event.StartDate);
        }

        private static string JoinTop(IReadOnlyDictionary<string, int> counts)
        {
            var ranked = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<string, int> pair in counts)
            {
                if (pair.Value > 0 && !string.IsNullOrWhiteSpace(pair.Key))
                {
                    ranked.Add(pair);
                }
            }

            ranked.Sort(delegate(KeyValuePair<string, int> left, KeyValuePair<string, int> right)
            {
                int byCount = right.Value.CompareTo(left.Value);
                if (byCount != 0)
                {
                    return byCount;
                }

                return string.Compare(left.Key, right.Key, StringComparison.OrdinalIgnoreCase);
            });

            int take = Math.Min(MaxPatternLabels, ranked.Count);
            if (take == 0)
            {
                return string.Empty;
            }

            if (take == 1)
            {
                return ranked[0].Key;
            }

            return ranked[0].Key + " and " + ranked[1].Key;
        }

        private sealed class ScoredEvent
        {
            public LocalEvent Event { get; set; }

            public int Score { get; set; }

            public string Reason { get; set; }
        }
    }
}
