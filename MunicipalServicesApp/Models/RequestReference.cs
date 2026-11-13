using System;

namespace MunicipalServicesApp.Models
{
    public static class RequestReference
    {
        public static bool TryParseNumber(string input, out int number)
        {
            number = 0;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string text = input.Trim();
            if (text.StartsWith("#", StringComparison.Ordinal))
            {
                text = text.Substring(1).Trim();
            }

            int dash = text.LastIndexOf('-');
            string tail = dash >= 0 ? text.Substring(dash + 1).Trim() : text;
            if (!int.TryParse(tail, out number) || number <= 0)
            {
                number = 0;
                return false;
            }

            return true;
        }

        public static string Display(string requestId)
        {
            int number;
            if (!TryParseNumber(requestId, out number))
            {
                return string.IsNullOrWhiteSpace(requestId) ? "#" : "#" + requestId;
            }

            return "#" + number.ToString();
        }
    }
}
