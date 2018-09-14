using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.Services;
using ModernKeePass.ViewModels;

namespace ModernKeePass.Views
{
    public partial class MainPage10
    {
        private readonly IList<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
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
            ContentFrame.Navigate(typeof(WelcomePage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var file = e.Parameter as StorageFile;
            DataContext = new MainVm10(file);
            if (file != null) ContentFrame.Navigate(typeof(OpenDatabasePage), file);
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

                NavView_Navigate(navItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem navItem)
        {
            var item = _pages.First(p => p.Tag.Equals(navItem.Tag));
            if (item.Tag == "database")
            {
                Frame.Navigate(typeof(GroupDetailPage), DatabaseService.Instance.RootGroup);
            }
            else
            {
                NavigationView.Header = navItem.Content;
                ContentFrame.Navigate(item.Page);
            }
        }
    }
}