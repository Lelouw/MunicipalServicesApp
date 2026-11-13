using System;
using System.Collections.Generic;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public sealed class EventRepository
    {
        private const int MaxRecentSearches = 20;
        private const int MaxRecentlyViewed = 6;
        private const int MinimumKeywordLength = 3;

        private static readonly EventRepository InstanceValue = new EventRepository();
        private static readonly HashSet<string> StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "and", "for", "with", "from", "your", "this", "that", "city", "jozi",
            "event", "events", "near", "area", "into", "over", "what"
        };

        // Hash table: event id -> event, so opening a card or a recent item is a direct lookup.
        private readonly Dictionary<string, LocalEvent> eventsById =
            new Dictionary<string, LocalEvent>(StringComparer.OrdinalIgnoreCase);

        // Sorted dictionary: date -> events that day. A date-range search can stop at the end date.
        private readonly SortedDictionary<DateTime, List<LocalEvent>> eventsByDate =
            new SortedDictionary<DateTime, List<LocalEvent>>();

        // Set of unique categories. The category filter is built from this set.
        private readonly HashSet<string> categories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Set of unique start dates, used to tell the resident when a chosen day has nothing on.
        private readonly HashSet<DateTime> eventDates = new HashSet<DateTime>();

        // Queue: announcements in the order they were issued (first in, first listed).
        private readonly Queue<LocalEvent> announcementQueue = new Queue<LocalEvent>();

        // Stack: the event opened most recently sits on top.
        private readonly Stack<string> recentlyViewedIds = new Stack<string>();

        // Priority queue: the same announcements, but urgent ones come out first.
        private readonly EventPriorityQueue priorityAnnouncements = new EventPriorityQueue();

        // Hash table of how often each category was searched.
        private readonly Dictionary<string, int> categorySearchCounts =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Hash table of how often each keyword was searched.
        private readonly Dictionary<string, int> keywordSearchCounts =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Queue of searches, oldest at the front, so patterns can be read in order.
        private readonly Queue<EventSearch> recentSearches = new Queue<EventSearch>();

        private EventRepository()
        {
            SeedEvents();
        }

        public static EventRepository Instance
        {
            get { return InstanceValue; }
        }

        public int CategoryCount
        {
            get { return categories.Count; }
        }

        public int DateCount
        {
            get { return eventDates.Count; }
        }

        public int RecentSearchCount
        {
            get { return recentSearches.Count; }
        }

        public IReadOnlyDictionary<string, int> CategorySearchCounts
        {
            get { return categorySearchCounts; }
        }

        public IReadOnlyDictionary<string, int> KeywordSearchCounts
        {
            get { return keywordSearchCounts; }
        }

        public bool ContainsDate(DateTime day)
        {
            return eventDates.Contains(day.Date);
        }

        public IList<string> GetCategories()
        {
            var list = new List<string>(categories);
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list;
        }

        public LocalEvent FindById(string id)
        {
            LocalEvent item;
            if (!string.IsNullOrWhiteSpace(id) && eventsById.TryGetValue(id, out item))
            {
                return item;
            }

            return null;
        }

        public IList<LocalEvent> GetPriorityAnnouncements()
        {
            return priorityAnnouncements.ToPriorityOrder();
        }

        public IList<LocalEvent> GetAnnouncementQueue()
        {
            return new List<LocalEvent>(announcementQueue.ToArray());
        }

        public IList<EventSearch> GetRecentSearches()
        {
            return new List<EventSearch>(recentSearches.ToArray());
        }

        public IList<LocalEvent> Search(string keyword, string category, DateTime? fromDate, DateTime? toDate)
        {
            string term = keyword == null ? string.Empty : keyword.Trim();
            bool filterCategory = !string.IsNullOrWhiteSpace(category);
            DateTime start = fromDate.HasValue ? fromDate.Value.Date : DateTime.MinValue.Date;
            DateTime end = toDate.HasValue ? toDate.Value.Date : DateTime.MaxValue.Date;

            var matches = new List<LocalEvent>();
            foreach (KeyValuePair<DateTime, List<LocalEvent>> day in eventsByDate)
            {
                if (day.Key < start)
                {
                    continue;
                }

                if (day.Key > end)
                {
                    break;
                }

                for (int i = 0; i < day.Value.Count; i++)
                {
                    LocalEvent item = day.Value[i];
                    if (filterCategory &&
                        !string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (term.Length > 0 && !MatchesKeyword(item, term))
                    {
                        continue;
                    }

                    matches.Add(item);
                }
            }

            return matches;
        }

        public void RecordSearch(string keyword, string category, DateTime? fromDate, DateTime? toDate)
        {
            string term = keyword == null ? string.Empty : keyword.Trim();
            bool hasCategory = !string.IsNullOrWhiteSpace(category);
            bool hasKeyword = term.Length >= MinimumKeywordLength;
            bool hasDate = fromDate.HasValue || toDate.HasValue;

            if (!hasCategory && !hasKeyword && !hasDate)
            {
                return;
            }

            if (hasCategory)
            {
                Increment(categorySearchCounts, category.Trim());
            }

            if (hasKeyword)
            {
                RecordKeywords(term);
            }

            recentSearches.Enqueue(new EventSearch
            {
                Keyword = hasKeyword ? term : string.Empty,
                Category = hasCategory ? category.Trim() : string.Empty,
                FromDate = fromDate.HasValue ? fromDate.Value.Date : (DateTime?)null,
                ToDate = toDate.HasValue ? toDate.Value.Date : (DateTime?)null,
                SearchedAt = DateTime.Now
            });

            while (recentSearches.Count > MaxRecentSearches)
            {
                recentSearches.Dequeue();
            }
        }

        public void RememberViewed(string eventId)
        {
            if (FindById(eventId) == null)
            {
                return;
            }

            var existing = new List<string>(recentlyViewedIds);
            existing.RemoveAll(id => string.Equals(id, eventId, StringComparison.OrdinalIgnoreCase));
            existing.Insert(0, eventId);

            if (existing.Count > MaxRecentlyViewed)
            {
                existing.RemoveRange(MaxRecentlyViewed, existing.Count - MaxRecentlyViewed);
            }

            recentlyViewedIds.Clear();
            for (int i = existing.Count - 1; i >= 0; i--)
            {
                recentlyViewedIds.Push(existing[i]);
            }
        }

        public IList<LocalEvent> GetRecentlyViewed()
        {
            string[] ids = recentlyViewedIds.ToArray();
            var items = new List<LocalEvent>(ids.Length);
            for (int i = 0; i < ids.Length; i++)
            {
                LocalEvent item = FindById(ids[i]);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private static bool MatchesKeyword(LocalEvent item, string term)
        {
            return Contains(item.Title, term) ||
                   Contains(item.Category, term) ||
                   Contains(item.Location, term) ||
                   Contains(item.Description, term);
        }

        private static bool Contains(string value, string term)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RecordKeywords(string term)
        {
            string[] words = term.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int recorded = 0;
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i].Trim();
                if (word.Length < MinimumKeywordLength || StopWords.Contains(word))
                {
                    continue;
                }

                Increment(keywordSearchCounts, word);
                recorded++;
            }

            if (recorded == 0)
            {
                Increment(keywordSearchCounts, term);
            }
        }

        private static void Increment(Dictionary<string, int> counts, string key)
        {
            int current;
            counts.TryGetValue(key, out current);
            counts[key] = current + 1;
        }

        private static int CompareEvents(LocalEvent left, LocalEvent right)
        {
            int byPriority = right.Priority.CompareTo(left.Priority);
            if (byPriority != 0)
            {
                return byPriority;
            }

            return string.Compare(left.Title, right.Title, StringComparison.OrdinalIgnoreCase);
        }

        public LocalEvent Publish(LocalEvent item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrWhiteSpace(item.Title))
            {
                throw new ArgumentException("An announcement needs a title.", nameof(item));
            }

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = NextEventId();
            }

            if (string.IsNullOrWhiteSpace(item.Category))
            {
                item.Category = "Other";
            }

            if (item.StartDate == default(DateTime))
            {
                item.StartDate = DateTime.Today;
            }

            if (item.Priority < LocalEvent.PriorityLow)
            {
                item.Priority = LocalEvent.PriorityNormal;
            }

            Add(item);
            return item;
        }

        private string NextEventId()
        {
            int highest = 0;
            foreach (string id in eventsById.Keys)
            {
                int number;
                if (RequestReference.TryParseNumber(id, out number) && number > highest)
                {
                    highest = number;
                }
            }

            return "EVT-" + DateTime.Now.Year + "-" + (highest + 1).ToString("0000");
        }

        private void Add(LocalEvent item)
        {
            eventsById.Add(item.Id, item);
            categories.Add(item.Category);

            DateTime day = item.StartDate.Date;
            eventDates.Add(day);

            List<LocalEvent> dayEvents;
            if (!eventsByDate.TryGetValue(day, out dayEvents))
            {
                dayEvents = new List<LocalEvent>();
                eventsByDate.Add(day, dayEvents);
            }

            dayEvents.Add(item);
            dayEvents.Sort(CompareEvents);

            if (item.IsAnnouncement)
            {
                announcementQueue.Enqueue(item);
                priorityAnnouncements.Enqueue(item);
            }
        }

        private void SeedEvents()
        {
            Add(new LocalEvent
            {
                Id = "EVT-2026-0001",
                Title = "Load reduction notice",
                Category = "Utilities",
                Location = "Johannesburg CBD",
                Description = "City Power will reduce load in the CBD on Monday evening. Charge essential devices and switch off geysers before 16:00.",
                StartDate = new DateTime(2026, 10, 13),
                IsAnnouncement = true,
                Priority = LocalEvent.PriorityHigh
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0002",
                Title = "Water interruption in Soweto",
                Category = "Water",
                Location = "Soweto",
                Description = "Planned work on the Soweto reservoir will interrupt supply from 08:00 to 18:00. Store drinking water the night before.",
                StartDate = new DateTime(2026, 10, 14),
                IsAnnouncement = true,
                Priority = LocalEvent.PriorityUrgent
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0003",
                Title = "Spring clean-up at Emmarentia",
                Category = "Parks and Recreation",
                Location = "Emmarentia Dam",
                Description = "Residents cleared the dam path and picnic lawns. Thank you to everyone who brought gloves and bags.",
                StartDate = new DateTime(2026, 10, 3),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityLow
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0004",
                Title = "Heritage walk at Constitution Hill",
                Category = "Arts and Culture",
                Location = "Constitution Hill",
                Description = "A guided walk through the Old Fort already took place. Photos from the morning are with the arts office.",
                StartDate = new DateTime(2026, 10, 4),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityLow
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0005",
                Title = "Street light repair blitz",
                Category = "Utilities",
                Location = "Hillbrow",
                Description = "Teams will replace dark street lights along Kotze and Claim streets. Report any pole that is still out after Friday.",
                StartDate = new DateTime(2026, 10, 16),
                IsAnnouncement = true,
                Priority = LocalEvent.PriorityImportant
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0006",
                Title = "Community clean-up in Alexandra",
                Category = "Waste and Environment",
                Location = "Alexandra",
                Description = "Bring gloves and join the ward clean-up along the Jukskei. Bags and a water point will be at the 3rd Avenue pickup.",
                StartDate = new DateTime(2026, 10, 17),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityNormal
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0007",
                Title = "M1 Grayston overnight closure",
                Category = "Roads and Transport",
                Location = "M1 Grayston interchange",
                Description = "The southbound M1 at Grayston closes from 21:00 for resurfacing. Use the Oxford Road detour and allow extra time.",
                StartDate = new DateTime(2026, 10, 18),
                IsAnnouncement = true,
                Priority = LocalEvent.PriorityImportant
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0008",
                Title = "Sandton Library extended hours",
                Category = "Community",
                Location = "Sandton Library",
                Description = "The library stays open until 20:00 on weekdays this month for exam season. Study desks are first come, first served.",
                StartDate = new DateTime(2026, 10, 20),
                IsAnnouncement = true,
                Priority = LocalEvent.PriorityLow
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0009",
                Title = "Safety imbizo in Tembisa",
                Category = "Safety",
                Location = "Tembisa civic centre",
                Description = "Meet the ward safety team, hear the new patrol roster, and raise lighting or taxi-rank concerns in person.",
                StartDate = new DateTime(2026, 10, 22),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityImportant
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0010",
                Title = "Jazz in the Park",
                Category = "Arts and Culture",
                Location = "Zoo Lake, Parkview",
                Description = "A free Sunday concert on the Zoo Lake lawn. Bring a blanket. Food stalls open from 11:00 and the band starts at 14:00.",
                StartDate = new DateTime(2026, 10, 25),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityLow
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0011",
                Title = "Water-wise workshop",
                Category = "Water",
                Location = "Kempton Park civic centre",
                Description = "Learn to find leaks, read a meter, and cut outdoor watering. The session is free and seats are limited to 80 residents.",
                StartDate = new DateTime(2026, 10, 29),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityNormal
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0012",
                Title = "Rates payment reminder",
                Category = "Community",
                Location = "City of Jozi",
                Description = "October municipal accounts are due on the last business day of the month. Pay at a walk-in centre or on the city portal.",
                StartDate = new DateTime(2026, 10, 31),
                IsAnnouncement = true,
                Priority = LocalEvent.PriorityNormal
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0013",
                Title = "Youth career expo",
                Category = "Youth",
                Location = "Braamfontein",
                Description = "Employers, colleges, and artisan programmes will be at the Braamfontein recreational centre from 09:00. Bring a copy of your ID.",
                StartDate = new DateTime(2026, 11, 1),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityNormal
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0014",
                Title = "Neighbourhood watch training",
                Category = "Safety",
                Location = "Roodepoort",
                Description = "A practical evening on radio checks, street patrols, and when to call the metro police. Open to any Roodepoort block.",
                StartDate = new DateTime(2026, 11, 5),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityNormal
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0015",
                Title = "Delta Park fun run",
                Category = "Parks and Recreation",
                Location = "Delta Park",
                Description = "A 5 km family run on the park loop. Registration at 07:00, start at 07:30. Dogs on short leads are welcome after the start.",
                StartDate = new DateTime(2026, 11, 8),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityLow
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0016",
                Title = "Waste drop-off day",
                Category = "Waste and Environment",
                Location = "Randburg",
                Description = "Bring e-waste, garden waste, and recyclables to the Randburg depot. Builders' rubble is not accepted on this day.",
                StartDate = new DateTime(2026, 11, 15),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityLow
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0017",
                Title = "Heritage concert",
                Category = "Arts and Culture",
                Location = "Mary Fitzgerald Square",
                Description = "An evening of choral and jazz sets in Newtown. Gates open at 17:00. Seating is on the square, so bring a jersey.",
                StartDate = new DateTime(2026, 12, 12),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityNormal
            });

            Add(new LocalEvent
            {
                Id = "EVT-2026-0018",
                Title = "Kids holiday programme",
                Category = "Parks and Recreation",
                Location = "Johannesburg Botanical Gardens",
                Description = "A daytime programme of garden trails, storytelling, and games for children aged 6 to 12. Caregivers stay on site.",
                StartDate = new DateTime(2026, 12, 14),
                IsAnnouncement = false,
                Priority = LocalEvent.PriorityLow
            });
        }
    }
}
