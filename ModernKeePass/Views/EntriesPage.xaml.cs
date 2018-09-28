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

        public EntriesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is GroupItem group) ViewModel = new EntriesVm(group);
            VisualStateManager.GoToState(this, Unselected.Name, false);
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
                var text = ((TextBox) sender).Text;
                if (string.IsNullOrEmpty(text)) return;
                ViewModel.AddNewEntry(text);
                AddEntryButton.IsEnabled = true;
                NewEntryNameTextBox.Visibility = Visibility.Collapsed;
                NewEntryNameTextBox.Text = string.Empty;
            }
        }
        
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var destinationFrame = EntrySelectedGroup.CurrentState == SelectedExpanded ? ContentFrame : Frame;
            destinationFrame.Navigate(typeof(EntryPage), e.AddedItems[0]);
        }

        private void VisualStateGroup_OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}