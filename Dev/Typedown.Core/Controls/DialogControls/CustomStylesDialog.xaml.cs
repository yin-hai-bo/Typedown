using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Typedown.Core.Models.RuntimeModels;
using Typedown.Core.Services;
using Typedown.Core.Utilities;
using Typedown.Core.ViewModels;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Typedown.Core.Controls.DialogControls
{
    public sealed partial class CustomStylesDialog : AppContentDialog
    {
        private AppViewModel ViewModel { get; set; }

        private CustomDocumentStyleService CustomStyleService => ViewModel?.ServiceProvider.GetService<CustomDocumentStyleService>();

        public ObservableCollection<CustomDocumentStyle> CustomStyles => CustomStyleService?.CustomStyles;

        public CustomStylesDialog()
        {
            InitializeComponent();
        }

        public static async Task OpenAsync(AppViewModel viewModel)
        {
            var dialog = new CustomStylesDialog()
            {
                XamlRoot = viewModel.XamlRoot,
                ViewModel = viewModel
            };
            dialog.CustomStyleService?.Reload();
            dialog.Bindings.Update();
            await dialog.ShowAsync();
        }

        private async void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".css");
                picker.SetOwnerWindow(ViewModel.MainWindow);
                var file = await picker.PickSingleFileAsync();
                if (file == null)
                    return;

                var result = CustomStyleService.Add(file.Path);
                if (result == AddCustomDocumentStyleResult.Success)
                    return;

                var msg = result switch
                {
                    AddCustomDocumentStyleResult.Duplicate => Locale.GetString("CustomStylesDialog.Duplicate"),
                    AddCustomDocumentStyleResult.ReservedName => Locale.GetString("CustomStylesDialog.ReservedName"),
                    AddCustomDocumentStyleResult.InvalidName => Locale.GetString("NameCannotBeEmpty"),
                    _ => Locale.GetString("CustomStylesDialog.InvalidFile"),
                };
                await AppContentDialog.Create(Locale.GetString("Error"), msg, Locale.GetString("Ok")).ShowAsync(XamlRoot);
            }
            catch (IOException ex)
            {
                await AppContentDialog.Create(Locale.GetString("Error"), ex.Message, Locale.GetString("Ok")).ShowAsync(XamlRoot);
            }
        }

        private async void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is not CustomDocumentStyle style)
                return;

            var result = await AppContentDialog.Create(
                Locale.GetString("CustomStylesDialog.DeleteTitle"),
                string.Format(Locale.GetString("CustomStylesDialog.DeleteContent"), style.Name),
                Locale.GetString("Cancel"),
                Locale.GetString("Delete"),
                null).ShowAsync(XamlRoot);
            if (result != ContentDialogResult.Primary)
                return;

            CustomStyleService.Remove(style);
        }

        private async void OnOpenButtonClick(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is not CustomDocumentStyle style)
                return;

            try
            {
                Common.OpenUrl(style.Path);
            }
            catch (Exception ex)
            {
                await AppContentDialog.Create(Locale.GetString("Error"), ex.Message, Locale.GetString("Ok")).ShowAsync(XamlRoot);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Bindings?.StopTracking();
        }
    }
}
