using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// One shared desk for City of Jozi staff. Residents do not register.
    /// The desk stays open only for this run of the application.
    /// </summary>
    public static class MunicipalDesk
    {
        public const string SettingKey = "MunicipalDeskCode";

        private static bool isOpen;

        public static bool IsOpen
        {
            get { return isOpen; }
        }

        public static bool TryOpen(IWin32Window owner)
        {
            if (isOpen)
            {
                return true;
            }

            using (var dialog = new Forms.MunicipalDeskForm())
            {
                if (dialog.ShowDialog(owner) != DialogResult.OK)
                {
                    return false;
                }
            }

            isOpen = true;
            return true;
        }

        public static bool Matches(string entered)
        {
            return Canonical(entered) != null;
        }

        public static string Canonical(string entered)
        {
            string typed = (entered ?? string.Empty).Trim();
            if (typed.Length == 0)
            {
                return null;
            }

            IList<string> codes = Codes();
            for (int i = 0; i < codes.Count; i++)
            {
                if (string.Equals(codes[i], typed, StringComparison.OrdinalIgnoreCase))
                {
                    return codes[i];
                }
            }

            return null;
        }

        public static DeskScope ScopeFor(string code)
        {
            string canonical = Canonical(code);
            if (canonical == null)
            {
                return null;
            }

            if (string.Equals(canonical, "SANDTON-01", StringComparison.OrdinalIgnoreCase))
            {
                return new DeskScope(canonical, "Sandton", "Sandton", "Rivonia", "Grayston");
            }

            if (string.Equals(canonical, "SOWETO-02", StringComparison.OrdinalIgnoreCase))
            {
                return new DeskScope(canonical, "Soweto", "Soweto", "Vilakazi", "Orlando");
            }

            if (string.Equals(canonical, "CBD-03", StringComparison.OrdinalIgnoreCase))
            {
                return new DeskScope(canonical, "Johannesburg CBD", "Johannesburg CBD", "Joubert", "Braamfontein", "Hillbrow", "Constitution Hill", "Mary Fitzgerald");
            }

            return new DeskScope(
                canonical,
                "Jozi",
                "Jozi",
                "Rosebank",
                "Alexandra",
                "Roodepoort",
                "Midrand",
                "Tembisa",
                "Kempton Park",
                "Kempton",
                "Randburg",
                "Parkview",
                "Emmarentia",
                "Zoo Lake",
                "Delta Park",
                "Botanical",
                "Ivory Park");
        }

        public static IList<string> Codes()
        {
            var codes = new List<string>();
            AddCodes(codes, ConfigurationManager.AppSettings["MunicipalDeskCodes"]);
            AddCodes(codes, ConfigurationManager.AppSettings[SettingKey]);
            if (codes.Count == 0)
            {
                codes.Add("JOZI-DESK");
                codes.Add("SANDTON-01");
                codes.Add("SOWETO-02");
                codes.Add("CBD-03");
            }

            return codes;
        }

        private static void AddCodes(List<string> codes, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return;
            }

            string[] parts = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                string code = parts[i].Trim();
                if (code.Length == 0)
                {
                    continue;
                }

                bool already = false;
                for (int n = 0; n < codes.Count; n++)
                {
                    if (string.Equals(codes[n], code, StringComparison.OrdinalIgnoreCase))
                    {
                        already = true;
                        break;
                    }
                }

                if (!already)
                {
                    codes.Add(code);
                }
            }
        }
    }

    /// <summary>
    /// The area a desk code is allowed to see. JOZI-DESK is the rest of the city.
    /// Sandton, Soweto, and the CBD each have their own code.
    /// </summary>
    public sealed class DeskScope
    {
        private readonly string[] places;

        public DeskScope(string code, string label, params string[] places)
        {
            Code = code;
            Label = label;
            this.places = places ?? new string[0];
        }

        public string Code { get; private set; }

        public string Label { get; private set; }

        public bool Includes(string area, string location)
        {
            string text = ((area ?? string.Empty) + " " + (location ?? string.Empty)).Trim();
            if (text.Length == 0)
            {
                return false;
            }

            if (IsJoziDesk() && BelongsToSpecialistDesk(text))
            {
                return false;
            }

            for (int i = 0; i < places.Length; i++)
            {
                if (text.IndexOf(places[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            if (IsJoziDesk() &&
                text.IndexOf("Johannesburg", StringComparison.OrdinalIgnoreCase) >= 0 &&
                text.IndexOf("CBD", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return true;
            }

            return false;
        }

        private bool IsJoziDesk()
        {
            return string.Equals(Code, "JOZI-DESK", StringComparison.OrdinalIgnoreCase);
        }

        private static bool BelongsToSpecialistDesk(string text)
        {
            return Contains(text, "Sandton") ||
                   Contains(text, "Rivonia") ||
                   Contains(text, "Grayston") ||
                   Contains(text, "Soweto") ||
                   Contains(text, "Vilakazi") ||
                   Contains(text, "Orlando") ||
                   Contains(text, "Johannesburg CBD") ||
                   Contains(text, "Joubert") ||
                   Contains(text, "Braamfontein") ||
                   Contains(text, "Hillbrow") ||
                   Contains(text, "Constitution Hill") ||
                   Contains(text, "Mary Fitzgerald");
        }

        private static bool Contains(string text, string place)
        {
            return text.IndexOf(place, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
