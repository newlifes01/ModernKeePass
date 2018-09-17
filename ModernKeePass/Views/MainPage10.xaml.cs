using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;

namespace ModernKeePass.Views
{
    public partial class MainPage10
    {
        private MainVm10 ViewModel => (MainVm10)DataContext;

        private readonly IList<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("welcome", typeof(WelcomePage)),
            ("open", typeof(OpenDatabasePage)),
            ("new", typeof(NewDatabasePage)),
            ("save", typeof(SaveDatabasePage)),
            ("recent", typeof(RecentDatabasesPage)),
            ("about", typeof(AboutPage)),
            ("donate", typeof(DonatePage)),
            ("database", typeof(GroupDetailPage))
        };

        public MainPage10()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.File = e.Parameter as StorageFile;
        }

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
                Frame.Navigate(typeof(SettingsPage));
            else
            {
                // Getting the Tag from Content (args.InvokedItem is the content of NavigationViewItem)
                var navItem = NavigationView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(i => args.InvokedItem.Equals(i.Content));

                NavigationView_Navigate(navItem);
            }
        }

        private void NavigationView_Navigate(NavigationViewItem navItem, object parameter = null)
        {
            var item = _pages.First(p => p.Tag.Equals(navItem.Tag));
            if (item.Tag == "database")
            {
                Frame.Navigate(typeof(GroupDetailPage), DatabaseService.Instance.RootGroup);
            }
            else
            {
                NavigationView.Header = navItem.Content;
                ContentFrame.Navigate(item.Page, parameter);
            }
        }

        private void NavigationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsDatabaseOpened) NavigationView.SelectedItem = Save;
            else if (ViewModel.File != null) NavigationView.SelectedItem = Open;
            else if (ViewModel.HasRecentItems) NavigationView.SelectedItem = Recent;
            else NavigationView.SelectedItem = Welcome;
            NavigationView_Navigate((NavigationViewItem) NavigationView.SelectedItem, ViewModel.File);
        }
    }
}