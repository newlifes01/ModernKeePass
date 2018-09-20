using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;
using ModernKeePassLib;

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

        private void AddButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewGroup();
        }
    }
}