using Typedown.Core.Enums;
using Windows.UI.Xaml;

namespace Typedown.Core.Controls.EditorControls.MenuBarItems
{
    public sealed partial class ThemeItem : MenuBarItemBase
    {
        public ThemeItem()
        {
            InitializeComponent();
        }

        protected override void OnRegisterShortcut()
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCheckedItems();
        }

        private void OnAppThemeItemClick(object sender, RoutedEventArgs e)
        {
            if (sender == UseSystemThemeItem)
                Settings.AppTheme = AppTheme.Default;
            else if (sender == LightThemeItem)
                Settings.AppTheme = AppTheme.Light;
            else if (sender == DarkThemeItem)
                Settings.AppTheme = AppTheme.Dark;

            UpdateCheckedItems();
        }

        private void OnThemeItemClick(object sender, RoutedEventArgs e)
        {
            if (sender == GitHubItem)
                Settings.DocumentTheme = DocumentTheme.GitHub;
            else if (sender == MinimalItem)
                Settings.DocumentTheme = DocumentTheme.Minimal;
            else if (sender == PaperItem)
                Settings.DocumentTheme = DocumentTheme.Paper;

            UpdateCheckedItems();
        }

        private void UpdateCheckedItems()
        {
            if (Settings == null)
                return;

            UseSystemThemeItem.IsChecked = Settings.AppTheme == AppTheme.Default;
            LightThemeItem.IsChecked = Settings.AppTheme == AppTheme.Light;
            DarkThemeItem.IsChecked = Settings.AppTheme == AppTheme.Dark;
            GitHubItem.IsChecked = Settings.DocumentTheme == DocumentTheme.GitHub;
            MinimalItem.IsChecked = Settings.DocumentTheme == DocumentTheme.Minimal;
            PaperItem.IsChecked = Settings.DocumentTheme == DocumentTheme.Paper;
        }
    }
}
