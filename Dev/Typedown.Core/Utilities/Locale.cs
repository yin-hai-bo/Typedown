using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Markup;

namespace Typedown.Core.Utilities
{
    public static class Locale
    {
        public enum ResourceSource
        {
            All,
            CommonResources,
            DialogResources,
            SettingsResources,
            Resources
        }

        private static readonly string assemblyName = typeof(Locale).Assembly.GetName().Name;

        public static IReadOnlyDictionary<ResourceSource, ResourceMap> ResourcesDictionary = new Dictionary<ResourceSource, ResourceMap>()
        {
            {ResourceSource.CommonResources,  ResourceManager.Current.MainResourceMap.GetSubtree($"{assemblyName}/" + nameof(ResourceSource.CommonResources))},
            {ResourceSource.DialogResources,  ResourceManager.Current.MainResourceMap.GetSubtree($"{assemblyName}/" + nameof(ResourceSource.DialogResources))},
            {ResourceSource.SettingsResources,  ResourceManager.Current.MainResourceMap.GetSubtree($"{assemblyName}/" + nameof(ResourceSource.SettingsResources))},
            {ResourceSource.Resources,  ResourceManager.Current.MainResourceMap.GetSubtree($"{assemblyName}/" + nameof(ResourceSource.Resources))}
        };

        public static Dictionary<string, string> SupportedLangs { get; } = new()
        {
            {"en","English"},
            {"zh-Hans","中文 (简体)"},
        };

        public static Dictionary<string, string> LangsOptions { get; } = new(SupportedLangs.Append(new("default", GetString("UseSystemSetting"))));

        public static string GetLangOptionDisplayName(string key) => LangsOptions[key];

        public static ResourceContext ResourceContext { get; } = new();

        public static string GetString(string key, ResourceSource source = 0)
        {
            key = key.Replace('.', '/');
            if (source == 0 || !ResourcesDictionary.ContainsKey(source))
                return ResourcesDictionary.Values.Select(x => x.GetValue(key)?.ValueAsString).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
            return ResourcesDictionary[source].GetValue(key, ResourceContext)?.ValueAsString;
        }

        public static string GetDialogString(string key)
        {
            return GetString(key, ResourceSource.DialogResources);
        }

        public static string GetTypeString(Type type)
        {
            return (type.GetCustomAttribute(typeof(LocaleAttribute)) as LocaleAttribute)?.Text;
        }
    }

    public class LocaleAttribute : Attribute
    {
        public string[] Keys { get; }

        public string Text => Texts.FirstOrDefault();

        public IEnumerable<string> Texts => Keys.Select(x => Locale.GetString(x));

        public LocaleAttribute(params string[] keys)
        {
            Keys = keys;
        }
    }

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class LocaleString : MarkupExtension
    {
        public string Key { get; set; }

        public Locale.ResourceSource Source { get; set; }

        protected override object ProvideValue()
        {
            return Locale.GetString(Key, Source);
        }
    }
}
