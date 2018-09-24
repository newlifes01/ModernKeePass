using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;
using ModernKeePassLib;
using GroupItem = ModernKeePass.ViewModels.GroupItem;
using TreeView = Microsoft.UI.Xaml.Controls.TreeView;
using TreeViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs;

namespace ModernKeePass.Views
{
    public partial class GroupsPage
    {
        public GroupsVm ViewModel { get; set; }

        public GroupsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is PwGroup group)) group = DatabaseService.Instance.RootGroup10;
            ViewModel = new GroupsVm(group);
        }

        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void NavigationTree_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is GroupItem group)
            {
                ViewModel.Title = group.Text;
                SplitViewFrame.Navigate(typeof(EntriesPage), group);
            }
        }
        
        private void RenameFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyout) ((GroupItem)flyout.DataContext).IsEditMode = true;
        }

        private async void DeleteFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyout)
            {
                var resource = new ResourcesService();
                var database = DatabaseService.Instance;
                var item = (GroupItem) flyout.DataContext;

                var deleteFileDialog = new ContentDialog
                {
                    Title = $"{resource.GetResourceValue("EntityDeleteActionButton")} {item.Text} ?",
                    Content = database.IsRecycleBinEnabled
                        ? resource.GetResourceValue("GroupRecyclingConfirmation")
                        : resource.GetResourceValue("GroupDeletingConfirmation"),
                    PrimaryButtonText = resource.GetResourceValue("EntityDeleteActionButton"),
                    CloseButtonText = resource.GetResourceValue("EntityDeleteCancelButton")
                };

                var result = await deleteFileDialog.ShowAsync();

                // Delete the file if the user clicked the primary button.
                // Otherwise, do nothing.
                if (result == ContentDialogResult.Primary)
                {
                    item.Parent.Children.Remove(item);
                    // TODO: refresh treeview
                    if (database.IsRecycleBinEnabled) database.RecyleBinGroup.AddGroup(item.Group, true);
                }
            }
        }

        private void GroupNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) GroupNameTextBox_OnLostFocus(sender, null);
        }

        private void GroupNameTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox) ((GroupItem)textBox.DataContext).IsEditMode = false;
        }

        private void NewGroupNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                var text = ((TextBox)sender).Text;
                if (string.IsNullOrEmpty(text)) return;
                ViewModel.AddNewGroup(text);
                AddButton.IsEnabled = true;
                NewGroupNameTextBox.Visibility = Visibility.Collapsed;
                NewGroupNameTextBox.Text = string.Empty;
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                sender.ItemsSource = ViewModel.RootItem.SubEntries.Where(e => e.Name.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
        }
    }
}