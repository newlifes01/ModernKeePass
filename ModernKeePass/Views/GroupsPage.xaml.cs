using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.ViewModels;
using ModernKeePassLib;

namespace ModernKeePass.Views
{
    public partial class GroupsPage
    {
        private GroupsVm ViewModel => (GroupsVm)DataContext;

        public GroupsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = new GroupsVm(e.Parameter as PwGroup);
        }

        private void NavigationTree_OnLoading(FrameworkElement sender, object args)
        {
            foreach (var item in ViewModel.MainMenu)
            {
                NavigationTree.RootNodes.Add(item.AsTreeViewNode());
            }
        }
    }
}