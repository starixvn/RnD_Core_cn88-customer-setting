namespace CN88.CustomerSetting.Services
{
    public static class LanguageNameConverter
    {
        private const string DefaultLanguageName = "en";
        private const string DefaultLanguageKey = "E";

        public static string GetRefLanguageName(string languageName = DefaultLanguageName)
        {
            if (string.IsNullOrWhiteSpace(languageName))
            {
                return DefaultLanguageName;
            }

            switch (languageName.ToUpperInvariant())
            {
                case "ZH-TW": return "ch"; // Traditional
                case "ZH-CN": return "cs"; // Simplified
                case "JA-JP": return "jp"; // Japanese
                case "TH-TH": return "th"; // Thai
                case "KO-KR": return "ko"; // Korean
                case "VI-VN": return "vn"; // Vietnamese
                default: return DefaultLanguageName;
            }
        }

        public static string GetLanguageKey(string languageName = DefaultLanguageKey)
        {
            if (string.IsNullOrWhiteSpace(languageName))
            {
                return DefaultLanguageName;
            }

            switch (languageName.ToUpperInvariant())
            {
                case "ZH-TW": return "CT"; // Traditional
                case "ZH-CN": return "CS"; // Simplified
                case "JA-JP": return "J"; // Japanese
                case "TH-TH": return "TH"; // Thai
                case "KO-KR": return "KO"; // Korean
                case "VI-VN": return "VN"; // Vietnamese
                default: return DefaultLanguageKey;
            }
        }
    }
}
