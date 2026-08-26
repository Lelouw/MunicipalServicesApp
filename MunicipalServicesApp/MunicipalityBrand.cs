namespace MunicipalServicesApp
{
    public static class MunicipalityBrand
    {
        public const string Name = "City of Jozi";

        public const string Slogan = "Your city. Your voice. Better service for every Joburger.";

        public const string AppShortName = "Jozi Municipal Services";

        public const string WelcomeMessage =
            "Welcome to City of Jozi. Choose a service below, or from the Services menu.";

        public const string ReportSubtitle =
            "Tell City of Jozi where the problem is and what you observed.";

        public const string CommunityReportsHint =
            "All issues reported by residents across City of Jozi. Check here for updates on incidents around us.";

        public static string WindowTitle(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                return AppShortName;
            }

            return AppShortName + " — " + section;
        }
    }
}
