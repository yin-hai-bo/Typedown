using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Typedown.Core.Models.RuntimeModels;

namespace Typedown.Core.Services
{
    public class CustomDocumentStyleService
    {
        private static readonly string[] BuiltInStyleNames = new[] { "github", "minimal", "paper" };

        public ObservableCollection<CustomDocumentStyle> CustomStyles { get; } = new();

        public string CustomStylesDirectory { get; } = Path.Combine(Config.GetLocalFolderPath(), "Custom-Styles");

        public CustomDocumentStyleService()
        {
            Reload();
        }

        public void Reload()
        {
            EnsureDirectory();
            var items = Directory.GetFiles(CustomStylesDirectory, "*.css", SearchOption.TopDirectoryOnly)
                .Select(path =>
                {
                    var name = Path.GetFileNameWithoutExtension(path);
                    return new CustomDocumentStyle() { Key = name, Name = name, Path = path };
                })
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            CustomStyles.Clear();
            items.ForEach(CustomStyles.Add);
        }

        public AddCustomDocumentStyleResult Add(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return AddCustomDocumentStyleResult.InvalidFile;

            if (!string.Equals(Path.GetExtension(sourcePath), ".css", StringComparison.OrdinalIgnoreCase))
                return AddCustomDocumentStyleResult.InvalidFile;

            var name = Path.GetFileNameWithoutExtension(sourcePath)?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return AddCustomDocumentStyleResult.InvalidName;

            if (BuiltInStyleNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                return AddCustomDocumentStyleResult.ReservedName;

            if (CustomStyles.Any(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return AddCustomDocumentStyleResult.Duplicate;

            EnsureDirectory();
            File.Copy(sourcePath, Path.Combine(CustomStylesDirectory, $"{name}.css"), false);
            Reload();
            return AddCustomDocumentStyleResult.Success;
        }

        public bool Remove(CustomDocumentStyle style)
        {
            if (style == null)
                return false;

            var target = CustomStyles.FirstOrDefault(x => x.Key.Equals(style.Key, StringComparison.OrdinalIgnoreCase));
            if (target == null || !File.Exists(target.Path))
                return false;

            File.Delete(target.Path);
            Reload();
            return true;
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(CustomStylesDirectory))
                Directory.CreateDirectory(CustomStylesDirectory);
        }
    }

    public enum AddCustomDocumentStyleResult
    {
        Success = 0,
        InvalidFile = 1,
        InvalidName = 2,
        Duplicate = 3,
        ReservedName = 4,
    }
}
