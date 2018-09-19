using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;
using ModernKeePassLib;

namespace ModernKeePass.Views
{
    public partial class GroupsPage
    {
        private GroupsVm ViewModel => (GroupsVm) DataContext;

        public GroupsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is PwGroup group)) group = DatabaseService.Instance.RootGroup10;
            DataContext = new GroupsVm(group);
        }
        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (HamburgerButton.IsChecked != null)
                VisualStateManager.GoToState(this, (bool)HamburgerButton.IsChecked ? "Expanded" : "Minimal", true);
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void AddButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}