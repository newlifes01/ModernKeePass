using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;
using ModernKeePassLib;
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
            ViewModel.Title = ((NavigationMenuGroup) args.InvokedItem).Text;
        }
        
        private void RenameFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyout) ((NavigationMenuGroup)flyout.DataContext).IsEditMode = true;
        }

        private async void DeleteFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyout)
            {
                var resource = new ResourcesService();
                var database = DatabaseService.Instance;
                var item = (NavigationMenuGroup) flyout.DataContext;

                var deleteFileDialog = new ContentDialog
                {
                    Title = $"{resource.GetResourceValue("EntityDeleteActionButton")} {item.Text} ?",
                    Content = database.IsRecycleBinEnabled
                        ? resource.GetResourceValue("EntryRecyclingConfirmation")
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
            if (sender is TextBox textBox) ((NavigationMenuGroup)textBox.DataContext).IsEditMode = false;
        }

        private void NewGroupNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                NewGroupNameTextBox_OnLostFocus(sender, null);
            }
        }

        private void NewGroupNameTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewGroup(((TextBox)sender).Text);
            AddButton.IsEnabled = true;
            NewGroupNameTextBox.Visibility = Visibility.Collapsed;
        }

    }
}