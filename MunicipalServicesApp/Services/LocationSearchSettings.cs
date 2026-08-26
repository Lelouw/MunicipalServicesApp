using System;
using System.Configuration;
using System.Globalization;

namespace MunicipalServicesApp.Services
{
    public sealed class LocationSearchSettings
    {
        public string SearchUrl { get; private set; }

        public string CountryCodes { get; private set; }

        public string UserAgent { get; private set; }

        public int ResultLimit { get; private set; }

        public int DebounceMilliseconds { get; private set; }

        public int MinimumQueryLength { get; private set; }

        public int TimeoutSeconds { get; private set; }

        public static LocationSearchSettings Load()
        {
            return new LocationSearchSettings
            {
                SearchUrl = Read("LocationSearchUrl", "https://nominatim.openstreetmap.org/search"),
                CountryCodes = Read("LocationSearchCountryCodes", "za"),
                UserAgent = Read(
                    "LocationSearchUserAgent",
                    "MunicipalServicesApp/1.0 (AAPD7112W POE; municipal issue reporting)"),
                ResultLimit = ReadInt("LocationSearchLimit", 8, 1, 15),
                DebounceMilliseconds = ReadInt("LocationSearchDebounceMs", 450, 250, 1500),
                MinimumQueryLength = ReadInt("LocationSearchMinQueryLength", 3, 2, 8),
                TimeoutSeconds = ReadInt("LocationSearchTimeoutSeconds", 8, 3, 20)
            };
        }

        private static string Read(string key, string fallback)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static int ReadInt(string key, int fallback, int min, int max)
        {
            int parsed;
            if (!int.TryParse(ConfigurationManager.AppSettings[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
            {
                parsed = fallback;
            }

            if (parsed < min)
            {
                return min;
            }

            if (parsed > max)
            {
                return max;
            }

            return parsed;
        }
    }
}
