using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;
using GroupItem = ModernKeePass.ViewModels.GroupItem;

// TODO: check https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/XamlListView/cs/Samples/MasterDetailSelection
namespace ModernKeePass.Views
{
    public partial class EntriesPage
    {
        public EntriesVm ViewModel { get; set; }

        public EntriesPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is GroupItem group) ViewModel = new EntriesVm(group);
        }

        private async void DeleteFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyout)
            {
                var resource = new ResourcesService();
                var database = DatabaseService.Instance;
                var item = (EntryItem)flyout.DataContext;

                var deleteFileDialog = new ContentDialog
                {
                    Title = $"{resource.GetResourceValue("EntityDeleteActionButton")} {item.Name} ?",
                    Content = database.IsRecycleBinEnabled
                        ? resource.GetResourceValue("EntryRecyclingConfirmation")
                        : resource.GetResourceValue("EntryDeletingConfirmation"),
                    PrimaryButtonText = resource.GetResourceValue("EntityDeleteActionButton"),
                    CloseButtonText = resource.GetResourceValue("EntityDeleteCancelButton")
                };

                var result = await deleteFileDialog.ShowAsync();

                // Delete the file if the user clicked the primary button.
                // Otherwise, do nothing.
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.Entries.Remove(item);
                    // TODO: refresh treeview
                    if (database.IsRecycleBinEnabled) database.RecyleBinGroup.AddEntry(item.Entry, true);
                }
            }
        }

        private void NewEntryNameTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                var text = NewEntryNameTextBox.Text;
                if (string.IsNullOrEmpty(text)) return;
                ViewModel.AddNewEntry(text);
                AddEntryButton.IsChecked = false;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                AddEntryButton.IsChecked = false;
            }
        }

        private void ColorPickerBackground_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ColorPicker colorPicker) (colorPicker.DataContext as EntryItem).BackgroundColor = colorPicker.Color;
        }
        private void ColorPickerForeground_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ColorPicker colorPicker) (colorPicker.DataContext as EntryItem).ForegroundColor = colorPicker.Color;
        }

        private void HistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}