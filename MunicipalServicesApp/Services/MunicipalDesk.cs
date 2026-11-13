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
            string typed = (entered ?? string.Empty).Trim();
            if (typed.Length == 0)
            {
                return false;
            }

            IList<string> codes = Codes();
            for (int i = 0; i < codes.Count; i++)
            {
                if (string.Equals(codes[i], typed, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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
}
