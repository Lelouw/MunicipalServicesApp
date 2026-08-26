using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public sealed class NominatimLocationSuggestionService : ILocationSuggestionService
    {
        private static readonly object HttpClientSync = new object();
        private static HttpClient httpClient;

        private readonly LocationSearchSettings settings;

        public NominatimLocationSuggestionService(LocationSearchSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        private HttpClient HttpClient
        {
            get
            {
                if (httpClient != null)
                {
                    return httpClient;
                }

                lock (HttpClientSync)
                {
                    if (httpClient == null)
                    {
                        httpClient = new HttpClient
                        {
                            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds)
                        };
                    }

                    return httpClient;
                }
            }
        }

        public async Task<IReadOnlyList<LocationSuggestion>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < settings.MinimumQueryLength)
            {
                return Array.Empty<LocationSuggestion>();
            }

            string url = BuildSearchUrl(query.Trim());
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", settings.UserAgent);
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("Accept-Language", "en-ZA,en");

                using (HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return Array.Empty<LocationSuggestion>();
                    }

                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return ParsePlaces(json);
                }
            }
        }

        private string BuildSearchUrl(string query)
        {
            string separator = settings.SearchUrl.IndexOf('?') >= 0 ? "&" : "?";
            return settings.SearchUrl
                + separator
                + "format=json"
                + "&addressdetails=1"
                + "&limit=" + settings.ResultLimit.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + "&countrycodes=" + Uri.EscapeDataString(settings.CountryCodes)
                + "&q=" + Uri.EscapeDataString(query);
        }

        private static IReadOnlyList<LocationSuggestion> ParsePlaces(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return Array.Empty<LocationSuggestion>();
            }

            var serializer = new JavaScriptSerializer();
            var places = serializer.Deserialize<List<NominatimPlace>>(json);
            if (places == null || places.Count == 0)
            {
                return Array.Empty<LocationSuggestion>();
            }

            var results = new List<LocationSuggestion>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (NominatimPlace place in places)
            {
                string displayName = FormatPlace(place);
                if (string.IsNullOrWhiteSpace(displayName) || !seen.Add(displayName))
                {
                    continue;
                }

                results.Add(new LocationSuggestion
                {
                    DisplayName = displayName,
                    Latitude = place.lat,
                    Longitude = place.lon
                });
            }

            return results;
        }

        private static string FormatPlace(NominatimPlace place)
        {
            if (place == null)
            {
                return string.Empty;
            }

            NominatimAddress address = place.address;
            if (address != null)
            {
                string formatted = JoinDistinct(
                    FirstNonEmpty(address.road, address.pedestrian, address.residential),
                    FirstNonEmpty(address.suburb, address.neighbourhood, address.quarter),
                    FirstNonEmpty(address.city, address.town, address.village, address.municipality),
                    address.state);

                if (!string.IsNullOrWhiteSpace(formatted))
                {
                    return formatted;
                }
            }

            return place.display_name ?? string.Empty;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values == null)
            {
                return string.Empty;
            }

            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }

        private static string JoinDistinct(params string[] parts)
        {
            var unique = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string part in parts)
            {
                if (string.IsNullOrWhiteSpace(part) || !seen.Add(part.Trim()))
                {
                    continue;
                }

                unique.Add(part.Trim());
            }

            return string.Join(", ", unique.ToArray());
        }

        private sealed class NominatimPlace
        {
            public string display_name { get; set; }

            public string lat { get; set; }

            public string lon { get; set; }

            public NominatimAddress address { get; set; }
        }

        private sealed class NominatimAddress
        {
            public string road { get; set; }

            public string pedestrian { get; set; }

            public string residential { get; set; }

            public string suburb { get; set; }

            public string neighbourhood { get; set; }

            public string quarter { get; set; }

            public string city { get; set; }

            public string town { get; set; }

            public string village { get; set; }

            public string municipality { get; set; }

            public string state { get; set; }
        }
    }
}
