using System;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace Typedown.Core.Utilities
{
    public static class AppLanguage
    {
        public const string DefaultSetting = "default";

        private const string English = "en-US";

        private const string SimplifiedChinese = "zh-Hans";

        public static string StartupLanguage { get; private set; } = English;

        public static string ResolveEffectiveLanguage(string settingLanguage)
        {
            if (string.IsNullOrWhiteSpace(settingLanguage))
                settingLanguage = DefaultSetting;

            if (Locale.IsSupportedLanguage(settingLanguage))
                return settingLanguage;

            return ResolveSupportedSystemLanguage();
        }

        public static string GetCurrentStartupLanguage()
        {
            return StartupLanguage;
        }

        public static void ApplyLanguageAtStartup(string settingLanguage)
        {
            var effectiveLanguage = ResolveEffectiveLanguage(settingLanguage);
            StartupLanguage = effectiveLanguage;
            Locale.SetCurrentLanguage(effectiveLanguage);

            try
            {
                ResourceContext.SetGlobalQualifierValue("Language", effectiveLanguage);
            }
            catch
            {
                // Ignore
            }

            if (Config.IsPackaged)
            {
                try
                {
                    ApplicationLanguages.PrimaryLanguageOverride = effectiveLanguage;
                }
                catch
                {
                    // Ignore
                }
            }
        }

        private static string ResolveSupportedSystemLanguage()
        {
            var systemLanguage = TryGetSystemLanguage();
            if (Locale.IsSupportedLanguage(systemLanguage))
                return systemLanguage;

            return systemLanguage?.StartsWith("zh", StringComparison.OrdinalIgnoreCase) == true ? SimplifiedChinese : English;
        }

        private static string TryGetSystemLanguage()
        {
            try
            {
                var systemLanguage = GlobalizationPreferences.Languages?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(systemLanguage))
                    return systemLanguage;
            }
            catch
            {
                // Ignore
            }

            try
            {
                return CultureInfo.CurrentUICulture.Name;
            }
            catch
            {
                return English;
            }
        }
    }
}
