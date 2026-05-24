using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.ApplicationModel;
using Windows.Storage;

namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit { }
}

namespace Typedown.Core
{
    public static class Config
    {
        private const string DefaultAppId = "Typedown-cs";

        public static bool IsMicaSupported { get; } = Environment.OSVersion.Version.Build >= 22000;

        public static IReadOnlyList<string> WebView2Args { get; } = new List<string>()
        {
            "--disable-web-security",
            "--allow-file-access-from-files",
            "--flag-switches-begin",
            "--enable-features=msOverlayScrollbarWinStyle",
            "--flag-switches-end"
        };

        public static JsonSerializerSettings EditorJsonSerializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(true, true)
            },
            MaxDepth = 256
        };

        public static string GetLocalFolderPath()
        {
            try
            {
                return ApplicationData.Current.LocalFolder.Path;
            }
            catch (Exception)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppId);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string GetSettingsFilePath()
        {
            return Path.Combine(GetLocalFolderPath(), "Settings.json");
        }

        public static string GetSavedLanguageSetting()
        {
            try
            {
                var settingsFile = GetSettingsFilePath();
                if (!File.Exists(settingsFile))
                    return Utilities.AppLanguage.DefaultSetting;

                var settings = JObject.Parse(File.ReadAllText(settingsFile, Encoding.UTF8));
                return settings["Language"]?.ToObject<string>() ?? Utilities.AppLanguage.DefaultSetting;
            }
            catch
            {
                return Utilities.AppLanguage.DefaultSetting;
            }
        }

        public static string AppDisplayName { get; } = ResolveAppDisplayName();

        public static string AppId { get; } = ResolveAppId();

        public static bool IsPackaged { get; private set; }

        static Config()
        {
            try
            {
                IsPackaged = Package.Current != null;
            }
            catch
            {
                IsPackaged = false;
            }
        }

        private static string ResolveAppDisplayName()
        {
            try
            {
                var displayName = Package.Current?.DisplayName;
                if (!string.IsNullOrWhiteSpace(displayName))
                    return displayName;
            }
            catch
            {
                // Ignore and fall back to the source-defined app name.
            }

            return DefaultAppId;
        }

        private static string ResolveAppId()
        {
            try
            {
                var packageName = Package.Current?.Id?.Name;
                if (!string.IsNullOrWhiteSpace(packageName))
                    return packageName;
            }
            catch
            {
                // Ignore and fall back to the source-defined app id.
            }

            return DefaultAppId;
        }
    }
}
