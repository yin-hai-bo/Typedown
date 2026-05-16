using System;
using System.Collections.Generic;
using System.Linq;

namespace Typedown.Core.Enums
{
    public enum DocumentTheme
    {
        GitHub = 0,

        Minimal = 1,

        Paper = 2,
    }

    public static partial class Enumerable
    {
        public static IReadOnlyList<DocumentTheme> DocumentThemes { get; } = Enum.GetValues(typeof(DocumentTheme)).Cast<DocumentTheme>().ToList();
    }
}
