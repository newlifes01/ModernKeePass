﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ModernKeePass.ViewModels;

namespace ModernKeePass.Views
{
    public partial class SettingsPage10
    {
        private SettingsVm10 ViewModel => (SettingsVm10)DataContext;

        private readonly IList<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("welcome", typeof(SettingsWelcomePage)),
            ("new", typeof(SettingsNewDatabasePage)),
            ("save", typeof(SettingsSavePage)),
            ("general", typeof(SettingsDatabasePage)),
            ("security", typeof(SettingsSecurityPage))
        };

        public SettingsPage10()
        {
            InitializeComponent();
        }

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // Getting the Tag from Content (args.InvokedItem is the content of NavigationViewItem)
            var navItem = NavigationView.MenuItems
                .OfType<NavigationViewItem>()
                .First(i => args.InvokedItem.Equals(i.Content));

            NavigationView_Navigate(navItem);
        }

        private void NavigationView_Navigate(NavigationViewItem navItem)
        {
            var item = _pages.First(p => p.Tag.Equals(navItem.Tag));
            NavigationView.Header = navItem.Content;
            ContentFrame.Navigate(item.Page);
        }

        private void NavigationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationView_Navigate(Welcome);
            NavigationView.IsBackEnabled = Frame.CanGoBack;
        }

        private void NavigationView_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            Frame.GoBack();
        }
    }
}